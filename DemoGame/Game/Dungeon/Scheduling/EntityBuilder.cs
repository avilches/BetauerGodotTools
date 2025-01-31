using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Veronenger.Game.Dungeon.Scheduling;

public class EntityBuilder {
    private readonly string _name;
    private readonly EntityStats _stats;
    private readonly Dictionary<Type, IComponent> _components = new();
    private readonly List<MultiplierEffect> _initialEffects = new();

    private Func<TurnContext, Task<EntityAction>>? _onDecideAction;
    private Func<TurnContext, bool>? _onCanAct;
    private Action<TurnContext, EntityAction>? _onExecute;
    private Action<TurnContext>? _onTickStart;
    private Action<TurnContext>? _onTickEnd;

    private EntityBuilder(string name, EntityStats stats) {
        _name = name;
        _stats = stats;
    }

    public static EntityBuilder Create(string name, EntityStats stats) {
        return new EntityBuilder(name, stats);
    }

    // Components
    public EntityBuilder Component<T>(T component) where T : IComponent {
        _components[typeof(T)] = component;
        return this;
    }

    // Status Effects
    public EntityBuilder SpeedEffect(MultiplierEffect effect) {
        _initialEffects.Add(effect);
        return this;
    }

    // Action handlers
    public EntityBuilder DecideAction(Func<TurnContext, Task<EntityAction>> action) {
        _onDecideAction = action;
        return this;
    }

    public EntityBuilder DecideAction(Func<TurnContext, EntityAction> action) {
        _onDecideAction = context => Task.FromResult(action(context));
        return this;
    }

    public EntityBuilder DecideAction(EntityAction action) {
        _onDecideAction = _ => Task.FromResult(action);
        return this;
    }

    public EntityBuilder DecideAction(ActionType type) {
        _onDecideAction = _ => Task.FromResult(new EntityAction(type, null));
        return this;
    }

    public EntityBuilder CanAct(Func<TurnContext, bool> canAct) {
        _onCanAct = canAct;
        return this;
    }

    public EntityBuilder Execute(Action<TurnContext, EntityAction> execute) {
        _onExecute = execute;
        return this;
    }

    public EntityBuilder OnTickStart(Action<TurnContext> tickStart) {
        _onTickStart = tickStart;
        return this;
    }

    public EntityBuilder OnTickEnd(Action<TurnContext> tickEnd) {
        _onTickEnd = tickEnd;
        return this;
    }

    public Entity Build() {
        var entity = new Entity(_name, _stats);

        // Add all components
        foreach (var (type, component) in _components) {
            entity.AddComponent(component);
        }

        // Add all initial effects
        foreach (var effect in _initialEffects) {
            entity.AddSpeedEffect(effect);
        }

        // Set all handlers with null-safe defaults
        entity.OnDecideAction = _onDecideAction ?? (_ => Task.FromResult(new EntityAction(ActionType.Wait, entity)));
        entity.OnCanAct = _onCanAct ?? (_ => true);

        if (_onExecute != null) entity.OnExecute += _onExecute;
        if (_onTickStart != null) entity.OnTickStart += _onTickStart;
        if (_onTickEnd != null) entity.OnTickEnd += _onTickEnd;

        return entity;
    }

    public EntityAsync BuildAsync() {
        if (_onDecideAction != null) {
            throw new Exception("Cannot build async entity with custom DecideAction (the original DecideAction will be overwritten)");
        }
        return new EntityAsync(Build());
    }
}