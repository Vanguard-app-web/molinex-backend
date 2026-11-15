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
[Tags("Machines")]
public class MachinesController(
    IMediator mediator,
    IMaintenanceQueryService queryService,
    ProblemDetailsFactory problemDetailsFactory)
    : ControllerBase
{
    [HttpPost]
    [EndpointSummary("Register a machine")]
    [ProducesResponseType<MachineResource>(StatusCodes.Status201Created)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> CreateMachine([FromBody] CreateMachineResource resource,
        CancellationToken cancellationToken)
    {
        var command = RegisterMachineCommandFromResourceAssembler.ToCommandFromResource(resource);
        var result = await mediator.SendCommandAsync<RegisterMachineCommand, Result<Machine>>(command, cancellationToken);
        return result.Match(
            machine => CreatedAtAction(nameof(GetMachineById), new { id = machine.Id.Value },
                MachineResourceFromEntityAssembler.ToResourceFromEntity(machine)),
            error => problemDetailsFactory.ToActionResult(this, error));
    }

    [HttpGet]
    [EndpointSummary("List machines")]
    [ProducesResponseType<IEnumerable<MachineResource>>(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetMachines(CancellationToken cancellationToken)
    {
        var machines = await queryService.Handle(new GetMachinesQuery(), cancellationToken);
        return Ok(machines.Select(MachineResourceFromEntityAssembler.ToResourceFromEntity));
    }

    [HttpGet("{id:long}")]
    [EndpointSummary("Get a machine by id")]
    [ProducesResponseType<MachineResource>(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetMachineById(long id, CancellationToken cancellationToken)
    {
        var machine = await queryService.Handle(new GetMachineByIdQuery(id), cancellationToken);
        return machine is null
            ? problemDetailsFactory.ToActionResult(this, MaintenanceErrors.MachineNotFound)
            : Ok(MachineResourceFromEntityAssembler.ToResourceFromEntity(machine));
    }

    [HttpGet("{id:long}/status")]
    [EndpointSummary("Get the current status of a machine")]
    [ProducesResponseType<MachineStatusResource>(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetMachineStatus(long id, CancellationToken cancellationToken)
    {
        var machine = await queryService.Handle(new GetMachineByIdQuery(id), cancellationToken);
        return machine is null
            ? problemDetailsFactory.ToActionResult(this, MaintenanceErrors.MachineNotFound)
            : Ok(MachineResourceFromEntityAssembler.ToStatusResourceFromEntity(machine));
    }
}
