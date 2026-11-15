using Molinex.Platform.IdentityAndAccessManagement.Application.Internal.OutboundServices;
using BCryptNet = BCrypt.Net.BCrypt;

namespace Molinex.Platform.IdentityAndAccessManagement.Infrastructure.Hashing.BCrypt.Services;

public class HashingService : IHashingService
{
    public string HashPassword(string password) => BCryptNet.HashPassword(password);

    public bool VerifyPassword(string password, string passwordHash)
    {
        try
        {
            return BCryptNet.Verify(password, passwordHash);
        }
        catch (global::BCrypt.Net.SaltParseException)
        {
            return false;
        }
    }
}
