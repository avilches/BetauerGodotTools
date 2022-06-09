using Godot;
using Betauer;
using Betauer.DI;
using Veronenger.Game.Managers;

namespace Veronenger.Game.Controller.Stage {
    /**
     * Add this script to a Comera2D in your Player.
     * The Player should have an Area2D called 'StageDetector' children
     * (So, this script and StageDetector should be siblings)
     *
     * Player (KB2d)
     *   +- StageDetector : Area2D
     *   +- StageCameraController (this class)
     */
    public class StageCameraController :Camera2D {

        [Inject] public StageManager StageManager;

        [OnReady("../Detector")] private Area2D stageDetector;

        public override void _Ready() {
            StageManager.ConfigureStageCamera(this, stageDetector);
        }

        public void ChangeStage(Rect2 rect2) {
            LoggerFactory.GetLogger(typeof(StageCameraController)).Debug($"Camera {rect2.Position} {rect2.End}");
            LimitLeft = (int)rect2.Position.x;
            LimitTop = (int)rect2.Position.y;
            LimitRight = (int)rect2.End.x;
            LimitBottom = (int)rect2.End.y;
        }
    }
}