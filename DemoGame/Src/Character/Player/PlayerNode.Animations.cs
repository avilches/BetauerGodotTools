using Betauer.Animation;
using Betauer.Animation.AnimationPlayer;
using Betauer.Animation.Easing;
using Betauer.Core.Nodes.Property;
using Betauer.Core.Restorer;

namespace Veronenger.Character.Player;

public partial class PlayerNode {

	public Anim AnimationIdle { get; private set; }
	public Anim AnimationRun { get; private set; }
	public Anim AnimationRunStop { get; private set; }
	public Anim AnimationJump { get; private set; }
	public Anim AnimationFall { get; private set; }
	public Anim AnimationAttack { get; private set; }
	public Anim AnimationAirAttack { get; private set; }
	public Anim AnimationHurt { get; private set; }

	private void ConfigureAnimations() {
		AnimationIdle = _animationPlayer.CreateAnimationPlayer("Idle");
		AnimationRun = _animationPlayer.CreateAnimationPlayer("Run");
		AnimationRunStop = _animationPlayer.CreateAnimationPlayer("RunStop");
		AnimationJump = _animationPlayer.CreateAnimationPlayer("Jump");
		AnimationFall = _animationPlayer.CreateAnimationPlayer("Fall");
		AnimationAttack = _animationPlayer.CreateAnimationPlayer("Attack");
		AnimationAirAttack = _animationPlayer.CreateAnimationPlayer("AirAttack");
		AnimationHurt = _animationPlayer.CreateAnimationPlayer("Hurt");

		// Restorer restorer = new MultiRestorer() 
		// 	.Add(CharacterBody2D.CreateRestorer(Properties.Modulate, Properties.Scale2D))
		// 	.Add(_mainSprite.CreateRestorer(Properties.Modulate, Properties.Scale2D));
		// restorer.Save();
	}
}