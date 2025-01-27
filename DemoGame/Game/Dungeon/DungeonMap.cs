using Godot;
using System;
using Betauer.Camera.Control;
using Betauer.Core.Nodes.Events;
using Betauer.DI;
using Betauer.DI.Attributes;
using Betauer.NodePath;

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

    public void Configure(CameraController cameraController) {
        CameraController = cameraController;
        Ready += () => { CameraController.Follow(Player); };

        OnProcess += (d) => {
            if (DungeonPlayerActions.Up.IsPressed) {
                Player.Position += new Vector2(0, -1);
            }
        };

        DungeonPlayerActions.Start();
        DungeonPlayerActions.EnableAll();
    }
}