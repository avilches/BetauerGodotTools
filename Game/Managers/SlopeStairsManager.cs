using Godot;
using Tools;
using Tools.Bus;
using Tools.Bus.Topics;
using Veronenger.Game.Controller.Character;
using static Veronenger.Game.Tools.LayerConstants;

namespace Veronenger.Game.Managers {

    [Singleton]
    public class SlopeStairsManager {
        [Inject] public PlatformManager PlatformManager;

        private const string GROUP_SLOPE_STAIRS = "slope_stairs";
        private readonly BodyOnArea2DTopic _downTopic = new BodyOnArea2DTopic("SlopeStairsDown");
        private readonly BodyOnArea2DTopic _upTopic = new BodyOnArea2DTopic("SlopeStairsUp");
        private readonly BodyOnArea2DTopic _enablerTopic = new BodyOnArea2DTopic("SlopeStairsEnabler");
        private readonly BodyOnArea2DTopic _disablerTopic = new BodyOnArea2DTopic("SlopeStairsDisabler");

        public void ConfigurePlayerCollisions(KinematicBody2D kb2d) {
            kb2d.SetCollisionMaskBit(LayerSlopeStairs, false);
            kb2d.SetCollisionMaskBit(LayerSlopeStairsCover, true);
        }
        public void ConfigureSlopeStairs(PhysicsBody2D platform) {
            platform.AddToGroup(GROUP_SLOPE_STAIRS);
            platform.CollisionMask = 0;
            platform.CollisionLayer = 0;
            platform.SetCollisionLayerBit(LayerSlopeStairs, true);
        }

        public void ConfigureSlopeStairsCover(PhysicsBody2D platform) {
            // platform.AddToGroup("slope_stairs_cover");
            platform.CollisionMask = 0;
            platform.CollisionLayer = 0;
            platform.SetCollisionLayerBit(LayerSlopeStairsCover, true);
        }

        public void ConfigureSlopeStairsDown(Area2D area2D) {
            PlatformManager.ConfigureArea2DAsPlatform(area2D);
            _downTopic.AddArea2D(area2D);
        }

        public void ConfigureSlopeStairsUp(Area2D area2D) {
            PlatformManager.ConfigureArea2DAsPlatform(area2D);
            _upTopic.AddArea2D(area2D);
        }

        public void ConfigureSlopeStairsEnabler(Area2D area2D) {
            PlatformManager.ConfigureArea2DAsPlatform(area2D);
            _enablerTopic.AddArea2D(area2D);
        }

        public void ConfigureSlopeStairsDisabler(Area2D area2D) {
            PlatformManager.ConfigureArea2DAsPlatform(area2D);
            _disablerTopic.AddArea2D(area2D);
        }

        public void SubscribeSlopeStairsDown(GodotListener<BodyOnArea2D> enterListener,
            GodotListener<BodyOnArea2D> exitListener = null) {
            _downTopic.Subscribe(enterListener, exitListener);
        }

        public BodyOnArea2DStatus CreateSlopeStairsUpStatusListener(string name, PlayerController playerController) {
            return _upTopic.StatusSubscriber(name, playerController, playerController);
        }

        public BodyOnArea2DStatus CreateSlopeStairsDownStatusListener(string name, PlayerController playerController) {
            return _downTopic.StatusSubscriber(name, playerController, playerController);
        }

        public void SubscribeSlopeStairsEnabler(GodotListener<BodyOnArea2D> enterListener,
            GodotListener<BodyOnArea2D> exitListener = null) {
            _enablerTopic.Subscribe(enterListener, exitListener);
        }

        public void SubscribeSlopeStairsDisabler(GodotListener<BodyOnArea2D> enterListener,
            GodotListener<BodyOnArea2D> exitListener = null) {
            _disablerTopic.Subscribe(enterListener, exitListener);
        }

        public bool IsSlopeStairs(PhysicsBody2D platform) => platform.IsInGroup(GROUP_SLOPE_STAIRS);

        public bool HasBodyEnabledSlopeStairs(KinematicBody2D kb2d) => kb2d.GetCollisionMaskBit(LayerSlopeStairs);
        public void EnableSlopeStairsForBody(KinematicBody2D kb2d) => kb2d.SetCollisionMaskBit(LayerSlopeStairs, true);
        public void DisableSlopeStairsForBody(KinematicBody2D kb2d) => kb2d.SetCollisionMaskBit(LayerSlopeStairs, false);

        public bool HasBodyEnabledSlopeStairsCover(KinematicBody2D kb2d) => kb2d.GetCollisionMaskBit(LayerSlopeStairsCover);
        public void EnableSlopeStairsCoverForBody(KinematicBody2D kb2d) => kb2d.SetCollisionMaskBit(LayerSlopeStairsCover, true);
        public void DisableSlopeStairsCoverForBody(KinematicBody2D kb2d) => kb2d.SetCollisionMaskBit(LayerSlopeStairsCover, false);

    }
}