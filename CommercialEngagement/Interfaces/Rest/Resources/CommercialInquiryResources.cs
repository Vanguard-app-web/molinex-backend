namespace Molinex.Platform.CommercialEngagement.Interfaces.Rest.Resources;

public record CreateCommercialInquiryResource(string FullName, string CompanyName, string Email, string Message);

public record CommercialInquiryResource(
    string Id,
    string FullName,
    string CompanyName,
    string Email,
    string Message,
    DateTime SubmittedAt);
