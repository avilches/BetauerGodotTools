using System;
using Godot;

namespace Betauer.Core.Signal; 

public static class SignalTools {
    public static uint SignalFlags(bool oneShot, bool deferred = false) =>
        (oneShot ? (uint)GodotObject.ConnectFlags.OneShot : 0) +
        (deferred ? (uint)GodotObject.ConnectFlags.Deferred : 0) +
        0;

    public static uint SignalOneShot() => (uint)GodotObject.ConnectFlags.OneShot;
    
    public static uint SignalDeferred() => (uint)GodotObject.ConnectFlags.Deferred;
}
