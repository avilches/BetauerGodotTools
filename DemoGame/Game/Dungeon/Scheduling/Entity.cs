using System.Threading.Tasks;
using Betauer.DI;
using Betauer.DI.Attributes;
using Betauer.FSM.Async;
using Godot;

namespace Veronenger.Game.Dungeon.Scheduling;

using System;
using System.Collections.Generic;
using System.Linq;

/*

Energy Cost

   There are two variables that determine how quickly a character will perform an action: one is the character's speed, the other is the energy
   cost associated with that action. Whereas speed reflects the overall quickness in thought and action of the character, energy cost reflects
    the player's particular quickness with that one particular action.

   ADOM is a turn-based game, but does simulate the flow of time and different speeds; the system it uses is described in detail in the game
   manual. In short: The game works in so-called segments. During this segment the creature in question accumulates energy points equal to the
   creature's speed score. Once a creature has gained 1000 energy points or more, it is allowed to perform an action. Actions will lower the
   character's energy points by a certain value; this is the energy cost. This energy cost will drop the character's energy points below 1000;
    they will be allowed to act again once enough segments have passed to get them back, and so on for infinity.

   The base energy cost is 1000. Most actions that are not otherwise stated to use more or less than that will cost 1000 energy points.
   With a base speed of 100 this means that the player can perform such an action precisely once every 10 segments. This base energy cost,
   however, can be modified by specific actions. For example, performing an attack with melee weapon only costs 1000 energy points
   if the character is completely untrained with the weapon; with a skill of 1 it costs merely 980 energy points. Many class powers
   significantly reduce energy points for certain actions the character is particularly talented in; for instance two Archer class
   powers significantly reduce the energy cost of missile attacks.

   Skill	0	| 1	    | 2	    | 3	    | 4	    | 5	    | 6	    | 7	    | 8	    | 9	    | 10	| 11	| 12	| 13	| 14	| 15
   Cost	 1000	| 980	| 960	| 940	| 920	| 895	| 870	| 845	| 820	| 790	| 760	| 730	| 700	| 665	| 630	| 595



   Energy cost reductions can stack, although they do suffer from diminishing returns. For example, a level 6 Archer (800 energy missile attack)
   using a missile at level 1 weapon skill
   (980 energy cost) would take 100000 / (100 + (100000 / 800 - 100) + (100000 / 980 - 100)) = 787 energy for the attack.
    Note that the formula uses all integer divisions. This can actually result in rounding errors even for single energy cost reductions.
    For example, long stride (950 energy cost movement) actually results in 100000 / (100 + (100000 / 950 - 100)) = 952 energy cost movement.

   Energy cost reductions can also apply to actions with higher than 1000 energy cost.
   In this case, the energy cost reduction (with rounding error included) is applied as a percentage multiplier to the high cost action.
   For example, a Barbarian using mighty blow (2500 energy cost) with a weapon at level 2 weapon skill
   (960 energy cost, 961 with rounding error) would take 2500 * 0.961 = 2403 energy for the attack.
   In the event that a character with Stiff muscles (all actions 1500 energy cost) takes an action that would normally cost more than
   1000 energy, it's treated as if the action cost 500 energy more, but energy cost reductions are applied to the extra 500 cost separately,
   potentially resulting in more rounding errors. For example, a Barbarian with stiff muscles using mighty blow with a weapon at
   level 2 weapon skill would take 2500 * 0.961 + 500 * 0.961 = 2884 energy for the attack.


Speed
   Speed is an inherent property of all creatures in ADOM (including the PC) that determines how quickly and in what order they make take actions.

   * How Speed Works

   Every action in ADOM has an associated energy cost to it. This energy cost determines how long it takes for that action to
   be performed or, more accurately, how quickly the actor may recover after the action has been performed
   (since the action itself otherwise is instantaneous). Most common actions such as walking and attacking have an energy cost of 1000.
   Speed determines the rate at which the actors restore their energy--the more speed that the player has, the faster that they may act.
   On the simplest level, if one actor has 100 speed and another has 120, then the one with 120 speed will be able to act
   6 times for every 5 that the one with 100 speed does. Things get more complicated when energy cost is taken into account, however.
   Seven league boots, for example, can reduce the energy cost of walking from 1000 to 750. If the actor with 100 speed was then wearing
   seven league boots, they could walk 5 steps for every 3 the actor with 120 speed would take.

   While players have a means to monitor the cost of their actions, there are no easy means to monitor the energy cost of the actions
   of monsters. The manual and some testing seem to strongly suggest that the vast majority of monster actions require exactly 1000 energy;
   however, this is offset by the fact that many monsters may attack multiple times per turn, whereas the player can normally only attack once.
   There has been some spectulation that certain monsters (eg. the Cat lord) may have lower energy movement, however. In general though,
   this simplifies the analysis considerably since all that would be required to determine if, eg. the player can outrun a monster, is to
   simply compare their speeds--if the monster has a lower speed than the PC, then the PC should be able to outrun the monster, so long as
   their base movement energy is 1000 or less.

   Speed is also known to have an effect on the player's ability to dodge magical bolts and missile attacks. The exact relation has not
   yet been quantified.

   * Effect on Experience

   Speed is a very desirable attribute. Even small increases in speed can have significant effects on the dynamics of combat, and may
   allow extra turns or determine whether or not the player can safely flee from combat. The downside is that speed has a direct effect
   on the experience that the player receives--there is an experience penalty for killing something slower than the PC, and an experience
   gain for killing something that is faster.

   JellySlayer performed an experiment relating monster speed to player speed, and found that the relation between the two is:
   Penalty = (MonsterSpeed/PlayerSpeed)

   In other words, a player with the Raven starsign (ie. 110 base speed) who kills a monster with 100 speed will receive only 91% of
   the experience points for that monster. For low speeds, this effect is not that substantial--an difference of 2 speed results in only
   a 2% penalty--if the difference is large, then the penalty can be severe: at a 100 speed difference, the player only receives half
   the available experience. On the other hand, this can be exploited: being strained and bloated will reduce the PC's speed by 20,
   which this yields a 25% increase in experience against monsters with 100 speed—with, of course, increased risk in combat.

   * Controlling Speed

   Because of the effect on experience, many players often manipulate their speed in order to maximize experience gains while minimizing risk.
   The easiest way to do this is encumbrance: being burdened reduces speed by 5, strained by 10, and strained! by 20. Simply by
   carrying around stacks of disposable items, such as large rations or huge rocks, the player can maintain a small but persistent experience bonus--and then
   abandon these items if they need to make a quick escape. The Slow Monster spell is often used to similar effect: the experience bonus is only calculated when the monster is killed, thus many players, while fighting a difficult monster, will cast Slow Monster on themselves when the monster is critically wounded, and then kill the monster with 50% of their base speed. This results in a massive increase in experience for, again, relatively minimal risk.

   The benefits of high speed should not be understated, however: the ability to move much faster than your opponents is significant, and,
   particularly since the energy cost of attacking decreases with weapon skill, with relatively modest speed values, it is possible to
   attack two or more times for every one that the opponent receives. In particular, players intending to pursue Ultra Endings often want
   speeds significantly above 100 to ensure a victory.

   * Common Ways to Modify Speed

   Activity -> Speed Modifier
   Slowed (Slow Monster, or by an opponent) -> -50%
   Overburdened! -> -40
   Strained! -> -20
   Strained -> -10
   Bloated -> -10
   Cold blood corruption -> -10
   Decay corruption -> -10
   Burdened -> -5
   Satiated -> -5
   Bloody sweat corruption	 -> +5
   Talents (Quick, Very Quick, Greased Lightning) -> +2, +3, +4 for 9 total
   Athletics Skill	 -> up to +8
   Raven-born -> +10
   Dexterity -> +1 for every 2 points above 17
   Very light corruption -> +20
   Wish (temporary only) -> +100-200

   Monks and beastfighters also receive speed bonuses as they level. Some items such as a quicksilver quarterstaff, ring of speed,
   or boots of the slow shuffle and the artifacts black torc, Ring of the Master Cat and Boots of Great Speed can also impact the PC's speed.
   Corpses of monster such as quicklings and claw bugs, and potions of quickling blood, can also increase base speed.
   The Devour order corruption grants a temporary 1d100 speed boost every time the PC eats the corpse of a lawful monster.
 */

