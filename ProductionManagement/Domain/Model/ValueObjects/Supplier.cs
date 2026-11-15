using Molinex.Platform.ProductionManagement.Domain.Model.Errors;

namespace Molinex.Platform.ProductionManagement.Domain.Model.ValueObjects;

public sealed record Supplier
{
    public const int MaxLength = 100;

    public Supplier(string? name)
    {
        Name = Guard.RequiredText(name, MaxLength, ProductionErrors.InvalidSupplier);
    }

    public string Name { get; }
}
