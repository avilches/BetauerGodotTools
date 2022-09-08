using System.Linq;
using Godot;

namespace Betauer.Memory {
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
            var watching = Watching != null ? Watching.Length == 1 ? Watching[0].ToStringSafe() : string.Join(",", Watching.Select(o => o.ToStringSafe())) : "null";
            var targets = Targets != null ? Targets.Length == 1 ? Targets[0].ToStringSafe() : string.Join(",", Targets.Select(o => o.ToStringSafe())) : "null";
            return $"Watching: {watching} | Freeing: {targets}";
        }
    }
}