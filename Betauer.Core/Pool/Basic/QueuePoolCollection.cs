using System.Collections.Generic;

namespace Betauer.Core.Pool.Basic;

public class QueuePoolCollection<T> : IPoolCollection<T> {
    public Queue<T> Pool { get; }

    public QueuePoolCollection(int initialCapacity = 4) {
        Pool = new Queue<T>(initialCapacity);
    }

    public T Get() => Pool.Dequeue();
    public void Add(T ele) => Pool.Enqueue(ele);
    public int Count => Pool.Count;
}