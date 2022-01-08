using Godot;
using Godot.Collections;

namespace Betauer.Bus.Topics {
    public class BodyOnArea2D : IGodotFilterEvent {
        public readonly Node Detected;
        public Node Origin { get; }
        public Node Filter => Detected;

        public BodyOnArea2D(Node detected, Area2D origin) {
            Detected = detected;
            Origin = origin;
        }
    }

    public abstract class BodyOnArea2DListener : GodotFilterListener<BodyOnArea2D> {
        protected BodyOnArea2DListener(string name, Node owner, Node filter) : base(name, owner, filter) {
        }
    }

    public class BodyOnArea2DListenerDelegate : GodotFilterListenerDelegate<BodyOnArea2D> {
        public BodyOnArea2DListenerDelegate(string name, Node owner, Node filter, ExecuteMethod executeMethod) :
            base(name, owner, filter, executeMethod) {
        }
    }

    public class BodyOnArea2DStatus {
        private Node _owner;

        public BodyOnArea2DStatus(Node owner) {
            _owner = owner;
        }

        public bool IsOverlapping { get; protected internal set; }

        public bool IsDisposed() {
            return !Object.IsInstanceValid(_owner);
        }
    }

    /**
     * The topic listen for all signals of body_entered/body_exited in all the Area2D added by the method AddArea2D
     * To receive these event, subscribe to them using the Subscribe method.
     *
     * All overlapping from any Body/TileMap with the areas added will be published to all subscribed listeners.
     *
     * If we only need a bool flag to know if the overlap happened instead of create listeners, use the
     * StatusSubscriber. It will return a BodyOnArea2DStatus where the internal variable IsOverlapping will be
     * updated by the events in real time.
     */
    public class BodyOnArea2DTopic : Object /* needed to connect to signals */  {
        private GodotTopic<BodyOnArea2D> _enterTopic;
        private GodotTopic<BodyOnArea2D> _exitTopic;

        public GodotTopic<BodyOnArea2D> EnterTopic =>
            _enterTopic ??= new GodotTopic<BodyOnArea2D>($"{Name}_BodyEntered");

        public GodotTopic<BodyOnArea2D> ExitTopic =>
            _exitTopic ??= new GodotTopic<BodyOnArea2D>($"{Name}_BodyExited");

        public string Name { get; }

        public BodyOnArea2DTopic(string name) {
            Name = name;
        }

        public void ListenSignalsOf(Area2D area2DToListen) {
            var binds = new Array { area2DToListen };
            area2DToListen.Connect(GodotConstants.GODOT_SIGNAL_body_entered, this, nameof(_BodyEntered), binds);
            area2DToListen.Connect(GodotConstants.GODOT_SIGNAL_body_exited, this, nameof(_BodyExited), binds);
        }

        /*
        Old way, using delegates
        public delegate void BodyOnArea2DSignalMethod(Node body, Area2D area2D);
        public void AddArea2D(Area2D area2D) {
            ListenArea2DCollisionsWithBodies(area2D, _BodyEntered, _BodyExited);
        }

        public static void ListenArea2DCollisionsWithBodies(Area2D area2D, BodyOnArea2DSignalMethod enter,
            BodyOnArea2DSignalMethod exit = null) {
            if (enter.Filter is Object nodeEnter) {
                area2D.Connect(GodotConstants.GODOT_SIGNAL_body_entered, nodeEnter, enter.Method.Name,
                    new Array { area2D });
                if (exit != null && enter.Filter is Object nodeExit) {
                    area2D.Connect(GodotConstants.GODOT_SIGNAL_body_exited, nodeExit, exit.Method.Name,
                        new Array { area2D });
                }
            }
        }
        */

        public void Subscribe(string name, Node owner, PhysicsBody2D filter,
            GodotFilterListenerDelegate<BodyOnArea2D>.ExecuteMethod enterMethod,
            GodotFilterListenerDelegate<BodyOnArea2D>.ExecuteMethod exitMethod = null) {
            _Subscribe(name, owner, filter, enterMethod, exitMethod);
        }

        public void Subscribe(string name, Node owner, TileMap filter,
            GodotFilterListenerDelegate<BodyOnArea2D>.ExecuteMethod enterMethod,
            GodotFilterListenerDelegate<BodyOnArea2D>.ExecuteMethod exitMethod = null) {
            _Subscribe(name, owner, filter, enterMethod, exitMethod);
        }

        private void _Subscribe(string name, Node owner, Node filter,
            GodotFilterListenerDelegate<BodyOnArea2D>.ExecuteMethod enterMethod,
            GodotFilterListenerDelegate<BodyOnArea2D>.ExecuteMethod exitMethod) {
            if (enterMethod != null)
                EnterTopic.Subscribe(
                    new GodotFilterListenerDelegate<BodyOnArea2D>(name, owner, filter, enterMethod));
            if (exitMethod != null)
                ExitTopic.Subscribe(new GodotFilterListenerDelegate<BodyOnArea2D>(name, owner, filter, exitMethod));
        }

        public BodyOnArea2DStatus StatusSubscriber(string name, Node owner, PhysicsBody2D filter) {
            return _StatusSubscriber(name, owner, filter);
        }

        public BodyOnArea2DStatus StatusSubscriber(string name, Node owner, TileMap filter) {
            return _StatusSubscriber(name, owner, filter);
        }

        private BodyOnArea2DStatus _StatusSubscriber(string name, Node owner, Node filter) {
            var status = new BodyOnArea2DStatus(owner);
            EnterTopic.Subscribe(
                new GodotFilterListenerDelegate<BodyOnArea2D>(name, owner, filter,
                    delegate(BodyOnArea2D @event) { status.IsOverlapping = true; }));
            ExitTopic.Subscribe(
                new GodotFilterListenerDelegate<BodyOnArea2D>(name, owner, filter,
                    delegate(BodyOnArea2D @event) { status.IsOverlapping = false; }));
            return status;
        }

        public void Subscribe(GodotListener<BodyOnArea2D> enterListener,
            GodotListener<BodyOnArea2D> exitListener = null) {
            EnterTopic.Subscribe(enterListener);
            ExitTopic.Subscribe(exitListener);
        }

        private void _BodyEntered(Node body, Area2D area2D) {
            EnterTopic?.Publish(new BodyOnArea2D(body, area2D));
        }

        private void _BodyExited(Node body, Area2D area2D) {
            ExitTopic?.Publish(new BodyOnArea2D(body, area2D));
        }

        public void ClearListeners() {
            EnterTopic.EventListeners.Clear();
            ExitTopic.EventListeners.Clear();
        }

        protected override void Dispose(bool disposing) {
            if (!disposing) GD.Print("Shutdown disposing "+GetType());
            base.Dispose(disposing);
        }

    }
}