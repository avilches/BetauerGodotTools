using System.Linq;
using System.Threading.Tasks;
using Betauer.Memory;
using Betauer.Signal;
using Betauer.TestRunner;
using Godot;
using NUnit.Framework;

namespace Betauer.Tests {
    [TestFixture]
    public class SignalExtensionsTests : Node {
        [SetUp]
        public void Setup() {
            LoggerFactory.SetTraceLevel(typeof(Consumer), TraceLevel.All);
            LoggerFactory.SetTraceLevel(typeof(SignalManager), TraceLevel.All);
            foreach (var child in GetChildren().OfType<Node>()) child.Free();
            DefaultObjectWatcherTask.Instance.Dispose();
            Assert.That(DefaultObjectWatcherTask.Instance.Size, Is.EqualTo(0));
            DefaultObjectWatcherTask.Instance = new ObjectWatcherTask();
        }

        [Test(Description = "Signal works + watch signal origin")]
        public async Task SignalLambdaWorksAndWatchTest() {
            var b1 = new CheckButton();
            AddChild(b1);
            await this.AwaitIdleFrame();
            var executed1 = 0;

            SignalHandler p1 = b1.OnPressed(() => executed1++);
            b1.EmitSignal("pressed");
            Assert.That(executed1, Is.EqualTo(1));
            Assert.That(DefaultObjectWatcherTask.Instance.Size, Is.EqualTo(1));
            Assert.That(DefaultSignalManager.Instance.GetSignalCount(b1), Is.EqualTo(1));

            // SignalHandler object are still there
            DefaultObjectWatcherTask.Instance.Consume();
            Assert.That(DefaultObjectWatcherTask.Instance.Size, Is.EqualTo(1));
            Assert.That(DefaultSignalManager.Instance.GetSignalCount(b1), Is.EqualTo(1));
            
            // When signal origin is freed, SignalHandler object is removed
            b1.Free();
            Assert.That(p1.IsConnected(), Is.False);
            Assert.That(p1.CheckOriginConnection(), Is.False);
            DefaultObjectWatcherTask.Instance.Consume();
            Assert.That(DefaultObjectWatcherTask.Instance.Size, Is.EqualTo(0));
            Assert.That(DefaultSignalManager.Instance.GetSignalCount(b1), Is.EqualTo(0));
        }


