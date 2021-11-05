using System.Collections;
using System.Collections.Generic;
using Veronenger.Game.Tools.Character;
using Godot;
using Tools.Events;
using static Veronenger.Game.Tools.LayerConstants;
using static Tools.GodotTools;

namespace Veronenger.Game.Tools.Platforms {
    public class PlatformManager {
        private const string GROUP_PLATFORMS = "platform";
        private const string GROUP_MOVING_PLATFORMS = "moving_platform";
        private const string GROUP_FALLING_PLATFORMS = "falling_platform";
        private const string GROUP_SLOPE_STAIRS = "slope_stairs";

        /**
         * No almacena nada, solo permite que characters se suscriban a cambios en una plataforma
         * y permite cambiar sus mascaras.
         */

        // Configura el layer de la plataforma segun el tipo
        public List<PhysicsBody2D> RegisterPlatforms(IList nodes, bool falling = false, bool moving = false) {
            var platforms = Filter<PhysicsBody2D>(nodes);
            platforms.ForEach(kb2d => RegisterPlatform(kb2d, falling, moving));
            return platforms;
        }

        /**
         * Si una plataforma falling tiene un Area2D dentro, se tomará como una zona que vuelve a activar las plataformas falling
         * Es lo mismo que si a esa zona se le añade el script FallingPlatformExit
         */
        public void RegisterPlatform(PhysicsBody2D platform, bool falling = false, bool moving = false) {
            var message = "PlatformManager.Platform " + (falling ? "falling" : "") + "/" + (moving ? "moving" : "") + ")";
            Debug.Register(message, platform);
            // platform.AddToGroup("platform");
            platform.CollisionMask = 0;
            platform.CollisionLayer = 0;
            platform.AddToGroup(GROUP_PLATFORMS);
            if (falling) {
                foreach (var child in platform.GetChildren())
                    if (child is CollisionShape2D colShape2d) {
                        colShape2d.OneWayCollision = true;
                    } else if (child is Area2D area2D) {
                        Debug.Register("PlatformManager.Platform(Falling platform exit)", area2D);
                        AddArea2DFallingPlatformExit(area2D);
                    }
                platform.AddToGroup(GROUP_FALLING_PLATFORMS);
                platform.SetCollisionLayerBit(FALL_PLATFORM_LAYER, true);
            } else {
                platform.SetCollisionLayerBit(REGULAR_PLATFORM_LAYER, true);
            }

            if (moving) {
                if (platform is KinematicBody2D kb2d) kb2d.Motion__syncToPhysics = true;
                platform.AddToGroup(GROUP_MOVING_PLATFORMS);
            }
        }

        public void ConfigureSlopeStairs(PhysicsBody2D platform) {
            Debug.Register("PlatformManager.Slope stair", platform);
            platform.AddToGroup(GROUP_SLOPE_STAIRS);
            platform.CollisionMask = 0;
            platform.CollisionLayer = 0;
            platform.SetCollisionLayerBit(SLOPE_STAIRS_LAYER, true);
        }

        public void ConfigureSlopeStairsCover(PhysicsBody2D platform) {
            Debug.Register("PlatformManager.Slope stair cover", platform);
            // platform.AddToGroup("slope_stairs_cover");
            platform.CollisionMask = 0;
            platform.CollisionLayer = 0;
            platform.SetCollisionLayerBit(SLOPE_STAIRS_COVER_LAYER, true);
        }

        public void ConfigureEnemyCollisions(CharacterController kb2d) {
            ConfigurePlayerCollisions(kb2d);
        }

        public void ConfigurePlayerCollisions(CharacterController kb2d) {
            Debug.Register("PlatformManager.PlayerController", kb2d);
            kb2d.SetCollisionMaskBit(REGULAR_PLATFORM_LAYER, true);
            kb2d.SetCollisionMaskBit(SLOPE_STAIRS_LAYER, false);
            kb2d.SetCollisionMaskBit(SLOPE_STAIRS_COVER_LAYER, true);
            kb2d.SetCollisionMaskBit(FALL_PLATFORM_LAYER, true);
        }

        public bool IsMovingPlatform(KinematicBody2D platform) => platform.IsInGroup(GROUP_MOVING_PLATFORMS);
        public bool IsFallingPlatform(PhysicsBody2D platform) => platform.IsInGroup(GROUP_FALLING_PLATFORMS);
        public bool IsSlopeStairs(PhysicsBody2D platform) => platform.IsInGroup(GROUP_SLOPE_STAIRS);

