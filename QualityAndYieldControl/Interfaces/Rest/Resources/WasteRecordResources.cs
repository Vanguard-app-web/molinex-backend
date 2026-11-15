using System.Text.Json.Serialization;

namespace Molinex.Platform.QualityAndYieldControl.Interfaces.Rest.Resources;

public record CreateWasteRecordResource(
    [property: JsonNumberHandling(JsonNumberHandling.AllowReadingFromString)]
    long ProductionRecordId,
    decimal QuantityValue,
    string QuantityUnit,
    decimal? BaseWeightValue,
    string? BaseWeightUnit,
    DateTime RecordedAt);

public record WasteRecordResource(
    string Id,
    string ProductionRecordId,
    decimal QuantityValue,
    string QuantityUnit,
    decimal? BaseWeightValue,
    string? BaseWeightUnit,
    decimal? Percentage,
    DateTime RecordedAt);
