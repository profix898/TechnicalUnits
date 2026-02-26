using System;
using System.Collections.Generic;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Metadata;
using Avalonia.Controls.Primitives;
using Avalonia.Data;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Layout;
using Avalonia.Media;
using Avalonia.Threading;
using TechnicalUnits.Avalonia.Internal;
using TechnicalUnits.Formatting;
using TechnicalUnits.Internal;
using TechnicalUnits.UnitDefinition;

namespace TechnicalUnits.Avalonia;

/// <summary>
/// Represents a specialized numeric up-down control that supports engineering notation
/// and unit formatting. This control allows users to input and manipulate numeric values
/// with advanced formatting and validation options.
/// </summary>
/// <remarks>
/// The <see cref="TechnicalUnitsUpDown" /> control mimics the functionality of the Avalonia
/// <see cref="NumericUpDown" /> providing additional support for engineering notation and unit
/// formatting. It integrates seamlessly with Avalonia's data binding and styling systems,
/// making it suitable for modern cross-platform applications.
/// 
/// This control is designed for scenarios requiring precise numeric input and display,
/// such as engineering, scientific, or technical applications. It offers features like
/// customizable formatting options, unit handling, and value constraints, ensuring
/// flexibility and accuracy in numeric input.
/// </remarks>
[TemplatePart("PART_Spinner", typeof(Spinner))]
[TemplatePart("PART_TextBox", typeof(TextBox), IsRequired = true)]
public class TechnicalUnitsUpDown : TemplatedControl, ITechnicalUnitsControl, ITechnicalUnitsControlImpl
{
    private readonly TechnicalUnitsControlMixin _mixin;
    private bool _hasTextChanged;

    private bool _isInternalUpdate;

    private IDisposable? _textBoxTextChangedSubscription;

    /// <summary>
    /// Initializes static members of the <see cref="TechnicalUnitsUpDown" /> class.
    /// </summary>
    static TechnicalUnitsUpDown()
    {
        FormattingOptionsProperty.Changed.Subscribe(OnFormattingOptionsChanged);
        UnitOptionsProperty.Changed.Subscribe(OnUnitOptionsChanged);
        EnableMathProperty.Changed.Subscribe(OnEnableMathChanged);
        MinimumProperty.Changed.Subscribe(OnMinimumChanged);
        MaximumProperty.Changed.Subscribe(OnMaximumChanged);
        ClipValueToMinMaxProperty.Changed.Subscribe(OnClipValueToMinMaxChanged);
        IncrementProperty.Changed.Subscribe(OnIncrementChanged);
        IncrementMultProperty.Changed.Subscribe(OnIncrementMultChanged);
        IsReadOnlyProperty.Changed.Subscribe(OnIsReadOnlyChanged);
        ValueProperty.Changed.Subscribe(OnValueChanged);
        TextProperty.Changed.Subscribe(OnTextChanged);

        FocusableProperty.OverrideDefaultValue<TechnicalUnitsUpDown>(true);
        IsTabStopProperty.OverrideDefaultValue<TechnicalUnitsUpDown>(false);
    }

    /// <summary>
    /// Initializes new instance of <see cref="TechnicalUnitsUpDown" /> class.
    /// </summary>
    public TechnicalUnitsUpDown()
    {
        _mixin = new TechnicalUnitsControlMixin(this);

        Initialized += (_, _) =>
        {
            if (IsInitialized)
                UpdateText();

            SetValidSpinDirection();
        };
    }

    /// <summary>
    /// Gets the Spinner template part.
    /// </summary>
    private Spinner? Spinner { get; set; }

    /// <summary>
    /// Gets the TextBox template part.
    /// </summary>
    private TextBox? TextBox { get; set; }

    /// <summary>
    /// Called when the <see cref="Value" /> property has to be coerced before being set.
    /// </summary>
    /// <param name="value">The value to be set, before coercion.</param>
    /// <returns>
    /// The value to actually set on the property, after applying any constraints or adjustments.
    /// </returns>
    /// <remarks>
    /// Coercion allows you to enforce additional rules or constraints on the value before it is accepted by the control.
    /// For example, you can clamp the value to a minimum or maximum, round it, or reject invalid input.
    /// Override this method in a derived class to implement custom value validation or transformation logic.
    /// By default, this method returns the input value unchanged.
    /// </remarks>
    protected virtual double OnCoerceValue(double value) => value; // Standard validation (min/max) inside mixin, so just return the value.

