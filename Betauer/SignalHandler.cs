using System;
using Godot;
using Object = Godot.Object;

namespace Betauer {
    public static partial class GodotExtension {
        public static SignalAwaiter AwaitPhysicsFrame(this Node node) {
            return AwaitPhysicsFrame(node.GetTree());
        }

        public static SignalAwaiter AwaitIdleFrame(this Node node) {
            return AwaitIdleFrame(node.GetTree());
        }

        public static SignalAwaiter AwaitPhysicsFrame(this SceneTree sceneTree) {
            return sceneTree.ToSignal(sceneTree, GodotConstants.GODOT_SIGNAL_physics_frame);
        }

        public static SignalAwaiter AwaitIdleFrame(this SceneTree sceneTree) {
            return sceneTree.ToSignal(sceneTree, GodotConstants.GODOT_SIGNAL_idle_frame);
        }

        public static OnResizeWindowHandler OnResizeWindow(this SceneTree sceneTree, Action action) {
            return new OnResizeWindowHandler(sceneTree, action);
        }
    }

    public class OnResizeWindowHandler : SignalHandler {
        public OnResizeWindowHandler(Object target, Action action) : base(target,
            GodotConstants.GODOT_SIGNAL_screen_resized, action) {
        }
    }

    public class SignalHandler : DisposableGodotObject {
        private readonly Object _target;
        private readonly string _signal;
        private readonly Action _action;

        public SignalHandler(Object target, string signal, Action action) {
            _target = target;
            _signal = signal;
            _action = action;
            if (IsInstanceValid(target)) target.Connect(signal, this, nameof(Call));
        }

        public void Disconnect() {
            if (IsInstanceValid(_target)) {
                _target.Disconnect(_signal, this, nameof(Call));
            }
        }

        protected override void Dispose(bool disposing) {
            try {
                if (disposing) Disconnect();
            } finally {
                base.Dispose(disposing);
            }
        }

        internal void Call() {
            _action();
        }
    }

    public class SignalHandler<T> : DisposableGodotObject {
        private readonly Object _target;
        private readonly string _signal;
        private readonly Action<T> _action;

        public SignalHandler(Object target, string signal, Action<T> action) {
            _target = target;
            _signal = signal;
            _action = action;
            if (IsInstanceValid(target)) target.Connect(signal, this, nameof(Call));
        }

        public void Disconnect() {
            if (IsInstanceValid(_target)) {
                _target.Disconnect(_signal, this, nameof(Call));
            }
        }

        protected override void Dispose(bool disposing) {
            try {
                if (disposing) Disconnect();
            } finally {
                base.Dispose(disposing);
            }
        }

        internal void Call(T v1) {
            _action(v1);
        }
    }

    public class SignalHandler<T1, T2> : DisposableGodotObject {
        private readonly Object _target;
        private readonly string _signal;
        private readonly Action<T1, T2> _action;

        public SignalHandler(Object target, string signal, Action<T1, T2> action) {
            _target = target;
            _signal = signal;
            _action = action;
            if (IsInstanceValid(target)) target.Connect(signal, this, nameof(Call));
        }

        public void Disconnect() {
            if (IsInstanceValid(_target)) {
                _target.Disconnect(_signal, this, nameof(Call));
            }
        }

        protected override void Dispose(bool disposing) {
            try {
                if (disposing) Disconnect();
            } finally {
                base.Dispose(disposing);
            }
        }

        internal void Call(T1 v1, T2 v2) {
            _action(v1, v2);
        }
    }

    public class SignalHandler<T1, T2, T3> : DisposableGodotObject {
        private readonly Object _target;
        private readonly string _signal;
        private readonly Action<T1, T2, T3> _action;

        public SignalHandler(Object target, string signal, Action<T1, T2, T3> action) {
            _target = target;
            _signal = signal;
            _action = action;
            if (IsInstanceValid(target)) target.Connect(signal, this, nameof(Call));
        }

        public void Disconnect() {
            if (IsInstanceValid(_target)) {
                _target.Disconnect(_signal, this, nameof(Call));
            }
        }

        protected override void Dispose(bool disposing) {
            try {
                if (disposing) Disconnect();
            } finally {
                base.Dispose(disposing);
            }
        }

        internal void Call(T1 v1, T2 v2, T3 v3) {
            _action(v1, v2, v3);
        }
    }

    public class SignalHandler<T1, T2, T3, T4> : DisposableGodotObject {
        private readonly Object _target;
        private readonly string _signal;
        private readonly Action<T1, T2, T3, T4> _action;

        public SignalHandler(Object target, string signal, Action<T1, T2, T3, T4> action) {
            _target = target;
            _signal = signal;
            _action = action;
            if (IsInstanceValid(target)) target.Connect(signal, this, nameof(Call));
        }

        public void Disconnect() {
            if (IsInstanceValid(_target)) {
                _target.Disconnect(_signal, this, nameof(Call));
            }
        }

        protected override void Dispose(bool disposing) {
            try {
                if (disposing) Disconnect();
            } finally {
                base.Dispose(disposing);
            }
        }

        internal void Call(T1 v1, T2 v2, T3 v3, T4 v4) {
            _action(v1, v2, v3, v4);
        }
    }

    public class SignalHandler<T1, T2, T3, T4, T5> : DisposableGodotObject {
        private readonly Object _target;
        private readonly string _signal;
        private readonly Action<T1, T2, T3, T4, T5> _action;

        public SignalHandler(Object target, string signal, Action<T1, T2, T3, T4, T5> action) {
            _target = target;
            _signal = signal;
            _action = action;
            if (IsInstanceValid(target)) target.Connect(signal, this, nameof(Call));
        }

        public void Disconnect() {
            if (IsInstanceValid(_target)) {
                _target.Disconnect(_signal, this, nameof(Call));
            }
        }

        protected override void Dispose(bool disposing) {
            try {
                if (disposing) Disconnect();
            } finally {
                base.Dispose(disposing);
            }
        }

        internal void Call(T1 v1, T2 v2, T3 v3, T4 v4, T5 v5) {
            _action(v1, v2, v3, v4, v5);
        }
    }
}