using Godot;
using Godot.Collections;

namespace Tools.Bus.Topics {
    public class Area2DOnArea2D : IGodotNodeEvent {
        public readonly Area2D From;
        public readonly Area2D Area2D;

        public Node GetFilter() {
            return From;
        }

        public Area2DOnArea2D(Area2D from, Area2D area2D) {
            From = from;
            Area2D = area2D;
        }
    }

    public abstract class Area2DOnArea2DEnterListener : GodotNodeListener<Area2DOnArea2D> {
        protected Area2DOnArea2DEnterListener(string name, Node filter) : base(name, filter) {
        }
    }

    public class Area2DOnArea2DEnterListenerDelegate : GodotNodeListenerDelegate<Area2DOnArea2D> {
        public Area2DOnArea2DEnterListenerDelegate(string name, Node filter, ExecuteMethod executeMethod) : base(name,
            filter, executeMethod) {
        }
    }

    /**
     * The topic listen for all signals of body_entered and body_exited in all the Area2D added by the method AddArea2D
     * To receive this event, subscribe to them. In order to filter the events on
     */
    public class Area2DOnArea2DTopic : Node {
        private GodotNodeMulticastTopic<Area2DOnArea2D> _enterTopic;
        private GodotNodeMulticastTopic<Area2DOnArea2D> _exitTopic;

        private GodotNodeMulticastTopic<Area2DOnArea2D> EnterTopic =>
            _enterTopic ??= new GodotNodeMulticastTopic<Area2DOnArea2D>($"{Name}_AreaEntered");

        private GodotNodeMulticastTopic<Area2DOnArea2D> ExitTopic =>
            _exitTopic ??= new GodotNodeMulticastTopic<Area2DOnArea2D>($"{Name}_AreaExited");

        public string Name { get; }

        public Area2DOnArea2DTopic(string name) {
            Name = name;
        }

        public void AddArea2D(Area2D area2D, CollisionShape2D stageCollisionShape2D) {
            area2D.Connect(GodotConstants.GODOT_SIGNAL_area_entered, this, nameof(_AreaEntered),
                new Array { area2D, stageCollisionShape2D.Shape });
            area2D.Connect(GodotConstants.GODOT_SIGNAL_area_exited, this, nameof(_AreaExited),
                new Array { area2D });
        }

        public void Subscribe(string name, Area2D filter,
            GodotNodeListenerDelegate<Area2DOnArea2D>.ExecuteMethod enterMethod,
            GodotNodeListenerDelegate<Area2DOnArea2D>.ExecuteMethod exitMethod = null) {
            if (enterMethod != null) {
                EnterTopic.Subscribe(new GodotNodeListenerDelegate<Area2DOnArea2D>(name, filter, enterMethod));
            }
            if (exitMethod != null) {
                ExitTopic.Subscribe(new GodotNodeListenerDelegate<Area2DOnArea2D>(name, filter, exitMethod));
            }
        }

        public void Subscribe(GodotNodeListener<Area2DOnArea2D> enterListener,
            GodotNodeListener<Area2DOnArea2D> exitListener = null) {
            EnterTopic.Subscribe(enterListener);
            ExitTopic.Subscribe(exitListener);
        }

        public void _AreaEntered(Area2D player, Area2D stageEnteredArea2D, RectangleShape2D shape2D) {
            _enterTopic?.Publish(new Area2DOnArea2D(player, stageEnteredArea2D));
        }

        public void _AreaExited(Area2D player, Area2D stageExitedArea2D) {
            _exitTopic?.Publish(new Area2DOnArea2D(player, stageExitedArea2D));
        }
    }
}