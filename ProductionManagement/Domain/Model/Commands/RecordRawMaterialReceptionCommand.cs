using Cortex.Mediator.Commands;
using Molinex.Platform.ProductionManagement.Domain.Model.Aggregates;

namespace Molinex.Platform.ProductionManagement.Domain.Model.Commands;

public record RecordRawMaterialReceptionCommand(
    DateTime ReceivedAt,
    string Supplier,
    string Origin,
    decimal Quantity,
    string Unit) : ICommand<Result<RawMaterialReception>>;
