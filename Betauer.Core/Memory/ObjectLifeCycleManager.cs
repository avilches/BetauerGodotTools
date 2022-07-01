using System.Collections.Generic;

namespace Betauer.Memory {
    public interface IObjectLifeCycle {
        public bool IsValid();
        public void Destroy();
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

        public void Free(IObjectLifeCycle o) {
            try {
                o.Destroy();
            } finally {
                lock (_signalHandlersList) _signalHandlersList.Remove(o);
            }
        }

        public void FreeAllInvalid() {
            lock (_signalHandlersList) {
                // loop backward allows to delete a List safely
                for (var i = _signalHandlersList.Count - 1; i >= 0; i--) {
                    var signal = _signalHandlersList[i];
                    if (!signal.IsValid()) {
                        try {
                            signal.Destroy();
                        } finally {
                            _signalHandlersList.RemoveAt(i);
                        }
                    }
                }                
            }
        }

        public void FreeAll() {
            lock (_signalHandlersList) {
                // loop backward allows to delete a List safely
                try {
                    for (var i = _signalHandlersList.Count - 1; i >= 0; i--) {
                        var signal = _signalHandlersList[i];
                        signal.Destroy();
                    }
                } finally {
                    _signalHandlersList.Clear();
                }
            }
        }
    }
}