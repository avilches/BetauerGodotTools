using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Betauer.Memory;
using Betauer.TestRunner;
using NUnit.Framework;
using Object = Godot.Object;

namespace Betauer.Tests.Memory {

    [TestFixture]
    public class ObjectLifeCycleManagerTests {
        [SetUp]
        public void Setup() {
            ObjectLifeCycleManager.Singleton.Reset();
        }

        private class Dummy : GodotObject, IObjectLifeCycle {
            internal int DisposedCalls = 0;
            internal bool mustBeDisposed = false;

            protected override void OnDispose(bool disposing) {
                base.OnDispose(disposing);
                DisposedCalls++;
            }

            public bool MustBeDisposed() => mustBeDisposed;
        }

        [Test]
        public void BasicTest() {
            var x1 = new Dummy();
            var x2 = new Dummy();
            var x3 = new Dummy();
            
            ObjectLifeCycleManager.Singleton.Watch(x1);
            ObjectLifeCycleManager.Singleton.Watch(x2);
            ObjectLifeCycleManager.Singleton.Watch(x3);

            // Nothing happens when no invalid data
            Assert.That(ObjectLifeCycleManager.Singleton.GetWatching().Count, Is.EqualTo(3));
            Assert.That(ObjectLifeCycleManager.Singleton.GetQueue().Count, Is.EqualTo(0));
            Assert.That(ObjectLifeCycleManager.Singleton.ProcessWatching(), Is.EqualTo(0));
            Assert.That(ObjectLifeCycleManager.Singleton.ProcessQueue(), Is.EqualTo(0));
            x1.mustBeDisposed = true;
            Assert.That(ObjectLifeCycleManager.Singleton.GetWatching().Count, Is.EqualTo(3));
            Assert.That(ObjectLifeCycleManager.Singleton.GetQueue().Count, Is.EqualTo(0));

            // ProcessWatching detects the invalid data 
            Assert.That(ObjectLifeCycleManager.Singleton.ProcessWatching(), Is.EqualTo(1));
            Assert.That(ObjectLifeCycleManager.Singleton.GetQueue().Count, Is.EqualTo(1));
            Assert.That(ObjectLifeCycleManager.Singleton.GetWatching().Count, Is.EqualTo(2));
            // Repeat ProcessWatching does nothing
            Assert.That(ObjectLifeCycleManager.Singleton.ProcessWatching(), Is.EqualTo(0));
            Assert.That(ObjectLifeCycleManager.Singleton.GetQueue().Count, Is.EqualTo(1));
            Assert.That(ObjectLifeCycleManager.Singleton.GetWatching().Count, Is.EqualTo(2));
            
            // ProcessWatching just enqueue, it doesn't free
            Assert.That(x1.DisposedCalls, Is.EqualTo(0));
            Assert.That(x2.DisposedCalls, Is.EqualTo(0));
            Assert.That(x3.DisposedCalls, Is.EqualTo(0));
            Assert.That(Object.IsInstanceValid(x1), Is.True);
            Assert.That(Object.IsInstanceValid(x2), Is.True);
            Assert.That(Object.IsInstanceValid(x3), Is.True);

            // ProcessQueue
            // 1) move the current queue to internal queue
            // 2) process internal queue, which is empty
            Assert.That(ObjectLifeCycleManager.Singleton.ProcessQueue(), Is.EqualTo(0));
            Assert.That(ObjectLifeCycleManager.Singleton.GetQueue().Count, Is.EqualTo(0));
            Assert.That(ObjectLifeCycleManager.Singleton.GetWatching().Count, Is.EqualTo(2));
            Assert.That(x1.DisposedCalls, Is.EqualTo(0));
            Assert.That(x2.DisposedCalls, Is.EqualTo(0));
            Assert.That(x3.DisposedCalls, Is.EqualTo(0));
            Assert.That(Object.IsInstanceValid(x1), Is.True);
            Assert.That(Object.IsInstanceValid(x2), Is.True);
            Assert.That(Object.IsInstanceValid(x3), Is.True);

            // ProcessQueue
            // 1) move the current queue to internal queue
            // 2) process internal queue with 1 element
            Assert.That(ObjectLifeCycleManager.Singleton.ProcessQueue(), Is.EqualTo(1));
            Assert.That(ObjectLifeCycleManager.Singleton.GetQueue().Count, Is.EqualTo(0));
            Assert.That(ObjectLifeCycleManager.Singleton.GetWatching().Count, Is.EqualTo(2));

            Assert.That(x1.DisposedCalls, Is.EqualTo(1));
            Assert.That(x2.DisposedCalls, Is.EqualTo(0));
            Assert.That(x3.DisposedCalls, Is.EqualTo(0));
            Assert.That(Object.IsInstanceValid(x1), Is.False);
            Assert.That(Object.IsInstanceValid(x2), Is.True);
            Assert.That(Object.IsInstanceValid(x3), Is.True);

            Assert.That(ObjectLifeCycleManager.Singleton.DisposeAllWatching(), Is.EqualTo(2));
            Assert.That(ObjectLifeCycleManager.Singleton.GetQueue().Count, Is.EqualTo(2));
            Assert.That(ObjectLifeCycleManager.Singleton.GetWatching().Count, Is.EqualTo(0));
            
            Assert.That(x1.DisposedCalls, Is.EqualTo(1));
            Assert.That(x2.DisposedCalls, Is.EqualTo(0));
            Assert.That(x3.DisposedCalls, Is.EqualTo(0));
            Assert.That(Object.IsInstanceValid(x1), Is.False);
            Assert.That(Object.IsInstanceValid(x2), Is.True);
            Assert.That(Object.IsInstanceValid(x3), Is.True);
            
            Assert.That(ObjectLifeCycleManager.Singleton.ProcessQueue(), Is.EqualTo(0));
            Assert.That(ObjectLifeCycleManager.Singleton.ProcessQueue(), Is.EqualTo(2));
            
            Assert.That(x1.DisposedCalls, Is.EqualTo(1));
            Assert.That(x2.DisposedCalls, Is.EqualTo(1));
            Assert.That(x3.DisposedCalls, Is.EqualTo(1));
            Assert.That(Object.IsInstanceValid(x1), Is.False);
            Assert.That(Object.IsInstanceValid(x2), Is.False);
            Assert.That(Object.IsInstanceValid(x3), Is.False);
        }

