using Betauer.Core.Pool;
using Godot;

namespace Veronenger.Items;

public partial class BulletTrail : Line2D, IBusyElement {
	private Vector2 _from;
	private Vector2 _velocity;
	private float _maxDistanceSquared;
	private float _trailLongSquared;
	private volatile bool _busy = false;
	public Sprite2D Sprite2D;

	public Vector2 EndTrailPos => GetPointPosition(2);

	public void SetTrail(Vector2 start, Vector2 end) {
		SetPointPosition(0, start);
		SetPointPosition(1, start.Lerp(end, 0.5f));
		SetPointPosition(2, end);
	}

	public bool IsBusy() => _busy;

	public BulletTrail Configure(Node parent) {
		Visible = false;
		Width = 1f;
		ClearPoints();
		AddPoint(Vector2.Zero);
		AddPoint(Vector2.Zero);
		AddPoint(Vector2.Zero);
		parent.AddChild(this);
		Sprite2D = new Sprite2D {
			Visible = false
		};
		AddChild(Sprite2D);
		return this;
	}

	public void ShootFrom(WeaponRangeItem item, Vector2 from, Vector2 direction) {
		SetPhysicsProcess(true);
		_busy = true;
		_velocity = direction * item.Config.Speed;
		_maxDistanceSquared = Mathf.Pow(item.Config.MaxDistance, 2);
		_trailLongSquared = Mathf.Pow(item.Config.TrailLong, 2);
		
		Sprite2D.Position = from;
		Sprite2D.Texture = item.Config.Bullet;
		Sprite2D.Visible = item.Config.Bullet != null;
		
		SetTrail(from, from);
		Visible = true;
	}
	
	public override void _PhysicsProcess(double delta) {
		var newBulletPosition = Sprite2D.Position + _velocity * (float)delta;
		if (newBulletPosition.DistanceSquaredTo(_from) >= _maxDistanceSquared) {
			EndShoot();
			return;
		}
		Sprite2D.Position = newBulletPosition;
		
		var endTrailPos = EndTrailPos;
		if (newBulletPosition.DistanceSquaredTo(endTrailPos) > _trailLongSquared) {
			endTrailPos += _velocity * (float)delta;
		}
		SetTrail(newBulletPosition, endTrailPos);
	}

	public void EndShoot() {
		SetPhysicsProcess(false);
		Sprite2D.Visible = Visible = false;
		_busy = false;
	}

}
