using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Betauer.Core.DataMath;
using Godot;

namespace Veronenger.Game.Dungeon.World;

public class TurnWorld {
    public static int TicksPerTurn = 10;

    private readonly List<Entity> _entities = [];
    private readonly Dictionary<string, Entity> _entitiesByName = [];

    public IReadOnlyList<Entity> Entities { get; }
    public int CurrentTick { get; private set; } = 0;
    public int CurrentTurn { get; private set; } = 0;

    public event Action<Entity>? OnEntityAdded;
    public event Action<Entity>? OnEntityRemoved;
    public event Action<int>? OnTick;

    public Array2D<WorldCell> Cells { get; }

    public TurnWorld(int width, int height) {
        Cells = new Array2D<WorldCell>(width, height);
        Cells.Load(_ => new WorldCell());
        for (var y = 0; y < height; y++) {
            for (var x = 0; x < width; x++) {
                Cells[x, y] = new WorldCell();
            }
        }
        Entities = _entities.AsReadOnly();
    }

    public TurnWorld() : this(10, 10) { }

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
        if (!Cells.IsValidPosition(position)) {
            throw new InvalidOperationException($"Invalid position: {position}");
        }

        // Update the data
        entity.World = this;
        entity.Location = new Location(this, entity, position);

        _entities.Add(entity);
        _entitiesByName[entity.Name] = entity;
        Cells[entity.Location.Position].AddEntity(entity);

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
        Cells[entity.Location.Position].RemoveEntity(entity);
        entity.World = null;
        entity.Location = null;
        return true;
    }

    internal void UpdatedEntityPosition(Entity entity, Vector2I oldPosition) {
        Cells[oldPosition].RemoveEntity(entity);
        Cells[entity.Location.Position].AddEntity(entity);
    }

    public void NextTick() {
        if (CurrentTick % TicksPerTurn == 0) {
            CurrentTurn++;
            // Console.WriteLine($"# Turn: {CurrentTurn}");
        }
        CurrentTick++;
        OnTick?.Invoke(CurrentTick);
    }

    public bool IsValidPosition(Vector2I position) {
        return Cells.IsValidPosition(position);
    }
}

public class TurnSystem(TurnWorld world) {
    public TurnWorld World { get; set; } = world;

    public async Task ProcessTickAsync() {
        World.NextTick();

        // ToArray() to avoid concurrent modification
        foreach (var entity in World.Entities.ToArray()) {
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
    private TurnSystem TurnSystem { get; } = turnSystem;

    private Exception? _exception = null;

    public void _Process() {
        if (_exception != null) {
            var e = _exception;
            _exception = null;
            throw e;
        }
        if (Busy) return;
        Busy = true;
        TurnSystem.ProcessTickAsync().ContinueWith(t => {
            if (t.IsFaulted) {
                _exception = t.Exception?.GetBaseException() ?? t.Exception;
            }
            Busy = false;
        });
    }
}