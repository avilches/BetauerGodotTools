using System.Runtime.CompilerServices;
using BenchmarkDotNet.Attributes;
using Betauer.Core;

namespace Benchmarks;

[MemoryDiagnoser(true)]
public class Benchmark {

    public enum MyEnum {
        A = 12, B
    }

    public class MyClass<T> where T : Enum {

        public T GetFromLambda(int x) {
            return x.ToEnum<T>();
        }

        public T GetFromCast(int x) {
            return (T) (object) x;
        }

        public T GetFromParse(int x) {
            return (T)Enum.Parse(typeof(T), x.ToString(), true);
        }

        public T GetFromUnsafe(int x) {
            return Unsafe.As<int, T>(ref x);
        }

    }
    
    public MyClass<MyEnum> myClass = new MyClass<MyEnum>();
    
    [Benchmark]
    public void GetFromLambda() {
        myClass.GetFromLambda(12);
    }
    
    [Benchmark]
    public void GetFromCast() {
        myClass.GetFromCast(12);
    }
    
    [Benchmark]
    public void GetFromParse() {
        myClass.GetFromParse(12);
    }

    [Benchmark]
    public void GetFromUnsafe() {
        myClass.GetFromUnsafe(12);
    }

}