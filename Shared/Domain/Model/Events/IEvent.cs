using Cortex.Mediator.Notifications;

namespace Molinex.Platform.Shared.Domain.Model.Events;

/// <summary>Domain event published in process through the mediator.</summary>
public interface IEvent : INotification;
