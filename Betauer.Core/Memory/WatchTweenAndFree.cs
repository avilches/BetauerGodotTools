using System.Linq;
using Godot;

namespace Betauer.Memory {
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
            var watching = string.Join(",", Watching.Select(o => o.ToStringSafe()));
            var targets = string.Join(",", Targets.Select(o => o.ToStringSafe()));
            return $"Watching: {watching} | Freeing: {targets}";
        }
    }
}