namespace Betauer.Core; 

public class BitTools {
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