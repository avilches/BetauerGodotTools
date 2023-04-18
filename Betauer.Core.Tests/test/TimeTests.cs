using System.Threading.Tasks;
using Betauer.Core.Time;
using Betauer.Core.Signal;
using Betauer.TestRunner;
using Godot;
using NUnit.Framework;

namespace Betauer.Core.Tests;

[TestRunner.Test]
public partial class TimeTests : Node {
    [SetUpClass]
    public void SetUp() {
        Engine.TimeScale = 1;
    }

    [TestRunner.Test]
    public async Task GodotSchedulerTests() {
        var steps = 0;
        var godotScheduler = new GodotScheduler(GetTree(), 0.4f, 0.1f, () => steps++);
        Assert.That(godotScheduler.IsRunning, Is.False);

        godotScheduler.Start();
        Assert.That(godotScheduler.IsRunning, Is.True);
        await Delay(0.39f);
        Assert.That(steps, Is.EqualTo(0));
        await Delay(0.6f);
        Assert.That(steps, Is.GreaterThanOrEqualTo(5));
        Assert.That(steps, Is.LessThan(8));

        // Pause
        godotScheduler.Stop();
        Assert.That(godotScheduler.IsRunning, Is.False);
        steps = 0;
        await Delay(0.4f);
        Assert.That(steps, Is.EqualTo(0));

        // Resume
        godotScheduler.Start();
        Assert.That(godotScheduler.IsRunning, Is.True);
        await Delay(0.2f);
        Assert.That(steps, Is.GreaterThanOrEqualTo(1));

    }

    [TestRunner.Test]
    public async Task StopwatchAlarmTests() {
        var x = new GodotStopwatch(GetTree()).Start();
        await this.AwaitProcessFrame();
        await this.AwaitProcessFrame();
        await this.AwaitProcessFrame();
        Assert.That(x.Elapsed, Is.GreaterThan(0));
        Assert.That(x.IsAlarm, Is.False);

        // SetAlarm() in the future
        x.SetAlarm(x.Elapsed + 0.2f);
        Assert.That(x.IsAlarm, Is.False);
        await Task.Delay(300);
        Assert.That(x.IsAlarm, Is.True);

        // SetAlarm in the past
        x.SetAlarm(x.Elapsed - 0.2f);
        Assert.That(x.IsAlarm, Is.True);

        // Remove alarm
        x.RemoveAlarm();
        Assert.That(x.IsAlarm, Is.False);
    }

    [TestRunner.Test(Description = "Ensure that a GodotStopwatch starts stopped")]
    public async Task GodotStopwatchStartsStoppedTests() {
        var x = new GodotStopwatch(GetTree());
        // It starts and stays stopped for 0.5s
        await Delay((0.5f));
        Assert.That(x.Elapsed, Is.EqualTo(0));
        Assert.That(x.IsRunning, Is.False);

        // Start and wait 0.5s
        x.Start();
        await Delay((0.5f));
        Assert.That(x.Elapsed, Is.EqualTo(0.5f).Within(0.1f));
        Assert.That(x.IsRunning, Is.True);
    }

