using System;

namespace Betauer.Core.PCG.GridTemplate;

[Flags]
public enum DirectionFlag : byte {
    None      = 0,
    Up        = 1 << 0, // 00000001
    UpRight   = 1 << 1, // 00000010
    Right     = 1 << 2, // 00000100
    DownRight = 1 << 3, // 00001000
    Down      = 1 << 4, // 00010000
    DownLeft  = 1 << 5, // 00100000
    Left      = 1 << 6, // 01000000
    UpLeft    = 1 << 7, // 10000000
}