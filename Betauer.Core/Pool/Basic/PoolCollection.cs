namespace Betauer.Core.Pool.Basic;

public abstract class PoolCollection {
    public abstract int Count { get; }

    public class Queue<T> : PoolCollection<T> {
        public System.Collections.Generic.Queue<T> Pool { get; }

        public Queue(int initialCapacity = 4) {
            Pool = new System.Collections.Generic.Queue<T>(initialCapacity);
        }

        public override T Get() => Pool.Dequeue();
        public override void Add(T ele) => Pool.Enqueue(ele);
        public override int Count => Pool.Count;
    }

    public class Stack<T> : PoolCollection<T> {
        public System.Collections.Generic.Stack<T> Pool { get; }

        public Stack(int initialCapacity = 4) {
            Pool = new System.Collections.Generic.Stack<T>(initialCapacity);
        }

        public override T Get() => Pool.Pop();
        public override void Add(T ele) => Pool.Push(ele);
        public override int Count => Pool.Count;
    }
}

public abstract class PoolCollection<T> : PoolCollection {
    public abstract T Get();
    public abstract void Add(T ele);
}