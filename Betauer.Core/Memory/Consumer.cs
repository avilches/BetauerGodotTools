using System;
using System.Collections.Generic;
using Betauer.Tools.Logging;

namespace Betauer.Memory {
    public interface IObjectConsumer {
        public bool Consume(bool force);
    }

    public class Consumer {
        private static readonly Logger Logger = LoggerFactory.GetLogger(typeof(Consumer));
        private readonly HashSet<IObjectConsumer> _objects = new();
        protected bool Disposed = false;

        public int PeakSize { get; private set; }
        public int Size => _objects.Count;

        public List<IObjectConsumer> ToList() {
            lock (_objects) return new List<IObjectConsumer>(_objects);
        }

        public int Consume(bool force = false) {
            if (Disposed) throw new Exception("Consumer disposed");
            lock (_objects) {
                return _objects.RemoveWhere(o => {
                    var consumed = o.Consume(force);
                    #if DEBUG
                    if (consumed) Logger.Debug($"Size: {_objects.Count - 1} | - {o}");
                    #endif
                    return consumed;
                });
            }
        }

        public void Add(IObjectConsumer o) {
            if (Disposed) throw new Exception("Consumer disposed");
            lock (_objects) {
                #if DEBUG
                Logger.Debug($"Size: {_objects.Count + 1} | + {o}");
                #endif
                _objects.Add(o);
                PeakSize = Math.Max(PeakSize, _objects.Count);
            }
        }

        public void Remove(IObjectConsumer o) {
            if (Disposed) throw new Exception("Consumer disposed");
            lock (_objects) {
                _objects.RemoveWhere(x => {
                    var matches = x == o;
                    #if DEBUG
                    if (matches) Logger.Debug($"Size: {_objects.Count - 1} | - {o}");
                    #endif
                    return matches;
                });
            }
        }

        public virtual void Dispose() {
            Consume(true);
            Disposed = true;
        }
    }
}