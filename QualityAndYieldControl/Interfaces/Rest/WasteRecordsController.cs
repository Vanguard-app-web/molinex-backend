using Cortex.Mediator;
using Microsoft.AspNetCore.Mvc;
using Molinex.Platform.IdentityAndAccessManagement.Infrastructure.Pipeline.Middleware.Attributes;
using Molinex.Platform.QualityAndYieldControl.Application.QueryServices;
using Molinex.Platform.QualityAndYieldControl.Domain.Model.Aggregates;
using Molinex.Platform.QualityAndYieldControl.Domain.Model.Commands;
using Molinex.Platform.QualityAndYieldControl.Domain.Model.Errors;
using Molinex.Platform.QualityAndYieldControl.Domain.Model.Queries;
using Molinex.Platform.QualityAndYieldControl.Interfaces.Rest.Resources;
using Molinex.Platform.QualityAndYieldControl.Interfaces.Rest.Transform;
using Molinex.Platform.Shared.Interfaces.Rest.ProblemDetails;

namespace Molinex.Platform.QualityAndYieldControl.Interfaces.Rest;

[ApiController]
[Authorize]
[Route("api/v1/[controller]")]
[Tags("Waste Records")]
public class WasteRecordsController(
    IMediator mediator,
    IQualityQueryService queryService,
    ProblemDetailsFactory problemDetailsFactory)
    : ControllerBase
{
    [HttpPost]
    [EndpointSummary("Record the waste of a production record")]
    [EndpointDescription("Waste and base weight may use different units; they are compared in kilograms.")]
    [ProducesResponseType<WasteRecordResource>(StatusCodes.Status201Created)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> CreateWasteRecord([FromBody] CreateWasteRecordResource resource,
        CancellationToken cancellationToken)
    {
        var command = RecordWasteCommandFromResourceAssembler.ToCommandFromResource(resource);
        var result = await mediator
            .SendCommandAsync<RecordWasteCommand, Result<WasteRecord>>(command, cancellationToken);
        return result.Match(
            wasteRecord => CreatedAtAction(nameof(GetWasteRecordById), new { id = wasteRecord.Id.Value },
                WasteRecordResourceFromEntityAssembler.ToResourceFromEntity(wasteRecord)),
            error => problemDetailsFactory.ToActionResult(this, error));
    }

    [HttpGet]
    [EndpointSummary("List waste records, optionally by production record")]
    [ProducesResponseType<IEnumerable<WasteRecordResource>>(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetWasteRecords([FromQuery] long? productionRecordId,
        CancellationToken cancellationToken)
    {
        var wasteRecords = await queryService.Handle(new GetWasteRecordsQuery(productionRecordId), cancellationToken);
        return Ok(wasteRecords.Select(WasteRecordResourceFromEntityAssembler.ToResourceFromEntity));
    }

    [HttpGet("{id:long}")]
    [EndpointSummary("Get a waste record by id")]
    [ProducesResponseType<WasteRecordResource>(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetWasteRecordById(long id, CancellationToken cancellationToken)
    {
        var wasteRecord = await queryService.Handle(new GetWasteRecordByIdQuery(id), cancellationToken);
        return wasteRecord is null
            ? problemDetailsFactory.ToActionResult(this, QualityErrors.WasteRecordNotFound)
            : Ok(WasteRecordResourceFromEntityAssembler.ToResourceFromEntity(wasteRecord));
    }
}
