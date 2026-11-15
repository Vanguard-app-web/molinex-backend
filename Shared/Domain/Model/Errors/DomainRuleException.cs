namespace Molinex.Platform.Shared.Domain.Model.Errors;

/// <summary>
///     Raised when a value object or aggregate would be built in an invalid state.
///     Application services translate it into a failed <c>Result</c>; it never reaches the HTTP pipeline.
/// </summary>
public sealed class DomainRuleException(Error error) : ArgumentException(error.Message)
{
    public Error Error { get; } = error;
}
