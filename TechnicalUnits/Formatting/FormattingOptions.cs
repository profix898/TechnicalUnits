using System;
using System.ComponentModel;
using System.Globalization;

namespace TechnicalUnits.Formatting;

/// <summary>
/// Specifies formatting options for parsing and displaying numeric values with units.
/// </summary>
[TypeConverter(typeof(ExpandableObjectConverter))]
public sealed class FormattingOptions
{
    [Category("Units")]
    [Description("Forces unit to lowercase if an SI prefix is preceding it.")]
    public bool AdaptCompositeUnitCase { get; set; }

    #region Default

    /// <summary>
    /// Gets the default formatting options instance.
    /// </summary>
    /// <remarks>
    /// This is a shared instance. Use <see cref="Clone" /> to create a modifiable copy
    /// if you need to customize options without affecting other code.
    /// </remarks>
    public static FormattingOptions Default { get; } = new FormattingOptions();

    #endregion

    [Category("Display Format")]
    [Description("Forces decimal separator even if no non-zero values follow.")]
    public bool ForceDecimalSeparator { get; set; }

    [Category("Display Format")]
    [Description("Forces sign display even for positive values.")]
    public bool ForceSign { get; set; }

    [Category("Display Format")]
    [Description("Number of digits after the relative separator (excluding the SI prefix value) for display.")]
    public int FractionalPrecision { get; set; } = 3;

    [Category("Number Format")]
    [Description("Specifies the (culture-specific) number format (default: current UI culture).")]
    public NumberFormatInfo NumberFormat { get; set; } = CultureInfo.CurrentUICulture.NumberFormat;

    [Category("Display Format")]
    [Description("Enables the SI prefix/unit to be used as decimal separator, e.g. '1k5'.")]
    public bool PrefixOrUnitAsDecimalSeparator { get; set; }

    [Category("Display Format")]
    [Description("Number of non-zero digits towards which the value is rounded for display.")]
    public int SignificantDigits { get; set; } = 6;

    [Category("Display Format")]
    [Description("(Optional) Characters before the SI prefix (default: ' ').")]
    public string SISeparator { get; set; } = " ";

    [Category("Display Format")]
    [Description("Formatting style used for number output.")]
    public SIStyles SIStyle { get; set; } = SIStyles.SIStyleSI;

    [Category("Units")]
    [Description("Enables non-greedy option for unit detection (recommended: true).")]
    public bool UnitMustBeAtEnd { get; set; } = true;

    [Category("Display Format")]
    [Description("(Optional) Characters before the unit.")]
    public string UnitSeparator { get; set; } = String.Empty;

    [Category("Display Format")]
    [Description("Number of characters after the decimal separator (filled with whitespace if necessary).")]
    public int WhitePostpad { get; set; }

    [Category("Display Format")]
    [Description("Number of characters before the decimal separator (filled with whitespace if necessary).")]
    public int WhitePrepad { get; set; }

    [Category("Display Format")]
    [Description("Number of characters after the decimal separator (filled with zeros before applying whitespace padding).")]
    public int ZeroPostpad { get; set; }

    [Category("Display Format")]
    [Description("Number of characters before the decimal separator (filled with zeros before applying whitespace padding).")]
    public int ZeroPrepad { get; set; }

    /// <summary>
    /// Creates a shallow copy of this <see cref="FormattingOptions" /> instance.
    /// </summary>
    /// <returns>A new <see cref="FormattingOptions" /> with the same property values.</returns>
    public FormattingOptions Clone()
        => new FormattingOptions
        {
            NumberFormat = (NumberFormatInfo) NumberFormat.Clone(), SIStyle = SIStyle, PrefixOrUnitAsDecimalSeparator = PrefixOrUnitAsDecimalSeparator,
            SISeparator = SISeparator, UnitSeparator = UnitSeparator, SignificantDigits = SignificantDigits, FractionalPrecision = FractionalPrecision,
            ForceDecimalSeparator = ForceDecimalSeparator, ForceSign = ForceSign, WhitePrepad = WhitePrepad, WhitePostpad = WhitePostpad, ZeroPrepad = ZeroPrepad,
            ZeroPostpad = ZeroPostpad, AdaptCompositeUnitCase = AdaptCompositeUnitCase, UnitMustBeAtEnd = UnitMustBeAtEnd
        };
}
