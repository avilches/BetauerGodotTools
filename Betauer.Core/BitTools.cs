using System;

namespace Betauer.Core;

public static partial class BitTools {
    public static int SetBit(int bits, int position, bool value) =>
        value ? EnableBit(bits, position) : DisableBit(bits, position);

    public static bool HasBit(int bits, int position) {
        var mask = 1 << (position - 1);
        return (bits & mask) == mask;
    }

    public static int DisableBit(int bits, int position) {
        var mask = 1 << (position - 1);
        bits &= ~mask;
        return bits;
    }

    public static int EnableBit(int bits, int position) {
        bits |= 1 << (position - 1);
        return bits;
    }
}