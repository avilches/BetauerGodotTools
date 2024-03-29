using System;
using System.Collections.Generic;
using Betauer.Core;
using Betauer.Core.Nodes;
using Betauer.Core.Pool;
using Betauer.DI;
using Betauer.DI.Attributes;
using Godot;
using Godot.Collections;

namespace Veronenger.Game.Platform.Items;

public partial class ProjectileTrail : Node, IInjectable {
	public enum Behaviour { Continue, Stop }

	[Inject] public Random Random { get; set; }
	[Inject] private NodePool<ProjectileTrail> ProjectilePool { get; set; }
	
	private static float RayLength = 40;

	private readonly HashSet<Rid> _exclude = new();
	private Vector2 _from;
	private Vector2 _velocity;
	private Vector2 _direction;
	private float _maxDistanceSquared;
	private float _trailLongSquared;
	private int _raycastLength;
	private Line2D Trail;
	private Func<RaycastCollision, Behaviour> _onCollide;
	public Sprite2D Sprite2D;

	private readonly LazyRaycast2D _lazyRaycast2D = new();
	private Vector2 _collisionPosition = Vector2.Zero;
	private bool _queueEnd = false;

	public void PostInject() {
		Trail = GetNode<Line2D>("Line2D");
		Trail.Visible = false;
		Trail.Width = 1f;
		Trail.ClearPoints();
		Trail.AddPoint(Vector2.Zero);
		Trail.AddPoint(Vector2.Zero);
		Trail.AddPoint(Vector2.Zero);
		Sprite2D = new Sprite2D {
			Visible = false
		};
		AddChild(Sprite2D);
		_lazyRaycast2D.GetDirectSpaceFrom(Trail);
	}

	public void ShootFrom(WeaponRangeGameObject gameObject, Vector2 from, Vector2 direction, Action<PhysicsRayQueryParameters2D> raycastConfig, Func<RaycastCollision, Behaviour> onCollide) {
		SetPhysicsProcess(true);
		direction = direction.Rotated(gameObject.NewRandomDispersion());
		_from = from;
		_velocity = direction * gameObject.Config.Speed;
		_direction = direction;
		_maxDistanceSquared = Mathf.Pow(gameObject.Config.MaxDistance, 2);
		_trailLongSquared = Mathf.Pow(gameObject.Config.TrailLength * Random.Range(0.6f, 1.4f), 2);
		_onCollide = onCollide;
		_queueEnd = false;
		_raycastLength = gameObject.Config.RaycastLength;

		_exclude.Clear();
		_lazyRaycast2D.Query.Exclude = new Array<Rid>();
		
		Sprite2D.Position = from;
		Sprite2D.Texture = gameObject.Config.Projectile;
		Sprite2D.Visible = gameObject.Config.Projectile != null;
		
		Trail.SetPointPosition(0, from);
		Trail.SetPointPosition(1, from);
		Trail.SetPointPosition(2, from);
		Trail.Visible = true;
		// this.OnDraw(canvas => {
			// canvas.DrawCircle(_collisionPosition, 3, Colors.Red);
		// });                   
		_lazyRaycast2D.Config(raycastConfig);
		
	}

	public override void _PhysicsProcess(double delta) {
		if (_queueEnd) {
			ProjectilePool.Release(this);
		} else {
			var currentPosition = Sprite2D.Position;
			if (IsTooFar(currentPosition)) {
				ProjectilePool.Release(this);
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

		var endTrailPos = Trail.GetPointPosition(2);
		if (newPosition.DistanceSquaredTo(_from) > _trailLongSquared) {
			endTrailPos += _velocity * (float)delta;
		}
		var middlePosition = newPosition.Lerp(endTrailPos, 0.5f);

		Sprite2D.Position = newPosition;    
		Trail.SetPointPosition(0, newPosition);
		Trail.SetPointPosition(1, middlePosition);
		Trail.SetPointPosition(2, endTrailPos);

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
}
