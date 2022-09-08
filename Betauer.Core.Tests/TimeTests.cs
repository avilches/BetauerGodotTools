using System;
using System.Threading.Tasks;
using Betauer.TestRunner;
using Betauer.Time;
using Godot;
using NUnit.Framework;

namespace Betauer.Tests {
    [TestFixture]
    public class TimeTests : Node {

        [SetUp]
        public void SetUp() {
            Engine.TimeScale = 1;
        }

        [Test]
        public async Task GodotSchedulerTests() {
            var steps = 0;
            var godotScheduler = new GodotScheduler(() => steps++);

            Assert.That(godotScheduler.IsRunning(), Is.False);
            Assert.That(godotScheduler.IsPaused(), Is.False);

            // Start every 0.1s, ignore the second start
            godotScheduler.Start(GetTree(), 0.1f);
            godotScheduler.Start(GetTree(), 0.0001f);
            Assert.That(godotScheduler.IsRunning(), Is.True);
            Assert.That(godotScheduler.IsPaused(), Is.False);
            await Task.Delay(TimeSpan.FromSeconds(0.55));
            Assert.That(steps, Is.GreaterThanOrEqualTo(4));
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
            godotScheduler.Start(GetTree(), 0.1f);
            Assert.That(godotScheduler.IsRunning(), Is.True);
            Assert.That(godotScheduler.IsPaused(), Is.False);
            await Task.Delay(TimeSpan.FromSeconds(0.55));
            Assert.That(steps, Is.GreaterThanOrEqualTo(5));
            Assert.That(steps, Is.LessThan(7));
            godotScheduler.Stop();
        }

        [Test]
        public async Task StopwatchAlarmTests() {
            var x = new GodotStopwatch(GetTree()).Start();
            await this.AwaitIdleFrame();
            await this.AwaitIdleFrame();
            await this.AwaitIdleFrame();
            Assert.That(x.Elapsed, Is.GreaterThan(0));
            Assert.That(x.IsAlarm(), Is.False);

            // SetAlarm() in the future
            x.SetAlarm(x.Elapsed + 0.2f);
            Assert.That(x.IsAlarm(), Is.False);
            await Task.Delay(300);
            Assert.That(x.IsAlarm(), Is.True);

            // SetAlarm in the past
            x.SetAlarm(x.Elapsed - 0.2f);
            Assert.That(x.IsAlarm(), Is.True);

            // Remove alarm
            x.RemoveAlarm();
            Assert.That(x.IsAlarm(), Is.False);
        }

        [Test(Description = "Ensure that a GodotStopwatch starts stopped")]
        public async Task GodotStopwatchStartsStoppedTests() {
            var x = new GodotStopwatch(GetTree());
            // It starts and stays stopped for 0.5s
            await Task.Delay(TimeSpan.FromSeconds(0.5));
            Assert.That(x.Elapsed, Is.EqualTo(0));
            Assert.That(x.IsRunning, Is.False);
            
            // Start and wait 0.5s
            x.Start();
            await Task.Delay(TimeSpan.FromSeconds(0.5));
            Assert.That(x.Elapsed, Is.EqualTo(0.5f).Within(0.1f));
            Assert.That(x.IsRunning, Is.True);
        }

