using System.Text.Json.Serialization;

namespace Molinex.Platform.ProductionManagement.Interfaces.Rest.Resources;

public record CreateProductionRecordResource(
    [property: JsonNumberHandling(JsonNumberHandling.AllowReadingFromString)]
    long BatchId,
    string ProcessName,
    decimal ProcessedWeight,
    string Unit,
    DateTime StartedAt,
    DateTime? FinishedAt,
    string Status);

public record UpdateProductionRecordResource(
    [property: JsonNumberHandling(JsonNumberHandling.AllowReadingFromString)]
    long BatchId,
    string ProcessName,
    decimal ProcessedWeight,
    string Unit,
    DateTime StartedAt,
    DateTime? FinishedAt,
    string Status);

public record ProductionRecordResource(
    string Id,
    string BatchId,
    string ProcessName,
    decimal ProcessedWeight,
    string Unit,
    DateTime StartedAt,
    DateTime? FinishedAt,
    string Status);
