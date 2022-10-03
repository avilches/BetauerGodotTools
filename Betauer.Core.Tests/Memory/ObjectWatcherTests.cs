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
            DefaultObjectWatcherTask.Instance.Dispose();
            Assert.That(DefaultObjectWatcherTask.Instance.Size, Is.EqualTo(0));
            DefaultObjectWatcherTask.Instance = new ObjectWatcherTask();
        }

        [Test]
        public async Task NotValidTest() {
            var origin = new Object();
            var target = new Object();
            var targetDeferred = new Object();
            Watcher.IfInvalidInstance(origin).Free(target, false);
            Watcher.IfInvalidInstance(origin).Free(targetDeferred);
            Assert.That(DefaultObjectWatcherTask.Instance.Size, Is.EqualTo(2));

            DefaultObjectWatcherTask.Instance.Consume();
            Assert.That(DefaultObjectWatcherTask.Instance.Size, Is.EqualTo(2));
            
            origin.Free();
            DefaultObjectWatcherTask.Instance.Consume();
            Assert.That(DefaultObjectWatcherTask.Instance.Size, Is.EqualTo(0));
            Assert.That(IsInstanceValid(target), Is.False);
            Assert.That(IsInstanceValid(targetDeferred), Is.True);

            await this.AwaitIdleFrame();
            Assert.That(IsInstanceValid(targetDeferred), Is.False);
        }

        [Test]
        public void WatchAndUnwatchTest() {
            Assert.That(DefaultObjectWatcherTask.Instance.Size, Is.EqualTo(0));
            
            var origin = new Object();
            var target = new Object();
            var w = Watcher.IfInvalidInstance(origin).Free(target);
            Assert.That(DefaultObjectWatcherTask.Instance.Size, Is.EqualTo(1));

            // Nothing happens
            DefaultObjectWatcherTask.Instance.Consume();
            DefaultObjectWatcherTask.Instance.Consume();
            Assert.That(DefaultObjectWatcherTask.Instance.Size, Is.EqualTo(1));

            // Add the same again is ignored
            w.StartWatching();
            w.StartWatching();
            w.StartWatching();
            Assert.That(DefaultObjectWatcherTask.Instance.Size, Is.EqualTo(1));

            // Unwatch removes the element
            w.StopWatching();
            w.StopWatching();
            w.StopWatching();
            w.StopWatching();
            Assert.That(DefaultObjectWatcherTask.Instance.Size, Is.EqualTo(0));
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
                    this.FreeDeferred();
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
                        DefaultObjectWatcherTask.Instance.Add(o);
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
                DefaultObjectWatcherTask.Instance.Add(o);
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
                        DefaultObjectWatcherTask.Instance.Consume();
                        loops++;
                    }
                    DefaultObjectWatcherTask.Instance.Consume();
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

            // An idle frame is needed to ensure CallDeferred("free") is called for all the objects
            await this.AwaitIdleFrame();
            await this.AwaitIdleFrame();

            Assert.That(dummies1.Count, Is.EqualTo(100));
            Assert.That(dummies1.Sum(d => d.DisposedCalls), Is.LessThanOrEqualTo(50));

            Assert.That(dummies2.Count, Is.EqualTo(50));
            Assert.That(dummies2.Sum(d => d.DisposedCalls), Is.LessThanOrEqualTo(25));
            
            Assert.That(dummies3.Count, Is.EqualTo(50));
            Assert.That(dummies3.Sum(d => d.DisposedCalls), Is.LessThanOrEqualTo(50));
            Assert.That(DefaultObjectWatcherTask.Instance.Size, Is.EqualTo(75));
        }
    }
}