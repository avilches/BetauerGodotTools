using System;
using Godot;

namespace Betauer.Core;

/// <summary>
/// A class to extract pixels from a Texture2D faster than using GetPixel
/// </summary>
public class FastPixelImage {
    private readonly byte[] _rawImage;
    private readonly int _width;
    private readonly int _height;
    private readonly bool _useMipmaps;
    private bool _dirty = false;
    private Image? _image;
    private Texture2D? _texture2D;

    private const int BytesPerPixel = 4;
    private const Image.Format Format = Image.Format.Rgba8;
    
    public Image Image {
        get {
            if (_image == null) {
                _image = Image.Create(_width, _height, _useMipmaps, Format);
            }
            return _image;
        }
    }

    public Texture2D Texture2D {
        get {
            if (_texture2D == null) {
                if (_dirty) Flush();
                _texture2D = ImageTexture.CreateFromImage(Image);
            }
            return _texture2D;
        }
    }

    public FastPixelImage(Texture2D texture) : this(texture.GetImage()) {
        _texture2D = texture;
    }

    public FastPixelImage(Image image) {
        _image = image;
        if (Image.GetFormat() != Format) {
            Image.Convert(Format);
        }
        _width = Image.GetWidth();
        _height = Image.GetHeight();
        _rawImage = Image.GetData();
        _useMipmaps = Image.HasMipmaps();
    }
    
    public FastPixelImage(int width, int height, bool useMipmaps = false) {
        _width = width;
        _height = height;
        _rawImage = new byte[width * height * 4];
        _useMipmaps = useMipmaps;
    }
	
    public Color GetPixel(int x, int y) {
        var ofs = y * _width + x;
        var r = _rawImage[ofs * BytesPerPixel + 0];
        var g = _rawImage[ofs * BytesPerPixel + 1] / 255.0f;
        var b = _rawImage[ofs * BytesPerPixel + 2] / 255.0f;
        var a = _rawImage[ofs * BytesPerPixel + 3] / 255.0f;
        return new Color(r, g, b, a);
    }

    public void Fill(Color color) {
        var r8 = (byte)color.R8;
        var g8 = (byte)color.G8;
        var b8 = (byte)color.B8;
        var a8 = (byte)color.A8;
        var size = _width * _height * BytesPerPixel;
        for (var i = 0; i < size; i += 4) {
            _rawImage[i + 0] = r8;
            _rawImage[i + 1] = g8;
            _rawImage[i + 2] = b8;
            _rawImage[i + 3] = a8;
        }
    }

    public void FillRect(Rect2I rect2I, Color color) {
        var position = rect2I.Position;
        var size = rect2I.Size;
        FillRect(position.X, position.Y, size.X, size.Y, color);
    }

    public void FillRect(int x, int y, int width, int height, Color color) {
        var r8 = (byte)color.R8;
        var g8 = (byte)color.G8;
        var b8 = (byte)color.B8;
        var a8 = (byte)color.A8;
        var lineEnd = y + height;
        for (var drawY = y; drawY < lineEnd; drawY++) {
            var pixelEnd = x + width;
            for (var drawX = x; drawX < pixelEnd; drawX++) {
                SetPixel(drawY, drawX, r8, g8, b8, a8);
            }
        }
    }

    public void SetPixel(int x, int y, Color color) {
        SetPixel(x, y, (byte)color.R8, (byte)color.G8, (byte)color.B8, (byte)color.A8);
    }

    public void SetPixel(int x, int y, byte r, byte g, byte b, byte a) {
        var ofs = (y * _width + x) * BytesPerPixel;
        _rawImage[ofs + 0] = r;
        _rawImage[ofs + 1] = g;
        _rawImage[ofs + 2] = b;
        _rawImage[ofs + 3] = a;
        _dirty = true;
    }
    
    public void Flush() {
        if (!_dirty) return;
        _dirty = false;
        Image.SetData(_width, _height, _useMipmaps, Image.Format.Rgba8, _rawImage);
    }
}