using System.Collections.Generic;

namespace Betauer.Core.Pool.Basic;

public class StackPoolCollection<T> : IPoolCollection<T> {
    public Stack<T> Pool { get;  }

    public StackPoolCollection(int initialCapacity = 4) {
        Pool = new Stack<T>(initialCapacity);
    }

    public T Get() => Pool.Pop();
    public void Add(T ele) => Pool.Push(ele);
    public int Count => Pool.Count;
}