using Betauer.Time;
using Godot;

namespace Betauer.Memory {
    public class DefaultObjectWatcherRunner {
        public static ObjectWatcherRunner Instance = new ObjectWatcherRunner();
    }

    public class ObjectWatcherRunner : Consumer {
        public readonly GodotScheduler Scheduler;

        public ObjectWatcherRunner() {
            // Process, because the Runner should run even if the SceneTree is paused
            Scheduler = new GodotScheduler(_OnSchedule, Node.PauseModeEnum.Process);
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