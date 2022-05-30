using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Betauer.Animation;
using Betauer.DI;
using Betauer.Input;
using Betauer.Screen;
using Betauer.StateMachine;
using Godot;
using Veronenger.Game.Managers;
using ActionButton = Veronenger.Game.Controller.UI.ActionButton;
using ActionCheckButton = Veronenger.Game.Controller.UI.ActionCheckButton;

namespace Veronenger.Game.Controller.Menu {
	public class SettingsMenu : DiCanvasLayer {
		
		[OnReady("Panel")] private Panel _panel;

		[OnReady("Panel/VBoxContainer/ScrollContainer/MarginContainer/Menu/Fullscreen")] private ActionCheckButton _fullscreenButton;
		[OnReady("Panel/VBoxContainer/ScrollContainer/MarginContainer/Menu/Resolution")] private ActionButton _resolutionButton;
		[OnReady("Panel/VBoxContainer/ScrollContainer/MarginContainer/Menu/PixelPerfect")] private ActionCheckButton _pixelPerfectButton;
		[OnReady("Panel/VBoxContainer/ScrollContainer/MarginContainer/Menu/Borderless")] private ActionCheckButton _borderlessButton;
		[OnReady("Panel/VBoxContainer/ScrollContainer/MarginContainer/Menu/VSync")] private ActionCheckButton _vsyncButton;
		[OnReady("Panel/VBoxContainer/ScrollContainer/MarginContainer/Menu/Controls")] private VBoxContainer _controls;
		[OnReady("Panel/VBoxContainer/ScrollContainer")] private ScrollContainer _scrollContainer;

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
			_fullscreenButton.OnFocusEntered(() => _scrollContainer.ScrollVertical = 0); 
			_controls.GetChild<ActionButton>(_controls.GetChildCount()-1).OnFocusEntered(() => _scrollContainer.ScrollVertical = int.MaxValue); 
			_fullscreenButton.Action = isChecked => {
				Disable(_resolutionButton, isChecked);
				Disable(_borderlessButton, isChecked);
				if (isChecked) {
					_borderlessButton.Pressed = false;
				}
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
			Disable(_borderlessButton, _screenManager.Settings.Fullscreen);
			UpdateResolutionButton();
			_resolutionButton.OnFocusEntered(UpdateResolutionButton);
			_resolutionButton.OnFocusExited(UpdateResolutionButton);

			HideSettingsMenu();
		}

		public async Task ShowSettingsMenu() {
			_panel.Show();
			_fullscreenButton.GrabFocus();
		}

		public void HideSettingsMenu() {
			_launcher.RemoveAll();
			_panel.Hide();
		}

		private void UpdateResolutionButton() {
			List<ScaledResolution> resolutions = _screenManager.GetResolutions();
			Resolution resolution = _screenManager.Settings.WindowedResolution;
			var pos = resolutions.FindIndex(scaledResolution => scaledResolution.Size == resolution.Size);
			pos = pos == -1 ? 0 : pos;

			var prefix = pos > 0 ? "< " : "";
			var suffix = pos < resolutions.Count - 1 ? " >" : "";
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
	}
}
