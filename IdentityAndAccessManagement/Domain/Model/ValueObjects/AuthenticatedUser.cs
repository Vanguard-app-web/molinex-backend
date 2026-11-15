using Molinex.Platform.IdentityAndAccessManagement.Domain.Model.Aggregates;

namespace Molinex.Platform.IdentityAndAccessManagement.Domain.Model.ValueObjects;

/// <summary>A signed-in user together with the access token issued for the session.</summary>
public sealed record AuthenticatedUser(User User, string Token);
