using System;
using System.Collections.Generic;
using System.Text;

namespace TechnicalUnits.Internal;

/// <summary>
/// Extension methods that format a list of <see cref="Exception" /> instances into
/// human-readable strings (used for parser warning output).
/// </summary>
public static class ExceptionFormatter
{
    /// <summary>
    /// Formats each exception as a numbered string and returns them as an enumerable.
    /// </summary>
    /// <param name="exceptions">The list of exceptions to format.</param>
    /// <returns>An enumerable of formatted exception strings, or an empty sequence when the list is empty.</returns>
    public static IEnumerable<string> FormatExceptionsToEnumerable(this IList<Exception> exceptions)
    {
        if (exceptions.Count == 0)
            return [];

        var exceptionsStrings = new List<string>(exceptions.Count);
        for (var i = 0; i < exceptions.Count; i++)
            exceptionsStrings.Add(FormatExceptionString(exceptions[i], i, exceptions.Count));

        return exceptionsStrings;
    }

    /// <summary>
    /// Formats all exceptions into a single pipe-separated (<c>" | "</c>) string.
    /// </summary>
    /// <param name="exceptions">The list of exceptions to format.</param>
    /// <returns>A concatenated string, or <see cref="String.Empty" /> when the list is empty.</returns>
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
