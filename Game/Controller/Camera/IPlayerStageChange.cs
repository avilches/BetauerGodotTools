using Godot;

namespace Veronenger.Game.Controller.Camera {
    public interface IPlayerStageChange {
        void _on_player_entered_stage(Area2D player, Area2D stageEnteredArea2D, RectangleShape2D stageShape2D);
        void _on_player_exited_stage(Area2D player, Area2D stageExitedArea2D);
    }
}