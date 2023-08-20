using Godot;

namespace Betauer.Core.Image;

/// <summary>
/// A Noise wrapper to get values faster.
///
/// The trick is to create a NoiseTexture2D with the noise where all the values are calculated and normalized and then, get the image as array of bytes, so
/// it can be used as a cached and access to the values directly (no inter-op with Godot), which is faster than using GetNoise2D.
/// </summary>
public class FastNoise {
    public NoiseTexture2D NoiseTexture2D { get; private set; }

    private byte[] _rawImage;
    private int _width;

    public FastNoise(Noise noise, int width, int height, bool seamless = false, bool normalize = true) {
        NoiseTexture2D = new NoiseTexture2D {
            Noise = noise,
            Width = width,
            Height = height,
            Seamless = seamless,
            Normalize = normalize,
        };
        Reload();
    }

    public FastNoise(NoiseTexture2D noiseTexture2D) {
        NoiseTexture2D = noiseTexture2D;
        Reload();
    }

    // Load the noise from the texture and the gradients
    public void Reload() {
        var image = NoiseTexture2D.GetImage();
        // L8 is a format with just 1 byte per pixel (the smallest!), and it allows to access directly to the values without any conversion
        if (image.GetFormat() != Godot.Image.Format.L8) {
            image.Convert(Godot.Image.Format.L8);
        }
        _width = image.GetWidth();
        _rawImage = image.GetData();
    }

    public float GetNoise(int x, int y) {
        var ofs = y * _width + x;
        return _rawImage[ofs] / 255.0f;
    }
}