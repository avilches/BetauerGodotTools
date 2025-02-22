using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

namespace Betauer.Core;

public static class AttributesHolderExtensions {
    public static AttributesHolder<T> Attributes<T>(this T instance) where T : class {
        return new AttributesHolder<T>(instance);
    }
}

public readonly struct AttributesHolder<T>(T instance) where T : class {
    public void Set(string key, object value) => AttributesContainer.Singleton.SetAttribute(instance, key, value);
    public object? Get(string key) => AttributesContainer.Singleton.GetAttribute(instance, key);
    public TValue? GetAs<TValue>(string key) => AttributesContainer.Singleton.GetAttributeAs<T, TValue>(instance, key);
    public TValue GetOr<TValue>(string key, TValue defaultValue) => AttributesContainer.Singleton.GetAttributeOr(instance, key, defaultValue);
    public TValue GetOrCreate<TValue>(string key, Func<TValue> factory) => AttributesContainer.Singleton.GetAttributeOrCreate(instance, key, factory);
    public bool Remove(string key) => AttributesContainer.Singleton.RemoveAttribute(instance, key);
    public bool Has(string key) => AttributesContainer.Singleton.HasAttribute(instance, key);

    public IEnumerable<KeyValuePair<string, object>> Get() => AttributesContainer.Singleton.GetAttributes(instance);
    public void Clear() => AttributesContainer.Singleton.ClearAttributes(instance);
    public int Count => AttributesContainer.Singleton.GetAttributeCount(instance);
    public bool HasAny => AttributesContainer.Singleton.HasAnyAttribute(instance);
}

public class AttributesContainer {
    public static readonly AttributesContainer Singleton = new();

    public const int CleanupInterval = 1000;
    public const int MaxSize = 1000;

    public readonly Dictionary<AttributeKey, object> Attributes = [];
    public readonly Dictionary<int, WeakReference> References = [];

    private int _operationCount;

    public readonly struct AttributeKey(object instance, string key) {
        public readonly int ReferenceId = RuntimeHelpers.GetHashCode(instance);
        public readonly string Key = key;

        public bool Is(object o) {
            return ReferenceId == RuntimeHelpers.GetHashCode(o);
        }
    }

    private void ValidateReferenceType<T>(T instance) where T : class {
        // Rechaza value types
        if (instance is null) {
            throw new ArgumentException(
                $"Cannot track null. Only reference types (classes) are supported because they need stable hash codes and proper garbage collection tracking.");
        }

        var type = instance.GetType();

        // Rechaza value types
        if (type.IsValueType) {
            throw new ArgumentException(
                $"Cannot track attributes for value types. The instance of type '{type.Name}' is a value type (struct or primitive). " +
                "Only reference types (classes) are supported because they need stable hash codes and proper garbage collection tracking.");
        }

        // Rechaza string
        if (type == typeof(string)) {
            throw new ArgumentException(
                $"Cannot track attributes for strings. Strings are immutable and can share references due to string interning, " +
                "which makes them unsuitable for stable hash code tracking.");
        }

        // Rechaza delegates
        if (typeof(Delegate).IsAssignableFrom(type)) {
            throw new ArgumentException(
                $"Cannot track attributes for delegates or lambda expressions. The instance of type '{type.Name}' is a delegate type, " +
                "which might not guarantee stable lifetime tracking.");
        }
    }
    private void TrackInstance<T>(T instance) where T : class {
        ValidateReferenceType(instance);
        var id = RuntimeHelpers.GetHashCode(instance);
        if (!References.ContainsKey(id)) {
            References[id] = new WeakReference(instance);
        }
    }

    private bool ShouldCleanup() {
        if (Attributes.Count < MaxSize) return false;
        return ++_operationCount >= CleanupInterval;
    }

    public void Cleanup() {
        var deadReferences = References.Where(pair => !pair.Value.IsAlive).ToArray();
        foreach (var (id, _) in deadReferences) {
            References.Remove(id);
            var keysToRemove = Attributes.Keys.Where(key => key.ReferenceId == id).ToArray();
            foreach (var key in keysToRemove) {
                Attributes.Remove(key);
            }
        }
        _operationCount = 0;
    }

    public void SetAttribute<T>(T instance, string key, object value) where T : class {
        ValidateReferenceType(instance);
        TrackInstance(instance);
        Attributes[new AttributeKey(instance, key)] = value;
    }

    public object? GetAttribute<T>(T instance, string key) where T : class {
        ValidateReferenceType(instance);
        if (ShouldCleanup()) Cleanup();
        return Attributes.GetValueOrDefault(new AttributeKey(instance, key));
    }

    public TValue? GetAttributeAs<T, TValue>(T instance, string key) where T : class {
        ValidateReferenceType(instance);
        if (ShouldCleanup()) Cleanup();
        return Attributes.TryGetValue(new AttributeKey(instance, key), out var value) && value is TValue typedValue ? typedValue : default;
    }

    public TValue GetAttributeOr<T, TValue>(T instance, string key, TValue defaultValue) where T : class {
        ValidateReferenceType(instance);
        if (ShouldCleanup()) Cleanup();
        return Attributes.TryGetValue(new AttributeKey(instance, key), out var value) && value is TValue typedValue ? typedValue : defaultValue;
    }

    public TValue GetAttributeOrCreate<T, TValue>(T instance, string key, Func<TValue> factory) where T : class {
        ValidateReferenceType(instance);
        if (ShouldCleanup()) Cleanup();
        var attributeKey = new AttributeKey(instance, key);
        if (Attributes.TryGetValue(attributeKey, out var value) && value is TValue typedValue) {
            return typedValue;
        }
        var newValue = factory();
        TrackInstance(instance);
        Attributes[attributeKey] = newValue;
        return newValue;
    }

    public bool RemoveAttribute<T>(T instance, string key) where T : class {
        ValidateReferenceType(instance);
        if (ShouldCleanup()) Cleanup();
        return Attributes.Remove(new AttributeKey(instance, key));
    }

    public bool HasAttribute<T>(T instance, string key) where T : class {
        ValidateReferenceType(instance);
        if (ShouldCleanup()) Cleanup();
        return Attributes.ContainsKey(new AttributeKey(instance, key));
    }

    public IEnumerable<KeyValuePair<string, object>> GetAttributes<T>(T instance) where T : class {
        ValidateReferenceType(instance);
        if (ShouldCleanup()) Cleanup();
        return Attributes.Where(kv => kv.Key.Is(instance))
            .Select(kv => new KeyValuePair<string, object>(kv.Key.Key, kv.Value));
    }

    public int GetAttributeCount<T>(T instance) where T : class {
        ValidateReferenceType(instance);
        if (ShouldCleanup()) Cleanup();
        return Attributes.Count(kv => kv.Key.Is(instance));
    }

    public bool HasAnyAttribute<T>(T instance) where T : class {
        ValidateReferenceType(instance);
        if (ShouldCleanup()) Cleanup();
        return Attributes.Keys.Any(k => k.Is(instance));
    }

    public void ClearAttributes<T>(T instance) where T : class {
        ValidateReferenceType(instance);
        if (ShouldCleanup()) Cleanup();
        foreach (var key in Attributes.Keys.Where(k => k.Is(instance)).ToArray()) Attributes.Remove(key);
    }
}