        // Provoca la caida del jugador desde la plataforma quitando la mascara
        public void BodyFallFromPlatform(KinematicBody2D kb2d) => kb2d.SetCollisionMaskBit(FALL_PLATFORM_LAYER, false);
        public void EnableSlopeStairsForBody(KinematicBody2D kb2d) => kb2d.SetCollisionMaskBit(SLOPE_STAIRS_LAYER, true);
        public void DisableSlopeStairsForBody(KinematicBody2D kb2d) => kb2d.SetCollisionMaskBit(SLOPE_STAIRS_LAYER, false);
        public bool HasBodyEnabledSlopeStairs(KinematicBody2D kb2d) => kb2d.GetCollisionMaskBit(SLOPE_STAIRS_LAYER);
        public void EnableSlopeStairsCoverForBody(KinematicBody2D kb2d) => kb2d.SetCollisionMaskBit(SLOPE_STAIRS_COVER_LAYER, true);
        public void DisableSlopeStairsCoverForBody(KinematicBody2D kb2d) => kb2d.SetCollisionMaskBit(SLOPE_STAIRS_COVER_LAYER, false);
        public bool HasBodyEnabledSlopeStairsCover(KinematicBody2D kb2d) => kb2d.GetCollisionMaskBit(SLOPE_STAIRS_COVER_LAYER);

        /**
         * Platform falling & body out
         */
        public bool IsBodyFallingFromPlatform(KinematicBody2D kb2d) => !kb2d.GetCollisionMaskBit(FALL_PLATFORM_LAYER);
        public void BodyStopFallFromPlatform(KinematicBody2D kb2d) => kb2d.SetCollisionMaskBit(FALL_PLATFORM_LAYER, true);
        private BodyOnArea2DTopic _platformBodyOutTopic = new BodyOnArea2DTopic("PlatformBodyOut");
        public void AddArea2DFallingPlatformExit(Area2D area2D) => _platformBodyOutTopic.AddArea2D(area2D);
        public void SubscribeFallingPlatformOut(GodotNodeListenerDelegate<BodyOnArea2D> enterListener) => _platformBodyOutTopic.Subscribe(enterListener);

        /**
         * Slope stairs
         */
        private readonly BodyOnArea2DTopic _slopeStairsDownTopic = new BodyOnArea2DTopic("SlopeStairsDown");
        private readonly BodyOnArea2DTopic _slopeStairsUpTopic = new BodyOnArea2DTopic("SlopeStairsUp");
        private readonly BodyOnArea2DTopic _slopeStairsEnablerTopic = new BodyOnArea2DTopic("SlopeStairsEnabler");
        private readonly BodyOnArea2DTopic _slopeStairsDisablerTopic = new BodyOnArea2DTopic("SlopeStairsDisabler");

        public void AddArea2DSlopeStairsDown(Area2D area2D) => _slopeStairsDownTopic.AddArea2D(area2D);
        public void AddArea2DSlopeStairsUp(Area2D area2D) => _slopeStairsUpTopic.AddArea2D(area2D);
        public void AddArea2DSlopeStairsEnabler(Area2D area2D) => _slopeStairsEnablerTopic.AddArea2D(area2D);
        public void AddArea2DSlopeStairsDisabler(Area2D area2D) => _slopeStairsDisablerTopic.AddArea2D(area2D);

        public void SubscribeSlopeStairsDown(GodotNodeListenerDelegate<BodyOnArea2D> enterListener,
            GodotNodeListenerDelegate<BodyOnArea2D> exitListener = null) {
            _slopeStairsDownTopic.Subscribe(enterListener, exitListener);
        }

        public void SubscribeSlopeStairsUp(GodotNodeListenerDelegate<BodyOnArea2D> enterListener,
            GodotNodeListenerDelegate<BodyOnArea2D> exitListener = null) {
            _slopeStairsUpTopic.Subscribe(enterListener, exitListener);
        }

        public void SubscribeSlopeStairsEnabler(GodotNodeListenerDelegate<BodyOnArea2D> enterListener,
            GodotNodeListenerDelegate<BodyOnArea2D> exitListener = null) {
            _slopeStairsEnablerTopic.Subscribe(enterListener, exitListener);
        }

        public void SubscribeSlopeStairsDisabler(GodotNodeListenerDelegate<BodyOnArea2D> enterListener,
            GodotNodeListenerDelegate<BodyOnArea2D> exitListener = null) {
            _slopeStairsDisablerTopic.Subscribe(enterListener, exitListener);
        }
    }
}