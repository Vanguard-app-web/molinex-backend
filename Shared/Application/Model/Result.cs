namespace Molinex.Platform.Shared.Application.Model;

/// <summary>Non generic view of a result, used by pipeline behaviors.</summary>
public interface IOperationResult
{
    bool IsSuccess { get; }
    bool IsFailure { get; }
    Error Error { get; }
}

/// <summary>Outcome of an application operation: a value or an expected <see cref="Error" />.</summary>
public class Result<T> : IOperationResult
{
    private Result(bool isSuccess, T? value, Error error)
    {
        IsSuccess = isSuccess;
        Value = value;
        Error = error;
    }

    public T? Value { get; }
    public bool IsSuccess { get; }
    public bool IsFailure => !IsSuccess;
    public Error Error { get; }

    public static Result<T> Success(T value) => new(true, value, Error.None);
    public static Result<T> Failure(Error error) => new(false, default, error);

    public TResult Match<TResult>(Func<T, TResult> onSuccess, Func<Error, TResult> onFailure) =>
        IsSuccess ? onSuccess(Value!) : onFailure(Error);
}
