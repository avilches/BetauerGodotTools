using Godot;

namespace Game.Tools.Events {
    public class GodotMulticastTopic<T> : MulticastTopic<NodeFromListenerDelegate<T>, T>
        where T : EventFromNode {
        public void Publish(T @event, string name) {
            if (Debug.DEBUG_EVENT_LISTENER) {
                GD.Print("[Event] " + name + ": " + _eventListeners.Count);
            }

            Publish(@event);
        }

        public override void Publish(T @event) {
            int size = _eventListeners.Count;
            int deleted = _eventListeners.RemoveAll(listener => listener.IsDisposed());
            if (Debug.DEBUG_EVENT_LISTENER) {
                GD.Print("[Event] Disposed listeners deleted: " + deleted + "/" + size);
            }

            _eventListeners.ForEach(listener => listener.OnEvent(@event));
        }
    }

    public class GodotUnicastTopic<T> : UnicastTopic<NodeFromListenerDelegate<T>, T>
        where T : EventFromNode {
        public void Publish(T @event, string name) {
            if (Debug.DEBUG_EVENT_LISTENER) {
                GD.Print("[Event] " + name+ ": event listener is "+(_eventListener == null?"null":(_eventListener.IsDisposed()?"Disposed":"Ok")));
            }
            Publish(@event);
        }

        public override void Publish(T @event) {
            if (_eventListener == null) return;
            if (_eventListener.IsDisposed()) {
                _eventListener = null;
                return;
            }

            _eventListener.OnEvent(@event);
        }
    }
}