        [Test]
        public async Task GodotStopwatchTests() {
            var x = new GodotStopwatch(GetTree());
            // It starts stopped
            Assert.That(x.Elapsed, Is.EqualTo(0));
            Assert.That(x.IsRunning, Is.False);
            await this.AwaitIdleFrame();
            await this.AwaitIdleFrame();
            await this.AwaitIdleFrame();
            Assert.That(x.Elapsed, Is.EqualTo(0));
            Assert.That(x.IsRunning, Is.False);

            // If reset, it's still stopped
            x.Reset();
            await this.AwaitIdleFrame();
            await this.AwaitIdleFrame();
            await this.AwaitIdleFrame();
            Assert.That(x.Elapsed, Is.EqualTo(0));
            Assert.That(x.IsRunning, Is.False);

            // Start
            x.Start();
            await this.AwaitIdleFrame();
            await this.AwaitIdleFrame();
            await this.AwaitIdleFrame();
            Assert.That(x.Elapsed, Is.GreaterThan(0));
            Assert.That(x.IsRunning, Is.True);
            var elapsed = x.Elapsed;

            // Start (if started it's ignored)
            x.Start();
            x.Start();
            x.Start();
            await this.AwaitIdleFrame();
            await this.AwaitIdleFrame();
            await this.AwaitIdleFrame();
            Assert.That(x.Elapsed, Is.GreaterThan(elapsed));
            Assert.That(x.IsRunning, Is.True);

            // Reset() when running
            x.Reset();
            Assert.That(x.Elapsed, Is.EqualTo(0));
            Assert.That(x.IsRunning, Is.True);
            await this.AwaitIdleFrame();
            await this.AwaitIdleFrame();
            await this.AwaitIdleFrame();
            Assert.That(x.Elapsed, Is.GreaterThan(0));
            Assert.That(x.IsRunning, Is.True);

            // Restart() when running (same as Reset(), because it's running)
            x.Restart();
            Assert.That(x.Elapsed, Is.EqualTo(0));
            Assert.That(x.IsRunning, Is.True);
            await this.AwaitIdleFrame();
            await this.AwaitIdleFrame();
            await this.AwaitIdleFrame();
            Assert.That(x.Elapsed, Is.GreaterThan(0));
            Assert.That(x.IsRunning, Is.True);

            // Stop (if stopped it's ignored)
            x.Stop();
            Assert.That(x.IsRunning, Is.False);
            Assert.That(x.Elapsed, Is.GreaterThan(0));
            elapsed = x.Elapsed;
            x.Stop();
            x.Stop();
            await this.AwaitIdleFrame();
            await this.AwaitIdleFrame();
            await this.AwaitIdleFrame();
            Assert.That(x.Elapsed, Is.EqualTo(elapsed));
            Assert.That(x.IsRunning, Is.False);

            // Start() when stopped resumes the timer
            x.Start();
            Assert.That(x.Elapsed, Is.EqualTo(elapsed));
            Assert.That(x.IsRunning, Is.True);
            await this.AwaitIdleFrame();
            await this.AwaitIdleFrame();
            await this.AwaitIdleFrame();
            Assert.That(x.Elapsed, Is.GreaterThan(elapsed));
            Assert.That(x.IsRunning, Is.True);

            // Restart() when stopped (same as Reset() and Start())
            x.Restart();
            Assert.That(x.Elapsed, Is.EqualTo(0));
            Assert.That(x.IsRunning, Is.True);
            await this.AwaitIdleFrame();
            await this.AwaitIdleFrame();
            await this.AwaitIdleFrame();
            Assert.That(x.Elapsed, Is.GreaterThan(0));
            Assert.That(x.IsRunning, Is.True);

            // Reset() when stopped
            x.Stop();
            await this.AwaitIdleFrame();
            await this.AwaitIdleFrame();
            await this.AwaitIdleFrame();
            Assert.That(x.Elapsed, Is.GreaterThan(0));
            Assert.That(x.IsRunning, Is.False);
            x.Reset();
            Assert.That(x.Elapsed, Is.EqualTo(0));
            Assert.That(x.IsRunning, Is.False);
            await this.AwaitIdleFrame();
            await this.AwaitIdleFrame();
            await this.AwaitIdleFrame();
            Assert.That(x.Elapsed, Is.EqualTo(0));
            Assert.That(x.IsRunning, Is.False);
        }

        [Test]
        public async Task GodotTimeoutMultipleExecutionsTests() {
            var timeouts = 0;
            var x = new GodotTimeout(GetTree(), 0.1f, () => timeouts ++).Start();
            await Task.Delay(TimeSpan.FromSeconds(0.3));
            Assert.That(x.IsRunning, Is.False);
            Assert.That(timeouts, Is.EqualTo(1));

            x.Start();
            Assert.That(x.IsRunning, Is.True);
            await Task.Delay(TimeSpan.FromSeconds(0.2));
            Assert.That(x.IsRunning, Is.False);
            Assert.That(timeouts, Is.EqualTo(2));
        }
        
