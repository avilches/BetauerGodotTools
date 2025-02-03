using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

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

    public TurnWorld() {
        Entities = _entities.AsReadOnly();
    }

    public void AddEntity(Entity entity) {
        if (!Entities.Contains(entity)) {
            _entities.Add(entity);
            _entitiesByName[entity.Name] = entity;
            OnEntityAdded?.Invoke(entity);
        }
    }

    public Entity GetEntity(string name) {
        return _entitiesByName[name];
    }

    public T GetEntity<T>(string name) where T : Entity {
        return _entitiesByName[name] as T;
    }

    public bool RemoveEntity(Entity entity) {
        if (!_entities.Remove(entity)) return false;
        entity.Destroy();
        _entitiesByName.Remove(entity.Name);
        OnEntityRemoved?.Invoke(entity);
        return true;
    }

    public void NextTick() {
        if (CurrentTick % TicksPerTurn == 0) {
            CurrentTurn++;
            // Console.WriteLine($"# Turn: {CurrentTurn}");
        }
        CurrentTick++;
        OnTick?.Invoke(CurrentTick);
    }

    public IEnumerable<Entity> GetEntities() {
        return Entities;
    }
}

public class TurnSystem(TurnWorld world) {
    public TurnWorld World { get; set; } = world;

    public async Task ProcessTickAsync() {
        World.NextTick();

        // ToArray() to avoid concurrent modification
        foreach (var entity in World.GetEntities().ToArray()) {
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