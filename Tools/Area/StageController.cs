using static System.Math;
using Betauer.Tools;
using Betauer.Tools.Platforms;
using Godot;
using static Betauer.Tools.LayerConstants;


namespace Betauer.Tools.Area {
    public class StageController : IStageController {
        private Stage _enteredStage;
        private bool _exitedStage;
        private Stage _currentStage;

        private AreaManager AreaManager;

        public StageController(AreaManager areaManager) {
            AreaManager = areaManager;
        }

        private Camera2D Camera2D => GameManager.Instance.Camera2D;

        public void RegisterStage(CollisionShape2D stageCollisionShape2D) {
            var stageArea2D = stageCollisionShape2D.GetParent<Area2D>();
            stageArea2D.CollisionLayer = 0;
            stageArea2D.CollisionMask = 0;
            stageArea2D.SetCollisionLayerBit(PLAYER_DETECTOR_LAYER, true);
        }

        public void RegisterPlayer(KinematicBody2D player) {
            var stageDetector = player.GetNode<Area2D>("StageDetector");
            if (stageDetector == null) {
                throw new System.Exception("Player must have an Area2D called 'StageDetector'");
            }

            stageDetector.CollisionLayer = 0;
            stageDetector.CollisionMask = 0;
            stageDetector.SetCollisionMaskBit(PLAYER_DETECTOR_LAYER, true);
        }

        private class Stage {
            public readonly Area2D Area2D;
            public readonly RectangleShape2D Shape2D;
            public Rect2 Rect2 => new Rect2(Area2D.Position - Shape2D.Extents, Shape2D.Extents * 2f);
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

        public void _on_player_entered_stage(Area2D player, Area2D stageEnteredArea2D, RectangleShape2D shape2D) {
            var enteredStage = new Stage(stageEnteredArea2D, shape2D);
            if (_currentStage == null) {
                ChangeStage(enteredStage);
                GD.Print("Entering first time: Current stage ", stageEnteredArea2D.Name);
                return;
            }

            if (stageEnteredArea2D.Equals(_currentStage.Area2D)) {
                GD.Print("Entering in ", stageEnteredArea2D.Name, ". We are already here. Ignoring");
                return;
            }

            GD.Print("Entering in ", stageEnteredArea2D.Name, ". (Scheduled)");
            _enteredStage = enteredStage;
            CheckChangeStage(false);
        }

        public void _on_player_exited_stage(Area2D player, Area2D stageExitedArea2D, RectangleShape2D stageShape2D) {
            if (_enteredStage != null && stageExitedArea2D.Equals(_enteredStage.Area2D)) {
                _enteredStage = null;
                _exitedStage = false;
                GD.Print("Exiting stage ", stageExitedArea2D.Name, ". Rollback!");
                return;
            }

            _exitedStage = true;

            CheckChangeStage(true);
        }

        private void CheckChangeStage(bool normal) {
            if (_exitedStage && _enteredStage != null) {
                GD.Print("Exiting stage ", _currentStage.Name, ", changing to scheduled ", _enteredStage.Name);
                if (!normal) {
                    GD.Print("WWWWWWWWWWWWWWWWW!!!!!");
                }
                ChangeStage(_enteredStage);
            }
        }

        float Lerp(float firstFloat, float secondFloat, float by) {
            return firstFloat * (1 - by) + secondFloat * by;
        }

        private void ChangeStage(Stage newStage) {
            // _changeFrom = _currentStage?.Rect2;
            // _changeTo = newStage.Rect2;

            _currentStage = newStage;
            _enteredStage = null;
            _exitedStage = false;
            var rect2 = newStage.Rect2;
            Camera2D.LimitTop = (int) rect2.Position.y;
            Camera2D.LimitLeft = (int) rect2.Position.x;
            Camera2D.LimitBottom = (int) rect2.End.y;
            Camera2D.LimitRight = (int) rect2.End.x;

        }

    }
}