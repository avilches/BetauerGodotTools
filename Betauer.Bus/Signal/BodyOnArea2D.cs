using Betauer.Core.Signal;
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
                area2D.OnBodyEntered(body => OnEnter(area2D, body));
                area2D.OnBodyExited(body => OnExit(area2D, body));
                return this;
            }
        }

        public class Status : SignalStatus<Area2D, Node, Node> {
            protected override Node Extract(Node signalArgs) {
                return signalArgs;
            }
            
            public Status Connect(Area2D area2D) {
                area2D.OnBodyEntered(target => OnEnter(area2D, target));
                area2D.OnBodyExited(target => OnExit(area2D, target));
                return this;
            }
        }
    }

}