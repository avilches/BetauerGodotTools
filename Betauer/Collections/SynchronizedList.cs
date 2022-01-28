using System.Collections;
using System.Collections.Generic;

namespace Betauer.Collections {
    public class SynchronizedList<T> : SynchronizedCollection<T>, IList<T> {
        private readonly IList<T> _list;

        public SynchronizedList(IList<T> list) : base(list) {
            _list = list;
        }

        public T this[int index] {
            get {
                lock (Root) {
                    return _list[index];
                }
            }
            set {
                lock (Root) {
                    _list[index] = value;
                }
            }
        }

        public int IndexOf(T item) {
            lock (Root) {
                return _list.IndexOf(item);
            }
        }

        public void Insert(int index, T item) {
            lock (Root) {
                _list.Insert(index, item);
            }
        }

        public void RemoveAt(int index) {
            lock (Root) {
                _list.RemoveAt(index);
            }
        }
    }
}