using System;
using System.Collections.Generic;
using System.Linq;
using Betauer.Core.DataMath;
using Godot;

namespace Veronenger.Game.Dungeon.World;

public class WorldMap {
    private readonly List<EntityBase> _entities = [];
    private readonly Dictionary<string, EntityBase> _entitiesByName = [];
    private readonly Queue<(EntityBase, Vector2I)> _pendingEntities = new();

    public Array2D<WorldCell?> Cells { get; private set; }

    public TurnSystem TurnSystem { get; private set; }

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

    public WorldMap(int width, int height) : this(new Array2D<WorldCell?>(width, height)) {
    }

    public WorldMap(Array2D<WorldCell?> cells) {
        Cells = cells;
        Entities = _entities.AsReadOnly();
        TurnSystem = new TurnSystem(this);
    }

    public WorldCell? this[Vector2I position] => Cells[position];

    public WorldCell? this[int y, int x] => Cells[y, x];

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

    public void QueueAddEntity(EntityBase entity, Vector2I position) {
        _pendingEntities.Enqueue((entity, position));
    }

    public void ApplyPendingChanges() {
        while (_pendingEntities.Count > 0) {
            var (entity, position) = _pendingEntities.Dequeue();
            AddEntity(entity, position);
        }
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

    internal void RemoveEntity(EntityBase entity) {
        if (!_entities.Contains(entity) || !entity.Removed) return;

        // Trigger events where the data is still valid
        entity.InvokeOnWorldRemoved();
        OnEntityRemoved?.Invoke(entity);

        // Remove the data
        _entities.Remove(entity);
        _entitiesByName.Remove(entity.Name);
        Cells[entity.Location.Position]?.RemoveEntity(entity);
        entity.Location = null;
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
}