        [Test(Description = "Allow multiple signal same origin same signal")]
        public async Task MultipleSignal() {
            var b1 = new CheckButton();
            AddChild(b1);
            await this.AwaitIdleFrame();
            var executed1 = 0;
            var executed2 = 0;
            var executed3 = 0;
            var executed4 = 0;

            SignalHandler p1 = b1.OnPressed(() => executed1++);
            SignalHandler p2 = b1.OnPressed(() => executed2++);
            SignalHandler p3 = b1.OnPressed(() => executed3++, true).Disconnect();
            SignalHandler p4 = b1.OnPressed(() => executed4++);
            Assert.That(DefaultSignalManager.Instance.GetSignalCount(b1), Is.EqualTo(3));
            Assert.That(executed1, Is.EqualTo(0));
            Assert.That(executed2, Is.EqualTo(0));
            Assert.That(executed4, Is.EqualTo(0));
            Assert.That(executed2, Is.EqualTo(0));
            Assert.That(p1.IsConnected(), Is.True);
            Assert.That(p2.IsConnected(), Is.True);
            Assert.That(p3.IsConnected(), Is.False);
            Assert.That(p4.IsConnected(), Is.True);

            b1.EmitSignal("pressed");
            Assert.That(executed1, Is.EqualTo(1));
            Assert.That(executed2, Is.EqualTo(1));
            Assert.That(executed3, Is.EqualTo(0)); // disconnected
            Assert.That(executed4, Is.EqualTo(1));
            
            p3.Connect();
            p3.Connect();
            Assert.That(DefaultSignalManager.Instance.GetSignalCount(b1), Is.EqualTo(4));
            p1.Disconnect();
            p1.Disconnect();
            Assert.That(DefaultSignalManager.Instance.GetSignalCount(b1), Is.EqualTo(3));
            Assert.That(p1.IsConnected(), Is.False);
            Assert.That(p2.IsConnected(), Is.True);
            Assert.That(p3.IsConnected(), Is.True);
            Assert.That(p4.IsConnected(), Is.True);

            
            b1.EmitSignal("pressed");
            Assert.That(executed1, Is.EqualTo(1)); // disconnected
            Assert.That(executed2, Is.EqualTo(2));
            Assert.That(executed3, Is.EqualTo(1)); // re-connected (one shot)
            Assert.That(executed4, Is.EqualTo(2));

            Assert.That(p1.IsConnected(), Is.False);
            Assert.That(p2.IsConnected(), Is.True);
            Assert.That(p3.IsConnected(), Is.False); // one shot disconnected
            Assert.That(p4.IsConnected(), Is.True);
            Assert.That(DefaultSignalManager.Instance.GetSignalCount(b1), Is.EqualTo(2));

            b1.EmitSignal("pressed");
            Assert.That(executed1, Is.EqualTo(1));
            Assert.That(executed2, Is.EqualTo(3));
            Assert.That(executed3, Is.EqualTo(1)); // one shot
            Assert.That(executed4, Is.EqualTo(3));
            
            Assert.That(DefaultObjectWatcherTask.Instance.Size, Is.EqualTo(1));
            Assert.That(DefaultSignalManager.Instance.GetSignalCount(b1), Is.EqualTo(2));

            p2.Disconnect();
            Assert.That(p1.IsConnected(), Is.False);
            Assert.That(p2.IsConnected(), Is.False);
            Assert.That(p3.IsConnected(), Is.False);
            Assert.That(p4.IsConnected(), Is.True);
            Assert.That(p1.CheckOriginConnection(), Is.True);
            Assert.That(p2.CheckOriginConnection(), Is.True);
            Assert.That(p3.CheckOriginConnection(), Is.True);
            Assert.That(p4.CheckOriginConnection(), Is.True);
            Assert.That(DefaultObjectWatcherTask.Instance.Size, Is.EqualTo(1));
            Assert.That(DefaultSignalManager.Instance.GetSignalCount(b1), Is.EqualTo(1));

            p4.Disconnect();
            Assert.That(p1.IsConnected(), Is.False);
            Assert.That(p2.IsConnected(), Is.False);
            Assert.That(p3.IsConnected(), Is.False);
            Assert.That(p4.IsConnected(), Is.False);
            Assert.That(p1.CheckOriginConnection(), Is.False);
            Assert.That(p2.CheckOriginConnection(), Is.False);
            Assert.That(p3.CheckOriginConnection(), Is.False);
            Assert.That(p4.CheckOriginConnection(), Is.False);
            DefaultObjectWatcherTask.Instance.Consume();
            Assert.That(DefaultObjectWatcherTask.Instance.Size, Is.EqualTo(1));
            Assert.That(DefaultSignalManager.Instance.GetSignalCount(b1), Is.EqualTo(0));
            
            b1.EmitSignal("pressed");
            Assert.That(p3.CheckOriginConnection(), Is.False);
        }


        [Test(Description = "Signal auto disconnect on watching node")]
        public async Task SignalAutoDisconnectTests() {
            var b1 = new CheckButton();
            var w = new Object();
            var no = new Object();
            AddChild(b1);
            await this.AwaitIdleFrame();

            SignalHandler p1 = b1.OnPressed(() => { });
            SignalHandler p2 = b1.OnPressed(() => { });
            
            // Watch added but removed, nothing happens when it's freed
            p1.DisconnectIfInvalid(no).StopWatching();
            no.Free();
            DefaultObjectWatcherTask.Instance.Consume();
            Assert.That(p1.IsConnected(), Is.True);
            Assert.That(p2.IsConnected(), Is.True);
            Assert.That(p1.CheckOriginConnection(), Is.True);
            Assert.That(p2.CheckOriginConnection(), Is.True);
            Assert.That(DefaultObjectWatcherTask.Instance.Size, Is.EqualTo(1));
            Assert.That(DefaultSignalManager.Instance.GetSignalCount(b1), Is.EqualTo(2));
            
            // Added a real watcher
            p1.DisconnectIfInvalid(w);
            Assert.That(DefaultObjectWatcherTask.Instance.Size, Is.EqualTo(2));
            
            w.Free();
            DefaultObjectWatcherTask.Instance.Consume();
            Assert.That(p1.IsConnected(), Is.False);
            Assert.That(p2.IsConnected(), Is.True);
            Assert.That(p1.CheckOriginConnection(), Is.True);
            Assert.That(p1.CheckOriginConnection(), Is.True);
            Assert.That(DefaultObjectWatcherTask.Instance.Size, Is.EqualTo(1));
            Assert.That(DefaultSignalManager.Instance.GetSignalCount(b1), Is.EqualTo(1));
        }

