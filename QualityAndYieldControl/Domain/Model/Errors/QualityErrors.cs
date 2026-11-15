namespace Molinex.Platform.QualityAndYieldControl.Domain.Model.Errors;

public static class QualityErrors
{
    public static readonly Error AssessmentNotFound =
        Error.NotFound("Quality.AssessmentNotFound", "The quality assessment was not found.");

    public static readonly Error WasteRecordNotFound =
        Error.NotFound("Quality.WasteRecordNotFound", "The waste record was not found.");

    public static readonly Error ProductionRecordNotFound =
        Error.NotFound("Quality.ProductionRecordNotFound", "The referenced production record was not found.");

    public static readonly Error ProductionBatchNotFound =
        Error.NotFound("Quality.ProductionBatchNotFound", "The referenced production batch was not found.");

    public static readonly Error InvalidPercentage =
        Error.Validation("Quality.InvalidPercentage", "A percentage must be between 0 and 100.");

    public static readonly Error InvalidQualityRange =
        Error.Validation("Quality.InvalidQualityRange", "The expected minimum cannot exceed the expected maximum.");

    public static readonly Error InvalidIndicator =
        Error.Validation("Quality.InvalidIndicator",
            "The indicator must be WHOLE_GRAIN_PERCENTAGE, BROKEN_GRAIN_PERCENTAGE or YIELD_PERCENTAGE.");

    public static readonly Error MeasurementsRequired =
        Error.Validation("Quality.MeasurementsRequired", "A quality assessment requires at least one measurement.");

    public static readonly Error DuplicateIndicator =
        Error.Validation("Quality.DuplicateIndicator", "Each quality indicator can be measured only once.");

    public static readonly Error GrainCompositionExceeded =
        Error.Validation("Quality.GrainCompositionExceeded", "Whole grain plus broken grain cannot exceed 100 %.");

    public static readonly Error InvalidAssessmentDate =
        Error.Validation("Quality.InvalidAssessmentDate", "The assessment date is required.");

    public static readonly Error InvalidWasteDate =
        Error.Validation("Quality.InvalidWasteDate", "The waste record date is required.");

    public static readonly Error InvalidBaseWeight =
        Error.Validation("Quality.InvalidBaseWeight", "The base weight requires both a value and a unit.");

    public static readonly Error WasteExceedsBaseWeight =
        Error.Validation("Quality.WasteExceedsBaseWeight", "Waste cannot exceed the base weight.");
}
