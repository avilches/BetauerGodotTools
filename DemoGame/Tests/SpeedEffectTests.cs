using System.Threading.Tasks;
using Betauer.TestRunner;
using NUnit.Framework;
using Veronenger.Game.Dungeon.Scheduling;

namespace Veronenger.Tests;

[TestFixture]
public class SpeedEffectTests {
    private TurnSystem _turnSystem;
    private Dummy _walker;

    [SetUp]
    public void Setup() {
        _turnSystem = new TurnSystem();
        _walker = new Dummy(ActionType.Walk, "Walker", new EntityStats {
            BaseSpeed = 100,
        });
        _turnSystem.AddEntity(_walker);
        ActionConfig.RegisterAction(ActionType.Walk, 1000);
    }

    [Test]
    public async Task SingleSpeedEffect_LastsTwoTurns() {
        // Add effect that lasts 2 turns (20 ticks)
        var speedEffect = MultiplierEffect.Turns("Speed Boost", 1.5f, 2);
        _walker.AddSpeedEffect(speedEffect);

        // Initial speed should be affected by the multiplier
        Assert.That(_walker.GetCurrentSpeed(), Is.EqualTo(150), "Initial speed should be multiplied by 1.5");

        // Process 19 ticks (effect should still be active)
        for (int i = 0; i < 19; i++) {
            await _turnSystem.ProcessTickAsync();
            Assert.That(_walker.GetCurrentSpeed(), Is.EqualTo(150), $"Speed should still be affected at tick {i}");
            Assert.That(speedEffect.RemainingTicks, Is.GreaterThan(0), "Effect should still have remaining ticks");
        }

        // Process last tick (effect should expire)
        await _turnSystem.ProcessTickAsync();

        Assert.That(_walker.SpeedEffects, Is.Empty, "Effect should be removed after expiration");
        Assert.That(_walker.GetCurrentSpeed(), Is.EqualTo(100), "Speed should return to normal after effect expires");
    }

    [Test]
    public async Task MultipleSpeedEffects_DifferentDurations() {
        // Add two effects:
        // - effect1: doubles speed for 2 turns (20 ticks)
        // - effect2: adds 50% speed for 5 ticks
        var effect1 = MultiplierEffect.Turns("Double Speed", 2.0f, 2);
        var effect2 = MultiplierEffect.Ticks("Speed Boost", 1.5f, 5);

        _walker.AddSpeedEffect(effect1);
        _walker.AddSpeedEffect(effect2);

        // Initial speed should be affected by both multipliers (2.0 * 1.5 = 3.0)
        Assert.That(_walker.GetCurrentSpeed(), Is.EqualTo(300), "Initial speed should be multiplied by both effects");

        // Process first 4 ticks (both effects should be active)
        for (int i = 0; i < 4; i++) {
            await _turnSystem.ProcessTickAsync();
            Assert.That(_walker.GetCurrentSpeed(), Is.EqualTo(300),
                $"Speed should still be affected by both effects at tick {i}");
            Assert.That(_walker.SpeedEffects.Count, Is.EqualTo(2),
                "Both effects should still be active");
        }

        // Process 5th tick (effect2 should expire)
        await _turnSystem.ProcessTickAsync();
        Assert.That(_walker.SpeedEffects.Count, Is.EqualTo(1),
            "Only effect1 should remain active after 5 ticks");
        Assert.That(_walker.GetCurrentSpeed(), Is.EqualTo(200),
            "Speed should now only be affected by effect1 (double speed)");

        // Process remaining 15 ticks (effect1 should still be active)
        for (int i = 5; i < 19; i++) {
            await _turnSystem.ProcessTickAsync();
            Assert.That(_walker.GetCurrentSpeed(), Is.EqualTo(200),
                $"Speed should still be doubled by effect1 at tick {i}");
            Assert.That(_walker.SpeedEffects.Count, Is.EqualTo(1),
                "effect1 should still be active");
        }

        // Process last tick (effect1 should expire)
        await _turnSystem.ProcessTickAsync();

        Assert.That(_walker.SpeedEffects, Is.Empty,
            "All effects should be removed after expiration");
        Assert.That(_walker.GetCurrentSpeed(), Is.EqualTo(100),
            "Speed should return to normal after all effects expire");
    }
}