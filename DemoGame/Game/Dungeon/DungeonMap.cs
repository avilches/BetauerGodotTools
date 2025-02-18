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
    [NodePath("%Player")] public Sprite2D PlayerSprite { get; private set; }

    public void PostInject() {
    }

    public Vector2I PlayerPos => Player.Location.Position;

    public PlayerEntity Player;

    public RogueWorld RogueWorld;

    public enum TileSetSourceId {
        SmAscii16x16 = 0
    }

    public void Configure(RogueWorld rogueWorld, CameraController cameraController) {
        RogueWorld = rogueWorld;
        CameraController = cameraController;
        AddChild(CameraController.Camera2D);
        Ready += () => {
            CameraController.Follow(PlayerSprite);
        };
    }

    public void UpdateTileMap() {
        TileMapLayer.Clear();
        RogueWorld.WorldMap.Cells.ForEach(cell => {
            if (cell == null) return;
            var glyph = cell.Config.Glyph;
            TileMapLayer.SetCell(cell.Position, (int)TileSetSourceId.SmAscii16x16, new Vector2I((byte)glyph % 16, (byte)glyph / 16));
        });
        PlayerSprite.Position = TileMapLayer.MapToLocal(PlayerPos);
    }

    public override void _Input(InputEvent @event) {
        if (Player.IsWaiting) {
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

    private void MoveTo(Vector2I targetPosition) {
        if (!RogueWorld.WorldMap.IsBlocked(targetPosition)) {
            Player.SetResult(new ActionCommand(ActionType.Walk, targetPosition: targetPosition));
        }
    }

    public void StartGame() {
        var result = RogueWorld.GenerateWorldMap(1);
        Player = new PlayerEntity("Player", new EntityStats { BaseSpeed = 100 });
        ConfigurePlayer();
        RogueWorld.WorldMap.AddEntity(Player, result.StartCell.Position);
        UpdateTileMap();
        RogueWorld.WorldMap.TurnSystem.Run();
    }

    private void ConfigurePlayer() {
        Player.OnMoved += (oldPosition, newPosition) => {
            PlayerSprite.Position = TileMapLayer.MapToLocal(PlayerPos);
        };
    }
}