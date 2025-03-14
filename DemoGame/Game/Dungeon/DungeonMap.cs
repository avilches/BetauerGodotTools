using Godot;
using System;
using Betauer.Camera.Control;
using Betauer.Core;
using Betauer.Core.Nodes.Events;
using Betauer.DI;
using Betauer.DI.Attributes;
using Betauer.DI.Factory;
using Betauer.NodePath;
using Betauer.Nodes;
using Betauer.Tools.Logging;
using Veronenger.Game.Dungeon.World;
using Veronenger.Game.Dungeon.World.Generation;

namespace Veronenger.Game.Dungeon;

public partial class DungeonMap : Node2D, IInjectable {
    public static readonly Logger Logger = LoggerFactory.GetLogger<DungeonMap>();

    CameraController CameraController;
    [Inject] public DungeonPlayerActions DungeonPlayerActions { get; set; }
    [Inject] private ITransient<EntityNode> EntityFactory { get; set; }

    [NodePath("%TileMapLayer")] public TileMapLayer TileMapLayer { get; private set; }

    public EntityNode PlayerSprite;

    public void PostInject() {
    }

    public RogueWorld RogueWorld;
    public Vector2I PlayerPos => Player.Location.Position;
    public PlayerEntity Player => RogueWorld.Player;


    public enum TileSetSourceId {
        SmAscii16x16 = 0
    }

    public void Configure(RogueWorld rogueWorld, CameraController cameraController) {
        RogueWorld = rogueWorld;
        CameraController = cameraController;
        AddChild(CameraController.Camera2D);
        PlayerSprite = CreateEntitySprite();
        Ready += () => { CameraController.Follow(PlayerSprite); };
    }

    private EntityNode CreateEntitySprite() {
        var playerSprite = EntityFactory.Create();
        playerSprite.TileMapLayer = TileMapLayer;
        AddChild(playerSprite);
        return playerSprite;
    }

    public void StartGame() {
        RogueWorld.StartNewGame(1);
        ConfigureWorld();
        UpdateTileMap();
        PlayerSprite.Position = TileMapLayer.MapToLocal(PlayerPos);
        RogueWorld.WorldMap.TurnSystem.Run();
    }

    private void ConfigureWorld() {
        Player.OnPositionChanged += (oldPosition, newPosition) => {
            // Logger.Debug("Sprite moved to "+newPosition);
            PlayerSprite.MoveTo(Player.Location.Position);
        };

        foreach (var entity in RogueWorld.WorldMap.Entities) {
            if (entity is EnemyEntity enemy) {
                var enemySprite = CreateEntitySprite();
                enemySprite.MoveTo(enemy.Location.Position);
            }
        }
    }

    public void UpdateTileMap() {
        TileMapLayer.Clear();
        RogueWorld.WorldMap.Cells.ForEach(cell => {
            if (cell == null) return;
            var glyph = cell.Config.Glyph;
            TileMapLayer.SetCell(cell.Position, (int)TileSetSourceId.SmAscii16x16, new Vector2I((byte)glyph % 16, (byte)glyph / 16));
        });
    }

    public override void _Input(InputEvent @event) {
        if (Player.IsWaiting) {
            PlayerEvent(@event);
        }
    }

    private void PlayerEvent(InputEvent @event) {
        if (DungeonPlayerActions.Right.IsJustPressed) {
            MoveTo(PlayerPos + Vector2I.Right);
            Console.WriteLine(RogueWorld.WorldMap[PlayerPos]?.GetMazeNode()?.GetTemplate());
        } else if (DungeonPlayerActions.Left.IsJustPressed) {
            MoveTo(PlayerPos + Vector2I.Left);
            Console.WriteLine(RogueWorld.WorldMap[PlayerPos]?.GetMazeNode()?.GetTemplate());
        } else if (DungeonPlayerActions.Up.IsJustPressed) {
            MoveTo(PlayerPos + Vector2I.Up);
            Console.WriteLine(RogueWorld.WorldMap[PlayerPos]?.GetMazeNode()?.GetTemplate());
        } else if (DungeonPlayerActions.Down.IsJustPressed) {
            MoveTo(PlayerPos + Vector2I.Down);
            Console.WriteLine(RogueWorld.WorldMap[PlayerPos]?.GetMazeNode()?.GetTemplate());
        }
    }

    private void MoveTo(Vector2I targetPosition) {
        if (!RogueWorld.WorldMap.IsBlocked(targetPosition)) {
            Player.SetResult(new ActionCommand(ActionType.Walk, targetPosition: targetPosition));
        }
    }
}