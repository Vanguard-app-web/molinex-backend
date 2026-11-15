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
[Tags("Production Batches")]
public class ProductionBatchesController(
    IMediator mediator,
    IProductionQueryService queryService,
    ProblemDetailsFactory problemDetailsFactory)
    : ControllerBase
{
    [HttpPost]
    [EndpointSummary("Register a production batch")]
    [ProducesResponseType<ProductionBatchResource>(StatusCodes.Status201Created)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> CreateProductionBatch([FromBody] CreateProductionBatchResource resource,
        CancellationToken cancellationToken)
    {
        var command = RegisterProductionBatchCommandFromResourceAssembler.ToCommandFromResource(resource);
        var result = await mediator
            .SendCommandAsync<RegisterProductionBatchCommand, Result<ProductionBatch>>(command, cancellationToken);
        return result.Match(
            batch => CreatedAtAction(nameof(GetProductionBatchById), new { id = batch.Id.Value },
                ProductionBatchResourceFromEntityAssembler.ToResourceFromEntity(batch)),
            error => problemDetailsFactory.ToActionResult(this, error));
    }

    [HttpGet]
    [EndpointSummary("List production batches, optionally by reception")]
    [ProducesResponseType<IEnumerable<ProductionBatchResource>>(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetProductionBatches([FromQuery] long? receptionId,
        CancellationToken cancellationToken)
    {
        var batches = await queryService.Handle(new GetProductionBatchesQuery(receptionId), cancellationToken);
        return Ok(batches.Select(ProductionBatchResourceFromEntityAssembler.ToResourceFromEntity));
    }

    [HttpGet("{id:long}")]
    [EndpointSummary("Get a production batch by id")]
    [ProducesResponseType<ProductionBatchResource>(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetProductionBatchById(long id, CancellationToken cancellationToken)
    {
        var batch = await queryService.Handle(new GetProductionBatchByIdQuery(id), cancellationToken);
        return batch is null
            ? problemDetailsFactory.ToActionResult(this, ProductionErrors.BatchNotFound)
            : Ok(ProductionBatchResourceFromEntityAssembler.ToResourceFromEntity(batch));
    }
}
