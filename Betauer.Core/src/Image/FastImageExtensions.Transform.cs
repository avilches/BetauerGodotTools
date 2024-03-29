using Godot;

namespace Betauer.Core.Image;

/// <summary>
/// A class to read and write pixels from a Image or a Texture2D faster than using GetPixel
/// </summary>
public static partial class FastImageExtensions {
    public static Color[,] GetRegion(this FastImage source, int startX, int startY, int width, int height) {
        if (startX < 0 || startY < 0 || startX + width > source.Width || startY + height > source.Height) {
            throw new System.Exception("Invalid region");
        }
        Color[,] region = new Color[height, width];
        for (var x = 0; x < width; x++) {
            for (var y = 0; y < height; y++) {
                region[y, x] = source.GetPixel(startX + x, startY + y);
            }
        }
        return region;
    }

    public static void SetRegion(this FastImage source, Color[,] region, int startX, int startY, int width = -1, int height = -1, bool blend = true) {
        height = height < 0 ? region.GetLength(0) : height;
        width = width < 0 ? region.GetLength(1) : width;
        for (var x = 0; x < width; x++) {
            for (var y = 0; y < height; y++) {
                source.SetPixel(startX + x, startY + y, region[y, x], blend);
            }
        }
    }

    public static void Rotate90(this FastImage source, int startX, int startY, int width, int height) {
        if (startX < 0 || startY < 0 || startX + width > source.Width || startY + height > source.Height) {
            throw new System.Exception("Invalid region");
        }
        var temp = new Color[width, height];
        for (var y = 0; y < height; y++) {
            for (var x = 0; x < width; x++) {
                temp[x, y] = source.GetPixel(x + startX,height - 1 - y + startY);
            }
        }
        SetRegion(source, temp, startX, startY);
    }

    public static void Rotate180(this FastImage source, int startX, int startY, int width, int height) {
        if (startX < 0 || startY < 0 || startX + width > source.Width || startY + height > source.Height) {
            throw new System.Exception("Invalid region");
        }
        var temp = new Color[height, width];
        for (var y = 0; y < height; y++) {
            for (var x = 0; x < width; x++) {
                temp[y, x] = source.GetPixel(width - 1 - x + startX, height - 1 - y + startY);
            }
        }
        SetRegion(source, temp, startX, startY);
    }

    public static void RotateMinus90(this FastImage source, int startX, int startY, int width, int height) {
        if (startX < 0 || startY < 0 || startX + width > source.Width || startY + height > source.Height) {
            throw new System.Exception("Invalid region");
        }
        var temp = new Color[width, height];
        for (var y = 0; y < height; y++) {
            for (var x = 0; x < width; x++) {
                temp[width - 1 - x, y] = source.GetPixel(x + startX, y + startY);
            }
        }
        SetRegion(source, temp, startX, startY);
    }

    public static void FlipH(this FastImage source, int startX, int startY, int width, int height) {
        if (startX < 0 || startY < 0 || startX + width > source.Width || startY + height > source.Height) {
            throw new System.Exception("Invalid region");
        }
        var temp = new Color[height, width];
        for (var y = 0; y < height; y++) {
            for (var x = 0; x < width; x++) {
                temp[y, x] = source.GetPixel( width - 1 - x + startX, y + startY);
            }
        }
        SetRegion(source, temp, startX, startY);
    }

    public static void MirrorLeftToRight(this FastImage source, int startX, int startY, int width, int height) {
        if (startX < 0 || startY < 0 || startX + width > source.Width || startY + height > source.Height) {
            throw new System.Exception("Invalid region");
        }
        var halfWidth = width / 2 + (width % 2 != 0 ? 1 : 0);
        var temp = new Color[height, halfWidth];
        var widthOffset = halfWidth - (width % 2 != 0 ? 1 : 0); 
        for (var y = 0; y < height; y++) {
            for (var x = 0; x < halfWidth; x++) {
                temp[y, halfWidth - 1 - x] = source.GetPixel(x + startX, y + startY);
            }
        }
        SetRegion(source, temp, startX + widthOffset, startY);
    }
    
