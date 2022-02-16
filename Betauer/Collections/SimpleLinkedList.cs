using System;
using System.Collections;
using System.Collections.Generic;

namespace Betauer.Collections {
    public class SimpleLinkedList<T> : ICollection<T> {
        private Node _head;
        private Node _last;

        public SimpleLinkedList() {
        }

        public SimpleLinkedList(IEnumerable<T> collection) {
            foreach (var o in collection) {
                Add(o);
            }
        }

        public int Count { get; private set; } = 0;

        public void Clear() {
            _head.Next = null;
            _last = _head;
            Count = 0;
        }

        // TODO: implement??
        public bool Contains(T item) {
            throw new NotImplementedException();
        }

        public void CopyTo(T[] array, int arrayIndex) {
            ToList().CopyTo(array, arrayIndex);
        }

        public bool Remove(T item) {
            throw new NotImplementedException();
        }

        public bool IsReadOnly { get; }

        public T First() {
            return Count > 0 ? _head.Next.Data : throw new Exception("There is no First() value for empty linked list");
        }

        public void AddStart(T data) {
            if (_head == null) _head = new Node();
            Node newHead = new Node();
            newHead.Next = _head.Next;
            newHead.Data = data;
            _head.Next = newHead;
            Count++;
        }

        public void Add(T data) {
            if (_head == null) _head = new Node();
            Node newEnd = new Node();
            newEnd.Data = data;
            if (Count == 0) {
                _head.Next = newEnd;
            } else {
                _last.Next = newEnd;
            }
            _last = newEnd;
            Count++;
        }

        public void RemoveStart() {
            if (Count == 0)
                throw new Exception("No element exist in this linked list.");

            _head.Next = _head.Next.Next;
            Count--;
            if (Count == 0) {
                _last = _head;
            }
        }

        public void RemoveEnd() {
            if (Count == 0)
                throw new Exception("No element exist in this linked list.");

            Node prevNode = new Node();
            Node cur = _head;
            while (cur.Next != null) {
                prevNode = cur;
                cur = cur.Next;
            }
            prevNode.Next = null;
            _last = prevNode;
            Count--;
        }

        public T[] ToArray() {
            return ToList().ToArray();
        }

        public List<T> ToList() {
            List<T> list = new List<T>(Count);
            ForEach(obj => list.Add(obj));
            return list;
        }

        public void ForEach(Action<T> action) {
            if (Count == 0) return;
            Node curr = _head;
            while (curr.Next != null) {
                curr = curr.Next;
                action.Invoke(curr.Data);
            }
        }

        public T Find(Predicate<T> element) {
            if (Count == 0) return (T)(object)null;
            Node curr = _head;
            while (curr.Next != null) {
                curr = curr.Next;
                if (element.Invoke(curr.Data)) {
                    return curr.Data;
                }
            }
            return (T)(object)null;
        }

        IEnumerator IEnumerable.GetEnumerator() {
            return GetEnumerator();
        }

        public IEnumerator<T> GetEnumerator() {
            return new Enumerator(this);
        }

        private class Enumerator : IEnumerator<T> {
            private readonly SimpleLinkedList<T> _list;
            private Node _current;

            internal Enumerator(SimpleLinkedList<T> list) {
                _list = list;
                Reset();
            }

            public bool MoveNext() {
                if (_list.Count == 0 || _current.Next == null) return false;
                Current = _current.Next.Data;
                _current = _current.Next;
                return true;
            }

            public void Reset() {
                Current = default;
                _current = _list._head;
                ;
            }

            object IEnumerator.Current => Current;

            public void Dispose() {
            }

            public T Current { get; private set; }
        }

        private class Node {
            internal T Data;
            internal Node Next;
        }
    }
}