namespace Molinex.Platform.Shared.Domain.Model.Errors;

public static class SharedErrors
{
    public static readonly Error InvalidInput =
        Error.Validation("Shared.InvalidInput", "Invalid or incomplete input.");

    public static readonly Error InvalidWeight =
        Error.Validation("Shared.InvalidWeight", "Weight must be greater than zero.");

    public static readonly Error InvalidMeasurementUnit =
        Error.Validation("Shared.InvalidMeasurementUnit", "The unit must be KILOGRAM or METRIC_TON.");

    public static readonly Error InvalidDateRange =
        Error.Validation("Shared.InvalidDateRange", "The end date cannot precede the start date.");

    public static readonly Error Unauthorized =
        Error.Unauthorized("Shared.Unauthorized", "A valid access token is required.");

    public static readonly Error Conflict =
        Error.Conflict("Shared.Conflict", "The operation conflicts with existing data.");

    public static readonly Error OperationCancelled =
        Error.Conflict("Shared.OperationCancelled", "The operation was cancelled.");

    public static readonly Error UnexpectedError =
        Error.Unexpected("Shared.UnexpectedError", "An unexpected error occurred.");
}
