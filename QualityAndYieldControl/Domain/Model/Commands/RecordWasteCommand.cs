using Cortex.Mediator.Commands;
using Molinex.Platform.QualityAndYieldControl.Domain.Model.Aggregates;

namespace Molinex.Platform.QualityAndYieldControl.Domain.Model.Commands;

public record RecordWasteCommand(
    long ProductionRecordId,
    decimal QuantityValue,
    string QuantityUnit,
    decimal? BaseWeightValue,
    string? BaseWeightUnit,
    DateTime RecordedAt) : ICommand<Result<WasteRecord>>;
