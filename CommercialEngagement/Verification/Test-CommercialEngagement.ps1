<#
.SYNOPSIS
Checks Commercial Engagement against an integrated API using synthetic data.
#>
param(
    [string]$BaseUrl = 'http://localhost:5081',
    [string]$Token
)
$ErrorActionPreference = 'Stop'
if ([string]::IsNullOrWhiteSpace($Token)) { throw 'Pass a valid IAM token with -Token.' }
$client = [System.Net.Http.HttpClient]::new()
$checks = [System.Collections.Generic.List[object]]::new()
function Send-Request {
    param([string]$Method, [string]$Path, $Body = $null, [switch]$Authenticated, [string]$Language = 'en')
    $request = [System.Net.Http.HttpRequestMessage]::new([System.Net.Http.HttpMethod]::new($Method), $BaseUrl.TrimEnd('/') + $Path)
    [void]$request.Headers.TryAddWithoutValidation('Accept-Language', $Language)
    if ($Authenticated) { [void]$request.Headers.TryAddWithoutValidation('Authorization', "Bearer $Token") }
    if ($null -ne $Body) { $request.Content = [System.Net.Http.StringContent]::new(($Body | ConvertTo-Json -Depth 6), [System.Text.Encoding]::UTF8, 'application/json') }
    $response = $client.SendAsync($request).GetAwaiter().GetResult()
    try {
        $text = $response.Content.ReadAsStringAsync().GetAwaiter().GetResult()
        $data = if ($text) { $text | ConvertFrom-Json } else { $null }
        [pscustomobject]@{ Status = [int]$response.StatusCode; Data = $data; Location = [string]$response.Headers.Location }
    } finally { $response.Dispose(); $request.Dispose() }
}
function Check {
    param([string]$Name, [bool]$Passed)
    $checks.Add([pscustomobject]@{ Check = $Name; Result = $(if ($Passed) { 'PASS' } else { 'FAIL' }) })
}
try {
    $body = @{ fullName = 'Brayan Demo'; companyName = 'Molino Demo'; email = 'demo@example.test'; message = 'Request a demonstration.' }
    $created = Send-Request POST '/api/v1/commercial-inquiries' $body
    Check 'anonymous creation returns 201' ($created.Status -eq 201)
    Check 'created resource includes a string id' ($created.Data.id -is [string] -and $created.Data.id -match '^\d+$')
    Check 'location identifies created inquiry' ($created.Location -match "/api/v1/commercial-inquiries/$($created.Data.id)$")
    Check 'submitted resource preserves fields' ($created.Data.fullName -eq $body.fullName -and $created.Data.email -eq $body.email -and $created.Data.message -eq $body.message)
    Check 'server assigns submission date' ([bool]$created.Data.submittedAt)
    $path = "/api/v1/commercial-inquiries/$($created.Data.id)"
    $anonymous = Send-Request GET $path
    Check 'anonymous read returns 401' ($anonymous.Status -eq 401 -and $anonymous.Data.code -eq 'Shared.Unauthorized')
    $read = Send-Request GET $path -Authenticated
    Check 'authenticated read returns persisted resource' ($read.Status -eq 200 -and $read.Data.id -eq $created.Data.id -and $read.Data.companyName -eq $body.companyName)
    $missing = Send-Request GET '/api/v1/commercial-inquiries/9223372036854775807' -Authenticated
    Check 'missing inquiry returns 404' ($missing.Status -eq 404 -and $missing.Data.code -eq 'Commercial.InquiryNotFound')
    foreach ($entry in @(@('fullName','Commercial.InvalidFullName'),@('companyName','Commercial.InvalidCompanyName'),@('message','Commercial.InvalidMessage'))) {
        $invalid = $body.Clone(); $invalid[$entry[0]] = ' '
        $response = Send-Request POST '/api/v1/commercial-inquiries' $invalid
        Check "blank $($entry[0]) returns 400" ($response.Status -eq 400 -and $response.Data.code -eq $entry[1])
        $invalid[$entry[0]] = 'x' * 2049
        $response = Send-Request POST '/api/v1/commercial-inquiries' $invalid
        Check "oversized $($entry[0]) returns 400" ($response.Status -eq 400 -and $response.Data.code -eq $entry[1])
    }
    $badEmail = $body.Clone(); $badEmail.email = 'invalid-email'
    $english = Send-Request POST '/api/v1/commercial-inquiries' $badEmail
    $spanish = Send-Request POST '/api/v1/commercial-inquiries' $badEmail -Language 'es'
    Check 'invalid email returns stable validation code' ($english.Status -eq 400 -and $english.Data.code -eq 'Commercial.InvalidEmail')
    Check 'spanish validation keeps code and changes text' ($spanish.Status -eq 400 -and $spanish.Data.code -eq $english.Data.code -and $spanish.Data.detail -ne $english.Data.detail)
    $maximum = $body.Clone(); $maximum.fullName = 'a' * 2048; $maximum.companyName = 'b' * 2048; $maximum.message = 'c' * 2048
    $accepted = Send-Request POST '/api/v1/commercial-inquiries' $maximum
    Check 'maximum text lengths remain valid' ($accepted.Status -eq 201)
    $checks | Format-Table -AutoSize
    $failed = @($checks | Where-Object Result -eq 'FAIL').Count
    Write-Output "Checks: $($checks.Count) Passed: $($checks.Count - $failed) Failed: $failed"
    if ($failed) { throw 'Commercial Engagement verification failed.' }
} finally { $client.Dispose() }
