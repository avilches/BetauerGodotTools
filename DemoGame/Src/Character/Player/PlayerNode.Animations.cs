using Betauer.Animation;
using Betauer.Animation.Easing;
using Betauer.Core.Nodes.Property;
using Betauer.Core.Restorer;
using Godot;

namespace Veronenger.Character.Player; 

public partial class PlayerNode {

	public ILoopStatus AnimationIdle { get; private set; }
	public ILoopStatus AnimationRun { get; private set; }
	public IOnceStatus AnimationRunStop { get; private set; }
	public IOnceStatus AnimationJump { get; private set; }
	public ILoopStatus AnimationFall { get; private set; }
	public IOnceStatus AnimationAttack { get; private set; }
	public IOnceStatus AnimationAirAttack { get; private set; }
	public IOnceStatus AnimationHurt { get; private set; }

	public IOnceStatus PulsateTween;
	public ILoopStatus DangerTween;
	public IOnceStatus SqueezeTween;

	private AnimationStack _animationStack;
	private AnimationStack _tweenStack;


	private void ConfigureAnimations() {
		_animationStack = new AnimationStack("Player.AnimationStack").SetAnimationPlayer(_animationPlayer);
		_tweenStack = new AnimationStack("Player.AnimationStack");

		AnimationIdle = _animationStack.AddLoopAnimation("Idle");
		AnimationRun = _animationStack.AddLoopAnimation("Run");
		AnimationRunStop = _animationStack.AddOnceAnimation("RunStop");
		AnimationJump = _animationStack.AddOnceAnimation("Jump");
		AnimationFall = _animationStack.AddLoopAnimation("Fall");
		AnimationAttack = _animationStack.AddOnceAnimation("Attack");
		AnimationAirAttack = _animationStack.AddOnceAnimation("AirAttack");
		AnimationHurt = _animationStack.AddOnceAnimation("Hurt");

		Restorer restorer = new MultiRestorer() 
			.Add(CharacterBody2D.CreateRestorer(Properties.Modulate, Properties.Scale2D))
			.Add(_mainSprite.CreateRestorer(Properties.Modulate, Properties.Scale2D));
		restorer.Save();
		PulsateTween = _tweenStack.AddOnceTween("Pulsate", CreateMoveLeft()).OnEnd(restorer.Restore);
		DangerTween = _tweenStack.AddLoopTween("Danger", CreateDanger()).OnEnd(restorer.Restore);
		SqueezeTween = _tweenStack.AddOnceTween("Squeeze", CreateSqueeze()).OnEnd(restorer.Restore);
	}

	private IAnimation CreateReset() {
		var seq = SequenceAnimation.Create(_mainSprite)
			.AnimateSteps(Properties.Modulate)
			.From(new Color(1, 1, 1, 0))
			.To(new Color(1, 1, 1, 1), 1)
			.EndAnimate();
		// seq.AddProperty(_mainSprite, "modulate", new Color(1, 1, 1, 1), 0.1f);
		// seq.Parallel().AddProperty(this, "scale", new Vector2(1f, 1f), 0.1f);
		return seq;
	}

	private IAnimation CreateMoveLeft() {
		var seq = KeyframeAnimation.Create(_mainSprite)
			.SetDuration(2f)
			.AnimateKeys(Properties.Modulate)
			.KeyframeTo(0.25f, new Color(1, 1, 1, 0))
			.KeyframeTo(0.75f, new Color(1, 1, 1, 0.5f))
			.KeyframeTo(1f, new Color(1, 1, 1, 1))
			.EndAnimate()
			.AnimateKeys<Vector2>(Properties.Scale2D)
			.KeyframeTo(0.5f, new Vector2(1.4f, 1f))
			.KeyframeTo(1f, new Vector2(1f, 1f))
			.EndAnimate();
		// seq.AddProperty(_mainSprite, "modulate", new Color(1, 1, 1, 0), 1f).SetTrans(Tween.TransitionType.Cubic);
		// seq.AddProperty(_mainSprite, "modulate", new Color(1, 1, 1, 1), 1f).SetTrans(Tween.TransitionType.Cubic);
		return seq;
	}

	private IAnimation CreateDanger() {
		var seq = SequenceAnimation.Create(_mainSprite)
			.AnimateSteps<Color>(Properties.Modulate, Easings.CubicInOut)
			.To(new Color(1, 0, 0, 1), 1)
			.To(new Color(1, 1, 1, 1), 1)
			.EndAnimate();
		return seq;
	}

	private IAnimation CreateSqueeze() {
		var seq = SequenceAnimation.Create(this)
			.AnimateSteps(Properties.Scale2D, Easings.SineInOut)
			.To(new Vector2(1.4f, 1f), 0.25f)
			.To(new Vector2(1f, 1f), 0.25f)
			.EndAnimate()
			.SetLoops(2);
		return seq;
	}

    
}