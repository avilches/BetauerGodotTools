using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq.Expressions;
using System.Reflection;

namespace Betauer.Tools.Reflection;

public class LambdaCtor<T> {
    private static Func<T>? _ctor;

    public static T CreateInstance() {
        _ctor ??= Expression.Lambda<Func<T>>(Expression.New(typeof(T))).Compile();
        return _ctor.Invoke();
    }
}


file class Ctor<T> {
    private static ConstructorInfo? _ctor;

    public static T CreateInstance() {
        _ctor ??= typeof(T).GetConstructor(Type.EmptyTypes);
        return (T)_ctor.Invoke(null);
    }
}

file class FastCtorTest {
    public static void Main() {
        var TIMES = 10000000;
        Stopwatch x = null;
        
        Console.WriteLine("* means it needs a first call to do something (cache, compile...");
        x = Stopwatch.StartNew();
        for (var i = 0; i < TIMES; i++) new List<string>();
        Console.WriteLine("new List<string>():" + x.ElapsedMilliseconds);
        GC.GetTotalMemory(true);

        x = Stopwatch.StartNew();
        var ctor = typeof(List<string>).GetConstructor(Type.EmptyTypes);
        for (var i = 0; i < TIMES; i++) ctor.Invoke(null);
        Console.WriteLine("* ctor.Invoke(null):" + x.ElapsedMilliseconds);
        GC.GetTotalMemory(true);

        x = Stopwatch.StartNew();
        for (var i = 0; i < TIMES; i++) Ctor<List<string>>.CreateInstance();
        Console.WriteLine("* Ctor<List<string>>.CreateInstance():" + x.ElapsedMilliseconds);
        GC.GetTotalMemory(true);

        x = Stopwatch.StartNew();
        var lambda = Expression.Lambda<Func<object>>(Expression.New(typeof(List<string>))).Compile();
        for (var i = 0; i < TIMES; i++) lambda.Invoke();
        Console.WriteLine("* lambda.Invoke():" + x.ElapsedMilliseconds);
        GC.GetTotalMemory(true);
        
        x = Stopwatch.StartNew();
        for (var i = 0; i < TIMES; i++) LambdaCtor<List<string>>.CreateInstance();
        Console.WriteLine("* LambdaCtor<List<string>>.CreateInstance():" + x.ElapsedMilliseconds);
        GC.GetTotalMemory(true);

        x = Stopwatch.StartNew();
        for (var i = 0; i < TIMES; i++) Activator.CreateInstance(typeof(List<string>));
        Console.WriteLine("Activator.CreateInstance(typeof(List<string>)):" + x.ElapsedMilliseconds);
        GC.GetTotalMemory(true);

        x = Stopwatch.StartNew();
        for (var i = 0; i < TIMES; i++) Activator.CreateInstance<List<string>>();
        Console.WriteLine("Activator.CreateInstance<List<string>>():" + x.ElapsedMilliseconds);
        GC.GetTotalMemory(true);



    }

}
