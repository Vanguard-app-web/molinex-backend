using Cortex.Mediator.Commands;
using Molinex.Platform.ProductionManagement.Domain.Model.Aggregates;

namespace Molinex.Platform.ProductionManagement.Domain.Model.Commands;

public record RegisterProductionBatchCommand(
    string Code,
    long ReceptionId,
    DateTime RegisteredAt) : ICommand<Result<ProductionBatch>>;
