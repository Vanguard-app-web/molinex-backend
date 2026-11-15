namespace Molinex.Platform.AssetAndMaintenanceManagement.Domain.Model.Errors;

public static class MaintenanceErrors
{
    public static readonly Error MachineNotFound =
        Error.NotFound("Maintenance.MachineNotFound", "The machine was not found.");

    public static readonly Error RecordNotFound =
        Error.NotFound("Maintenance.RecordNotFound", "The maintenance record was not found.");

    public static readonly Error DuplicateMachineCode =
        Error.Conflict("Maintenance.DuplicateMachineCode", "This machine code is already in use.");

    public static readonly Error InvalidMachineCode =
        Error.Validation("Maintenance.InvalidMachineCode", "The machine code is required and cannot exceed 30 characters.");

    public static readonly Error InvalidMachineName =
        Error.Validation("Maintenance.InvalidMachineName", "The machine name is required and cannot exceed 100 characters.");

    public static readonly Error InvalidMachineModel =
        Error.Validation("Maintenance.InvalidMachineModel", "The machine model is required and cannot exceed 100 characters.");

    public static readonly Error InvalidMachineStatus =
        Error.Validation("Maintenance.InvalidMachineStatus",
            "The status must be OPERATIONAL, REQUIRES_ATTENTION, UNDER_MAINTENANCE or OUT_OF_SERVICE.");

    public static readonly Error StatusChangeDateRequired =
        Error.Validation("Maintenance.StatusChangeDateRequired", "The date of the status change is required.");

    public static readonly Error InvalidMaintenanceType =
        Error.Validation("Maintenance.InvalidMaintenanceType", "The maintenance type must be PREVENTIVE or CORRECTIVE.");

    public static readonly Error InvalidMaintenanceDate =
        Error.Validation("Maintenance.InvalidMaintenanceDate", "The maintenance date is required.");

    public static readonly Error InvalidDescription =
        Error.Validation("Maintenance.InvalidDescription", "The description is required and cannot exceed 500 characters.");

    public static readonly Error InvalidTechnician =
        Error.Validation("Maintenance.InvalidTechnician", "The technician name is required and cannot exceed 100 characters.");

    public static readonly Error InvalidAnomalyReference =
        Error.Validation("Maintenance.InvalidAnomalyReference", "The anomaly reference cannot exceed 100 characters.");

    public static readonly Error CorrectiveDetailsRequired =
        Error.Validation("Maintenance.CorrectiveDetailsRequired",
            "Corrective maintenance requires failure and cause (up to 250 characters), action taken (up to 500) and downtime minutes.");

    public static readonly Error InvalidDowntime =
        Error.Validation("Maintenance.InvalidDowntime", "Downtime minutes must be an integer greater than or equal to zero.");

    public static readonly Error PreventiveWithFailureDetails =
        Error.Validation("Maintenance.PreventiveWithFailureDetails", "Preventive maintenance cannot include failure details.");
}
