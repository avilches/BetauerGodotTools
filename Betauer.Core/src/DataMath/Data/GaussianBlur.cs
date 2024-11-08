using System.Threading.Tasks;
using Godot;

namespace Betauer.Core.DataMath.Data;

public class GaussianBlur {
    public static float[,] ApplyGaussianBlur(float[,] map, int radius) {
        var width = map.GetLength(0);
        var height = map.GetLength(1);
        var blurredMap = new float[width, height];
        var paddedMap = PadMap(map, radius);
        var kernel = GenerateKernel(radius);

        Parallel.For(0, height, y => {
            for (var x = 0; x < width; x++) {
                var sum = 0f;
                for (var ky = -radius; ky <= radius; ky++) {
                    for (var kx = -radius; kx <= radius; kx++) {
                        sum += paddedMap[x + kx + radius, y + ky + radius] * kernel[kx + radius, ky + radius];
                    }
                }
                blurredMap[x, y] = sum;
            }
        });

        return blurredMap;
    }

    // Genera un kernel gaussiano
    private static float[,] GenerateKernel(int radius) {
        var size = 2 * radius + 1;
        var sigma = radius / 3.0f;

        var kernel = new float[size, size];
        var sum = 0f;
        radius = size / 2;

        for (var y = -radius; y <= radius; y++) {
            for (var x = -radius; x <= radius; x++) {
                float exponent = (x * x + y * y) / (2 * sigma * sigma);
                kernel[x + radius, y + radius] = Mathf.Exp(-exponent) / (2 * Mathf.Pi * sigma * sigma);
                sum += kernel[x + radius, y + radius];
            }
        }

        // Normalize kernel
        for (var y = 0; y < size; y++) {
            for (var x = 0; x < size; x++) {
                kernel[x, y] /= sum;
            }
        }

        return kernel;
    }

    private static float[,] PadMap(float[,] map, int padding) {
        var width = map.GetLength(0);
        var height = map.GetLength(1);
        var paddedWidth = width + 2 * padding;
        var paddedHeight = height + 2 * padding;
        var paddedMap = new float[paddedWidth, paddedHeight];

        // Copy the original in the pad
        for (var y = 0; y < height; y++) {
            for (var x = 0; x < width; x++) {
                paddedMap[x + padding, y + padding] = map[x, y];
            }
        }

        // Fill the extra border out of the boundaries of the original using the mirror effect
        for (var y = 0; y < paddedHeight; y++) {
            for (var x = 0; x < padding; x++) {
                // Left side
                paddedMap[x, y] = paddedMap[2 * padding - x, y];
                // right side
                paddedMap[paddedWidth - 1 - x, y] = paddedMap[paddedWidth - 1 - 2 * padding + x, y];
            }
        }
        for (var x = 0; x < paddedWidth; x++) {
            for (var y = 0; y < padding; y++) {
                // top side
                paddedMap[x, y] = paddedMap[x, 2 * padding - y];
                // bottom side
                paddedMap[x, paddedHeight - 1 - y] = paddedMap[x, paddedHeight - 1 - 2 * padding + y];
            }
        }

        return paddedMap;
    }
}