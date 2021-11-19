using Tools;
using Veronenger.Game.Managers.Autoload;

namespace Veronenger.Game.Managers {
    public class SceneManager {

        private Logger _logger = LoggerFactory.GetLogger(typeof(SceneManager));

        public void ChangeScene(string scene) {
            _logger.Debug($"Change scene to: {scene}");
            GameManager.Instance.GetTree().ChangeScene(scene);
        }

    }
}