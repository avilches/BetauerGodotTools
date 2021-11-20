using Godot;
using Godot.Collections;
using Tools;
using Veronenger.Game.Managers.Autoload;
using static Veronenger.Game.Tools.LayerConstants;

namespace Veronenger.Game.Managers {
    public class AreaManager : Object /* needed to signal listen */{

        [Inject] public CharacterManager CharacterManager;
        [Inject] public GameManager GameManager;

        public void ConfigureDeathZone(Area2D deathArea2D) {
            deathArea2D.CollisionLayer = 0;
            deathArea2D.CollisionMask = 0;
            // TODO: this should be a topic, so other places can subscribe like remove all bullets
            deathArea2D.SetCollisionLayerBit(LayerPlayerStageDetector, true);
            deathArea2D.Connect(GodotConstants.GODOT_SIGNAL_area_entered, this, nameof(_on_player_entered_death_zone),
                new Array { deathArea2D });
        }

        public void _on_player_entered_death_zone(Area2D player, Area2D deathArea2D) {
            // TODO Send and event instead
            CharacterManager.PlayerEnteredDeathZone(deathArea2D);
        }

        // TODO: World complete area2d should
        public void ConfigureSceneChange(Area2D sceneChangeArea2D, string scene) {
            sceneChangeArea2D.Connect(GodotConstants.GODOT_SIGNAL_area_entered, this,
                nameof(_on_player_entered_scene_change),
                new Array { sceneChangeArea2D, scene });
            sceneChangeArea2D.CollisionLayer = 0;
            sceneChangeArea2D.CollisionMask = 0;
            sceneChangeArea2D.SetCollisionLayerBit(LayerPlayerStageDetector, true);
        }

        public void _on_player_entered_scene_change(Area2D player, Area2D stageEnteredArea2D, string scene) {
            // TODO: Send an event instead
            GameManager.ChangeScene(scene);
        }
    }
}