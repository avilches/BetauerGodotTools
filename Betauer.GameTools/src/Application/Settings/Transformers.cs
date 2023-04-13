using System;
using System.Collections.Generic;
using Betauer.Application.Screen;
using Betauer.Core;
using Betauer.Tools.FastReflection;
using Godot;

namespace Betauer.Application.Settings; 

public interface ITransformer {
    object ToVariant(object d);
    object FromVariant(object d);
    object FromString(string d);
}

public class ResolutionTransformer : ITransformer {
    public object ToVariant(object d) => ((Resolution)d).Size;
    public object FromVariant(object d) => new Resolution((Vector2I)d);
    public object FromString(string d) {
        var parts = d.Split("x");
        return new Resolution(new Vector2I {
            X = int.Parse(parts[0]),
            Y = int.Parse(parts[1])
        });
    }
}

public static class Transformers {
    private static readonly Dictionary<Type, ITransformer> Registry = new();

    static Transformers() {
        AddTransformer(new ResolutionTransformer(), typeof(Resolution), typeof(ScaledResolution));
    }

    public static void AddTransformer(ITransformer transformer, params Type[] types) {
        foreach (var type in types) Registry[type] = transformer;
    }

    public static object ToVariant(object value) {
        return Registry.TryGetValue(value.GetType(), out var t) ? t.ToVariant(value) : value;
    }

    public static object FromVariant(Type type, object value) {
        return Registry.TryGetValue(type, out var t) ? t.FromVariant(value) : value;
    }

    public static object FromString(Type type, string value) {
        return Registry.TryGetValue(type, out var t) ? t.FromString(value) : throw new Exception($"Missing Transformer for type: {type.GetTypeName()}");
    }
}