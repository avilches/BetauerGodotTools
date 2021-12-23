using Godot;
using Godot.Collections;

namespace Betauer.Bus.Topics {
    public class Area2DShapeOnArea2D : IGodotFilterEvent {
        public readonly RID Area2dRid;
        public readonly Area2D Detected;
        public readonly int DetectedAreaShape;
        public Node Origin { get; }
        public readonly int OriginShape;

        public Node Filter => Detected;

        public Area2DShapeOnArea2D(RID area2dRid, Area2D detected, int detectedAreaShape, int originShape, Area2D origin) {
            Area2dRid = area2dRid;
            Detected = detected;
            DetectedAreaShape = detectedAreaShape;
            OriginShape = originShape;
            Origin = origin;
        }
    }

    public abstract class Area2DShapeOnArea2DListener : GodotFilterListener<Area2DShapeOnArea2D> {
        protected Area2DShapeOnArea2DListener(string name, Node owner, Node filter) : base(name, owner, filter) {
        }
    }

    public class Area2DShapeOnArea2DListenerDelegate : GodotFilterListenerDelegate<Area2DShapeOnArea2D> {
        public Area2DShapeOnArea2DListenerDelegate(string name, Node owner, Node filter, ExecuteMethod executeMethod) :
            base(name, owner, filter, executeMethod) {
        }
    }

    /**
     * The topic listen for all signals of area_entered and area_exited in all the Area2D added by the method AddArea2D
     * To receive this event, subscribe to them.
     */
    public class Area2DShapeOnArea2DTopic  : Object /* needed to connect to signals */ {
        private GodotTopic<Area2DShapeOnArea2D> _enterTopic;
        private GodotTopic<Area2DShapeOnArea2D> _exitTopic;

        private GodotTopic<Area2DShapeOnArea2D> EnterTopic =>
            _enterTopic ??= new GodotTopic<Area2DShapeOnArea2D>($"{Name}_AreaShapeEntered");

        private GodotTopic<Area2DShapeOnArea2D> ExitTopic =>
            _exitTopic ??= new GodotTopic<Area2DShapeOnArea2D>($"{Name}_AreaShapeExited");

        public string Name { get; }

        public Area2DShapeOnArea2DTopic(string name) {
            Name = name;
        }

        public void ListenSignalsOf(Area2D areaToListen) {
            areaToListen.Connect(GodotConstants.GODOT_SIGNAL_area_shape_entered, this, nameof(_AreaEntered),
                new Array { areaToListen });
            areaToListen.Connect(GodotConstants.GODOT_SIGNAL_area_shape_exited, this, nameof(_AreaExited),
                new Array { areaToListen });
        }

        public void Subscribe(string name, Node owner, Area2D filter,
            GodotFilterListenerDelegate<Area2DShapeOnArea2D>.ExecuteMethod enterMethod,
            GodotFilterListenerDelegate<Area2DShapeOnArea2D>.ExecuteMethod exitMethod = null) {
            if (enterMethod != null) {
                EnterTopic.Subscribe(new GodotFilterListenerDelegate<Area2DShapeOnArea2D>(name, owner, filter, enterMethod));
            }
            if (exitMethod != null) {
                ExitTopic.Subscribe(new GodotFilterListenerDelegate<Area2DShapeOnArea2D>(name, owner, filter, exitMethod));
            }
        }

        public void Subscribe(GodotListener<Area2DShapeOnArea2D> enterListener,
            GodotListener<Area2DShapeOnArea2D> exitListener = null) {
            EnterTopic.Subscribe(enterListener);
            ExitTopic.Subscribe(exitListener);
        }

        public void _AreaEntered(RID area2dRid, Area2D detected, int areaShape, int localShape, Area2D origin) {
            _enterTopic?.Publish(new Area2DShapeOnArea2D(area2dRid, detected, areaShape, localShape, origin));
        }

        public void _AreaExited(RID area2dRid, Area2D detected, int areaShape, int localShape, Area2D origin) {
            _exitTopic?.Publish(new Area2DShapeOnArea2D(area2dRid, detected, areaShape, localShape, origin));
        }
    }
}