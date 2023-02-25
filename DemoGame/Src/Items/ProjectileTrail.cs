using System;
using System.Collections.Generic;
using Betauer.Core;
using Betauer.Core.Nodes;
using Betauer.Core.Pool;
using Betauer.Nodes;
using Godot;
using Godot.Collections;

namespace Veronenger.Items;

public partial class ProjectileTrail : Line2D, IBusyElement {
	public enum Behaviour { Continue, Stop }
	private static readonly Random Random = new Pcg.PcgRandom();
	private static float RayLength = 40;

	private readonly HashSet<Rid> _exclude = new();
	private Vector2 _from;
	private Vector2 _velocity;
	private Vector2 _direction;
	private float _maxDistanceSquared;
	private float _trailLongSquared;
	private int _raycastLength;
	private volatile bool _busy = false;
	private Func<RaycastCollision, Behaviour> _onCollide;
	public Sprite2D Sprite2D;

	public LazyRaycast2D _lazyRaycast2D;
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


	public void ShootFrom(WeaponRangeItem item, Vector2 from, Vector2 direction, Action<PhysicsRayQueryParameters2D> raycastConfig, Func<RaycastCollision, Behaviour> onCollide) {
		SetPhysicsProcess(true);
		direction = direction.Rotated(item.NewRandomDispersion());
		_busy = true;
		_from = from;
		_velocity = direction * item.Config.Speed;
		_direction = direction;
		_maxDistanceSquared = Mathf.Pow(item.Config.MaxDistance, 2);
		_trailLongSquared = Mathf.Pow(item.Config.TrailLength * Random.Range(0.6f, 1.4f), 2);
		_onCollide = onCollide;
		_queueEnd = false;
		_raycastLength = item.Config.RaycastLength;

		_exclude.Clear();
		_lazyRaycast2D.Query.Exclude = new Array<Rid>();
		
		Sprite2D.Position = from;
		Sprite2D.Texture = item.Config.Projectile;
		Sprite2D.Visible = item.Config.Projectile != null;
		
		SetPointPosition(0, from);
		SetPointPosition(1, from);
		SetPointPosition(2, from);
		Visible = true;
		// this.OnDraw(canvas => {
			// canvas.DrawCircle(_collisionPosition, 3, Colors.Red);
		// });                   
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
		var rayStartPosition = _raycastLength > 0 ? currentPosition - _direction * _raycastLength : _from;
		var collision = _lazyRaycast2D.From(rayStartPosition).To(currentPosition).Cast().Collision;

		// this.QueueDraw(canvas => {
			// canvas.DrawLine(rayStartPosition, currentPosition, Colors.Red, 2f);
		// });

		if (collision.IsColliding) {
			if (_onCollide(collision) == Behaviour.Stop) {
				collisionPosition = collision.Position;
				return true;
			}
			if (collision.Collider is CollisionObject2D co) {
				// Ignore the collision
				_exclude.Add(co.GetRid());
				_lazyRaycast2D.Query.Exclude = new Array<Rid>(_exclude);
			}
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
