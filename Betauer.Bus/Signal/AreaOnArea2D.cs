using Godot;
   
namespace Betauer.Bus.Signal {
    public static class AreaOnArea2D {
        public class Collection : SignalCollection<Area2D, Area2D, Area2D> {

            public Collection(string? name = null) : base(name) {
            }

            public bool IsOverlapping => Size() > 0;

            protected override Area2D Extract(Area2D signalArgs) {
                return signalArgs;
            }

            public Collection Connect(Area2D area2D) {
                area2D.AreaEntered += target => OnEnter(area2D, target);
                area2D.AreaExited += target => OnExit(area2D, target);
                return this;
            }
        }

        public class Status : SignalStatus<Area2D, Area2D, Area2D> {
            protected override Area2D Extract(Area2D signalArgs) {
                return signalArgs;
            }
            
            public Status Connect(Area2D area2D) {
                area2D.AreaEntered += target => OnEnter(area2D, target);
                area2D.AreaExited += target => OnExit(area2D, target);
                return this;
            }
        }
    }
}