    #region Options

    /// <summary>
    /// Defines the <see cref="FormattingOptions" /> property
    /// </summary>
    public static readonly StyledProperty<FormattingOptions> FormattingOptionsProperty =
        AvaloniaProperty.Register<TechnicalUnitsUpDown, FormattingOptions>(nameof(FormattingOptions), new FormattingOptions());

    /// <summary>
    /// Defines the <see cref="UnitOptions" /> property
    /// </summary>
    public static readonly StyledProperty<UnitOptions> UnitOptionsProperty = AvaloniaProperty.Register<TechnicalUnitsUpDown, UnitOptions>(nameof(UnitOptions), new UnitOptions());

    /// <summary>
    /// Defines the <see cref="EnableMath" /> property
    /// </summary>
    public static readonly StyledProperty<bool> EnableMathProperty = AvaloniaProperty.Register<TechnicalUnitsUpDown, bool>(nameof(EnableMath));

    /// <summary>
    /// Defines the <see cref="MathModeIcon" /> property
    /// </summary>
    public static readonly StyledProperty<object?> MathModeIconProperty = AvaloniaProperty.Register<TechnicalUnitsUpDown, object?>(nameof(MathModeIcon), new TextBlock
    {
        // Default math mode icon (bold text "π")
        Text = "π", FontWeight = FontWeight.Bold, VerticalAlignment = VerticalAlignment.Center, Margin = new Thickness(5)
    });

    #endregion

    #region MinMax

    /// <summary>
    /// Defines the <see cref="Minimum" /> property
    /// </summary>
    public static readonly StyledProperty<double> MinimumProperty = AvaloniaProperty.Register<TechnicalUnitsUpDown, double>(nameof(Minimum), Double.MinValue);

    /// <summary>
    /// Defines the <see cref="Maximum" /> property
    /// </summary>
    public static readonly StyledProperty<double> MaximumProperty = AvaloniaProperty.Register<TechnicalUnitsUpDown, double>(nameof(Maximum), Double.MaxValue);

    /// <summary>
    /// Defines the <see cref="ClipValueToMinMax" /> property
    /// </summary>
    public static readonly StyledProperty<bool> ClipValueToMinMaxProperty = AvaloniaProperty.Register<TechnicalUnitsUpDown, bool>(nameof(ClipValueToMinMax), true);

    #endregion

    #region UpDown

    /// <summary>
    /// Defines the <see cref="Increment" /> property
    /// </summary>
    public static readonly StyledProperty<double> IncrementProperty = AvaloniaProperty.Register<TechnicalUnitsUpDown, double>(nameof(Increment), 1.0);

    /// <summary>
    /// Defines the <see cref="IncrementMult" /> property
    /// </summary>
    public static readonly StyledProperty<double> IncrementMultProperty = AvaloniaProperty.Register<TechnicalUnitsUpDown, double>(nameof(IncrementMult), 10.0);

    /// <summary>
    /// Defines the <see cref="IsReadOnly" /> property
    /// </summary>
    public static readonly StyledProperty<bool> IsReadOnlyProperty = AvaloniaProperty.Register<TechnicalUnitsUpDown, bool>(nameof(IsReadOnly));

    /// <summary>
    /// Defines the <see cref="Value" /> property
    /// </summary>
    public static readonly StyledProperty<double> ValueProperty =
        AvaloniaProperty.Register<TechnicalUnitsUpDown, double>(nameof(Value), 1000.0, coerce: (s, v) => ((TechnicalUnitsUpDown) s).OnCoerceValue(v),
                                                                defaultBindingMode: BindingMode.TwoWay, enableDataValidation: true);

    /// <summary>
    /// Defines the <see cref="ValueChanged" /> event.
    /// </summary>
    public static readonly RoutedEvent<NumericUpDownValueChangedEventArgs> ValueChangedEvent =
        RoutedEvent.Register<TechnicalUnitsUpDown, NumericUpDownValueChangedEventArgs>(nameof(ValueChanged), RoutingStrategies.Bubble);

    /// <summary>
    /// Defines the <see cref="Text" /> property
    /// </summary>
    public static readonly StyledProperty<string> TextProperty =
        AvaloniaProperty.Register<TechnicalUnitsUpDown, string>(nameof(Text), String.Empty, defaultBindingMode: BindingMode.TwoWay, enableDataValidation: true);

    #endregion

    #region AvaloniaNumericUpDown

