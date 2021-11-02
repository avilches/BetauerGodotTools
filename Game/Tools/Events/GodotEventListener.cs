using System;
using Godot;

namespace Game.Tools.Events {
    public interface EventFromNode {
        Node GetFrom();
    }

    public abstract class NodeFromListener<T> : ConditionalEventListener<T> where T : EventFromNode {
        private Node _body;

        public NodeFromListener(Node body) {
            _body = body;
        }

        public bool IsDisposed() => _body.NativeInstance == IntPtr.Zero;

        public override bool CanBeExecuted(T @event) {
            if (Debug.DEBUG_EVENT_LISTENER) {
                GD.Print("[Event] From " + @event.GetFrom().Name + " " + @event.GetFrom().NativeInstance + "/0x" +
                         @event.GetFrom().GetHashCode().ToString("X") +
                         " Me: " + _body.Name + " " + _body.NativeInstance + "/0x" + _body.GetHashCode().ToString("X") +
                         ": " + (@event.GetFrom() == _body ? "Ok!!" : ":("));
            }

            return @event.GetFrom() == _body;
        }
    }


    public class NodeFromListenerDelegate<T> : NodeFromListener<T> where T : EventFromNode {
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