using System;

namespace Betauer.Core.Pool;

public interface IPool {
    int Size();
    void Fill(int desiredSize);
    void RemoveAll();
    Type ElementType { get; }
}