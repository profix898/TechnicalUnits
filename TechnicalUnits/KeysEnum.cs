using System;

namespace TechnicalUnits;

public enum Keys
{
    Enter,
    Up,
    Down,
    M
}

[Flags]
public enum KeyModifiers
{
    None = 0x00,
    Ctrl = 0x01,
    Shift = 0x02,
    Alt = 0x04
}