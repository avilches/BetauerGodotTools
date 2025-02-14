using Godot;
using System;
using Betauer.Camera.Control;
using Betauer.Core.Nodes.Events;
using Betauer.DI;
using Betauer.DI.Attributes;
using Betauer.NodePath;
using Betauer.Nodes;

namespace Veronenger.Game.Dungeon;

[Notifications(Process = false, PhysicsProcess = false)]
public partial class DungeonMap : Node2D, IInjectable {
    public override partial void _Notification(int what);

    CameraController CameraController;
    [Inject] public DungeonPlayerActions DungeonPlayerActions { get; set; }

    [NodePath("%TileMapLayer")] public TileMapLayer TileMapLayer { get; private set; }
    [NodePath("%Player")] public Sprite2D Player { get; private set; }

    public void PostInject() {
    }

    public Vector2I PlayerPos = Vector2I.Zero;

    public enum TileSetSourceId {
        SmAscii16x16 = 0
    }

    public void Configure(CameraController cameraController) {
        CameraController = cameraController;
        Ready += () => {
            // TileMapLayer.SetCell(new Vector2I(0, 0), (int)TileSetSourceId.SmAscii16x16, new Vector2I(3, 2));
            Player.Position = TileMapLayer.MapToLocal(PlayerPos);
            CameraController.Follow(Player);
        };

        OnProcess += (d) => {
            if (DungeonPlayerActions.Right.IsJustPressed) {
                PlayerPos += Vector2I.Right;
                Player.Position = TileMapLayer.MapToLocal(PlayerPos);
            } else if (DungeonPlayerActions.Left.IsJustPressed) {
                PlayerPos += Vector2I.Left;
                Player.Position = TileMapLayer.MapToLocal(PlayerPos);
            } else if (DungeonPlayerActions.Up.IsJustPressed) {
                PlayerPos += Vector2I.Up;
                Player.Position = TileMapLayer.MapToLocal(PlayerPos);
            } else if (DungeonPlayerActions.Down.IsJustPressed) {
                PlayerPos += Vector2I.Down;
                Player.Position = TileMapLayer.MapToLocal(PlayerPos);
            }
        };

    }
}