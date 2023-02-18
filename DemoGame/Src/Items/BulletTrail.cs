using Betauer.Core.Pool;
using Godot;

namespace Veronenger.Items;

public partial class BulletTrail : Line2D, IBusyElement {
	private WeaponRangeItem _weapon;
	private Vector2 _from;
	private Vector2 _velocity;
	private float _maxDistanceSquared;
	private float _trailLongSquared;
	private volatile bool _busy = false;
	public Sprite2D Sprite2D;

	public Vector2 StartTrailPos {
		get => GetPointPosition(0);
		set => SetPointPosition(0, value);
	}

	public Vector2 EndTrailPos {
		get => GetPointPosition(1);
		set => SetPointPosition(1, value);
	}

	public bool IsBusy() => _busy;

	public BulletTrail Configure(Node parent, Texture2D? bulletTexture = null) {
		Width = 1f;
		DefaultColor = Colors.DimGray;
		Visible = false;
		ClearPoints();
		AddPoint(Vector2.Zero);
		AddPoint(Vector2.Zero);
		parent.AddChild(this);
		Sprite2D = new Sprite2D {
			Texture = bulletTexture,
			Visible = false
		};
		AddChild(Sprite2D);
		return this;
	}

	public void ShootFrom(Vector2 from, WeaponRangeItem item, Vector2 direction) {
		SetPhysicsProcess(true);
		_busy = true;
		_weapon = item;
		_velocity = direction * item.Config.Speed;
		_maxDistanceSquared = Mathf.Pow(item.Config.MaxDistance, 2);
		_trailLongSquared = Mathf.Pow(item.Config.TrailLong, 2);
		
		_from = StartTrailPos = EndTrailPos = from;
		Visible = true;
		Sprite2D.Position = from;
		Sprite2D.Visible = Sprite2D.Texture != null;
	}
	
	public override void _PhysicsProcess(double delta) {
		var newBulletPosition = Sprite2D.Position + _velocity * (float)delta;
		if (newBulletPosition.DistanceSquaredTo(_from) > _maxDistanceSquared) {
			GD.Print(newBulletPosition.DistanceTo(_from).ToString());                 
			EndShoot();
			return;
		}
		Sprite2D.Position = newBulletPosition;
		StartTrailPos = newBulletPosition;
		var endTrailPos = EndTrailPos;
		if (newBulletPosition.DistanceSquaredTo(endTrailPos) > _trailLongSquared) {
			EndTrailPos = endTrailPos + _velocity * (float)delta;
		}
	}

	public void EndShoot() {
		SetPhysicsProcess(false);
		Sprite2D.Visible = Visible = false;
		_weapon = null;
		_busy = false;
	}

}