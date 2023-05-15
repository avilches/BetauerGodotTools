using Godot;

namespace Veronenger.Config;

public abstract class PickableConfig {
    public abstract void ConfigurePickableSprite2D(Sprite2D sprite2D);
    public abstract void ConfigureInventoryTextureRect(AtlasTexture atlasTexture);
}