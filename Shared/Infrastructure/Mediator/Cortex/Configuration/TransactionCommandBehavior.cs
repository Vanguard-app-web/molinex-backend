using Cortex.Mediator.Commands;

namespace Molinex.Platform.Shared.Infrastructure.Mediator.Cortex.Configuration;

/// <summary>
///     Completes the unit of work once the command handler succeeds, so every change of a command
///     (aggregates and outbox messages) is written in a single database transaction.
/// </summary>
public class TransactionCommandBehavior<TCommand, TResult>(
    IUnitOfWork unitOfWork,
    ILogger<TransactionCommandBehavior<TCommand, TResult>> logger)
    : ICommandPipelineBehavior<TCommand, TResult> where TCommand : ICommand<TResult>
{
    public async Task<TResult> Handle(TCommand command, CommandHandlerDelegate<TResult> next,
        CancellationToken cancellationToken)
    {
        var result = await next();
        if (result is IOperationResult { IsFailure: true }) return result;
        logger.LogDebug("Committing unit of work for {CommandName}", typeof(TCommand).Name);
        await unitOfWork.CompleteAsync(cancellationToken);
        return result;
    }
}
