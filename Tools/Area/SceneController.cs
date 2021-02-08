using Godot;
using static Betauer.Tools.LayerConstants;


namespace Betauer.Tools.Area {
    public class SceneController : ISceneController {

        public void RegisterSceneChange(Area2D sceneChangeArea2D) {
            sceneChangeArea2D.CollisionLayer = 0;
            sceneChangeArea2D.CollisionMask = 0;
            sceneChangeArea2D.SetCollisionLayerBit(PLAYER_DETECTOR_LAYER, true);
        }

        public void _on_player_entered_scene_change(Area2D player, Area2D stageEnteredArea2D, string scene) {
            GD.Print("Entered "+scene);
            // var packedScene = (PackedScene) ResourceLoader.Load(scene);
            // GameManager.Instance.GetTree().Root.AddChild(packedScene.Instance());
            GameManager.Instance.GetTree().ChangeScene(scene);
        }

    }
}