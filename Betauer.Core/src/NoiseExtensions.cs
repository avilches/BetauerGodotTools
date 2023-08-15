using Godot;

namespace Betauer.Core;

public static class NoiseExtensions {
    public static float GetNormalizedNoise1D(this Noise noise, float x) =>
        Normalized(noise.GetNoise1D(x));

    public static float GetNoise1D(this Noise noise, float x) =>
        Normalized(noise.GetNoise1D(x));

    public static float GetNormalizedNoise2D(this Noise noise, float x, float y) =>
        Normalized(noise.GetNoise2D(x, y));

    public static float GetNormalizedNoise2Dv(this Noise noise, Vector2 pos) =>
        Normalized(noise.GetNoise2Dv(pos));

    public static float GetNormalizedNoise3D(this Noise noise, float x, float y, float z) =>
        Normalized(noise.GetNoise3D(x, y, z));

    public static float GetNormalizedNoise3Dv(this Noise noise, Vector3 pos) =>
        Normalized(noise.GetNoise3Dv(pos));

    private static float Normalized(float value) => (value + 1f) / 2f;

}