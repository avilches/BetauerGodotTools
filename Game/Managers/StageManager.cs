using Godot;
using Betauer;
using Betauer.Bus;
using Betauer.Bus.Topics;
using Veronenger.Game.Controller.Stage;
using static Veronenger.Game.LayerConstants;

namespace Veronenger.Game.Managers {

    [Singleton]
    public class StageManager {
        private static readonly Logger Logger = LoggerFactory.GetLogger(typeof(StageManager));
        private Stage _enteredStage;
        private bool _exitedStage;
        private Stage _currentStage;
        private StageCameraController _stageCameraController;

        private readonly Area2DShapeOnArea2DTopic _stageTopic = new Area2DShapeOnArea2DTopic("StageTopic");

        public void ConfigureStageCamera(StageCameraController stageCameraController, Area2D stageDetector) {
            _stageCameraController = stageCameraController;
            stageDetector.CollisionLayer = 0;
            stageDetector.CollisionMask = 0;
            stageDetector.SetCollisionMaskBit(LayerPlayerStageDetector, true);
            _stageTopic.Subscribe("StageDetector", stageDetector, stageDetector, OnEnterStage, OnExitStage);
        }

        public void ConfigureStage(Area2D stageArea2D) {
            _stageTopic.ListenSignalsOf(stageArea2D);
            stageArea2D.CollisionLayer = 0;
            stageArea2D.CollisionMask = 0;
            stageArea2D.SetCollisionLayerBit(LayerPlayerStageDetector, true);
        }

        public void Subscribe(GodotListener<Area2DShapeOnArea2D> enterListener,
            GodotListener<Area2DShapeOnArea2D> exitListener = null) {
            _stageTopic.Subscribe(enterListener, exitListener);
        }

        public void OnEnterStage(Area2DShapeOnArea2D e) {
            Area2D stageEnteredArea2D = (Area2D)e.Origin;
            Area2D stageDetector = e.Detected;
            var stageToEnter = new Stage(stageEnteredArea2D);
            var stageDetectorName = stageDetector.Name;
            if (_currentStage == null) {
                Logger.Debug($"\"{stageDetectorName}\" entered to \"{stageEnteredArea2D.Name}\" and no current stage: changing now");
                ChangeStage(stageToEnter);
                return;
            }
            if (stageEnteredArea2D.Equals(_currentStage.Area2D)) {
                Logger.Debug($"\"{stageDetectorName}\" entered to \"{stageEnteredArea2D.Name}\" but it's the same as current stage: ignoring!");
                return;
            }
            Logger.Debug($"\"{stageDetectorName}\" entered to \"{stageEnteredArea2D.Name}\". Transition enter is ok.");
            _enteredStage = stageToEnter;
            CheckChangeStage(stageDetectorName, false);
        }

        public void OnExitStage(Area2DShapeOnArea2D e) {
            Area2D stageExitedArea2D = (Area2D)e.Origin;
            Area2D stageDetector = e.Detected;
            var stageDetectorName = stageDetector.Name;
            var stageExitedName = stageExitedArea2D.Name;
            if (_enteredStage != null && stageExitedArea2D.Equals(_enteredStage.Area2D)) {
                _enteredStage = null;
                _exitedStage = false;
                Logger.Debug($"\"{stageDetectorName}\" exited from \"{stageExitedName}\" == entered stage. Rollback whole transition.");
            }
            Logger.Debug($"\"{stageDetectorName}\" exited from \"{stageExitedName}\". Transition exit is ok.");
            _exitedStage = true;
            CheckChangeStage(stageDetectorName, true);
        }

        private void CheckChangeStage(string stageDetectorName, bool enterFirstThenExit) {
            if (_exitedStage && _enteredStage != null) {
                var reversedFirstExitThenEnter = enterFirstThenExit ? "" : " REVERSED (first exit -> then enter)";
                Logger.Debug($"\"{stageDetectorName}\" transition finished. Exit: \"{_currentStage.Name}\" -> Enter: \"{_enteredStage.Name}\" {reversedFirstExitThenEnter}");
                ChangeStage(_enteredStage);
            }
        }

        private void ChangeStage(Stage newStage) {
            _currentStage = newStage;
            _enteredStage = null;
            _exitedStage = false;
            _stageCameraController.ChangeStage(_currentStage.CreateAbsoluteRect2());
        }

        public void ClearTransition() {
            _exitedStage = false;
            _currentStage = null;
            _enteredStage = null;
        }
    }

    public class Stage {
        public readonly Area2D Area2D;
        public readonly string Name;

        public Rect2 CreateAbsoluteRect2() {
            RectangleShape2D shape2D = (Area2D.GetChild(0) as CollisionShape2D)?.Shape as RectangleShape2D;
            return new Rect2(Area2D.GlobalPosition - shape2D.Extents, shape2D.Extents * 2f);
        }

        public Stage(Area2D area2D) {
            Area2D = area2D;
            Name = area2D.Name;
        }
    }
}