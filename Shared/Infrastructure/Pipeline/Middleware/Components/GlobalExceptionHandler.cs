using Microsoft.AspNetCore.Diagnostics;
using Microsoft.EntityFrameworkCore;
using MySql.Data.MySqlClient;
using Molinex.Platform.Shared.Interfaces.Rest.ProblemDetails;

namespace Molinex.Platform.Shared.Infrastructure.Pipeline.Middleware.Components;

/// <summary>Answers unexpected failures with Problem Details. Expected errors never reach this handler.</summary>
public class GlobalExceptionHandler(
    ILogger<GlobalExceptionHandler> logger,
    ProblemDetailsFactory problemDetailsFactory)
    : IExceptionHandler
{
    private const int MySqlDuplicateEntry = 1062;

    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {
        Error error;
        switch (exception)
        {
            case OperationCanceledException:
                logger.LogWarning("Request was cancelled: {Path}", httpContext.Request.Path);
                error = SharedErrors.OperationCancelled;
                break;
            case DbUpdateException { InnerException: MySqlException { Number: MySqlDuplicateEntry } }:
                logger.LogWarning("Concurrent duplicate rejected by a unique index: {Path}", httpContext.Request.Path);
                error = SharedErrors.Conflict;
                break;
            default:
                logger.LogError(exception, "An unhandled exception occurred: {Message}", exception.Message);
                error = SharedErrors.UnexpectedError;
                break;
        }

        var problem = problemDetailsFactory.Create(httpContext, error);
        httpContext.Response.StatusCode = problem.Status!.Value;
        await httpContext.Response.WriteAsJsonAsync(problem, options: null,
            contentType: ProblemDetailsFactory.ProblemContentType, cancellationToken: cancellationToken);
        return true;
    }
}
