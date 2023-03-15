using System.Collections.Generic;

namespace Betauer.Core.Pool;

public interface IPool<out T> {
    T Get();
    public void Fill(int desiredSize);
    public IEnumerable<T> GetAll();
    public void Clear();
}