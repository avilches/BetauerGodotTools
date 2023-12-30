using Betauer.Core;
using Betauer.TestRunner;
using NUnit.Framework;

namespace Betauer.Bus.Tests; 

[TestRunner.Test]
public class BusUnicastTests {

    [TestRunner.Test]
    public void BasicTest() {
        var bus = new Unicast<string>();
        var calls1 = 0;
        var calls2 = 0;
        bus.Subscribe((args) => {
            Assert.That(args, Is.EqualTo("args"));
            calls1++;
        });
        bus.Execute( "args");
        Assert.That(calls1, Is.EqualTo(1));

        bus.Subscribe((args) => {
            Assert.That(args, Is.EqualTo("args"));
            calls2++;
        });
            
        bus.Execute("args");
        bus.Execute("args");
        Assert.That(calls1, Is.EqualTo(1));
        Assert.That(calls2, Is.EqualTo(2));
    }

    [TestRunner.Test]
    public void BasicRemoveTest() {
        var bus = new Unicast<string>();
        var calls = 0;
        var consumer = bus.Subscribe((args) => {
            calls++;
        });
            
        bus.Execute("args");
        Assert.That(calls, Is.EqualTo(1));
        consumer.Unsubscribe();
            
        bus.Execute("args");
        Assert.That(calls, Is.EqualTo(1));
    }
        
    [TestRunner.Test]
    public void BasicRemoveIfTest() {
        var bus = new Unicast<string>();
        var remove = false;
        var calls = 0;
        bus.Subscribe((args) => {
            calls++;
        }).UnsubscribeIf(() => remove);
            
        bus.Execute( "args");
        Assert.That(calls, Is.EqualTo(1));
        remove = true;
            
        bus.Execute( "args");
        Assert.That(calls, Is.EqualTo(1));
    }
        
    [TestRunner.Test]
    public void BasicRemoveIfInvalidTest() {
        var bus = new Unicast<string>();
        var o = new Godot.GodotObject();
        var calls = 0;
        var consumer = bus.Subscribe((args) => {
            calls++;
        });
        consumer.UnsubscribeIf(Predicates.IsInvalid(o));
            
        bus.Execute( "args");
        Assert.That(calls, Is.EqualTo(1));
        o.Free();
            
        bus.Execute( "args");
        Assert.That(calls, Is.EqualTo(1));

    }
    public interface IEvent {
    }

    public class Event1 : IEvent {
    }

    public class Event2 : IEvent {
    }

    [TestRunner.Test]
    public void AllowSubTypesArgs() {
        var bus = new Unicast<IEvent>();
        var calls = 0;
        var consumer = bus.Subscribe((args) => {
            calls++;
        });
        bus.Execute(new Event1());
        bus.Execute(new Event2());
        bus.Execute(null);
        Assert.That(calls, Is.EqualTo(3));

        consumer.Unsubscribe();
        calls = 0;
        consumer = bus.Subscribe(ev => {
            calls++;
        });
        bus.Execute(new Event1());
        bus.Execute(new Event2());
        bus.Execute(null);
        Assert.That(calls, Is.EqualTo(3));

        consumer.Unsubscribe();
        calls = 0;
        consumer = bus.Subscribe((Event1 ev) => {
            calls++;
        });
        bus.Execute(new Event1());
        Assert.That(calls, Is.EqualTo(1));
        bus.Execute(new Event2());
        Assert.That(calls, Is.EqualTo(1));
        bus.Execute(null);
        Assert.That(calls, Is.EqualTo(2));

        consumer.Unsubscribe();
        calls = 0;
        consumer = bus.Subscribe((Event2 ev) => {
            calls++;
        });
        bus.Execute(new Event1());
        Assert.That(calls, Is.EqualTo(0));
        bus.Execute(new Event2());
        Assert.That(calls, Is.EqualTo(1));
        bus.Execute(null);
        Assert.That(calls, Is.EqualTo(2));
    }

}