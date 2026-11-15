using Molinex.Platform.QualityAndYieldControl.Domain.Model.Commands;
using Molinex.Platform.QualityAndYieldControl.Domain.Model.Errors;
using Molinex.Platform.QualityAndYieldControl.Domain.Model.Events;
using Molinex.Platform.QualityAndYieldControl.Domain.Model.ValueObjects;

namespace Molinex.Platform.QualityAndYieldControl.Domain.Model.Aggregates;

public class WasteRecord : AuditableAggregateRoot
{
    private WasteRecord()
    {
        Quantity = null!;
    }

    public WasteRecord(RecordWasteCommand command)
    {
        Guard.Against(command.ProductionRecordId <= 0, QualityErrors.ProductionRecordNotFound);
        ProductionRecordId = new ProductionRecordId(command.ProductionRecordId);
        RecordedAt = Guard.UtcInstant(command.RecordedAt, QualityErrors.InvalidWasteDate);
        Quantity = Weight.From(command.QuantityValue, command.QuantityUnit);

        var hasBaseValue = command.BaseWeightValue.HasValue;
        var hasBaseUnit = !string.IsNullOrWhiteSpace(command.BaseWeightUnit);
        Guard.Against(hasBaseValue != hasBaseUnit, QualityErrors.InvalidBaseWeight);
        if (hasBaseValue)
        {
            BaseWeight = Weight.From(command.BaseWeightValue!.Value, command.BaseWeightUnit);
            // Units may differ: both weights are compared in kilograms.
            Guard.Against(Quantity.Exceeds(BaseWeight), QualityErrors.WasteExceedsBaseWeight);
        }

        AddDomainEvent(() => new WasteRecordedEvent(Id, ProductionRecordId));
    }

    public WasteRecordId Id { get; private set; }
    public ProductionRecordId ProductionRecordId { get; private set; }
    public Weight Quantity { get; private set; }
    public Weight? BaseWeight { get; private set; }
    public DateTime RecordedAt { get; private set; }

    /// <summary>Waste as a percentage of the base weight, or <c>null</c> when no base weight was given.</summary>
    public Percentage? Percentage => CalculatePercentage();

    public Percentage? CalculatePercentage()
    {
        if (BaseWeight is null) return null;
        var percentage = Quantity.ToKilograms() / BaseWeight.ToKilograms() * 100m;
        return new Percentage(Math.Round(percentage, 2, MidpointRounding.AwayFromZero));
    }
}
