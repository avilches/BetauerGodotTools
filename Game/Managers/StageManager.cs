using Godot;
using Tools;
using Tools.Bus;
using Tools.Bus.Topics;
using Veronenger.Game.Controller.Stage;
using static Veronenger.Game.Tools.LayerConstants;

namespace Veronenger.Game.Managers {
    public class StageManager {
        private Stage _enteredStage;
        private bool _exitedStage;
        private Stage _currentStage;
        private StageCameraController _stageCameraController;
        private Area2DOnArea2DTopic _topic = new Area2DOnArea2DTopic("StageTopic");

        public void ConfigureStageCamera(StageCameraController stageCameraController, Area2D stageDetector) {
            _stageCameraController = stageCameraController;
            stageDetector.CollisionLayer = 0;
            stageDetector.CollisionMask = 0;
            stageDetector.SetCollisionMaskBit(PLAYER_DETECTOR_LAYER, true);
            _topic.Subscribe("StageDetector", stageDetector, OnEnterStage, OnExitStage);
        }

        public void ConfigureStage(Area2D stageArea2D, CollisionShape2D stageCollisionShape2D) {
            _topic.AddArea2D(stageArea2D, stageCollisionShape2D);
            stageArea2D.CollisionLayer = 0;
            stageArea2D.CollisionMask = 0;
            stageArea2D.SetCollisionLayerBit(PLAYER_DETECTOR_LAYER, true);
        }

        public void Subscribe(GodotNodeListener<Area2DOnArea2D> enterListener,
            GodotNodeListener<Area2DOnArea2D> exitListener = null) {
            _topic.Subscribe(enterListener, exitListener);
        }

        public void OnEnterStage(Area2DOnArea2D e) {
            RectangleShape2D shape2D = FindFirstCollisionShape2D(e.Area2D);
            Area2D stageEnteredArea2D = e.Area2D;
            Area2D player = e.From;
            var enteredStage = new Stage(stageEnteredArea2D, shape2D);
            if (_currentStage == null) {
                Debug.Stage($"Enter: {player.Name} to {stageEnteredArea2D.Name}. No current stage: changing now");
                _stageCameraController.ChangeStage(enteredStage);
                return;
            }
            if (stageEnteredArea2D.Equals(_currentStage.Area2D)) {
                Debug.Stage(
                    $"Enter: {player.Name} to {stageEnteredArea2D.Name} = _currentStage (we are already here): ignoring!");
                return;
            }
            Debug.Stage($"Enter: {player.Name} to {stageEnteredArea2D.Name}. New place...");
            _enteredStage = enteredStage;
            CheckChangeStage(false);
        }

        public void OnExitStage(Area2DOnArea2D e) {
            Area2D stageExitedArea2D = e.Area2D;
            Area2D stageDetector = e.From;
            if (_enteredStage != null && stageExitedArea2D.Equals(_enteredStage.Area2D)) {
                _enteredStage = null;
                _exitedStage = false;
                Debug.Stage($"Exit: {stageDetector.Name} from {stageExitedArea2D.Name}. Invalid transition, rollback!");
                return;
            }
            Debug.Stage($"Stage exit: {stageDetector.Name} from {stageExitedArea2D.Name}....");
            _exitedStage = true;
            CheckChangeStage(true);
        }

        private void CheckChangeStage(bool normal) {
            if (_exitedStage && _enteredStage != null) {
                Debug.Stage("Change: from " + _currentStage.Name + " to " + _enteredStage.Name +
                            (normal ? "" : " REVERSED (exit old place -> enter new place)"));

                _currentStage = _enteredStage;
                _enteredStage = null;
                _exitedStage = false;
                _stageCameraController.ChangeStage(_enteredStage);
            }
        }

        private RectangleShape2D FindFirstCollisionShape2D(Area2D area2D) {
            foreach (var nodeChild in area2D.GetChildren()) {
                if (nodeChild is CollisionShape2D collisionShape2D && collisionShape2D.Shape is RectangleShape2D r2) {
                    return r2;
                }
            }
            return null;
        }
    }

    public class Stage {
        public readonly Area2D Area2D;
        private readonly RectangleShape2D Shape2D;
        public Rect2 CreateAbsoluteRect2() => new Rect2(Area2D.GlobalPosition - Shape2D.Extents, Shape2D.Extents * 2f);
        public string Name => Area2D.Name;

        public Stage(Area2D area2D, RectangleShape2D shape2D) {
            Area2D = area2D;
            Shape2D = shape2D;
        }

        public override bool Equals(object obj) {
            return this == (Stage)obj;
        }

        public override int GetHashCode() {
            return Area2D.GetHashCode();
        }
    }
}