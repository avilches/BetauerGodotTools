using System;
using System.Threading.Tasks;
using Godot;
using Object = Godot.Object;

namespace Betauer.Animation {

    public interface ITweenPlayer<out TBuilder> {
        public Tween Tween { get; }
        public TBuilder CreateNewTween(Node node);
        public TBuilder WithTween(Tween tween);
        public TBuilder RemoveTween();
        public bool IsRunning();
        public TBuilder Start();
        public TBuilder Stop();
        public TBuilder Reset();
        public TBuilder Kill();

    }
    public abstract class RepeatablePlayer<TBuilder> : ITweenPlayer<TBuilder> where TBuilder : class {
        private static readonly Logger Logger = LoggerFactory.GetLogger(typeof(RepeatablePlayer<>));

        protected bool Started = false;
        protected bool Running = false;
        protected bool FreeTweenOnFinish = false;
        protected SimpleLinkedList<Action> OnFinishAll;

        public Tween Tween { get; private set; }

        public RepeatablePlayer() {
        }

        public RepeatablePlayer(Tween tween, bool freeOnFinish = false) {
            WithTween(tween, freeOnFinish);
        }

        public TBuilder CreateNewTween(Node node) {
            RemoveTween();
            var tween = new Tween();
            node.AddChild(tween);
            FreeTweenOnFinish = true;
            return WithTween(tween);
        }

        public TBuilder WithTween(Tween tween) {
            return WithTween(tween, false);
        }

        public TBuilder WithTween(Tween tween, bool freeOnFinish) {
            RemoveTween();
            Tween = tween;
            FreeTweenOnFinish = freeOnFinish;
            // _tween.Connect("tween_all_completed", this, nameof(OnTweenAllCompletedSignaled));
            // _tween.Connect("tween_started", this, nameof(TweenStarted));
            // _tween.Connect("tween_completed", this, nameof(TweenCompleted));
            return this as TBuilder;
        }

        public TBuilder RemoveTween() {
            if (Tween != null) {
                Running = false;
                Reset();
                Tween = null;
            }
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

        public TBuilder SetFreeTweenOnFinish(bool freeTweenOnFinish) {
            FreeTweenOnFinish = freeTweenOnFinish;
            return this as TBuilder;
        }

        public bool IsRunning() => Running;

        /*
         * Flow:
         * 1 Start <-> Stop
         * 2 Reset keeps the current status. So, Start -> Reset will continue playing (but from beginning)
         */

        public TBuilder Start() {
            if (!Object.IsInstanceValid(Tween)) {
                Logger.Warning("Can't Start with a freed Tween instance");
                return this as TBuilder;
            }
            if (!Started) {
                Started = true;
                Running = true;
                OnStart();
                Logger.Info("Tween.Start()");
                Tween.Start();
            } else {
                if (!Running) {
                    Logger.Info("Tween.ResumeAll()");
                    Tween.ResumeAll();
                    Running = true;
                }
            }
            return this as TBuilder;
        }

        public TBuilder Stop() {
            if (!Object.IsInstanceValid(Tween)) {
                Logger.Warning("Can't Stop with a freed Tween instance");
                return this as TBuilder;
            }
            if (Running) {
                Logger.Info("Tween.StopAll()");
                Tween.StopAll();
                Running = false;
            }
            return this as TBuilder;
        }

        public TBuilder Reset() {
            if (!Object.IsInstanceValid(Tween)) {
                Logger.Warning("Can't Reset with a freed Tween instance");
                return this as TBuilder;
            }
            Logger.Info("Tween.StopAll()");
            Tween.StopAll();
            Logger.Info("Tween.RemoveAll()");
            Tween.RemoveAll();
            OnReset();
            if (Running) {
                OnStart();
                Logger.Info("Tween.Start()");
                Tween.Start();
            } else {
                Started = false;
            }
            return this as TBuilder;
        }

        public TBuilder Kill() {
            if (!Object.IsInstanceValid(Tween)) return this as TBuilder;
            if (Running) {
                Stop();
            }
            Reset();
            Logger.Info("Tween.QueueFree()");
            Tween.QueueFree();
            return this as TBuilder;
        }

        protected abstract void OnStart();
        protected abstract void OnReset();

        protected void Finished() {
            Logger.Debug($"Finished. End: stop & reset. Kill {FreeTweenOnFinish}");
            // Reset keeps the state, so Reset() will play again the sequence, meaning it will never finish
            Stop();
            // End of ALL THE LOOPS of all the sequences of the player
            OnFinishAll?.ForEach(callback => callback.Invoke());
            // EmitSignal(nameof(finished));
            if (FreeTweenOnFinish) Kill();
        }

        public Task<TBuilder> Await() {
            var promise = new TaskCompletionSource<TBuilder>();
            AddOnFinishAll(() => promise.TrySetResult(this as TBuilder));
            return promise.Task;
        }
    }
}