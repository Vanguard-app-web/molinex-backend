using Molinex.Platform.ProductionManagement.Domain.Model.Errors;

namespace Molinex.Platform.ProductionManagement.Domain.Model.ValueObjects;

public enum ProductionStatus
{
    Pending,
    InProgress,
    Completed,
    Cancelled
}

public static class ProductionStatusExtensions
{
    public static string ToCode(this ProductionStatus status)
    {
        return status switch
        {
            ProductionStatus.Pending => "PENDING",
            ProductionStatus.InProgress => "IN_PROGRESS",
            ProductionStatus.Completed => "COMPLETED",
            _ => "CANCELLED"
        };
    }

    public static ProductionStatus FromCode(string? code)
    {
        return code?.Trim() switch
        {
            "PENDING" => ProductionStatus.Pending,
            "IN_PROGRESS" => ProductionStatus.InProgress,
            "COMPLETED" => ProductionStatus.Completed,
            "CANCELLED" => ProductionStatus.Cancelled,
            _ => throw new DomainRuleException(ProductionErrors.InvalidProductionStatus)
        };
    }
}
