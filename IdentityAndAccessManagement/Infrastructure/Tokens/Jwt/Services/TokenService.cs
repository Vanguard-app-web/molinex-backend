using System.Globalization;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using Molinex.Platform.IdentityAndAccessManagement.Application.Internal.OutboundServices;
using Molinex.Platform.IdentityAndAccessManagement.Domain.Model.Aggregates;
using Molinex.Platform.IdentityAndAccessManagement.Infrastructure.Tokens.Jwt.Configuration;

namespace Molinex.Platform.IdentityAndAccessManagement.Infrastructure.Tokens.Jwt.Services;

public class TokenService(IOptions<TokenSettings> tokenSettings, TimeProvider timeProvider) : ITokenService
{
    private readonly TokenSettings _settings = tokenSettings.Value;

    private SymmetricSecurityKey SigningKey => new(Encoding.UTF8.GetBytes(_settings.Secret!));

    public string GenerateToken(User user)
    {
        var now = timeProvider.GetUtcNow().UtcDateTime;
        var descriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity([
                new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
                new Claim(JwtRegisteredClaimNames.Email, user.Email.Address),
                new Claim(ClaimTypes.Role, user.Role)
            ]),
            Issuer = _settings.Issuer,
            Audience = _settings.Audience,
            IssuedAt = now,
            NotBefore = now,
            Expires = now.AddMinutes(_settings.ExpirationMinutes),
            SigningCredentials = new SigningCredentials(SigningKey, SecurityAlgorithms.HmacSha256)
        };
        return new JsonWebTokenHandler().CreateToken(descriptor);
    }

    public async Task<long?> ValidateToken(string token)
    {
        if (string.IsNullOrWhiteSpace(token)) return null;
        var result = await new JsonWebTokenHandler().ValidateTokenAsync(token, new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = SigningKey,
            ValidateIssuer = true,
            ValidIssuer = _settings.Issuer,
            ValidateAudience = true,
            ValidAudience = _settings.Audience,
            ValidateLifetime = true,
            ClockSkew = TimeSpan.FromSeconds(30)
        });
        if (!result.IsValid || result.SecurityToken is not JsonWebToken jwt) return null;
        return long.TryParse(jwt.Subject, NumberStyles.None, CultureInfo.InvariantCulture, out var userId)
            ? userId
            : null;
    }
}
