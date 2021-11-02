using Godot;
using Godot.Collections;
using static Betauer.Tools.LayerConstants;

namespace Betauer.Tools.Area {
    public class AreaManager : Node, ISceneEvents {
        private IPlayerStageChange _stageCameraController;

        public void ListenPlayerStageChanges(IPlayerStageChange stageCameraController) {
            // TODO: now, only the StageCameraController is subscribed to these events. It could a list instead...?
            _stageCameraController = stageCameraController;
        }

        public void RegisterPlayerStageDetector(Area2D stageDetector) {
            Debug.Register("AreaManager.PlayerStageDetector", stageDetector);
            stageDetector.CollisionLayer = 0;
            stageDetector.CollisionMask = 0;
            stageDetector.SetCollisionMaskBit(PLAYER_DETECTOR_LAYER, true);
        }

        public void RegisterStage(Area2D stageArea2D, CollisionShape2D stageCollisionShape2D) {
            Debug.Register("AreaManager.Stage", stageArea2D);
            stageArea2D.Connect(GodotConstants.GODOT_SIGNAL_area_entered, this, nameof(_on_player_entered_stage),
                new Array {stageArea2D, stageCollisionShape2D.Shape});
            stageArea2D.Connect(GodotConstants.GODOT_SIGNAL_area_exited, this, nameof(_on_player_exited_stage),
                new Array {stageArea2D});
            stageArea2D.CollisionLayer = 0;
            stageArea2D.CollisionMask = 0;
            stageArea2D.SetCollisionLayerBit(PLAYER_DETECTOR_LAYER, true);
        }


        public void _on_player_entered_stage(Area2D player, Area2D stageEnteredArea2D, RectangleShape2D shape2D) {
            _stageCameraController?._on_player_entered_stage(player, stageEnteredArea2D, shape2D);
        }

        public void _on_player_exited_stage(Area2D player, Area2D stageExitedArea2D) {
            _stageCameraController?._on_player_exited_stage(player, stageExitedArea2D);
        }

        public void RegisterDeathZone(Area2D deathArea2D) {
            Debug.Register("AreaManager.DeathZone", deathArea2D);
            deathArea2D.CollisionLayer = 0;
            deathArea2D.CollisionMask = 0;
            deathArea2D.SetCollisionLayerBit(PLAYER_DETECTOR_LAYER, true);
            deathArea2D.Connect(GodotConstants.GODOT_SIGNAL_area_entered, this, nameof(_on_player_entered_death_zone),
                new Array {deathArea2D});
        }

        public void _on_player_entered_death_zone(Area2D player, Area2D deathArea2D) {
            GameManager.Instance.PlayerEnteredDeathZone(deathArea2D);
        }

        // TODO: World complete area2d should register
        public void RegisterSceneChange(Area2D sceneChangeArea2D, string scene) {
            Debug.Register("AreaManager.SceneChange", sceneChangeArea2D);
            sceneChangeArea2D.Connect(GodotConstants.GODOT_SIGNAL_area_entered, this, nameof(_on_player_entered_scene_change),
                new Array {sceneChangeArea2D, scene});
            sceneChangeArea2D.CollisionLayer = 0;
            sceneChangeArea2D.CollisionMask = 0;
            sceneChangeArea2D.SetCollisionLayerBit(PLAYER_DETECTOR_LAYER, true);
        }

        public void _on_player_entered_scene_change(Area2D player, Area2D stageEnteredArea2D, string scene) {
            GameManager.Instance.SceneManager.ChangeScene(scene);
        }
    }
}