public class ActionConfig {
    private static readonly Dictionary<ActionType, ActionConfig> Actions = new();

    public ActionType Type { get; }
    public int EnergyCost { get; }

    private ActionConfig(ActionType type, int energyCost) {
        Type = type;
        EnergyCost = energyCost;
    }

    public static void RegisterAction(ActionType type, int energyCost) {
        Actions[type] = new ActionConfig(type, energyCost);
    }

    public static ActionConfig Get(ActionType type) {
        return Actions.TryGetValue(type, out var action)
            ? action
            : throw new Exception($"Action type {type} not registered!");
    }
}

public enum ActionType {
    Wait,
    Walk,
    Run,
    Attack,
}

/// <summary>
/// Representa una acción realizada por una entidad
/// </summary>
public class EntityAction {
    public ActionConfig Config { get; }
    public Entity Actor { get; }
    public Entity Target { get; }
    public int EnergyCost { get; private set; }
    public int AnimationDurationMillis { get; set; } = 1;

    public EntityAction(ActionType type, Entity actor, Entity target = null) {
        Config = ActionConfig.Get(type);
        Actor = actor;
        Target = target;
        EnergyCost = Config.EnergyCost;
    }

    // Modificadores pueden alterar el coste de energía
    public void ModifyEnergyCost(float multiplier) {
        EnergyCost = (int)(EnergyCost * multiplier);
    }
}

