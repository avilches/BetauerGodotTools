using Godot;
using Godot.Collections;

namespace Tools.Bus.Topics {
    public class Area2DOnArea2D : IGodotNodeEvent {
        public readonly Area2D Detected;
        public Node Origin { get;}
        public Node Filter => Detected;

        public Area2DOnArea2D(Area2D detected, Area2D origin) {
            Detected = detected;
            Origin = origin;
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
     * The topic listen for all signals of area_entered and area_exited in all the Area2D added by the method AddArea2D
     * To receive this event, subscribe to them.
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

        public void AddArea2D(Area2D areaToListen) {
            var binds = new Array { areaToListen };
            areaToListen.Connect(GodotConstants.GODOT_SIGNAL_area_entered, this, nameof(_AreaEntered), binds);
            areaToListen.Connect(GodotConstants.GODOT_SIGNAL_area_exited, this, nameof(_AreaExited), binds);
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

        public void _AreaEntered(Area2D detected, Area2D origin) {
            _enterTopic?.Publish(new Area2DOnArea2D(detected, origin));
        }

        public void _AreaExited(Area2D detected, Area2D origin) {
            _exitTopic?.Publish(new Area2DOnArea2D(detected, origin));
        }
    }
}