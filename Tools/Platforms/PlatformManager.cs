using Betauer.Tools.Character;
using Betauer.Tools.Events;
using Godot;
using static Betauer.Tools.LayerConstants;
using Godot.Collections;

namespace Betauer.Tools.Platforms {
    public class PlatformManager : Node {
        private const string GROUP_MOVING_PLATFORMS = "moving_platform";
        private const string GROUP_FALLING_PLATFORMS = "falling_platform";
        private const string GROUP_SLOPE_STAIRS = "slope_stairs";

        /**
         * No almacena nada, solo permite que characters se suscriban a cambios en una plataforma
         * y permite cambiar sus mascaras.
         */

        // Configura el layer de la plataforma segun el tipo
        void RegisterPlatform(PhysicsBody2D platform) {
            Debug.Register("PlatformManager.Platform", platform);
            // platform.AddToGroup("platform");
            platform.CollisionMask = 0;
            platform.CollisionLayer = 0;
            platform.SetCollisionLayerBit(REGULAR_PLATFORM_LAYER, true);
        }

        void RegisterMovingPlatform(KinematicBody2D platform) {
            Debug.Register("PlatformManager.Moving platform", platform);
            platform.Motion__syncToPhysics = true;
            // platform.AddToGroup("platform");
            platform.AddToGroup(GROUP_MOVING_PLATFORMS);
            platform.CollisionMask = 0;
            platform.CollisionLayer = 0;
            platform.SetCollisionLayerBit(REGULAR_PLATFORM_LAYER, true);
        }

        void RegisterSlopeStairs(PhysicsBody2D platform) {
            Debug.Register("PlatformManager.Slope stair", platform);
            platform.AddToGroup(GROUP_SLOPE_STAIRS);
            platform.CollisionMask = 0;
            platform.CollisionLayer = 0;
            platform.SetCollisionLayerBit(SLOPE_STAIRS_LAYER, true);
        }

        void RegisterSlopeStairsCover(PhysicsBody2D platform) {
            Debug.Register("PlatformManager.Slope stair cover", platform);
            // platform.AddToGroup("slope_stairs_cover");
            platform.CollisionMask = 0;
            platform.CollisionLayer = 0;
            platform.SetCollisionLayerBit(SLOPE_STAIRS_COVER_LAYER, true);
        }

        void RegisterFallingPlatform(PhysicsBody2D platform) {
            Debug.Register("PlatformManager.Falling platform", platform);
            foreach (var col in platform.GetChildren()) {
                if (col is CollisionShape2D colShape2d) {
                    colShape2d.OneWayCollision = true;
                }

                platform.AddToGroup(GROUP_FALLING_PLATFORMS);
                platform.CollisionMask = 0;
                platform.CollisionLayer = 0;
                platform.SetCollisionLayerBit(FALL_PLATFORM_LAYER, true);
            }
        }

        public void RegisterPlayer(CharacterController kb2d) {
            Debug.Register("PlatformManager.PlayerController", kb2d);
            kb2d.SetCollisionMaskBit(REGULAR_PLATFORM_LAYER, true);
            kb2d.SetCollisionMaskBit(SLOPE_STAIRS_LAYER, false);
            kb2d.SetCollisionMaskBit(SLOPE_STAIRS_COVER_LAYER, true);
            kb2d.SetCollisionMaskBit(FALL_PLATFORM_LAYER, true);
        }

        // void disable_moving_platform(KinematicBody2D platform) {
        // platform.RemoveFromGroup(GROUP_MOVING_PLATFORMS);
        // }

        // void enable_moving_platform(KinematicBody2D platform) {
        // platform.RemoveFromGroup(GROUP_MOVING_PLATFORMS);
        // }

        // bool is_a_platform(PhysicsBody2D platform) {
        // return platform.IsInGroup("platform");
        // }

        public bool IsMovingPlatform(KinematicBody2D platform) {
            return platform.IsInGroup(GROUP_MOVING_PLATFORMS);
        }

        public bool IsFallingPlatform(PhysicsBody2D platform) {
            return platform.IsInGroup(GROUP_FALLING_PLATFORMS);
        }

        public bool IsSlopeStairs(PhysicsBody2D platform) {
            return platform.IsInGroup(GROUP_SLOPE_STAIRS);
        }

        // Provoca la caida del jugador desde la plataforma quitando la mascara
        public void BodyFallFromPlatform(KinematicBody2D kb2d) {
            kb2d.SetCollisionMaskBit(FALL_PLATFORM_LAYER, false);
        }

        public void EnableSlopeStairsForBody(KinematicBody2D kb2d) {
            kb2d.SetCollisionMaskBit(SLOPE_STAIRS_LAYER, true);
        }

