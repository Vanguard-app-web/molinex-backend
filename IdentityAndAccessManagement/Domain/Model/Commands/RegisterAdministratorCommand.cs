using Cortex.Mediator.Commands;
using Molinex.Platform.IdentityAndAccessManagement.Domain.Model.Aggregates;

namespace Molinex.Platform.IdentityAndAccessManagement.Domain.Model.Commands;

/// <summary>Registers the first administrator account. It fails once any account exists.</summary>
public record RegisterAdministratorCommand(string Email, string Password) : ICommand<Result<User>>;
