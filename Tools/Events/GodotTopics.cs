using Godot;

namespace Tools.Events {
    public class GodotMulticastTopic<T> : MulticastTopic<NodeFromListenerDelegate<T>, T>
        where T : IEventFromNode {
        public void Publish(T @event, string name) {
            if (Debug.DEBUG_EVENT_LISTENER) {
                GD.Print("[Event.GodotMultiCast] Published(" + name + ") event to " + _eventListeners.Count +
                         " listeners");
            }

            Publish(@event);
        }

        public override void Publish(T @event) {
            int size = _eventListeners.Count;
            int deleted = _eventListeners.RemoveAll(listener => listener.IsDisposed());
            if (Debug.DEBUG_EVENT_LISTENER && deleted > 0) {
                GD.Print("[Event.GodotMultiCast] Disposed listeners deleted: " + deleted + "/" + size);
            }

            _eventListeners.ForEach(listener => listener.OnEvent(@event));
        }
    }

    public class GodotUnicastTopic<T> : UnicastTopic<NodeFromListenerDelegate<T>, T>
        where T : IEventFromNode {
        public void Publish(T @event, string name) {
            if (Debug.DEBUG_EVENT_LISTENER) {
                var status = _eventListener == null ? "null" : (_eventListener.IsDisposed() ? "Disposed" : "Ok");
                GD.Print("[Event.GodotUniCast] Published(" + name + ") event to single listener. Status: " + status);
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