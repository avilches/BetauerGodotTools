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
        private readonly Area2DShapeOnArea2DTopic _stageTopic = new Area2DShapeOnArea2DTopic("StageTopic");

        public void ConfigureStageCamera(StageCameraController stageCameraController, Area2D stageDetector) {
            _stageCameraController = stageCameraController;
            stageDetector.CollisionLayer = 0;
            stageDetector.CollisionMask = 0;
            stageDetector.SetCollisionMaskBit(PLAYER_DETECTOR_LAYER, true);
            _stageTopic.Subscribe("StageDetector", stageDetector, stageDetector, OnEnterStage, OnExitStage);
        }

        public void ConfigureStage(Area2D stageArea2D) {
            _stageTopic.AddArea2D(stageArea2D);
            stageArea2D.CollisionLayer = 0;
            stageArea2D.CollisionMask = 0;
            stageArea2D.SetCollisionLayerBit(PLAYER_DETECTOR_LAYER, true);
        }

        public void Subscribe(GodotListener<Area2DShapeOnArea2D> enterListener,
            GodotListener<Area2DShapeOnArea2D> exitListener = null) {
            _stageTopic.Subscribe(enterListener, exitListener);
        }

        public void OnEnterStage(Area2DShapeOnArea2D e) {
            Area2D stageEnteredArea2D = (Area2D)e.Origin;
            Area2D stageDetector = e.Detected;
            var stageToEnter = new Stage(stageEnteredArea2D);
            if (_currentStage == null) {
                Debug.Stage($"{stageDetector.Name}.Enter",
                    $"{stageEnteredArea2D.Name}. No current stage: changing now");
                ChangeStage(stageToEnter);
                return;
            }
            if (stageEnteredArea2D.Equals(_currentStage.Area2D)) {
                Debug.Stage(
                    $"{stageDetector.Name}.Enter",
                    $"{stageEnteredArea2D.Name} == current stage (we are already here): ignoring!");
                return;
            }
            Debug.Stage($"{stageDetector.Name}.Enter", $"{stageEnteredArea2D.Name}. Ok");
            _enteredStage = stageToEnter;
            CheckChangeStage(false);
        }

        public void OnExitStage(Area2DShapeOnArea2D e) {
            Area2D stageExitedArea2D = (Area2D)e.Origin;
            Area2D stageDetector = e.Detected;
            if (_enteredStage != null && stageExitedArea2D.Equals(_enteredStage.Area2D)) {
                _enteredStage = null;
                _exitedStage = false;
                Debug.Stage($"{stageDetector.Name}.Exit",
                    $"{stageExitedArea2D.Name} == entered stage. Rollback!");
                return;
            }
            Debug.Stage($"{stageDetector.Name}.Exit", $"{stageExitedArea2D.Name}. Ok");
            _exitedStage = true;
            CheckChangeStage(true);
        }

        private void CheckChangeStage(bool enterFirstThenExit) {
            if (_exitedStage && _enteredStage != null) {
                Debug.Stage("Change", "From " + _currentStage.Name + " to " + _enteredStage.Name +
                                      (enterFirstThenExit ? "" : " REVERSED (first exit -> then enter)"));
                ChangeStage(_enteredStage);
            }
        }

        private void ChangeStage(Stage newStage) {
            _currentStage = newStage;
            _enteredStage = null;
            _exitedStage = false;
            _stageCameraController.ChangeStage(_currentStage.CreateAbsoluteRect2());
        }

    }

    public class Stage {
        public readonly Area2D Area2D;

        public Rect2 CreateAbsoluteRect2() {
            RectangleShape2D shape2D = (Area2D.GetChild(0) as CollisionShape2D)?.Shape as RectangleShape2D;
            return new Rect2(Area2D.GlobalPosition - shape2D.Extents, shape2D.Extents * 2f);
        }

        public string Name => Area2D.Name;

        public Stage(Area2D area2D) {
            Area2D = area2D;
        }
    }
}