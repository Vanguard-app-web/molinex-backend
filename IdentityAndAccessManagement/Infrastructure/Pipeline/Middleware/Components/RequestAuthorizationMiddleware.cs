using Molinex.Platform.IdentityAndAccessManagement.Application.Internal.OutboundServices;
using Molinex.Platform.IdentityAndAccessManagement.Application.QueryServices;
using Molinex.Platform.IdentityAndAccessManagement.Domain.Model.Queries;
using Molinex.Platform.IdentityAndAccessManagement.Infrastructure.Pipeline.Middleware.Attributes;

namespace Molinex.Platform.IdentityAndAccessManagement.Infrastructure.Pipeline.Middleware.Components;

/// <summary>Validates the Bearer token, when present, and attaches the signed-in user to the request.</summary>
public class RequestAuthorizationMiddleware(RequestDelegate next)
{
    private const string BearerPrefix = "Bearer ";

    public async Task InvokeAsync(HttpContext context, IUserQueryService userQueryService, ITokenService tokenService)
    {
        var header = context.Request.Headers.Authorization.FirstOrDefault();
        if (header is not null && header.StartsWith(BearerPrefix, StringComparison.OrdinalIgnoreCase))
        {
            var userId = await tokenService.ValidateToken(header[BearerPrefix.Length..].Trim());
            if (userId.HasValue)
                context.Items[AuthorizeAttribute.UserItemKey] =
                    await userQueryService.Handle(new GetUserByIdQuery(userId.Value), context.RequestAborted);
        }

        await next(context);
    }
}
