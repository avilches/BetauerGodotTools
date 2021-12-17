using System;
using System.Collections;
using System.Collections.Generic;

namespace Tools {
    public class SimpleLinkedList<T> : ICollection<T> {
        private readonly Node<T> _head = new Node<T>();
        private Node<T> _last;

        public SimpleLinkedList() {
        }

        public SimpleLinkedList(IEnumerable<T> tweenList) {
            foreach (var o in tweenList) {
                Add(o);
            }
        }

        public int Count { get; private set; } = 0;

        public void Clear() {
            _head.next = null;
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
            return Count > 0 ? _head.next.data : throw new Exception("There is no First() value for empty linked list");
        }

        public void AddStart(T data) {
            Node<T> newHead = new Node<T>();
            newHead.next = _head.next;
            newHead.data = data;
            _head.next = newHead;
            Count++;
        }

        public void Add(T data) {
            Node<T> newEnd = new Node<T>();
            newEnd.data = data;
            if (Count == 0) {
                _head.next = newEnd;
            } else {
                _last.next = newEnd;
            }
            _last = newEnd;
            Count++;
        }

        public void RemoveStart() {
            if (Count == 0)
                throw new Exception("No element exist in this linked list.");

            _head.next = _head.next.next;
            Count--;
            if (Count == 0) {
                _last = _head;
            }
        }

        public void RemoveEnd() {
            if (Count == 0)
                throw new Exception("No element exist in this linked list.");

            Node<T> prevNode = new Node<T>();
            Node<T> cur = _head;
            while (cur.next != null) {
                prevNode = cur;
                cur = cur.next;
            }
            prevNode.next = null;
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
            Node<T> curr = _head;
            while (curr.next != null) {
                curr = curr.next;
                action.Invoke(curr.data);
            }
        }

        public T Find(Predicate<T> element) {
            Node<T> curr = _head;
            while (curr.next != null) {
                curr = curr.next;
                if (element.Invoke(curr.data)) {
                    return curr.data;
                }
            }
            return (T)(object)null;
        }

        IEnumerator IEnumerable.GetEnumerator() {
            return GetEnumerator();
        }

        public IEnumerator<T> GetEnumerator() {
            return new Enumerator<T>(this);
        }

        private class Enumerator<T> : IEnumerator<T> {
            private readonly SimpleLinkedList<T> _list;
            private SimpleLinkedList<T>.Node<T> _current;

            internal Enumerator(SimpleLinkedList<T> list) {
                _list = list;
                Reset();
            }

            public bool MoveNext() {
                if (_current.next == null) return false;
                Current = _current.next.data;
                _current = _current.next;
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

        private class Node<T> {
            public T data;
            public Node<T> next;
        }
    }
}