using Molinex.Platform.IdentityAndAccessManagement.Domain.Model.Aggregates;

namespace Molinex.Platform.IdentityAndAccessManagement.Application.Internal.OutboundServices;

public interface ITokenService
{
    string GenerateToken(User user);

    /// <returns>The user id carried by a valid token, or <c>null</c> when the token is not valid.</returns>
    Task<long?> ValidateToken(string token);
}
