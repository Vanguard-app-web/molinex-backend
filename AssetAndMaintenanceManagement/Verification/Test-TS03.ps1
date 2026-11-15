#Requires -Version 7.0
[CmdletBinding()]
param(
    [Parameter(Mandatory)][string]$Token,
    [string]$BaseUri = 'http://localhost:5080',
    [string]$ReportPath
)
$ErrorActionPreference = 'Stop'
$BaseUri = $BaseUri.TrimEnd('/')
$checks = [System.Collections.Generic.List[object]]::new()
function Assert-That([bool]$Condition, [string]$Name) {
    if (-not $Condition) { throw "FAILED: $Name" }
    $checks.Add([pscustomobject]@{ check = $Name; passed = $true })
    Write-Host "PASS: $Name"
}
function Send-Request([string]$Method, [string]$Path, [int]$ExpectedStatus, $Body = $null, [bool]$Authenticated = $true, [string]$ErrorCode = '') {
    $parameters = @{
        Method = $Method; Uri = "$BaseUri$Path"; SkipHttpErrorCheck = $true
        Headers = @{}; TimeoutSec = 30
    }
    if ($Authenticated) { $parameters.Headers.Authorization = "Bearer $Token" }
    if ($null -ne $Body) {
        $parameters.Body = $Body | ConvertTo-Json -Depth 10
        $parameters.ContentType = 'application/json'
    }
    $response = Invoke-WebRequest @parameters
    Assert-That ($response.StatusCode -eq $ExpectedStatus) "$Method $Path returns $ExpectedStatus (actual $($response.StatusCode))"
    $content = if ($response.Content -is [byte[]]) { [Text.Encoding]::UTF8.GetString($response.Content) } else { [string]$response.Content }
    $value = $content | ConvertFrom-Json -Depth 20
    if ($ErrorCode) {
        Assert-That ($value.code -eq $ErrorCode) "Problem Details code: $ErrorCode (actual $($value.code))"
        Assert-That (($response.Headers.'Content-Type' -join '; ') -like '*application/problem+json*') 'Error uses application/problem+json'
    }
    return [pscustomobject]@{ Value = $value; Headers = $response.Headers }
}
$null = Send-Request GET '/api/v1/machines' 401 $null $false 'Shared.Unauthorized'
$code = 'TS03-' + [guid]::NewGuid().ToString('N').Substring(0, 12).ToUpperInvariant()
$machineBody = @{ code = " $($code.ToLowerInvariant()) "; name = 'TS-03 rice mill'; model = 'MX-100'; status = 'OPERATIONAL'; statusChangedAt = '2026-01-01T08:00:00Z' }
$created = Send-Request POST '/api/v1/machines' 201 $machineBody
$machine = $created.Value
Assert-That ($machine.code -eq $code) 'Machine code is trimmed and normalized'
Assert-That (-not [string]::IsNullOrWhiteSpace(($created.Headers.Location -join ''))) 'Machine response includes Location'
$null = Send-Request GET "/api/v1/machines/$($machine.id)" 200
$status = (Send-Request GET "/api/v1/machines/$($machine.id)/status" 200).Value
Assert-That ($status.status -eq 'OPERATIONAL' -and -not $status.pending) 'Current machine status is available'
$emptyHistory = (Send-Request GET "/api/v1/maintenance-records/by-machine/$($machine.id)" 200).Value
Assert-That (@($emptyHistory).Count -eq 0) 'New machine has an empty history'
$null = Send-Request POST '/api/v1/machines' 409 $machineBody $true 'Maintenance.DuplicateMachineCode'
$invalidMachine = $machineBody.Clone(); $invalidMachine.code = "$code-X"; $invalidMachine.status = 'INVALID'
$null = Send-Request POST '/api/v1/machines' 400 $invalidMachine $true 'Maintenance.InvalidMachineStatus'
$invalidMachine.status = 'OPERATIONAL'; $invalidMachine.statusChangedAt = $null
$null = Send-Request POST '/api/v1/machines' 400 $invalidMachine $true 'Maintenance.StatusChangeDateRequired'
$otherBody = $machineBody.Clone(); $otherBody.code = "$code-OTHER"
$other = (Send-Request POST '/api/v1/machines' 201 $otherBody).Value
$preventiveBody = @{ machineId = $machine.id; type = 'PREVENTIVE'; performedAt = '2026-01-02T10:00:00Z'; description = 'Lubricate bearings'; technicianName = 'Antony'; technicianId = $null; anomalyId = $null; failure = $null; cause = $null; actionTaken = $null; downtimeMinutes = $null }
$preventive = (Send-Request POST '/api/v1/maintenance-records' 201 $preventiveBody).Value
Assert-That ($preventive.machineId -eq $machine.id -and $null -eq $preventive.failure) 'Preventive maintenance keeps its machine and has no failure details'
$correctiveBody = $preventiveBody.Clone(); $correctiveBody.type = 'CORRECTIVE'; $correctiveBody.performedAt = '2026-01-03T10:00:00Z'
$correctiveBody.failure = 'Bearing noise'; $correctiveBody.cause = 'Wear'; $correctiveBody.actionTaken = 'Replace bearing'; $correctiveBody.downtimeMinutes = 30; $correctiveBody.anomalyId = 'TS03-ANOMALY'
$corrective = (Send-Request POST '/api/v1/maintenance-records' 201 $correctiveBody).Value
Assert-That ($corrective.downtimeMinutes -eq 30 -and $corrective.anomalyId -eq 'TS03-ANOMALY') 'Corrective maintenance preserves details'
# Insert an older record last: history must sort by date, not insertion order.
$olderBody = $preventiveBody.Clone(); $olderBody.performedAt = '2026-01-01T10:00:00Z'
$older = (Send-Request POST '/api/v1/maintenance-records' 201 $olderBody).Value
$record = (Send-Request GET "/api/v1/maintenance-records/$($corrective.id)" 200).Value
Assert-That ($record.failure -eq 'Bearing noise' -and $record.technicianName -eq 'Antony') 'Record query preserves corrective details and technician'
$history = (Send-Request GET "/api/v1/maintenance-records/by-machine/$($machine.id)" 200).Value
Assert-That (@($history).Count -eq 3) 'History contains all three interventions'
Assert-That ($history[0].id -eq $corrective.id -and $history[1].id -eq $preventive.id -and $history[2].id -eq $older.id) 'History is ordered by maintenance date, newest first'
$otherHistory = (Send-Request GET "/api/v1/maintenance-records/by-machine/$($other.id)" 200).Value
Assert-That (@($otherHistory).Count -eq 0) 'History excludes interventions from another machine'
$filtered = (Send-Request GET "/api/v1/maintenance-records?machineId=$($machine.id)" 200).Value
Assert-That (@($filtered).Count -eq 3 -and @($filtered | Where-Object machineId -NE $machine.id).Count -eq 0) 'Record list filters by machine'
$invalidRecord = $preventiveBody.Clone(); $invalidRecord.failure = 'Not allowed'
$null = Send-Request POST '/api/v1/maintenance-records' 400 $invalidRecord $true 'Maintenance.PreventiveWithFailureDetails'
$invalidRecord = $correctiveBody.Clone(); $invalidRecord.downtimeMinutes = -1
$null = Send-Request POST '/api/v1/maintenance-records' 400 $invalidRecord $true 'Maintenance.InvalidDowntime'
$invalidRecord = $correctiveBody.Clone(); $invalidRecord.cause = $null
$null = Send-Request POST '/api/v1/maintenance-records' 400 $invalidRecord $true 'Maintenance.CorrectiveDetailsRequired'
$invalidRecord = $preventiveBody.Clone(); $invalidRecord.performedAt = '0001-01-01T00:00:00Z'
$null = Send-Request POST '/api/v1/maintenance-records' 400 $invalidRecord $true 'Maintenance.InvalidMaintenanceDate'
$missingId = [long]::MaxValue
$null = Send-Request GET "/api/v1/machines/$missingId" 404 $null $true 'Maintenance.MachineNotFound'
$null = Send-Request GET "/api/v1/machines/$missingId/status" 404 $null $true 'Maintenance.MachineNotFound'
$null = Send-Request GET "/api/v1/maintenance-records/by-machine/$missingId" 404 $null $true 'Maintenance.MachineNotFound'
$null = Send-Request GET "/api/v1/maintenance-records/$missingId" 404 $null $true 'Maintenance.RecordNotFound'
$missingBody = $preventiveBody.Clone(); $missingBody.machineId = $missingId
$null = Send-Request POST '/api/v1/maintenance-records' 404 $missingBody $true 'Maintenance.MachineNotFound'
$unchanged = (Send-Request GET "/api/v1/maintenance-records/by-machine/$($machine.id)" 200).Value
Assert-That (@($unchanged).Count -eq 3) 'Rejected records do not change history'
$report = [pscustomobject]@{ scenario = 'TS-03'; executedAt = [datetime]::UtcNow.ToString('o'); baseUri = $BaseUri; machineId = $machine.id; checksPassed = $checks.Count; checks = $checks }
if ($ReportPath) { $report | ConvertTo-Json -Depth 10 | Set-Content -LiteralPath $ReportPath -Encoding utf8 }
Write-Host "TS-03 completed: $($checks.Count) checks passed. Machine ID: $($machine.id)"
