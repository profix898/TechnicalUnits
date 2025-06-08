using System;
using System.Collections.Generic;
using System.Text;

namespace TechnicalUnits.Internal;

public static class ExceptionFormatter
{
    public static IEnumerable<string> FormatExceptionsToEnumerable(this IList<Exception> exceptions)
    {
        if (exceptions.Count == 0)
            return [];

        var exceptionsStrings = new List<string>();
        for (var i = 0; i < exceptions.Count; i++)
            exceptionsStrings.Add(FormatExceptionString(exceptions[i], i, exceptions.Count));

        return exceptionsStrings;
    }

    public static string FormatExceptionsToString(this IList<Exception> exceptions)
    {
        if (exceptions.Count == 0)
            return String.Empty;

        var stringBuilder = new StringBuilder();
        for (var i = 0; i < exceptions.Count; i++)
        {
            stringBuilder.Append(FormatExceptionString(exceptions[i], i, exceptions.Count));

            if (i < exceptions.Count - 1)
                stringBuilder.Append(" | ");
        }

        return stringBuilder.ToString();
    }

    #region Private

    private static string FormatExceptionString(Exception ex, int i, int count)
    {
        var exString = count > 1 ? $"[{i}] {ex.GetType().Name}: {ex.Message}" : $"{ex.GetType().Name}: {ex.Message}";

        // Inner exception
        if (ex.InnerException != null)
            exString += $" (Inner: {ex.InnerException.GetType().Name}: {ex.InnerException.Message})";

        return exString;
    }

    #endregion
}
