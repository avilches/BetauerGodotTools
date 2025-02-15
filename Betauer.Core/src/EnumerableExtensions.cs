using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Godot;

namespace Betauer.Core;

public static partial class EnumerableExtensions {
    public static void ForEach<TValue>(this TValue[] source, Action<TValue> action) {
        foreach (var element in source) action(element);
    }

    public static void ForEach<TValue>(this TValue[] source, Action<TValue, int> action) {
        var i = 0;
        foreach (var element in source) action(element, i++);
    }

    public static void ForEach<TValue>(this List<TValue> source, Action<TValue, int> action) {
        for (var index = 0; index < source.Count; index++) {
            var element = source[index];
            action(element, index);
        }
    }

    public static void ForEach<TValue>(this LinkedList<TValue> source, Action<TValue> action) {
        foreach (var element in source) action(element);
    }

    public static void ForEach<TValue>(this LinkedList<TValue> source, Action<TValue, int> action) {
        var i = 0;
        foreach (var element in source) action(element, i++);
    }

    public static void ForEach<TKey, TValue>(this Dictionary<TKey, TValue>.ValueCollection source, Action<TValue> action) where TKey : notnull {
        foreach (var element in source) action(element);
    }

    public static void ForEach<TValue>(this ReadOnlyCollection<TValue> source, Action<TValue> action) {
        foreach (var element in source) action(element);
    }

    public static void ForEach<TValue>(this ReadOnlyCollection<TValue> source, Action<TValue, int> action) {
        var i = 0;
        foreach (var element in source) action(element, i++);
    }

    public static void ForEach<TValue>(this HashSet<TValue> source, Action<TValue> action) {
        foreach (var element in source) action(element);
    }

    public static void ForEach<TValue>(this HashSet<TValue> source, Action<TValue, int> action) {
        var i = 0;
        foreach (var element in source) action(element, i++);
    }

    public static void ForEach<[MustBeVariant] TValue>(this Godot.Collections.Array<TValue> source, Action<TValue> action) {
        var count = source.Count;
        for (var i = 0; i < count; ++i) action(source[i]);
    }

    public static void ForEach<[MustBeVariant] TValue>(this Godot.Collections.Array<TValue> source, Action<TValue, int> action) {
        var count = source.Count;
        for (var i = 0; i < count; ++i) action(source[i], i);
    }

    public static void ForEach<TKey, TValue>(this Dictionary<TKey, TValue> source, Action<KeyValuePair<TKey, TValue>> action) where TKey : notnull {
        foreach (var element in source) action(element);
    }

    public static void ForEach<TKey, TValue>(this Godot.Collections.Dictionary<TKey, TValue> source, Action<KeyValuePair<TKey, TValue>> action) where TKey : notnull {
        foreach (var element in source) action(element);
    }

    public static void ForEach<TKey, TValue>(this ReadOnlyDictionary<TKey, TValue> source, Action<KeyValuePair<TKey, TValue>> action) where TKey : notnull {
        foreach (var element in source) action(element);
    }
}