    /// <summary>
    /// Defines the <see cref="AllowSpin" /> property.
    /// </summary>
    public static readonly StyledProperty<bool> AllowSpinProperty = ButtonSpinner.AllowSpinProperty.AddOwner<TechnicalUnitsUpDown>();

    /// <summary>
    /// Defines the <see cref="ButtonSpinnerLocation" /> property.
    /// </summary>
    public static readonly StyledProperty<Location> ButtonSpinnerLocationProperty = ButtonSpinner.ButtonSpinnerLocationProperty.AddOwner<TechnicalUnitsUpDown>();

    /// <summary>
    /// Defines the <see cref="ShowButtonSpinner" /> property.
    /// </summary>
    public static readonly StyledProperty<bool> ShowButtonSpinnerProperty = ButtonSpinner.ShowButtonSpinnerProperty.AddOwner<TechnicalUnitsUpDown>();

    /// <summary>
    /// Defines the <see cref="Watermark" /> property.
    /// </summary>
    public static readonly StyledProperty<string?> WatermarkProperty = AvaloniaProperty.Register<TechnicalUnitsUpDown, string?>(nameof(Watermark));

    /// <summary>
    /// Defines the <see cref="HorizontalContentAlignment" /> property.
    /// </summary>
    public static readonly StyledProperty<HorizontalAlignment> HorizontalContentAlignmentProperty =
        ContentControl.HorizontalContentAlignmentProperty.AddOwner<TechnicalUnitsUpDown>();

    /// <summary>
    /// Defines the <see cref="VerticalContentAlignment" /> property.
    /// </summary>
    public static readonly StyledProperty<VerticalAlignment> VerticalContentAlignmentProperty = ContentControl.VerticalContentAlignmentProperty.AddOwner<TechnicalUnitsUpDown>();

    /// <summary>
    /// Defines the <see cref="TextAlignment" /> property
    /// </summary>
    public static readonly StyledProperty<TextAlignment> TextAlignmentProperty = TextBox.TextAlignmentProperty.AddOwner<TechnicalUnitsUpDown>();

    /// <summary>
    /// Defines the <see cref="InnerLeftContent" /> property
    /// </summary>
    public static readonly StyledProperty<object?> InnerLeftContentProperty = TextBox.InnerLeftContentProperty.AddOwner<TechnicalUnitsUpDown>();

    /// <summary>
    /// Defines the <see cref="InnerRightContent" /> property
    /// </summary>
    public static readonly StyledProperty<object?> InnerRightContentProperty = TextBox.InnerRightContentProperty.AddOwner<TechnicalUnitsUpDown>();

    #endregion

    #region StaticPropertyChangedHandlers

    /// <summary>Called when the <see cref="FormattingOptions" /> property value changed.</summary>
    private static void OnFormattingOptionsChanged(AvaloniaPropertyChangedEventArgs e)
    {
        if (e.Sender is TechnicalUnitsUpDown upDown)
            upDown.OnFormattingOptionsChanged((FormattingOptions?) e.OldValue, (FormattingOptions?) e.NewValue);
    }

    /// <summary>Called when the <see cref="EnableMath" /> property value changed.</summary>
    private static void OnEnableMathChanged(AvaloniaPropertyChangedEventArgs e)
    {
        if (e.Sender is TechnicalUnitsUpDown upDown)
            upDown.OnEnableMathChanged((bool?) e.OldValue, (bool?) e.NewValue);
    }

    /// <summary>Called when the <see cref="UnitOptions" /> property value changed.</summary>
    private static void OnUnitOptionsChanged(AvaloniaPropertyChangedEventArgs e)
    {
        if (e.Sender is TechnicalUnitsUpDown upDown)
            upDown.OnUnitOptionsChanged((UnitOptions?) e.OldValue, (UnitOptions?) e.NewValue);
    }

    /// <summary>Called when the <see cref="Minimum" /> property value changed.</summary>
    private static void OnMinimumChanged(AvaloniaPropertyChangedEventArgs e)
    {
        if (e.Sender is TechnicalUnitsUpDown upDown)
            upDown.OnMinimumChanged((double?) e.OldValue, (double?) e.NewValue);
    }

    /// <summary>Called when the <see cref="Maximum" /> property value changed.</summary>
    private static void OnMaximumChanged(AvaloniaPropertyChangedEventArgs e)
    {
        if (e.Sender is TechnicalUnitsUpDown upDown)
            upDown.OnMaximumChanged((double?) e.OldValue, (double?) e.NewValue);
    }

