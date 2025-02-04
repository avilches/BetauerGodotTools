using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Betauer.Core.DataMath;
using Godot;

namespace Veronenger.Game.Dungeon.World;

public class TurnWorld {
    private readonly List<Entity> _entities = [];
    private readonly Dictionary<string, Entity> _entitiesByName = [];
    private readonly Array2D<WorldCell?> _cells;

    public CellType DefaultCellType { get; set; } = CellType.Floor;

    public int TicksPerTurn { get; set; } = 10;
    public int CurrentTick { get; private set; } = 0;
    public int CurrentTurn { get; private set; } = 0;

    public event Action<Entity>? OnEntityAdded;
    public event Action<Entity>? OnEntityRemoved;
    public event Action<int>? OnTick;

    public IReadOnlyList<Entity> Entities { get; }
    public int Width => _cells.Width;
    public int Height => _cells.Height;
    public Rect2I Bounds => _cells.Bounds;

    public TurnWorld(int width, int height) {
        _cells = new Array2D<WorldCell?>(width, height);
        Entities = _entities.AsReadOnly();
    }

    public WorldCell? this[Vector2I position] => _cells[position];

    public WorldCell? this[int y, int x] => _cells[y, x];

    public WorldCell GetOrCreate(Vector2I position) =>
        _cells.GetOrSet(position, () => new WorldCell(DefaultCellType, position))!;

    public bool IsCellType(Vector2I position, CellType type) =>
        _cells[position] != null && _cells[position]!.Type == type;

    public bool HasCellEntity(Vector2I position, Entity entity) =>
        _cells[position] != null && _cells[position]!.Entities.Contains(entity);

    public bool IsValidPosition(Vector2I position) {
        return _cells.IsValidPosition(position);
    }

    public void AddEntity(Entity entity) {
        AddEntity(entity, Vector2I.Zero);
    }

    public Entity GetEntity(string name) {
        return _entitiesByName[name];
    }

    public T GetEntity<T>(string name) where T : Entity {
        return _entitiesByName[name] as T;
    }

    public void AddEntity(Entity entity, Vector2I position) {
        if (Entities.Contains(entity)) {
            throw new InvalidOperationException($"Entity already added to world: {entity}");
        }
        if (!_cells.IsValidPosition(position)) {
            throw new InvalidOperationException($"Invalid position: {position}");
        }

        // Update the data
        entity.Location = new Location(entity, this, position);

        _entities.Add(entity);
        _entitiesByName[entity.Name] = entity;
        GetOrCreate(entity.Location.Position).AddEntity(entity);

        // Trigger events
        entity.InvokeOnWorldAdded();
        OnEntityAdded?.Invoke(entity);
    }

    public bool RemoveEntity(Entity entity) {
        if (!_entities.Contains(entity)) return false;

        // Trigger events where the data is still valid
        entity.InvokeOnWorldRemoved();
        OnEntityRemoved?.Invoke(entity);

        // Remove the data
        _entities.Remove(entity);
        _entitiesByName.Remove(entity.Name);
        GetOrCreate(entity.Location.Position).RemoveEntity(entity);
        entity.Location = null;
        return true;
    }

    internal void MoveEntity(Entity entity, Vector2I origin, Vector2I to) {
        this[origin]!.RemoveEntity(entity);
        GetOrCreate(to).AddEntity(entity);
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
}

public class TurnSystem(TurnWorld world) {
    public async Task ProcessTickAsync() {
        world.NextTick();

        // ToArray() to avoid concurrent modification
        foreach (var entity in world.Entities.ToArray()) {
            entity.TickStart();
            if (entity.CanAct()) {
                var action = await entity.DecideAction();
                entity.Execute(action);
            }
            entity.TickEnd();
        }
    }
}

public class TurnSystemProcess(TurnSystem turnSystem) {
    public bool Busy { get; private set; } = false;

    private Exception? _exception = null;

    public void _Process() {
        if (_exception != null) {
            var e = _exception;
            _exception = null;
            throw e;
        }
        if (Busy) return;
        Busy = true;
        turnSystem.ProcessTickAsync().ContinueWith(t => {
            if (t.IsFaulted) {
                _exception = t.Exception?.GetBaseException() ?? t.Exception;
            }
            Busy = false;
        });
    }
}