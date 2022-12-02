using Godot;
using Betauer;
using Betauer.Bus.Signal;
using Betauer.Core.Nodes;
using Betauer.DI;
using Betauer.Tools.Logging;
using Veronenger.Controller.Stage;
using static Veronenger.LayerConstants;

namespace Veronenger.Managers {

    [Service]
    public class StageManager {
        private static readonly Logger Logger = LoggerFactory.GetLogger(typeof(StageManager));
        private Stage _enteredStage;
        private bool _exitedStage;
        private Stage _currentStage;
        private StageCameraController _stageCameraController;

        private readonly AreaOnArea2DEntered.Unicast _enterStageTopic = new("StageTopic");
        private readonly AreaOnArea2DExited.Unicast _exitStageTopic = new("StageTopic");

        public void ConfigureStageCamera(StageCameraController stageCameraController, Area2D stageDetector) {
            _stageCameraController = stageCameraController;
            _enterStageTopic.Subscribe(OnEnterStage).WithFilter(stageDetector);
            _exitStageTopic.Subscribe(OnExitStage).WithFilter(stageDetector);
        }

        public void ConfigureStage(Area2D stageArea2D) {
            _enterStageTopic.Connect(stageArea2D);
            _exitStageTopic.Connect(stageArea2D);
            stageArea2D.CollisionLayer = 0;
            stageArea2D.CollisionMask = 0;
            stageArea2D.AddToLayer(LayerPlayerStageDetector);
        }

        public void OnEnterStage(Area2D stageEnteredArea2D, Area2D stageDetector) {
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

        public void OnExitStage(Area2D stageExitedArea2D, Area2D stageDetector) {
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
            // TODO Godot 4
            // return new Rect2(Area2D.GlobalPosition - shape2D.Extents, shape2D.Extents * 2f);
            return new Rect2(Area2D.GlobalPosition - shape2D.Size / 2, shape2D.Size);
        }

        public Stage(Area2D area2D) {
            Area2D = area2D;
            Name = area2D.Name;
        }
    }
}