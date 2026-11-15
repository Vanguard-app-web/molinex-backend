using Molinex.Platform.IdentityAndAccessManagement.Application.QueryServices;
using Molinex.Platform.IdentityAndAccessManagement.Domain.Model.Aggregates;
using Molinex.Platform.IdentityAndAccessManagement.Domain.Model.Queries;
using Molinex.Platform.IdentityAndAccessManagement.Domain.Repositories;

namespace Molinex.Platform.IdentityAndAccessManagement.Application.Internal.QueryServices;

public class UserQueryService(IUserRepository userRepository) : IUserQueryService
{
    public async Task<User?> Handle(GetUserByIdQuery query, CancellationToken cancellationToken)
    {
        return await userRepository.FindByIdAsNoTrackingAsync(query.Id, cancellationToken);
    }
}
