using Cortex.Mediator;
using Microsoft.AspNetCore.Mvc;
using Molinex.Platform.IdentityAndAccessManagement.Infrastructure.Pipeline.Middleware.Attributes;
using Molinex.Platform.ProductionManagement.Application.QueryServices;
using Molinex.Platform.ProductionManagement.Domain.Model.Aggregates;
using Molinex.Platform.ProductionManagement.Domain.Model.Commands;
using Molinex.Platform.ProductionManagement.Domain.Model.Errors;
using Molinex.Platform.ProductionManagement.Domain.Model.Queries;
using Molinex.Platform.ProductionManagement.Interfaces.Rest.Resources;
using Molinex.Platform.ProductionManagement.Interfaces.Rest.Transform;
using Molinex.Platform.Shared.Interfaces.Rest.ProblemDetails;

namespace Molinex.Platform.ProductionManagement.Interfaces.Rest;

[ApiController]
[Authorize]
[Route("api/v1/[controller]")]
[Tags("Production Records")]
public class ProductionRecordsController(
    IMediator mediator,
    IProductionQueryService queryService,
    ProblemDetailsFactory problemDetailsFactory)
    : ControllerBase
{
    [HttpPost]
    [EndpointSummary("Record production information")]
    [ProducesResponseType<ProductionRecordResource>(StatusCodes.Status201Created)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> CreateProductionRecord([FromBody] CreateProductionRecordResource resource,
        CancellationToken cancellationToken)
    {
        var command = RecordProductionInformationCommandFromResourceAssembler.ToCommandFromResource(resource);
        var result = await mediator
            .SendCommandAsync<RecordProductionInformationCommand, Result<ProductionRecord>>(command, cancellationToken);
        return result.Match(
            record => CreatedAtAction(nameof(GetProductionRecordById), new { id = record.Id.Value },
                ProductionRecordResourceFromEntityAssembler.ToResourceFromEntity(record)),
            error => problemDetailsFactory.ToActionResult(this, error));
    }

    [HttpPut("{id:long}")]
    [EndpointSummary("Update production information")]
    [EndpointDescription("Updates the process information. The record keeps its identity, batch and start date.")]
    [ProducesResponseType<ProductionRecordResource>(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateProductionRecord(long id, [FromBody] UpdateProductionRecordResource resource,
        CancellationToken cancellationToken)
    {
        var command = UpdateProductionInformationCommandFromResourceAssembler.ToCommandFromResource(id, resource);
        var result = await mediator
            .SendCommandAsync<UpdateProductionInformationCommand, Result<ProductionRecord>>(command, cancellationToken);
        return result.Match(
            record => Ok(ProductionRecordResourceFromEntityAssembler.ToResourceFromEntity(record)),
            error => problemDetailsFactory.ToActionResult(this, error));
    }

    [HttpGet]
    [EndpointSummary("List production records, optionally by batch")]
    [ProducesResponseType<IEnumerable<ProductionRecordResource>>(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetProductionRecords([FromQuery] long? batchId,
        CancellationToken cancellationToken)
    {
        var result = await queryService.Handle(new GetProductionRecordsQuery(batchId, null, null), cancellationToken);
        return result.Match(
            records => Ok(records.Select(ProductionRecordResourceFromEntityAssembler.ToResourceFromEntity)),
            error => problemDetailsFactory.ToActionResult(this, error));
    }

    [HttpGet("history")]
    [EndpointSummary("Get the production history, newest first")]
    [ProducesResponseType<IEnumerable<ProductionRecordResource>>(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetProductionHistory([FromQuery] DateTime? from, [FromQuery] DateTime? to,
        CancellationToken cancellationToken)
    {
        var result = await queryService.Handle(new GetProductionHistoryQuery(from, to), cancellationToken);
        return result.Match(
            records => Ok(records.Select(ProductionRecordResourceFromEntityAssembler.ToResourceFromEntity)),
            error => problemDetailsFactory.ToActionResult(this, error));
    }

    [HttpGet("{id:long}")]
    [EndpointSummary("Get a production record by id")]
    [ProducesResponseType<ProductionRecordResource>(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetProductionRecordById(long id, CancellationToken cancellationToken)
    {
        var record = await queryService.Handle(new GetProductionRecordByIdQuery(id), cancellationToken);
        return record is null
            ? problemDetailsFactory.ToActionResult(this, ProductionErrors.RecordNotFound)
            : Ok(ProductionRecordResourceFromEntityAssembler.ToResourceFromEntity(record));
    }
}
