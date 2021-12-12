using System;
using System.Collections;
using System.Collections.Generic;

namespace Tools {
    public class SimpleLinkedList<T> : IEnumerable<T> {
        private readonly Node<T> _head = new Node<T>();
        private Node<T> _last;
        public int Count { get; private set; } = 0;

        public SimpleLinkedList() {
            _last = _head;
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
            _last.next = newEnd;
            _last = newEnd;
            Count++;
        }

        public void RemoveStart() {
            if (Count == 0)
                throw new Exception("No element exist in this linked list.");

            _head.next = _head.next.next;
            Count--;
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