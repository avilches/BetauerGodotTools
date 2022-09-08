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
            var watching = Watching != null ? Watching.Length == 1 ? Watching[0].ToStringSafe() : string.Join(",", Watching.Select(o => o.ToStringSafe())) : "null";
            var targets = Targets != null ? Targets.Length == 1 ? Targets[0].ToStringSafe() : string.Join(",", Targets.Select(o => o.ToStringSafe())) : "null";
            return $"Watching: {watching} | Freeing: {targets}";
        }
    }
}