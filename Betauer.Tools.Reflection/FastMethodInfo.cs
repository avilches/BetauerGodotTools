using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq.Expressions;
using System.Reflection;

namespace Betauer.Tools.Reflection; 

public class FastMethodInfo {
    public FastMethodInfo(MethodInfo methodInfo) {
        var instanceExpression = Expression.Parameter(typeof(object), "instance");
        var argumentsExpression = Expression.Parameter(typeof(object[]), "arguments");
        var argumentExpressions = new List<Expression>();
        var parameterInfos = methodInfo.GetParameters();
        for (var i = 0; i < parameterInfos.Length; ++i) {
            var parameterInfo = parameterInfos[i];
            argumentExpressions.Add(Expression.Convert(
                Expression.ArrayIndex(argumentsExpression, Expression.Constant(i)), parameterInfo.ParameterType));
        }
        var callExpression =
            Expression.Call(
                !methodInfo.IsStatic ? Expression.Convert(instanceExpression, methodInfo.ReflectedType) : null,
                methodInfo, argumentExpressions);
        if (callExpression.Type == typeof(void)) {
            var voidDelegate = Expression
                .Lambda<VoidDelegate>(callExpression, instanceExpression, argumentsExpression).Compile();
            Delegate = (instance, arguments) => {
                voidDelegate(instance, arguments);
                return null;
            };
        } else {
            Delegate = Expression.Lambda<ReturnValueDelegate>(Expression.Convert(callExpression, typeof(object)),
                instanceExpression, argumentsExpression).Compile();
        }
    }

    private ReturnValueDelegate Delegate { get; }

    public object? Invoke(object instance, params object[] arguments) {
        return Delegate(instance, arguments);
    }

    private delegate object? ReturnValueDelegate(object instance, object[] arguments);

    private delegate void VoidDelegate(object instance, object[] arguments);
}

file class FastMethodInfoTest {
    private string v;

    public string Get() {
        return v;
    }

    public void Set(string value) {
        v = value;
    }

    public static void Main() {
        var ins = new FastMethodInfoTest();
        var TIMES = 1000000;

        var get = typeof(FastMethodInfoTest).GetMethod("Get");
        var x = Stopwatch.StartNew();
        for (var i = 0; i < TIMES; i++) get.Invoke(ins, new object[] { });
        Console.WriteLine("get.Invoke:" + x.ElapsedMilliseconds);

        var getf = new FastMethodInfo(get);
        x = Stopwatch.StartNew();
        for (var i = 0; i < TIMES; i++) getf.Invoke(ins);
        Console.WriteLine("get.Invoke fast:" + x.ElapsedMilliseconds);

        var set = typeof(FastMethodInfoTest).GetMethod("Set");
        x = Stopwatch.StartNew();
        for (var i = 0; i < TIMES; i++) set.Invoke(ins, new[] { "a" });
        Console.WriteLine("set.Invoke:" + x.ElapsedMilliseconds);

        var setf = new FastMethodInfo(set);
        x = Stopwatch.StartNew();
        for (var i = 0; i < TIMES; i++) setf.Invoke(ins, "a");
        Console.WriteLine("set.Invoke fast:" + x.ElapsedMilliseconds);
    }
}