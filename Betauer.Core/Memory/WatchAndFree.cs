using System.Linq;
using Godot;

namespace Betauer.Memory {
    /// <summary>
    /// If any of the Watching objects is not valid, it will free (deferred or immediately) the targets 
    /// </summary>
    public abstract class FreeConsumer<TBuilder> : IObjectConsumer where TBuilder : class {
        public Object[]? Targets;
        public bool Deferred;

        protected FreeConsumer(bool deferred) {
            Deferred = deferred;
        }

        public TBuilder Free(params Object[] targets) {
            Targets = targets;
            return this as TBuilder;
        }

        public TBuilder AddToDefaultObjectWatcher() {
            DefaultObjectWatcherRunner.Instance.Add(this);
            return this as TBuilder;
        }

        public bool Consume(bool force) {
            if (Targets == null || Targets.Length == 0) return true;
            if (force) {
                FreeNow();
                OnFree(true);
                return true;
            }
            if (MustBeFreed()) {
                return false; // All watching objects are ok
            }
            // At least one watching object is not valid, free!
            if (Deferred) FreeDeferred();
            else FreeNow();
            OnFree(false);
            return true;
        }

        private void FreeNow() {
            Targets?.Where(Object.IsInstanceValid).ForEach(o => o.Free());
        }

        private void FreeDeferred() {
            foreach (var target in Targets.Where(Object.IsInstanceValid)) {
                if (target is Node node) node.QueueFree();
                else target.CallDeferred("free");
            }
        }

        public virtual void OnFree(bool force) {
        }

        public abstract bool MustBeFreed();
    }

    public class WatchObjectAndFree : FreeConsumer<WatchObjectAndFree> {
        public Object[]? Watching;

        public WatchObjectAndFree(bool deferred = true) : base(deferred) {
        }

        public WatchObjectAndFree Watch(params Object[] watching) {
            Watching = watching;
            return this;
        }

        public override bool MustBeFreed() {
            return Watching?.All(Object.IsInstanceValid) ?? false;
        }
        
        public override string ToString() {
            return "Watching: " + string.Join(",", 
                Watching?.Select(o => Object.IsInstanceValid(o) ? o.ToString(): "(disposed)"));
        }
    }

    public class WatchTweenAndFree : FreeConsumer<WatchTweenAndFree> {
        public SceneTreeTween[]? Watching;

        public WatchTweenAndFree(bool deferred = true) : base(deferred) {
        }

        public WatchTweenAndFree Watch(params SceneTreeTween[] watching) {
            Watching = watching;
            return this;
        }
        
        public override bool MustBeFreed() {
            return Watching?.All(tween => tween.IsValid()) ?? false;
        }

        public override string ToString() {
            return "Watching: " + string.Join(",", 
                Watching?.Select(o => Object.IsInstanceValid(o) ? o.ToString(): "(disposed)"));
        }
    }
}