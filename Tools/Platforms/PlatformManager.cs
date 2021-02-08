using Betauer.Tools.Character;
using Godot;
using static Betauer.Tools.LayerConstants;
using Godot.Collections;

namespace Betauer.Tools.Platforms {
    public class PlatformManager : Node {
        private const string GROUP_MOVING_PLATFORMS = "moving_platform";
        private const string GROUP_FALLING_PLATFORMS = "falling_platform";
        private const string GROUP_SLOPE_STAIRS = "slope_stairs";


        public override void _EnterTree() {
            GameManager.Instance.AddManager(this);
        }

        // Configura el layer de la plataforma segun el tipo
        void RegisterPlatform(PhysicsBody2D platform) {
            GD.Print("Platform:", platform.Name);
            // platform.AddToGroup("platform");
            platform.CollisionLayer = 0;
            platform.SetCollisionLayerBit(REGULAR_PLATFORM_LAYER, true);
        }

        void RegisterMovingPlatform(KinematicBody2D platform) {
            GD.Print("Moving platform:", platform.Name);
            platform.Motion__syncToPhysics = true;
            // platform.AddToGroup("platform");
            platform.AddToGroup(GROUP_MOVING_PLATFORMS);
            platform.CollisionLayer = 0;
            platform.SetCollisionLayerBit(REGULAR_PLATFORM_LAYER, true);
        }

        void RegisterSlopeStairs(PhysicsBody2D platform) {
            GD.Print("Slope stair platform:", platform.Name);
            platform.AddToGroup(GROUP_SLOPE_STAIRS);
            platform.CollisionLayer = 0;
            platform.SetCollisionLayerBit(SLOPE_STAIRS_LAYER, true);
        }

        void RegisterSlopeStairsCover(PhysicsBody2D platform) {
            GD.Print("Slope stair cover platform:", platform.Name);
            // platform.AddToGroup("slope_stairs_cover");
            platform.CollisionLayer = 0;
            platform.SetCollisionLayerBit(SLOPE_STAIRS_COVER_LAYER, true);
        }

        void RegisterFallingPlatform(PhysicsBody2D platform) {
            GD.Print("Falling platform:", platform.Name);
            foreach (var col in platform.GetChildren()) {
                if (col is CollisionShape2D colShape2d) {
                    colShape2d.OneWayCollision = true;
                }

                platform.AddToGroup(GROUP_FALLING_PLATFORMS);
                platform.CollisionLayer = 0;
                platform.SetCollisionLayerBit(FALL_PLATFORM_LAYER, true);
            }
        }

