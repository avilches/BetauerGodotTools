using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;
using Betauer.Memory;
using Godot;
using Object = Godot.Object;

namespace Betauer.Tests.Experimental {

    public class GodotObjectOnDemandFreer : OnDemandConsumer<Object> {
        private static readonly Logger Logger = LoggerFactory.GetLogger(typeof(GodotObjectOnDemandFreer));

        public override void Consume(Object obj) {
            Logger.Info($"Consume. {obj.GetType()}.Free() {obj}");
            obj.Free();
        }
    }

    public class GodotObjectOnDemandDisposer : OnDemandConsumer<Object> {
        private static readonly Logger Logger = LoggerFactory.GetLogger(typeof(GodotObjectOnDemandDisposer));

        public override void Consume(Object obj) {
            Logger.Info($"Consume. {obj.GetType()}.Dispose() {obj}");
            obj.Dispose();
        }
    }

    public class GodotObjectImmediateConsumerTaskFreer : ImmediateConsumerTask<Object> {
        private static readonly Logger Logger = LoggerFactory.GetLogger(typeof(GodotObjectImmediateConsumerTaskFreer));

        public override void Consume(Object obj) {
            Logger.Info($"Consume. {obj.GetType()}.Free() {obj}");
            obj.Free();
        }
    }

    public class GodotObjectImmediateConsumerTaskDisposer : ImmediateConsumerTask<IDisposable> {
        private static readonly Logger Logger = LoggerFactory.GetLogger(typeof(GodotObjectImmediateConsumerTaskDisposer));

        public override void Consume(IDisposable obj) {
            Logger.Info($"Consume. {obj.GetType()}.Dispose() {obj}");
            obj.Dispose();
        }
    }

    public interface IConsumer<in T> : IDisposable {
        public void Add(T o);
        public void Consume(T obj);
    }

    public abstract class ImmediateConsumerTask<T> : DisposableObject, IConsumer<T> {
        private static readonly Logger Logger = LoggerFactory.GetLogger(typeof(ImmediateConsumerTask<T>));
        private readonly BlockingCollection<T> _cq = new BlockingCollection<T>();
        public int ConsumedObjects { get; private set; } = 0;

        public Task Start() {
            return Task.Run(Execute);
        }

        private async Task Execute() {
            if (Disposed) return;
            Logger.Info("Run: start");
            try {
                while (!_cq.IsCompleted) {
                    var obj = _cq.Take(); // wait until a new object is taken
                    Consume(obj);
                    ConsumedObjects++;
                }
                GD.Print("+++");
            } catch (InvalidOperationException) {
                // Take throws this exception when the BlockingCollection is empty and marked as completed by CompleteAdding()
            } catch (ThreadAbortException) {
                // Take() throws this exception when the BlockingCollection thread is aborted by an app shutdown
                // Call to Dispose() on shutdown to avoid it
            } catch (Exception e) {
                Console.WriteLine(e);
                throw;
            }
            Logger.Info("Run: end");
        }

        public abstract void Consume(T obj);

        public void Add(T o) {
            if (!Disposed) _cq.Add(o);
        }

        protected override void OnDispose(bool disposing) {
            Logger.Info("Disposing...");
            _cq.CompleteAdding();
        }
    }

    public abstract class OnDemandConsumer<T> : DisposableObject, IConsumer<T> {
        private static readonly Logger Logger = LoggerFactory.GetLogger(typeof(OnDemandConsumer<T>));
        private readonly BlockingCollection<T> _cq = new BlockingCollection<T>();
        public int ConsumedObjects { get; private set; } = 0;

        public void ConsumeAll() {
            if (Disposed) return;
            IsBusy = true;
            var consumedObjects = 0;
            Logger.Info("ConsumeAll: start");
            try {
                while (!_cq.IsCompleted && _cq.TryTake(out var obj)) {
                    Consume(obj);
                    ConsumedObjects++;
                    consumedObjects++;
                }
            } catch (InvalidOperationException) {
                // Take throws this exception when the collections is empty and marked as completed by CompleteAdding()
            } catch (Exception e) {
                Console.WriteLine(e);
                throw;
            }
            IsBusy = false;
            Logger.Info("ConsumeAll: end. " + consumedObjects + " consumed objects in this call");
        }

        public bool IsBusy { get; private set; }

        public abstract void Consume(T obj);

        public void Add(T o) {
            if (!Disposed) _cq.Add(o);
        }

        protected override void OnDispose(bool disposing) {
            Logger.Info("Disposing...");
            _cq.CompleteAdding();
        }
    }
    
}