    /// <summary>Called when the <see cref="ClipValueToMinMax" /> property value changed.</summary>
    private static void OnClipValueToMinMaxChanged(AvaloniaPropertyChangedEventArgs e)
    {
        if (e.Sender is TechnicalUnitsUpDown upDown)
            upDown.OnClipValueToMinMaxChanged((bool?) e.OldValue, (bool?) e.NewValue);
    }

    /// <summary>Called when the <see cref="Increment" /> property value changed.</summary>
    private static void OnIncrementChanged(AvaloniaPropertyChangedEventArgs e)
    {
        if (e.Sender is TechnicalUnitsUpDown upDown)
            upDown.OnIncrementChanged((double?) e.OldValue, (double?) e.NewValue);
    }

    /// <summary>Called when the <see cref="IncrementMult" /> property value changed.</summary>
    private static void OnIncrementMultChanged(AvaloniaPropertyChangedEventArgs e)
    {
        if (e.Sender is TechnicalUnitsUpDown upDown)
            upDown.OnIncrementMultChanged((double?) e.OldValue, (double?) e.NewValue);
    }

    /// <summary>Called when the <see cref="IsReadOnly" /> property value changed.</summary>
    private static void OnIsReadOnlyChanged(AvaloniaPropertyChangedEventArgs e)
    {
        if (e.Sender is TechnicalUnitsUpDown upDown)
            upDown.OnIsReadOnlyChanged((bool?) e.OldValue, (bool?) e.NewValue);
    }

    /// <summary>Called when the <see cref="Value" /> property value changed.</summary>
    private static void OnValueChanged(AvaloniaPropertyChangedEventArgs e)
    {
        if (e.Sender is TechnicalUnitsUpDown upDown)
            upDown.OnValueChanged((double?) e.OldValue, (double?) e.NewValue);
    }

    /// <summary>Called when the <see cref="Text" /> property value changed.</summary>
    private static void OnTextChanged(AvaloniaPropertyChangedEventArgs e)
    {
        if (e.Sender is TechnicalUnitsUpDown upDown)
            upDown.OnTextChanged((string?) e.OldValue, (string?) e.NewValue);
    }

    #endregion

    #region Options

    /// <summary>
    /// Specifies the formatting options and culture info for parsing and displaying values.
    /// </summary>
    public FormattingOptions FormattingOptions
    {
        get => GetValue(FormattingOptionsProperty);
        set => SetValue(FormattingOptionsProperty, value);
    }

    /// <summary>
    /// Specifies the unit options for parsing and displaying values.
    /// </summary>
    public UnitOptions UnitOptions
    {
        get => GetValue(UnitOptionsProperty);
        set => SetValue(UnitOptionsProperty, value);
    }

    /// <summary>
    /// Specifies the unit for parsing and displaying values (simplified alias for <see cref="UnitOptions" />).
    /// </summary>
    public Unit Unit
    {
        get => GetValue(UnitOptionsProperty).Unit;
        set => SetValue(UnitOptionsProperty, value);
    }

    /// <summary>
    /// Indicates whether math expressions are enabled.
    /// </summary>
    public bool EnableMath
    {
        get => GetValue(EnableMathProperty);
        set => SetValue(EnableMathProperty, value);
    }

    /// <summary>
    /// Indicates whether math expressions are enabled.
    /// </summary>
    public object? MathModeIcon
    {
        get => GetValue(MathModeIconProperty);
        set => SetValue(MathModeIconProperty, value);
    }

    #endregion

    #region MinMax

    /// <summary>
    /// Specifies the minimum allowed value (set 'EnableLimits' option to enforce).
    /// </summary>
    public double Minimum
    {
        get => GetValue(MinimumProperty);
        set => SetValue(MinimumProperty, value);
    }

    /// <summary>
    /// Specifies the maxium allowed value (set 'EnableLimits' option to enforce).
    /// </summary>
    public double Maximum
    {
        get => GetValue(MaximumProperty);
        set => SetValue(MaximumProperty, value);
    }

    /// <summary>
    /// Indicates whether to clip the value to the min/max bounds.
    /// </summary>
    public bool ClipValueToMinMax
    {
        get => GetValue(ClipValueToMinMaxProperty);
        set => SetValue(ClipValueToMinMaxProperty, value);
    }

    #endregion

    #region UpDown

