using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq.Expressions;
using System.Reflection;

namespace Betauer.Tools.Reflection; 

public class FastMethodInfo {
    private delegate object? ReturnValueDelegate(object instance, object[] arguments);
    private delegate void VoidDelegate(object instance, object[] arguments);
    
    private readonly ReturnValueDelegate _delegate;

    public FastMethodInfo(MethodInfo methodInfo) {
        var instanceParameter = Expression.Parameter(typeof(object), "instance");
        var argumentsParameter = Expression.Parameter(typeof(object[]), "arguments");

        var instance = methodInfo.IsStatic ? null : Expression.Convert(instanceParameter, methodInfo.ReflectedType!);
        var argumentExpressionList = CreateArgumentExpressionList(methodInfo.GetParameters(), argumentsParameter);
        var callExpression = Expression.Call(instance, methodInfo, argumentExpressionList);
        if (callExpression.Type == typeof(void)) {
            var voidDelegate = Expression.Lambda<VoidDelegate>(callExpression, instanceParameter, argumentsParameter).Compile();
            _delegate = (instance, arguments) => {
                voidDelegate(instance, arguments);
                return null;
            };
        } else {
            _delegate = Expression.Lambda<ReturnValueDelegate>(Expression.Convert(callExpression, typeof(object)),
                instanceParameter, argumentsParameter).Compile();
        }
    }

    private static Expression[]? CreateArgumentExpressionList(ParameterInfo[] parameterInfos, ParameterExpression argumentsExpression) {
        if (parameterInfos.Length == 0) return null;
        var argumentExpressions = new Expression[parameterInfos.Length];
        for (var i = 0; i < parameterInfos.Length; ++i) {
            var parameterInfo = parameterInfos[i];
            argumentExpressions[i] = Expression.Convert(
                Expression.ArrayIndex(argumentsExpression, Expression.Constant(i)), parameterInfo.ParameterType);
        }
        return argumentExpressions;
    }

    public object? Invoke(object instance) {
        return _delegate(instance, Array.Empty<object>());
    }

    public object? Invoke(object instance, params object[] arguments) {
        return _delegate(instance, arguments);
    }

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
        var TIMES = 10000000;

        var get = typeof(FastMethodInfoTest).GetMethod("Get");
        var x = Stopwatch.StartNew();
        for (var i = 0; i < TIMES; i++) get.Invoke(ins, Array.Empty<object>());
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