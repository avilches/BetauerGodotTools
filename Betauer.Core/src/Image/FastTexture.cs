using Godot;

namespace Betauer.Core.Image;

public class FastTexture : FastImage {
    private readonly ImageTexture _imageTexture;

    /// <summary>
    /// Link the Sprite2D to the FastImage and overwrites the texture with a new (and empty) image with the format specified.
    /// </summary>
    /// <param name="sprite2D"></param>
    /// <param name="width"></param>
    /// <param name="height"></param>
    /// <param name="useMipmaps"></param>
    /// <param name="format"></param>
    public FastTexture(Sprite2D sprite2D, int width, int height, bool useMipmaps = false, Godot.Image.Format format = DefaultFormat) : base(width, height, useMipmaps, format) {
        _imageTexture = CreateImageTexture();
        sprite2D.Texture = _imageTexture;
    }

    public FastTexture(TextureRect textureRect, int width, int height, bool useMipmaps = false, Godot.Image.Format format = DefaultFormat) : base(width, height, useMipmaps, format) {
        _imageTexture = CreateImageTexture();
        textureRect.Texture = _imageTexture;
        textureRect.Size = new Vector2(width, height);
    }

    /// <summary>
    /// Load the image from the Sprite2D and create a new FastImage. If the Sprite2D is not an ImageTexture (like a CompressedTexture2D), it will read the
    /// image from the texture and it will be converted to an ImageTexture using the proper format (Rgba8 for png or Rgb8 for jpg)
    /// </summary>
    /// <param name="sprite2D"></param>
    public FastTexture(Sprite2D sprite2D) : this((ImageTexture)EnsureTextureIsImageTexture(sprite2D).Texture) {
    }

    private static Sprite2D EnsureTextureIsImageTexture(Sprite2D sprite2D) {
        var sprite2DTexture = sprite2D.Texture;
        if (sprite2DTexture is not ImageTexture) {
            var image = sprite2DTexture.GetImage();
            sprite2D.Texture = ImageTexture.CreateFromImage(image);
        }
        return sprite2D;
    }

    public FastTexture(ImageTexture imageTexture) : base(imageTexture.GetImage()) {
        _imageTexture = imageTexture;
    }

    public override void Flush() {
        base.Flush();
        _imageTexture.Update(Image); // Only null in constructor when calling to CreateImageTexture()
    }
}