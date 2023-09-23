using System;
using System.Linq;
using Godot;

namespace Betauer.Core.Image;

/// <summary>
/// A class to read and write pixels from a Image or a Texture2D faster than using GetPixel
/// </summary>
public class FastImage {
    public const Godot.Image.Format DefaultFormat = Godot.Image.Format.Rgba8;
    public static readonly Godot.Image.Format[] AllowedFormats = { 
        Godot.Image.Format.Rgbaf, // 4 channels, 4 bytes (float) per channel 
        Godot.Image.Format.Rgba8, 
        Godot.Image.Format.L8 };

    public byte[] RawImage { get; private set; }
    public int Width { get; private set; }
    public int Height { get; private set; }
    public bool UseMipmaps { get; private set; }
    private bool _dirty = false;
    private Pixel _pixel;
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
    
    public FastImage(int width, int height, bool useMipmaps = false, Godot.Image.Format? format = null) {
        Width = width;
        Height = height;
        UseMipmaps = useMipmaps;
        _format = format ?? DefaultFormat;
        UpdatedPixel();
        RawImage = new byte[width * height * _pixel!.GetBytesPerPixel()];
    }

    public Godot.Image.Format Format {
        get => _format;
        set {
            if (_format == value) return;
            _format = value;
            UpdatedPixel();
            if (Image != null && Image.GetFormat() != _format) {
                Image.Convert(_format);
            }
        }
    }

    public bool HasImage => _image != null;

