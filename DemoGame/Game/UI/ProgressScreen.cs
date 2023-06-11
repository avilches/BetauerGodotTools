using Betauer.NodePath;
using Godot;

namespace Veronenger.Game.UI;

public partial class ProgressScreen : CanvasLayer {
	[NodePath("%LoadProgressBar")] private TextureProgressBar _loadProgressBar;
	[NodePath("%LoadLabel")] private Label _loadLabel;
	[NodePath("%SavingLabel")] private Label _savingLabel;

	public ProgressScreen() {
		Visible = false;
	}

	public override void _Ready() {
		_loadProgressBar.Value = 0;
	}

	public void ShowSaving(float progress) {
		ShowSaving($"Saving {progress * 100f:0}%...");
	}

	public void ShowSaving(string label = "Saving...") {
		Visible = true;
		_loadProgressBar.Visible = _loadLabel.Visible = false;
		_savingLabel.Visible = true;
		_savingLabel.Text = label;
	}

	public void Loading(float progress) {
		Loading(progress, $"Loading {progress * 100f:0}%...");
	}

	public void Loading(float progress, string label) {
		Visible = true;
		_savingLabel.Visible = false;
		_loadProgressBar.Visible = _loadLabel.Visible = true;
		_loadProgressBar.Value = progress * 100f;
		_loadLabel.Text = label;
	}
}
