using BenchmarkDotNet.Attributes;
using Betauer.Core;
using Godot;

namespace Benchmarks;

[MemoryDiagnoser(true)]
public class BenchmarkEnumCompare {
    public enum MyEnum {
        A,
        B,
        C,
    }

    public int a = 1;
    public int b = 1;
    public MyEnum A = MyEnum.A;
    public MyEnum B = MyEnum.B;

    [Benchmark]
    public void CompareEquals() {
        var x = MyEnum.A.Equals(MyEnum.B);
    }

    [Benchmark]
    public void CompareToInt() {
        var x = MyEnum.A.ToInt() == MyEnum.B.ToInt();
    }

    [Benchmark]
    public void CompareInts() {
        var x = a == b;
    }

    [Benchmark]
    public void CompareEnums() {
        var x = A == B;
    }

}