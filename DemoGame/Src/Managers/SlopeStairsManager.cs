using System;
using Betauer.Bus.Signal;
using Godot;
using Betauer.DI;
using Veronenger.Controller.Character;
using static Veronenger.LayerConstants;
using Object = Godot.Object;

namespace Veronenger.Managers {

    [Service]
    public class SlopeStairsManager {
        [Inject] public PlatformManager PlatformManager { get; set;}

        private const string GROUP_SLOPE_STAIRS = "slope_stairs";
        public readonly BodyOnArea2D.Collection DownTopic = new("SlopeStairsDown");
        public readonly BodyOnArea2D.Collection UpTopic = new("SlopeStairsUp");
        public readonly BodyOnArea2DEntered.Unicast EnablerTopic = new("SlopeStairsEnabler");
        public readonly BodyOnArea2DEntered.Unicast DisablerTopic = new("SlopeStairsDisabler");

        public void ConfigureCharacterCollisionsWithSlopeStairs(CharacterBody2D kb2d) {
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
            DownTopic.Connect(area2D);
        }

        public void ConfigureSlopeStairsUp(Area2D area2D) {
            PlatformManager.ConfigureArea2DAsPlatform(area2D);
            UpTopic.Connect(area2D);
        }

        public void ConfigureSlopeStairsEnabler(Area2D area2D) {
            PlatformManager.ConfigureArea2DAsPlatform(area2D);
            EnablerTopic.Connect(area2D);
        }

        public void ConfigureSlopeStairsDisabler(Area2D area2D) {
            PlatformManager.ConfigureArea2DAsPlatform(area2D);
            DisablerTopic.Connect(area2D);
        }

        public bool UpOverlap(PlayerController playerController) {
            return UpTopic.Contains(playerController);
        }

        public bool DownOverlap(PlayerController playerController) {
            return DownTopic.Contains(playerController);
        }

        public void SubscribeSlopeStairsEnabler(Node filter, Action<Area2D> enterListener) {
            EnablerTopic.Subscribe((area2d, _) => enterListener(area2d)).WithFilter(filter);
        }

        public void SubscribeSlopeStairsDisabler(Node filter, Action<Area2D> enterListener) {
            DisablerTopic.Subscribe((area2d, _) => enterListener(area2d)).WithFilter(filter);
        }

        public bool IsSlopeStairs(Object? platform) => platform is PhysicsBody2D psb && psb.IsInGroup(GROUP_SLOPE_STAIRS);

        public bool HasBodyEnabledSlopeStairs(CharacterBody2D kb2d) => kb2d.GetCollisionMaskBit(LayerSlopeStairs);
        public void EnableSlopeStairsForBody(CharacterBody2D kb2d) => kb2d.SetCollisionMaskBit(LayerSlopeStairs, true);
        public void DisableSlopeStairsForBody(CharacterBody2D kb2d) => kb2d.SetCollisionMaskBit(LayerSlopeStairs, false);

        public bool HasBodyEnabledSlopeStairsCover(CharacterBody2D kb2d) => kb2d.GetCollisionMaskBit(LayerSlopeStairsCover);
        public void EnableSlopeStairsCoverForBody(CharacterBody2D kb2d) => kb2d.SetCollisionMaskBit(LayerSlopeStairsCover, true);
        public void DisableSlopeStairsCoverForBody(CharacterBody2D kb2d) => kb2d.SetCollisionMaskBit(LayerSlopeStairsCover, false);

    }
}