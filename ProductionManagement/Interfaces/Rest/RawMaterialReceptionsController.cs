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
[Tags("Raw Material Receptions")]
public class RawMaterialReceptionsController(
    IMediator mediator,
    IProductionQueryService queryService,
    ProblemDetailsFactory problemDetailsFactory)
    : ControllerBase
{
    [HttpPost]
    [EndpointSummary("Record a raw material reception")]
    [ProducesResponseType<RawMaterialReceptionResource>(StatusCodes.Status201Created)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> CreateRawMaterialReception([FromBody] CreateRawMaterialReceptionResource resource,
        CancellationToken cancellationToken)
    {
        var command = RecordRawMaterialReceptionCommandFromResourceAssembler.ToCommandFromResource(resource);
        var result = await mediator
            .SendCommandAsync<RecordRawMaterialReceptionCommand, Result<RawMaterialReception>>(command, cancellationToken);
        return result.Match(
            reception => CreatedAtAction(nameof(GetRawMaterialReceptionById), new { id = reception.Id.Value },
                RawMaterialReceptionResourceFromEntityAssembler.ToResourceFromEntity(reception)),
            error => problemDetailsFactory.ToActionResult(this, error));
    }

    [HttpGet]
    [EndpointSummary("List raw material receptions")]
    [ProducesResponseType<IEnumerable<RawMaterialReceptionResource>>(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetAllRawMaterialReceptions(CancellationToken cancellationToken)
    {
        var receptions = await queryService.Handle(new GetAllRawMaterialReceptionsQuery(), cancellationToken);
        return Ok(receptions.Select(RawMaterialReceptionResourceFromEntityAssembler.ToResourceFromEntity));
    }

    [HttpGet("{id:long}")]
    [EndpointSummary("Get a raw material reception by id")]
    [ProducesResponseType<RawMaterialReceptionResource>(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetRawMaterialReceptionById(long id, CancellationToken cancellationToken)
    {
        var reception = await queryService.Handle(new GetRawMaterialReceptionByIdQuery(id), cancellationToken);
        return reception is null
            ? problemDetailsFactory.ToActionResult(this, ProductionErrors.ReceptionNotFound)
            : Ok(RawMaterialReceptionResourceFromEntityAssembler.ToResourceFromEntity(reception));
    }
}
