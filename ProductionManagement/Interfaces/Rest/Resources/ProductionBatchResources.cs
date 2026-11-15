using System.Text.Json.Serialization;

namespace Molinex.Platform.ProductionManagement.Interfaces.Rest.Resources;

public record CreateProductionBatchResource(
    string Code,
    [property: JsonNumberHandling(JsonNumberHandling.AllowReadingFromString)]
    long ReceptionId,
    DateTime RegisteredAt);

public record ProductionBatchResource(string Id, string Code, string ReceptionId, DateTime RegisteredAt);
