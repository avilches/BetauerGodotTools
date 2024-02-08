using Godot;

namespace Betauer.Core.Image;

/// <summary>
/// A class to read and write pixels from a Image or a Texture2D faster than using GetPixel
/// </summary>
public static partial class FastImageExtensions {
    
    public static void BlitRect(this FastImage me, FastImage other, Rect2I source, Vector2I dest) {
        CopyTo(other, me, null, source.Position.X, source.Position.Y, source.Size.X, source.Size.Y, dest.X, dest.Y, false);
    }

    public static void BlitRectMask(this FastImage me, FastImage other, FastImage mask, Rect2I source, Vector2I dest) {
        CopyTo(other, me, mask, source.Position.X, source.Position.Y, source.Size.X, source.Size.Y, dest.X, dest.Y, false);
    }

    public static void BlendRectMask(this FastImage me, FastImage other, FastImage mask, Rect2I source, Vector2I dest) {
        CopyTo(other, me, mask, source.Position.X, source.Position.Y, source.Size.X, source.Size.Y, dest.X, dest.Y, true);
    }

    public static void BlendRect(this FastImage me, FastImage other, Rect2I source, Vector2I dest) {
        CopyTo(other, me, null, source.Position.X, source.Position.Y, source.Size.X, source.Size.Y, dest.X, dest.Y, true);
    }

    public static FastImage Extract(this FastImage me, FastImage? mask, int fromX, int fromY, int width, int height) {
        var other = new FastImage().Create(width, height, false, me.Format);
        CopyTo(me, other, mask, fromX, fromY, width, height, 0, 0, false);
        return other;
    }

    /// <summary>
    /// Copy a rectangle from the source image to the destination image, using a mask to allow or not the copy of each pixel
    /// </summary>
    /// <param name="source"></param>
    /// <param name="other"></param>
    /// <param name="mask"></param>
    /// <param name="startX"></param>
    /// <param name="startY"></param>
    /// <param name="width"></param>
    /// <param name="height"></param>
    /// <param name="destX"></param>
    /// <param name="destY"></param>
    /// <param name="blend"></param>
    public static void CopyTo(this FastImage source, FastImage other, FastImage? mask, int startX, int startY, int width, int height, int destX, int destY,
        bool blend) {
        for (var j = 0; j < height; j++) {
            for (var i = 0; i < width; i++) {
                var color = source.GetPixel(i + startX, j + startY);
                var allow = mask == null || mask.GetChannel(i + startX, j + startY, 3) > 0;
                if (allow) {
                    other.SetPixel(i + destX, j + destY, color, blend);
                }
            }
        }
    }

    /// <summary>
    /// Create a new image and copy the original image to the new one, starting from the 0,0 and repeating the image if necessary
    /// </summary>
    /// <param name="source"></param>
    /// <param name="width"></param>
    /// <param name="height"></param>
    /// <returns></returns>
    public static FastImage ExpandTiled(this FastImage source, int width, int height) {
        var expanded = new FastImage().Create(width, height, false, source.Format);
        for (var y = 0; y < height; y += source.Height) {
            for (var x = 0; x < width; x += source.Width) {
                CopyTo(source, expanded, null, 0, 0, source.Width, source.Height, x, y, false);
            }
        }
        return expanded;
    }
}