using System.Collections.Generic;

namespace Betauer.Application.Lifecycle.Pool;

public interface IManagedPool {
    public IEnumerable<object> Drain();
    public IEnumerable<object> GetBusy();
    void Clear();
    void PreInject(string name);
}