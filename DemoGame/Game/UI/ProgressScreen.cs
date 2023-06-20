using Betauer.NodePath;
using Godot;

namespace Veronenger.Game.UI;

public partial class ProgressScreen : CanvasLayer {
	[NodePath("%ProgressBack")] private Line2D _loadProgressBack;
	[NodePath("%ProgressFront")] private Line2D _loadProgressFront;
	[NodePath("%LoadLabel")] private Label _loadLabel;
	[NodePath("%SavingLabel")] private Label _savingLabel;

	private float _progressTotalSize = -1;
	private Vector2 _progressStart;

	public ProgressScreen() {
		Visible = false;
	}

	public override void _Ready() {
		_progressTotalSize = _loadProgressBack.GetPointPosition(1).Y - 1;
		_progressStart = _loadProgressFront.GetPointPosition(0);
	}

	public void ShowSaving(float progress) {
		ShowSaving($"Saving {progress * 100f:0}%...");
	}

	public void ShowSaving(string label = "Saving...") {
		Visible = true;
		_loadProgressBack.Visible = _loadProgressFront.Visible = _loadLabel.Visible = false;
		_savingLabel.Visible = true;
		_savingLabel.Text = label;
	}

	public void Loading(float progress) {
		Loading(progress, $"Loading {progress * 100f:0}%...");
	}

	public void Loading(float progress, string label) {
		Visible = true;
		_savingLabel.Visible = false;
		_loadProgressBack.Visible = _loadProgressFront.Visible = _loadLabel.Visible = true;
		_loadLabel.Text = label;
		SetProgress(progress);
	}

	private void SetProgress(float percent) {
		_loadProgressFront.SetPointPosition(1, new Vector2(_progressStart.X + _progressTotalSize * percent, _progressStart.Y));
	}
}
