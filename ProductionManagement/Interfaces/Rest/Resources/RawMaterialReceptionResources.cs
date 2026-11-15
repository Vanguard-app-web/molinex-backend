namespace Molinex.Platform.ProductionManagement.Interfaces.Rest.Resources;

public record CreateRawMaterialReceptionResource(
    DateTime ReceivedAt,
    string Supplier,
    string Origin,
    decimal Quantity,
    string Unit);

public record RawMaterialReceptionResource(
    string Id,
    DateTime ReceivedAt,
    string Supplier,
    string Origin,
    decimal Quantity,
    string Unit);