    public Godot.Image Image {
        get {
            if (_image == null) {
                _image = Godot.Image.Create(Width, Height, UseMipmaps, _format);
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

    private void UpdatedPixel() {
        if (!AllowedFormats.Contains(_format)) throw new Exception($"Format not supported: {_format}");
        if (_format == Godot.Image.Format.Rgba8) {
            _pixel = PixelRgba8.Instance;
        } else if (_format == Godot.Image.Format.Rgbaf) {
            _pixel = PixelRgbaF.Instance;
        } else if (_format == Godot.Image.Format.L8) {
            _pixel = PixelL8.Instance;
        }
    }

    public void Reload() {
        if (_image == null && _imageTexture != null) {
            _image = _imageTexture.GetImage();
        }
        if (_image != null) {
            _format = _image.GetFormat();
            if (!AllowedFormats.Contains(_format)) {
                _image.Convert(DefaultFormat);
                _format = DefaultFormat;
            }
            UpdatedPixel();
            Width = _image.GetWidth();
            Height = _image.GetHeight();
            UseMipmaps = _image.HasMipmaps();
            RawImage = _image.GetData();
        }
    }

    public void SetAlpha(int x, int y, float alpha) {
        SetChannel(x, y, 3, alpha);
    }

    public float GetAlpha(int x, int y) {
        return GetChannel(x, y, 3);
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
        _pixel.SetPixel(this, x, y, color);
        _dirty = true;
    }

    public void SetChannel(int x, int y, int channel, float value) {
        ValidateChannel(channel);
        if (x < 0 || y < 0 || x >= Width || y >= Height) return;
        _pixel.SetChannel(this, x, y, channel, value);
    }

    private void ValidateChannel(int channel) {
        if (channel >= _pixel.GetChannels()) {
            if (_pixel.GetChannels() == 1) {
                throw new Exception($"Channel {channel} out of bounds: 1 channel available as 0");
            } else {
                throw new Exception($"Channel {channel} out of bounds: {_pixel.GetChannels()} channels available as {string.Join(", ", Enumerable.Range(0, _pixel.GetChannels()))}");
            }
        }
    }

    public Color GetPixel(int x, int y) {
        if (x < 0 || y < 0 || x >= Width || y >= Height) throw new Exception($"Coords x:{x} y:{y} out of bounds");
        return _pixel.GetPixel(this, x, y);
    }

    public float GetChannel(int x, int y, int channel) {
        ValidateChannel(channel);
        if (x < 0 || y < 0 || x >= Width || y >= Height) throw new Exception($"Coords x:{x} y:{y} out of bounds");
        return _pixel.GetChannel(this, x, y, channel);
    }

    public void Flush() {
        if (!_dirty) return;
        _dirty = false;
        Image.SetData(Width, Height, UseMipmaps, _format, RawImage);
        _imageTexture?.Update(Image);
    }
}

public abstract class Pixel {
    public abstract int GetBytesPerPixel();
    public abstract int GetBytesPerChannel();
    public abstract int GetChannels();

    public abstract Color GetPixel(FastImage image, int x, int y);
    public abstract float GetChannel(FastImage image, int x, int y, int channel);
    public abstract void SetPixel(FastImage image, int x, int y, Color color);
    public abstract void SetChannel(FastImage image, int x, int y, int channel, float value);
    
    public int GetPixelPosition(FastImage image, int x, int y) {
        return (y * image.Width + x) * GetBytesPerPixel();
    }

    protected static void WriteFloat(byte[] arr, float value, int startIndex) {
        var bytes = BitConverter.GetBytes(value);
        // Array.Reverse(bytes);
        Buffer.BlockCopy(bytes, 0, arr, startIndex, 4);
    }
    
    protected const float MaxByteValue = byte.MaxValue;

    protected static byte FloatToByte(float value) {
        return (byte)Math.Round(value * MaxByteValue);
    }
}

public class PixelL8 : Pixel {
    public static readonly PixelL8 Instance = new();
    
    public const int Channels = 1;
    public const int BytesPerChannel = 1;
    public const int BytesPerPixel = BytesPerChannel * Channels;

    public override int GetBytesPerPixel() => BytesPerPixel;
    public override int GetBytesPerChannel() => BytesPerChannel;
    public override int GetChannels() => Channels;

    public override Color GetPixel(FastImage image, int x, int y) {
        var ofs = GetPixelPosition(image, x, y);
        var l = image.RawImage[ofs + 0] / MaxByteValue;
        return new Color(l, l, l, 1);
    }

    public override float GetChannel(FastImage image, int x, int y, int channel) {
        var ofs = GetPixelPosition(image, x, y);
        return image.RawImage[ofs + channel] / MaxByteValue;
    }

    public override void SetPixel(FastImage image, int x, int y, Color color) {
        var ofs = GetPixelPosition(image, x, y);
        image.RawImage[ofs + 0] = FloatToByte(color.V);
    }

    public override void SetChannel(FastImage image, int x, int y, int channel, float value) {
        var ofs = GetPixelPosition(image, x, y);
        image.RawImage[ofs + channel] = FloatToByte(value);
    }
}

public class PixelRgba8 : Pixel {
    public static readonly PixelRgba8 Instance = new();
    
    public const int Channels = 4;
    public const int BytesPerChannel = 1;
    public const int BytesPerPixel = BytesPerChannel * Channels;

    public override int GetBytesPerPixel() => BytesPerPixel;
    public override int GetBytesPerChannel() => BytesPerChannel;
    public override int GetChannels() => Channels;

    public override Color GetPixel(FastImage image, int x, int y) {
        var ofs = GetPixelPosition(image, x, y);
        var r = image.RawImage[ofs + 0] / 255f;
        var g = image.RawImage[ofs + 1] / 255f;
        var b = image.RawImage[ofs + 2] / 255f;
        var a = image.RawImage[ofs + 3] / 255f;
        return new Color(r, g, b, a);
    }

    public override float GetChannel(FastImage image, int x, int y, int channel) {
        var ofs = GetPixelPosition(image, x, y);
        return image.RawImage[ofs + channel] / 255f;
    }

    public override void SetPixel(FastImage image, int x, int y, Color color) {
        var ofs = GetPixelPosition(image, x, y);
        image.RawImage[ofs + 0] = (byte)color.R8;
        image.RawImage[ofs + 1] = (byte)color.G8;
        image.RawImage[ofs + 2] = (byte)color.B8;
        image.RawImage[ofs + 3] = (byte)color.A8;
    }

    public override void SetChannel(FastImage image, int x, int y, int channel, float value) {
        var ofs = GetPixelPosition(image, x, y);
        image.RawImage[ofs + channel] = (byte)(value * 255f);
    }
}

public class PixelRgbaF : Pixel {
    public static readonly PixelRgbaF Instance = new();

    public const int Channels = 4;
    public const int BytesPerChannel = 4;
    public const int BytesPerPixel = BytesPerChannel * Channels;
    
    public override int GetBytesPerPixel() => BytesPerPixel;
    public override int GetBytesPerChannel() => BytesPerChannel;
    public override int GetChannels() => Channels;
    
    public override Color GetPixel(FastImage image, int x, int y) {
        var ofs = GetPixelPosition(image, x, y);
        var r = BitConverter.ToSingle(image.RawImage, ofs + 0);
        var g = BitConverter.ToSingle(image.RawImage, ofs + 4);
        var b = BitConverter.ToSingle(image.RawImage, ofs + 8);
        var a = BitConverter.ToSingle(image.RawImage, ofs + 12);
        return new Color(r, g, b, a);
    }

    public override float GetChannel(FastImage image, int x, int y, int channel) {
        var ofs = GetPixelPosition(image, x, y);
        return BitConverter.ToSingle(image.RawImage, ofs + (channel * 4));
    }

    public override void SetPixel(FastImage image, int x, int y, Color color) {
        var ofs = GetPixelPosition(image, x, y);
        WriteFloat(image.RawImage, color.R, ofs + 0);
        WriteFloat(image.RawImage, color.G, ofs + 4);
        WriteFloat(image.RawImage, color.B, ofs + 8);
        WriteFloat(image.RawImage, color.A, ofs + 12);
    }

    public override void SetChannel(FastImage image, int x, int y, int channel, float value) {
        var ofs = GetPixelPosition(image, x, y);
        WriteFloat(image.RawImage, value, ofs + 4 * channel);
    }
}