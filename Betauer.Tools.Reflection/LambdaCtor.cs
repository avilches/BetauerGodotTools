using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq.Expressions;
using System.Reflection;

namespace Betauer.Tools.Reflection;

public class LambdaCtor {
    public static Func<T> CreateCtor<T>() => 
        LambdaCtor<T>.CreateInstance;

    public static Func<object> CreateCtor(Type type) =>
        Expression.Lambda<Func<object>>(Expression.New(type)).Compile();
}

public class LambdaCtor<T> {
    private static Func<T>? _ctor;
    public static Func<T> Ctor => _ctor ??= Expression.Lambda<Func<T>>(Expression.New(typeof(T))).Compile();

    public static T CreateInstance() {
        return Ctor.Invoke();
    }
}


 class Ctor<T> {
    private static ConstructorInfo? _ctor;

    public static T CreateInstance() {
        _ctor ??= typeof(T).GetConstructor(Type.EmptyTypes);
        return (T)_ctor.Invoke(null);
    }
}

 class FastCtorTest {
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
        Console.WriteLine("Ctor<List<string>>.CreateInstance():" + x.ElapsedMilliseconds);
        GC.GetTotalMemory(true);

        x = Stopwatch.StartNew();
        for (var i = 0; i < TIMES; i++) LambdaCtor<List<string>>.CreateInstance();
        Console.WriteLine("LambdaCtor<List<string>>.CreateInstance():" + x.ElapsedMilliseconds);
        GC.GetTotalMemory(true);

        x = Stopwatch.StartNew();
        var func = LambdaCtor.CreateCtor(typeof(List<string>));
        for (var i = 0; i < TIMES; i++) func.Invoke();
        Console.WriteLine("* LambdaCtor.CreateCtor(typeof(List<string>)):" + x.ElapsedMilliseconds);
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
