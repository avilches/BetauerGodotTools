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
            var watching = string.Join(",", Watching.Select(o => o.ToStringSafe()));
            var targets = string.Join(",", Targets.Select(o => o.ToStringSafe()));
            return $"Watching: {watching} | Freeing: {targets}";
        }
    }
}