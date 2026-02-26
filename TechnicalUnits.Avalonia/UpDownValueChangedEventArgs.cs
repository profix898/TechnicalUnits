using Avalonia.Interactivity;

namespace TechnicalUnits.Avalonia;

/// <summary>
///     Provides data for the value changed event of a TechnicalUpDown control.
/// </summary>
public class UpDownValueChangedEventArgs : RoutedEventArgs
{
    public UpDownValueChangedEventArgs(RoutedEvent routedEvent, double? oldValue, double? newValue)
        : base(routedEvent)
    {
        OldValue = oldValue;
        NewValue = newValue;
    }

    public double? NewValue { get; }

    public double? OldValue { get; }
}
