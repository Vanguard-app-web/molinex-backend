using Cortex.Mediator.Commands;
using Molinex.Platform.IdentityAndAccessManagement.Domain.Model.ValueObjects;

namespace Molinex.Platform.IdentityAndAccessManagement.Domain.Model.Commands;

public record SignInCommand(string Email, string Password) : ICommand<Result<AuthenticatedUser>>;
