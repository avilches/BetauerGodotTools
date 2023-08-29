using System;
using System.Linq;
using Godot;

namespace Betauer.Core.Image;

/// <summary>
/// A class to read and write pixels from a Image or a Texture2D faster than using GetPixel
/// </summary>
public class FastImage {
    public const Godot.Image.Format DefaultFormat = Godot.Image.Format.Rgbaf;
    public static readonly Godot.Image.Format[] AllowedFormats = { Godot.Image.Format.Rgbaf, Godot.Image.Format.Rgba8 };

    public byte[] RawImage;
    public int Width { get; private set; }
    public int Height { get; private set; }
    public bool UseMipmaps { get; private set; }
    private bool _dirty = false;
    private Godot.Image? _image;
    private ImageTexture? _imageTexture;
    private Godot.Image.Format _format;

    public FastImage(Texture2D texture) {
        Load(texture);
    }

    public FastImage(string resource, Godot.Image.Format? format = null) {
        var image = Godot.Image.LoadFromFile(resource);
        if (format.HasValue && image.GetFormat() != format) {
            image.Convert(format.Value);
        }
        Load(image);
    }

    public FastImage(Godot.Image image) {
        Load(image);
    }

    public FastImage(int width, int height, bool useMipmaps = false, Godot.Image.Format format = DefaultFormat) {
        Width = width;
        Height = height;
        UseMipmaps = useMipmaps;
        _format = format;
        RawImage = new byte[width * height * BytesPerPixel()];
    }

    public Godot.Image.Format Format {
        get => _format;
        set {
            if (_format == value) return;
            Image.Convert(value);
            _format = value;
            Reload();
        }
    }

    private int BytesPerPixel() {
        return Format switch {
            Godot.Image.Format.Rgba8 => 4,
            Godot.Image.Format.Rgbaf => 16,
            _ => throw new Exception($"Format not supported: {Format}")
        };
    }

    public bool HasImage => _image != null;

    public Godot.Image Image {
        get {
            if (_image == null) {
                _image = Godot.Image.Create(Width, Height, UseMipmaps, Format);
                _dirty = true;
                Flush();
            }
            return _image;
        }
    }

    public bool HasImageTexture => _imageTexture != null;

    public ImageTexture ImageTexture {
        get {
            if (_imageTexture != null) return _imageTexture;
            if (_dirty) Flush();
            _imageTexture = ImageTexture.CreateFromImage(Image);
            return _imageTexture;
        }
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
            Format = _image.GetFormat();
            if (!AllowedFormats.Contains(_image.GetFormat())) {
                _image.Convert(DefaultFormat);
            }
            Width = _image.GetWidth();
            Height = _image.GetHeight();
            UseMipmaps = _image.HasMipmaps();
            RawImage = _image.GetData();
        }
    }

    public void SetPixel(int x, int y, Color color, bool blend = true) {
        if (x < 0 || y < 0 || x >= Width || y >= Height) return;
        if (blend) {
            if (color.A <= 0) return;
            if (color.A < 1f) {
                var currentColor = GetPixel(x, y);
                color = currentColor.Blend(color);
            }
        }
        if (Format == Godot.Image.Format.Rgba8) {
            SetPixelRgba8(x, y, color);
            _dirty = true;
        } else if (Format == Godot.Image.Format.Rgbaf) {
            SetPixelRgbaF(x, y, color);
            _dirty = true;
        } else {
            throw new Exception($"Format not supported: {Format}");
        }
    }

    public Color GetPixel(int x, int y) {
        if (Format == Godot.Image.Format.Rgba8) {
            return GetPixelRgba8(x, y);
        } else if (Format == Godot.Image.Format.Rgbaf) {
            return GetPixelRgbaF(x, y);
        }
        throw new Exception($"Format not supported: {Format}");
    }

    public float GetChannel(int x, int y, int channel) {
        if (Format == Godot.Image.Format.Rgba8) {
            return GetChannelRgba8(x, y, channel) / 255.0f;
        } else if (Format == Godot.Image.Format.Rgbaf) {
            return GetChannelRgbaF(x, y, channel);
        }
        throw new Exception($"Format not supported: {Format}");
    }

    public void Flush() {
        if (!_dirty) return;
        _dirty = false;
        Image.SetData(Width, Height, UseMipmaps, Format, RawImage);
        _imageTexture?.Update(Image);
    }

    private int GetChannelRgba8(int x, int y, int channel) {
        var ofs = (y * Width + x) * BytesPerPixel();
        return RawImage[ofs + channel];
    }

    private Color GetPixelRgba8(int x, int y) {
        var ofs = (y * Width + x) * BytesPerPixel();
        var r = RawImage[ofs + 0] / 255.0f;
        var g = RawImage[ofs + 1] / 255.0f;
        var b = RawImage[ofs + 2] / 255.0f;
        var a = RawImage[ofs + 3] / 255.0f;
        return new Color(r, g, b, a);
    }

    private float GetChannelRgbaF(int x, int y, int channel) {
        var ofs = (y * Width + x) * BytesPerPixel();
        return BitConverter.ToSingle(RawImage, ofs + (channel * 4));
    }

    private Color GetPixelRgbaF(int x, int y) {
        var ofs = (y * Width + x) * BytesPerPixel();
        var r = BitConverter.ToSingle(RawImage, ofs + 0);
        var g = BitConverter.ToSingle(RawImage, ofs + 4);
        var b = BitConverter.ToSingle(RawImage, ofs + 8);
        var a = BitConverter.ToSingle(RawImage, ofs + 12);
        return new Color(r, g, b, a);
    }

    private void SetPixelRgbaF(int x, int y, Color color) {
        var bytesPerPixel = BytesPerPixel();
        var ofs = (y * Width + x) * bytesPerPixel;
        WriteFloat(RawImage, color.R, ofs + 4 * 0);
        WriteFloat(RawImage, color.G, ofs + 4 * 1);
        WriteFloat(RawImage, color.B, ofs + 4 * 2);
        WriteFloat(RawImage, color.A, ofs + 4 * 3);
    }

    private void SetPixelRgba8(int x, int y, Color color) {
        var ofs = (y * Width + x) * BytesPerPixel();
        RawImage[ofs + 0] = (byte)color.R8;
        RawImage[ofs + 1] = (byte)color.G8;
        RawImage[ofs + 2] = (byte)color.B8;
        RawImage[ofs + 3] = (byte)color.A8;
    }

    private static void WriteFloat(byte[] arr, float value, int startIndex) {
        var bytes = BitConverter.GetBytes(value);
        // Array.Reverse(bytes);
        Buffer.BlockCopy(bytes, 0, arr, startIndex, 4);
    }

}