    /// <summary>
    /// Specifies the increment value for up/down operations. Determines the amount by which the value is increased/decreased when the up/down button is pressed.
    /// </summary>
    public double Increment
    {
        get => GetValue(IncrementProperty);
        set => SetValue(IncrementProperty, value);
    }

    /// <summary>
    /// Specifies the multiplicative increment for value on up/down. Determines the amount by which the value is multiplied/divided when the up/down button is pressed (while the Shift key
    /// is pressed).
    /// </summary>
    public double IncrementMult
    {
        get => GetValue(IncrementMultProperty);
        set => SetValue(IncrementMultProperty, value);
    }

    /// <summary>
    /// Indicates whether the control is read-only. When set to true, the value cannot be edited by the user.
    /// </summary>
    public bool IsReadOnly
    {
        get => GetValue(IsReadOnlyProperty);
        set => SetValue(IsReadOnlyProperty, value);
    }

    /// <summary>
    /// Specifies the (current) numeric value of the control.
    /// </summary>
    public double Value
    {
        get => GetValue(ValueProperty);
        set => SetValue(ValueProperty, value);
    }

    /// <summary>
    /// Raised when the <see cref="Value" /> changes.
    /// </summary>
    public event EventHandler<NumericUpDownValueChangedEventArgs>? ValueChanged
    {
        add => AddHandler(ValueChangedEvent, value);
        remove => RemoveHandler(ValueChangedEvent, value);
    }

    /// <summary>
    /// Gets the formatted text representation of the current value.
    /// </summary>
    public string Text => GetValue(TextProperty);

    #endregion

    #region AvaloniaNumericUpDown

    /// <summary>
    /// Gets or sets the ability to perform increment/decrement operations via the keyboard, button spinners, or mouse wheel.
    /// </summary>
    public bool AllowSpin
    {
        get => GetValue(AllowSpinProperty);
        set => SetValue(AllowSpinProperty, value);
    }

    /// <summary>
    /// Gets or sets current location of the <see cref="ButtonSpinner" />.
    /// </summary>
    public Location ButtonSpinnerLocation
    {
        get => GetValue(ButtonSpinnerLocationProperty);
        set => SetValue(ButtonSpinnerLocationProperty, value);
    }

    /// <summary>
    /// Gets or sets a value indicating whether the spin buttons should be shown.
    /// </summary>
    public bool ShowButtonSpinner
    {
        get => GetValue(ShowButtonSpinnerProperty);
        set => SetValue(ShowButtonSpinnerProperty, value);
    }

    /// <summary>
    /// Gets or sets the object to use as a watermark if the <see cref="Value" /> is null.
    /// </summary>
    public string? Watermark
    {
        get => GetValue(WatermarkProperty);
        set => SetValue(WatermarkProperty, value);
    }

    /// <summary>
    /// Gets or sets the horizontal alignment of the content within the control.
    /// </summary>
    public HorizontalAlignment HorizontalContentAlignment
    {
        get => GetValue(HorizontalContentAlignmentProperty);
        set => SetValue(HorizontalContentAlignmentProperty, value);
    }

    /// <summary>
    /// Gets or sets the vertical alignment of the content within the control.
    /// </summary>
    public VerticalAlignment VerticalContentAlignment
    {
        get => GetValue(VerticalContentAlignmentProperty);
        set => SetValue(VerticalContentAlignmentProperty, value);
    }

    /// <summary>
    /// Gets or sets the <see cref="Avalonia.Media.TextAlignment" /> of the <see cref="TechnicalUnitsUpDown" />
    /// </summary>
    public TextAlignment TextAlignment
    {
        get => GetValue(TextAlignmentProperty);
        set => SetValue(TextAlignmentProperty, value);
    }

    /// <summary>
    /// Gets or sets custom content that is positioned on the left side of the text layout box
    /// </summary>
    public object? InnerLeftContent
    {
        get => GetValue(InnerLeftContentProperty);
        set => SetValue(InnerLeftContentProperty, value);
    }

    /// <summary>
    /// Gets or sets custom content that is positioned on the right side of the text layout box
    /// </summary>
    public object? InnerRightContent
    {
        get => GetValue(InnerRightContentProperty);
        set => SetValue(InnerRightContentProperty, value);
    }

    #endregion

    #region PropertyChangedHandlers

