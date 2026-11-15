namespace Molinex.Platform.ProductionManagement.Domain.Model.Errors;

public static class ProductionErrors
{
    public static readonly Error ReceptionNotFound =
        Error.NotFound("Production.ReceptionNotFound", "The raw material reception was not found.");

    public static readonly Error BatchNotFound =
        Error.NotFound("Production.BatchNotFound", "The production batch was not found.");

    public static readonly Error RecordNotFound =
        Error.NotFound("Production.RecordNotFound", "The production record was not found.");

    public static readonly Error DuplicateBatchCode =
        Error.Conflict("Production.DuplicateBatchCode", "This batch code is already in use.");

    public static readonly Error InvalidReceptionDate =
        Error.Validation("Production.InvalidReceptionDate", "The reception date is required and cannot be in the future.");

    public static readonly Error InvalidSupplier =
        Error.Validation("Production.InvalidSupplier", "The supplier is required and cannot exceed 100 characters.");

    public static readonly Error InvalidMaterialOrigin =
        Error.Validation("Production.InvalidMaterialOrigin", "The material origin is required and cannot exceed 150 characters.");

    public static readonly Error InvalidBatchCode =
        Error.Validation("Production.InvalidBatchCode", "The batch code is required and cannot exceed 50 characters.");

    public static readonly Error InvalidRegistrationDate =
        Error.Validation("Production.InvalidRegistrationDate", "The batch registration date is required.");

    public static readonly Error InvalidProcessName =
        Error.Validation("Production.InvalidProcessName", "The process name is required and cannot exceed 100 characters.");

    public static readonly Error InvalidProductionPeriod =
        Error.Validation("Production.InvalidProductionPeriod", "The start date is required and the finish date cannot precede it.");

    public static readonly Error InvalidProductionStatus =
        Error.Validation("Production.InvalidProductionStatus", "The status must be PENDING, IN_PROGRESS, COMPLETED or CANCELLED.");

    public static readonly Error CompletedRequiresFinishDate =
        Error.Validation("Production.CompletedRequiresFinishDate", "A completed process requires its finish date.");

    public static readonly Error BatchChangeNotAllowed =
        Error.Validation("Production.BatchChangeNotAllowed", "A production record cannot be moved to another batch.");

    public static readonly Error StartDateChangeNotAllowed =
        Error.Validation("Production.StartDateChangeNotAllowed", "The start date of a production record cannot be changed.");
}
