using Godot;

namespace Tools {
    public abstract class Timer {
        public float Elapsed { get; protected set; } = 0;
        public bool Stopped { get; private set; } = true;
        public float Alarm { get; private set; } = float.MaxValue;

        public Timer Start() {
            Stopped = false;
            return this;
        }

        public Timer Stop() {
            Stopped = true;
            return this;
        }

        public Timer Reset() {
            Elapsed = 0;
            return this;
        }

        public Timer ClearAlarm() {
            Alarm = float.MaxValue;
            return this;
        }

        public Timer SetAlarm(float finish) {
            Alarm = finish;
            return this;
        }

        public bool IsAlarm() => Elapsed > Alarm;

        protected Timer _Update(float delta) {
            if (!Stopped) {
                Elapsed += delta;
            }
            return this;
        }
    }

    public class ManualTimer : Timer {
        public Timer Update(float delta) => _Update(delta);
    }

    public class AutoTimer : Timer {
        public AutoTimer(Node node) => node.AddChild(new NodeTimer(this));

        private class NodeTimer : Node {
            private readonly AutoTimer _autoTimer;
            public NodeTimer(AutoTimer autoTimer) => _autoTimer = autoTimer;
            public override void _PhysicsProcess(float delta) => _autoTimer._Update(delta);
        }
    }


}