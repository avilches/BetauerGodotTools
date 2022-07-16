using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Betauer.Memory;
using Betauer.TestRunner;
using Godot;
using NUnit.Framework;
using Object = Godot.Object;

namespace Betauer.Tests.Memory {
    [TestFixture]
    public class ObjectWatcherTests : Node {
        [SetUp]
        public void Setup() {
            DefaultObjectWatcher.Instance.Dispose();
        }

        private class Dummy : DisposableGodotObject, IObjectWatched {
            internal int DisposedCalls = 0;
            internal bool mustBeFreed = false;

            protected override void OnDispose(bool disposing) {
                base.OnDispose(disposing);
                DisposedCalls++;
            }

            public Object Object => this;

            public bool MustBeFreed() => mustBeFreed;
        }

        [Test]
        public async Task Node() {
            AddChild(new ObjectWatcherNode(1));

            var w1 = new Dummy();
            var w2d = new Dummy();
            var w3 = new Dummy();
            var w4d = new Dummy();
            var freed1 = new Dummy();
            var disposed1 = new Dummy();
            var unwatch = new Dummy();

            DefaultObjectWatcher.Instance.Watch(w1);
            DefaultObjectWatcher.Instance.Watch(w2d);
            DefaultObjectWatcher.Instance.Watch(w3);
            DefaultObjectWatcher.Instance.Watch(w4d);
            DefaultObjectWatcher.Instance.Watch(unwatch);
            DefaultObjectWatcher.Instance.Watch(freed1);
            DefaultObjectWatcher.Instance.Watch(disposed1);
            Assert.That(DefaultObjectWatcher.Instance.WatchingCount, Is.EqualTo(7));

            // Nothing happens
            DefaultObjectWatcher.Instance.Process();
            Assert.That(DefaultObjectWatcher.Instance.WatchingCount, Is.EqualTo(7));

            // Unwatch removes the element
            DefaultObjectWatcher.Instance.Unwatch(unwatch);
            Assert.That(DefaultObjectWatcher.Instance.WatchingCount, Is.EqualTo(6));

            // Unwatch a non-existent elements does nothing
            DefaultObjectWatcher.Instance.Unwatch(unwatch);
            Assert.That(DefaultObjectWatcher.Instance.WatchingCount, Is.EqualTo(6));

            // Freed or disposed objects are removed from watching list
            freed1.Free();
            disposed1.Dispose();
            Assert.That(IsInstanceValid(freed1), Is.False);
            Assert.That(IsInstanceValid(disposed1), Is.False);
            DefaultObjectWatcher.Instance.Process();
            Assert.That(DefaultObjectWatcher.Instance.WatchingCount, Is.EqualTo(4));
            
            // objects with mustBeFreed = true will be removed and disposed
            w2d.mustBeFreed = true;
            w4d.mustBeFreed = true;
            DefaultObjectWatcher.Instance.Process();
            Assert.That(DefaultObjectWatcher.Instance.WatchingCount, Is.EqualTo(2));

            Assert.That(IsInstanceValid(w1), Is.True);
            Assert.That(IsInstanceValid(w2d), Is.True);
            Assert.That(IsInstanceValid(w3), Is.True);
            Assert.That(IsInstanceValid(w4d), Is.True);
            
            // Objects are deleted using CallDeferred("free") so a new frame is needed
            await this.AwaitIdleFrame();
            Assert.That(IsInstanceValid(w1), Is.True);
            Assert.That(IsInstanceValid(w2d), Is.False);
            Assert.That(IsInstanceValid(w3), Is.True);
            Assert.That(IsInstanceValid(w4d), Is.False);
            
            // the unwatched remains valid after all of course...
            Assert.That(IsInstanceValid(unwatch), Is.True);
        }

        [Test]
        public async Task LockTest() {
            Exception errorAdd1 = null;
            var dummies1 = new List<Dummy>();
            var stop1 = false;
            // Add 100 objects to watch every 5ms = 500ms (50 of them will be marked mustBeFreed)
            Task taskA1 = new Task(async () => {
                try {
                    for (var i = 0; i < 100; i++) {
                        var o = new Dummy();
                        dummies1.Add(o);
                        DefaultObjectWatcher.Instance.Watch(o);
                        await Task.Delay(5);
                        if (i % 2 == 0) {
                            o.mustBeFreed = true;
                        }
                    }
                } catch (Exception e) {
                    errorAdd1 = e;
                }
                stop1 = true;
            });

            // Add 50 objects to watch, 25 of them will be marked mustBeFreed every 20ms = 500ms
            Exception errorAdd2 = null;
            var dummies2 = new List<Dummy>();
            var stop2 = false;
            for (var i = 0; i < 50; i++) {
                var o = new Dummy();
                dummies2.Add(o);
                DefaultObjectWatcher.Instance.Watch(o);
            }
            Task taskA2 = new Task(async () => {
                try {
                    for (var i = 0; i < 25; i++) {
                        dummies2[i * 2].mustBeFreed = true;
                        await Task.Delay(20);
                    }
                } catch (Exception e) {
                    errorAdd2 = e;
                }
                stop2 = true;
            });

            // Add 50 objects to be freed every 10ms = 500ms
            Exception errorAdd3 = null;
            var dummies3 = new List<Dummy>();
            var stop3 = false;
            Task taskA3 = new Task(async () => {
                try {
                    for (int i = 0; i < 50; i++) {
                        var o = new Dummy();
                        dummies3.Add(o);
                        o.Free();
                        await Task.Delay(10);
                    }
                } catch (Exception e) {
                    errorAdd3 = e;
                }
                stop3 = true;
            });

            // Process
            Exception errorProcess = null;
            Task taskB = new Task(() => {
                try {
                    var loops = 0;
                    while (!stop1 || !stop2 || !stop3) {
                        DefaultObjectWatcher.Instance.Process();
                        loops++;
                    }
                    DefaultObjectWatcher.Instance.Process();
                    Console.WriteLine("Processed queue times: " + loops);
                } catch (Exception e) {
                    errorProcess = e;
                }
            });
            taskB.Start();
            taskA1.Start();
            taskA2.Start();
            taskA3.Start();
            await Task.WhenAll(taskA1, taskA2, taskA3, taskB);
            if (errorProcess != null) throw errorProcess;
            if (errorAdd1 != null) throw errorAdd1;
            if (errorAdd2 != null) throw errorAdd2;
            if (errorAdd3 != null) throw errorAdd3;

            // An idle frame is needed to ensure CallDeferred("queue") is called for all the objects
            await this.AwaitIdleFrame();

            Assert.That(dummies1.Count, Is.EqualTo(100));
            Assert.That(dummies1.Sum(d => d.DisposedCalls), Is.EqualTo(50));

            Assert.That(dummies2.Count, Is.EqualTo(50));
            Assert.That(dummies2.Sum(d => d.DisposedCalls), Is.EqualTo(25));
            
            Assert.That(dummies3.Count, Is.EqualTo(50));
            Assert.That(dummies3.Sum(d => d.DisposedCalls), Is.EqualTo(50));
            Assert.That(DefaultObjectWatcher.Instance.WatchingCount, Is.EqualTo(75));
        }
    }
}