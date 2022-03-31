using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Betauer.Animation;
using Betauer.DI;
using Betauer.Input;
using Betauer.Screen;
using Godot;
using Veronenger.Game.Managers;
using ActionButton = Veronenger.Game.Controller.UI.ActionButton;
using ActionCheckButton = Veronenger.Game.Controller.UI.ActionCheckButton;

namespace Veronenger.Game.Controller.Menu {
	public class OptionsMenu : DiNode {
		[OnReady("Panel")] private Panel _panel;

		[OnReady("Panel/VBoxContainer/Menu/Fullscreen")] private ActionCheckButton _fullscreenButton;
		[OnReady("Panel/VBoxContainer/Menu/Resolution")] private ActionButton _resolutionButton;
		[OnReady("Panel/VBoxContainer/Menu/PixelPerfect")] private ActionCheckButton _pixelPerfectButton;
		[OnReady("Panel/VBoxContainer/Menu/Borderless")] private ActionCheckButton _borderlessButton;
		[OnReady("Panel/VBoxContainer/Menu/VSync")] private ActionCheckButton _vsyncButton;

		[Inject] private GameManager _gameManager;
		[Inject] private ScreenManager _screenManager;
		[Inject] private InputManager _inputManager;

		private ActionState UiAccept => _inputManager.UiAccept;
		private ActionState UiCancel => _inputManager.UiCancel;
		private ActionState UiStart => _inputManager.UiStart;

		private Launcher _launcher;

		private void Disable(Button control, bool isDisabled) {
			control.FocusMode = isDisabled ? Control.FocusModeEnum.None : Control.FocusModeEnum.All;
			control.Disabled = isDisabled;
		}

		public override void Ready() {
			_launcher = new Launcher().WithParent(this);
			_fullscreenButton.Action = isChecked => {
				Disable(_resolutionButton, isChecked);
				_screenManager.SetFullscreen(isChecked);
			};
			_resolutionButton.ActionWithInputEventContext = ctx => {
				List<ScaledResolution> resolutions = _screenManager.GetResolutions();
				Resolution resolution = _screenManager.Settings.WindowedResolution;
				var pos = resolutions.FindIndex(scaledResolution => scaledResolution.Size == resolution.Size);
				pos = pos == -1 ? 0 : pos;

				if (ctx.InputEvent.IsActionPressed("ui_left")) {
					if (pos > 0) {
						_screenManager.SetWindowed(resolutions[pos - 1]);
						UpdateResolutionButton();
						return true;
					}
				} else if (ctx.InputEvent.IsActionPressed("ui_right")) {
					if (pos < resolutions.Count - 1) {
						_screenManager.SetWindowed(resolutions[pos + 1]);
						UpdateResolutionButton();
						return true;
					}
				} else if (ctx.InputEvent.IsActionPressed("ui_accept")) {
					_screenManager.SetWindowed(pos == resolutions.Count - 1
						? resolutions[0]
						: resolutions[pos + 1]);
					UpdateResolutionButton();
					return true;
				}
				return false;
			};
			_pixelPerfectButton.Action = isChecked => {
				_screenManager.SetPixelPerfect(isChecked);
			};
			_borderlessButton.Action = isChecked => {
				_screenManager.SetBorderless(isChecked);
			};
			_vsyncButton.Action = isChecked => {
				_screenManager.SetVSync(isChecked);
			};
			_fullscreenButton.Pressed = _screenManager.Settings.Fullscreen;
			_pixelPerfectButton.Pressed = _screenManager.Settings.PixelPerfect;
			_vsyncButton.Pressed = _screenManager.Settings.VSync;;
			_borderlessButton.Pressed = _screenManager.Settings.Borderless;

			Disable(_resolutionButton, _screenManager.Settings.Fullscreen);
			UpdateResolutionButton();
			_resolutionButton.OnFocusEntered(UpdateResolutionButton);
			_resolutionButton.OnFocusExited(UpdateResolutionButton);

			Hide();
		}

		public async Task Show() {
			_panel.Visible = true;

			_fullscreenButton.GrabFocus();
		}

		public void Hide() {
			_launcher.RemoveAll();
			_panel.Visible = false;
		}

		private void UpdateResolutionButton() {
			List<ScaledResolution> resolutions = _screenManager.GetResolutions();
			Resolution resolution = _screenManager.Settings.WindowedResolution;
			var pos = resolutions.FindIndex(scaledResolution => scaledResolution.Size == resolution.Size);
			pos = pos == -1 ? 0 : pos;

			var prefix = "";
			var suffix = "";
			if (_resolutionButton.HasFocus()) {
				prefix = pos > 0 ? "< " : "";
				suffix = pos < resolutions.Count - 1 ? " >" : "";
			}
			ScaledResolution scaledResolution = resolutions[pos];
			var res = scaledResolution.ToString();
			if (scaledResolution.IsPixelPerfectScale()) {
				if (scaledResolution.GetPixelPerfectScale() == 1) {
					res += " (Original)";
				} else {
					res += " (x" + scaledResolution.GetPixelPerfectScale() + ")";
				}
			}
			_resolutionButton.Text = prefix + res + suffix;
		}

		private const float MenuEffectTime = 0.10f;

		public override void _Input(InputEvent @event) {
			if (!_gameManager.IsOptions()) {
				return;
			}
			if (UiCancel.IsEventPressed(@event)) {
				_gameManager.CloseOptionsMenu();
				GetTree().SetInputAsHandled();
			} else if (UiStart.IsEventPressed(@event)) {
				// _gameManager.CloseOptionsMenu();
				// GetTree().SetInputAsHandled();
			}
		}
	}
}
