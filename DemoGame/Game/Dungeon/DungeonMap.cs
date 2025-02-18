using Godot;
using System;
using Betauer.Camera.Control;
using Betauer.Core.Nodes.Events;
using Betauer.DI;
using Betauer.DI.Attributes;
using Betauer.NodePath;
using Betauer.Nodes;
using Veronenger.Game.Dungeon.World;

namespace Veronenger.Game.Dungeon;

public partial class DungeonMap : Node2D, IInjectable {

    CameraController CameraController;
    [Inject] public DungeonPlayerActions DungeonPlayerActions { get; set; }

    [NodePath("%TileMapLayer")] public TileMapLayer TileMapLayer { get; private set; }
    [NodePath("%Player")] public Sprite2D Player { get; private set; }

    public void PostInject() {
    }

    public Vector2I PlayerPos = Vector2I.Zero;
    public RogueWorld RogueWorld;

    public enum TileSetSourceId {
        SmAscii16x16 = 0
    }

    public void Configure(CameraController cameraController) {
        CameraController = cameraController;
        Ready += () => {
            // TileMapLayer.SetCell(new Vector2I(0, 0), (int)TileSetSourceId.SmAscii16x16, new Vector2I(3, 2));
            CameraController.Follow(Player);

            TileMapLayer.Clear();

            RogueWorld.TurnWorld.Cells.ForEach((cell) => {
                if (cell == null) return;
                var glyph = cell.Config.Glyph;
                TileMapLayer.SetCell(cell.Position, (int)TileSetSourceId.SmAscii16x16, new Vector2I((byte)glyph % 16, (byte)glyph / 16));
                if (PlayerPos == Vector2I.Zero && cell.Config.IsBlocked == false) {
                    PlayerPos = cell.Position;
                }
            });
            RogueWorld.TurnWorld.AddEntity(RogueWorld.Player, PlayerPos);
            Player.Position = TileMapLayer.MapToLocal(PlayerPos);

            // TileMapLayer.SetCell();
        };
    }

    public override void _Input(InputEvent @event) {
        if (RogueWorld.Player.IsWaiting) {
            PlayerEvent(@event);
        }
    }

    private void PlayerEvent(InputEvent @event) {
        if (DungeonPlayerActions.Right.IsJustPressed) {
            MoveTo(PlayerPos + Vector2I.Right);
        } else if (DungeonPlayerActions.Left.IsJustPressed) {
            MoveTo(PlayerPos + Vector2I.Left);
        } else if (DungeonPlayerActions.Up.IsJustPressed) {
            MoveTo(PlayerPos + Vector2I.Up);
        } else if (DungeonPlayerActions.Down.IsJustPressed) {
            MoveTo(PlayerPos + Vector2I.Down);
        }
    }

    public void Configure(RogueWorld rogueWorld) {
        RogueWorld = rogueWorld;
    }

    private void MoveTo(Vector2I targetPosition) {
        if (!RogueWorld.TurnWorld.IsBlocked(targetPosition)) {
            PlayerPos = targetPosition;
            RogueWorld.Player.SetResult(new ActionCommand(ActionType.Walk, targetPosition: targetPosition));
            Player.Position = TileMapLayer.MapToLocal(targetPosition);
        }
    }
}