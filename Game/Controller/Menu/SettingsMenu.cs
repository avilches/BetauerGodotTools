using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Betauer;
using Betauer.Animation;
using Betauer.DI;
using Betauer.Input;
using Betauer.Screen;
using Betauer.StateMachine;
using Godot;
using Veronenger.Game.Controller.UI;
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

		private readonly Launcher _launcher = new Launcher();

		private void Disable(Button control, bool isDisabled) {
			control.FocusMode = isDisabled ? Control.FocusModeEnum.None : Control.FocusModeEnum.All;
			control.Disabled = isDisabled;
		}

		public override void Ready() {
			_launcher.WithParent(this);
			
			ConfigureCheckboxes();
			ConfigureResolutionButton();
			ConfigureControls();

			_fullscreenButton.Pressed = _screenManager.Settings.Fullscreen;
			_pixelPerfectButton.Pressed = _screenManager.Settings.PixelPerfect;
			_vsyncButton.Pressed = _screenManager.Settings.VSync;;
			_borderlessButton.Pressed = _screenManager.Settings.Borderless;

			Disable(_resolutionButton, _screenManager.Settings.Fullscreen);
			Disable(_borderlessButton, _screenManager.Settings.Fullscreen);
			UpdateResolutionButton();

			HideSettingsMenu();
		}

		private void ConfigureResolutionButton() {
			_resolutionButton.OnFocusEntered(() => {
				UpdateResolutionButton();
				_gameManager.MainMenuBottomBarScene.ConfigureSettingsResolution();
			});
			_resolutionButton.OnFocusExited(UpdateResolutionButton);
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

		private void ConfigureCheckboxes() {
			_fullscreenButton.OnFocusEntered(() => {
				_scrollContainer.ScrollVertical = 0;
				_gameManager.MainMenuBottomBarScene.ConfigureSettingsChangeBack();
			});
			_fullscreenButton.Action = isChecked => {
				Disable(_resolutionButton, isChecked);
				Disable(_borderlessButton, isChecked);
				if (isChecked) {
					_borderlessButton.Pressed = false;
				}
				_screenManager.SetFullscreen(isChecked);
			};
			_pixelPerfectButton.OnFocusEntered(_gameManager.MainMenuBottomBarScene.ConfigureSettingsChangeBack);
			_pixelPerfectButton.Action = isChecked => {
				_screenManager.SetPixelPerfect(isChecked);
			};
			_borderlessButton.OnFocusEntered(_gameManager.MainMenuBottomBarScene.ConfigureSettingsChangeBack);
			_borderlessButton.Action = isChecked => {
				_screenManager.SetBorderless(isChecked);
			};
			_vsyncButton.OnFocusEntered(_gameManager.MainMenuBottomBarScene.ConfigureSettingsChangeBack);
			_vsyncButton.Action = isChecked => {
				_screenManager.SetVSync(isChecked);
			};
		}

		private void ConfigureControls() {
			if (_controls.GetChildCount() < _inputManager.ConfigurableActionList.Count)
				throw new Exception("Please, clone and add more RedefineActionButton nodes in Controls container");
			
			while (_controls.GetChildCount() > _inputManager.ConfigurableActionList.Count) 
				_controls.RemoveChild(_controls.GetChild(0));

			var x = 0;
			_controls.GetChildrenFilter<RedefineActionButton>().ForEach(button => {
				var action = _inputManager.ConfigurableActionList[x]; 
				button.OnFocusEntered(() => {
					_gameManager.MainMenuBottomBarScene.ConfigureSettingsChangeBack();
				});
				button.ActionHint.Labels(null, action.Name).Button(action);
				x++;
			});
			_controls.GetChild<ActionButton>(_controls.GetChildCount() - 1).OnFocusEntered(() => {
				_gameManager.MainMenuBottomBarScene.ConfigureSettingsChangeBack();
				_scrollContainer.ScrollVertical = int.MaxValue;
			});
		}

		public async Task ShowSettingsMenu() {
			_panel.Show();
			_fullscreenButton.GrabFocus();
		}

		public void HideSettingsMenu() {
			_launcher.RemoveAll();
			_panel.Hide();
		}

		private const float MenuEffectTime = 0.10f;
	}
}