    [TestRunner.Test]
    public async Task GodotStopwatchTests() {
        var x = new GodotStopwatch(GetTree());
        // It starts stopped
        Assert.That(x.Elapsed, Is.EqualTo(0));
        Assert.That(x.IsRunning, Is.False);
        await this.AwaitProcessFrame();
        await this.AwaitProcessFrame();
        await this.AwaitProcessFrame();
        Assert.That(x.Elapsed, Is.EqualTo(0));
        Assert.That(x.IsRunning, Is.False);

        // If reset, it's still stopped
        x.Reset();
        await this.AwaitProcessFrame();
        await this.AwaitProcessFrame();
        await this.AwaitProcessFrame();
        Assert.That(x.Elapsed, Is.EqualTo(0));
        Assert.That(x.IsRunning, Is.False);

        // Start
        x.Start();
        await this.AwaitProcessFrame();
        await this.AwaitProcessFrame();
        await this.AwaitProcessFrame();
        Assert.That(x.Elapsed, Is.GreaterThan(0));
        Assert.That(x.IsRunning, Is.True);
        var elapsed = x.Elapsed;

        // Start (if started it's ignored)
        x.Start();
        x.Start();
        x.Start();
        await this.AwaitProcessFrame();
        await this.AwaitProcessFrame();
        await this.AwaitProcessFrame();
        Assert.That(x.Elapsed, Is.GreaterThan(elapsed));
        Assert.That(x.IsRunning, Is.True);

        // Reset() when running
        x.Reset();
        Assert.That(x.Elapsed, Is.EqualTo(0));
        Assert.That(x.IsRunning, Is.True);
        await this.AwaitProcessFrame();
        await this.AwaitProcessFrame();
        await this.AwaitProcessFrame();
        Assert.That(x.Elapsed, Is.GreaterThan(0));
        Assert.That(x.IsRunning, Is.True);

        // Restart() when running (same as Reset(), because it's running)
        x.Restart();
        Assert.That(x.Elapsed, Is.EqualTo(0));
        Assert.That(x.IsRunning, Is.True);
        await this.AwaitProcessFrame();
        await this.AwaitProcessFrame();
        await this.AwaitProcessFrame();
        Assert.That(x.Elapsed, Is.GreaterThan(0));
        Assert.That(x.IsRunning, Is.True);

        // Stop (if stopped it's ignored)
        x.Stop();
        Assert.That(x.IsRunning, Is.False);
        Assert.That(x.Elapsed, Is.GreaterThan(0));
        elapsed = x.Elapsed;
        x.Stop();
        x.Stop();
        await this.AwaitProcessFrame();
        await this.AwaitProcessFrame();
        await this.AwaitProcessFrame();
        Assert.That(x.Elapsed, Is.EqualTo(elapsed));
        Assert.That(x.IsRunning, Is.False);

        // Start() when stopped resumes the timer
        x.Start();
        Assert.That(x.Elapsed, Is.EqualTo(elapsed));
        Assert.That(x.IsRunning, Is.True);
        await this.AwaitProcessFrame();
        await this.AwaitProcessFrame();
        await this.AwaitProcessFrame();
        Assert.That(x.Elapsed, Is.GreaterThan(elapsed));
        Assert.That(x.IsRunning, Is.True);

        // Restart() when stopped (same as Reset() and Start())
        x.Restart();
        Assert.That(x.Elapsed, Is.EqualTo(0));
        Assert.That(x.IsRunning, Is.True);
        await this.AwaitProcessFrame();
        await this.AwaitProcessFrame();
        await this.AwaitProcessFrame();
        Assert.That(x.Elapsed, Is.GreaterThan(0));
        Assert.That(x.IsRunning, Is.True);

        // Reset() when stopped
        x.Stop();
        await this.AwaitProcessFrame();
        await this.AwaitProcessFrame();
        await this.AwaitProcessFrame();
        Assert.That(x.Elapsed, Is.GreaterThan(0));
        Assert.That(x.IsRunning, Is.False);
        x.Reset();
        Assert.That(x.Elapsed, Is.EqualTo(0));
        Assert.That(x.IsRunning, Is.False);
        await this.AwaitProcessFrame();
        await this.AwaitProcessFrame();
        await this.AwaitProcessFrame();
        Assert.That(x.Elapsed, Is.EqualTo(0));
        Assert.That(x.IsRunning, Is.False);
    }

    [TestRunner.Test]
    public async Task GodotTimeoutMultipleExecutionsTests() {
        var timeouts = 0;
        var x = new GodotTimeout(GetTree(), 0.1f, () => timeouts++).Start();
        await Delay((0.3f));
        Assert.That(x.IsRunning, Is.False);
        Assert.That(timeouts, Is.EqualTo(1));

        x.Start();
        Assert.That(x.IsRunning, Is.True);
        await Delay(0.2f);
        Assert.That(x.Elapsed, Is.EqualTo(0.1f).Within(0.05f));
        Assert.That(x.IsRunning, Is.False);
        Assert.That(timeouts, Is.EqualTo(2));
    }

