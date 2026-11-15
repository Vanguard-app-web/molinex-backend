using Cortex.Mediator;
using Microsoft.AspNetCore.Mvc;
using Molinex.Platform.AssetAndMaintenanceManagement.Application.QueryServices;
using Molinex.Platform.AssetAndMaintenanceManagement.Domain.Model.Aggregates;
using Molinex.Platform.AssetAndMaintenanceManagement.Domain.Model.Commands;
using Molinex.Platform.AssetAndMaintenanceManagement.Domain.Model.Errors;
using Molinex.Platform.AssetAndMaintenanceManagement.Domain.Model.Queries;
using Molinex.Platform.AssetAndMaintenanceManagement.Interfaces.Rest.Resources;
using Molinex.Platform.AssetAndMaintenanceManagement.Interfaces.Rest.Transform;
using Molinex.Platform.IdentityAndAccessManagement.Infrastructure.Pipeline.Middleware.Attributes;
using Molinex.Platform.Shared.Interfaces.Rest.ProblemDetails;

namespace Molinex.Platform.AssetAndMaintenanceManagement.Interfaces.Rest;

[ApiController]
[Authorize]
[Route("api/v1/[controller]")]
[Tags("Maintenance Records")]
public class MaintenanceRecordsController(
    IMediator mediator,
    IMaintenanceQueryService queryService,
    ProblemDetailsFactory problemDetailsFactory)
    : ControllerBase
{
    [HttpPost]
    [EndpointSummary("Record preventive or corrective maintenance")]
    [ProducesResponseType<MaintenanceRecordResource>(StatusCodes.Status201Created)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> CreateMaintenanceRecord([FromBody] CreateMaintenanceRecordResource resource,
        CancellationToken cancellationToken)
    {
        var command = RecordMaintenanceCommandFromResourceAssembler.ToCommandFromResource(resource);
        var result = await mediator
            .SendCommandAsync<RecordMaintenanceCommand, Result<MaintenanceRecord>>(command, cancellationToken);
        return result.Match(
            record => CreatedAtAction(nameof(GetMaintenanceRecordById), new { id = record.Id.Value },
                MaintenanceRecordResourceFromEntityAssembler.ToResourceFromEntity(record)),
            error => problemDetailsFactory.ToActionResult(this, error));
    }

    [HttpGet]
    [EndpointSummary("List maintenance records, optionally by machine")]
    [ProducesResponseType<IEnumerable<MaintenanceRecordResource>>(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetMaintenanceRecords([FromQuery] long? machineId,
        CancellationToken cancellationToken)
    {
        var records = await queryService.Handle(new GetMaintenanceRecordsQuery(machineId), cancellationToken);
        return Ok(records.Select(MaintenanceRecordResourceFromEntityAssembler.ToResourceFromEntity));
    }

    [HttpGet("by-machine/{machineId:long}")]
    [EndpointSummary("Get the maintenance history of a machine, most recent first")]
    [ProducesResponseType<IEnumerable<MaintenanceRecordResource>>(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetMaintenanceHistory(long machineId, CancellationToken cancellationToken)
    {
        var result = await queryService.Handle(new GetMaintenanceHistoryQuery(machineId), cancellationToken);
        return result.Match(
            records => Ok(records.Select(MaintenanceRecordResourceFromEntityAssembler.ToResourceFromEntity)),
            error => problemDetailsFactory.ToActionResult(this, error));
    }

    [HttpGet("{id:long}")]
    [EndpointSummary("Get a maintenance record by id")]
    [ProducesResponseType<MaintenanceRecordResource>(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetMaintenanceRecordById(long id, CancellationToken cancellationToken)
    {
        var record = await queryService.Handle(new GetMaintenanceRecordByIdQuery(id), cancellationToken);
        return record is null
            ? problemDetailsFactory.ToActionResult(this, MaintenanceErrors.RecordNotFound)
            : Ok(MaintenanceRecordResourceFromEntityAssembler.ToResourceFromEntity(record));
    }
}
