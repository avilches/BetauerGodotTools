using Godot;

namespace Betauer.Core.Image;

/// <summary>
/// A class to read and write pixels from a Image or a Texture2D faster than using GetPixel
/// </summary>
public class FastImage {
    private byte[] _rawImage;
    private int _width;
    private int _height;
    private bool _useMipmaps;
    private bool _dirty = false;
    private Godot.Image? _image;
    private ImageTexture? _imageTexture;

    private const int BytesPerPixel = 4;
    private const Godot.Image.Format Format = Godot.Image.Format.Rgba8;

    public Godot.Image Image {
        get {
            if (_image == null) {
                _image = Godot.Image.Create(_width, _height, _useMipmaps, Format);
            }
            return _image;
        }
    }

    public bool HasImageTexture => _imageTexture != null;
    
    public ImageTexture ImageTexture {
        get {
            if (_imageTexture == null) {
                if (_dirty) Flush();
                _imageTexture = ImageTexture.CreateFromImage(Image);
            }
            return _imageTexture;
        }
    }

    public FastImage(int width, int height, bool useMipmaps = false) {
        _width = width;
        _height = height;
        _rawImage = new byte[width * height * 4];
        _useMipmaps = useMipmaps;
    }

    public FastImage(Texture2D texture) {
        Load(texture);
    }

    public FastImage(Godot.Image image) {
        Load(image);
    }

    public void Load(Texture2D texture) {
        _image = texture.GetImage();
        _imageTexture = texture as ImageTexture;
        Reload();
    }

    public void Load(Godot.Image image) {
        _image = image;
        _imageTexture?.SetImage(_image);
        Reload();
    }

    public void Reload() {
        if (_image == null && _imageTexture != null) {
            _image = _imageTexture.GetImage();
        }
        if (_image != null) {
            if (_image.GetFormat() != Format) {
                _image.Convert(Format);
            }
            _width = _image.GetWidth();
            _height = _image.GetHeight();
            _rawImage = _image.GetData();
            _useMipmaps = _image.HasMipmaps();
        }
    }

    public Color GetPixel(int x, int y) {
        var ofs = y * _width + x;
        var r = _rawImage[ofs * BytesPerPixel + 0] / 255.0f;
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
        if (x < 0 || y < 0 || x >= _width || y >= _height) return;
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
        Image.SetData(_width, _height, _useMipmaps, Godot.Image.Format.Rgba8, _rawImage);
        _imageTexture?.Update(Image);
    }

    public void DrawCircle(int centerX, int centerY, int r, Color color) {
        var x = r;
        var y = 0;

        SetPixel(centerX + x, centerY + y, color);
        if (r > 0) {
            SetPixel(centerX - x, centerY - y, color);
            SetPixel(centerX + y, centerY + x, color);
            SetPixel(centerX - y, centerY - x, color);
        }

        var error = 1 - r;
        while (x > y) {
            y++;                                                         
            if (error <= 0) 
                error = error + 2 * y + 1; // draw up
            else {
                x--;
                error = error + 2 * y - 2 * x + 1;
            }
            if (x < y) break; // circle finished
            SetPixel(centerX + x, centerY + y, color);
            SetPixel(centerX - x, centerY + y, color);
            SetPixel(centerX + x, centerY - y, color);
            SetPixel(centerX - x, centerY - y, color);

            if (x != y) {
                SetPixel(centerX + y, centerY + x, color);
                SetPixel(centerX - y, centerY + x, color);
                SetPixel(centerX + y, centerY - x, color);
                SetPixel(centerX - y, centerY - x, color);
            }
        }
    }
}