using Godot;
using static Betauer.Tools.LayerConstants;


namespace Betauer.Tools.Area {
    public class SceneManager {

        public void ChangeScene(string scene) {
            GD.Print("[SceneManager] Change scene to: "+scene);
            GameManager.Instance.GetTree().ChangeScene(scene);
        }

    }
}