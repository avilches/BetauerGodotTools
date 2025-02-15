using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Godot;

namespace Veronenger.Game.Dungeon.World;

/// <summary>
/// Clase base para todas las entidades del juego
/// </summary>
public class Entity {
    public string Name { get; }
    public EntityStats BaseStats { get; }
    public int CurrentEnergy { get; protected set; } = 0;
    public List<MultiplierEffect>? SpeedEffects { get; private set; } = null;

    public Func<Task<ActionCommand>> OnDecideAction { get; set; }
    public Func<bool> OnCanAct { get; set; } = () => true;

    public event Action<ActionCommand>? OnExecute;
    public event Action? OnTickStart;
    public event Action? OnTickEnd;
    public event Action? OnWorldRemoved;
    public event Action? OnWorldAdded;
    public event Action<Vector2I, Vector2I>? OnMoved;

    public TurnWorld World => Location?.World;
    public WorldCell Cell => Location?.Cell;
    public Location Location { get; internal set; }

    public Entity(EntityStats stats) : this(Guid.NewGuid().ToString(), stats) {
    }

    public Entity(string name, EntityStats stats) {
        Name = name;
        BaseStats = stats;
        OnDecideAction = () => Task.FromResult(new ActionCommand(ActionType.Wait, this));
    }

    public Entity Configure(Action<Entity> config) {
        config(this);
        return this;
    }

    public MultiplierEffect AddSpeedEffect(string name, float multiplier, int turns) {
        var effect = MultiplierEffect.Ticks(name, multiplier, turns * World.TicksPerTurn);
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

    public Task<ActionCommand> DecideAction() {
        return OnDecideAction.Invoke();
    }

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