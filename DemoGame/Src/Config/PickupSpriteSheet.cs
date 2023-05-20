using Betauer.Application.Lifecycle;
using Betauer.DI.Attributes;
using Godot;

namespace Veronenger.Config;

[Singleton]
public class PickupSpriteSheet {
    private const int PickupItemW = 16;
    private const int PickupItemH = 16;
    private const int PickupColumns = 6;
    private const int PickupRows = 6;
    [Inject("Pickups")] private ResourceHolder<Texture2D> Pickups { get; set; }

    private const int Pickup2ItemW = 32;
    private const int Pickup2ItemH = 16;
    private const int Pickup2Columns = 3;
    private const int Pickup2Rows = 6;
    [Inject("Pickups2")] private ResourceHolder<Texture2D> Pickups2 { get; set; }
    
    public void ConfigurePickups(AtlasTexture texture, int row, int column) {
        texture.Region = new Rect2(column * PickupItemW, row * PickupItemH, PickupItemW, PickupItemH);
    }

    public void ConfigurePickups(Sprite2D sprite, int row, int column) {
        var frame = row * PickupColumns + column;
        sprite.Texture = Pickups.Get();
        sprite.Hframes = PickupColumns;
        sprite.Vframes = PickupRows;
        sprite.Frame = frame;
    }

    public void ConfigureBigPickups(Sprite2D sprite, int row, int column) { 
        var frame = row * Pickup2Columns + column;
        sprite.Texture = Pickups2.Get();
        sprite.Hframes = Pickup2Columns;
        sprite.Vframes = Pickup2Rows;
        sprite.Frame = frame;
    }
}