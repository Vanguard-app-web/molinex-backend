using Microsoft.AspNetCore.Mvc.Filters;
using Molinex.Platform.IdentityAndAccessManagement.Domain.Model.Aggregates;
using Molinex.Platform.Shared.Interfaces.Rest.ProblemDetails;

namespace Molinex.Platform.IdentityAndAccessManagement.Infrastructure.Pipeline.Middleware.Attributes;

/// <summary>Requires a request authenticated by <c>RequestAuthorizationMiddleware</c>; otherwise answers 401.</summary>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
public class AuthorizeAttribute : Attribute, IAuthorizationFilter
{
    public const string UserItemKey = "User";

    public void OnAuthorization(AuthorizationFilterContext context)
    {
        if (context.ActionDescriptor.EndpointMetadata.OfType<AllowAnonymousAttribute>().Any()) return;
        if (context.HttpContext.Items[UserItemKey] is User { Active: true }) return;
        var problemDetailsFactory = context.HttpContext.RequestServices.GetRequiredService<ProblemDetailsFactory>();
        context.HttpContext.Response.Headers.WWWAuthenticate = "Bearer";
        context.Result = problemDetailsFactory.ToActionResult(context.HttpContext, SharedErrors.Unauthorized);
    }
}
