using Godot;

namespace Betauer.Core.Image;

public class Layer {
    public FastImage Image;
    public bool Seamless;

    public Layer(FastImage image, bool seamless) {
        Image = image;
        Seamless = seamless;
    }

    public bool Enabled { get; set; } = true;

    public Color GetPixel(int x, int y) {
        if (Seamless) {
            return Image.GetPixel(x % Image.Width, y % Image.Height);
        }
        if (x >= Image.Width || y >= Image.Height) return new Color(0, 0, 0, 0);
        return Image.GetPixel(x, y);
    }
}

public class LayerImageComposition {
    public int Layers { get; init; }
    public int Width { get; init; }
    public int Height { get; init; }
    
    private readonly Layer[] _images;

    public LayerImageComposition(int layers, int width, int height) {
        Layers = layers;
        Width = width;
        Height = height;
        _images = new Layer[layers];
    }

    public void SetLayer(int layer, FastImage image, bool seamless = false) {
        _images[layer] = new Layer(image, seamless);
    }

    public FastImage Export() {
        var canvas = new FastImage().Create(Width, Height, false, FastImage.DefaultFormat);
        for (var layer = 0; layer < Layers; layer++) {
            var layerImage = _images[layer];
            if (layerImage is not { Enabled: true }) continue;
            for (var x = 0; x < Width; x++) {
                for (var y = 0; y < Height; y++) {
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
        image = new Layer(new FastImage().Create(Width, Height, false, FastImage.DefaultFormat), false);
        _images[i] = image;
        return image;
    }
}