using Godot;

namespace Betauer.Bus.Signal {
    public static class BodyOnArea2D {
        public class Collection : SignalCollection<Area2D, Node, Node> {
            
            public Collection(string? name = null) : base(name) {
            }

            public bool IsOverlapping => Size() > 0;

            protected override Node Extract(Node signalArgs) {
                return signalArgs;
            }

            public Collection Connect(Area2D area2D) {
                area2D.BodyEntered += body => OnEnter(area2D, body);
                area2D.BodyExited += body => OnExit(area2D, body);
                return this;
            }
        }

        public class Status : SignalStatus<Area2D, Node, Node> {
            protected override Node Extract(Node signalArgs) {
                return signalArgs;
            }
            
            public Status Connect(Area2D area2D) {
                area2D.BodyEntered += target => OnEnter(area2D, target);
                area2D.BodyExited += target => OnExit(area2D, target);
                return this;
            }
        }
    }

}