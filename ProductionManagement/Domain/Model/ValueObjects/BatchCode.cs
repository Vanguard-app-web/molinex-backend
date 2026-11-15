using Molinex.Platform.ProductionManagement.Domain.Model.Errors;

namespace Molinex.Platform.ProductionManagement.Domain.Model.ValueObjects;

/// <summary>Batch code. Two codes that differ only in letter case are the same code.</summary>
public sealed record BatchCode
{
    public const int MaxLength = 50;

    public BatchCode(string? value)
    {
        Value = Guard.RequiredText(value, MaxLength, ProductionErrors.InvalidBatchCode);
    }

    public string Value { get; }

    public bool Equals(BatchCode? other) =>
        other is not null && string.Equals(Value, other.Value, StringComparison.OrdinalIgnoreCase);

    public override int GetHashCode() => StringComparer.OrdinalIgnoreCase.GetHashCode(Value);
}
