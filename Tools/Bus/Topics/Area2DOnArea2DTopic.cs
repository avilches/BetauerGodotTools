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

    public abstract class Area2DOnArea2DListener : GodotListener<Area2DOnArea2D> {
        protected Area2DOnArea2DListener(string name, Node owner, Node filter) : base(name, owner, filter) {
        }
    }

    public class Area2DOnArea2DListenerDelegate : GodotListenerDelegate<Area2DOnArea2D> {
        public Area2DOnArea2DListenerDelegate(string name, Node owner, Node filter, ExecuteMethod executeMethod) :
            base(name, owner, filter, executeMethod) {
        }
    }

    /**
     * The topic listen for all signals of body_entered and body_exited in all the Area2D added by the method AddArea2D
     * To receive this event, subscribe to them. In order to filter the events on
     */
    public class Area2DOnArea2DTopic : Node {
        private GodotMulticastTopic<Area2DOnArea2D> _enterTopic;
        private GodotMulticastTopic<Area2DOnArea2D> _exitTopic;

        private GodotMulticastTopic<Area2DOnArea2D> EnterTopic =>
            _enterTopic ??= new GodotMulticastTopic<Area2DOnArea2D>($"{Name}_AreaEntered");

        private GodotMulticastTopic<Area2DOnArea2D> ExitTopic =>
            _exitTopic ??= new GodotMulticastTopic<Area2DOnArea2D>($"{Name}_AreaExited");

        public string Name { get; }

        public Area2DOnArea2DTopic(string name) {
            Name = name;
        }

        public void AddArea2D(Area2D area2D) {
            area2D.Connect(GodotConstants.GODOT_SIGNAL_area_entered, this, nameof(_AreaEntered),
                new Array { area2D });
            area2D.Connect(GodotConstants.GODOT_SIGNAL_area_exited, this, nameof(_AreaExited),
                new Array { area2D });
        }

        public void Subscribe(string name, Node owner, Area2D filter,
            GodotListenerDelegate<Area2DOnArea2D>.ExecuteMethod enterMethod,
            GodotListenerDelegate<Area2DOnArea2D>.ExecuteMethod exitMethod = null) {
            if (enterMethod != null) {
                EnterTopic.Subscribe(new GodotListenerDelegate<Area2DOnArea2D>(name, owner, filter, enterMethod));
            }
            if (exitMethod != null) {
                ExitTopic.Subscribe(new GodotListenerDelegate<Area2DOnArea2D>(name, owner, filter, exitMethod));
            }
        }

        public void Subscribe(GodotListener<Area2DOnArea2D> enterListener,
            GodotListener<Area2DOnArea2D> exitListener = null) {
            EnterTopic.Subscribe(enterListener);
            ExitTopic.Subscribe(exitListener);
        }

        public void _AreaEntered(Area2D player, Area2D stageEnteredArea2D) {
            _enterTopic?.Publish(new Area2DOnArea2D(player, stageEnteredArea2D));
        }

        public void _AreaExited(Area2D player, Area2D stageExitedArea2D) {
            _exitTopic?.Publish(new Area2DOnArea2D(player, stageExitedArea2D));
        }
    }
}