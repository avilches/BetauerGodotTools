Crear una plataforma de la que poder bajar
------------------------------------------

La plataforma debe estar marcada como layer 2. El player debe tener mask 2, de manera que colisione.
Para que baje de ahi, se debe quitar la mask 2.

1 Crear
- KB2D
    - CollisionShape

2 OPCIONAL: En el KB2D script añadir:
    func _ready():
        PlatformManager.add_kineticbody2d_platform(self)

  Esta llamada activa la colision contra el layer de plataformas de las que se puede bajar

  Si no, hay que añadir manualmente a la plataforma el layer 2 en el editor.

3 En el Player script:

    func _ready():
        PlatformManager.SubscribeFallingPlatformOut(self, "enable_platform_collide")
        PlatformManager.configure_player(self)

    func fall_from_platform():
        if is_on_floor(): PlatformManager.fall_from_platform(self)

    func enable_platform_collide():
        PlatformManager.enable_platform_collide(self)

    func _physics_process(delta):
        if Input.is_action_pressed("ui_down"): fall_from_platform()
        // MOVER
        if is_on_floor(): enable_platform_collide()

4 Si hay varias plataformas, la de arriba debe tener un Area2D que indique cuando se cruza para volver
a activar la colision. Este Area2D debe tener un script como PlatformBodyOut con:

    func _ready():
        PlatformManager.AddArea2DFallingPlatformExit(self)





