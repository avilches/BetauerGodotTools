using System;
using Godot;

namespace Betauer.Core.Image;

/// <summary>
/// A class to read and write pixels from a Image or a Texture2D faster than using GetPixel
/// </summary>
public static partial class FastImageExtensions {

    public static void BlitRect(this FastImage me, FastImage other, Rect2I source, Vector2I dest) {
        DumpTo(other, me, null, source.Position.X, source.Position.Y, source.Size.X, source.Size.Y, dest.X, dest.Y, false);
    }

    public static void BlitRectMask(this FastImage me, FastImage other, FastImage mask, Rect2I source, Vector2I dest) {
        DumpTo(other, me, mask, source.Position.X, source.Position.Y, source.Size.X, source.Size.Y, dest.X, dest.Y, false);
    }

    public static void BlendRectMask(this FastImage me, FastImage other, FastImage mask, Rect2I source, Vector2I dest) {
        DumpTo(other, me, mask, source.Position.X, source.Position.Y, source.Size.X, source.Size.Y, dest.X, dest.Y, true);
    }

    public static void BlendRect(this FastImage me, FastImage other, Rect2I source, Vector2I dest) {
        DumpTo(other, me, null, source.Position.X, source.Position.Y, source.Size.X, source.Size.Y, dest.X, dest.Y, true);
    }

    public static FastImage Extract(this FastImage me, FastImage? mask, int fromX, int fromY, int width, int height) {
        var other = new FastImage(width, height, false, me.Format);
        DumpTo(me, other, mask, fromX, fromY, width, height, 0, 0, false);
        return other;
    }

    public static void DumpTo(this FastImage me, FastImage other, FastImage? mask, int fromX, int fromY, int width, int height, int destX, int destY, bool blend) {
        for (var j = 0; j < height; j++) {
            for (var i = 0; i < width; i++) {
                var color = me.GetPixel(i + fromX, j + fromY);
                var allow = mask == null || mask.GetChannel(i + fromX, j + fromY, 3) > 0;
                if (allow) {
                    other.SetPixel(i + destX, j + destY, color, blend);
                }
            }
        }
    }

    public static FastImage ExpandTiled(this FastImage me, int width, int height) {
        var other = new FastImage(width, height, false, me.Format);
        for (var j = 0; j < height; j += me.Height) {
            for (var i = 0; i < width; i += me.Width) {
                DumpTo(me, other, null, 0, 0, me.Width, me.Height, i, j, false);
            }
        }
        return other;
    }
}