        [Test]
        public async Task GodotTimeoutTests() {
            var timeout = false;
            var x = new GodotTimeout(GetTree(), 1f, () => timeout = true);
            // It starts stopped
            Assert.That(x.TimeLeft, Is.EqualTo(1f));
            Assert.That(x.IsRunning, Is.False);
            await this.AwaitIdleFrame();
            await this.AwaitIdleFrame();
            await this.AwaitIdleFrame();
            Assert.That(x.TimeLeft, Is.EqualTo(1f));
            Assert.That(x.IsRunning, Is.False);

            // If reset, it's still stopped
            x.Reset();
            await this.AwaitIdleFrame();
            await this.AwaitIdleFrame();
            await this.AwaitIdleFrame();
            Assert.That(x.TimeLeft, Is.EqualTo(1f));
            Assert.That(x.IsRunning, Is.False);

            // Start
            x.Start();
            await this.AwaitIdleFrame();
            await this.AwaitIdleFrame();
            await this.AwaitIdleFrame();
            Assert.That(x.TimeLeft, Is.LessThan(1f));
            Assert.That(x.IsRunning, Is.True);
            var timeLeft = x.TimeLeft;

            // Start (if started it's ignored)
            x.Start();
            x.Start();
            x.Start();
            await this.AwaitIdleFrame();
            await this.AwaitIdleFrame();
            await this.AwaitIdleFrame();
            Assert.That(x.TimeLeft, Is.LessThan(timeLeft));
            Assert.That(x.IsRunning, Is.True);

            // Reset() when running
            x.Reset();
            Assert.That(x.TimeLeft, Is.EqualTo(1f));
            Assert.That(x.IsRunning, Is.True);
            await this.AwaitIdleFrame();
            await this.AwaitIdleFrame();
            await this.AwaitIdleFrame();
            Assert.That(x.TimeLeft, Is.LessThan(1f));
            Assert.That(x.IsRunning, Is.True);

            // Restart() when running (same as Reset(), because it's running)
            x.Restart();
            Assert.That(x.TimeLeft, Is.EqualTo(1f));
            Assert.That(x.IsRunning, Is.True);
            await this.AwaitIdleFrame();
            await this.AwaitIdleFrame();
            await this.AwaitIdleFrame();
            Assert.That(x.TimeLeft, Is.LessThan(1f));
            Assert.That(x.IsRunning, Is.True);

            // Stop (if stopped it's ignored)
            x.Stop();
            Assert.That(x.IsRunning, Is.False);
            Assert.That(x.TimeLeft, Is.LessThan(1));
            timeLeft = x.TimeLeft;
            x.Stop();
            x.Stop();
            await this.AwaitIdleFrame();
            await this.AwaitIdleFrame();
            await this.AwaitIdleFrame();
            Assert.That(x.TimeLeft, Is.EqualTo(timeLeft));
            Assert.That(x.IsRunning, Is.False);

            // Start() when stopped resumes the timer
            x.Start();
            Assert.That(x.TimeLeft, Is.EqualTo(timeLeft));
            Assert.That(x.IsRunning, Is.True);
            await this.AwaitIdleFrame();
            await this.AwaitIdleFrame();
            await this.AwaitIdleFrame();
            Assert.That(x.TimeLeft, Is.LessThan(timeLeft));
            Assert.That(x.IsRunning, Is.True);

            // Restart() when stopped (same as Reset() and Start())
            x.Restart();
            Assert.That(x.TimeLeft, Is.EqualTo(1f));
            Assert.That(x.IsRunning, Is.True);
            await this.AwaitIdleFrame();
            await this.AwaitIdleFrame();
            await this.AwaitIdleFrame();
            Assert.That(x.TimeLeft, Is.LessThan(1f));
            Assert.That(x.IsRunning, Is.True);

            // Reset() when stopped
            x.Stop();
            await this.AwaitIdleFrame();
            await this.AwaitIdleFrame();
            await this.AwaitIdleFrame();
            Assert.That(x.TimeLeft, Is.LessThan(1f));
            Assert.That(x.IsRunning, Is.False);
            x.Reset();
            Assert.That(x.TimeLeft, Is.EqualTo(1f));
            Assert.That(x.IsRunning, Is.False);
            await this.AwaitIdleFrame();
            await this.AwaitIdleFrame();
            await this.AwaitIdleFrame();
            Assert.That(x.TimeLeft, Is.EqualTo(1f));
            Assert.That(x.IsRunning, Is.False);

            // Set timeout is a reset;
            x.SetTimeout(0.1f);
            await this.AwaitIdleFrame();
            await this.AwaitIdleFrame();
            await this.AwaitIdleFrame();
            Assert.That(x.TimeLeft, Is.EqualTo(0.1f));
            Assert.That(x.IsRunning, Is.False);

            x.Start();
            Assert.That(timeout, Is.False);
            Assert.That(x.TimeLeft, Is.EqualTo(0.1f));
            Assert.That(x.IsRunning, Is.True);
            await this.AwaitIdleFrame();
            await Task.Delay(200);
            Assert.That(x.TimeLeft, Is.EqualTo(0f));
            Assert.That(x.IsRunning, Is.False);
            Assert.That(timeout, Is.True);
        }
    }
}