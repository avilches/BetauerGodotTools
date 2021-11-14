namespace Tools {
    public class Clock {
        public float Elapsed { get; private set; } = 0;
        public bool Paused { get; private set; } = false;
        public float Max { get; private set; } = float.MaxValue;

        public Clock() {
        }

        public Clock Start() {
            Elapsed = 0;
            Paused = false;
            return this;
        }

        public Clock Add(float add) {
            if (!Paused) Elapsed += add;
            return this;
        }

        public Clock Finish(float finish) {
            Max = finish;
            return this;
        }

        public bool IsFinished() => Elapsed > Max;

        public Clock Pause() {
            Paused = true;
            return this;
        }

        public Clock Resume() {
            Paused = false;
            return this;
        }
    }
}