        public void DisableSlopeStairsForBody(KinematicBody2D kb2d) {
            kb2d.SetCollisionMaskBit(SLOPE_STAIRS_LAYER, false);
        }

        public bool HasBodyEnabledSlopeStairs(KinematicBody2D kb2d) {
            return kb2d.GetCollisionMaskBit(SLOPE_STAIRS_LAYER);
        }

        public void EnableSlopeStairsCoverForBody(KinematicBody2D kb2d) {
            kb2d.SetCollisionMaskBit(SLOPE_STAIRS_COVER_LAYER, true);
        }

        public void DisableSlopeStairsCoverForBody(KinematicBody2D kb2d) {
            kb2d.SetCollisionMaskBit(SLOPE_STAIRS_COVER_LAYER, false);
        }

        public bool HasBodyEnabledSlopeStairsCover(KinematicBody2D kb2d) {
            return kb2d.GetCollisionMaskBit(SLOPE_STAIRS_COVER_LAYER);
        }

        // ¿Esta el jugador cayendo de una plataforma?
        public bool IsBodyFallingFromPlatform(KinematicBody2D kb2d) {
            return kb2d.GetCollisionMaskBit(FALL_PLATFORM_LAYER) == false;
        }

        // Deja la mascara como estaba (cuando toca el suelo o cuando sale del area2d que ocupa la plataforma)
        public void BodyStopFallFromPlatform(KinematicBody2D kb2d) {
            kb2d.SetCollisionMaskBit(FALL_PLATFORM_LAYER, true);
        }

        // añade un area2D en la que cualquier objeto que la traspase, enviara una señal
        // Suscribirse a esta señal desde el jugador para llamar a BodyStopFallFromPlatform
        void AddArea2DFallingPlatformExit(Area2D area2D) {
            area2D.Connect(GodotConstants.GODOT_SIGNAL_body_entered, this, nameof(PlatformBodyOut_BodyEntered),
                new Array {area2D});
        }

        private GodotUnicastTopic<BodyOnArea2D> _platformBodyOut_BodyEnteredTopic =
            new GodotUnicastTopic<BodyOnArea2D>();

        void PlatformBodyOut_BodyEntered(Node body, Area2D area2D) =>
            _platformBodyOut_BodyEnteredTopic.Publish(new BodyOnArea2D(body, area2D));

        //Se suscribe a la señal de cualquier plataforma de la que se caiga (no importa cual)
        public void SubscribeFallingPlatformOut(NodeFromListenerDelegate<BodyOnArea2D> enterListener) =>
            _platformBodyOut_BodyEnteredTopic.Subscribe(enterListener);

        // añade un area2D en la que cualquier objeto que la traspase, enviara una señal
        // Suscribirse a esta señal desde el jugador para llamar a body_*
        void AddArea2DSlopeStairsDown(Area2D area2D) {
            area2D.Connect(GodotConstants.GODOT_SIGNAL_body_entered, this,
                nameof(SlopeStairsDown_BodyEntered), new Array {area2D});
            area2D.Connect(GodotConstants.GODOT_SIGNAL_body_exited, this,
                nameof(SlopeStairsDown_BodyExited),
                new Array {area2D});
        }

        public void AddArea2DSlopeStairsUp(Area2D area2D) {
            area2D.Connect(GodotConstants.GODOT_SIGNAL_body_entered, this,
                nameof(SlopeStairsUp_BodyEntered),
                new Array {area2D});
            area2D.Connect(GodotConstants.GODOT_SIGNAL_body_exited, this,
                nameof(SlopeStairsUp_BodyExited),
                new Array {area2D});
        }

        void AddArea2DSlopeStairsEnabler(Area2D area2D) {
            area2D.Connect(GodotConstants.GODOT_SIGNAL_body_entered, this,
                nameof(SlopeStairsEnabler_BodyEntered),
                new Array {area2D});
            area2D.Connect(GodotConstants.GODOT_SIGNAL_body_exited, this,
                nameof(SlopeStairsEnabler_BodyExited),
                new Array {area2D});
        }

        void AddArea2DSlopeStairsDisabler(Area2D area2D) {
            area2D.Connect(GodotConstants.GODOT_SIGNAL_body_entered, this,
                nameof(SlopeStairsDisabler_BodyEntered),
                new Array {area2D});
            area2D.Connect(GodotConstants.GODOT_SIGNAL_body_exited, this,
                nameof(SlopeStairsDisabler_BodyExited),
                new Array {area2D});
        }

