namespace Betauer.Core.Pool;

public abstract class PoolCollection<T> {
    public abstract int Count { get; }
    public abstract T Get();
    public abstract void Add(T ele);
    public abstract void Clear();

    public class Queue : PoolCollection<T> {
        private System.Collections.Generic.Queue<T> Pool { get; }

        public Queue(int initialCapacity = 4) {
            Pool = new System.Collections.Generic.Queue<T>(initialCapacity);
        }

        public override T Get() => Pool.Dequeue();
        public override void Add(T ele) {
            if (Pool.Contains(ele)) return;
            Pool.Enqueue(ele);
        }

        public override int Count => Pool.Count;
        public override void Clear() => Pool.Clear();
    }

    public class Stack : PoolCollection<T> {
        private System.Collections.Generic.Stack<T> Pool { get; }

        public Stack(int initialCapacity = 4) {
            Pool = new System.Collections.Generic.Stack<T>(initialCapacity);
        }

        public override T Get() => Pool.Pop();
        public override void Add(T ele) {
            if (Pool.Contains(ele)) return;
            Pool.Push(ele);
        }

        public override int Count => Pool.Count;
        public override void Clear() => Pool.Clear();
    }
}

