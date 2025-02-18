using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Betauer.Core.DataMath;
using Betauer.Tools.Logging;
using Godot;

namespace Veronenger.Game.Dungeon.World;

public class TurnWorld {
    private readonly List<EntityBase> _entities = [];
    private readonly Dictionary<string, EntityBase> _entitiesByName = [];

    public Array2D<WorldCell?> Cells { get; private set; }

    public int TicksPerTurn { get; set; } = 10;
    public int CurrentTick { get; private set; } = 0;
    public int CurrentTurn { get; private set; } = 0;

    public event Action<EntityBase>? OnEntityAdded;
    public event Action<EntityBase>? OnEntityRemoved;
    public event Action<int>? OnTick;

    public IReadOnlyList<EntityBase> Entities { get; }
    public int Width => Cells.Width;
    public int Height => Cells.Height;
    public Rect2I Bounds => Cells.Bounds;

    public TurnWorld(int width, int height) : this(new Array2D<WorldCell?>(width, height)) {
    }

    public TurnWorld(Array2D<WorldCell?> cells) {
        Cells = cells;
        Entities = _entities.AsReadOnly();
    }

    public WorldCell? this[Vector2I position] => Cells[position];

    public WorldCell? this[int y, int x] => Cells[y, x];

    // public WorldCell GetOrCreate(Vector2I position, CellType defaultCellType) =>
    //     Cells.GetOrSet(position, () => new WorldCell(defaultCellType, position))!;

    public bool IsCellType(Vector2I position, CellType type) =>
        Cells[position] != null && Cells[position]!.Type == type;

    public bool HasCellEntity(Vector2I position, EntityBase entity) =>
        Cells[position] != null && Cells[position]!.Entities.Contains(entity);

    public bool IsValidPosition(Vector2I position) {
        return Cells.IsInBounds(position);
    }

    public bool IsBlocked(Vector2I position) {
        return !IsValidPosition(position) || Cells[position] != null && Cells[position]!.Config.IsBlocked;
    }

    public EntityBase GetEntity(string name) {
        return _entitiesByName[name];
    }

    public T GetEntity<T>(string name) where T : EntityBase {
        return _entitiesByName[name] as T;
    }

    public void AddEntity(EntityBase entity, Vector2I position) {
        if (Entities.Contains(entity)) {
            throw new InvalidOperationException($"Entity already added to world: {entity}");
        }
        if (!Cells.IsInBounds(position)) {
            throw new InvalidOperationException($"Invalid position: {position}");
        }
        var worldCell = Cells[position];
        if (worldCell == null) {
            throw new InvalidOperationException($"Cell is empty: {position}");
        }

        // Update the data
        entity.Location = new Location(entity, this, position);

        AddEntityToList(entity);

        _entitiesByName[entity.Name] = entity;
        worldCell.AddEntity(entity);

        // Trigger events
        entity.InvokeOnWorldAdded();
        OnEntityAdded?.Invoke(entity);
    }

    private void AddEntityToList(EntityBase entity) {
        if (entity is IEntityAsync) { // IEntityAsync entities go first, but ordered by insertion
            var insertIndex = 0;
            for (var i = 0; i < _entities.Count; i++) {
                if (_entities[i] is IEntityAsync) {
                    insertIndex = i + 1;
                } else {
                    break; // Stop when we hit the first non-IEntityAsync
                }
            }
            _entities.Insert(insertIndex, entity);
        } else if (entity is IEntitySync) { // IEntitySync goes at the end
            _entities.Add(entity);
        } else {
            throw new InvalidOperationException($"Unknown entity type: {entity.GetType()}: the type must implements {nameof(IEntityAsync)} or {nameof(IEntitySync)}");
        }
    }

    public bool RemoveEntity(EntityBase entity) {
        if (!_entities.Contains(entity)) return false;


        // Trigger events where the data is still valid
        entity.InvokeOnWorldRemoved();
        OnEntityRemoved?.Invoke(entity);

        // Remove the data
        _entities.Remove(entity);
        _entitiesByName.Remove(entity.Name);
        Cells[entity.Location.Position]?.RemoveEntity(entity);
        entity.Location = null;
        return true;
    }

    internal void MoveEntity(EntityBase entity, Vector2I origin, Vector2I to) {
        var worldCell = Cells[to];
        if (worldCell == null) {
            throw new InvalidOperationException($"Cell is empty: {to}");
        }


        this[origin]!.RemoveEntity(entity);
        worldCell.AddEntity(entity);
        entity.InvokeOnMoved(origin, to);
    }

    public void NextTick() {
        if (CurrentTick % TicksPerTurn == 0) {
            CurrentTurn++;
            // Console.WriteLine($"# Turn: {CurrentTurn}");
        }
        CurrentTick++;
        OnTick?.Invoke(CurrentTick);
    }

    public TurnSystem CreateTurnSystem() {
        return new TurnSystem(this);
    }
}

public class TurnSystem {
    private readonly TurnWorld _world;

    private static readonly Logger Logger = LoggerFactory.GetLogger<TurnSystem>();
    public bool Running { get; private set; } = false;
    public bool Busy { get; private set; } = false;

    internal TurnSystem(TurnWorld world) {
        _world = world;
    }

    public async Task Run(double maxLoopDurationSecs = 0.0166d, Func<Task>? awaiter = null) {
        if (Running) return;
        Running = true;

        var loopStartTime = DateTime.Now;

        while (Running) {
            await ProcessTickAsync();

            if (awaiter != null) {
                var loopDuration = (DateTime.Now - loopStartTime).TotalSeconds;
                if (loopDuration > maxLoopDurationSecs) {
                    await awaiter();
                    loopStartTime = DateTime.Now;
                }
            }
        }
    }

    public async Task ProcessTickAsync() {
        if (Busy) return;
        Running = true;
        Busy = true;
        _world.NextTick();

        foreach (var entity in _world.Entities.ToArray()) { // ToArray() to avoid concurrent modification
            // AddEntity ensures all the Async entities goes first

            entity.TickStart();
            if (entity.CanAct()) {
                if (entity is IEntitySync syncEntity) {
                    entity.Execute(syncEntity.DecideAction());

                } else if (entity is IEntityAsync asyncEntity) {
                    var action = await DecideAction(entity.Name, asyncEntity);
                    if (action != null) entity.Execute(action);
                    if (!Running || action?.Type == ActionType.EndGame) {
                        break;
                    }
                }
            }
            entity.TickEnd();
        }
        Busy = false;
    }

    public void Stop() {
        Running = false;
    }

    private async Task<ActionCommand?> DecideAction(string name, IEntityAsync entity) {
        while (Running) {
            try {
                var decideTask = entity.DecideAction();
                var timeoutTask = Task.Delay(166);
                var completedTask = await Task.WhenAny(decideTask, timeoutTask);
                if (completedTask == decideTask) {
                    return await decideTask;
                }
            } catch (Exception e) {
                Logger.Error($"Error deciding action for {name}: {e.Message}");
                return null;
            }
        }
        return null;
    }
}