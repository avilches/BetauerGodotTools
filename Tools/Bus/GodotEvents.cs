using Godot;
using Godot.Collections;

namespace Tools.Bus {
    public class BodyOnArea2D : IGodotNodeEvent {
        public readonly Node Body;
        public readonly Area2D Area2D;

        public Node GetFrom() {
            return Body;
        }

        public BodyOnArea2D(Node body, Area2D area2D) {
            Body = body;
            Area2D = area2D;
        }
    }

    public abstract class BodyOnArea2DEnterListener : GodotNodeListener<BodyOnArea2D> {
        protected BodyOnArea2DEnterListener(string name, Node body) : base(name, body) {
        }
    }

    public class BodyOnArea2DEnterListenerDelegate : GodotNodeListenerDelegate<BodyOnArea2D> {
        public BodyOnArea2DEnterListenerDelegate(string name, Node body, ExecuteMethod executeMethod) : base(name, body, executeMethod) {
        }
    }

    /**
     * The topic listen for all signals of body_entered and body_exited in all the Area2D added by the method AddArea2D
     * To receive this event, subscribe to them. In order to filter the events on
     */
    public class BodyOnArea2DTopic : Node {
        private GodotNodeMulticastTopic<BodyOnArea2D> _enterTopic;
        private GodotNodeMulticastTopic<BodyOnArea2D> _exitTopic;
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
            if (enter.Target is Object nodeEnter) {
                area2D.Connect(GodotConstants.GODOT_SIGNAL_body_entered, nodeEnter, enter.Method.Name,
                    new Array { area2D });
                if (exit != null && enter.Target is Object nodeExit) {
                    area2D.Connect(GodotConstants.GODOT_SIGNAL_body_exited, nodeExit, exit.Method.Name,
                        new Array { area2D });
                }
            }
        }
        */

        public void Subscribe(string name, Node body, GodotNodeListenerDelegate<BodyOnArea2D>.ExecuteMethod enterMethod,
            GodotNodeListenerDelegate<BodyOnArea2D>.ExecuteMethod exitMethod = null) {
            GodotNodeListener<BodyOnArea2D> enterListener = null;
            GodotNodeListener<BodyOnArea2D> exitListener = null;
            if (enterMethod != null)
                enterListener = new GodotNodeListenerDelegate<BodyOnArea2D>(name, body, enterMethod);
            if (exitMethod != null) exitListener = new GodotNodeListenerDelegate<BodyOnArea2D>(name, body, exitMethod);
            if (enterListener != null && exitListener != null) {
                Subscribe(enterListener, exitListener);
            }
        }

        public void Subscribe(GodotNodeListener<BodyOnArea2D> enterListener,
            GodotNodeListener<BodyOnArea2D> exitListener = null) {
            if (enterListener != null) {
                // Topic is crated only when there is at least one subscriber
                if (_enterTopic == null) _enterTopic = new GodotNodeMulticastTopic<BodyOnArea2D>($"{Name}_BodyEntered");
                _enterTopic.Subscribe(enterListener);
            }
            if (exitListener != null) {
                // Topic is crated only when there is at least one subscriber
                if (_exitTopic == null) _exitTopic = new GodotNodeMulticastTopic<BodyOnArea2D>($"{Name}_BodyExited");
                _exitTopic.Subscribe(exitListener);
            }
        }

        private void _BodyEntered(Node body, Area2D area2D) {
            _enterTopic?.Publish(new BodyOnArea2D(body, area2D));
        }

        private void _BodyExited(Node body, Area2D area2D) {
            _exitTopic?.Publish(new BodyOnArea2D(body, area2D));
        }
    }

}