using System;
using System.Collections.Generic;

namespace Betauer.Memory {
    public interface IObjectLifeCycle : IDisposable {
        public bool MustBeDisposed();
    }

    public class ObjectLifeCycleManager {
        public static readonly ObjectLifeCycleManager Singleton = new ObjectLifeCycleManager();
        
        private readonly List<IObjectLifeCycle> _signalHandlersList = new List<IObjectLifeCycle>();

        public void Add(IObjectLifeCycle signalHolder) {
            lock (_signalHandlersList) _signalHandlersList.Add(signalHolder);
        }

        public List<IObjectLifeCycle> GetAll() {
            lock (_signalHandlersList) return new List<IObjectLifeCycle>(_signalHandlersList);
        }

        public void Dispose(IObjectLifeCycle o) {
            try {
                o.Dispose();
            } finally {
                lock (_signalHandlersList) _signalHandlersList.Remove(o);
            }
        }

        public int DisposeAllInvalid() {
            var x = 0;
            lock (_signalHandlersList) {
                // loop backward allows to delete a List safely
                for (var i = _signalHandlersList.Count - 1; i >= 0; i--) {
                    var signal = _signalHandlersList[i];
                    if (signal.MustBeDisposed()) {
                        try {
                            signal.Dispose();
                        } finally {
                            x++;
                            _signalHandlersList.RemoveAt(i);
                        }
                    }
                }                
            }
            return x;
        }

        public int DisposeAll() {
            var x = 0;
            lock (_signalHandlersList) {
                // loop backward allows to delete a List safely
                x = _signalHandlersList.Count;
                try {
                    for (var i = _signalHandlersList.Count - 1; i >= 0; i--) {
                        var signal = _signalHandlersList[i];
                        signal.Dispose();
                    }
                } finally {
                    _signalHandlersList.Clear();
                }
            }
            return x;
        }
    }
}