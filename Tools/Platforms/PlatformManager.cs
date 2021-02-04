using Godot;
using Godot.Collections;

namespace Betauer.Tools.Platforms {
    public class PlatformManager : Node {

        private const string GODOT_SIGNAL_body_shape_entered = "body_shape_entered";
        private const string GODOT_SIGNAL_body_shape_exited = "body_shape_exited";

        private const string GROUP_MOVING_PLATFORMS = "moving_platform";
        private const string GROUP_FALLING_PLATFORMS = "falling_platform";
        private const string GROUP_SLOPE_STAIRS = "slope_stairs";


        public static PlatformManager Instance { get; private set; }

        public override void _EnterTree() {
            Instance = this;
        }


        const int REGULAR_PLATFORM_LAYER = 0;
        const int SLOPE_STAIRS_LAYER = 1;
        const int SLOPE_STAIRS_COVER_LAYER = 2;
        const int FALL_PLATFORM_LAYER = 3;
        const int PLAYER_LAYER = 10;

        // # Configura el layer de la plataforma segun el tipo
        void register_platform(PhysicsBody2D platform) {
            GD.Print("Platform:", platform.Name);
            // platform.AddToGroup("platform");
            platform.CollisionLayer = 0;
            platform.SetCollisionLayerBit(REGULAR_PLATFORM_LAYER, true);
        }

        void register_moving_platform(KinematicBody2D platform) {
            GD.Print("Moving platform:", platform.Name);
            platform.Motion__syncToPhysics = true;
            // platform.AddToGroup("platform");
            platform.AddToGroup(GROUP_MOVING_PLATFORMS);
            platform.CollisionLayer = 0;
            platform.SetCollisionLayerBit(REGULAR_PLATFORM_LAYER, true);
        }

        void register_slope_stairs(PhysicsBody2D platform) {
            GD.Print("Slope stair platform:", platform.Name);
            platform.AddToGroup(GROUP_SLOPE_STAIRS);
            platform.CollisionLayer = 0;
            platform.SetCollisionLayerBit(SLOPE_STAIRS_LAYER, true);
        }

        void register_slope_stairs_cover(PhysicsBody2D platform) {
            GD.Print("Slope stair cover platform:", platform.Name);
            // platform.AddToGroup("slope_stairs_cover");
            platform.CollisionLayer = 0;
            platform.SetCollisionLayerBit(SLOPE_STAIRS_COVER_LAYER, true);
        }

