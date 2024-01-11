using System;
using System.Collections.Generic;
using System.Linq;
using Betauer.Bus;
using Betauer.Core;
using Godot;

namespace Betauer.Application;

public class GodotBus<TF> {
    private HashSet<Multicast> Multicasts { get; } = new();

    public Multicast<T> Bus<T>() where T : TF {
        Multicast<T> multicast = Holder<T>.Bus;
        if (Holder<T>.Initialized) return multicast;
        Multicasts.Add(multicast);
        Holder<T>.Initialized = true;
        return multicast;
    }

    public EventConsumer<T> Subscribe<T>(Action<T> action) where T : TF {
        return Bus<T>().Subscribe(action);
    }

    public EventConsumer<T> Subscribe<T>(GodotObject o, Action<T> action) where T : TF {
        var eventConsumer = Subscribe(action);
        eventConsumer.UnsubscribeIf(Predicates.IsInvalid(o));
        return eventConsumer;
    }

    public void Publish<T>(T e) where T : TF {
        Bus<T>().Publish(e);
    }

    public void Clear() {
        foreach (var multicast in Multicasts) {
            multicast.Clear();
        }
        Multicasts.Clear();
    }

    public void VerifyNoConsumers() {
        Multicasts.ForEach(m => m.Purge());
        var consumerCount = Multicasts.Sum(m => m.ConsumerCount);
        if (consumerCount > 0) throw new Exception($"There are {consumerCount} consumers left");
    }

    private static class Holder<T> where T : TF {
        internal static readonly Multicast<T> Bus = new();
        internal static bool Initialized = false;
    }
}