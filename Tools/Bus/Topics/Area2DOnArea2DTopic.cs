using Godot;
using Godot.Collections;

namespace Tools.Bus.Topics {
    public class Area2DOnArea2D : IGodotFilterEvent {
        public readonly Area2D Detected;
        public Node Origin { get; }
        public Node Filter => Detected;

        public Area2DOnArea2D(Area2D detected, Area2D origin) {
            Detected = detected;
            Origin = origin;
        }
    }

    public abstract class Area2DOnArea2DListener : GodotFilterListener<Area2DOnArea2D> {
        protected Area2DOnArea2DListener(string name, Node owner, Node filter) : base(name, owner, filter) {
        }
    }

    public class Area2DOnArea2DListenerDelegate : GodotFilterListenerDelegate<Area2DOnArea2D> {
        public Area2DOnArea2DListenerDelegate(string name, Node owner, Node filter, ExecuteMethod executeMethod) :
            base(name, owner, filter, executeMethod) {
        }
    }

    public class Area2DOnArea2DStatus {
        private Node _owner;

        public Area2DOnArea2DStatus(Node owner) {
            _owner = owner;
        }

        public bool IsOverlapping { get; protected internal set; }

        public bool IsDisposed() {
            return GodotTools.IsDisposed(_owner);
        }
    }

    /**
     * The topic listen for all signals of area_entered/area_exited in all the Area2D added by the method AddArea2D
     * To receive these event, subscribe to them using the Subscribe method.
     *
     * All overlapping from any other Area2D with the areas added will be published to all subscribed listeners.
     *
     * If we only need a bool flag to know if the overlap happened instead of create listeners, use the
     * StatusSubscriber. It will return a Area2DOnArea2DStatus where the internal variable IsOverlapping will be
     * updated by the events in real time.
     */
    public class Area2DOnArea2DTopic : Node {
        private GodotMulticastTopic<Area2DOnArea2D> _enterTopic;
        private GodotMulticastTopic<Area2DOnArea2D> _exitTopic;

        public GodotMulticastTopic<Area2DOnArea2D> EnterTopic =>
            _enterTopic ??= new GodotMulticastTopic<Area2DOnArea2D>($"{Name}_AreaEntered");

        public GodotMulticastTopic<Area2DOnArea2D> ExitTopic =>
            _exitTopic ??= new GodotMulticastTopic<Area2DOnArea2D>($"{Name}_AreaExited");

        public string Name { get; }

        public Area2DOnArea2DTopic(string name) {
            Name = name;
        }

        public void AddArea2D(Area2D areaToListen) {
            var binds = new Array { areaToListen };
            areaToListen.Connect(GodotConstants.GODOT_SIGNAL_area_entered, this, nameof(_AreaEntered), binds);
            areaToListen.Connect(GodotConstants.GODOT_SIGNAL_area_exited, this, nameof(_AreaExited), binds);
        }

        public void Subscribe(string name, Node owner, Area2D filter,
            GodotFilterListenerDelegate<Area2DOnArea2D>.ExecuteMethod enterMethod,
            GodotFilterListenerDelegate<Area2DOnArea2D>.ExecuteMethod exitMethod = null) {
            if (enterMethod != null) {
                EnterTopic.Subscribe(
                    new GodotFilterListenerDelegate<Area2DOnArea2D>(name, owner, filter, enterMethod));
            }
            if (exitMethod != null) {
                ExitTopic.Subscribe(
                    new GodotFilterListenerDelegate<Area2DOnArea2D>(name, owner, filter, exitMethod));
            }
        }

        public Area2DOnArea2DStatus StatusSubscriber(string name, Node owner, Area2D filter) {
            var status = new Area2DOnArea2DStatus(owner);
            EnterTopic.Subscribe(
                new GodotFilterListenerDelegate<Area2DOnArea2D>(name, owner, filter,
                    delegate(Area2DOnArea2D @event) { status.IsOverlapping = true; }));
            ExitTopic.Subscribe(
                new GodotFilterListenerDelegate<Area2DOnArea2D>(name, owner, filter,
                    delegate(Area2DOnArea2D @event) { status.IsOverlapping = false; }));
            return status;
        }

        public void Subscribe(GodotListener<Area2DOnArea2D> enterListener,
            GodotListener<Area2DOnArea2D> exitListener = null) {
            EnterTopic.Subscribe(enterListener);
            ExitTopic.Subscribe(exitListener);
        }

        public void _AreaEntered(Area2D detected, Area2D origin) {
            _enterTopic?.Publish(new Area2DOnArea2D(detected, origin));
        }

        public void _AreaExited(Area2D detected, Area2D origin) {
            _exitTopic?.Publish(new Area2DOnArea2D(detected, origin));
        }

        public void ClearListeners() {
            EnterTopic.EventListeners.Clear();
            ExitTopic.EventListeners.Clear();
        }
    }
}