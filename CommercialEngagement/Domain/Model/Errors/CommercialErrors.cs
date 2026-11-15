namespace Molinex.Platform.CommercialEngagement.Domain.Model.Errors;

public static class CommercialErrors
{
    public static readonly Error InquiryNotFound =
        Error.NotFound("Commercial.InquiryNotFound", "The commercial inquiry was not found.");

    public static readonly Error InvalidFullName =
        Error.Validation("Commercial.InvalidFullName", "The full name is required.");

    public static readonly Error InvalidCompanyName =
        Error.Validation("Commercial.InvalidCompanyName", "The company name is required.");

    public static readonly Error InvalidEmail =
        Error.Validation("Commercial.InvalidEmail", "A valid email address is required.");

    public static readonly Error InvalidMessage =
        Error.Validation("Commercial.InvalidMessage", "The message is required.");
}
