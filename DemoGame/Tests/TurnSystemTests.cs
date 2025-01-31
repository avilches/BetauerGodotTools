using System;
using Betauer.TestRunner;
using Veronenger.Game.Dungeon.Scheduling;

namespace Veronenger.Tests;

using NUnit.Framework;
using System.Threading.Tasks;
using System.Linq;

[TestFixture]
public class TurnSystemTests {
    private TurnSystem _turnSystem;
    private EntityAsync _player;
    private Dummy _fastWalker;
    private Dummy _slowAttacker;

    [SetUp]
    public void Setup() {
        _turnSystem = new TurnSystem();

        // Create player with default speed (100)
        _player = new EntityAsync("Player", new EntityStats {
            BaseSpeed = 100,
        });

        // Create monsters with different speeds
        _fastWalker = new Dummy(ActionType.Walk, "FastMonster", new EntityStats {
            BaseSpeed = 120,
        });

        _slowAttacker = new Dummy(ActionType.Attack, "SlowMonster", new EntityStats {
            BaseSpeed = 80,
        });

        // Add entities to the system
        _turnSystem.AddEntity(_player);
        _turnSystem.AddEntity(_fastWalker);
        _turnSystem.AddEntity(_slowAttacker);

        ActionConfig.RegisterAction(ActionType.Walk, 1000);
        ActionConfig.RegisterAction(ActionType.Attack, 1200);
        ActionConfig.RegisterAction(ActionType.Run, 2000);
    }

    [Test]
    public async Task ProcessTicks_CorrectEnergy() {
        _player.ScheduleNextAction(new EntityAction(ActionType.Walk, _player) { AnimationDurationMillis = 1 });
        await _turnSystem.ProcessTickAsync();
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
            _player.ScheduleNextAction(new EntityAction(ActionType.Run, _player) { AnimationDurationMillis = 1 });
            await _turnSystem.ProcessTickAsync();
        }

        // Verify turn and tick counts
        Assert.That(_turnSystem.CurrentTick, Is.EqualTo(ticks));
        Assert.That(_turnSystem.CurrentTurn, Is.EqualTo(ticks / TurnSystem.TicksPerTurn));
    }

    [Test]
    public async Task ProcessTicks_SpeedManagedOk() {
        var ticks = 100;
        // Process enough ticks to let entities act
        // We'll process 100 ticks which should be 10 full turns
        for (var i = 0; i < ticks; i++) {
            _player.ScheduleNextAction(new EntityAction(ActionType.Walk, _player) { AnimationDurationMillis = 1 });
            await _turnSystem.ProcessTickAsync();
        }

        // Player always Walks (1000) and speed is 100
        // FastWalker walks too, speed is 120

        Assert.That(_player.History.Count, Is.EqualTo(10));
        Assert.That(_fastWalker.History.Count, Is.EqualTo(12));
        Assert.That(_slowAttacker.History.Count, Is.EqualTo(7));
    }
}