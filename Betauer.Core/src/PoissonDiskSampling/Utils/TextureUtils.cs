using System;
using System.IO;
using Godot;

namespace Betauer.Core.PoissonDiskSampling.Utils; 

public static class TextureUtils {
    /// <summary>
    /// Sets all pixels in the provided texture to the specified color.
    /// Must call <see cref="Texture2D.Apply"/> afterwards for the change to take effect.
    /// </summary>
    /// <param name="texture"></param>
    /// <param name="clearColor"></param>
    public static void Clear(Texture2D texture, Color clearColor) {
        // The .NET version used by Unity does not have the helpful Array.Fill method.
        int count = texture.GetWidth() * texture.GetHeight();
        Color[] buffer = new Color[count];

        for (int i = 0; i < count; ++i) {
            buffer[i] = clearColor;
        }

        // texture.SetPixels(0, 0, texture.width, texture.height, buffer);
        throw new Exception("Not implemented");
    }

    /// <summary>
    /// Adds a solid color point with given radius at the position. If radius is 0, sets only the specified pixel.
    /// Must call <see cref="Texture2D.Apply"/> afterwards for the change to take effect.
    /// </summary>
    /// <param name="texture"></param>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <param name="radius"></param>
    /// <param name="color"></param>
    public static void AddPoint(Texture2D texture, int x, int y, int radius, Color color) {
        Vector2 xy = new Vector2(x, y);

        var image = texture.GetImage();
        for (int iy = (y - radius); iy <= (y + radius); ++iy) {
            for (int ix = (x - radius); ix <= (x + radius); ++ix) {
                int d = (int)xy.DistanceTo(new Vector2(ix, iy));

                if (d <= radius) {
                    image.SetPixel(ix, iy, color);
                }
            }
        }
    }

    /// <summary>
    /// Saves the texture as a PNG file on disk. Make sure <see cref="Texture2D.Apply"/> was called.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="path"></param>
    public static void SavePng(Texture2D source, string directory, string filename) {
        string path = Path.Combine(directory, $"{filename}.png");
        // Debug.Log($"Saving texture to '{path}' ...");

        // var bytes = source.EncodeToPNG();
        // Directory.CreateDirectory(directory);

        // File.WriteAllBytes(path, bytes);
        throw new Exception("Not implemented");
    }

    /// <summary>
    /// Saves the texture as a PNG file on disk, to the <c>Debug</c> directory. Make sure <see cref="Texture2D.Apply"/> was called.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="filename"></param>
    public static void DebugSavePng(Texture2D source, string filename) {
#if DEBUG
        SavePng(source, "Debug", filename);
#endif
    }
}