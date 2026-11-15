namespace Molinex.Platform.Shared.Domain.Model;

/// <summary>Invariant checks shared by value objects and aggregates.</summary>
public static class Guard
{
    public static void Against(bool invalid, Error error)
    {
        if (invalid) throw new DomainRuleException(error);
    }

    public static string RequiredText(string? value, int maxLength, Error error)
    {
        var text = value?.Trim();
        Against(string.IsNullOrEmpty(text) || text.Length > maxLength, error);
        return text!;
    }

    public static string? OptionalText(string? value, int maxLength, Error error)
    {
        var text = value?.Trim();
        if (string.IsNullOrEmpty(text)) return null;
        Against(text.Length > maxLength, error);
        return text;
    }

    /// <summary>Rejects missing dates and normalizes the instant to UTC.</summary>
    public static DateTime UtcInstant(DateTime value, Error error)
    {
        Against(value == default, error);
        return value.Kind switch
        {
            DateTimeKind.Utc => value,
            DateTimeKind.Local => value.ToUniversalTime(),
            _ => DateTime.SpecifyKind(value, DateTimeKind.Utc)
        };
    }
}