        private GodotUnicastTopic<BodyOnArea2D> _slopeStairsDown_BodyEnteredTopic =
            new GodotUnicastTopic<BodyOnArea2D>();

        void SlopeStairsDown_BodyEntered(Node body, Area2D area2D) =>
            _slopeStairsDown_BodyEnteredTopic.Publish(new BodyOnArea2D(body, area2D));

        private GodotUnicastTopic<BodyOnArea2D> _slopeStairsDown_BodyExitedTopic =
            new GodotUnicastTopic<BodyOnArea2D>();

        void SlopeStairsDown_BodyExited(Node body, Area2D area2D) =>
            _slopeStairsDown_BodyExitedTopic.Publish(new BodyOnArea2D(body, area2D));

        private GodotUnicastTopic<BodyOnArea2D> _slopeStairsUp_BodyEnteredTopic =
            new GodotUnicastTopic<BodyOnArea2D>();

        void SlopeStairsUp_BodyEntered(Node body, Area2D area2D) =>
            _slopeStairsUp_BodyEnteredTopic.Publish(new BodyOnArea2D(body, area2D));

        private GodotUnicastTopic<BodyOnArea2D> _slopeStairsUp_BodyExitedTopic =
            new GodotUnicastTopic<BodyOnArea2D>();

        void SlopeStairsUp_BodyExited(Node body, Area2D area2D) =>
            _slopeStairsUp_BodyExitedTopic.Publish(new BodyOnArea2D(body, area2D));

        private GodotUnicastTopic<BodyOnArea2D> _slopeStairsEnabler_BodyEnteredTopic =
            new GodotUnicastTopic<BodyOnArea2D>();

        void SlopeStairsEnabler_BodyEntered(Node body, Area2D area2D) =>
            _slopeStairsEnabler_BodyEnteredTopic.Publish(new BodyOnArea2D(body, area2D));

        private GodotUnicastTopic<BodyOnArea2D> _slopeStairsEnabler_BodyExitedTopic =
            new GodotUnicastTopic<BodyOnArea2D>();

        void SlopeStairsEnabler_BodyExited(Node body, Area2D area2D) =>
            _slopeStairsEnabler_BodyExitedTopic.Publish(new BodyOnArea2D(body, area2D));

        private GodotUnicastTopic<BodyOnArea2D> _slopeStairsDisabler_BodyEnteredTopic =
            new GodotUnicastTopic<BodyOnArea2D>();

        void SlopeStairsDisabler_BodyEntered(Node body, Area2D area2D) =>
            _slopeStairsDisabler_BodyEnteredTopic.Publish(new BodyOnArea2D(body, area2D), "disabler entered");

        private GodotUnicastTopic<BodyOnArea2D> _slopeStairsDisabler_BodyExitedTopic =
            new GodotUnicastTopic<BodyOnArea2D>();

        void SlopeStairsDisabler_BodyExited(Node body, Area2D area2D) =>
            _slopeStairsDisabler_BodyExitedTopic.Publish(new BodyOnArea2D(body, area2D), "disabler exited");

        public void SubscribeSlopeStairsDown(NodeFromListenerDelegate<BodyOnArea2D> enterListener,
            NodeFromListenerDelegate<BodyOnArea2D> exitListener = null) {
            _slopeStairsDown_BodyEnteredTopic.Subscribe(enterListener);
            if (exitListener != null) _slopeStairsDown_BodyExitedTopic.Subscribe(exitListener);
        }

        public void SubscribeSlopeStairsUp(NodeFromListenerDelegate<BodyOnArea2D> enterListener,
            NodeFromListenerDelegate<BodyOnArea2D> exitListener = null) {
            _slopeStairsUp_BodyEnteredTopic.Subscribe(enterListener);
            if (exitListener != null) _slopeStairsUp_BodyExitedTopic.Subscribe(exitListener);
        }

        public void SubscribeSlopeStairsEnabler(NodeFromListenerDelegate<BodyOnArea2D> enterListener,
            NodeFromListenerDelegate<BodyOnArea2D> exitListener = null) {
            _slopeStairsEnabler_BodyEnteredTopic.Subscribe(enterListener);
            if (exitListener != null) _slopeStairsEnabler_BodyExitedTopic.Subscribe(exitListener);
        }

        public void SubscribeSlopeStairsDisabler(NodeFromListenerDelegate<BodyOnArea2D> enterListener,
            NodeFromListenerDelegate<BodyOnArea2D> exitListener = null) {
            _slopeStairsDisabler_BodyEnteredTopic.Subscribe(enterListener);
            if (exitListener != null) _slopeStairsDisabler_BodyExitedTopic.Subscribe(exitListener);
        }
    }
}