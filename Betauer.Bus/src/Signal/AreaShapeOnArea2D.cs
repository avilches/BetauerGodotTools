using Godot;

namespace Betauer.Bus.Signal {
    public static class AreaShapeOnArea2D {
        public class Collection : SignalCollection<Area2D, (Rid, Node, long, long), Node> {
            
            public Collection(string? name = null) : base(name) {
            }

            public bool IsOverlapping => Size() > 0;

            protected override Node Extract((Rid, Node, long, long) signalArgs) {
                return signalArgs.Item2;
            }

            public Collection Connect(Area2D area2D) {
                area2D.AreaShapeEntered += (rid, node, areaShapeIndex, localShapeIndex) => OnEnter(area2D, (rid, node, areaShapeIndex, localShapeIndex));
                area2D.AreaShapeExited += (rid, node, areaShapeIndex, localShapeIndex) => OnExit(area2D, (rid, node, areaShapeIndex, localShapeIndex));
                return this;
            }
        }

        public class Status : SignalStatus<Area2D, (Rid, Node, long, long), Node> {
            protected override Node Extract((Rid, Node, long, long) signalArgs) {
                return signalArgs.Item2;
            }
            
            public Status Connect(Area2D area2DFrom) {
                area2DFrom.AreaShapeEntered += (rid, node, areaShapeIndex, localShapeIndex) => OnEnter(area2DFrom, (rid, node, areaShapeIndex, localShapeIndex));
                area2DFrom.AreaShapeExited += (rid, node, areaShapeIndex, localShapeIndex) => OnExit(area2DFrom, (rid, node, areaShapeIndex, localShapeIndex));
                return this;
            }
        }
    }

}