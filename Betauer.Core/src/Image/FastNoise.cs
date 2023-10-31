using System;
using Godot;

namespace Betauer.Core.Image;

/// <summary>
/// A Noise wrapper to get values faster from a Godot Noise class. It creates a image with the noise, so all the values in the width x height size
/// must b calculated and normalized to create the image. Then, it convert the internal image to a L8 format (just one byte per color), which is the smallest
/// possible binary to format to store the noise and get the image as array of bytes. This array of bytes can be used as a cache and allow access to the noise
/// values directly (no inter-op with Godot), which is faster than using noise.GetNoise2D.
/// </summary>
public class FastNoise : INoise2D {
    private readonly Noise _noise;

    private byte[][] _rawImage3D;
    private readonly int _width;
    private readonly int _height;
    private readonly int _z3dDepth;
    private readonly bool _seamless;
    private readonly bool _normalize;
    private readonly bool _invert;
    private readonly float _seamlessBlendSkirt;

    public FastNoise(Noise noise, int width, int height, int z3dDepth = 1, bool seamless = false, bool normalize = true, bool invert = false,
        float seamlessBlendSkirt = 0.1f) {
        if (z3dDepth <= 0) throw new ArgumentOutOfRangeException(nameof(z3dDepth));
        if (width <= 0) throw new ArgumentOutOfRangeException(nameof(width));
        if (height <= 0) throw new ArgumentOutOfRangeException(nameof(width));
        _noise = noise;
        _width = width;
        _height = height;
        _z3dDepth = z3dDepth;
        _seamless = seamless;
        _normalize = normalize;
        _invert = invert;
        _seamlessBlendSkirt = seamlessBlendSkirt;
        Reload();
    }

    public FastNoise(NoiseTexture2D noiseTexture2D, int z3dDepth = 1) {
        if (z3dDepth <= 0) throw new ArgumentOutOfRangeException(nameof(z3dDepth));
        _noise = noiseTexture2D.Noise;
        _width = noiseTexture2D.Width;
        _height = noiseTexture2D.Height;
        _z3dDepth = z3dDepth;
        _seamless = noiseTexture2D.Seamless;
        _normalize = noiseTexture2D.Normalize;
        _invert = noiseTexture2D.Invert;
        _seamlessBlendSkirt = noiseTexture2D.SeamlessBlendSkirt;
        Reload();
    }

    public NoiseTexture2D CreateTexture(bool generateMipmaps = false, bool asNormalMap = false, float bumpStrength = 1f, Gradient colorRamp = null) {
        return new NoiseTexture2D {
            Noise = _noise,
            Width = _width,
            Height = _height,
            In3DSpace = _z3dDepth > 1,
            Seamless = _seamless,
            Normalize = _normalize,
            GenerateMipmaps = generateMipmaps,
            Invert = _invert,
            SeamlessBlendSkirt = _seamlessBlendSkirt,
            AsNormalMap = asNormalMap,
            BumpStrength = bumpStrength,
            ColorRamp = colorRamp,
        };
    }

    // Load the noise from the texture and the gradients
    public void Reload() {
        if (_z3dDepth <= 1) {
            var image = _seamless
                ? _noise.GetSeamlessImage(_width, _height, _invert, false, _seamlessBlendSkirt, _normalize)
                : _noise.GetImage(_width, _height, _invert, false, _normalize);
            var rawImage = image.GetData();
            _rawImage3D = new[] { rawImage };
        } else {
            var images = _seamless
                ? _noise.GetSeamlessImage3D(_width, _height, _z3dDepth, _invert, _seamlessBlendSkirt, _normalize)
                : _noise.GetImage3D(_width, _height, _z3dDepth, _invert, _normalize);
            _rawImage3D = new byte[_z3dDepth][];
            for (var i = 0; i < _z3dDepth; i++) {
                _rawImage3D[i] = images[i].GetData();
            }
        }
    }

    public float GetNoise(int x) {
        x %= _width;
        return _rawImage3D[0][x] / 255.0f;
    }

    public float GetNoise(int x, int y) {
        x %= _width;
        y %= _height;
        var ofs = y * _width + x;
        return _rawImage3D[0][ofs] / 255.0f;
    }

    public float GetNoise(int x, int y, int z) {
        x %= _width;
        y %= _height;
        z %= _z3dDepth;
        var ofs = y * _width + x;
        return _rawImage3D[z][ofs] / 255.0f;
    }

    public float GetNoise(Vector2I pos) {
        return GetNoise(pos.X, pos.Y);
    }

    public float GetNoise(Vector3I pos) {
        return GetNoise(pos.X, pos.Y, pos.Z);
    }

    public Godot.Image GetImage(int z3dDepth = 0) {
        if (z3dDepth < 0) throw new ArgumentOutOfRangeException(nameof(z3dDepth));
        var image = Godot.Image.CreateFromData(_width, _height, false, Godot.Image.Format.L8, _rawImage3D[z3dDepth]);
        return image;
    }
}