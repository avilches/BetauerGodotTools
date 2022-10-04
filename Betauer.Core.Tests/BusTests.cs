using Betauer.Bus;
using Betauer.TestRunner;
using NUnit.Framework;

namespace Betauer.Tests {
    [TestFixture]
    [Only]
    public class BusTests {

        [Test]
        public void BasicTest() {
            var bus = new Multicast<string, string>();
            var calls = 0;
            bus.Subscribe((sender, args) => {
                Assert.That(sender, Is.EqualTo("sender"));
                Assert.That(args, Is.EqualTo("args"));
                calls++;
            });
            bus.Subscribe((sender, args) => {
                Assert.That(sender, Is.EqualTo("sender"));
                Assert.That(args, Is.EqualTo("args"));
                calls++;
            });
            Assert.That(bus.Consumers.Count, Is.EqualTo(2));
            
            bus.Publish("sender", "args");
            bus.Publish("sender", "args");
            Assert.That(calls, Is.EqualTo(4));
        }
        
        [Test]
        public void BasicRemoveTest() {
            var bus = new Multicast<string, string>();
            var calls = 0;
            var consumer = bus.Subscribe((sender, args) => {
                calls++;
            });
            Assert.That(bus.Consumers.Count, Is.EqualTo(1));
            
            bus.Publish("sender", "args");
            Assert.That(calls, Is.EqualTo(1));
            consumer.Remove();
            
            bus.Publish("sender", "args");
            Assert.That(calls, Is.EqualTo(1));
            Assert.That(bus.Consumers.Count, Is.EqualTo(0));
        }
        
        [Test]
        public void BasicRemoveIfTest() {
            var bus = new Multicast<string, string>();
            var remove = false;
            var calls = 0;
            bus.Subscribe((sender, args) => {
                calls++;
            }).RemoveIf(() => remove);
            Assert.That(bus.Consumers.Count, Is.EqualTo(1));
            
            bus.Publish("sender", "args");
            Assert.That(calls, Is.EqualTo(1));
            remove = true;
            
            bus.Publish("sender", "args");
            Assert.That(calls, Is.EqualTo(1));
            Assert.That(bus.Consumers.Count, Is.EqualTo(0));
        }
        
        [Test]
        public void BasicRemoveIfInvalidTest() {
            var bus = new Multicast<string, string>();
            var o = new Godot.Object();
            var calls = 0;
            var consumer = bus.Subscribe((sender, args) => {
                calls++;
            }).RemoveIfInvalid(o);
            Assert.That(bus.Consumers.Count, Is.EqualTo(1));
            
            bus.Publish("sender", "args");
            Assert.That(calls, Is.EqualTo(1));
            o.Free();
            
            bus.Publish("sender", "args");
            Assert.That(calls, Is.EqualTo(1));
            Assert.That(bus.Consumers.Count, Is.EqualTo(0));

        }
        
    }
}