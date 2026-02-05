TechnicalUnits
==========
[![Nuget](https://img.shields.io/nuget/v/TechnicalUnits?style=flat-square&logo=nuget&color=blue)](https://www.nuget.org/packages/TechnicalUnits)

A .NET library for working with **SI units** and **engineering notation** — format numeric values with SI prefixes, parse user input back to base values, and define/convert between units.

---

## Motivation

Engineering and scientific applications frequently need to:

- **Display** values like `1.5 kHz`, `3.30 mV`, or `10.0 mT`.
- **Accept user input** in engineering notation (e.g., `"3u3"` for 3.3 µ or `"1k5"` for 1500).
- **Convert** between units (e.g., inches to meters, Celsius to Kelvin).

**TechnicalUnits** provides a small, focused library that handles all of this — with optional UI control helpers for Avalonia and WinForms.

---

## Features

- ?? **Unit system** with SI base units, derived units, and dimensional analysis.
- ?? **Formatting** of `double` values with SI prefixes (`k`, `M`, `µ`, `n`, etc.).
- ?? **Parsing** of strings back to numeric values, including unit conversion.
- ?? **Math expression evaluation** (optional) — evaluate expressions like `"2 * pi * 1k"`.
- ??? **UI control helpers** for Avalonia and WinForms with validation and increment/decrement logic.

---

## Architecture

The library is organized into two main conceptual layers:

### 1. Units (`TechnicalUnits.Units`)

Defines the unit system:

| Type | Description |
|------|-------------|
| `Dimension` | Represents SI base dimensions (time, length, mass, current, temperature, etc.). |
| `Unit` | A named unit with a symbol and dimensions (e.g., `Volt`, `Hertz`). |
| `DerivedUnit` | A unit derived from another with a conversion factor or function. |

**Built-in unit catalogs:**

- `SIUnits` — SI base and derived units (Meter, Second, Volt, Hertz, Tesla, etc.).
- `LengthUnits` — Inch, Feet, Yard, Mile, Mil.
- `MassUnits` — Gram, Ounce, Pound, Metric Ton.
- `TemperatureUnits` — Celsius, Fahrenheit (with conversion functions).
- `TimeUnits`, `SpeedUnits`, `VolumetricUnits`, `ElectricalUnits`, `MagneticalUnits`, `ChemicalUnits`, `PhysicalUnits`.

Units can be combined using operators:

```csharp
// Newton = kg·m/s²
var newton = SIUnits.Kilogram * SIUnits.Meter / (SIUnits.Second * SIUnits.Second);
```

### 2. Formatting + Parsing (`TechnicalUnits.Formatting`, `TechnicalUnits.Extensions`)

| Class | Description |
|-------|-------------|
| `Formatter` | Formats `double` values in engineering notation with SI prefixes. |
| `Parser` | Parses strings (e.g., `"1.5 kHz"`) back to numeric values. |
| `FormattingOptions` | Controls output style, precision, separators, etc. |
| `UnitOptions` | Specifies the base unit and alternate units for parsing. |
| `UnitUtility` | Extension methods for convenient `Format` / `Parse` / `FormatSimple`. |

**Formatting styles (`SIStyles`):**

| Style | Example output |
|-------|----------------|
| `SIStyleSI` | `3.30 mV` (Unicode µ) |
| `SIStyleSIAz` | `3.30 mV` (ASCII `u` for micro) |
| `SIStyleSINamesEN` | `3.30 milliV` |
| `SIStyleFloat` | `0.0033 V` |

---

## Quickstart

### Install

```sh
dotnet add package TechnicalUnits
```

### Formatting

```csharp
using TechnicalUnits.Extensions;
using TechnicalUnits.Units;

// Full formatting with options
string text = SIUnits.Volt.Format(0.0033);       // "3.300 mV"
string text2 = SIUnits.Hertz.Format(1500000);    // "1.500 MHz"
```

### Lightweight formatting (`FormatSimple`)

A simpler API with fewer options — useful when you just need quick SI-prefixed output:

```csharp
using TechnicalUnits.Extensions;
using TechnicalUnits.Units;

string text = SIUnits.Tesla.FormatSimple(0.010);                 // "10.0 mT"
string text2 = SIUnits.Tesla.FormatSimple(0.010, precision: 1);  // "10 mT"
```

### Parsing

```csharp
using TechnicalUnits.Extensions;
using TechnicalUnits.Units;

double value = SIUnits.Hertz.Parse("1.5 kHz");   // 1500.0
double value2 = SIUnits.Volt.Parse("3u3");       // 0.0000033 (SI prefix as decimal separator)
```

### Parsing with alternate units

```csharp
using TechnicalUnits.Extensions;
using TechnicalUnits.Units;

// Parse "10 cm" as meters
double meters = SIUnits.Meter.Parse("10 cm", new[] { LengthUnits.Centimeter }); // 0.1
```

### Defining custom units

```csharp
using TechnicalUnits.Units;

// Derived unit with conversion factor
var centimeter = new DerivedUnit("centimeter", "cm", 0.01, SIUnits.Meter);

// Derived unit with conversion functions (for non-linear conversions)
var celsius = new DerivedUnit(
    "celsius", "°C",
    fromBase: k => k - 273.15,
    toBase: c => c + 273.15,
    SIUnits.Kelvin);
```

---

## Math expression evaluation

When used via `ITechnicalUnitsControl` (with `EnableMath = true`), the library can evaluate expressions:

```
2 * pi * 1k        ? 6283.185...
sqrt(2) * 100m     ? 141.42 mV (if unit is Volt)
```

**Built-in constants:** `pi`, `e`, `sqrt2`, `c` (speed of light), `g` (gravity), `h` (Planck), `kB` (Boltzmann), `NA` (Avogadro), and more.

**Built-in functions:** `sin`, `cos`, `tan`, `sqrt`, `log`, `exp`, `abs`, `floor`, `ceil`, `round`, etc.

---

## UI control helpers

The core library includes interfaces and helpers for building UI controls:

| Type | Description |
|------|-------------|
| `ITechnicalUnitsControl` | Interface for a control with `Value`, `Text`, `UnitOptions`, `FormattingOptions`, min/max, etc. |
| `TechnicalUnitsControlExtensions` | Extension methods for `ConvertValueToText`, `ConvertTextToValue`, `ValidateLimits`. |
| `TechnicalUnitsControlMixin` | Reusable logic for increment/decrement with keyboard modifiers (Shift, Alt). |

**Framework-specific packages:**

- `TechnicalUnits.Avalonia` — Avalonia control implementation.
- `TechnicalUnits.WinForms` — WinForms control implementation.

---

## Configuration

### `FormattingOptions`

| Property | Default | Description |
|----------|---------|-------------|
| `SIStyle` | `SIStyleSI` | SI symbol style (Unicode, ASCII, or English names). |
| `FractionalPrecision` | `3` | Digits after the decimal separator. |
| `SignificantDigits` | `6` | Total significant digits for rounding. |
| `PrefixOrUnitAsDecimalSeparator` | `false` | Allow `1k5` syntax. |
| `ForceSign` | `false` | Always show `+` for positive values. |
| `NumberFormat` | Current culture | Culture-specific decimal/group separators. |

> ?? `FormattingOptions.Default` is a shared instance. Use `.Clone()` if you need to modify options without side effects.

### `UnitOptions`

| Property | Description |
|----------|-------------|
| `Unit` | The base unit for formatting/parsing. |
| `AlternateUnits` | Additional units accepted during parsing (e.g., `cm`, `in` for a `Meter` base). |

---

## Status

?? **Experimental / early-stage.** API may change. Feedback and contributions welcome!

---

## License

TechnicalUnits is licensed under the MIT license ([http://opensource.org/licenses/MIT](http://opensource.org/licenses/MIT), see LICENSE.txt).
