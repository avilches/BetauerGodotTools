using System;
using System.Collections.Generic;
using Godot;
using Betauer.Bus.Signal;
using Betauer.Core.Nodes;
using Betauer.DI;
using static Veronenger.LayerConstants;
using Object = Godot.Object;

namespace Veronenger.Managers {
    [Service]
    public class PlatformManager {
        private const string GROUP_PLATFORMS = "platform";
        private const string GROUP_MOVING_PLATFORMS = "moving_platform";
        private const string GROUP_FALLING_PLATFORMS = "falling_platform";

        /**
         * No almacena nada, solo permite que characters se suscriban a cambios en una plataforma
         * y permite cambiar sus mascaras.
         */

        // Configura el layer de la plataforma segun el tipo
        public void ConfigurePlatformList(List<PhysicsBody2D> platforms, bool falling = false, bool moving = false) {
            platforms.ForEach(kb2d => ConfigurePlatform(kb2d, falling, moving));
        }

        /**
         * Si una plataforma falling tiene un Area2D dentro, se tomará como una zona que vuelve a activar las plataformas falling
         * Es lo mismo que si a esa zona se le añade el script FallingPlatformExit
         */
        public void ConfigurePlatform(PhysicsBody2D platform, bool falling = false, bool moving = false) {
            // platform.AddToGroup("platform");
            platform.CollisionMask = 0;
            platform.CollisionLayer = 0;
            platform.AddToGroup(GROUP_PLATFORMS);
            if (falling) {
                foreach (var child in platform.GetChildren())
                    if (child is CollisionShape2D colShape2d) {
                        colShape2d.OneWayCollision = true;
                    } else if (child is Area2D area2D) {
                        AddArea2DFallingPlatformExit(area2D);
                    }
                platform.AddToGroup(GROUP_FALLING_PLATFORMS);
                platform.AddToLayer(LayerFallPlatform);
            } else {
                platform.AddToLayer(LayerRegularPlatform);
            }

            if (moving) {
                // TODO Godot 4: discover how to enable/disable sync to physics 
                // if (platform is CharacterBody2D kb2d) kb2d.Motion__syncToPhysics = true;
                platform.AddToGroup(GROUP_MOVING_PLATFORMS);
            }
        }

        public void ConfigureTileMapCollision(TileMap tileMap) {
            tileMap.TileSet.SetPhysicsLayerCollisionLayer(0, 0);
            tileMap.TileSet.SetPhysicsLayerCollisionMask(0, 0);
            tileMap.AddToLayer(0, LayerRegularPlatform);
        }

        public void ConfigureArea2DAsPlatform(Area2D upHall) {
            upHall.CollisionMask = 0;
            upHall.CollisionLayer = 0;
            upHall.AddToLayer(LayerRegularPlatform);
        }

        // It accepts Object so it can be used from a GetSlideCollision(x).Collider
        public bool IsPlatform(Object? platform) => platform is PhysicsBody2D psb && psb.IsInGroup(GROUP_PLATFORMS);
        public bool IsMovingPlatform(Object? platform) => platform is PhysicsBody2D psb && psb.IsInGroup(GROUP_MOVING_PLATFORMS);
        public bool IsFallingPlatform(Object? platform) => platform is PhysicsBody2D psb && psb.IsInGroup(GROUP_FALLING_PLATFORMS);


        /**
         * Platform falling & body out
         */
        // Provoca la caida del jugador desde la plataforma quitando la mascara
        public void BodyFallFromPlatform(CharacterBody2D kb2d) {
            kb2d.IgnoreLayer(LayerFallPlatform);
        }

        public bool IsBodyFallingFromPlatform(CharacterBody2D kb2d) => !kb2d.GetCollisionMaskValue(LayerFallPlatform);
        // Para la caida del jugador

        public BodyOnArea2DEntered.Unicast PlatformBodyOutTopicBodyOnArea2D = new("PlatformBodyOut");
        
        public void BodyStopFallFromPlatform(CharacterBody2D kb2d) {
            kb2d.DetectLayer(LayerFallPlatform);
        }

        private void AddArea2DFallingPlatformExit(Area2D area2D) => PlatformBodyOutTopicBodyOnArea2D.Connect(area2D);

        public void SubscribeFallingPlatformOut(Node filter, Action<Area2D> action) =>
            PlatformBodyOutTopicBodyOnArea2D.Subscribe((area2D, _) => action(area2D)).WithFilter(filter);
    }
}