    [TestRunner.Test]
    public async Task GodotTimeoutTests() {
        var timeout = false;
        var x = new GodotTimeout(GetTree(), 1f, () => timeout = true);
        // It starts stopped
        Assert.That(x.TimeLeft, Is.EqualTo(1f));
        Assert.That(x.IsRunning, Is.False);
        await this.AwaitProcessFrame();
        await this.AwaitProcessFrame();
        await this.AwaitProcessFrame();
        Assert.That(x.TimeLeft, Is.EqualTo(1f));
        Assert.That(x.IsRunning, Is.False);

        // If reset, it's still stopped
        x.Reset();
        await this.AwaitProcessFrame();
        await this.AwaitProcessFrame();
        await this.AwaitProcessFrame();
        Assert.That(x.TimeLeft, Is.EqualTo(1f));
        Assert.That(x.IsRunning, Is.False);

        // Start
        x.Start();
        await this.AwaitProcessFrame();
        await this.AwaitProcessFrame();
        await this.AwaitProcessFrame();
        Assert.That(x.TimeLeft, Is.LessThan(1f));
        Assert.That(x.IsRunning, Is.True);
        var timeLeft = x.TimeLeft;

        // Start (if started it's ignored)
        x.Start();
        x.Start();
        x.Start();
        await this.AwaitProcessFrame();
        await this.AwaitProcessFrame();
        await this.AwaitProcessFrame();
        Assert.That(x.TimeLeft, Is.LessThan(timeLeft));
        Assert.That(x.IsRunning, Is.True);

        // Reset() when running
        x.Reset();
        Assert.That(x.TimeLeft, Is.EqualTo(1f));
        Assert.That(x.IsRunning, Is.True);
        await this.AwaitProcessFrame();
        await this.AwaitProcessFrame();
        await this.AwaitProcessFrame();
        Assert.That(x.TimeLeft, Is.LessThan(1f));
        Assert.That(x.IsRunning, Is.True);

        // Restart() when running (same as Reset(), because it's running)
        x.Restart();
        Assert.That(x.TimeLeft, Is.EqualTo(1f));
        Assert.That(x.IsRunning, Is.True);
        await this.AwaitProcessFrame();
        await this.AwaitProcessFrame();
        await this.AwaitProcessFrame();
        Assert.That(x.TimeLeft, Is.LessThan(1f));
        Assert.That(x.IsRunning, Is.True);

        // Stop (if stopped it's ignored)
        x.Stop();
        Assert.That(x.IsRunning, Is.False);
        Assert.That(x.TimeLeft, Is.LessThan(1));
        timeLeft = x.TimeLeft;
        x.Stop();
        x.Stop();
        await this.AwaitProcessFrame();
        await this.AwaitProcessFrame();
        await this.AwaitProcessFrame();
        Assert.That(x.TimeLeft, Is.EqualTo(timeLeft));
        Assert.That(x.IsRunning, Is.False);

        // Start() when stopped resumes the timer
        x.Start();
        Assert.That(x.TimeLeft, Is.EqualTo(timeLeft));
        Assert.That(x.IsRunning, Is.True);
        await this.AwaitProcessFrame();
        await this.AwaitProcessFrame();
        await this.AwaitProcessFrame();
        Assert.That(x.TimeLeft, Is.LessThan(timeLeft));
        Assert.That(x.IsRunning, Is.True);

        // Restart() when stopped (same as Reset() and Start())
        x.Restart();
        Assert.That(x.TimeLeft, Is.EqualTo(1f));
        Assert.That(x.IsRunning, Is.True);
        await this.AwaitProcessFrame();
        await this.AwaitProcessFrame();
        await this.AwaitProcessFrame();
        Assert.That(x.TimeLeft, Is.LessThan(1f));
        Assert.That(x.IsRunning, Is.True);

        // Reset() when stopped
        x.Stop();
        await this.AwaitProcessFrame();
        await this.AwaitProcessFrame();
        await this.AwaitProcessFrame();
        Assert.That(x.TimeLeft, Is.LessThan(1f));
        Assert.That(x.IsRunning, Is.False);

        x.Reset();
        x.Start();
        Assert.That(x.TimeLeft, Is.EqualTo(1f));
        Assert.That(x.IsRunning, Is.True);
        // A smaller timeout just trigger it and stops
        timeout = false;
        await Delay(0.3f);
        Assert.That(x.Elapsed, Is.EqualTo(0.3f).Within(0.05f));
        x.SetTimeout(0.1f);
        Assert.That(timeout, Is.True);
        Assert.That(x.IsRunning, Is.False);

        // A bigger timeout is ok
        timeout = false;
        x.Restart();
        Assert.That(timeout, Is.False);
        Assert.That(x.IsRunning, Is.True);
        Assert.That(x.TimeLeft, Is.EqualTo(0.1f));
        x.SetTimeout(0.2f);
        Assert.That(timeout, Is.False);
        Assert.That(x.IsRunning, Is.True);
        Assert.That(x.TimeLeft, Is.EqualTo(0.2f));
        await Delay(0.3f);
        Assert.That(x.Elapsed, Is.EqualTo(0.2f).Within(0.05f));
        Assert.That(timeout, Is.True);
        Assert.That(x.IsRunning, Is.False);

        // A bigger timeout is ok when stopped
        timeout = false;
        x.Restart();
        Assert.That(timeout, Is.False);
        Assert.That(x.IsRunning, Is.True);
        Assert.That(x.TimeLeft, Is.EqualTo(0.2f));
        await Delay(0.1f);
        Assert.That(x.Elapsed, Is.EqualTo(0.1f).Within(0.05f));
        x.Stop();
        Assert.That(timeout, Is.False);
        Assert.That(x.IsRunning, Is.False);
        Assert.That(x.TimeLeft, Is.EqualTo(0.1f).Within(0.05));
        x.SetTimeout(0.3f);
        Assert.That(x.TimeLeft, Is.EqualTo(0.2f).Within(0.05));
        x.Start();
        Assert.That(x.TimeLeft, Is.EqualTo(0.2f).Within(0.05));
    }

    public SignalAwaiter Delay(float s) {
        return GetTree().CreateTimer(s).AwaitTimeout();
    }
}