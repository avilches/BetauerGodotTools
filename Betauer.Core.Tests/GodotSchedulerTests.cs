using System;
using System.Threading.Tasks;
using Betauer.TestRunner;
using Godot;
using NUnit.Framework;

namespace Betauer.Tests {
    [TestFixture]
    public class GodotSchedulerTests {

        [Test]
        public async Task Test() {
            Engine.TimeScale = 1;
            var steps = 0;
            var godotScheduler = new GodotScheduler(() => steps++, true);

            Assert.That(godotScheduler.IsRunning(), Is.False);
            Assert.That(godotScheduler.IsPaused(), Is.False);

            // Start every 0.1s, ignore the second start
            godotScheduler.Start(0.1f);
            godotScheduler.Start(0.0001f);
            Assert.That(godotScheduler.IsRunning(), Is.True);
            Assert.That(godotScheduler.IsPaused(), Is.False);
            await Task.Delay(TimeSpan.FromSeconds(0.55));
            Assert.That(steps, Is.GreaterThanOrEqualTo(5));
            Assert.That(steps, Is.LessThan(7));

            // Pause
            godotScheduler.Pause();
            Assert.That(godotScheduler.IsRunning(), Is.True);
            Assert.That(godotScheduler.IsPaused(), Is.True);
            steps = 0;
            await Task.Delay(TimeSpan.FromSeconds(0.4));
            Assert.That(steps, Is.EqualTo(0));
            
            // Resume
            godotScheduler.Resume();
            Assert.That(godotScheduler.IsRunning(), Is.True);
            Assert.That(godotScheduler.IsPaused(), Is.False);
            await Task.Delay(TimeSpan.FromSeconds(0.2));
            Assert.That(steps, Is.GreaterThanOrEqualTo(1));
            
            // Stop
            steps = 0;
            godotScheduler.Stop();
            // It's still running, it will stop in the next execution...
            Assert.That(godotScheduler.IsRunning(), Is.True);
            Assert.That(godotScheduler.IsPaused(), Is.False);
            await Task.Delay(TimeSpan.FromSeconds(0.3));
            Assert.That(godotScheduler.IsRunning(), Is.False);
            Assert.That(godotScheduler.IsPaused(), Is.False);
            // ... but the lambda is not executed
            Assert.That(steps, Is.EqualTo(0));

            // Start again every 0.1s, same behaviour
            godotScheduler.Start(0.1f);
            Assert.That(godotScheduler.IsRunning(), Is.True);
            Assert.That(godotScheduler.IsPaused(), Is.False);
            await Task.Delay(TimeSpan.FromSeconds(0.55));
            Assert.That(steps, Is.GreaterThanOrEqualTo(5));
            Assert.That(steps, Is.LessThan(7));
            godotScheduler.Stop();

        }
        
    }
}