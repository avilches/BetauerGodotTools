using Betauer.Core;
using Betauer.TestRunner;
using NUnit.Framework;

namespace Betauer.Bus.Tests; 

[TestRunner.Test]
public class BusMulticastTests {

    [TestRunner.Test]
    public void BasicTest() {
        var bus = new Multicast<string>();
        var calls1 = 0;
        var calls2 = 0;
        bus.Subscribe((args) => {
            Assert.That(args, Is.EqualTo("args"));
            calls1++;
        });
        bus.Publish("args");
        Assert.That(calls1, Is.EqualTo(1));

        bus.Subscribe((args) => {
            Assert.That(args, Is.EqualTo("args"));
            calls2++;
        });
        Assert.That(bus.Consumers.Count, Is.EqualTo(2));
            
        bus.Publish("args");
        Assert.That(calls1, Is.EqualTo(2));
        Assert.That(calls2, Is.EqualTo(1));
    }
        
    [TestRunner.Test]
    public void BasicRemoveTest() {
        var bus = new Multicast<string>();
        var calls = 0;
        var consumer = bus.Subscribe((args) => {
            calls++;
        });
        Assert.That(bus.Consumers.Count, Is.EqualTo(1));
            
        bus.Publish("args");
        Assert.That(calls, Is.EqualTo(1));
        consumer.Unsubscribe();
            
        bus.Publish("args");
        Assert.That(calls, Is.EqualTo(1));
        Assert.That(bus.Consumers.Count, Is.EqualTo(0));
    }
        
    [TestRunner.Test]
    public void BasicRemoveIfTest() {
        var bus = new Multicast<string>();
        var remove = false;
        var calls = 0;
        bus.Subscribe((args) => {
            calls++;
        }).UnsubscribeIf(() => remove);
        Assert.That(bus.Consumers.Count, Is.EqualTo(1));
            
        bus.Publish( "args");
        Assert.That(calls, Is.EqualTo(1));
        remove = true;
            
        bus.Publish( "args");
        Assert.That(calls, Is.EqualTo(1));
        Assert.That(bus.Consumers.Count, Is.EqualTo(0));
    }
        
    [TestRunner.Test]
    public void BasicRemoveIfInvalidTest() {
        var bus = new Multicast<string>();
        var o = new Godot.GodotObject();
        var calls = 0;
        var consumer = bus.Subscribe((args) => {
            calls++;
        });
        consumer.UnsubscribeIf(Predicates.IsInvalid(o));
        Assert.That(bus.Consumers.Count, Is.EqualTo(1));
            
        bus.Publish("args");
        Assert.That(calls, Is.EqualTo(1));
        o.Free();
            
        bus.Publish("args");
        Assert.That(calls, Is.EqualTo(1));
        Assert.That(bus.Consumers.Count, Is.EqualTo(0));

    }

    public interface IEvent {
    }

    public class Event1 : IEvent {
    }

    public class Event2 : IEvent {
    }

    [TestRunner.Test]
    public void AllowSubTypesArgs() {
        var bus = new Multicast<IEvent>();
        var calls = 0;
        var consumer = bus.Subscribe((args) => {
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

}