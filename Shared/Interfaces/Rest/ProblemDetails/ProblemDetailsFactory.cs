using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using Molinex.Platform.Resources.Errors;
using Molinex.Platform.Resources.Shared;

namespace Molinex.Platform.Shared.Interfaces.Rest.ProblemDetails;

/// <summary>
///     Builds RFC 9457 Problem Details from an <see cref="Error" />: localized title and detail,
///     HTTP status derived from the error type and the stable error code in the <c>code</c> member.
/// </summary>
public class ProblemDetailsFactory(
    IStringLocalizer<ErrorMessages> errorLocalizer,
    IStringLocalizer<CommonMessages> commonLocalizer)
{
    public const string ProblemContentType = "application/problem+json";

    public static int ToStatusCode(EErrorType type)
    {
        return type switch
        {
            EErrorType.Validation => StatusCodes.Status400BadRequest,
            EErrorType.NotFound => StatusCodes.Status404NotFound,
            EErrorType.Conflict => StatusCodes.Status409Conflict,
            EErrorType.Unauthorized => StatusCodes.Status401Unauthorized,
            EErrorType.Forbidden => StatusCodes.Status403Forbidden,
            EErrorType.Unexpected => StatusCodes.Status500InternalServerError,
            _ => StatusCodes.Status400BadRequest
        };
    }

    public Microsoft.AspNetCore.Mvc.ProblemDetails Create(HttpContext httpContext, Error error)
    {
        var detail = errorLocalizer[error.Code];
        var status = ToStatusCode(error.Type);
        var problem = new Microsoft.AspNetCore.Mvc.ProblemDetails
        {
            Type = $"https://httpstatuses.io/{status}",
            Status = status,
            Title = commonLocalizer[error.Type.ToString()],
            Detail = detail.ResourceNotFound ? error.Message : detail.Value,
            Instance = httpContext.Request.Path
        };
        problem.Extensions["code"] = error.Code;
        return problem;
    }

    public ObjectResult ToActionResult(HttpContext httpContext, Error error)
    {
        var problem = Create(httpContext, error);
        return new ObjectResult(problem) { StatusCode = problem.Status, ContentTypes = { ProblemContentType } };
    }

    public IActionResult ToActionResult(ControllerBase controller, Error error) =>
        ToActionResult(controller.HttpContext, error);
}