    public static void MirrorRightToLeft(this FastImage source, int startX, int startY, int width, int height) {
        if (startX < 0 || startY < 0 || startX + width > source.Width || startY + height > source.Height) {
            throw new System.Exception("Invalid region");
        }
        var halfWidth = width / 2 + (width % 2 != 0 ? 1 : 0);
        var temp = new Color[height, halfWidth];
        var widthOffset = halfWidth - (width % 2 != 0 ? 1 : 0); 
        for (var y = 0; y < height; y++) {
            for (var x = 0; x < halfWidth; x++) {
                temp[y, halfWidth - 1 - x] = source.GetPixel(x + startX + widthOffset, y + startY);
            }
        }
        SetRegion(source, temp, startX, startY);
    }

    public static void FlipV(this FastImage source, int startX, int startY, int width, int height) {
        if (startX < 0 || startY < 0 || startX + width > source.Width || startY + height > source.Height) {
            throw new System.Exception("Invalid region");
        }
        var temp = new Color[height, width];
        for (var y = 0; y < height; y++) {
            for (var x = 0; x < width; x++) {
                temp[y, x] = source.GetPixel(x + startX, height - 1 - y + startY);
            }
        }
        SetRegion(source, temp, startX, startY);
    }

    public static void MirrorTopToBottom(this FastImage source, int startX, int startY, int width, int height) {
        if (startX < 0 || startY < 0 || startX + width > source.Width || startY + height > source.Height) {
            throw new System.Exception("Invalid region");
        }
        var halfHeight = height / 2 + (height % 2 != 0 ? 1 : 0);
        var temp = new Color[halfHeight, width];
        var heightOffset = halfHeight - (height % 2 != 0 ? 1 : 0); 
        for (var y = 0; y < halfHeight; y++) {
            for (var x = 0; x < width; x++) {
                temp[halfHeight - 1 - y, x] = source.GetPixel(x + startX, y + startY);
            }
        }
        SetRegion(source, temp, startX, startY + heightOffset);
    }

    public static void MirrorBottomToTop(this FastImage source, int startX, int startY, int width, int height) {
        if (startX < 0 || startY < 0 || startX + width > source.Width || startY + height > source.Height) {
            throw new System.Exception("Invalid region");
        }
        var halfHeight = height / 2 + (height % 2 != 0 ? 1 : 0);
        var temp = new Color[halfHeight, width];
        var heightOffset = halfHeight - (height % 2 != 0 ? 1 : 0); 
        for (var y = 0; y < halfHeight; y++) {
            for (var x = 0; x < width; x++) {
                temp[halfHeight - 1 - y, x] = source.GetPixel(x + startX, y + startY + heightOffset);
            }
        }
        SetRegion(source, temp, startX, startY);
    }

    public static void FlipDiagonal(this FastImage source, int startX, int startY, int width, int height) {
        if (startX < 0 || startY < 0 || startX + width > source.Width || startY + height > source.Height) {
            throw new System.Exception("Invalid region");
        }
        var temp = new Color[width, height];
        for (var y = 0; y < height; y++) {
            for (var x = 0; x < width; x++) {
                temp[x, y] = source.GetPixel(x + startX, y + startY);
            }
        }
        SetRegion(source, temp, startX, startY);
    }

    public static void FlipDiagonalSecondary(this FastImage source, int startX, int startY, int width, int height) {
        if (startX < 0 || startY < 0 || startX + width > source.Width || startY + height > source.Height) {
            throw new System.Exception("Invalid region");
        }
        var temp = new Color[width, height];
        for (var y = 0; y < height; y++) {
            for (var x = 0; x < width; x++) {
                temp[width - 1 - x, height - 1 - y] = source.GetPixel(x + startX, y + startY);
            }
        }
        SetRegion(source, temp, startX, startY);
    }
}