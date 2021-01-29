using Godot;
using System;

public abstract class CharacterController : KinematicBody2D {

    protected CharacterConfig CharacterConfig;
    public float Delta { get; private set; } = 0.16f;
    public Vector2 Motion  = Vector2.Zero;
    private Vector2 _lastMotion = Vector2.Zero;

    public void SetMotion(Vector2 motion) {
        this.Motion = motion;
    }

    public void SetMotionX(float x) {
        Motion.x = x;
    }

    public void SetMotionY(float x) {
        Motion.x = x;
    }

    public void ApplyGravity(float factor = 1.0F) {
        Motion.y += CharacterConfig.GRAVITY * factor * Delta;
    }

    public sealed override void _PhysicsProcess(float delta) {
        Delta = delta;
        _lastMotion = Motion;
        PhysicsProcess();
        if (CharacterConfig.DEBUG_MOTION && Motion != _lastMotion) {
            GD.Print("Motion:"+Motion+" (diff "+(_lastMotion-Motion)+")");
        }
    }

    protected abstract void PhysicsProcess();

    public void LimitMotion(float maxSpeedFactor = 1.0F) {
        var realMaxSpeed = CharacterConfig.MAX_SPEED * maxSpeedFactor;
        Motion.x = Mathf.Clamp(Motion.x, -realMaxSpeed, realMaxSpeed);
        Motion.y = Mathf.Min(Motion.y, CharacterConfig.MAX_FALLING_SPEED);  //  avoid gravity continue forever in free fall
    }

    public void MoveSnapping() {
        MoveSnapping(Vector2.One);
    }

    public void MoveSnapping(Vector2 slowdownVector) {
        /* stopOnSlopes debe ser true para que no se resbale en las pendientes, pero tiene que ser false si se
        mueve una plataforma y nos queremos quedar pegados
        Hay un bug conocido: si la plataforma que se mueve tiene slope, entonces para que detected el slope como suelo,
        se para y ya no sigue a la plataforma
        */
        var stopOnSlopes = !HasFloorLateralMovement();
        var remain = MoveAndSlideWithSnap(Motion * slowdownVector, CharacterConfig.SLOPE_RAYCAST_VECTOR,
            CharacterConfig.FLOOR, stopOnSlopes);
        Motion.y = remain.y; // this line stops the gravity accumulation
        // motion.x = remain.x:  // WARNING!! this line should be always commented, player can't climb slopes with it!!
    }

    public void Slide() {
        Slide(Vector2.One);
    }

    public void Slide(Vector2 slowdownVector) {
        // stopOnSlopes debe ser true para al caer sobre una pendiente la tome comoelo
        var stopOnSlopes = true;
        var remain = MoveAndSlideWithSnap(Motion * slowdownVector, Vector2.Zero, CharacterConfig.FLOOR, stopOnSlopes);
        Motion.y = remain.y; // this line stops the gravity accumulation
		/*
        inertia false = se mantiene el remain.x = al chocar con la cabeza pierde toda la inercia lateral que tenia y se va para abajo. Y si choca al subir y se
        se sube, pierde tambien la inercia teniendo que alecerar desde 0

        intertia true = se pierde el remain.x = al saltar y chocar (temporalmente) con un objeto hace que al dejar de chocar, recupere
        totalmente la movilidad = si choca justo antes de subir y luego se sube, corre a tope. Si choca con la cabeza y baja un poco,
        cuando de chocar, continua hacia delante a tope.
        */
        var inertia = false;

        if (!inertia) {
            Motion.x = remain.x;
        }

    }
    public bool HasFloorLateralMovement() {
        return GetFloorVelocity().x != 0;
    }
}