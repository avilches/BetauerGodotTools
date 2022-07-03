using System;
using System.Threading.Tasks;
using Betauer.Memory;
using Betauer.TestRunner;
using NUnit.Framework;
using Object = Godot.Object;

namespace Betauer.Tests {

    [TestFixture]
    public class ObjectLifeCycleManagerTests {

        private class Dummy : GodotObject, IObjectLifeCycle {
            internal int DisposedCalls = 0;
            internal bool mustBeDisposed = false;

            protected override void OnDispose(bool disposing) {
                base.OnDispose(disposing);
                DisposedCalls++;
            }

            public bool MustBeDisposed() => mustBeDisposed;
        }

        [SetUp]
        public void Setup() {
            ObjectLifeCycleManager.Singleton.DisposeAll();
            Assert.That(ObjectLifeCycleManager.Singleton.GetAll().Count, Is.EqualTo(0));
        }

        [Test]
        public void BasicTest() {
            var x1 = new Dummy();
            var x2 = new Dummy();
            var x3 = new Dummy();
            
            ObjectLifeCycleManager.Singleton.Add(x1);
            ObjectLifeCycleManager.Singleton.Add(x2);
            ObjectLifeCycleManager.Singleton.Add(x3);
            
            Assert.That(ObjectLifeCycleManager.Singleton.GetAll().Count, Is.EqualTo(3));
            Assert.That(ObjectLifeCycleManager.Singleton.DisposeAllInvalid(), Is.EqualTo(0));
            x1.mustBeDisposed = true;
            Assert.That(ObjectLifeCycleManager.Singleton.DisposeAllInvalid(), Is.EqualTo(1));
            
            Assert.That(x1.DisposedCalls, Is.EqualTo(1));
            Assert.That(x2.DisposedCalls, Is.EqualTo(0));
            Assert.That(x3.DisposedCalls, Is.EqualTo(0));

            Assert.That(Object.IsInstanceValid(x1), Is.False);
            Assert.That(Object.IsInstanceValid(x2), Is.True);
            Assert.That(Object.IsInstanceValid(x3), Is.True);

            Assert.That(ObjectLifeCycleManager.Singleton.GetAll().Count, Is.EqualTo(2));
            
            Assert.That(ObjectLifeCycleManager.Singleton.DisposeAll(), Is.EqualTo(2));
            Assert.That(x1.DisposedCalls, Is.EqualTo(1));
            Assert.That(x2.DisposedCalls, Is.EqualTo(1));
            Assert.That(x3.DisposedCalls, Is.EqualTo(1));
        }

        [Test]
        public async Task LockTest() {

            var stop = false;
            Exception errorAdd = null;
            Task taskA = new Task(async () => {
                try {
                    for (int i = 0; i < 1000; i++) {
                        var o = new Dummy();
                        ObjectLifeCycleManager.Singleton.Add(o);
                        await Task.Delay(1);
                        if (i % 2 == 0) {
                            o.mustBeDisposed = true;
                        }
                    }
                } catch (Exception e) {
                    errorAdd = e;
                }
                stop = true;
            });
            Exception errorClean = null;
            Task taskB = new Task(async () => {
                try {
                    while (!stop) {
                        ObjectLifeCycleManager.Singleton.GetAll();
                        ObjectLifeCycleManager.Singleton.DisposeAllInvalid();
                    }
                } catch (Exception e) {
                    errorClean = e;
                }
            });
            if (errorClean != null) throw errorClean;
            taskB.Start();
            taskA.Start();
            await Task.WhenAll(taskA, taskB);
            Assert.That(ObjectLifeCycleManager.Singleton.GetAll().Count, Is.EqualTo(500));
        }


    }
}