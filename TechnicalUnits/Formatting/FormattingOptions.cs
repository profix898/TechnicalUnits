using System;
using System.ComponentModel;
using System.Globalization;

namespace TechnicalUnits.Formatting;

[TypeConverter(typeof(ExpandableObjectConverter))]
public sealed class FormattingOptions
{
    #region Default

    public static FormattingOptions Default { get; } = new FormattingOptions();

    #endregion

    [Category("Number Format")]
    [Description("Characters used as decimal separator")]
    public string DecimalSeparator { get; set; } = NumberFormatInfo.CurrentInfo.NumberDecimalSeparator;

    [Category("Number Format")]
    [Description("Characters used as group separator")]
    public string GroupSeparator { get; set; } = NumberFormatInfo.CurrentInfo.NumberGroupSeparator;

    [Category("Number Format")]
    [Description("Characters used as plus sign")]
    public string PositiveSign { get; set; } = "+";

    [Category("Number Format")]
    [Description("Characters used as minus sign")]
    public string NegativeSign { get; set; } = "-";

    [Category("Display Format")]
    [Description("Formatting style used for number output")]
    public SIStyles SIStyle { get; set; } = SIStyles.SIStyleSI;

    [Category("Display Format")]
    [Description("Use the SI prefix/unit as decimal separator, e.g. '1k5'")]
    public bool PrefixOrUnitAsDecimalSeparator { get; set; }

    [Category("Display Format")]
    [Description("(Optional) Characters before the SI prefix")]
    public string SISeparator { get; set; } = String.Empty;

    [Category("Display Format")]
    [Description("(Optional) Characters before the unit")]
    public string UnitSeparator { get; set; } = String.Empty;

    [Category("Display Format")]
    [Description("Number of non-zero digits towards which the value is rounded for display")]
    public int SignificantDigits { get; set; } = 6;

    [Category("Display Format")]
    [Description("Number of digits after the relative separator (excluding the SI prefix value) for display")]
    public int FractionalPrecision { get; set; } = 3;

    [Category("Display Format")]
    [Description("Force decimal separator even if no non-zero values follow")]
    public bool ForceDecimalSeparator { get; set; }

    [Category("Display Format")]
    [Description("Force sign even for positive values")]
    public bool ForceSign { get; set; }

    [Category("Display Format")]
    [Description("Number of characters before the decimal separator (filled with whitespace if neccesary)")]
    public int WhitePrepad { get; set; }

    [Category("Display Format")]
    [Description("Number of characters after the decimal separator (filled with whitespace if neccesary)")]
    public int WhitePostpad { get; set; }

    [Category("Display Format")]
    [Description("Number of characters before the decimal separator (filled with zeros before applying whitespace padding)")]
    public int ZeroPrepad { get; set; }

    [Category("Display Format")]
    [Description("Number of characters after the decimal separator (filled with zeros before applying whitespace padding)")]
    public int ZeroPostpad { get; set; }

    [Category("Units")]
    [Description("Force unit to lowercase if an SI prefix is preceeding it")]
    public bool AdaptCompositeUnitCase { get; set; }

    [Category("Units")]
    [Description("Enable non-greedy option for unit detection (recommended: true)")]
    public bool UnitMustBeAtEnd { get; set; } = true;

    public FormattingOptions Clone()
    {
        return new FormattingOptions
        {
            DecimalSeparator = DecimalSeparator,
            GroupSeparator = GroupSeparator,
            PositiveSign = PositiveSign,
            NegativeSign = NegativeSign,
            SIStyle = SIStyle,
            PrefixOrUnitAsDecimalSeparator = PrefixOrUnitAsDecimalSeparator,
            SISeparator = SISeparator,
            UnitSeparator = UnitSeparator,
            SignificantDigits = SignificantDigits,
            FractionalPrecision = FractionalPrecision,
            ForceDecimalSeparator = ForceDecimalSeparator,
            ForceSign = ForceSign,
            WhitePrepad = WhitePrepad,
            WhitePostpad = WhitePostpad,
            ZeroPrepad = ZeroPrepad,
            ZeroPostpad = ZeroPostpad,
            AdaptCompositeUnitCase = AdaptCompositeUnitCase,
            UnitMustBeAtEnd = UnitMustBeAtEnd
        };
    }
}