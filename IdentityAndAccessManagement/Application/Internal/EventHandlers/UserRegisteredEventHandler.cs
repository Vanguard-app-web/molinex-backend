using Molinex.Platform.IdentityAndAccessManagement.Domain.Model.Events;
using Molinex.Platform.Shared.Application.Internal.EventHandlers;

namespace Molinex.Platform.IdentityAndAccessManagement.Application.Internal.EventHandlers;

public class UserRegisteredEventHandler(ILogger<UserRegisteredEventHandler> logger)
    : IEventHandler<UserRegisteredEvent>
{
    public Task Handle(UserRegisteredEvent domainEvent, CancellationToken cancellationToken)
    {
        logger.LogInformation("User {UserId} registered with role {Role}", domainEvent.UserId.Value, domainEvent.Role);
        return Task.CompletedTask;
    }
}
