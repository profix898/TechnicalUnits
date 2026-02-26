TechnicalUnits
==========
[![Nuget](https://img.shields.io/nuget/v/TechnicalUnits?style=flat-square&logo=nuget&color=blue)](https://www.nuget.org/packages/TechnicalUnits)

A .NET library for working with **SI units** and **engineering notation** - format numeric values with SI prefixes, parse user input back to base values, and define/convert between units.

## Motivation

Engineering and scientific applications frequently need to:

- **Display** values like `1.50 MHz`, `3.30 mV`, or `10.0 mT`.
- **Accept user input** in engineering notation (e.g., `"3u3"` for 3.3 µ or `"1k5"` for 1500).
- **Convert** between units (e.g., inches to meters, Celsius to Kelvin).

**TechnicalUnits** provides a small, focused library that handles all of this - with UI controls for Avalonia and WinForms.

## Features

- **Unit system** with SI base units, derived units, and dimensional analysis.
- **Formatting** of `double` values with SI prefixes (`k`, `M`, `µ`, `n`, etc.).
- **Parsing** of strings back to numeric values, including unit conversion.
- **Math expression evaluation** (optional) - evaluate expressions like `"2 * pi * 1k"`.
- **UI controls** for Avalonia and WinForms with validation and increment/decrement logic.

## Architecture

The library is organized into the following layers:

### 1. Unit Definitions (`TechnicalUnits.UnitDefinition`)

Defines the unit system:

| Type | Description |
|------|-------------|
| `Dimension` | Represents SI base dimensions (time, length, mass, current, temperature, amount of substance, luminous intensity, angle) as a set of exponents. Supports dimensional arithmetic (`*`, `/`, `^`). |
| `Unit` | A named unit with a symbol and `Dimension` (e.g., `Volt`, `Hertz`). Supports operator overloads to combine dimensions. |
| `DerivedUnit` | A unit derived from a base `Unit` via a linear conversion factor or arbitrary from-base/to-base functions (e.g., temperature offsets). |
| `UnitsLocator` | Central registry of all known `Unit` instances with lookup by name or symbol. |

**Built-in units:**

`SIUnits` provides the seven SI base units and all named SI derived units:

| Category | Units |
|----------|-------|
| **SI Base** | `Second`, `Meter`, `Kilogram`, `Ampere`, `Kelvin`, `Mol`, `Candela` |
| **SI Derived** | `Radian`, `Degree`, `Steradian`, `Hertz`, `Newton`, `Pascal`, `Joule`, `Watt`, `Coulomb`, `Volt`, `Farad`, `Ohm`, `Siemens`, `Weber`, `Tesla`, `Henry`, `Lumen`, `Lux`, `Becquerel`, `Gray`, `Sievert`, `CatalyticActivity` |
| **Compound** | `Area`, `Volume`, `Speed`, `Acceleration`, `VolumetricFlow`, `AngularVelocity` |

`Units` provides commonly used derived and non-SI units organized by category:

| Category | Units |
|----------|-------|
| **Length** | `Millimeter`, `Centimeter`, `Kilometer`, `Micrometer`, `Nanometer`, `Inch`, `Feet`, `Yard`, `Mile`, `NauticalMile`, `Mil` |
| **Mass** | `Milligram`, `Gram`, `Ounce`, `Pound`, `MetricTon`, `ShortTon`, `LongTon` |
| **Temperature** | `Celsius`, `Fahrenheit` (with non-linear conversion functions) |
| **Time** | `Minute`, `Hour`, `Day`, `Week` |
| **Force** | `KiloNewton`, `Dyne` |
| **Pressure** | `KiloPascal`, `Bar`, `Millibar`, `Atmosphere`, `Torr`, `PSI` |
| **Energy** | `Calorie`, `Kilocalorie`, `KilowattHour`, `Electronvolt`, `BTU` |
| **Speed** | `Knot`, `Mach` |
| **Volumetric** | `Liter`, `Pint`, `Quart`, `Gallon` |
| **Flow** | `LiterPerMinute` |
| **Angular Velocity** | `RevolutionsPerMinute` |
| **Electrical** | `ElectricField`, `CurrentDensity` |
| **Magnetic** | `MagneticField`, `MagneticMoment`, `MagneticGradient` |
| **Chemical** | `MassDensity`, `MassConcentration`, `MolarConcentration` |
| **Physical** | `DynamicViscosity`, `MomentOfForce`, `SurfaceTension`, `GyromagneticRatio`, `EnergyDensity` |

Units can be combined using operators to create new dimensions:

```csharp
using TechnicalUnits.UnitDefinition;

// Newton = kg·m/s²
var newton = SIUnits.Kilogram * SIUnits.Meter / (SIUnits.Second * SIUnits.Second);
```

### 2. Formatting + Parsing (`TechnicalUnits.Formatting`)

| Class | Description |
|-------|-------------|
| `Formatter` | Formats `double` values with SI prefixes, unit symbols, and configurable padding/separators. Uses multiples-of-three exponents (k, M, G, …, m, µ, n, …). |
| `Parser` | Parses strings (e.g., `"1.5 kHz"`, `"3u3"`) back to numeric values via a state-machine design driven by `ParserPartEnum`. |
| `FormattingOptions` | Controls output style, precision, separators, padding, and more. |
| `UnitOptions` | Specifies the base `Unit` and optional `AlternateUnits` (as `DerivedUnit`) for parsing. |
| `SIStyles` | Enum controlling SI prefix rendering (Unicode, ASCII, English names, or plain float). |

**Formatting styles (`SIStyles`):**

| Style | Description | Example output |
|-------|-------------|----------------|
| `SIStyleSI` | Unicode SI prefix symbols | `3.30 mV` (uses `µ` for micro) |
| `SIStyleSIAz` | ASCII-safe SI prefix symbols | `3.30 mV` (uses `u` for micro) |
| `SIStyleSINamesEN` | English SI prefix names | `3.30 milliV` |
| `SIStyleFloat` | Plain floating-point, no SI prefix | `0.0033 V` |

### 3. Extension Methods (`TechnicalUnits.Extensions`)

| Class | Description |
|-------|-------------|
| `UnitUtility` | Extension methods on `Unit` for convenient `Format`, `Parse`, and `FormatSimple` operations. |

`FormatSimple` is a lightweight formatting path with fewer options - useful when you just need quick SI-prefixed output.

### 4. Math Expression Evaluation (`TechnicalUnits.Math`)

| Class | Description |
|-------|-------------|
| `MathEvaluator` | Evaluates infix math expressions containing SI-prefixed numbers, unit symbols, built-in functions, named constants, and unit conversion brackets. Uses the Shunting-Yard algorithm. |
| `ExpressionBase` | Abstract base class for expression nodes (`NumberExpression`, `OperatorExpression`, `FunctionExpression`, `ConvertExpression`). |

### 5. UI Control Infrastructure

| Type | Description |
|------|-------------|
| `ITechnicalUnitsControl` | Interface for a control with `Value`, `Text`, `UnitOptions`, `FormattingOptions`, `EnableMath`, min/max, increment, and read-only properties. |
| `ITechnicalUnitsControlImpl` | Extended interface adding `SetValue` and `UpdateText` for control implementations. |
| `TechnicalUnitsControlExtensions` | Extension methods for `ConvertValueToText`, `ConvertTextToValue`, `ValidateLimits`. |
| `TechnicalUnitsControlMixin` | Reusable logic for increment/decrement with keyboard modifiers (None, Shift, Alt, Shift+Alt). |

## Quickstart

### Install

```sh
dotnet add package TechnicalUnits
```

### Formatting

```csharp
using TechnicalUnits.Extensions;
using TechnicalUnits.UnitDefinition;

// Full formatting with SI prefixes
string text = SIUnits.Volt.Format(0.0033);       // "3.30 mV"
string text2 = SIUnits.Hertz.Format(1500000);    // "1.50 MHz"

// Control precision
string text3 = SIUnits.Meter.Format(1.23e3, precision: 2);  // "1.23 km"
```

### Lightweight Formatting (`FormatSimple`)

A simpler API with fewer options - useful when you just need quick SI-prefixed output:

```csharp
using TechnicalUnits.Extensions;
using TechnicalUnits.UnitDefinition;

string text = SIUnits.Tesla.FormatSimple(0.010);                 // "10 mT"
string text2 = SIUnits.Tesla.FormatSimple(0.010, precision: 1);  // "10.0 mT"
```

`FormatSimple` automatically shortens trailing zeros by default (configurable via the `shortenTrailingZeros` parameter).

### Parsing

```csharp
using TechnicalUnits.Extensions;
using TechnicalUnits.UnitDefinition;

double value = SIUnits.Hertz.Parse("1.5 kHz");   // 1500.0
double value2 = SIUnits.Volt.Parse("3u3");        // 0.0000033 (SI prefix as decimal separator)
```

### Parsing with Alternate Units

```csharp
using TechnicalUnits.Extensions;
using TechnicalUnits.UnitDefinition;

// Parse "10.0in" as meters, using Inch and Mil as alternate units
double meters = SIUnits.Meter.Parse("10.0in", [Units.Inch, Units.Mil]);  // 0.254

// Parse "10.0mil" as meters
double meters2 = SIUnits.Meter.Parse("10.0mil", [Units.Inch, Units.Mil]); // 0.0000254
```

### Defining Custom Units

```csharp
using TechnicalUnits.UnitDefinition;

// Derived unit with a linear conversion factor
var centimeter = new DerivedUnit("centimeter", "cm", 0.01, SIUnits.Meter);

// Derived unit with conversion functions (for non-linear conversions)
var celsius = new DerivedUnit(
    "celsius", "°C",
    fromBaseUnitFunc: k => k - 273.15,
    toBaseUnitFunc: c => c + 273.15,
    baseUnit: SIUnits.Kelvin);

// Convert between base and derived
double kelvin = celsius.ToBase(100.0);   // 373.15
double degC = celsius.FromBase(373.15);  // 100.0
```

### Looking Up Units by Name or Symbol

```csharp
using TechnicalUnits.UnitDefinition;

Unit? volt = UnitsLocator.FindBySymbol("V");     // case-sensitive symbol lookup
Unit? meter = UnitsLocator.FindByName("length");  // case-insensitive name lookup

// Enumerate all registered units
IReadOnlyList<Unit> all = UnitsLocator.AllUnits;
```

## Math Expression Evaluation

The `MathEvaluator` class can parse and evaluate infix mathematical expressions containing SI-prefixed numbers, functions, constants, and unit conversions:

```csharp
using TechnicalUnits.Math;
using TechnicalUnits.Formatting;
using TechnicalUnits.UnitDefinition;

var evaluator = new MathEvaluator();
var unitOpts = new UnitOptions(SIUnits.Volt);
var fmtOpts = FormattingOptions.Default;

double result = evaluator.Evaluate("2 * pi * 1k", unitOpts, fmtOpts, warnings: null);
// result ≈ 6283.185...

double result2 = evaluator.Evaluate("sqrt(2) * 100m", unitOpts, fmtOpts, warnings: null);
// result2 ≈ 0.14142... (100m = 0.1 in base units)
```

When used via `ITechnicalUnitsControl` (with `EnableMath = true`), expression evaluation is enabled automatically in the UI controls.

**Unit conversions** use bracket notation inside expressions:

```
100 [°C>K]          → 373.15
```

### Built-in Constants

| Constant | Value | Description |
|----------|-------|-------------|
| `pi` | 3.14159… | Pi |
| `e` | 2.71828… | Euler's number |
| `sqrt2` | 1.41421… | Square root of 2 |
| `c` | 299 792 458 | Speed of light (m/s) |
| `g` | 9.80665 | Standard gravity (m/s²) |
| `h` | 6.626 070 15 × 10⁻³⁴ | Planck's constant (J·s) |
| `hbar` | 1.054 571 817 × 10⁻³⁴ | Reduced Planck's constant (J·s) |
| `kB` | 1.380 649 × 10⁻²³ | Boltzmann constant (J/K) |
| `NA` | 6.022 140 76 × 10²³ | Avogadro's number |
| `Gc` | 6.674 30 × 10⁻¹¹ | Gravitational constant |
| `qe` | 1.602 176 634 × 10⁻¹⁹ | Elementary charge (C) |
| `me` | 9.109 383 70 × 10⁻³¹ | Electron mass (kg) |
| `mp` | 1.672 621 92 × 10⁻²⁷ | Proton mass (kg) |
| `mn` | 1.674 927 50 × 10⁻²⁷ | Neutron mass (kg) |
| `mu0` | 1.256 637 06 × 10⁻⁶ | Vacuum permeability |
| `eps0` | 8.854 187 81 × 10⁻¹² | Vacuum permittivity (F/m) |
| `R` | 8.314 462 618 | Gas constant (J/(mol·K)) |
| `Ry` | 2.179 872 36 × 10⁻¹⁸ | Rydberg constant (J) |
| `F` | 96 485.332 12 | Faraday constant (C/mol) |
| `Vm` | 22.413 962 | Molar volume at STP (L/mol) |
| `atm` | 101 325 | Standard atmosphere (Pa) |

Constants are stored in `MathEvaluator.Constants` and can be added or removed at runtime.

### Built-in Functions

**Single-argument:** `abs`, `acos`, `asin`, `atan`, `ceiling`, `cos`, `cosh`, `exp`, `floor`, `log`, `log10`, `sin`, `sinh`, `sqrt`, `tan`, `tanh`

**Two-argument:** `max`, `min`, `pow`

Custom functions can be registered via `MathEvaluator.RegisterFunction`.

## UI Controls

The core library defines the `ITechnicalUnitsControl` interface and shared `TechnicalUnitsControlMixin` logic. Framework-specific packages provide ready-to-use controls:

### `TechnicalUnits.Avalonia`

```sh
dotnet add package TechnicalUnits.Avalonia
```

Provides `TechnicalUnitsUpDown` - an Avalonia control that integrates with Avalonia's data binding and styling. It includes a `Spinner` and `TextBox` template part.

### `TechnicalUnits.WinForms`

```sh
dotnet add package TechnicalUnits.WinForms
```

Provides:

| Type | Description |
|------|-------------|
| `TechnicalUnitsUpDown` | A WinForms `UpDownBase` control with engineering notation support. Also implements `IDataGridViewEditingControl`. |
| `TechnicalUnitsUpDownColumn` | A `DataGridView` column that hosts `TechnicalUnitsUpDown` cells. |
| `TechnicalUnitsUpDownCell` | A `DataGridView` cell for inline editing with `TechnicalUnitsUpDown`. |

### Increment/Decrement Behavior

The `TechnicalUnitsControlMixin` provides smart up/down behavior based on keyboard modifiers:

| Modifier | Up Action | Down Action |
|----------|-----------|-------------|
| None | `value += Increment` | `value -= Increment` |
| Shift | `value *= IncrementMult` | `value /= IncrementMult` |
| Alt | Increment at the position before the relative decimal separator | Decrement at the position before the relative decimal separator |
| Shift+Alt | Increment at the highest significant digit | Decrement at the highest significant digit |

## Configuration

### `FormattingOptions`

| Property | Default | Description |
|----------|---------|-------------|
| `SIStyle` | `SIStyleSI` | SI prefix style (Unicode, ASCII, English names, or plain float). |
| `FractionalPrecision` | `3` | Number of digits after the decimal separator. |
| `SignificantDigits` | `6` | Total significant digits for rounding. |
| `PrefixOrUnitAsDecimalSeparator` | `false` | Enable `1k5` syntax (SI prefix used as decimal separator). |
| `ForceSign` | `false` | Always show `+` for positive values. |
| `ForceDecimalSeparator` | `false` | Show decimal separator even when no fractional digits follow. |
| `NumberFormat` | Current UI culture | Culture-specific decimal/group separators (`NumberFormatInfo`). |
| `SISeparator` | `" "` | Characters inserted before the SI prefix. |
| `UnitSeparator` | `""` | Characters inserted before the unit symbol. |

> **Note:** `FormattingOptions.Default` is a shared singleton instance. Use `.Clone()` if you need to modify options without side effects.

### `UnitOptions`

| Property | Description |
|----------|-------------|
| `Unit` | The base `Unit` for formatting and parsing. |
| `AlternateUnits` | A list of `DerivedUnit` instances accepted during parsing (e.g., `cm`, `in` for a `Meter` base). |

`UnitOptions` supports implicit conversion from `Unit`, so you can pass a `Unit` directly where a `UnitOptions` is expected.

## License

TechnicalUnits is licensed under the terms of the MIT license (<http://opensource.org/licenses/MIT>, see LICENSE.txt).
