using System;
using Godot;
using Array = Godot.Collections.Array;
using Object = Godot.Object;

namespace Betauer.Bus.Topics {
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

    public class Area2DOnArea2DListenerAction : GodotFilterListenerAction<Area2DOnArea2D> {
        public Area2DOnArea2DListenerAction(string name, Node owner, Node filter,
            Action<Area2DOnArea2D> actionWithEvent) :
            base(name, owner, filter, actionWithEvent) {
        }

        public Area2DOnArea2DListenerAction(string name, Node owner, Node filter, Action action) :
            base(name, owner, filter, action) {
        }
    }

    public class Area2DOnArea2DStatus {
        private Node _owner;

        public Area2DOnArea2DStatus(Node owner) {
            _owner = owner;
        }

        public bool JustHappened; // TODO it could be interesting to know so

        /*
         * JustHappened | IsOverlapping |
         * true         | true          | Overlap happens in current frame
         * false        | true          | Overlap happened in previous frame and continued in this one
         * true         | false         | Overlapped in the previous frame, but it's not overlapping any more
         * false        | false         | Overlapping is not happening now neither in the previous frame
         */
        public bool IsOverlapping { get; protected internal set; }
        public void EnableOverlapping() => IsOverlapping = true;
        public void DisableOverlapping() => IsOverlapping = false;

        public bool IsDisposed() {
            return !Object.IsInstanceValid(_owner);
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
    public class Area2DOnArea2DTopic : GodotObject /* needed to connect to signals */ {
        private GodotTopic<Area2DOnArea2D>? _enterTopic;
        private GodotTopic<Area2DOnArea2D>?_exitTopic;

        public GodotTopic<Area2DOnArea2D> EnterTopic =>
            _enterTopic ??= new GodotTopic<Area2DOnArea2D>($"{Name}_AreaEntered");

        public GodotTopic<Area2DOnArea2D> ExitTopic =>
            _exitTopic ??= new GodotTopic<Area2DOnArea2D>($"{Name}_AreaExited");

        public string Name { get; }

        public Area2DOnArea2DTopic(string name) {
            Name = name;
        }

        public override string ToString() {
            return base.ToString() + " " + Name;
        }

        public void ListenSignalsOf(Area2D areaToListen) {
            var binds = new Array { areaToListen };
            areaToListen.Connect(SignalExtensions.Area2D_AreaEnteredSignal, this, nameof(_AreaEntered), binds);
            areaToListen.Connect(SignalExtensions.Area2D_AreaExitedSignal, this, nameof(_AreaExited), binds);
        }

        public void Subscribe(string name, Node owner, Area2D filter,
            Action<Area2DOnArea2D>? enterMethod,
            Action<Area2DOnArea2D>? exitMethod = null) {
            if (enterMethod != null) {
                EnterTopic.Subscribe(new Area2DOnArea2DListenerAction(name, owner, filter, enterMethod));
            }
            if (exitMethod != null) {
                ExitTopic.Subscribe(new Area2DOnArea2DListenerAction(name, owner, filter, exitMethod));
            }
        }

        public Area2DOnArea2DStatus StatusSubscriber(string name, Node owner, Area2D filter) {
            var status = new Area2DOnArea2DStatus(owner);
            EnterTopic.Subscribe(new Area2DOnArea2DListenerAction(name, owner, filter, status.EnableOverlapping));
            ExitTopic.Subscribe(new Area2DOnArea2DListenerAction(name, owner, filter, status.DisableOverlapping));
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