using System;
using System.Threading.Tasks;
using Godot;

namespace Betauer.Animation {
    public interface ITweenPlayer<out TBuilder> {
        public bool IsRunning();
        public TBuilder Play();
        public TBuilder Stop();
        public TBuilder Reset();
    }


    public abstract class RepeatablePlayer<TBuilder> : ActionTween,
        ITweenPlayer<TBuilder> where TBuilder : class {
        private static readonly Logger Logger = LoggerFactory.GetLogger(typeof(RepeatablePlayer<>));

        protected bool Started = false;
        protected bool Running = false;
        protected bool DisposeOnFinish = false;
        protected SimpleLinkedList<Action>? OnFinishAll;
        protected bool Disposed = false;

        public RepeatablePlayer() {
        }

        public TBuilder WithParent(Node parent, bool disposeOnFinish = true) {
            DisposeOnFinish = disposeOnFinish;
            parent.AddChild(this);
            return this as TBuilder;
        }

        public TBuilder AddOnFinishAll(Action onFinishAll) {
            if (OnFinishAll == null) {
                OnFinishAll = new SimpleLinkedList<Action>
                    { onFinishAll };
            } else {
                OnFinishAll.Add(onFinishAll);
            }
            return this as TBuilder;
        }

        public bool IsRunning() => Running;

        /*
         * Flow:
         * 1 Start <-> Stop
         * 2 Reset keeps the current status. So, Start -> Reset will continue playing (but from beginning)
         */

        public TBuilder Play() {
            if (Disposed) return this as TBuilder;
            if (!Started) {
                DoStart();
            } else {
                if (!Running) {
                    Logger.Info("Tween.ResumeAll()");
                    base.ResumeAll();
                    Running = true;
                }
            }
            return this as TBuilder;
        }

        public TBuilder Stop() {
            if (Disposed) return this as TBuilder;
            if (Running) {
                Logger.Info("Tween.StopAll()");
                base.StopAll();
                Running = false;
            }
            return this as TBuilder;
        }

        public TBuilder Reset() {
            if (Disposed) return this as TBuilder;
            Logger.Info("Tween.RemoveAll()");
            base.RemoveAll();
            OnReset();
            if (Running) {
                DoStart();
            } else {
                Started = false;
            }
            return this as TBuilder;
        }


        protected abstract void OnStart();
        protected abstract void OnReset();

        private TaskCompletionSource<TBuilder>? _promise;

        private void DoStart() {
            if (_promise != null && !_promise.Task.IsCompleted) {
                // This can't happen, but just in case...
                _promise.SetCanceled();
            }
            _promise = new TaskCompletionSource<TBuilder>();
            Started = true;
            Running = true;
            OnStart();
            Logger.Info("Tween.Start()");
            base.Start();
        }

        protected void Finished() {
            Logger.Debug($"Finished. End: stop & reset. Kill {DisposeOnFinish}");
            Running = false; // this will avoid to start again in the next Reset() call
            Reset(); // it also set Started = false
            // End of ALL THE LOOPS of all the sequences of the player
            if (OnFinishAll != null)
                foreach (var callback in OnFinishAll)
                    callback.Invoke();
            if (DisposeOnFinish) {
                Disposed = true;
                QueueFree();
            }
            _promise!.TrySetResult(this as TBuilder);
            // EmitSignal(nameof(finished));
        }

        protected override void Dispose(bool disposing) {
            base.Dispose(disposing);
            Disposed = true;
        }

        public Task<TBuilder> Await() {
            return _promise!.Task;
        }
    }
}