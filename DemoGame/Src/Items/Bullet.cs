using Betauer.Core.Pool;
using Godot;

namespace Veronenger.Items;

public partial class Bullet : Sprite2D, IBusyElement {
    private volatile bool _busy = false;
    private WeaponRangeItem _weapon;
    private Vector2 _from;
    private Vector2 _velocity;

    public Bullet(Node parent, Texture2D icon) {
        parent.AddChild(this);
        Texture = icon;
        Scale = new Vector2(0.05f, 0.05f);
        Visible = false;
    }

    public void ShootFrom(Vector2 from, WeaponRangeItem item, Vector2 direction) {
        SetPhysicsProcess(true);
        _busy = true;
        _weapon = item;
        _velocity = direction * item.Model.Speed;
        GlobalPosition = _from = from;
        Visible = true;
    }

    public void EndShoot() {
        SetPhysicsProcess(false);
        Visible = false;
        _weapon = null;
        _busy = false;
    }


    public override void _PhysicsProcess(double delta) {
        Position += _velocity * (float)delta;
        if (Position.DistanceTo(_from) > _weapon.Model.MaxDistance) EndShoot();
    }

    public bool IsBusy() => _busy;
}