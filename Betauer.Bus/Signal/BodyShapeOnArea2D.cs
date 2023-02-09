using Betauer.Core.Signal;
using Godot;

namespace Betauer.Bus.Signal {
    public static class BodyShapeOnArea2D {
        public class Collection : SignalCollection<Area2D, (Rid, Node, int, int), Node> {
            
            public Collection(string? name = null) : base(name) {
            }

            public bool IsOverlapping => Size() > 0;

            protected override Node Extract((Rid, Node, int, int) signalArgs) {
                return signalArgs.Item2;
            }

            public Collection Connect(Area2D area2D) {
                area2D.OnBodyShapeEntered((rid, node, bodyShapeIndex, localShapeIndex) => OnEnter(area2D, (rid, node, bodyShapeIndex, localShapeIndex)));
                area2D.OnBodyShapeExited((rid, node, bodyShapeIndex, localShapeIndex) => OnExit(area2D, (rid, node, bodyShapeIndex, localShapeIndex)));
                return this;
            }
        }

        public class Status : SignalStatus<Area2D, (Rid, Node, int, int), Node> {
            protected override Node Extract((Rid, Node, int, int) signalArgs) {
                return signalArgs.Item2;
            }
            
            public Status Connect(Area2D area2DFrom) {
                area2DFrom.OnBodyShapeEntered((rid, node, bodyShapeIndex, localShapeIndex) => OnEnter(area2DFrom, (rid, node, bodyShapeIndex, localShapeIndex)));
                area2DFrom.OnBodyShapeExited((rid, node, bodyShapeIndex, localShapeIndex) => OnExit(area2DFrom, (rid, node, bodyShapeIndex, localShapeIndex)));
                return this;
            }
        }
    }

}