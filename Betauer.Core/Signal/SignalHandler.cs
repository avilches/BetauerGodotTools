using System;
using Godot;
using Object = Godot.Object;

namespace Betauer.Core.Signal; 

public static class SignalTools {
    public static uint SignalFlags(bool oneShot, bool deferred = false) =>
        (oneShot ? (uint)Object.ConnectFlags.OneShot : 0) +
        (deferred ? (uint)Object.ConnectFlags.Deferred : 0) +
        0;
}

public class SignalHandler {
    public readonly Object Origin;
    public readonly string Signal;
    public readonly bool OneShot;
    public readonly bool Deferred;
    public readonly Callable Callable;
    private string? _targetName;
    public SignalHandler(Object origin, string signal, Callable callable, bool oneShot = false, bool deferred = false) {
        Origin = origin;
        Signal = signal;
        OneShot = oneShot;
        Deferred = deferred;
        Callable = callable;
        _targetName = callable.Target is Node node ? node.Name : null;
        Connect();
    }

    public bool IsValid() => (Callable.Target == null || Object.IsInstanceValid(Callable.Target)) && Object.IsInstanceValid(Origin);

    public void Connect() {
        if (!IsValid()) throw new Exception($"Can't connect '{Signal}' to a freed object");
        if (Origin.IsConnected(Signal, Callable)) return;
        Error err = Origin.Connect(Signal, Callable, SignalTools.SignalFlags(OneShot, Deferred));
        if (err != Error.Ok) {
            throw new Exception($"Connecting signal '{Signal}' from ${Origin} failed: '{err}'");
        }
    }

    public bool IsConnected() {
        return IsValid() && Origin.IsConnected(Signal, Callable);
    }

    public SignalHandler Disconnect() {
        if (IsConnected()) Origin.Disconnect(Signal, Callable);
        return this;
    }
}