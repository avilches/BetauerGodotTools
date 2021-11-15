namespace Tools {
    public class Clock {
        public float Elapsed { get; private set; } = 0;
        public bool Stopped { get; private set; } = true;
        public float Max { get; private set; } = float.MaxValue;

        public Clock() {
            Stop();
        }

        public Clock Start() {
            Elapsed = 0;
            Stopped = false;
            return this;
        }

        public Clock Add(float add) {
            if (!Stopped) Elapsed += add;
            return this;
        }

        public Clock Finish(float finish) {
            Max = finish;
            return this;
        }

        public bool IsFinished() => Elapsed > Max;

        public Clock Pause() {
            Stopped = true;
            return this;
        }

        public Clock Resume() {
            Stopped = false;
            return this;
        }

        public Clock Stop() {
            Elapsed = 0;
            Stopped = true;
            return this;
        }
    }
}