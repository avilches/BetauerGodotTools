using System;
using Godot;

namespace Betauer.Core.Image;

public class FastTexture : FastImage {
    private ImageTexture _imageTexture;

    /// <summary>
    /// Link the FastTexture to the Sprite2D, so next calls to SetPixel() and Flush() can update the Sprite2D.Texture
    /// The link overwrites the Sprite2D.Texture with a new (and empty) image with the format specified. If you don't want to loose the previous texture, use
    /// the Load() method instead.
    /// </summary>
    /// <param name="sprite2D"></param>
    /// <param name="width"></param>
    /// <param name="height"></param>
    /// <param name="useMipmaps"></param>
    /// <param name="format"></param>
    public FastTexture Link(Sprite2D sprite2D, int width, int height, bool useMipmaps = false, Godot.Image.Format format = DefaultFormat) {
        Create(width, height, useMipmaps, format);
        _imageTexture = ImageTexture.CreateFromImage(Image);
        sprite2D.Texture = _imageTexture;
        return this;
    }

    /// <summary>
    /// Link the FastTexture to the TextureRect, so next calls to SetPixel() and Flush() can update the TextureRect.Texture
    /// The link overwrites the TextureRect.Texture with a new (and empty) image with the format specified. If you don't want to loose the previous texture, use
    /// the Load() method instead.
    /// </summary>
    /// <param name="textureRect"></param>
    /// <param name="width"></param>
    /// <param name="height"></param>
    /// <param name="useMipmaps"></param>
    /// <param name="format"></param>
    public FastTexture Link(TextureRect textureRect, int width, int height, bool useMipmaps = false, Godot.Image.Format format = DefaultFormat) {
        Create(width, height, useMipmaps, format);
        _imageTexture = CreateImageTexture();
        textureRect.Texture = _imageTexture;
        textureRect.Size = new Vector2(width, height);
        return this;
    }

    /// <summary>
    /// Load the image from the Sprite2D and link the FastTexture to it so the Flush() method can update it.
    /// If the Sprite2D.Texture is not an ImageTexture (like a CompressedTexture2D), it will read the image from the texture and it will be converted to an
    /// ImageTexture using the proper format (Rgba8 for png or Rgb8 for jpg)
    /// </summary>
    /// <param name="sprite2D"></param>
    public FastTexture Load(Sprite2D sprite2D) {
        var texture = sprite2D.Texture;
        if (texture == null) throw new Exception("Can't load Sprite2D without a texture");
        if (texture is ImageTexture imageTexture) {
            Load(texture.GetImage());
            _imageTexture = imageTexture;
        } else {
            var image = texture.GetImage();
            var textureImage = ImageTexture.CreateFromImage(image);
            Load(image);
            sprite2D.Texture = textureImage;
            _imageTexture = textureImage;
        }
        return this;
    }
    /// <summary>
    /// Load the image from the TextureRect and link the FastTexture to it so the Flush() method can update it.
    /// If the TextureRect.Texture is not an ImageTexture (like a CompressedTexture2D), it will read the image from the texture and it will be converted to an
    /// ImageTexture using the proper format (Rgba8 for png or Rgb8 for jpg)
    /// </summary>
    /// <param name="textureRect"></param>
    public FastTexture Load(TextureRect textureRect) {
        var texture = textureRect.Texture;
        if (texture == null) throw new Exception("Can't load TextureRect without a texture");
        if (texture is ImageTexture imageTexture) {
            Load(texture.GetImage());
            _imageTexture = imageTexture;
        } else {
            var image = texture.GetImage();
            var textureImage = ImageTexture.CreateFromImage(image);
            Load(image);
            textureRect.Texture = textureImage;
            _imageTexture = textureImage;
        }
        return this;
    }
    
    public FastTexture Load(ImageTexture textureImage) {
        _imageTexture = textureImage;
        Load(textureImage.GetImage());
        return this;
    }

    public override void Flush() {
        base.Flush();
        _imageTexture?.Update(Image); // Only null in constructor when calling to CreateImageTexture()
    }
}