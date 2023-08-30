using Godot;

namespace Betauer.Core.Image;

public class Layer {
    public FastImage Image;
    public bool Tiled;

    public Layer(FastImage image, bool tiled) {
        Image = image;
        Tiled = tiled;
    }

    public Color GetPixel(int x, int y) {
        if (Tiled) {
            return Image.GetPixel(x % Image.Width, y % Image.Height);
        }
        if (x >= Image.Width || y >= Image.Height) return new Color(0, 0, 0, 0);
        return Image.GetPixel(x, y);
    }
}

public class LayerImageComposition {
    private readonly int _layers;
    private readonly int _width;
    private readonly int _height;
    private readonly Layer[] _images;

    public LayerImageComposition(int layers, int width, int height) {
        _layers = layers;
        _width = width;
        _height = height;
        _images = new Layer[layers];
    }

    public void SetLayer(int layer, FastImage image, bool tiled = false) {
        _images[layer] = new Layer(image, tiled);
    }

    public FastImage Export() {
        var canvas = new FastImage(_width, _height, false, FastImage.DefaultFormat);
        for (var layer = 0; layer < _layers; layer++) {
            var layerImage = _images[layer];
            for (var x = 0; x < _width; x++) {
                for (var y = 0; y < _height; y++) {
                    if (layerImage == null) continue;
                    var layerPixel = layerImage.GetPixel(x, y);
                    if (layer == 0) {
                        canvas.SetPixel(x, y, layerPixel, false);
                    } else {
                        canvas.SetPixel(x, y, layerPixel, true);
                    }
                }
            }
        }

        return canvas;
    }

    public Layer GetLayer(int i) {
        var image = _images[i];
        if (image != null) return image;
        image = new Layer(new FastImage(_width, _height, false, FastImage.DefaultFormat), false);
        _images[i] = image;
        return image;
    }
}