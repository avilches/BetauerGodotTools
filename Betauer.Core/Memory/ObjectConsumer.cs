using System.Collections.Generic;

namespace Betauer.Memory {
    public interface IObjectConsumer {
        public bool Consume(bool force);
    }

    public class Consumer {
        private readonly HashSet<IObjectConsumer> _objects = new HashSet<IObjectConsumer>();

        public int Count => _objects.Count;

        public List<IObjectConsumer> ToList() {
            lock (_objects) return new List<IObjectConsumer>(_objects);
        }

        public int Consume(bool force = false) {
            lock (_objects) return _objects.RemoveWhere(e => e.Consume(force));
        }

        public void Add(IObjectConsumer o) {
            lock (_objects) _objects.Add(o);
        }

        public void Remove(IObjectConsumer o) {
            lock (_objects) _objects.RemoveWhere(x => x == o);
        }

        public virtual void Dispose() {
            Consume(true);
        }
    }
}