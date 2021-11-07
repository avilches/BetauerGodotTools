using Godot;
using Godot.Collections;

namespace Tools.Bus.Topics {
    public class BodyOnArea2D : IGodotNodeEvent {
        public readonly Node From;
        public readonly Area2D Area2D;

        public Node Filter => From;

        public BodyOnArea2D(Node from, Area2D area2D) {
            From = from;
            Area2D = area2D;
        }
    }

    public abstract class BodyOnArea2DListener : GodotListener<BodyOnArea2D> {
        protected BodyOnArea2DListener(string name, Node owner, Node filter) : base(name, owner, filter) {
        }
    }

    public class BodyOnArea2DListenerDelegate : GodotListenerDelegate<BodyOnArea2D> {
        public BodyOnArea2DListenerDelegate(string name, Node owner, Node filter, ExecuteMethod executeMethod) :
            base(name, owner, filter, executeMethod) {
        }
    }

    /**
     * The topic listen for all signals of body_entered and body_exited in all the Area2D added by the method AddArea2D
     * To receive this event, subscribe to them.
     */
    public class BodyOnArea2DTopic : Node {
        private GodotMulticastTopic<BodyOnArea2D> _enterTopic;
        private GodotMulticastTopic<BodyOnArea2D> _exitTopic;

        public GodotMulticastTopic<BodyOnArea2D> EnterTopic =>
            _enterTopic ??= new GodotMulticastTopic<BodyOnArea2D>($"{Name}_BodyEntered");

        public GodotMulticastTopic<BodyOnArea2D> ExitTopic =>
            _exitTopic ??= new GodotMulticastTopic<BodyOnArea2D>($"{Name}_BodyExited");

        public string Name { get; }

        public BodyOnArea2DTopic(string name) {
            Name = name;
        }

        public void AddArea2D(Area2D area2D) {
            var binds = new Array { area2D };
            area2D.Connect(GodotConstants.GODOT_SIGNAL_body_entered, this, nameof(_BodyEntered),
                binds);
            area2D.Connect(GodotConstants.GODOT_SIGNAL_body_exited, this, nameof(_BodyExited),
                binds);
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

        public void Subscribe(string name, Node owner, Node filter,
            GodotListenerDelegate<BodyOnArea2D>.ExecuteMethod enterMethod,
            GodotListenerDelegate<BodyOnArea2D>.ExecuteMethod exitMethod = null) {
            if (enterMethod != null)
                EnterTopic.Subscribe(new GodotListenerDelegate<BodyOnArea2D>(name, owner, filter, enterMethod));
            if (exitMethod != null)
                ExitTopic.Subscribe(new GodotListenerDelegate<BodyOnArea2D>(name, owner, filter, exitMethod));
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
    }
}