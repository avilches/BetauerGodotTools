namespace Tools {
    public class Timer {
        public float Elapsed { get; private set; } = 0;
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

        public Timer Add(float add) {
            if (!Stopped) Elapsed += add;
            return this;
        }

    }
}