using System;
using Godot;
using Betauer.Core.Nodes;
using Betauer.DI;
using Betauer.Tools.Logging;
using static Veronenger.LayerConstants;

namespace Veronenger.Managers {

    [Service]
    public class StageManager {
        private static readonly Logger Logger = LoggerFactory.GetLogger(typeof(StageManager));
        private Area2D? _enteredStage;
        private bool _exitedStage;
        private Area2D? _currentStage;
        private Camera2D _camera2D;
        private bool IsValidStageChange => _exitedStage && _enteredStage != null;

        public void ConfigureStageCamera(Camera2D camera2D, Area2D stageDetector) {
            _camera2D = camera2D;
            stageDetector.OnAreaEntered(LayerPlayerArea2DDetector, OnEnterStage);
            stageDetector.OnAreaExited(LayerPlayerArea2DDetector, OnExitStage);
        }

        public void ConfigureStage(Area2D stageArea2D) {
            ValidateStageArea2D(stageArea2D);
            stageArea2D.CollisionLayer = 0;
            stageArea2D.CollisionMask = 0;
            stageArea2D.AddToLayer(LayerPlayerArea2DDetector);
        }

        private void ValidateStageArea2D(Area2D area2D) {
            var nodeChild = area2D.GetChildCount() > 0 ? area2D.GetChild<CollisionShape2D>(0) : null;
            if (nodeChild?.Shape is not RectangleShape2D ) {
                throw new Exception(
                    $"Stage {area2D.Name}/{nodeChild?.Name} is not a CollisionShape2D with a RectangleShape2D shape");
            }
        }

        public void OnEnterStage(Area2D stageToEnter) {
            if (_currentStage == null) {
                ChangeStage(stageToEnter);
                return;
            }
            _enteredStage = stageToEnter;
            if (IsValidStageChange) {
                Logger.Debug($"Transition finished. Exit: \"{_currentStage.Name}\" -> Enter: \"{_enteredStage.Name}\" REVERSED (first exit -> then enter)");
                ChangeStage(_enteredStage);
            }
        }

        public void OnExitStage(Area2D stageExitedArea2D) {
            if (_enteredStage != null && stageExitedArea2D.Equals(_enteredStage)) {
                _enteredStage = null;
                _exitedStage = false;
            }
            _exitedStage = true;
            if (IsValidStageChange) {
                Logger.Debug($"Transition finished. Exit: \"{_currentStage!.Name}\" -> Enter: \"{_enteredStage!.Name}\"");
                ChangeStage(_enteredStage);
            }
        }

        private void ChangeStage(Area2D newStage) {
            ClearTransition();
            _currentStage = newStage;
            var rect2 = CreateAbsoluteRect2(_currentStage);
            Logger.Debug($"Camera {rect2.Position} {rect2.End}");
            _camera2D.LimitLeft = (int)rect2.Position.x;
            _camera2D.LimitTop = (int)rect2.Position.y;
            _camera2D.LimitRight = (int)rect2.End.x;
            _camera2D.LimitBottom = (int)rect2.End.y;
        }

        public void ClearTransition() {
            _exitedStage = false;
            _currentStage = null;
            _enteredStage = null;
        }
        
        public Rect2 CreateAbsoluteRect2(Area2D Area2D) {
            RectangleShape2D shape2D = (Area2D.GetChild(0) as CollisionShape2D)?.Shape as RectangleShape2D;
            return new Rect2(Area2D.GlobalPosition - shape2D.Size / 2, shape2D.Size);
        }

    }
}