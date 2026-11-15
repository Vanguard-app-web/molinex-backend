using Molinex.Platform.ProductionManagement.Domain.Model.Errors;

namespace Molinex.Platform.ProductionManagement.Domain.Model.ValueObjects;

public sealed record ProductionProcessName
{
    public const int MaxLength = 100;

    public ProductionProcessName(string? value)
    {
        Value = Guard.RequiredText(value, MaxLength, ProductionErrors.InvalidProcessName);
    }

    public string Value { get; }
}
