using Betauer.Core;
using Betauer.TestRunner;
using NUnit.Framework;

namespace Betauer.Bus.Tests {
    [TestFixture]
    public class BusMulticastTests {

        [Test]
        public void BasicTest() {
            var bus = new Multicast<string, string>();
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
            Assert.That(bus.Consumers.Count, Is.EqualTo(2));
            
            bus.Publish("sender", "args");
            Assert.That(calls1, Is.EqualTo(2));
            Assert.That(calls2, Is.EqualTo(1));
        }
        
        [Test]
        public void ConditionalTest() {
            var bus = new Multicast<string, string> {
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
            var bus = new Multicast<string, string>();
            var calls = 0;
            var consumer = bus.Subscribe((sender, args) => {
                calls++;
            });
            Assert.That(bus.Consumers.Count, Is.EqualTo(1));
            
            bus.Publish("sender", "args");
            Assert.That(calls, Is.EqualTo(1));
            consumer.Unsubscribe();
            
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
            }).UnsubscribeIf(() => remove);
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
            }).UnsubscribeIf(Predicates.IsInvalid(o));
            Assert.That(bus.Consumers.Count, Is.EqualTo(1));
            
            bus.Publish("sender", "args");
            Assert.That(calls, Is.EqualTo(1));
            o.Free();
            
            bus.Publish("sender", "args");
            Assert.That(calls, Is.EqualTo(1));
            Assert.That(bus.Consumers.Count, Is.EqualTo(0));

        }

        public interface ISender {
        }

        public class Sender1 : ISender {
        }

        public class Sender2 : ISender {
        }

        public interface IEvent {
        }

        public class Event1 : IEvent {
        }

        public class Event2 : IEvent {
        }

        [Test]
        public void AllowSubTypesArgs() {
            var bus = new Multicast<ISender, IEvent>();
            var calls = 0;
            var consumer = bus.Subscribe(() => {
                calls++;
            });
            bus.Publish(new Event1());
            bus.Publish(new Event2());
            bus.Publish(null);
            Assert.That(calls, Is.EqualTo(3));

            consumer.Unsubscribe();
            calls = 0;
            consumer = bus.Subscribe(ev => {
                calls++;
            });
            bus.Publish(new Event1());
            bus.Publish(new Event2());
            bus.Publish(null);
            Assert.That(calls, Is.EqualTo(3));

            consumer.Unsubscribe();
            calls = 0;
            consumer = bus.Subscribe((Event1 ev) => {
                calls++;
            });
            bus.Publish(new Event1());
            Assert.That(calls, Is.EqualTo(1));
            bus.Publish(new Event2());
            Assert.That(calls, Is.EqualTo(1));
            bus.Publish(null);
            Assert.That(calls, Is.EqualTo(2));

            consumer.Unsubscribe();
            calls = 0;
            consumer = bus.Subscribe((Event2 ev) => {
                calls++;
            });
            bus.Publish(new Event1());
            Assert.That(calls, Is.EqualTo(0));
            bus.Publish(new Event2());
            Assert.That(calls, Is.EqualTo(1));
            bus.Publish(null);
            Assert.That(calls, Is.EqualTo(2));
        }

        [Test]
        public void AllowSubTypesSenderAndArgs() {
            var bus = new Multicast<ISender, IEvent>();
            var calls = 0;
            var consumer = bus.Subscribe((s, ev) => {
                calls++;
            });
            bus.Publish(new Event1());
            bus.Publish(new Event2());
            bus.Publish(null);
            Assert.That(calls, Is.EqualTo(3));

            consumer.Unsubscribe();
            calls = 0;
            consumer = bus.Subscribe((Sender1 s, Event1 ev) => {
                calls++;
            });
            bus.Publish(new Sender1(), new Event2());
            bus.Publish(new Sender2(), new Event1());
            bus.Publish(new Sender2(), new Event2());
            bus.Publish(new Sender2(), null);
            bus.Publish(null, new Event2());
            Assert.That(calls, Is.EqualTo(0));

            
            bus.Publish(new Sender1(), new Event1());
            bus.Publish(new Sender1(), null);
            bus.Publish(null, new Event1());
            bus.Publish(null, null);
            Assert.That(calls, Is.EqualTo(4));
            
            consumer.Unsubscribe();
            calls = 0;
            consumer = bus.Subscribe((Sender2 s, Event2 ev) => {
                calls++;
            });
            bus.Publish(new Sender1(), new Event1());
            bus.Publish(new Sender1(), new Event2());
            bus.Publish(new Sender1(), null);
            bus.Publish(new Sender2(), new Event1());
            bus.Publish(null, new Event1());
            Assert.That(calls, Is.EqualTo(0));

            bus.Publish(new Sender2(), new Event2());
            bus.Publish(new Sender2(), null);
            bus.Publish(null, new Event2());
            bus.Publish(null, null);
            Assert.That(calls, Is.EqualTo(4));
        }
        
    }
}