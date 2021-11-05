using Godot;

namespace Tools.Events {

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
        public BodyOnArea2DEnterListener(Node body) : base(body) {
        }
    }

    public class BodyOnArea2DEnterListenerDelegate : GodotNodeListenerDelegate<BodyOnArea2D> {
        public BodyOnArea2DEnterListenerDelegate(Node body, ExecuteMethod executeMethod) : base(body, executeMethod) {
        }
    }
}