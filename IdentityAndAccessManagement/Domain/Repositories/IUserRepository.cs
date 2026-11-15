using Molinex.Platform.IdentityAndAccessManagement.Domain.Model.Aggregates;
using Molinex.Platform.IdentityAndAccessManagement.Domain.Model.ValueObjects;

namespace Molinex.Platform.IdentityAndAccessManagement.Domain.Repositories;

public interface IUserRepository : IBaseRepository<User>
{
    Task<User?> FindByEmailAsync(EmailAddress email, CancellationToken cancellationToken = default);
    Task<bool> AnyAsync(CancellationToken cancellationToken = default);
}