        [Test(Description = "Signal connect and disconnect")]
        public async Task ConnectAndDisconnectLambdaTests() {
            var b1 = new CheckButton();
            AddChild(b1);
            await this.AwaitIdleFrame();
            var executed1 = 0;

            SignalHandler p1 = b1.OnPressed(() => executed1 ++);
            Assert.That(p1.IsConnected(), Is.True);
            Assert.That(p1.CheckOriginConnection(), Is.True);
            Assert.That(DefaultSignalManager.Instance.GetSignalCount(b1), Is.EqualTo(1));

            b1.EmitSignal("pressed");
            Assert.That(executed1, Is.EqualTo(1));
            
            p1.Disconnect();
            p1.Disconnect(); // Idempotent
            Assert.That(p1.IsConnected(), Is.False);
            Assert.That(p1.CheckOriginConnection(), Is.False);
            Assert.That(DefaultSignalManager.Instance.GetSignalCount(b1), Is.EqualTo(0));

            b1.EmitSignal("pressed");
            Assert.That(executed1, Is.EqualTo(1));

            p1.Connect();
            p1.Connect(); // Idempotent
            Assert.That(p1.IsConnected(), Is.True);
            Assert.That(p1.CheckOriginConnection(), Is.True);
            Assert.That(DefaultSignalManager.Instance.GetSignalCount(b1), Is.EqualTo(1));

            b1.EmitSignal("pressed");
            b1.EmitSignal("pressed");
            Assert.That(executed1, Is.EqualTo(3));
            
            b1.Free();
            Assert.That(p1.IsConnected(), Is.False);
            Assert.That(p1.CheckOriginConnection(), Is.False);
        }

        [Test(Description = "Signal oneShot should only work once then it is auto freed")]
        public async Task SignalToLambdaOneShotTest() {
            var b1 = new CheckButton();
            AddChild(b1);
            await this.AwaitIdleFrame();
            var executed1 = 0;

            SignalHandler p1 = b1.OnPressed(() => executed1 ++, true);
            Assert.That(p1.IsConnected(), Is.True);
            Assert.That(p1.CheckOriginConnection(), Is.True);
            Assert.That(DefaultSignalManager.Instance.GetSignalCount(b1), Is.EqualTo(1));

            b1.EmitSignal("pressed");
            Assert.That(executed1, Is.EqualTo(1));
            Assert.That(DefaultSignalManager.Instance.GetSignalCount(b1), Is.EqualTo(0));
            Assert.That(p1.IsConnected(), Is.False);
            Assert.That(p1.CheckOriginConnection(), Is.False);
            
            b1.EmitSignal("pressed");
            b1.EmitSignal("pressed");
            // More executions are ignored
            Assert.That(executed1, Is.EqualTo(1));
        }

        [Test(Description = "Signal deferred should work next frame")]
        public async Task SignalToLambdaDeferredTest() {
            var b1 = new CheckButton();
            AddChild(b1);
            await this.AwaitIdleFrame();
            var executed1 = 0;

            SignalHandler p1 = b1.OnPressed(() => executed1 ++, false, true);
            Assert.That(p1.IsConnected(), Is.True);
            Assert.That(p1.CheckOriginConnection(), Is.True);

            b1.EmitSignal("pressed");
            b1.EmitSignal("pressed");
            b1.EmitSignal("pressed");
            Assert.That(executed1, Is.EqualTo(0));
            
            await this.AwaitIdleFrame();
            Assert.That(executed1, Is.EqualTo(3));
            Assert.That(p1.IsConnected(), Is.True);
            Assert.That(p1.CheckOriginConnection(), Is.True);
        }

        [Test(Description = "Signal deferred and one shot should work next frame and no more")]
        public async Task SignalToLambdaOneShotDeferredTest() {
            var b1 = new CheckButton();
            AddChild(b1);
            await this.AwaitIdleFrame();
            var executed1 = 0;

            SignalHandler p1 = b1.OnPressed(() => executed1 ++, true, true);
            Assert.That(p1.IsConnected(), Is.True);
            Assert.That(p1.CheckOriginConnection(), Is.True);
            Assert.That(DefaultSignalManager.Instance.GetSignalCount(b1), Is.EqualTo(1));

            b1.EmitSignal("pressed");
            Assert.That(executed1, Is.EqualTo(0));
            Assert.That(p1.IsConnected(), Is.True);
            Assert.That(p1.CheckOriginConnection(), Is.True);
            Assert.That(DefaultSignalManager.Instance.GetSignalCount(b1), Is.EqualTo(1));
            
            await this.AwaitIdleFrame();
            Assert.That(executed1, Is.EqualTo(1));
            Assert.That(p1.IsConnected(), Is.False);
            Assert.That(p1.CheckOriginConnection(), Is.False);
            Assert.That(DefaultSignalManager.Instance.GetSignalCount(b1), Is.EqualTo(0));
        }

