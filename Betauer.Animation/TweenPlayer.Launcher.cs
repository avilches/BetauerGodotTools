using System.Collections.Generic;
using Godot;
using Object = Godot.Object;

namespace Betauer.Animation {
    public class Launcher : TweenActionCallback, ITweenPlayer<Launcher> {
        private static readonly Logger Logger = LoggerFactory.GetLogger(typeof(Launcher));

        public Launcher WithParent(Node parent) {
            parent.AddChild(this);
            return this;
        }


        public bool IsRunning() => IsActive();

        /*
         * Flow:
         * 1 Start <-> Stop
         * 2 Reset keeps the current status. So, Start -> Reset will continue playing (but from beginning)
         */

        public Launcher Play() {
            if (!Object.IsInstanceValid(this)) {
                Logger.Warning("Can't Start in a freed Tween instance");
                return this;
            }
            base.Start();
            base.ResumeAll();
            return this;
        }

        public Launcher Stop() {
            if (!Object.IsInstanceValid(this)) {
                Logger.Warning("Can't Stop AnimationTweenPlayer in a freed Tween instance");
                return this;
            }
            Logger.Info("Tween.StopAll()");
            base.StopAll();
            return this;
        }

        public Launcher Reset() {
            if (!Object.IsInstanceValid(this)) {
                Logger.Warning("Can't Reset AnimationTweenPlayer in a freed Tween instance");
                return this;
            }
            Logger.Info("Tween.RemoveAll()");
            base.RemoveAll();
            Logger.Info("Tween.Start()");
            base.Start();
            return this;
        }

        public Launcher Kill() {
            if (!Object.IsInstanceValid(this)) return this;
            Logger.Info("Tween.StopAll()");
            base.StopAll();
            Logger.Info("Tween.Free()");
            base.QueueFree();
            return this;
        }

        public LoopStatus Play(ISequence sequence, Node? defaultTarget = null,
            float initialDelay = 0, float duration = -1) {
            var loops = sequence is ILoopedSequence loopedSequence ? loopedSequence.Loops : 1;
            return Play(loops, sequence, defaultTarget, initialDelay, duration);
        }

        public LoopStatus PlayForever(ISequence sequence, Node? defaultTarget = null, 
            float initialDelay = 0, float duration = -1) {
            return Play(-1, sequence, defaultTarget, initialDelay, duration);
        }

        public LoopStatus Play(int loops, ISequence sequence, Node? defaultTarget = null, 
            float initialDelay = 0, float duration = -1) {
            LoopStatus loopStatus = new LoopStatus(this, loops, sequence, defaultTarget, duration);
            return loopStatus.Start(initialDelay);
        }

        public LoopStatus MultiPlay(ISequence sequence, IEnumerable<Node>? targets,
            float delayBetweenTargets = 0, float initialDelay = 0, float duration = -1) {
            var loops = sequence is ILoopedSequence loopedSequence ? loopedSequence.Loops : 1;
            return MultiPlay(loops, sequence, targets, delayBetweenTargets, initialDelay, duration);
        }

        public LoopStatus MultiPlayForever(ISequence sequence, IEnumerable<Node>? targets, 
            float delayBetweenTargets = 0, float initialDelay = 0, float duration = -1) {
            return MultiPlay(-1, sequence, targets, delayBetweenTargets, initialDelay, duration);
        }

        public LoopStatus MultiPlay(int loops, ISequence sequence, IEnumerable<Node>? targets,
            float delayBetweenTargets = 0, float initialDelay = 0, float duration = -1) {
            LoopStatus loopStatus = new LoopStatus(this, loops, sequence, targets, delayBetweenTargets, duration);
            return loopStatus.Start(initialDelay);
        }
    }
}