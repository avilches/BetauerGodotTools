using System;
using System.Collections.Generic;
using Betauer.TestRunner;
using Godot;
using Veronenger.Game.Dungeon.World;

namespace Veronenger.Tests;

using NUnit.Framework;
using System.Threading.Tasks;
using System.Linq;

[TestFixture]
public class TurnSystemTests : TurnBaseTests {
    private SchedulingEntity _player;
    private EntityBase _fastWalker;
    private EntityBase _slowAttacker;
    private WorldMap _worldMap;
    private TurnSystem TurnSystem => _worldMap.TurnSystem;

    [SetUp]
    public void Setup() {
        _worldMap = new WorldMap(1, 1);
        _worldMap.Cells.Load((p) => new WorldCell(CellType.Floor, p));

        // Create player with default speed (100)
        _player = new SchedulingEntity("Player", new EntityStats { BaseSpeed = 100 });

        // Create monsters with different speeds
        _fastWalker = EntityBuilder.Create("FastMonster", new EntityStats { BaseSpeed = 120 })
            .DecideAction(ActionType.Walk)
            .Build();

        _slowAttacker = EntityBuilder.Create("SlowMonster", new EntityStats { BaseSpeed = 80 })
            .DecideAction(ActionType.Attack)
            .Build();

        // Add entities to the system
        _worldMap.AddEntity(_player, Vector2I.Zero);
        _worldMap.AddEntity(_fastWalker, Vector2I.Zero);
        _worldMap.AddEntity(_slowAttacker, Vector2I.Zero);
    }

    [Test]
    public async Task ProcessTicks_CorrectEnergy() {
        _player.ScheduleNextAction(new ActionCommand(ActionType.Walk, _player));
        await TurnSystem.ProcessTickAsync();
        // 1 tick -> - (1*ActionCost) +(1*BaseEnergy)
        Assert.That(_player.CurrentEnergy, Is.EqualTo(-1000 + 100));
        Assert.That(_fastWalker.CurrentEnergy, Is.EqualTo(-1000 + 120));
        Assert.That(_slowAttacker.CurrentEnergy, Is.EqualTo(-1200 + 80));
    }

    [Test]
    public async Task ProcessTicks_CorrectCurrentTickAndCurrentTurn() {
        var ticks = 20;
        // Process enough ticks to let entities act
        // We'll process 20 ticks which should be 2 full turns
        for (var i = 0; i < ticks; i++) {
            _player.ScheduleNextAction(new ActionCommand(ActionType.Run, _player));
            await TurnSystem.ProcessTickAsync();
        }

        // Verify turn and tick counts
        Assert.That(_worldMap.CurrentTick, Is.EqualTo(ticks));
        Assert.That(_worldMap.CurrentTurn, Is.EqualTo(ticks / _worldMap.TicksPerTurn));
    }

    [Test]
    public async Task ProcessTicks_SpeedManagedOk() {
        var ticks = 100;
        // Process enough ticks to let entities act
        // We'll process 100 ticks which should be 10 full turns
        var playerHistory = new List<ActionCommand>();
        var fastWalkerHistory = new List<ActionCommand>();
        var slowAttackerHistory = new List<ActionCommand>();

        _player.OnExecute += (action) => playerHistory.Add(action);
        _fastWalker.OnExecute += (action) => fastWalkerHistory.Add(action);
        _slowAttacker.OnExecute += (action) => slowAttackerHistory.Add(action);
        for (var i = 0; i < ticks; i++) {
            _player.ScheduleNextAction(new ActionCommand(ActionType.Walk, _player));
            await TurnSystem.ProcessTickAsync();
        }

        // Player always Walks (1000) and speed is 100
        // FastWalker walks too, speed is 120

        Assert.That(playerHistory.Count, Is.EqualTo(10));
        Assert.That(fastWalkerHistory.Count, Is.EqualTo(12));
        Assert.That(slowAttackerHistory.Count, Is.EqualTo(7));
    }
}

[TestFixture]
public class EntityEventsTests : TurnBaseTests {
    private WorldMap _worldMap;
    private TurnSystem TurnSystem => _worldMap.TurnSystem;
    private EntityBase _entity;
    private int _tickStartCount;
    private int _tickEndCount;
    private int _executeCount;
    private int _decideActionCount;
    private int _canActCount;

    [SetUp]
    public void Setup() {
        _worldMap = new WorldMap(1, 1);
        _worldMap.Cells.Load((p) => new WorldCell(CellType.Floor, p));
        ResetCounters();
    }

    private void ResetCounters() {
        _tickStartCount = 0;
        _tickEndCount = 0;
        _executeCount = 0;
        _decideActionCount = 0;
        _canActCount = 0;
    }

