using System;
using Betauer.Time;
using Godot;

namespace Betauer.Memory {
    public class DefaultObjectWatcherTask {
        public static ObjectWatcherTask Instance = new();
    }

    public class ObjectWatcherTask : Consumer {
        public readonly GodotScheduler Scheduler;

        public ObjectWatcherTask() {
            // Process, because the Runner should run even if the SceneTree is paused
            Scheduler = new GodotScheduler(_OnSchedule, true, false, true);
        }

        public GodotScheduler Start(SceneTree sceneTree, float seconds = 10f) {
            if (Disposed) throw new Exception("Consumer disposed");
            return Scheduler.Start(sceneTree, seconds);
        }

        private void _OnSchedule() => Consume(false);

        public override void Dispose() {
            base.Dispose();
            Scheduler.Stop();
        }
    }
}