        void register_falling_platform(PhysicsBody2D platform) {
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

        void disable_moving_platform(KinematicBody2D platform) {
            platform.RemoveFromGroup(GROUP_MOVING_PLATFORMS);
        }

        void enable_moving_platform(KinematicBody2D platform) {
            platform.RemoveFromGroup(GROUP_MOVING_PLATFORMS);
        }

        bool is_a_platform(PhysicsBody2D platform) {
            return platform.IsInGroup("platform");
        }

        public bool IsMovingPlatform(KinematicBody2D platform) {
            return platform.IsInGroup(GROUP_MOVING_PLATFORMS);
        }

        public bool IsFallingPlatform(PhysicsBody2D platform) {
            return platform.IsInGroup(GROUP_FALLING_PLATFORMS);
        }

        public bool IsSlopeStairs(PhysicsBody2D platform) {
            return platform.IsInGroup(GROUP_SLOPE_STAIRS);
        }

        public void ConfigureBodyCollisions(KinematicBody2D kb2d) {
            kb2d.SetCollisionMaskBit(REGULAR_PLATFORM_LAYER, true);
            kb2d.SetCollisionMaskBit(SLOPE_STAIRS_LAYER, false);
            kb2d.SetCollisionMaskBit(SLOPE_STAIRS_COVER_LAYER, true);
            kb2d.SetCollisionMaskBit(FALL_PLATFORM_LAYER, true);
        }

        // # Provoca la caida del jugador desde la plataforma quitando la mascara
        public void BodyFallFromPlatform(KinematicBody2D kb2d) {
            kb2d.SetCollisionMaskBit(FALL_PLATFORM_LAYER, false);
        }

        public void body_enable_slope_stairs(KinematicBody2D kb2d) {
            kb2d.SetCollisionMaskBit(SLOPE_STAIRS_LAYER, true);
        }

        public void body_disable_slope_stairs(KinematicBody2D kb2d) {
            kb2d.SetCollisionMaskBit(SLOPE_STAIRS_LAYER, false);
        }

        bool body_has_slope_stairs_enabled(KinematicBody2D kb2d) {
            return kb2d.GetCollisionMaskBit(SLOPE_STAIRS_LAYER);
        }

        public void body_enable_slope_stairs_cover(KinematicBody2D kb2d) {
            kb2d.SetCollisionMaskBit(SLOPE_STAIRS_COVER_LAYER, true);
        }

        public void body_disable_slope_stairs_cover(KinematicBody2D kb2d) {
            kb2d.SetCollisionMaskBit(SLOPE_STAIRS_COVER_LAYER, false);
        }

        bool body_has_slope_stairs_cover_enabled(KinematicBody2D kb2d) {
            return kb2d.GetCollisionMaskBit(SLOPE_STAIRS_COVER_LAYER);
        }

        // # ¿Esta el jugador cayendo de una plataforma?
        bool is_body_falling_from_platform(KinematicBody2D kb2d) {
            return kb2d.GetCollisionMaskBit(FALL_PLATFORM_LAYER) == false;
        }

        // # Deja la mascara como estaba (cuando toca el suelo o cuando sale del area2d que ocupa la plataforma)
        public void body_stop_falling_from_platform(KinematicBody2D kb2d) {
            kb2d.SetCollisionMaskBit(FALL_PLATFORM_LAYER, true);
        }

        [Signal]
        public delegate void platform_fall_started();

        // # añade un area2D en la que cualquier objeto que la traspase, enviara una señal
        // # Suscribirse a esta señal desde el jugador para llamar a body_stop_falling_from_platform
        void add_area2d_platform_exit(Area2D area2D) {
            area2D.Connect(GODOT_SIGNAL_body_shape_entered, this, nameof(_on_Area2D_platform_exit_body_shape_entered));
        }

        void _on_Area2D_platform_exit_body_shape_entered(Object body_id, Node body, Node body_shape, Node area_shape) {
            EmitSignal(nameof(platform_fall_started));
        }

// # se suscribe a la señal de cualquier plataforma de la que se caiga (no importa cual)
        public void subscribe_platform_out(Object o, string f) {
            this.Connect("platform_fall_started", o, f);
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

        // # añade un area2D en la que cualquier objeto que la traspase, enviara una señal
        // # Suscribirse a esta señal desde el jugador para llamar a body_*
        void add_area2d_slope_stairs_down(Area2D area2D) {
            area2D.Connect(GODOT_SIGNAL_body_shape_entered, this, nameof(_on_Area2D_slope_stairs_down_body_shape_entered), new Array() {area2D});
            area2D.Connect(GODOT_SIGNAL_body_shape_exited, this, nameof(_on_Area2D_slope_stairs_down_body_shape_exited), new Array() {area2D});
        }

        public void AddArea2DSlopeStairsUp(Area2D area2D) {
            area2D.Connect(GODOT_SIGNAL_body_shape_entered, this, nameof(_on_Area2D_slope_stairs_up_body_shape_entered),
                new Array() {area2D});
            area2D.Connect(GODOT_SIGNAL_body_shape_exited, this, nameof(_on_Area2D_slope_stairs_up_body_shape_exited),
                new Array() {area2D});
        }

        void add_area2d_slope_stairs_enabler(Area2D area2D) {
            area2D.Connect(GODOT_SIGNAL_body_shape_entered, this, nameof(_on_Area2D_slope_stairs_enabler_body_shape_entered),
                new Array() {area2D});
            area2D.Connect(GODOT_SIGNAL_body_shape_exited, this, nameof(_on_Area2D_slope_stairs_enabler_body_shape_exited),
                new Array() {area2D});
        }

        void add_area2d_slope_stairs_disabler(Area2D area2D) {
            area2D.Connect(GODOT_SIGNAL_body_shape_entered, this, nameof(_on_Area2D_slope_stairs_disabler_body_shape_entered),
                new Array() {area2D});
            area2D.Connect(GODOT_SIGNAL_body_shape_exited, this, nameof(_on_Area2D_slope_stairs_disabler_body_shape_exited),
                new Array() {area2D});
        }

        void _on_Area2D_slope_stairs_down_body_shape_entered(int bodyId, Node body, int bodyShape, int areaShape, Area2D area2D) {
            EmitSignal(nameof(slope_stairs_down_in), body, area2D);
        }

        void _on_Area2D_slope_stairs_down_body_shape_exited(int bodyId, Node body, int bodyShape, int areaShape, Area2D area2D) {
            EmitSignal(nameof(slope_stairs_down_out), body, area2D);
        }

        void _on_Area2D_slope_stairs_up_body_shape_entered(int bodyId, Node body, int bodyShape, int areaShape, Area2D area2D) {
            EmitSignal(nameof(slope_stairs_up_in), body, area2D);
        }

        void _on_Area2D_slope_stairs_up_body_shape_exited(int bodyId, Node body, int bodyShape, int areaShape, Area2D area2D) {
            EmitSignal(nameof(slope_stairs_up_out), body, area2D);
        }

        void _on_Area2D_slope_stairs_enabler_body_shape_entered(int bodyId, Node body, int bodyShape, int areaShape, Area2D area2D) {
            EmitSignal(nameof(slope_stairs_enabler_in), body, area2D);
        }

        void _on_Area2D_slope_stairs_enabler_body_shape_exited(int bodyId, Node body, int bodyShape, int areaShape, Area2D area2D) {
            EmitSignal(nameof(slope_stairs_enabler_out), body, area2D);
        }

        void _on_Area2D_slope_stairs_disabler_body_shape_entered(int bodyId, Node body, int bodyShape, int areaShape, Area2D area2D) {
            EmitSignal(nameof(slope_stairs_disabler_in), body, area2D);
        }

        void _on_Area2D_slope_stairs_disabler_body_shape_exited(int bodyId, Node body, int bodyShape, int areaShape, Area2D area2D) {
            EmitSignal(nameof(slope_stairs_disabler_out), body, area2D);
        }

        // # se suscribe a la señal de cualquier entrada a slope stairs

        public void subscribe_slope_stairs_down(Object o, string f_in, string f_out = null) {
            Connect(nameof(slope_stairs_down_in), o, f_in);
            if (f_out != null) {
                Connect(nameof(slope_stairs_down_out), o, f_out);
            }
        }

        // void on_slope_stairs_down_flag(Object o, flag) {
            // this.Connect("slope_stairs_down_in", this, "_enable_flag", [o, flag])
            // this.Connect("slope_stairs_down_out", this, "_disable_flag", [o, flag])
        // }

        public void subscribe_slope_stairs_up(Object o, string f_in, string f_out = null) {
            Connect(nameof(slope_stairs_up_in), o, f_in);
            if (f_out != null) {
                Connect(nameof(slope_stairs_up_out), o, f_out);
            }
        }

        // void on_slope_stairs_up_flag(Object o, flag) {
            // this.Connect("slope_stairs_up_in", this, "_enable_flag", [o, flag])
            // this.Connect("slope_stairs_up_out", this, "_disable_flag", [o, flag])
        // }

        public void subscribe_slope_stairs_enabler(Object o, string f_in, string f_out = null) {
            Connect(nameof(slope_stairs_enabler_in), o, f_in);
            if (f_out != null) {
                this.Connect(nameof(slope_stairs_enabler_out), o, f_out);
            }
        }

        public void subscribe_slope_stairs_disabler(Object o, string f_in, string f_out = null) {
            this.Connect(nameof(slope_stairs_disabler_in), o, f_in);
            if (f_out != null) {
                this.Connect(nameof(slope_stairs_disabler_out), o, f_out);
            }
        }

        // void _enable_flag(body, area2d, o, string flag) {
            // if (body == o) {
                // o[flag] = true;
            // }
        // }

        // void _disable_flag(body, area2d, o, string flag) {
            // if (body == o) {
                // o[flag] = false;
            // }
        // }
    }
}