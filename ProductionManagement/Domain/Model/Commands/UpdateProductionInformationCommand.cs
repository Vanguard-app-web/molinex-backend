using Cortex.Mediator.Commands;
using Molinex.Platform.ProductionManagement.Domain.Model.Aggregates;

namespace Molinex.Platform.ProductionManagement.Domain.Model.Commands;

public record UpdateProductionInformationCommand(
    long RecordId,
    long BatchId,
    string ProcessName,
    decimal ProcessedWeight,
    string Unit,
    DateTime StartedAt,
    DateTime? FinishedAt,
    string Status) : ICommand<Result<ProductionRecord>>;
