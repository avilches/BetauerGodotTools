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
            DefaultObjectWatcherRunner.Instance.Dispose();
            Assert.That(DefaultObjectWatcherRunner.Instance.Count, Is.EqualTo(0));
            DefaultObjectWatcherRunner.Instance = new ObjectWatcherRunner();
        }

        [Test]
        public async Task NotValidTest() {
            new WatchObjectAndFree().AddToDefaultObjectWatcher();
            new WatchObjectAndFree().Free(new Object()).AddToDefaultObjectWatcher();
            new WatchObjectAndFree().Watch(new Object()).AddToDefaultObjectWatcher();
            Assert.That(DefaultObjectWatcherRunner.Instance.Count, Is.EqualTo(3));

            DefaultObjectWatcherRunner.Instance.Consume();
            Assert.That(DefaultObjectWatcherRunner.Instance.Count, Is.EqualTo(0));
        }

        [Test]
        public async Task WatchAndUnwatchTest() {
            Assert.That(DefaultObjectWatcherRunner.Instance.Count, Is.EqualTo(0));
            
            var watch = new WatchObjectAndFree().Watch(new Object()).Free(new Object()).AddToDefaultObjectWatcher();
            Assert.That(DefaultObjectWatcherRunner.Instance.Count, Is.EqualTo(1));

            // Nothing happens
            DefaultObjectWatcherRunner.Instance.Consume();
            DefaultObjectWatcherRunner.Instance.Consume();
            Assert.That(DefaultObjectWatcherRunner.Instance.Count, Is.EqualTo(1));

            // Add the same again is ok
            DefaultObjectWatcherRunner.Instance.Add(watch);
            Assert.That(DefaultObjectWatcherRunner.Instance.Count, Is.EqualTo(2));

            // Unwatch removes the element
            DefaultObjectWatcherRunner.Instance.Remove(watch);
            Assert.That(DefaultObjectWatcherRunner.Instance.Count, Is.EqualTo(0));

            // Unwatch a non-existent elements does nothing
            DefaultObjectWatcherRunner.Instance.Remove(watch);
            Assert.That(DefaultObjectWatcherRunner.Instance.Count, Is.EqualTo(0));
        }

        [Test]
        public async Task FreeObjectsNoDeferredTest() {
            var freed1 = new WatchObjectAndFree(false)
                .Watch(new Object(), new Object())
                .Free(new Node(), new Object())
                .AddToDefaultObjectWatcher();

            Assert.That(DefaultObjectWatcherRunner.Instance.Count, Is.EqualTo(1));
            DefaultObjectWatcherRunner.Instance.Consume();
            Assert.That(DefaultObjectWatcherRunner.Instance.Count, Is.EqualTo(1));

            // Freed or disposed objects are removed from watching list
            freed1.Watching[0].Free();
            Assert.That(IsInstanceValid(freed1.Targets[0]), Is.True);
            Assert.That(IsInstanceValid(freed1.Targets[1]), Is.True);
            
            DefaultObjectWatcherRunner.Instance.Consume();
            Assert.That(DefaultObjectWatcherRunner.Instance.Count, Is.EqualTo(0));
            Assert.That(IsInstanceValid(freed1.Targets[0]), Is.False);
            Assert.That(IsInstanceValid(freed1.Targets[1]), Is.False);
        }

        [Test]
        public async Task FreeObjectsDefaultDeferredTest() {
            var freed1 = new WatchObjectAndFree()
                .Watch(new Object(), new Object())
                .Free(new Node(), new Object())
                .AddToDefaultObjectWatcher();
            
            Assert.That(DefaultObjectWatcherRunner.Instance.Count, Is.EqualTo(1));
            DefaultObjectWatcherRunner.Instance.Consume();
            Assert.That(DefaultObjectWatcherRunner.Instance.Count, Is.EqualTo(1));


            // Freed or disposed objects are removed from watching list
            freed1.Watching[1].Free();
            Assert.That(IsInstanceValid(freed1.Targets[0]), Is.True);
            Assert.That(((Node)freed1.Targets[0]).IsQueuedForDeletion(), Is.False);
            Assert.That(IsInstanceValid(freed1.Targets[1]), Is.True);

            DefaultObjectWatcherRunner.Instance.Consume();
            Assert.That(DefaultObjectWatcherRunner.Instance.Count, Is.EqualTo(0));

            // Node is queued for deletion
            Assert.That(IsInstanceValid(freed1.Targets[0]), Is.True);
            Assert.That(((Node)freed1.Targets[0]).IsQueuedForDeletion(), Is.True);
            Assert.That(IsInstanceValid(freed1.Targets[1]), Is.True);

            // Objects are deleted using CallDeferred("free") or QueueFree() so a new frame is needed
            await this.AwaitIdleFrame();
            Assert.That(IsInstanceValid(freed1.Targets[0]), Is.False);
            Assert.That(IsInstanceValid(freed1.Targets[1]), Is.False);
        }

        private class Dummy : DisposableGodotObject, IObjectConsumer {
            internal int DisposedCalls = 0;
            internal bool MustBeFreed = false;

            protected override void OnDispose(bool disposing) {
                base.OnDispose(disposing);
                DisposedCalls++;
            }

            public bool Consume(bool force) {
                if (force || MustBeFreed) {
                    CallDeferred("free");
                }
                return force || MustBeFreed;
            }
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
                        DefaultObjectWatcherRunner.Instance.Add(o);
                        await Task.Delay(5);
                        if (i % 2 == 0) {
                            o.MustBeFreed = true;
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
                DefaultObjectWatcherRunner.Instance.Add(o);
            }
            Task taskA2 = new Task(async () => {
                try {
                    for (var i = 0; i < 25; i++) {
                        dummies2[i * 2].MustBeFreed = true;
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
                        DefaultObjectWatcherRunner.Instance.Consume();
                        loops++;
                    }
                    DefaultObjectWatcherRunner.Instance.Consume();
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
            await this.AwaitIdleFrame();

            Assert.That(dummies1.Count, Is.EqualTo(100));
            Assert.That(dummies1.Sum(d => d.DisposedCalls), Is.LessThanOrEqualTo(50));

            Assert.That(dummies2.Count, Is.EqualTo(50));
            Assert.That(dummies2.Sum(d => d.DisposedCalls), Is.LessThanOrEqualTo(25));
            
            Assert.That(dummies3.Count, Is.EqualTo(50));
            Assert.That(dummies3.Sum(d => d.DisposedCalls), Is.LessThanOrEqualTo(50));
            Assert.That(DefaultObjectWatcherRunner.Instance.Count, Is.EqualTo(75));
        }
    }
}