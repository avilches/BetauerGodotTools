using System;
using Godot;
using System.Collections.Generic;
using Environment = Godot.Environment;
using Animation = Godot.Animation;
using Object = Godot.Object;

namespace Betauer.GodotAction.Proxy {
    public class SceneTreeTweenAction : ProxyNode {

        private List<Action>? _onFinishedAction; 
        public SceneTreeTweenAction OnFinished(Action action, bool oneShot = false, bool deferred = false) {
            AddSignal(ref _onFinishedAction, "finished", nameof(_GodotSignalFinished), action, oneShot, deferred);
            return this;
        }

        public SceneTreeTweenAction RemoveOnFinished(Action action) {
            RemoveSignal(_onFinishedAction, "finished", nameof(_GodotSignalFinished), action);
            return this;
        }

        private SceneTreeTweenAction _GodotSignalFinished() {
            ExecuteSignal(_onFinishedAction);
            return this;
        }

        private List<Action<int>>? _onLoopFinishedAction; 
        public SceneTreeTweenAction OnLoopFinished(Action<int> action, bool oneShot = false, bool deferred = false) {
            AddSignal(ref _onLoopFinishedAction, "loop_finished", nameof(_GodotSignalLoopFinished), action, oneShot, deferred);
            return this;
        }

        public SceneTreeTweenAction RemoveOnLoopFinished(Action<int> action) {
            RemoveSignal(_onLoopFinishedAction, "loop_finished", nameof(_GodotSignalLoopFinished), action);
            return this;
        }

        private SceneTreeTweenAction _GodotSignalLoopFinished(int loop_count) {
            ExecuteSignal(_onLoopFinishedAction, loop_count);
            return this;
        }

        private List<Action>? _onScriptChangedAction; 
        public SceneTreeTweenAction OnScriptChanged(Action action, bool oneShot = false, bool deferred = false) {
            AddSignal(ref _onScriptChangedAction, "script_changed", nameof(_GodotSignalScriptChanged), action, oneShot, deferred);
            return this;
        }

        public SceneTreeTweenAction RemoveOnScriptChanged(Action action) {
            RemoveSignal(_onScriptChangedAction, "script_changed", nameof(_GodotSignalScriptChanged), action);
            return this;
        }

        private SceneTreeTweenAction _GodotSignalScriptChanged() {
            ExecuteSignal(_onScriptChangedAction);
            return this;
        }

        private List<Action<int>>? _onStepFinishedAction; 
        public SceneTreeTweenAction OnStepFinished(Action<int> action, bool oneShot = false, bool deferred = false) {
            AddSignal(ref _onStepFinishedAction, "step_finished", nameof(_GodotSignalStepFinished), action, oneShot, deferred);
            return this;
        }

        public SceneTreeTweenAction RemoveOnStepFinished(Action<int> action) {
            RemoveSignal(_onStepFinishedAction, "step_finished", nameof(_GodotSignalStepFinished), action);
            return this;
        }

        private SceneTreeTweenAction _GodotSignalStepFinished(int idx) {
            ExecuteSignal(_onStepFinishedAction, idx);
            return this;
        }
    }
}