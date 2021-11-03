using Godot;

namespace Veronenger.Game.Tools.Events {

    public class BodyOnArea2D : EventFromNode {
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

    public abstract class Area2DEnterListener : NodeFromListener<BodyOnArea2D> {
        public Area2DEnterListener(Node body) : base(body) {
        }
    }

    public class Area2DEnterListenerDelegate : NodeFromListenerDelegate<BodyOnArea2D> {
        public Area2DEnterListenerDelegate(Node body, ExecuteMethod executeMethod) : base(body, executeMethod) {
        }
    }
}