        public void RegisterPlayer(CharacterController kb2d) {
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

        [Signal]
        public delegate void platform_fall_started();

        // añade un area2D en la que cualquier objeto que la traspase, enviara una señal
        // Suscribirse a esta señal desde el jugador para llamar a BodyStopFallFromPlatform
        void AddArea2DFallingPlatformExit(Area2D area2D) {
            area2D.Connect(GodotConstants.GODOT_SIGNAL_body_shape_entered, this, nameof(_on_Area2D_platform_exit_body_shape_entered),
                new Array() {area2D});
        }

        void _on_Area2D_platform_exit_body_shape_entered(int bodyId, Node body, int bodyShape, int areaShape,
            Area2D area2D) {
            EmitSignal(nameof(platform_fall_started), body, area2D);
        }

        //Se suscribe a la señal de cualquier plataforma de la que se caiga (no importa cual)
        public void SubscribeFallingPlatformOut(Object o, string f) {
            Connect(nameof(platform_fall_started), o, f);
        }

        [Signal]
        public delegate void slope_stairs_down_in(Node body, Area2D area2D);

        [Signal]
        public delegate void slope_stairs_down_out(Node body, Area2D area2D);

        [Signal]
        public delegate void slope_stairs_up_in(Node body, Area2D area2D);

        [Signal]
        public delegate void slope_stairs_up_out(Node body, Area2D area2D);

        [Signal]
        public delegate void slope_stairs_enabler_in(Node body, Area2D area2D);

        [Signal]
        public delegate void slope_stairs_enabler_out(Node body, Area2D area2D);

        [Signal]
        public delegate void slope_stairs_disabler_in(Node body, Area2D area2D);

        [Signal]
        public delegate void slope_stairs_disabler_out(Node body, Area2D area2D);

        // añade un area2D en la que cualquier objeto que la traspase, enviara una señal
        // Suscribirse a esta señal desde el jugador para llamar a body_*
        void AddArea2DSlopeStairsDown(Area2D area2D) {
            area2D.Connect(GodotConstants.GODOT_SIGNAL_body_shape_entered, this,
                nameof(_on_Area2D_slope_stairs_down_body_shape_entered), new Array() {area2D});
            area2D.Connect(GodotConstants.GODOT_SIGNAL_body_shape_exited, this, nameof(_on_Area2D_slope_stairs_down_body_shape_exited),
                new Array() {area2D});
        }

        public void AddArea2DSlopeStairsUp(Area2D area2D) {
            area2D.Connect(GodotConstants.GODOT_SIGNAL_body_shape_entered, this, nameof(_on_Area2D_slope_stairs_up_body_shape_entered),
                new Array() {area2D});
            area2D.Connect(GodotConstants.GODOT_SIGNAL_body_shape_exited, this, nameof(_on_Area2D_slope_stairs_up_body_shape_exited),
                new Array() {area2D});
        }

        void AddArea2DSlopeStairsEnabler(Area2D area2D) {
            area2D.Connect(GodotConstants.GODOT_SIGNAL_body_shape_entered, this,
                nameof(_on_Area2D_slope_stairs_enabler_body_shape_entered),
                new Array() {area2D});
            area2D.Connect(GodotConstants.GODOT_SIGNAL_body_shape_exited, this,
                nameof(_on_Area2D_slope_stairs_enabler_body_shape_exited),
                new Array() {area2D});
        }

        void AddArea2DSlopeStairsDisabler(Area2D area2D) {
            area2D.Connect(GodotConstants.GODOT_SIGNAL_body_shape_entered, this,
                nameof(_on_Area2D_slope_stairs_disabler_body_shape_entered),
                new Array() {area2D});
            area2D.Connect(GodotConstants.GODOT_SIGNAL_body_shape_exited, this,
                nameof(_on_Area2D_slope_stairs_disabler_body_shape_exited),
                new Array() {area2D});
        }

        void _on_Area2D_slope_stairs_down_body_shape_entered(int bodyId, Node body, int bodyShape, int areaShape,
            Area2D area2D) {
            EmitSignal(nameof(slope_stairs_down_in), body, area2D);
        }

        void _on_Area2D_slope_stairs_down_body_shape_exited(int bodyId, Node body, int bodyShape, int areaShape,
            Area2D area2D) {
            EmitSignal(nameof(slope_stairs_down_out), body, area2D);
        }

        void _on_Area2D_slope_stairs_up_body_shape_entered(int bodyId, Node body, int bodyShape, int areaShape,
            Area2D area2D) {
            EmitSignal(nameof(slope_stairs_up_in), body, area2D);
        }

        void _on_Area2D_slope_stairs_up_body_shape_exited(int bodyId, Node body, int bodyShape, int areaShape,
            Area2D area2D) {
            EmitSignal(nameof(slope_stairs_up_out), body, area2D);
        }

        void _on_Area2D_slope_stairs_enabler_body_shape_entered(int bodyId, Node body, int bodyShape, int areaShape,
            Area2D area2D) {
            EmitSignal(nameof(slope_stairs_enabler_in), body, area2D);
        }

        void _on_Area2D_slope_stairs_enabler_body_shape_exited(int bodyId, Node body, int bodyShape, int areaShape,
            Area2D area2D) {
            EmitSignal(nameof(slope_stairs_enabler_out), body, area2D);
        }

        void _on_Area2D_slope_stairs_disabler_body_shape_entered(int bodyId, Node body, int bodyShape, int areaShape,
            Area2D area2D) {
            EmitSignal(nameof(slope_stairs_disabler_in), body, area2D);
        }

        void _on_Area2D_slope_stairs_disabler_body_shape_exited(int bodyId, Node body, int bodyShape, int areaShape,
            Area2D area2D) {
            EmitSignal(nameof(slope_stairs_disabler_out), body, area2D);
        }

        public void SubscribeSlopeStairsDown(Object o, string f_in, string f_out = null) {
            Connect(nameof(slope_stairs_down_in), o, f_in);
            if (f_out != null) {
                Connect(nameof(slope_stairs_down_out), o, f_out);
            }
        }

        public void SubscribeSlopeStairsUp(Object o, string f_in, string f_out = null) {
            Connect(nameof(slope_stairs_up_in), o, f_in);
            if (f_out != null) {
                Connect(nameof(slope_stairs_up_out), o, f_out);
            }
        }

        public void SubscribeSlopeStairsEnabler(Object o, string f_in, string f_out = null) {
            Connect(nameof(slope_stairs_enabler_in), o, f_in);
            if (f_out != null) {
                Connect(nameof(slope_stairs_enabler_out), o, f_out);
            }
        }

        public void SubscribeSlopeStairsDisabler(Object o, string f_in, string f_out = null) {
            Connect(nameof(slope_stairs_disabler_in), o, f_in);
            if (f_out != null) {
                Connect(nameof(slope_stairs_disabler_out), o, f_out);
            }
        }
    }
}