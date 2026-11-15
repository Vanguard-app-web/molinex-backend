namespace Molinex.Platform.Shared.Infrastructure.Persistence.EntityFrameworkCore.Outbox;

/// <summary>Integration event waiting to be delivered to other systems.</summary>
public sealed class OutboxMessage
{
    private OutboxMessage()
    {
        Type = string.Empty;
        Payload = string.Empty;
    }

    public OutboxMessage(string type, string payload, DateTime occurredAt)
    {
        Id = Guid.NewGuid();
        Type = type;
        Payload = payload;
        OccurredAt = occurredAt;
    }

    public Guid Id { get; private set; }
    public string Type { get; private set; }
    public string Payload { get; private set; }
    public DateTime OccurredAt { get; private set; }
    public DateTime? ProcessedAt { get; private set; }
}
