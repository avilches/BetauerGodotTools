using Betauer.DI;
using Betauer.OnReady;
using Godot;
using Veronenger.Character.Player;
using Veronenger.Managers;

namespace Veronenger.UI; 

public partial class HUD : CanvasLayer {
	[OnReady("%HealthBar")] private TextureProgressBar _healthBar;
	[Inject] private EventBus EventBus { get; set; }

	
	public override void _Ready() {
		EventBus.Subscribe((PlayerUpdateHealthEvent he) => {
			_healthBar.MinValue = 0;
			_healthBar.MaxValue = he.Max;
			_healthBar.Value = he.ToHealth;
		});
	}

	public void StartGame() {
		Visible = true;
	}

	public void EndGame() {
		Visible = false;
	}
}