    [Test]
    public async Task AllEventsShouldBeCalledInOrder() {
        var actualOrder = new List<string>();

        _entity = EntityBuilder.Create("TestEntity", new EntityStats { BaseSpeed = 100 })
            .OnTickStart(() => {
                actualOrder.Add("TickStart");
                _tickStartCount++;
            })
            .OnTickEnd(() => {
                actualOrder.Add("TickEnd");
                _tickEndCount++;
            })
            .Execute((action) => {
                actualOrder.Add("Execute");
                _executeCount++;
            })
            .DecideAction(() => {
                actualOrder.Add("DecideAction");
                _decideActionCount++;
                return new ActionCommand(ActionType.Walk, _entity);
            })
            .CanAct(() => {
                actualOrder.Add("CanAct");
                _canActCount++;
                return true;
            })
            .Build();

        _worldMap.AddEntity(_entity, Vector2I.Zero);

        // Process one tick
        await TurnSystem.ProcessTickAsync();

        // Verify the order of events
        var expectedOrder = new[] { "TickStart", "CanAct", "DecideAction", "Execute", "TickEnd" };
        Assert.That(actualOrder, Is.EqualTo(expectedOrder));

        // Verify each event was called exactly once
        Assert.Multiple(() => {
            Assert.That(_tickStartCount, Is.EqualTo(1));
            Assert.That(_tickEndCount, Is.EqualTo(1));
            Assert.That(_executeCount, Is.EqualTo(1));
            Assert.That(_decideActionCount, Is.EqualTo(1));
            Assert.That(_canActCount, Is.EqualTo(1));
        });
    }

    [Test]
    public async Task CanActFalseSkipsDecideActionAndExecute() {
        var actualOrder = new List<string>();

        _entity = EntityBuilder.Create("TestEntity", new EntityStats { BaseSpeed = 100 })
            .OnTickStart(() => {
                actualOrder.Add("TickStart");
                _tickStartCount++;
            })
            .OnTickEnd(() => {
                actualOrder.Add("TickEnd");
                _tickEndCount++;
            })
            .Execute((action) => {
                actualOrder.Add("Execute");
                _executeCount++;
            })
            .DecideAction(() => {
                actualOrder.Add("DecideAction");
                _decideActionCount++;
                return new ActionCommand(ActionType.Walk, _entity);
            })
            .CanAct(() => {
                actualOrder.Add("CanAct");
                _canActCount++;
                return false; // Entity cannot act
            })
            .Build();

        _worldMap.AddEntity(_entity, Vector2I.Zero);

        await TurnSystem.ProcessTickAsync();

        // Verify only TickStart, CanAct and TickEnd were called
        var expectedOrder = new[] { "TickStart", "CanAct", "TickEnd" };
        Assert.That(actualOrder, Is.EqualTo(expectedOrder));

        Assert.Multiple(() => {
            Assert.That(_tickStartCount, Is.EqualTo(1));
            Assert.That(_tickEndCount, Is.EqualTo(1));
            Assert.That(_executeCount, Is.EqualTo(0), "Execute should not be called");
            Assert.That(_decideActionCount, Is.EqualTo(0), "DecideAction should not be called");
            Assert.That(_canActCount, Is.EqualTo(1));
        });
    }

    [Test]
    public async Task EnergyBelowZeroSkipsAction() {
        var actualOrder = new List<string>();

        _entity = EntityBuilder.Create("TestEntity", new EntityStats { BaseSpeed = 100 })
            .OnTickStart(() => {
                actualOrder.Add("TickStart");
                _tickStartCount++;
            })
            .OnTickEnd(() => {
                actualOrder.Add("TickEnd");
                _tickEndCount++;
            })
            .Execute((action) => {
                actualOrder.Add("Execute");
                _executeCount++;
            })
            .DecideAction(() => {
                actualOrder.Add("DecideAction");
                _decideActionCount++;
                return new ActionCommand(ActionType.Walk, _entity);
            })
            .CanAct(() => {
                actualOrder.Add("CanAct");
                _canActCount++;
                return true;
            })
            .Build();

        _worldMap.AddEntity(_entity, Vector2I.Zero);

        // First tick: entity acts normally
        await TurnSystem.ProcessTickAsync();

        // Entity now has negative energy due to the Walk action (-1000 + 100 = -900)
        Assert.That(_entity.CurrentEnergy, Is.EqualTo(-900));

        ResetCounters();
        actualOrder.Clear();

        // Second tick: entity should skip action due to negative energy
        await TurnSystem.ProcessTickAsync();

        // Verify only TickStart and TickEnd were called
        var expectedOrder = new[] { "TickStart", "TickEnd" };
        Assert.That(actualOrder, Is.EqualTo(expectedOrder));

        Assert.Multiple(() => {
            Assert.That(_tickStartCount, Is.EqualTo(1));
            Assert.That(_tickEndCount, Is.EqualTo(1));
            Assert.That(_executeCount, Is.EqualTo(0), "Execute should not be called when energy is below 0");
            Assert.That(_decideActionCount, Is.EqualTo(0), "DecideAction should not be called when energy is below 0");
            Assert.That(_canActCount, Is.EqualTo(0), "CanAct should not be called when energy is below 0");
        });

        // Entity gains 100 energy in TickEnd
        Assert.That(_entity.CurrentEnergy, Is.EqualTo(-800));
    }

