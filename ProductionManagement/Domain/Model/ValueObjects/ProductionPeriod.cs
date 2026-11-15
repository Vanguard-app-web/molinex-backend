using Molinex.Platform.ProductionManagement.Domain.Model.Errors;

namespace Molinex.Platform.ProductionManagement.Domain.Model.ValueObjects;

/// <summary>Start and optional finish of a production process. The finish never precedes the start.</summary>
public sealed record ProductionPeriod
{
    public ProductionPeriod(DateTime startedAt, DateTime? finishedAt)
    {
        StartedAt = Guard.UtcInstant(startedAt, ProductionErrors.InvalidProductionPeriod);
        FinishedAt = finishedAt.HasValue
            ? Guard.UtcInstant(finishedAt.Value, ProductionErrors.InvalidProductionPeriod)
            : null;
        Guard.Against(FinishedAt < StartedAt, ProductionErrors.InvalidProductionPeriod);
    }

    public DateTime StartedAt { get; }
    public DateTime? FinishedAt { get; }
    public bool IsFinished => FinishedAt.HasValue;
}
