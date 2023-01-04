using System.Collections.Generic;
using System.Linq;
using Godot;

namespace Betauer.Core.Restorer {
    public class MultiRestorer : Restorer {
        public readonly List<Restorer> Restorers = new();

        public MultiRestorer Add(Restorer restorer) {
            Restorers.Add(restorer);
            return this;
        }

        public MultiRestorer Add(IEnumerable<Restorer> toList) {
            Restorers.AddRange(toList);
            return this;
        }

        protected override void DoSave() {
            foreach (var restorer in Restorers) restorer.Save();
        }

        protected override void DoRestore() {
            foreach (var restorer in Restorers) restorer.Restore();
        }
    }
}