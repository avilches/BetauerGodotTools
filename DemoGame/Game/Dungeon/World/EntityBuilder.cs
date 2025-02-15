using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Veronenger.Game.Dungeon.World;

public class EntityBuilder {
    private readonly string _name;
    private readonly EntityStats _stats;
    private readonly List<MultiplierEffect> _initialEffects = new();

    private Func<Task<ActionCommand>>? _onDecideAction;
    private Func<bool>? _onCanAct;
    private Action<ActionCommand>? _onExecute;
    private Action? _onTickStart;
    private Action? _onTickEnd;

    private EntityBuilder(string name, EntityStats stats) {
        _name = name;
        _stats = stats;
    }

    public static EntityBuilder Create(string name, EntityStats stats) {
        return new EntityBuilder(name, stats);
    }

    // Status Effects
    public EntityBuilder SpeedEffect(MultiplierEffect effect) {
        _initialEffects.Add(effect);
        return this;
    }

    public EntityBuilder DecideAction(Func<Task<ActionCommand>> action) {
        _onDecideAction = action;
        return this;
    }

    public EntityBuilder DecideAction(Func<ActionCommand> action) {
        _onDecideAction = () => Task.FromResult(action());
        return this;
    }

    public EntityBuilder DecideAction(ActionCommand command) {
        _onDecideAction = () => Task.FromResult(command);
        return this;
    }

    public EntityBuilder DecideAction(ActionType type) {
        _onDecideAction = () => Task.FromResult(new ActionCommand(type, null));
        return this;
    }

    public EntityBuilder CanAct(Func<bool> canAct) {
        _onCanAct = canAct;
        return this;
    }

    public EntityBuilder Execute(Action<ActionCommand> onExecute) {
        _onExecute = onExecute;
        return this;
    }

    public EntityBuilder OnTickStart(Action onTickStart) {
        _onTickStart = onTickStart;
        return this;
    }

    public EntityBuilder OnTickEnd(Action onTickEnd) {
        _onTickEnd = onTickEnd;
        return this;
    }

    public Entity Build() {
        var entity = new Entity(_name, _stats);

        // Add all initial effects
        foreach (var effect in _initialEffects) {
            entity.AddSpeedEffect(effect);
        }

        // Set all handlers with null-safe defaults
        entity.OnDecideAction = _onDecideAction ?? (() => Task.FromResult(new ActionCommand(ActionType.Wait, entity)));
        entity.OnCanAct = _onCanAct ?? (() => true);

        if (_onExecute != null) entity.OnExecute += _onExecute;
        if (_onTickStart != null) entity.OnTickStart += _onTickStart;
        if (_onTickEnd != null) entity.OnTickEnd += _onTickEnd;

        return entity;
    }

    public EntityBlocking BuildAsync() {
        if (_onDecideAction != null) {
            throw new Exception("Cannot build async entity with custom DecideAction (the original DecideAction will be overwritten)");
        }
        return new EntityBlocking(Build());
    }
}