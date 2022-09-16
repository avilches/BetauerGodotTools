using Godot;

namespace Betauer.Signal.Bus {
    public static class BodyOnArea2D {
        public class Collection : SignalCollection<Area2D, Node, Node> {
            
            public Collection(string? name = null) : base(name) {
            }

            public bool IsOverlapping => Size() > 0;

            protected override Node Extract(Node signalParams) {
                return signalParams;
            }

            public Collection Connect(Area2D area2D) {
                area2D.OnBodyEntered(body => OnEnter(area2D, body));
                area2D.OnBodyExited(body => OnExit(area2D, body));
                return this;
            }
        }

        public class Status : SignalStatus<Area2D, Node, Node> {
            protected override Node Extract(Node signalParams) {
                return signalParams;
            }
            
            public Status Connect(Area2D area2D) {
                area2D.OnBodyEntered(target => OnEnter(area2D, target));
                area2D.OnBodyExited(target => OnExit(area2D, target));
                return this;
            }
        }
    }

    public static class BodyOnArea2DEntered {
        public class Multicast : Multicast<Area2D, Node, Node> {
            public Multicast(string name) : base(name) {
            }

            public override SignalHandler Connect(Area2D area2D) {
                return area2D.OnBodyEntered(target => Emit(area2D, target));
            }

            protected override bool Matches(Node e, Node detect) {
                return e == detect;
            }
        }

        public class Unicast : Unicast<Area2D, Node, Node> {
            public Unicast(string name) : base(name) {
            }

            public override SignalHandler Connect(Area2D area2D) {
                return area2D.OnBodyEntered(target => Emit(area2D, target));
            }

            protected override bool Matches(Node e, Node detect) {
                return e == detect;
            }
        }
    }

    public static class BodyOnArea2DExited {
        public class Multicast : Multicast<Area2D, Node, Node> {
            public Multicast(string name) : base(name) {
            }

            public override SignalHandler Connect(Area2D area2D) {
                return area2D.OnBodyExited(target => Emit(area2D, target));
            }

            protected override bool Matches(Node e, Node detect) {
                return e == detect;
            }
        }

        public class Unicast : Unicast<Area2D, Node, Node> {
            public Unicast(string name) : base(name) {
            }

            public override SignalHandler Connect(Area2D area2D) {
                return area2D.OnBodyExited(target => Emit(area2D, target));
            }

            protected override bool Matches(Node e, Node detect) {
                return e == detect;
            }
        }
    }
}