using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Betauer.Collections {
    public class SynchronizedCollection<T> : ICollection<T> {
        protected readonly ICollection<T> List;
        protected readonly object Root;

        public SynchronizedCollection(ICollection<T> list) {
            List = list;
            Root = ((ICollection)list).SyncRoot;
        }

        public int Count {
            get {
                lock (Root) {
                    return List.Count;
                }
            }
        }

        public bool IsReadOnly => List.IsReadOnly;

        public void Add(T item) {
            lock (Root) {
                List.Add(item);
            }
        }

        public void Clear() {
            lock (Root) {
                List.Clear();
            }
        }

        public bool Contains(T item) {
            lock (Root) {
                return List.Contains(item);
            }
        }

        public void CopyTo(T[] array, int arrayIndex) {
            lock (Root) {
                List.CopyTo(array, arrayIndex);
            }
        }

        public bool Remove(T item) {
            lock (Root) {
                return List.Remove(item);
            }
        }

        IEnumerator IEnumerable.GetEnumerator() {
            lock (Root) {
                return List.ToList().GetEnumerator();
            }
        }

        IEnumerator<T> IEnumerable<T>.GetEnumerator() {
            lock (Root) {
                return List.ToList().GetEnumerator();
            }
        }

    }
}