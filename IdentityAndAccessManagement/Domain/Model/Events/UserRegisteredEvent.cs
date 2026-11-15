using Molinex.Platform.IdentityAndAccessManagement.Domain.Model.ValueObjects;

namespace Molinex.Platform.IdentityAndAccessManagement.Domain.Model.Events;

public record UserRegisteredEvent(UserId UserId, string Role) : IEvent;
