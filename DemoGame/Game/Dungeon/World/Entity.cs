using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Godot;

namespace Veronenger.Game.Dungeon.World;

public interface IEntityAsync {
    Task<ActionCommand> DecideAction();
}

public interface IEntitySync {
    ActionCommand DecideAction();
}

public class BlockingEntity : EntityBase, IEntityAsync {
    public Func<Task<ActionCommand>> OnDecideAction { get; set; }

    public BlockingEntity(EntityStats stats) : base(stats) {
        OnDecideAction = () => Task.FromResult(new ActionCommand(ActionType.Wait, this));
    }

    public BlockingEntity(string name, EntityStats stats) : base(name, stats) {
        OnDecideAction = () => Task.FromResult(new ActionCommand(ActionType.Wait, this));
    }

    public Task<ActionCommand> DecideAction() {
        return OnDecideAction.Invoke();
    }
}

public class SchedulingEntity : EntityBase, IEntityAsync {
    private TaskCompletionSource<ActionCommand>? _promise;

    public SchedulingEntity(EntityStats stats) : base(stats) {
    }

    public SchedulingEntity(string name, EntityStats stats) : base(name, stats) {
    }

    public Queue<ActionCommand> Queue { get; } = [];

    public bool IsWaiting => _promise != null;

    public void SetResult(ActionCommand actionCommand) {
        var promise = _promise;
        if (promise == null) {
            throw new Exception("No action to resolve");
        }
        if (promise.Task.IsCompleted) {
            throw new Exception("Player action already set");
        }
        _promise = null;
        promise.TrySetResult(actionCommand);
    }

    public void ScheduleNextAction(ActionCommand nextActionCommand) {
        Queue.Enqueue(nextActionCommand);
    }

    public Task<ActionCommand> DecideAction() {
        if (Queue.Count > 0) {
            var action = Queue.Dequeue();
            return Task.FromResult(action);
        }
        _promise ??= new TaskCompletionSource<ActionCommand>();
        return _promise.Task;
    }
}

public class Entity : EntityBase, IEntitySync {
    public Func<ActionCommand> OnDecideAction { get; set; }

    public Entity(EntityStats stats) : base(stats) {
        OnDecideAction = () => new ActionCommand(ActionType.Wait, this);
    }

    public Entity(string name, EntityStats stats) : base(name, stats) {
        OnDecideAction = () => new ActionCommand(ActionType.Wait, this);
    }

    public ActionCommand DecideAction() {
        return OnDecideAction.Invoke();
    }
}

public class EntityBase {
    public string Name { get; }
    public EntityStats BaseStats { get; }
    public int CurrentEnergy { get; protected set; } = 0;
    public List<MultiplierEffect>? SpeedEffects { get; private set; } = null;

    public Func<bool> OnCanAct { get; set; } = () => true;

    public event Action<ActionCommand>? OnExecute;
    public event Action? OnTickStart;
    public event Action? OnTickEnd;
    public event Action? OnWorldRemoved;
    public event Action? OnWorldAdded;
    public event Action<Vector2I, Vector2I>? OnMoved;

    public WorldMap WorldMap => Location?.WorldMap;
    public WorldCell Cell => Location?.Cell;
    public Location Location { get; internal set; }

    public EntityBase(EntityStats stats) : this(Guid.NewGuid().ToString(), stats) {
    }

    public EntityBase(string name, EntityStats stats) {
        Name = name;
        BaseStats = stats;
    }

    public EntityBase Configure(Action<EntityBase> config) {
        config(this);
        return this;
    }

    public MultiplierEffect AddSpeedEffect(string name, float multiplier, int turns) {
        var effect = MultiplierEffect.Ticks(name, multiplier, turns * WorldMap.TicksPerTurn);
        AddSpeedEffect(effect);
        return effect;
    }

    public void AddSpeedEffect(MultiplierEffect effect) {
        SpeedEffects ??= [];
        SpeedEffects.Add(effect);
    }

    internal void InvokeOnWorldAdded() {
        OnWorldAdded?.Invoke();
    }

    internal void InvokeOnWorldRemoved() {
        OnWorldRemoved?.Invoke();
    }

    internal void InvokeOnMoved(Vector2I from, Vector2I to) {
        OnMoved?.Invoke(from, to);
    }

    public int GetCurrentSpeed() {
        if (SpeedEffects == null) {
            return BaseStats.BaseSpeed;
        }
        var multiplier = SpeedEffects.Aggregate(1f, (current, effect) => current * effect.Multiplier);
        return Mathf.RoundToInt(BaseStats.BaseSpeed * multiplier);
    }

    public void TickStart() {
        OnTickStart?.Invoke();
    }

    public bool CanAct() => CurrentEnergy >= 0 && OnCanAct.Invoke();

    public void Execute(ActionCommand actionCommand) {
        CurrentEnergy -= actionCommand.EnergyCost;
        // Console.WriteLine($"{Name} -{action.EnergyCost} = {CurrentEnergy} (action: {action.Config.Type})");
        OnExecute?.Invoke(actionCommand);
    }

    public void TickEnd() {
        var currentSpeed = GetCurrentSpeed();
        CurrentEnergy += currentSpeed;
        // Console.WriteLine($"{Name} +{currentSpeed} = " + CurrentEnergy);
        RemoveExpiredEffects();

        OnTickEnd?.Invoke();
    }

    private void RemoveExpiredEffects() {
        if (SpeedEffects == null) return;
        for (var i = SpeedEffects.Count - 1; i >= 0; i--) {
            var effect = SpeedEffects[i];
            effect.RemainingTicks--;
            if (effect.RemainingTicks <= 0) {
                SpeedEffects.RemoveAt(i);
            }
        }
    }
}