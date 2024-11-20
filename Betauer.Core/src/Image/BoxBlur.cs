namespace Betauer.Core.Image;

public class BoxBlur {
    public static float[,] ApplyBoxBlur(float[,] map, int blurSize) {
        var width = map.GetLength(0);
        var height = map.GetLength(1);
        var blurredMap = new float[width, height];

        for (var y = 0; y < height; y++) {
            for (var x = 0; x < width; x++) {
                var average = 0f;
                var count = 0;

                for (var ky = -blurSize; ky <= blurSize; ky++) {
                    for (var kx = -blurSize; kx <= blurSize; kx++) {
                        var pixelX = x + kx;
                        var pixelY = y + ky;

                        if (pixelX >= 0 && pixelX < width && pixelY >= 0 && pixelY < height) {
                            average += map[pixelX, pixelY];
                            count++;
                        }
                    }
                }

                blurredMap[x, y] = average / count;
            }
        }

        return blurredMap;
    }
}