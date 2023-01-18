using Betauer.Core;
using NUnit.Framework;

namespace Betauer.Bus.Tests {
    [TestFixture]
    public class BusUnicastTests {

        [Test]
        public void BasicTest() {
            var bus = new Unicast<string, string>();
            var calls1 = 0;
            var calls2 = 0;
            bus.Subscribe((sender, args) => {
                Assert.That(sender, Is.EqualTo("sender"));
                Assert.That(args, Is.EqualTo("args"));
                calls1++;
            });
            bus.Publish("sender", "args");
            Assert.That(calls1, Is.EqualTo(1));

            bus.Subscribe((sender, args) => {
                Assert.That(sender, Is.EqualTo("sender"));
                Assert.That(args, Is.EqualTo("args"));
                calls2++;
            });
            
            bus.Publish("sender", "args");
            bus.Publish("sender", "args");
            Assert.That(calls1, Is.EqualTo(1));
            Assert.That(calls2, Is.EqualTo(2));
        }

        [Test]
        public void ConditionalTest() {
            var bus = new Unicast<string, string> {
                Condition = (publisher, args) => publisher == "sender" && args == "args"
            };
            var calls = 0;
            bus.Subscribe((sender, args) => calls++);
            
            bus.Publish("sender", "1");
            bus.Publish("sender", "args");
            bus.Publish("1", "args");
            Assert.That(calls, Is.EqualTo(1));
        }
        
        [Test]
        public void BasicRemoveTest() {
            var bus = new Unicast<string, string>();
            var calls = 0;
            var consumer = bus.Subscribe((sender, args) => {
                calls++;
            });
            
            bus.Publish("sender", "args");
            Assert.That(calls, Is.EqualTo(1));
            consumer.Unsubscribe();
            
            bus.Publish("sender", "args");
            Assert.That(calls, Is.EqualTo(1));
        }
        
        [Test]
        public void BasicRemoveIfTest() {
            var bus = new Unicast<string, string>();
            var remove = false;
            var calls = 0;
            bus.Subscribe((sender, args) => {
                calls++;
            }).UnsubscribeIf(() => remove);
            
            bus.Publish("sender", "args");
            Assert.That(calls, Is.EqualTo(1));
            remove = true;
            
            bus.Publish("sender", "args");
            Assert.That(calls, Is.EqualTo(1));
        }
        
        [Test]
        public void BasicRemoveIfInvalidTest() {
            var bus = new Unicast<string, string>();
            var o = new Godot.Object();
            var calls = 0;
            var consumer = bus.Subscribe((sender, args) => {
                calls++;
            }).UnsubscribeIf(Predicates.IsInvalid(o));
            
            bus.Publish("sender", "args");
            Assert.That(calls, Is.EqualTo(1));
            o.Free();
            
            bus.Publish("sender", "args");
            Assert.That(calls, Is.EqualTo(1));

        }
        
    }
}