using System;
using Godot;

namespace Betauer.Tests.Experimental {
    public abstract class DynamicSignal {
        public readonly Node Node;
        public readonly string Signal;
        public readonly bool OneShot = false;

        private bool _valid = true;

        protected DynamicSignal(Node node, string signal, bool oneShot) {
            Node = node;
            Signal = signal;
            OneShot = oneShot;
        }

        public bool IsValid() => _valid && Godot.Object.IsInstanceValid(Node);

        internal void Consume() {
            if (OneShot) _valid = false;
        }

        internal static string CreateKey(ulong instanceId, string signal, int actionHashCode) {
            return "" + instanceId + signal + actionHashCode;
        }
    }

    public abstract class DynamicSignal<T> : DynamicSignal {
        internal readonly T Action;

        protected DynamicSignal(Node node, string signal, T action, bool oneShot) : base(node, signal, oneShot) {
            Action = action;
        }

        public string GetKey() {
            return CreateKey(Node.GetInstanceId(), Signal, Action.GetHashCode());
        }
    }

    public class DynamicSignal0P : DynamicSignal<Action> {
        public DynamicSignal0P(Node node, string signal, Action action, bool oneShot) : base(node, signal, action,
            oneShot) {
        }
    }

    public class DynamicSignal1P : DynamicSignal<Action<object>> {
        public DynamicSignal1P(Node node, string signal, Action<object> action, bool oneShot) : base(node, signal,
            action,
            oneShot) {
        }
    }
}