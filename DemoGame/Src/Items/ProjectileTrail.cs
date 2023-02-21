using System;
using System.Collections.Generic;
using Betauer.Core.Nodes;
using Betauer.Core.Pool;
using Betauer.DI;
using Betauer.Nodes;
using Godot;
using Veronenger.Managers;

namespace Veronenger.Items;

public partial class ProjectileTrail : Line2D, IBusyElement {
	private static float RayLength = 40;

	private Vector2 _from;
	private Vector2 _velocity;
	private Vector2 _direction;
	private float _maxDistanceSquared;
	private float _trailLongSquared;
	private volatile bool _busy = false;
	private Func<RaycastCollision, bool> _onCollide;
	public Sprite2D Sprite2D;

	private LazyRaycast2D _lazyRaycast2D;

	public bool IsBusy() => _busy;
	private readonly List<RaycastCollision> _lastCollision = new();

	public ProjectileTrail Configure(Node parent) {
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
		_lazyRaycast2D = new(this);
		return this;
	}


	public void ShootFrom(WeaponRangeItem item, Vector2 from, Vector2 direction, Action<PhysicsRayQueryParameters2D> raycastConfig, Func<RaycastCollision, bool> onCollide) {
		SetPhysicsProcess(true);
		_busy = true;
		_from = from;
		_velocity = direction * item.Config.Speed;
		_direction = direction;
		_maxDistanceSquared = Mathf.Pow(item.Config.MaxDistance, 2);
		_trailLongSquared = Mathf.Pow(item.Config.TrailLong, 2);
		_onCollide = onCollide;
		
		Sprite2D.Position = from;
		Sprite2D.Texture = item.Config.Bullet;
		Sprite2D.Visible = item.Config.Bullet != null;
		
		SetPointPosition(0, from);
		SetPointPosition(1, from);
		SetPointPosition(2, from);
		Visible = true;
		_lastCollision.Clear();
		this.OnDraw(canvas => {
			_lastCollision.ForEach(l => {
				canvas.DrawCircle(l.Position, 3, Colors.Red);
			});
		});
		_lazyRaycast2D.Config(raycastConfig);
	}

	public override void _PhysicsProcess(double delta) {
		var currentBulletPosition = Sprite2D.Position;
		var newBulletPosition = currentBulletPosition + _velocity * (float)delta;
		
		if (newBulletPosition.DistanceSquaredTo(_from) >= _maxDistanceSquared) {
			GD.Print("Too far, last position: "+currentBulletPosition.DistanceTo(_from)+$" Collisions: "+_lastCollision.Count);
			_lastCollision.Clear();
			EndShoot();
			return;
		}
		
		var rayOrigin = newBulletPosition - _direction * RayLength;
		var collision = _lazyRaycast2D.From(rayOrigin).To(newBulletPosition).Cast().Collision;
		if (collision.IsColliding) {
			_lastCollision.Add(collision);
			if (_onCollide(collision)) {
				EndShoot();
				return;
			}
		}
		
		var endTrailPos = GetPointPosition(2);
		if (newBulletPosition.DistanceSquaredTo(endTrailPos) > _trailLongSquared) {
			endTrailPos += _velocity * (float)delta;
		}
		Sprite2D.Position = newBulletPosition;
		SetPointPosition(0, newBulletPosition);
		SetPointPosition(1, newBulletPosition.Lerp(endTrailPos, 0.5f));
		SetPointPosition(2, endTrailPos);

		this.QueueDraw(canvas => {
			canvas.DrawLine(newBulletPosition, newBulletPosition.Lerp(endTrailPos, 0.5f), Colors.Green, 2f);
			canvas.DrawLine(newBulletPosition.Lerp(endTrailPos, 0.5f), endTrailPos, Colors.Blue, 2f);
			canvas.DrawLine(rayOrigin, newBulletPosition, Colors.Red, 1f);
		});

	}

	public void EndShoot() {
		SetPhysicsProcess(false);
		Sprite2D.Visible = Visible = false;
		_busy = false;
	}

}
