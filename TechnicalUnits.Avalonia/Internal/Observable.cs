using System;
using Avalonia.Reactive;

namespace TechnicalUnits.Avalonia.Internal;

/// <summary>
/// Provides common observable methods as a replacement for the Rx framework.
/// </summary>
internal static class Observable
{
    public static IDisposable Subscribe<T>(this IObservable<T> source, Action<T> action)
    {
        return source.Subscribe(new AnonymousObserver<T>(action));
    }
}
