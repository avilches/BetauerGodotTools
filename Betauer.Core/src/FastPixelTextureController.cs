using Godot;

namespace Betauer.Core;

/// <summary>
/// A class to extract pixels from a Texture2D faster than using GetPixel
/// </summary>
public class FastPixelTextureController {
    private readonly Image _image;
    private readonly byte[] _rawImage;
    private readonly int _width;

    public FastPixelTextureController(Texture2D noiseTexture) {
        _image = noiseTexture.GetImage();
        if (_image.GetFormat() != Image.Format.Rgba8) {
            _image.Convert(Image.Format.Rgba8);
        }
        _width = _image.GetWidth();
        _rawImage = _image.GetData();
    }
	
    public Color GetPixel(int x, int y) {
        var ofs = y * _width + x;
        var r = _rawImage[ofs * 4 + 0] / 255.0f;
        var g = _rawImage[ofs * 4 + 1] / 255.0f;
        var b = _rawImage[ofs * 4 + 2] / 255.0f;
        var a = _rawImage[ofs * 4 + 3] / 255.0f;
        return new Color(r, g, b, a);
    }
}