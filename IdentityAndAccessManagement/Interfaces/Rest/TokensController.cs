using Cortex.Mediator;
using Microsoft.AspNetCore.Mvc;
using Molinex.Platform.IdentityAndAccessManagement.Domain.Model.Commands;
using Molinex.Platform.IdentityAndAccessManagement.Domain.Model.ValueObjects;
using Molinex.Platform.IdentityAndAccessManagement.Interfaces.Rest.Resources;
using Molinex.Platform.IdentityAndAccessManagement.Interfaces.Rest.Transform;
using Molinex.Platform.Shared.Interfaces.Rest.ProblemDetails;

namespace Molinex.Platform.IdentityAndAccessManagement.Interfaces.Rest;

[ApiController]
[Route("api/v1/[controller]")]
[Tags("Authentication")]
public class TokensController(IMediator mediator, ProblemDetailsFactory problemDetailsFactory) : ControllerBase
{
    [HttpPost]
    [EndpointSummary("Create an access token")]
    [EndpointDescription("Public. Authenticates the credentials and issues a JWT Bearer token.")]
    [ProducesResponseType<AuthenticatedUserResource>(StatusCodes.Status201Created)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> CreateToken([FromBody] SignInResource resource,
        CancellationToken cancellationToken)
    {
        var command = SignInCommandFromResourceAssembler.ToCommandFromResource(resource);
        var result = await mediator
            .SendCommandAsync<SignInCommand, Result<AuthenticatedUser>>(command, cancellationToken);
        return result.Match<IActionResult>(
            authenticatedUser => StatusCode(StatusCodes.Status201Created,
                AuthenticatedUserResourceFromEntityAssembler.ToResourceFromEntity(authenticatedUser)),
            error => problemDetailsFactory.ToActionResult(this, error));
    }
}
