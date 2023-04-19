using System;
using Godot;

namespace Veronenger.Config;

public class PickableConfig {
    public Action<Sprite2D>? ConfigurePickableSprite2D;
    public Action<AtlasTexture>? ConfigureInventoryTextureRect;
}