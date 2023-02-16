using Godot;

namespace Betauer;

public class PcgRandom {
    private RandomNumberGenerator random;

    public PcgRandom() {
        random = new();
        random.Randomize();
    }

    public PcgRandom(ulong seed) {
        random = new(){
            Seed = seed
        };
    }

    public int Range(int start, int end) => random.RandiRange(start, end);
    public float Range(float start, float end) => random.RandfRange(start, end);

    public uint NextUint() => random.Randi();
    public int NextInt() => random.RandiRange(int.MinValue, int.MaxValue);
    public float NextFloat() => random.Randf();
    public float NextNormal(float mean = 0, float deviation = 1) => random.Randfn(mean, deviation);
}


internal class PcgRandomTest {
 
    public static void Main() {
    }
}