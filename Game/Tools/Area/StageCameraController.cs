using System;
using Godot;


namespace Veronenger.Game.Tools.Area {
    /**
     * Add this script to a Comera2D in your Player.
     *
     * It will receive
     */
     public class StageCameraController : Camera2D, IPlayerStageChange {
        private Stage _enteredStage;

        private bool _exitedStage;
        private Stage _currentStage;

        public override void _EnterTree() {
            var stageDetector = GetNode<Area2D>("../StageDetector");
            if (stageDetector == null) {
                throw new Exception("Missing node Area2D StageDetector");
            }
            GameManager.Instance.AreaManager.RegisterPlayerStageDetector(stageDetector);
            GameManager.Instance.AreaManager.ListenPlayerStageChanges(this);
        }

        public void _on_player_entered_stage(Area2D player, Area2D stageEnteredArea2D, RectangleShape2D shape2D) {
            var enteredStage = new Stage(stageEnteredArea2D, shape2D);
            if (_currentStage == null) {
                Debug.Stage("Enter: " + player.Name + " to " + stageEnteredArea2D.Name + ". No current stage: changing now");
                ChangeStage(enteredStage);
                return;
            }

            if (stageEnteredArea2D.Equals(_currentStage.Area2D)) {
                Debug.Stage("Enter: " + player.Name + " to " + stageEnteredArea2D.Name +
                      " = _currentStage (we are already here): ignoring!");
                return;
            }

            Debug.Stage("Enter: " + player.Name + " to " + stageEnteredArea2D.Name + ". New place...");
            _enteredStage = enteredStage;
            CheckChangeStage(false);
        }

        public void _on_player_exited_stage(Area2D player, Area2D stageExitedArea2D) {
            if (_enteredStage != null && stageExitedArea2D.Equals(_enteredStage.Area2D)) {
                _enteredStage = null;
                _exitedStage = false;
                Debug.Stage("Exit: " + player.Name + " from " + stageExitedArea2D.Name + ". Invalid transition, rollback!");
                return;
            }

            Debug.Stage("Stage exit: " + player.Name + " from " + stageExitedArea2D.Name + "....");

            _exitedStage = true;

            CheckChangeStage(true);
        }

        private void CheckChangeStage(bool normal) {
            if (_exitedStage && _enteredStage != null) {
                Debug.Stage("Change: from " + _currentStage.Name + " to " + _enteredStage.Name+ (normal?"":" REVERSED (exit old place -> enter new place)"));
                ChangeStage(_enteredStage);
            }
        }

        private void ChangeStage(Stage newStage) {
            _currentStage = newStage;
            _enteredStage = null;
            _exitedStage = false;
            var rect2 = newStage.CreateAbsoluteRect2();
            Debug.Stage("Rect: " + rect2.Position + " " +rect2.End);
            LimitLeft = (int) rect2.Position.x;
            LimitTop = (int) rect2.Position.y;
            LimitRight = (int) rect2.End.x;
            LimitBottom = (int) rect2.End.y;
        }
    }

    internal class Stage {
        public readonly Area2D Area2D;
        private readonly RectangleShape2D Shape2D;
        public Rect2 CreateAbsoluteRect2() => new Rect2(Area2D.GlobalPosition - Shape2D.Extents, Shape2D.Extents * 2f);
        public string Name => Area2D.Name;
        public Stage(Area2D area2D, RectangleShape2D shape2D) {
            Area2D = area2D;
            Shape2D = shape2D;
        }

        public override bool Equals(object obj) {
            return this == (Stage) obj;
        }

        public override int GetHashCode() {
            return Area2D.GetHashCode();
        }
    }

}