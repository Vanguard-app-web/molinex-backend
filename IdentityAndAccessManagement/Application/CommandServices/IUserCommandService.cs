using Molinex.Platform.IdentityAndAccessManagement.Domain.Model.Aggregates;
using Molinex.Platform.IdentityAndAccessManagement.Domain.Model.Commands;
using Molinex.Platform.IdentityAndAccessManagement.Domain.Model.ValueObjects;

namespace Molinex.Platform.IdentityAndAccessManagement.Application.CommandServices;

/// <summary>Application contract for identity commands, dispatched through the mediator.</summary>
public interface IUserCommandService
{
    Task<Result<AuthenticatedUser>> Handle(SignInCommand command, CancellationToken cancellationToken);
    Task<Result<User>> Handle(RegisterAdministratorCommand command, CancellationToken cancellationToken);
}
