namespace Betauer.Core;

public static partial class BitTools {
    public static uint SetBit(uint bits, uint position, bool value) =>
        value ? EnableBit(bits, position) : DisableBit(bits, position);

    public static uint DisableBit(uint bits, uint position) {
        var mask = (uint)1 << (int)(position - 1);
        bits &= ~mask;
        return bits;
    }

    public static uint EnableBit(uint bits, uint position) {
        bits |= (uint)1 << (int)(position - 1);
        return bits;
    }
}