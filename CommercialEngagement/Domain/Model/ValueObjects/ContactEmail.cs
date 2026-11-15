using System.Net.Mail;
using Molinex.Platform.CommercialEngagement.Domain.Model.Errors;

namespace Molinex.Platform.CommercialEngagement.Domain.Model.ValueObjects;

public sealed record ContactEmail
{
    public const int MaxLength = 254;

    public ContactEmail(string? address)
    {
        var text = Guard.RequiredText(address, MaxLength, CommercialErrors.InvalidEmail);
        Guard.Against(!MailAddress.TryCreate(text, out var parsed) || parsed.Address != text,
            CommercialErrors.InvalidEmail);
        Address = text;
    }

    public string Address { get; }
}