        [Test(Description = "Test signal parameters")]
        public async Task SignalParameterTest() {
            var b1 = new CheckButton();
            AddChild(b1);
            await this.AwaitIdleFrame();
            
            b1.AddUserSignal("1p");
            b1.AddUserSignal("2p");
            b1.AddUserSignal("3p");
            b1.AddUserSignal("4p");
            b1.AddUserSignal("5p");
            b1.AddUserSignal("6p");
            b1.On("1p", (int p1) => {
                Assert.That(p1, Is.EqualTo(1));
            });
            
            b1.On("2p", (int p1, int p2) => {
                Assert.That(p1, Is.EqualTo(1));
                Assert.That(p2, Is.EqualTo(2));
            });
            
            b1.On("3p", (int p1, int p2, int p3) => {
                Assert.That(p1, Is.EqualTo(1));
                Assert.That(p2, Is.EqualTo(2));
                Assert.That(p3, Is.EqualTo(3));
            });
            
            b1.On("4p", (int p1, int p2, int p3, int p4) => {
                Assert.That(p1, Is.EqualTo(1));
                Assert.That(p2, Is.EqualTo(2));
                Assert.That(p3, Is.EqualTo(3));
                Assert.That(p4, Is.EqualTo(4));
            });
            
            b1.On("5p", (int p1, int p2, int p3, int p4, int p5) => {
                Assert.That(p1, Is.EqualTo(1));
                Assert.That(p2, Is.EqualTo(2));
                Assert.That(p3, Is.EqualTo(3));
                Assert.That(p4, Is.EqualTo(4));
                Assert.That(p5, Is.EqualTo(5));
            });
            
            b1.On("6p", (int p1, int p2, int p3, int p4, int p5, int p6) => {
                Assert.That(p1, Is.EqualTo(1));
                Assert.That(p2, Is.EqualTo(2));
                Assert.That(p3, Is.EqualTo(3));
                Assert.That(p4, Is.EqualTo(4));
                Assert.That(p5, Is.EqualTo(5));
                Assert.That(p6, Is.EqualTo(6));
            });

            Assert.Throws<AssertionException>(() => b1.EmitSignal("1p", 999));
            Assert.Throws<AssertionException>(() => b1.EmitSignal("2p", 999, 2));
            Assert.Throws<AssertionException>(() => b1.EmitSignal("3p", 999, 2, 3));
            Assert.Throws<AssertionException>(() => b1.EmitSignal("4p", 999, 2, 3, 4));
            Assert.Throws<AssertionException>(() => b1.EmitSignal("5p", 999, 2, 3, 4, 5));
            Assert.Throws<AssertionException>(() => b1.EmitSignal("6p", 999, 2, 3, 4, 5, 6));

            b1.EmitSignal("1p", 1);
            b1.EmitSignal("2p", 1, 2);
            b1.EmitSignal("3p", 1, 2, 3);
            b1.EmitSignal("4p", 1, 2, 3, 4);
            b1.EmitSignal("5p", 1, 2, 3, 4, 5);
            b1.EmitSignal("6p", 1, 2, 3, 4, 5, 6);
        }

