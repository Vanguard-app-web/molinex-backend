namespace Molinex.Platform.IdentityAndAccessManagement.Infrastructure.Tokens.Jwt.Configuration;

/// <summary>JWT settings. The secret comes from <c>TokenSettings__Secret</c>; it is never stored in files.</summary>
public class TokenSettings
{
    public const string SectionName = "TokenSettings";
    public const int MinimumSecretBytes = 32;

    public string? Secret { get; set; }
    public string Issuer { get; set; } = "Molinex.Web";
    public string Audience { get; set; } = "Molinex.Vue";
    public int ExpirationMinutes { get; set; } = 60;
}
