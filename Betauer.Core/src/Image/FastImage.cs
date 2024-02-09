using System;
using System.Collections.Generic;
using System.Linq;
using Godot;

namespace Betauer.Core.Image;

/// <summary>
/// A class to read and write pixels from a Image faster than using GetPixel
/// </summary>
public class FastImage {
    public const Godot.Image.Format DefaultFormat = Godot.Image.Format.Rgba8;
    public byte[] RawImage { get; private set; }
    public int Width { get; private set; }
    public int Height { get; private set; }
    public bool UseMipmaps { get; private set; }
    public Godot.Image Image { get; private set; }
    public Godot.Image.Format Format => _pixel.Format;
    private Pixel _pixel;
    private bool _dirty = false;

    public FastImage() {
    }

    public FastImage Create(int width, int height, bool useMipmaps = false, Godot.Image.Format format = DefaultFormat) {
        var pixel = Pixel.Get(format);
        var rawImage = new byte[width * height * pixel.GetBytesPerPixel()];
        _pixel = pixel;
        Width = width;
        Height = height;
        UseMipmaps = useMipmaps;
        RawImage = rawImage;
        Image = Godot.Image.CreateFromData(width, height, useMipmaps, format, rawImage);
        return this;
    }

    public FastImage LoadResource(string resource, Godot.Image.Format? format = null) {
        var image = Godot.Image.LoadFromFile(resource);
        if (format.HasValue) {
            var pixel = Pixel.Get(format.Value); // fail fast if the format is not supported
            if (pixel.Format != image.GetFormat()) {
                image.Convert(pixel.Format);
            }
        }
        Image = image;
        Reload();
        return this;
    }

    public FastImage Load(Godot.Image image) {
        Image = image;
        Reload();
        return this;
    }

    public FastImage Reload() {
        _pixel = Pixel.Get(Image.GetFormat());
        Width = Image.GetWidth();
        Height = Image.GetHeight();
        UseMipmaps = Image.HasMipmaps();
        RawImage = Image.GetData();
        return this;
    }

    public FastImage Convert(Godot.Image.Format newFormat) {
        if (Format == newFormat) return this;
        _pixel = Pixel.Get(newFormat);
        Image.Convert(newFormat);
        RawImage = Image.GetData();
        return this;
    }

    public void SetAlpha(int x, int y, float alpha) {
        SetChannel(x, y, 3, alpha);
    }

    public void SetAlpha(Vector2I position, float alpha) {
        SetAlpha(position.X, position.Y, alpha);
    }

    public float GetAlpha(Vector2I position) {
        return GetAlpha(position.X, position.Y);
    }

    public float GetAlpha(int x, int y) {
        return GetChannel(x, y, 3);
    }

    public void SetPixel(Vector2I position, Color color, bool blend = true) {
        SetPixel(position.X, position.Y, color, blend);
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

    public void SetChannel(Vector2I position, int channel, float value) {
        SetChannel(position.X, position.Y, channel, value);
    }

    public void SetChannel(int x, int y, int channel, float value) {
        ValidateChannel(channel);
        if (x < 0 || y < 0 || x >= Width || y >= Height) return;
        _pixel.SetChannel(this, x, y, channel, value);
    }

    private void ValidateChannel(int channel) {
        if (channel >= _pixel.GetChannels()) {
            if (_pixel.GetChannels() == 1) {
                throw new Exception($"Channel {channel} out of bounds: 1 channel only available as 0");
            } else {
                throw new Exception($"Channel {channel} out of bounds: {_pixel.GetChannels()} channels available as {string.Join(", ", Enumerable.Range(0, _pixel.GetChannels()))}");
            }
        }
    }

    public Color GetPixel(Vector2I position) {
        return GetPixel(position.X, position.Y);
    }

    public Color GetPixel(int x, int y) {
        if (x < 0 || y < 0 || x >= Width || y >= Height) throw new Exception($"Coords x:{x} y:{y} out of bounds");
        return _pixel.GetPixel(this, x, y);
    }

    public float GetChannel(Vector2I position, int channel) {
        return GetChannel(position.X, position.Y, channel);
    }

    public float GetChannel(int x, int y, int channel) {
        ValidateChannel(channel);
        if (x < 0 || y < 0 || x >= Width || y >= Height) throw new Exception($"Coords x:{x} y:{y} out of bounds");
        return _pixel.GetChannel(this, x, y, channel);
    }

    public virtual void Flush() {
        if (!_dirty) return;
        Image.SetData(Width, Height, UseMipmaps, Format, RawImage);
        _dirty = false;
    }

    public ImageTexture CreateImageTexture() => ImageTexture.CreateFromImage(Image);
}

public abstract class Pixel {
    public abstract int GetBytesPerPixel();
    public abstract int GetBytesPerChannel();
    public abstract int GetChannels();

