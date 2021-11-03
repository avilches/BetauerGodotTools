using Godot;

namespace Veronenger.Game.Tools {
    public class SceneManager {

        public void ChangeScene(string scene) {
            GD.Print("[SceneManager] Change scene to: "+scene);
            GameManager.Instance.GetTree().ChangeScene(scene);
        }

    }
}