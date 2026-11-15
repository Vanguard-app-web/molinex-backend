using System.Net.Mail;
using Molinex.Platform.IdentityAndAccessManagement.Domain.Model.Errors;

namespace Molinex.Platform.IdentityAndAccessManagement.Domain.Model.ValueObjects;

/// <summary>Email address, normalized to lower case so sign-in ignores letter case.</summary>
public sealed record EmailAddress
{
    public const int MaxLength = 254;

    public EmailAddress(string? address)
    {
        var text = Guard.RequiredText(address, MaxLength, IamErrors.InvalidUserData);
        Guard.Against(!MailAddress.TryCreate(text, out var parsed) || parsed.Address != text,
            IamErrors.InvalidUserData);
        Address = text.ToLowerInvariant();
    }

    public string Address { get; }
}
