using Godot;
using Veronenger.Game.Managers.Autoload;

namespace Veronenger.Game.Managers {
    public class SceneManager {

        public void ChangeScene(string scene) {
            GD.Print($"[SceneManager] Change scene to: {scene}");
            GameManager.Instance.GetTree().ChangeScene(scene);
        }

    }
}