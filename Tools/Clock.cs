
namespace Tools {
    public class Clock {
        public float Elapsed { get; private set; } = 0;
        public bool Enabled { get; private set; } = false;

        public Clock() {
        }

        public Clock EnableAndStart() {
            Elapsed = 0;
            Enabled = true;
            return this;
        }

        public Clock Add(float add) {
            if (Enabled) Elapsed += add;
            return this;
        }

        public Clock Disable() {
            Enabled = false;
            return this;
        }
    }
}