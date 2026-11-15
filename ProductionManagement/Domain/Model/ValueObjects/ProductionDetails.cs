using Molinex.Platform.ProductionManagement.Domain.Model.Errors;

namespace Molinex.Platform.ProductionManagement.Domain.Model.ValueObjects;

/// <summary>What was processed, how much, when and in which state. A completed process has a finish date.</summary>
public sealed record ProductionDetails
{
    private ProductionDetails()
    {
        ProcessName = null!;
        ProcessedWeight = null!;
        Period = null!;
    }

    public ProductionDetails(ProductionProcessName processName, Weight processedWeight, ProductionPeriod period,
        ProductionStatus status)
    {
        Guard.Against(status == ProductionStatus.Completed && !period.IsFinished,
            ProductionErrors.CompletedRequiresFinishDate);
        ProcessName = processName;
        ProcessedWeight = processedWeight;
        Period = period;
        Status = status;
    }

    public ProductionProcessName ProcessName { get; private set; }
    public Weight ProcessedWeight { get; private set; }
    public ProductionPeriod Period { get; private set; }
    public ProductionStatus Status { get; private set; }

    public static ProductionDetails From(string? processName, decimal processedWeight, string? unit,
        DateTime startedAt, DateTime? finishedAt, string? status)
    {
        return new ProductionDetails(
            new ProductionProcessName(processName),
            Weight.From(processedWeight, unit),
            new ProductionPeriod(startedAt, finishedAt),
            ProductionStatusExtensions.FromCode(status));
    }
}
