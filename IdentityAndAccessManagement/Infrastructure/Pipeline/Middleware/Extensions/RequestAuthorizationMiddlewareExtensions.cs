using Molinex.Platform.IdentityAndAccessManagement.Infrastructure.Pipeline.Middleware.Components;

namespace Molinex.Platform.IdentityAndAccessManagement.Infrastructure.Pipeline.Middleware.Extensions;

public static class RequestAuthorizationMiddlewareExtensions
{
    public static IApplicationBuilder UseRequestAuthorization(this IApplicationBuilder builder) =>
        builder.UseMiddleware<RequestAuthorizationMiddleware>();
}
