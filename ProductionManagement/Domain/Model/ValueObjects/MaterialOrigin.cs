using Molinex.Platform.ProductionManagement.Domain.Model.Errors;

namespace Molinex.Platform.ProductionManagement.Domain.Model.ValueObjects;

public sealed record MaterialOrigin
{
    public const int MaxLength = 150;

    public MaterialOrigin(string? description)
    {
        Description = Guard.RequiredText(description, MaxLength, ProductionErrors.InvalidMaterialOrigin);
    }

    public string Description { get; }
}
