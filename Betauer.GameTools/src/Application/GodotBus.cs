using System;
using System.Collections.Generic;
using System.Linq;
using Betauer.Bus;
using Betauer.Core;
using Godot;

namespace Betauer.Application; 

public class GodotBus<TF> : IDisposable {

    public HashSet<Multicast> Multicasts { get; } = new();

    public void Publish<T>(T e) where T : TF {
        Holder<T>.Bus.Publish(e);
    }

    public EventConsumer<T> Subscribe<T>(Action<T> action) where T : TF {
        Multicast<T> multicast = Holder<T>.Bus;
        Multicasts.Add(multicast);
        return multicast.Subscribe(action);
    }

    public EventConsumer<T> Subscribe<T>(GodotObject o, Action<T> action) where T : TF {
        var eventConsumer = Subscribe(action);
        eventConsumer.UnsubscribeIf(Predicates.IsInvalid(o));
        return eventConsumer;
    }

    public void Dispose() {
        foreach (var multicast in Multicasts) {
            multicast.Dispose();
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
    }
}