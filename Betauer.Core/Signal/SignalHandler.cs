using System;
using Godot;
using Object = Godot.Object;

namespace Betauer.Signal {
    public static class SignalTools {
        public static uint SignalFlags(bool oneShot, bool deferred = false) =>
            (oneShot ? (uint)Object.ConnectFlags.Oneshot : 0) +
            (deferred ? (uint)Object.ConnectFlags.Deferred : 0) +
            0;
    }

    public class SignalHandler {
        public readonly Object Origin;
        public readonly string Signal;
        public readonly Object Target;
        public readonly string TargetMethodName;
        public readonly bool OneShot;
        public readonly bool Deferred;
        private string? _targetName;
        public SignalHandler(Object origin, string signal, Object target, string targetMethodName, bool oneShot = false, bool deferred = false) {
            Origin = origin;
            Signal = signal;
            Target = target;
            TargetMethodName = targetMethodName;
            OneShot = oneShot;
            Deferred = deferred;
            _targetName = Target is Node node ? node.Name : null;
            Connect();
        }

        public bool IsValid() => Object.IsInstanceValid(Target) && Object.IsInstanceValid(Origin);

        public void Connect() {
            if (!IsValid()) throw new Exception($"Can't connect '{Signal}' to a freed object");
            Error err = Origin.Connect(Signal, Target, TargetMethodName, null, SignalTools.SignalFlags(OneShot, Deferred));
            if (err != Error.Ok) {
                throw new Exception($"Connecting signal '{Signal}' to ${Target} failed: '{err}'");
            }
        }

        public bool IsConnected() {
            return IsValid() && Origin.IsConnected(Signal, Target, TargetMethodName);
        }

        public void Disconnect() {
            if (IsConnected()) Origin.Disconnect(Signal, Target, TargetMethodName);
        }

        public void Dispose() {
            if (Target is SignalObjectTarget a) a.Consume(true);
        }

        public void Unwatch() {
            if (Target is SignalObjectTarget a) a.Unwatch();
        }

        public void Watch() {
            if (Target is SignalObjectTarget a) a.Watch();
        }
    }
}