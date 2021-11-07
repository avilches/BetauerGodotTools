using Godot;
using Tools.Bus;
using Tools.Bus.Topics;
using Veronenger.Game.Character;
using Veronenger.Game.Managers.Autoload;
using static Veronenger.Game.Tools.LayerConstants;

namespace Veronenger.Game.Managers {
    public class SlopeStairsManager {
        public PlatformManager PlatformManager => GameManager.Instance.PlatformManager;
        private const string GROUP_SLOPE_STAIRS = "slope_stairs";
        private readonly BodyOnArea2DTopic _downTopic = new BodyOnArea2DTopic("SlopeStairsDown");
        private readonly BodyOnArea2DTopic _upTopic = new BodyOnArea2DTopic("SlopeStairsUp");
        private readonly BodyOnArea2DTopic _enablerTopic = new BodyOnArea2DTopic("SlopeStairsEnabler");
        private readonly BodyOnArea2DTopic _disablerTopic = new BodyOnArea2DTopic("SlopeStairsDisabler");

        public void ConfigurePlayerCollisions(CharacterController kb2d) {
            kb2d.SetCollisionMaskBit(SLOPE_STAIRS_LAYER, false);
            kb2d.SetCollisionMaskBit(SLOPE_STAIRS_COVER_LAYER, true);
        }
        public void ConfigureSlopeStairs(PhysicsBody2D platform) {
            platform.AddToGroup(GROUP_SLOPE_STAIRS);
            platform.CollisionMask = 0;
            platform.CollisionLayer = 0;
            platform.SetCollisionLayerBit(SLOPE_STAIRS_LAYER, true);
        }

        public void ConfigureSlopeStairsCover(PhysicsBody2D platform) {
            // platform.AddToGroup("slope_stairs_cover");
            platform.CollisionMask = 0;
            platform.CollisionLayer = 0;
            platform.SetCollisionLayerBit(SLOPE_STAIRS_COVER_LAYER, true);
        }

        public void ConfigureSlopeStairsDown(Area2D area2D) {
            GameManager.Instance.PlatformManager.ConfigureArea2DAsPlatform(area2D);
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

        public void SubscribeSlopeStairsUp(GodotListener<BodyOnArea2D> enterListener,
            GodotListener<BodyOnArea2D> exitListener = null) {
            _upTopic.Subscribe(enterListener, exitListener);
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

        public bool HasBodyEnabledSlopeStairs(KinematicBody2D kb2d) => kb2d.GetCollisionMaskBit(SLOPE_STAIRS_LAYER);
        public void EnableSlopeStairsForBody(KinematicBody2D kb2d) => kb2d.SetCollisionMaskBit(SLOPE_STAIRS_LAYER, true);
        public void DisableSlopeStairsForBody(KinematicBody2D kb2d) => kb2d.SetCollisionMaskBit(SLOPE_STAIRS_LAYER, false);

        public bool HasBodyEnabledSlopeStairsCover(KinematicBody2D kb2d) => kb2d.GetCollisionMaskBit(SLOPE_STAIRS_COVER_LAYER);
        public void EnableSlopeStairsCoverForBody(KinematicBody2D kb2d) => kb2d.SetCollisionMaskBit(SLOPE_STAIRS_COVER_LAYER, true);
        public void DisableSlopeStairsCoverForBody(KinematicBody2D kb2d) => kb2d.SetCollisionMaskBit(SLOPE_STAIRS_COVER_LAYER, false);

    }
}