    [Test]
    public async Task SpeedEffectsShouldModifyEnergyGain() {
        _entity = EntityBuilder.Create("TestEntity", new EntityStats { BaseSpeed = 100 })
            .SpeedEffect(MultiplierEffect.Ticks("SpeedBoost", 2.0f, 2)) // Double speed for 2 ticks
            .DecideAction(ActionType.Walk) // Always walk
            .Build();

        _worldMap.AddEntity(_entity, Vector2I.Zero);

        // First tick
        await TurnSystem.ProcessTickAsync();
        Assert.That(_entity.CurrentEnergy, Is.EqualTo(-1000 + 200), "Should gain 200 energy (100 * 2.0)");

        // Second tick
        await TurnSystem.ProcessTickAsync();
        Assert.That(_entity.CurrentEnergy, Is.EqualTo(-1000 + 200 + 200), "Should gain another 200 energy");

        // Third tick - effect should have expired
        await TurnSystem.ProcessTickAsync();
        Assert.That(_entity.CurrentEnergy, Is.EqualTo(-1000 + 200 + 200 + 100), "Should gain normal 100 energy");
    }

    [Test]
    public async Task AsyncEntityShouldRespectScheduledActions() {
        var asyncEntity = new SchedulingEntity("Player", new EntityStats { BaseSpeed = 100 });

        var history = new List<ActionCommand>();
        asyncEntity.OnExecute += (action) => {
            // Execute action immediately
            history.Add(action);
        };

        _worldMap.AddEntity(asyncEntity, Vector2I.Zero);

        // Schedule three actions
        asyncEntity.ScheduleNextAction(new ActionCommand(ActionType.Walk, asyncEntity));
        asyncEntity.ScheduleNextAction(new ActionCommand(ActionType.Attack, asyncEntity));
        asyncEntity.ScheduleNextAction(new ActionCommand(ActionType.Run, asyncEntity));

        // Process three ticks
        while (asyncEntity.Queue.Count > 0) {
            await TurnSystem.ProcessTickAsync();
        }

        // Verify actions were executed in order
        Assert.That(history.Count, Is.EqualTo(3));
        Assert.Multiple(() => {
            Assert.That(history[0].Type, Is.EqualTo(ActionType.Walk));
            Assert.That(history[1].Type, Is.EqualTo(ActionType.Attack));
            Assert.That(history[2].Type, Is.EqualTo(ActionType.Run));
        });

        // Calculate expected energy
        // Actions cost: Walk (-1000) + Attack (-1200) + Run (-2000) = -4200
        // Energy gained: +100 * 50 = 5000
        var expectedEnergy = -4200 + (100 * _worldMap.CurrentTick);
        Assert.That(asyncEntity.CurrentEnergy, Is.EqualTo(expectedEnergy),
            "Final energy should reflect all actions and energy gains");
    }

    [Test]
    public async Task EntitiesCanBeRemovedAndAddedDuringTick() {
        // Create an entity that will remove another entity and queue a new one during its action
        var removedEntityCalled = false;
        var addedEntityCalled = false;

        // Entity to be removed
        var entityToRemove = EntityBuilder.Create("EntityToRemove", new EntityStats { BaseSpeed = 100 })
            .DecideAction(ActionType.Wait)
            .Build();

        entityToRemove.OnRemoved += () => removedEntityCalled = true;


        // Entity that will trigger the changes
        var triggerEntity = EntityBuilder.Create("TriggerEntity", new EntityStats { BaseSpeed = 100 })
            .Execute((action) => {
                entityToRemove.Removed = true; // Mark entity for removal

                // Queue a new entity
                var newEntity = EntityBuilder.Create("NewEntity", new EntityStats { BaseSpeed = 100 })
                    .DecideAction(ActionType.Wait)
                    .Build();
                newEntity.OnAdded += () => addedEntityCalled = true;

                _worldMap.QueueAddEntity(newEntity, Vector2I.Zero);
            })
            .DecideAction(ActionType.Walk)
            .Build();

        // Add initial entities
        _worldMap.AddEntity(triggerEntity, Vector2I.Zero);
        _worldMap.AddEntity(entityToRemove, Vector2I.Zero);

        Assert.That(_worldMap.Entities.Count, Is.EqualTo(2), "Should start with 2 entities");

        // Process one tick
        await TurnSystem.ProcessTickAsync();

        Assert.Multiple(() => {
            Assert.That(_worldMap.Entities.Count, Is.EqualTo(2), "Should end with 2 entities (1 removed, 1 added)");
            Assert.That(removedEntityCalled, Is.True, "OnRemoved should have been called");
            Assert.That(addedEntityCalled, Is.True, "OnAdded should have been called");
            Assert.That(_worldMap.Entities.Any(e => e.Name == "EntityToRemove"), Is.False, "Removed entity should not be present");
            Assert.That(_worldMap.Entities.Any(e => e.Name == "NewEntity"), Is.True, "New entity should be present");
        });
    }
}