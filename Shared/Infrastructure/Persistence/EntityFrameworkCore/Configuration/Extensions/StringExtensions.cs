using Humanizer;

namespace Molinex.Platform.Shared.Infrastructure.Persistence.EntityFrameworkCore.Configuration.Extensions;

public static class StringExtensions
{
    public static string ToSnakeCase(this string text) => text.Underscore();
}