    /// <summary>Called when the <see cref="FormattingOptions" /> property value changed.</summary>
    protected virtual void OnFormattingOptionsChanged(FormattingOptions? oldValue, FormattingOptions? newValue)
    {
        if (!IsInitialized)
            return;

        UpdateText();
    }

    /// <summary>Called when the <see cref="UnitOptions" /> property value changed.</summary>
    protected virtual void OnUnitOptionsChanged(UnitOptions? oldValue, UnitOptions? newValue)
    {
        if (!IsInitialized)
            return;

        UpdateText();
    }

    /// <summary>Called when the <see cref="EnableMath" /> property value changed.</summary>
    protected virtual void OnEnableMathChanged(bool? oldValue, bool? newValue)
    {
        if (!IsInitialized)
            return;

        InnerRightContent = EnableMath ? MathModeIcon : null;
    }

    /// <summary>Called when the <see cref="Minimum" /> property value changed.</summary>
    protected virtual void OnMinimumChanged(double? oldValue, double? newValue)
    {
        if (!IsInitialized)
            return;

        _mixin.TryUpdateValue();
    }

    /// <summary>Called when the <see cref="Maximum" /> property value changed.</summary>
    protected virtual void OnMaximumChanged(double? oldValue, double? newValue)
    {
        if (!IsInitialized)
            return;

        _mixin.TryUpdateValue();
    }

    /// <summary>Called when the <see cref="ClipValueToMinMax" /> property value changed.</summary>
    protected virtual void OnClipValueToMinMaxChanged(bool? oldValue, bool? newValue)
    {
        if (!IsInitialized)
            return;

        _mixin.TryUpdateValue();
    }

    /// <summary>Called when the <see cref="Increment" /> property value changed.</summary>
    protected virtual void OnIncrementChanged(double? oldValue, double? newValue)
    {
        if (!IsInitialized)
            return;

        SetValidSpinDirection();
    }

    /// <summary>Called when the <see cref="IncrementMult" /> property value changed.</summary>
    protected virtual void OnIncrementMultChanged(double? oldValue, double? newValue)
    {
        if (!IsInitialized)
            return;

        SetValidSpinDirection();
    }

    /// <summary>Called when the <see cref="IsReadOnly" /> property value changed.</summary>
    protected virtual void OnIsReadOnlyChanged(bool? oldValue, bool? newValue)
    {
        if (!IsInitialized)
            return;

        SetValidSpinDirection();
    }

    /// <summary>Called when the <see cref="Value" /> property value changed.</summary>
    protected virtual void OnValueChanged(double? oldValue, double? newValue)
    {
        if (!IsInitialized)
            return;

        if (!_isInternalUpdate)
            UpdateText();

        RaiseEvent(new UpDownValueChangedEventArgs(ValueChangedEvent, oldValue, newValue));
    }

    /// <summary>Called when the <see cref="Text" /> property value changed.</summary>
    protected virtual void OnTextChanged(string? oldValue, string? newValue)
    {
        if (!IsInitialized || _isInternalUpdate)
            return;

        _mixin.TryUpdateValue();
    }

    #endregion

    #region ITechnicalUnitsControlImpl Members

    /// <inheritdoc />
    public void SetValue(double value, List<Exception>? errors = null)
    {
        if (!IsInitialized)
            return;

        // Set the value and update the text representation

        _isInternalUpdate = true;

        SetCurrentValue(ValueProperty, value);
        SetValidSpinDirection();

        _isInternalUpdate = false;

        UpdateText();

        // Show any validation errors in tooltip
        if (errors != null && errors.Count > 0)
            DataValidationErrors.SetErrors(this, errors.FormatExceptionsToEnumerable());
        else
            DataValidationErrors.ClearErrors(this);
    }

    /// <inheritdoc />
    public void UpdateText()
    {
        _isInternalUpdate = true;

        SetCurrentValue(TextProperty, this.ConvertValueToText(Value));
        if (TextBox != null)
            TextBox.Text = Text; // Update TextBox with the new text value

        _isInternalUpdate = false;
        _hasTextChanged = false;
    }

    #endregion

    #region Overrides

    protected override Type StyleKeyOverride => typeof(NumericUpDown);

    /// <inheritdoc />
    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        if (TextBox != null)
        {
            TextBox.PointerPressed -= TextBoxOnPointerPressed;
            _textBoxTextChangedSubscription?.Dispose();
        }
        TextBox = e.NameScope.Find<TextBox>("PART_TextBox");
        if (TextBox != null)
        {
            TextBox.Text = Text;
            TextBox.PointerPressed += TextBoxOnPointerPressed;
            _textBoxTextChangedSubscription = TextBox.GetObservable(TextBox.TextProperty).Subscribe(_ => TextBoxOnTextChanged());
        }

