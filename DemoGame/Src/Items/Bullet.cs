using Betauer.Core.Pool;
using Godot;

namespace Veronenger.Items;

public partial class Bullet : Node, IBusyElement {
    private WeaponRangeItem _weapon;
    private Vector2 _from;
    private Vector2 _velocity;
    private Line2D _ray;
    private float _maxDistanceSquared;
    private volatile bool _busy = false;
    private Sprite2D _sprite2D;

    public Vector2 StartTrailPos {
        get => _ray.GetPointPosition(0);
        set => _ray.SetPointPosition(0, value);
    }

    public Vector2 EndTrailPos {
        get => _ray.GetPointPosition(1);
        set => _ray.SetPointPosition(1, value);
    }

    public bool IsBusy() => _busy;

    public Bullet(Node parent, Texture2D icon) {
        _ray = new Line2D {
            Width = 1f,
            DefaultColor = Colors.DimGray,
            Visible = false
        };
        _ray.AddPoint(Vector2.Zero);
        _ray.AddPoint(Vector2.Zero);
        _sprite2D = new Sprite2D {
            Texture = icon,
            Scale = new Vector2(0.05f, 0.05f),
            Visible = false
        };
        AddChild(_ray);
        AddChild(_sprite2D);
        parent.AddChild(this);
    }

    public void ShootFrom(Vector2 from, WeaponRangeItem item, Vector2 direction) {
        SetPhysicsProcess(true);
        _busy = true;
        _weapon = item;
        _velocity = direction * item.Config.Speed;
        _maxDistanceSquared = Mathf.Pow(item.Config.MaxDistance, 2);
        
        _sprite2D.Position = _from = StartTrailPos = EndTrailPos = from;
        _sprite2D.Visible = _ray.Visible = true;
    }
    
    public override void _PhysicsProcess(double delta) {
        var newBulletPosition = _sprite2D.Position + _velocity * (float)delta;
        if (newBulletPosition.DistanceSquaredTo(_from) > _maxDistanceSquared) {
            GD.Print(newBulletPosition.DistanceTo(_from).ToString());
            EndShoot();
            return;
        }
        _sprite2D.Position = newBulletPosition;
        StartTrailPos = newBulletPosition;
        // var endTrailPos = EndTrailPos;
        // if (newBulletPosition.DistanceSquaredTo(endTrailPos) > Mathf.Pow(100, 2)) {
            // EndTrailPos = endTrailPos + _velocity * (float)delta;
        // }
    }

    public void EndShoot() {
        SetPhysicsProcess(false);
        _sprite2D.Visible = _ray.Visible = false;
        _weapon = null;
        _busy = false;
    }

}