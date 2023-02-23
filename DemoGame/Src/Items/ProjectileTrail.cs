using System;
using System.Collections.Generic;
using Betauer.Core;
using Betauer.Core.Nodes;
using Betauer.Core.Pool;
using Betauer.Nodes;
using Godot;

namespace Veronenger.Items;

public partial class ProjectileTrail : Line2D, IBusyElement {
	private static readonly Random Random = new Pcg.PcgRandom();
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
	private Vector2 _collisionPosition = Vector2.Zero;
	private bool _queueEnd = false;

	public bool IsBusy() => _busy;

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
		direction = direction.Rotated(item.NewRandomDispersion());
		_busy = true;
		_from = from;
		_velocity = direction * item.Config.Speed;
		_direction = direction;
		_maxDistanceSquared = Mathf.Pow(item.Config.MaxDistance, 2);
		_trailLongSquared = Mathf.Pow(item.Config.TrailLong * Random.Range(0.6f, 1.4f), 2);
		_onCollide = onCollide;
		_queueEnd = false;
		
		Sprite2D.Position = from;
		Sprite2D.Texture = item.Config.Projectile;
		Sprite2D.Visible = item.Config.Projectile != null;
		
		SetPointPosition(0, from);
		SetPointPosition(1, from);
		SetPointPosition(2, from);
		Visible = true;
		this.OnDraw(canvas => {
			canvas.DrawCircle(_collisionPosition, 3, Colors.Red);
		});                   
		_lazyRaycast2D.Config(raycastConfig);
	}

	public override void _PhysicsProcess(double delta) {
		if (_queueEnd) {
			EndShoot();
		} else {
			var currentPosition = Sprite2D.Position;
			if (IsTooFar(currentPosition)) {
				EndShoot();
				return;
			}
			var newPosition = currentPosition + _velocity * (float)delta;
			if (TryGetCollision(newPosition, out var collisionPosition)) {
				MoveProjectile(collisionPosition, delta);
				_collisionPosition = collisionPosition; 
				// We need one frame more to update the bullet in the collision position
				_queueEnd = true;
			} else {
				MoveProjectile(newPosition, delta);
			}
		}
	}

	private void MoveProjectile(Vector2 newPosition, double delta) {

		var endTrailPos = GetPointPosition(2);
		if (newPosition.DistanceSquaredTo(_from) > _trailLongSquared) {
			endTrailPos += _velocity * (float)delta;
		}
		var middlePosition = newPosition.Lerp(endTrailPos, 0.5f);

		Sprite2D.Position = newPosition;    
		SetPointPosition(0, newPosition);
		SetPointPosition(1, middlePosition);
		SetPointPosition(2, endTrailPos);

		// this.QueueDraw(canvas => {
			// canvas.DrawLine(newPosition, middlePosition, Colors.Green, 2f);
			// canvas.DrawLine(middlePosition, endTrailPos, Colors.Blue, 2f);
		// });
	}

	private bool IsTooFar(Vector2 currentPosition) {
		if (currentPosition.DistanceSquaredTo(_from) >= _maxDistanceSquared) {
			GD.Print("Too far, last position: " + currentPosition.DistanceTo(_from));
			return true;
		}
		return false;
	}


	private bool TryGetCollision(Vector2 currentPosition, out Vector2 collisionPosition) {
		// var rayOrigin = currentPosition - _direction * RayLength;
		var collision = _lazyRaycast2D.From(_from).To(currentPosition).Cast().Collision;
		if (collision.IsColliding && _onCollide(collision)) {
			collisionPosition = collision.Position;
			return true;
		}
		collisionPosition = Vector2.Zero;
		return false;
	}

	public void EndShoot() {
		SetPhysicsProcess(false);
		Sprite2D.Visible = Visible = false;
		_busy = false;
	}

}