        if (Spinner != null)
            Spinner.Spin -= SpinnerOnSpin;
        Spinner = e.NameScope.Find<Spinner>("PART_Spinner");
        if (Spinner != null)
            Spinner.Spin += SpinnerOnSpin;

        SetValidSpinDirection();
    }

    /// <inheritdoc />
    protected override void OnLostFocus(RoutedEventArgs e)
    {
        if (_hasTextChanged)
            _mixin.TryUpdateValue();

        base.OnLostFocus(e);
    }

    /// <inheritdoc />
    protected override void OnKeyDown(KeyEventArgs e)
    {
        if (e.Key == Key.Enter)
        {
            _mixin.TryUpdateValue();
            e.Handled = true;
        }
        else
        {
            if (_mixin.OnKeyDown(MapKeys(e.Key), MapKeyModifiers(e.Key)))
                e.Handled = true;
        }
    }

    /// <inheritdoc />
    protected override void OnKeyUp(KeyEventArgs e)
    {
        if (_mixin.OnKeyUp(MapKeyModifiers(e.Key)))
            e.Handled = true;
    }

    /// <inheritdoc />
    protected override void UpdateDataValidation(AvaloniaProperty property, BindingValueType state, Exception? error)
    {
        if (property == TextProperty || property == ValueProperty)
            DataValidationErrors.SetError(this, error);
    }

    #endregion

    #region Private

    private void TextBoxOnPointerPressed(object? sender, PointerPressedEventArgs e)
    {
        if (e.Pointer.Captured != Spinner)
            Dispatcher.UIThread.InvokeAsync(() => { e.Pointer.Capture(Spinner); }, DispatcherPriority.Input);
    }

    private void TextBoxOnTextChanged()
    {
        if (TextBox == null)
            return;

        _isInternalUpdate = true;

        SetCurrentValue(TextProperty, TextBox.Text ?? String.Empty);

        _isInternalUpdate = false;
        _hasTextChanged = true;
    }

    private void SpinnerOnSpin(object? sender, SpinEventArgs e)
    {
        if (!AllowSpin || IsReadOnly)
            return;

        var spin = !e.UsingMouseWheel;
        spin |= TextBox != null && TextBox.IsFocused;
        if (!spin)
            return;

        if (e.Direction == SpinDirection.Increase)
        {
            if (Spinner == null || (Spinner.ValidSpinDirection & ValidSpinDirections.Increase) == ValidSpinDirections.Increase)
                _mixin.OnUpButton();
        }
        else
        {
            if (Spinner == null || (Spinner.ValidSpinDirection & ValidSpinDirections.Decrease) == ValidSpinDirections.Decrease)
                _mixin.OnDownButton();
        }

        e.Handled = true;
    }

    /// <summary>
    /// Sets the valid spin directions.
    /// </summary>
    private void SetValidSpinDirection()
    {
        var validDirections = ValidSpinDirections.None;

        // Zero increment always prevents spin.
        if (Increment != 0 && !IsReadOnly)
        {
            if (Value < Maximum)
                validDirections |= ValidSpinDirections.Increase;

            if (Value > Minimum)
                validDirections |= ValidSpinDirections.Decrease;
        }

        if (Spinner != null)
            Spinner.ValidSpinDirection = validDirections;
    }

    private static Keys? MapKeys(Key keyCode)
    {
        return keyCode switch
        {
            Key.Enter => Keys.Enter,
            Key.Up => Keys.Up,
            Key.Down => Keys.Down,
            Key.M => Keys.M,
            _ => null
        };
    }

    private static KeyModifiers MapKeyModifiers(Key keyCode)
    {
        var mappedModifiers = KeyModifiers.None;

        if (keyCode == Key.LeftCtrl || keyCode == Key.RightCtrl)
            mappedModifiers |= KeyModifiers.Ctrl;
        if (keyCode == Key.LeftShift || keyCode == Key.RightShift)
            mappedModifiers |= KeyModifiers.Shift;
        if (keyCode == Key.LeftAlt || keyCode == Key.RightAlt)
            mappedModifiers |= KeyModifiers.Alt;

        return mappedModifiers;
    }

    #endregion
}
