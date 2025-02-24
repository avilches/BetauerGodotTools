using Betauer.NodePath;
using Godot;

namespace Veronenger.Game.Dungeon;

public partial class EntityNode : Node2D {

	[NodePath("Sprite2D")]
	public Sprite2D Sprite { get; private set; }

	public TileMapLayer TileMapLayer { get; set; }

	public void MoveToCell(Vector2I cell) {

	}

	public void MoveTo(Vector2I cell) {
		Position = TileMapLayer.MapToLocal(cell);
	}
}