        [Test]
        public async Task LockTest() {

            var stop1 = false;
            var stop2 = false;
            Exception errorAdd = null;
            var dummies1 = new List<Dummy>();
            Task taskA1 = new Task(async () => {
                try {
                    for (int i = 0; i < 100; i++) {
                        var o = new Dummy();
                        dummies1.Add(o);
                        ObjectLifeCycleManager.Singleton.Watch(o);
                        await Task.Delay(1);
                        if (i % 2 == 0) {
                            o.mustBeDisposed = true;
                        }
                    }
                } catch (Exception e) {
                    errorAdd = e;
                }
                stop1 = true;
            });
            var dummies2 = new List<Dummy>();
            for (int i = 0; i < 10; i++) {
                var o = new Dummy();
                dummies2.Add(o);
                ObjectLifeCycleManager.Singleton.Watch(o);
            }
            Task taskA2 = new Task(async () => {
                try {
                    for (int i = 0; i < 50; i++) {
                        dummies2[i * 2].mustBeDisposed = true;
                        await Task.Delay(10);
                    }
                } catch (Exception e) {
                    errorAdd = e;
                }
                stop2 = true;
            });
            Exception errorClean = null;
            Task taskB = new Task(() => {
                try {
                    var loops = 0;
                    while (!stop1 || !stop2) {
                        ObjectLifeCycleManager.Singleton.GetWatching();
                        ObjectLifeCycleManager.Singleton.ProcessWatching();
                        ObjectLifeCycleManager.Singleton.ProcessQueue();
                        loops++;
                    } 
                    ObjectLifeCycleManager.Singleton.ProcessWatching();
                    ObjectLifeCycleManager.Singleton.ProcessQueue();
                    Console.WriteLine("Processed queue times: "+loops);
                } catch (Exception e) {
                    errorClean = e;
                }
            });
            if (errorClean != null) throw errorClean;
            if (errorAdd != null) throw errorAdd;
            taskB.Start();
            taskA1.Start();
            taskA2.Start();
            await Task.WhenAll(taskA1, taskA2, taskB);
            Assert.That(dummies1.Sum(d => d.DisposedCalls), Is.EqualTo(50));
            Assert.That(dummies2.Sum(d => d.DisposedCalls), Is.EqualTo(5));
            Assert.That(ObjectLifeCycleManager.Singleton.GetWatching().Count, Is.EqualTo(55));
        }


    }
}