    public abstract Color GetPixel(FastImage image, int x, int y);
    public abstract float GetChannel(FastImage image, int x, int y, int channel);
    public abstract void SetPixel(FastImage image, int x, int y, Color color);
    public abstract void SetChannel(FastImage image, int x, int y, int channel, float value);
    
    public abstract Godot.Image.Format Format { get;  }

    public static readonly Dictionary<Godot.Image.Format, Pixel> PixelByFormat = new();

    static Pixel() {
        new Pixel[] {
            PixelRgbaF.Instance, 
            PixelRgbF.Instance, 
            PixelRgba8.Instance, 
            PixelRgb8.Instance, 
            PixelL8.Instance
        }.ForEach(p => PixelByFormat.Add(p.Format, p));
    }
    
    public int GetPixelPosition(FastImage image, int x, int y) {
        return (y * image.Width + x) * GetBytesPerPixel();
    }

    public static Pixel Get(Godot.Image.Format format) {
        return PixelByFormat.TryGetValue(format, out var pixel) 
            ? pixel 
            : throw new Exception($"Format {format} is not supported. Allowed formats: {string.Join(", ", PixelByFormat.Keys)}");
    }

    protected static void WriteFloat(byte[] arr, float value, int startIndex) {
        var bytes = BitConverter.GetBytes(value);
        // Array.Reverse(bytes);
        Buffer.BlockCopy(bytes, 0, arr, startIndex, 4);
    }
    
    protected static byte FloatToByte(float value) {
        return (byte)Math.Round(value * 255f);
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
    public override Godot.Image.Format Format => Godot.Image.Format.L8;

    public override Color GetPixel(FastImage image, int x, int y) {
        var ofs = GetPixelPosition(image, x, y);
        var l = image.RawImage[ofs + 0] / 255f;
        return new Color(l, l, l, 1);
    }

    public override float GetChannel(FastImage image, int x, int y, int channel) {
        var ofs = GetPixelPosition(image, x, y);
        return image.RawImage[ofs + channel] / 255f;
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
    public override Godot.Image.Format Format => Godot.Image.Format.Rgba8;

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

public class PixelRgb8 : Pixel {
    public static readonly PixelRgb8 Instance = new();
    
    public const int Channels = 3;
    public const int BytesPerChannel = 1;
    public const int BytesPerPixel = BytesPerChannel * Channels;

    public override int GetBytesPerPixel() => BytesPerPixel;
    public override int GetBytesPerChannel() => BytesPerChannel;
    public override int GetChannels() => Channels;
    public override Godot.Image.Format Format => Godot.Image.Format.Rgb8;

    public override Color GetPixel(FastImage image, int x, int y) {
        var ofs = GetPixelPosition(image, x, y);
        var r = image.RawImage[ofs + 0] / 255f;
        var g = image.RawImage[ofs + 1] / 255f;
        var b = image.RawImage[ofs + 2] / 255f;
        return new Color(r, g, b, 1);
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
    public override Godot.Image.Format Format => Godot.Image.Format.Rgbaf;
    
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

public class PixelRgbF : Pixel {
    public static readonly PixelRgbF Instance = new();

    public const int Channels = 3;
    public const int BytesPerChannel = 4;
    public const int BytesPerPixel = BytesPerChannel * Channels;
    
    public override int GetBytesPerPixel() => BytesPerPixel;
    public override int GetBytesPerChannel() => BytesPerChannel;
    public override int GetChannels() => Channels;
    public override Godot.Image.Format Format => Godot.Image.Format.Rgbf;
    
    public override Color GetPixel(FastImage image, int x, int y) {
        var ofs = GetPixelPosition(image, x, y);
        var r = BitConverter.ToSingle(image.RawImage, ofs + 0);
        var g = BitConverter.ToSingle(image.RawImage, ofs + 4);
        var b = BitConverter.ToSingle(image.RawImage, ofs + 8);
        return new Color(r, g, b, 1);
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
    }

    public override void SetChannel(FastImage image, int x, int y, int channel, float value) {
        var ofs = GetPixelPosition(image, x, y);
        WriteFloat(image.RawImage, value, ofs + 4 * channel);
    }
}