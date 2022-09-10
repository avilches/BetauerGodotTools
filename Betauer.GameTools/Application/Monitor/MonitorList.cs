using System.Collections.Generic;
using Godot;

namespace Betauer.Application.Monitor {
    public class MonitorList {
        public readonly List<IMonitor> Monitors = new List<IMonitor>();

        public IMonitor Add(IMonitor monitor) {
            Monitors.Add(monitor);
            return monitor;
        }

        public Monitor Create() {
            var monitor = new Monitor();
            Monitors.Add(monitor);
            return monitor;
        }

        public IEnumerable<string> GetText() {
            var texts = new List<string>(Monitors.Count);
            Monitors.RemoveAll(monitor => {
                if (monitor.IsDestroyed) return true;
                if (monitor.IsEnabled) texts.Add(monitor.GetText());
                return false;
            });
            return texts;
        }
    }
}