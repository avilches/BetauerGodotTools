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

public abstract class EntityAsyncBase : EntityBase, IEntityAsync {
    protected EntityAsyncBase(EntityStats stats) : base(stats) {
    }

    protected EntityAsyncBase(string name, EntityStats stats) : base(name, stats) {
    }

    public abstract Task<ActionCommand> DecideAction();

    public override void Execute(ActionCommand actionCommand) {
        CurrentEnergy -= actionCommand.EnergyCost;
        DoExecute(actionCommand);
    }

    public abstract void DoExecute(ActionCommand actionCommand);
}

public class EntityAsync : EntityAsyncBase, IEntityAsync {
    public Func<Task<ActionCommand>> OnDecideAction { get; set; }
    public event Action<ActionCommand>? OnExecute;

    public EntityAsync(EntityStats stats) : base(stats) {
        OnDecideAction = () => Task.FromResult(new ActionCommand(ActionType.Wait, this));
    }

    public EntityAsync(string name, EntityStats stats) : base(name, stats) {
        OnDecideAction = () => Task.FromResult(new ActionCommand(ActionType.Wait, this));
    }

    public override Task<ActionCommand> DecideAction() {
        return OnDecideAction.Invoke();
    }

    public override void DoExecute(ActionCommand actionCommand) {
        OnExecute?.Invoke(actionCommand);
    }
}

public abstract class SchedulingEntityBase : EntityBase, IEntityAsync {
    private TaskCompletionSource<ActionCommand>? _promise;

    protected SchedulingEntityBase(EntityStats stats) : base(stats) {
    }

    protected SchedulingEntityBase(string name, EntityStats stats) : base(name, stats) {
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


    public override void Execute(ActionCommand actionCommand) {
        CurrentEnergy -= actionCommand.EnergyCost;
        DoExecute(actionCommand);
    }

    public abstract void DoExecute(ActionCommand actionCommand);
}

public class SchedulingEntity : SchedulingEntityBase {
    public event Action<ActionCommand>? OnExecute;

    public SchedulingEntity(EntityStats stats) : base(stats) {
    }

    public SchedulingEntity(string name, EntityStats stats) : base(name, stats) {
    }

    public override void DoExecute(ActionCommand actionCommand) {
        OnExecute?.Invoke(actionCommand);
    }
}

public abstract class EntitySyncBase : EntityBase, IEntitySync {
    protected EntitySyncBase(EntityStats stats) : base(stats) {
    }

    protected EntitySyncBase(string name, EntityStats stats) : base(name, stats) {
    }

    public abstract ActionCommand DecideAction();

    public override void Execute(ActionCommand actionCommand) {
        CurrentEnergy -= actionCommand.EnergyCost;
        DoExecute(actionCommand);
    }

    public abstract void DoExecute(ActionCommand actionCommand);
}


public class EntitySync : EntitySyncBase, IEntitySync {
    public Func<ActionCommand> OnDecideAction { get; set; }
    public event Action<ActionCommand>? OnExecute;


    public EntitySync(EntityStats stats) : base(stats) {
        OnDecideAction = () => new ActionCommand(ActionType.Wait, this);
    }

    public EntitySync(string name, EntityStats stats) : base(name, stats) {
        OnDecideAction = () => new ActionCommand(ActionType.Wait, this);
    }

    public override ActionCommand DecideAction() {
        return OnDecideAction.Invoke();
    }

    public override void DoExecute(ActionCommand actionCommand) {
        OnExecute?.Invoke(actionCommand);
    }
}

public abstract class EntityBase {
    public string Name { get; }
    public EntityStats BaseStats { get; }
    public int CurrentEnergy { get; protected set; } = 0;
    public List<MultiplierEffect>? SpeedEffects { get; private set; } = null;

    public bool Removed = false;

    public Func<bool> OnCanAct { get; set; } = () => true;

    public char Glyph { get; set; }

    public event Action? OnTickStart;
    public event Action? OnTickEnd;
    public event Action? OnRemoved;
    public event Action? OnAdded;
    public event Action<Vector2I, Vector2I>? OnPositionChanged;

    public WorldMap WorldMap => Location?.WorldMap;
    public WorldCell Cell => Location?.Cell;
    public Location Location { get; internal set; }

    protected EntityBase(EntityStats stats) : this(Guid.NewGuid().ToString(), stats) {
    }

    protected EntityBase(string name, EntityStats stats) {
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
        OnAdded?.Invoke();
    }

    internal void InvokeOnWorldRemoved() {
        OnRemoved?.Invoke();
    }

    internal void InvokeOnPositionChanged(Vector2I from, Vector2I to) {
        OnPositionChanged?.Invoke(from, to);
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

    public abstract void Execute(ActionCommand actionCommand);
}