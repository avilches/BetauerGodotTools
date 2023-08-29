using System;
using Godot;

namespace Betauer.Core.Image;

/// <summary>
/// A class to read and write pixels from a Image or a Texture2D faster than using GetPixel
/// </summary>
public static partial class FastImageExtensions {

    public static void BlitRect(this FastImage me, FastImage other, Rect2I source, Vector2I dest) {
        Dump(me, other, null, source.Position.X, source.Position.Y, source.Size.X, source.Size.Y, dest.X, dest.Y, false);
    }

    public static void BlitRectMask(this FastImage me, FastImage other, FastImage mask, Rect2I source, Vector2I dest) {
        Dump(me, other, mask, source.Position.X, source.Position.Y, source.Size.X, source.Size.Y, dest.X, dest.Y, false);
    }

    public static void BlendRectMask(this FastImage me, FastImage other, FastImage mask, Rect2I source, Vector2I dest) {
        Dump(me, other, mask, source.Position.X, source.Position.Y, source.Size.X, source.Size.Y, dest.X, dest.Y, true);
    }

    public static void BlendRect(this FastImage me, FastImage other, Rect2I source, Vector2I dest) {
        Dump(me, other, null, source.Position.X, source.Position.Y, source.Size.X, source.Size.Y, dest.X, dest.Y, true);
    }

    public static void Dump(this FastImage me, FastImage other, FastImage? mask, int fromX, int fromY, int width, int height, int destX, int destY, bool blend) {
        for (var j = 0; j < height; j++) {
            for (var i = 0; i < width; i++) {
                var color = other.GetPixel(i + fromX, j + fromY);
                var allow = mask == null || mask.GetChannel(i + fromX, j + fromY, 3) > 0;
                if (allow) {
                    me.SetPixel(i + destX, j + destY, color, blend);
                }
            }
        }
    }
}