using Godot;

namespace Game.Tools.Area {
    public interface ISceneEvents {
        void _on_player_entered_scene_change(Area2D player, Area2D stageEnteredArea2D, string scene);

    }
}