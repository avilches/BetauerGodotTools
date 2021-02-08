using Godot;

namespace Betauer.Tools.Area {
    public interface ISceneController {
        void _on_player_entered_scene_change(Area2D player, Area2D stageEnteredArea2D, string scene);

    }
}