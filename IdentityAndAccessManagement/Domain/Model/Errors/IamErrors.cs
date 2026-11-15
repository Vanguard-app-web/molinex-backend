namespace Molinex.Platform.IdentityAndAccessManagement.Domain.Model.Errors;

public static class IamErrors
{
    public static readonly Error InvalidCredentials =
        Error.Unauthorized("Iam.InvalidCredentials", "The credentials are invalid.");

    public static readonly Error InvalidUserData =
        Error.Validation("Iam.InvalidUserData",
            "A valid email and a password of at least 12 characters are required.");

    public static readonly Error AdministratorAlreadyRegistered =
        Error.Conflict("Iam.AdministratorAlreadyRegistered", "An account is already registered.");
}
