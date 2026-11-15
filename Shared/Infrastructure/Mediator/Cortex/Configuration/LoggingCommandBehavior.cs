using Cortex.Mediator.Commands;

namespace Molinex.Platform.Shared.Infrastructure.Mediator.Cortex.Configuration;

public class LoggingCommandBehavior<TCommand, TResult>(
    ILogger<LoggingCommandBehavior<TCommand, TResult>> logger)
    : ICommandPipelineBehavior<TCommand, TResult> where TCommand : ICommand<TResult>
{
    public async Task<TResult> Handle(TCommand command, CommandHandlerDelegate<TResult> next,
        CancellationToken cancellationToken)
    {
        var commandName = typeof(TCommand).Name;
        logger.LogInformation("Executing command {CommandName}", commandName);
        var result = await next();
        if (result is IOperationResult { IsFailure: true } failure)
            logger.LogInformation("Command {CommandName} failed with {ErrorCode}", commandName, failure.Error.Code);
        else
            logger.LogInformation("Executed command {CommandName}", commandName);
        return result;
    }
}
