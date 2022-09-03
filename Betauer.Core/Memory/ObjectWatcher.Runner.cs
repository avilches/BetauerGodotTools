namespace Betauer.Memory {
    public class DefaultObjectWatcherRunner {
        public static ConsumerRunner Instance = new ConsumerRunner();
    }

    public class ConsumerRunner : Consumer {
        public readonly GodotScheduler Scheduler;

        public ConsumerRunner() {
            // The Runner should run even if the SceneTree is paused
            Scheduler = new GodotScheduler(_OnSchedule, false);
        }

        public GodotScheduler Start(float seconds = 10f) {
            return Scheduler.Start(seconds);
        }

        private void _OnSchedule() => Consume(false);

        public override void Dispose() {
            base.Dispose();
            Scheduler.Stop();
        }
    }
}