        [Test(Description = "RemoveAndDisconnectAll")]
        public async Task RemoveAndDisconnectAllTest() {
            var b1 = new CheckButton();
            var b2 = new CheckButton();
            AddChild(b1);
            AddChild(b2);
            await this.AwaitIdleFrame();

            var x = 0;
            SignalHandler p11 = b1.OnPressed(() => x++);
            SignalHandler p12 = b1.OnPressed(() => x++);
            SignalHandler p13 = b1.OnToggled((b) => x++);
            SignalHandler p14 = b1.OnToggled((b) => x++);
            
            SignalHandler p21 = b2.OnPressed(() => x++);
            SignalHandler p22 = b2.OnPressed(() => x++);
            SignalHandler p23 = b2.OnToggled((b) => x++);
            SignalHandler p24 = b2.OnToggled((b) => x++);

            Assert.That(DefaultObjectWatcherTask.Instance.Size, Is.EqualTo(2));
            Assert.That(DefaultSignalManager.Instance.GetSignalCount(b1), Is.EqualTo(4));
            Assert.That(DefaultSignalManager.Instance.GetSignalCount(b2), Is.EqualTo(4));

            DefaultSignalManager.Instance.DisconnectAll(b1);

            Assert.That(DefaultObjectWatcherTask.Instance.Size, Is.EqualTo(2));
            Assert.That(DefaultSignalManager.Instance.GetSignalCount(b1), Is.EqualTo(0));
            Assert.That(DefaultSignalManager.Instance.GetSignalCount(b2), Is.EqualTo(4));

            Assert.That(p11.IsConnected(), Is.False);
            Assert.That(p11.CheckOriginConnection(), Is.False);
            Assert.That(p12.IsConnected(), Is.False);
            Assert.That(p12.CheckOriginConnection(), Is.False);
            Assert.That(p13.IsConnected(), Is.False);
            Assert.That(p13.CheckOriginConnection(), Is.False);
            Assert.That(p14.IsConnected(), Is.False);
            Assert.That(p14.CheckOriginConnection(), Is.False);

            Assert.That(p21.IsConnected(), Is.True);
            Assert.That(p21.CheckOriginConnection(), Is.True);
            Assert.That(p22.IsConnected(), Is.True);
            Assert.That(p22.CheckOriginConnection(), Is.True);
            Assert.That(p23.IsConnected(), Is.True);
            Assert.That(p23.CheckOriginConnection(), Is.True);
            Assert.That(p24.IsConnected(), Is.True);
            Assert.That(p24.CheckOriginConnection(), Is.True);
        }

        [Test(Description = "Multiple signals")]
        public async Task MultipleSignals() {
            var b1 = new CheckButton();
            var b2 = new CheckButton();
            AddChild(b1);
            AddChild(b2);
            await this.AwaitIdleFrame();
            var executed11 = 0;
            var executed12 = 0;
            var executed21 = 0;
            var executed23 = 0;

            SignalHandler p11 = b1.OnPressed(() => executed11++);
            SignalHandler p12 = b1.OnToggled((x) => executed12++);
            SignalHandler p21 = b2.OnPressed(() => executed21++);
            SignalHandler p23 = b2.OnToggled((x) => executed23++);
            b1.EmitSignal("pressed");
            b1.Pressed = true;
            b1.Pressed = false;
            b2.EmitSignal("pressed");
            b2.EmitSignal("pressed");
            b2.EmitSignal("pressed");
            b2.Pressed = true;
            b2.Pressed = false;
            b2.Pressed = true;
            b2.Pressed = false;
            Assert.That(executed11, Is.EqualTo(1));
            Assert.That(executed12, Is.EqualTo(2));
            Assert.That(executed21, Is.EqualTo(3));
            Assert.That(executed23, Is.EqualTo(4));
            Assert.That(DefaultObjectWatcherTask.Instance.Size, Is.EqualTo(2));
            Assert.That(DefaultSignalManager.Instance.GetSignalCount(b1), Is.EqualTo(2));
            Assert.That(DefaultSignalManager.Instance.GetSignalCount(b2), Is.EqualTo(2));

            // SignalHandler object are still there
            DefaultObjectWatcherTask.Instance.Consume();
            Assert.That(DefaultObjectWatcherTask.Instance.Size, Is.EqualTo(2));
            Assert.That(DefaultSignalManager.Instance.GetSignalCount(b1), Is.EqualTo(2));
            Assert.That(DefaultSignalManager.Instance.GetSignalCount(b2), Is.EqualTo(2));
            
            // When signal origin is freed, SignalHandler object is removed
            b1.Free();
            DefaultObjectWatcherTask.Instance.Consume();
            Assert.That(DefaultObjectWatcherTask.Instance.Size, Is.EqualTo(1));
            Assert.That(DefaultSignalManager.Instance.GetSignalCount(b1), Is.EqualTo(0));
            Assert.That(DefaultSignalManager.Instance.GetSignalCount(b2), Is.EqualTo(2));

            b2.Free();
            DefaultObjectWatcherTask.Instance.Consume();
            Assert.That(DefaultObjectWatcherTask.Instance.Size, Is.EqualTo(0));
            Assert.That(DefaultSignalManager.Instance.GetSignalCount(b1), Is.EqualTo(0));
            Assert.That(DefaultSignalManager.Instance.GetSignalCount(b2), Is.EqualTo(0));
        }
    }
}