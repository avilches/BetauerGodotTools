using System;
using Godot;

namespace Tools.Events {
    public interface IEventFromNode {
        Node GetFrom();
    }

    public abstract class NodeFromListener<T> : ConditionalEventListener<T> where T : IEventFromNode {
        private Node _body;

        public NodeFromListener(Node body) {
            _body = body;
        }

        public bool IsDisposed() => _body.NativeInstance == IntPtr.Zero;

        public override bool CanBeExecuted(T @event) {
            var nodeFrom = @event.GetFrom();
            var matches = nodeFrom == _body;

            if (Debug.DEBUG_EVENT_LISTENER) {
                var nodeFromName = nodeFrom.GetType().FullName + " \"" + nodeFrom.Name + "\" [" +
                                   nodeFrom.NativeInstance + "/0x" + nodeFrom.GetHashCode().ToString("X") + "]";

                var me = _body.GetType().FullName + " \"" + _body.GetType().FullName + "\" [" + _body.NativeInstance +
                         "/0x" +
                         _body.GetHashCode().ToString("X") + "]";

                GD.Print("[Event.NodeFromListener] Listener: " + me + " | Received: " + nodeFromName + " ? "+matches);
            }

            return matches;
        }
    }


    public class NodeFromListenerDelegate<T> : NodeFromListener<T> where T : IEventFromNode {
        public delegate void ExecuteMethod(T @event);

        private readonly ExecuteMethod _executeMethod;

        public NodeFromListenerDelegate(Node body, ExecuteMethod executeMethod) : base(body) {
            _executeMethod = executeMethod;
        }

        public override void Execute(T @event) {
            _executeMethod.Invoke(@event);
        }
    }
}