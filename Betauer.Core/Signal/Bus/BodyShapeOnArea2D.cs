using Godot;

namespace Betauer.Signal.Bus {
    public static class BodyShapeOnArea2D {
        public class Collection : SignalCollection<Area2D, (RID, Node, int, int), Node> {
            
            public Collection(string? name = null) : base(name) {
            }

            public bool IsOverlapping => Size() > 0;

            protected override Node Extract((RID, Node, int, int) signalParams) {
                return signalParams.Item2;
            }

            public Collection Connect(Area2D area2D) {
                area2D.OnBodyShapeEntered((rid, node, bodyShapeIndex, localShapeIndex) => OnEnter(area2D, (rid, node, bodyShapeIndex, localShapeIndex)));
                area2D.OnBodyShapeExited((rid, node, bodyShapeIndex, localShapeIndex) => OnExit(area2D, (rid, node, bodyShapeIndex, localShapeIndex)));
                return this;
            }
        }

        public class Status : SignalStatus<Area2D, (RID, Node, int, int), Node> {
            protected override Node Extract((RID, Node, int, int) signalParams) {
                return signalParams.Item2;
            }
            
            public Status Connect(Area2D area2DFrom) {
                area2DFrom.OnBodyShapeEntered((rid, node, bodyShapeIndex, localShapeIndex) => OnEnter(area2DFrom, (rid, node, bodyShapeIndex, localShapeIndex)));
                area2DFrom.OnBodyShapeExited((rid, node, bodyShapeIndex, localShapeIndex) => OnExit(area2DFrom, (rid, node, bodyShapeIndex, localShapeIndex)));
                return this;
            }
        }
    }

    public static class BodyShapeOnArea2DEntered {
        public class Multicast : Multicast<Area2D, (RID, Node, int, int), Node> {
            public Multicast(string name) : base(name) {
            }

            public override SignalHandler Connect(Area2D area2D) {
                // Console.WriteLine(Name + ":ListenSignalsOf " + area2DFrom.ToStringSafe());
                return area2D.OnBodyShapeEntered((rid, node, bodyShapeIndex, localShapeIndex) => Emit(area2D, (rid, node, bodyShapeIndex, localShapeIndex)));
            }

            protected override bool Matches((RID, Node, int, int) e, Node detect) {
                return e.Item2 == detect;
            }
        }

        public class Unicast : Unicast<Area2D, (RID, Node, int, int), Node> {
            public Unicast(string name) : base(name) {
            }

            public override SignalHandler Connect(Area2D area2D) {
                // Console.WriteLine(Name + ":ListenSignalsOf " + area2DFrom.ToStringSafe());
                return area2D.OnBodyShapeEntered((rid, node, bodyShapeIndex, localShapeIndex) => Emit(area2D, (rid, node, bodyShapeIndex, localShapeIndex)));
            }

            protected override bool Matches((RID, Node, int, int) e, Node detect) {
                return e.Item2 == detect;
            }
        }
    }

    public static class BodyShapeOnArea2DExited {
        public class Multicast : Multicast<Area2D, (RID, Node, int, int), Node> {
            public Multicast(string name) : base(name) {
            }

            public override SignalHandler Connect(Area2D area2D) {
                // Console.WriteLine(Name + ":ListenSignalsOf " + area2DFrom.ToStringSafe());
                return area2D.OnBodyShapeExited((rid, node, bodyShapeIndex, localShapeIndex) => Emit(area2D, (rid, node, bodyShapeIndex, localShapeIndex)));
            }

            protected override bool Matches((RID, Node, int, int) e, Node detect) {
                return e.Item2 == detect;
            }
        }

        public class Unicast : Unicast<Area2D, (RID, Node, int, int), Node> {
            public Unicast(string name) : base(name) {
            }

            public override SignalHandler Connect(Area2D area2D) {
                // Console.WriteLine(Name + ":ListenSignalsOf " + area2DFrom.ToStringSafe());
                return area2D.OnBodyShapeExited((rid, node, bodyShapeIndex, localShapeIndex) => Emit(area2D, (rid, node, bodyShapeIndex, localShapeIndex)));
            }

            protected override bool Matches((RID, Node, int, int) e, Node detect) {
                return e.Item2 == detect;
            }
        }
    }
}