/// <summary>
/// Efectos de estado que pueden afectar a una entidad
/// </summary>
public class StatusEffect {
    /// <summary>
    /// Efectos de estado que pueden afectar a una entidad
    /// </summary>
    private StatusEffect(string name, float multiplier, int ticks) {
        Name = name;
        Multiplier = multiplier;
        RemainingTicks = ticks;
    }

    public string Name { get; }
    public float Multiplier { get; }
    public int RemainingTicks { get; set; }

    public static StatusEffect Ticks(string name, float multiplier, int ticks) {
        return new StatusEffect(name, multiplier, ticks);
    }

    public static StatusEffect Turns(string name, float multiplier, int turns) {
        return new StatusEffect(name, multiplier, turns * TurnSystem.TicksPerTurn);
    }
}

/// <summary>
/// Estadísticas base de una entidad
/// </summary>
public struct EntityStats {
    public int BaseSpeed { get; set; } // Velocidad base (energía ganada por tick)
}

/// <summary>
/// Clase base para todas las entidades del juego
/// </summary>
public class Entity {
    public string Name { get; }
    public EntityStats BaseStats { get; }
    public int CurrentEnergy { get; protected set; } = 0;
    public List<StatusEffect> SpeedEffects { get; } = [];
    public List<EntityAction> History { get; } = [];

    public Func<TurnContext, Task<EntityAction>> OnDecideAction { get; set; }
    public Func<TurnContext, bool> OnCanAct { get; set; } = _ => true;

    public event Action<TurnContext, EntityAction>? OnExecute;
    public event Action<TurnContext>? OnTickEnd;

    public Entity(string name, EntityStats stats) {
        Name = name;
        BaseStats = stats;
        OnDecideAction = _ => Task.FromResult(new EntityAction(ActionType.Wait, this));
    }

    public int GetCurrentSpeed() {
        var multiplier = SpeedEffects.Aggregate(1f, (current, effect) => current * effect.Multiplier);
        return Mathf.RoundToInt(BaseStats.BaseSpeed * multiplier);
    }

    public void TickEnd(TurnContext context) {
        var currentSpeed = GetCurrentSpeed();
        CurrentEnergy += currentSpeed;
        Console.WriteLine($"{Name} +{currentSpeed} = " + CurrentEnergy);
        RemoveExpiredEffects();

        OnTickEnd?.Invoke(context);
    }

    private void RemoveExpiredEffects() {
        for (var i = SpeedEffects.Count - 1; i >= 0; i--) {
            var effect = SpeedEffects[i];
            effect.RemainingTicks--;
            if (effect.RemainingTicks <= 0) {
                SpeedEffects.RemoveAt(i);
            }
        }
    }

    public bool CanAct(TurnContext context) => CurrentEnergy >= 0 && OnCanAct.Invoke(context);

    public Task<EntityAction> DecideAction(TurnContext context) {
        return OnDecideAction(context);
    }

    public void Execute(TurnContext context, EntityAction action) {
        CurrentEnergy -= action.EnergyCost;
        Console.WriteLine($"{Name} -{action.EnergyCost} = {CurrentEnergy} (action: {action.Config.Type})");
        OnExecute?.Invoke(context, action);
        History.Add(action);
    }
}

public class EntityAsync {
    private TaskCompletionSource<EntityAction>? _promise;
    private readonly Queue<EntityAction> _queue = [];

    public Entity Entity { get; }

    public EntityAsync(Entity entity) {
        Entity = entity;
        entity.OnDecideAction = DecideAction;
    }

    public bool IsWaiting => _promise != null;

    public void SetResult(EntityAction action) {
        var promise = _promise;
        if (promise == null) {
            throw new Exception("No action to resolve");
        }
        if (promise.Task.IsCompleted) {
            throw new Exception("Player action already set");
        }
        _promise = null;
        promise.TrySetResult(action);
    }

    public void ScheduleNextAction(EntityAction nextAction) {
        _queue.Enqueue(nextAction);
    }

    public Task<EntityAction> DecideAction(TurnContext context) {
        if (_queue.Count > 0) {
            var action = _queue.Dequeue();
            return Task.FromResult(action);
        }
        _promise ??= new TaskCompletionSource<EntityAction>();
        return _promise.Task;
    }
}

public class Dummy : Entity {
    public Dummy(ActionType type, string name, EntityStats stats) : base(name, stats) {
        OnDecideAction = _ => Task.FromResult(new EntityAction(type, this));
    }
}