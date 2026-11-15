using Cortex.Mediator.Commands;
using Microsoft.EntityFrameworkCore;

namespace Molinex.Platform.Shared.Infrastructure.Mediator.Cortex.Configuration;

/// <summary>Logs unexpected failures with the command name and lets the global handler answer.</summary>
public class ExceptionHandlingCommandBehavior<TCommand, TResult>(
    ILogger<ExceptionHandlingCommandBehavior<TCommand, TResult>> logger)
    : ICommandPipelineBehavior<TCommand, TResult> where TCommand : ICommand<TResult>
{
    public async Task<TResult> Handle(TCommand command, CommandHandlerDelegate<TResult> next,
        CancellationToken cancellationToken)
    {
        try
        {
            return await next();
        }
        catch (OperationCanceledException)
        {
            logger.LogWarning("Command {CommandName} was cancelled.", typeof(TCommand).Name);
            throw;
        }
        catch (DbUpdateException exception)
        {
            logger.LogError(exception, "Database update error during command {CommandName}.", typeof(TCommand).Name);
            throw;
        }
        catch (Exception exception)
        {
            logger.LogError(exception, "Unhandled error during command {CommandName}.", typeof(TCommand).Name);
            throw;
        }
    }
}
