using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Betauer.Animation;
using Betauer.Application.Screen;
using Betauer.DI;
using Betauer.Input;
using Betauer.Core.Nodes;
using Betauer.NodePath;
using Betauer.Core.Signal;
using Betauer.DI.Factory;
using Betauer.Nodes;
using Betauer.Tools.Logging;
using Godot;
using Veronenger.Managers;

namespace Veronenger.UI; 

public partial class SettingsMenu : CanvasLayer {
	[NodePath("Panel")] 
	private Panel _panel;

	[NodePath("Panel/SettingsBox")] 
	private VBoxContainer _settingsBox;

	[NodePath("Panel/SettingsBox/ScrollContainer/MarginContainer/Menu/Fullscreen")]
	private CheckButton _fullscreenButtonWrapper;

	[NodePath("Panel/SettingsBox/ScrollContainer/MarginContainer/Menu/Resolution")]
	private Button _resolutionButton;

	[NodePath("Panel/SettingsBox/ScrollContainer/MarginContainer/Menu/PixelPerfect")]
	private CheckButton _pixelPerfectButtonWrapper;

	[NodePath("Panel/SettingsBox/ScrollContainer/MarginContainer/Menu/Borderless")]
	private CheckButton _borderlessButtonWrapper;

	[NodePath("Panel/SettingsBox/ScrollContainer/MarginContainer/Menu/VSync")]
	private CheckButton _vsyncButtonWrapper;

	[NodePath("Panel/SettingsBox/ScrollContainer/MarginContainer/Menu/GamepadControls")]
	private VBoxContainer _gamepadControls;

	[NodePath("Panel/SettingsBox/ScrollContainer/MarginContainer/Menu/KeyboardControls")]
	private VBoxContainer _keyboardControls;

	[NodePath("Panel/SettingsBox/ScrollContainer")]
	private ScrollContainer _scrollContainer;

	[NodePath("Panel/RedefineBox")] 
	private VBoxContainer _redefineBox;

	[NodePath("Panel/RedefineBox/Message")] 
	private Label _redefineActionMessage;

	[NodePath("Panel/RedefineBox/ActionName")] 
	private Label _redefineActionName;

	[Inject] private IFactory<BottomBar> BottomBarSceneFactory { get; set; }
	[Inject] private ScreenSettingsManager _screenSettingsManager { get; set; }

	private BottomBar BottomBarScene => BottomBarSceneFactory.Get();
	
	[Inject] private InputAction UiAccept { get; set; }
	[Inject] private InputAction UiCancel { get; set; }
	[Inject] private InputAction UiLeft { get; set; }
	[Inject] private InputAction UiRight { get; set; }

	[Inject] private InputAction Attack { get; set; }
	[Inject] private InputAction Jump { get; set; }
		
	[Inject] private InputAction Up { get; set; }
	[Inject] private InputAction Down { get; set; }
	[Inject] private InputAction Left { get; set; }
	[Inject] private InputAction Right { get; set; }
		
	[Inject] private EventBus EventBus { get; set; }

	public override void _Ready() {
		ConfigureScreenSettingsButtons();
		ConfigureControls();

		_fullscreenButtonWrapper.ButtonPressed = _screenSettingsManager.Fullscreen;
		_pixelPerfectButtonWrapper.ButtonPressed = _screenSettingsManager.PixelPerfect;
		_vsyncButtonWrapper.ButtonPressed = _screenSettingsManager.VSync;
		_borderlessButtonWrapper.ButtonPressed = _screenSettingsManager.Borderless;
		_borderlessButtonWrapper.SetFocusDisabled(_screenSettingsManager.Fullscreen);

		_resolutionButton.SetFocusDisabled(_screenSettingsManager.Fullscreen);
		UpdateResolutionButton();

		Hide();
	}

	public async Task ShowSettingsMenu() {
		Show();
		_settingsBox.Show();
		_scrollContainer.ScrollVertical = 0;
		_redefineBox.Hide();
		await Templates.BounceIn.Play(_panel, 0f, 0.2f).AwaitFinished();
		_fullscreenButtonWrapper.GrabFocus();
	}

	public async Task HideSettingsMenu() {
		await Templates.BounceOut.Play(_panel, 0f, 0.2f).AwaitFinished();
		Hide();
	}

	private void ConfigureScreenSettingsButtons() {
		_fullscreenButtonWrapper
			.FocusEntered += () => {
				_scrollContainer.ScrollVertical = 0;
				BottomBarScene.ConfigureSettingsChangeBack();
			};
		_fullscreenButtonWrapper.Toggled += isChecked => {
			_resolutionButton.SetFocusDisabled(isChecked);
			_borderlessButtonWrapper.SetFocusDisabled(isChecked);
			if (isChecked) {
				_borderlessButtonWrapper.ButtonPressed = false;
			}
			_screenSettingsManager.SetFullscreen(isChecked);
			CheckIfResolutionStillMatches();
		};
		_resolutionButton.FocusEntered += () => {
			UpdateResolutionButton();
			BottomBarScene.ConfigureSettingsResolution();
		};
		_resolutionButton.FocusExited += UpdateResolutionButton;
		_pixelPerfectButtonWrapper.FocusEntered += BottomBarScene.ConfigureSettingsChangeBack;
		_pixelPerfectButtonWrapper.Pressed += () => {
			_screenSettingsManager.SetPixelPerfect(_pixelPerfectButtonWrapper.ButtonPressed);
			CheckIfResolutionStillMatches();
		};

		_borderlessButtonWrapper.FocusEntered += BottomBarScene.ConfigureSettingsChangeBack;
		_borderlessButtonWrapper.Toggled += isChecked => _screenSettingsManager.SetBorderless(isChecked);

		_vsyncButtonWrapper.FocusEntered += BottomBarScene.ConfigureSettingsChangeBack;
		_vsyncButtonWrapper.Toggled += isChecked => _screenSettingsManager.SetVSync(isChecked);
	}

	private void ConfigureControls() {
		// Remove all
		foreach (Node child in _gamepadControls.GetChildren()) child.QueueFree();
		foreach (Node child in _keyboardControls.GetChildren()) child.QueueFree();
			
		// TODO: i18n
		AddConfigureControl("Jump", Jump, false);
		AddConfigureControl("Attack", Attack, false);
			
		AddConfigureControl("Up", Up, true);
		AddConfigureControl("Down", Down, true);
		AddConfigureControl("Left", Left, true);
		AddConfigureControl("Right", Right, true);
		AddConfigureControl("Jump", Jump, true);
		AddConfigureControl("Attack", Attack, true);
			
		_keyboardControls.GetChild<Button>(_gamepadControls.GetChildCount() - 1).FocusEntered += () => {
			BottomBarScene.ConfigureSettingsChangeBack();
			_scrollContainer.ScrollVertical = int.MaxValue;
		};
	}

	[Inject] private IFactory<RedefineActionButton> RedefineActionButton { get; set; }

	private void AddConfigureControl(string name, InputAction action, bool isKey) {
		var button = RedefineActionButton.Get();
		button.Pressed += () => ShowRedefineActionPanel(button);
		button.FocusEntered += BottomBarScene.ConfigureSettingsChangeBack;
		button.SetInputAction(name, action, isKey);
		if (isKey) _keyboardControls.AddChild(button);
		else _gamepadControls.AddChild(button);
	} 

	private Tuple<ScaledResolution, List<ScaledResolution>, int> FindClosestResolutionToSelected() {
		List<ScaledResolution> resolutions = _screenSettingsManager.GetResolutions();
		Resolution currentResolution = _screenSettingsManager.WindowedResolution;
		var pos = resolutions.FindIndex(scaledResolution => scaledResolution.Size == currentResolution.Size);
		if (pos == -1) {
			// Find the closest resolution with the same or smaller height
			pos = resolutions.Count(scaledResolution => scaledResolution.Size.Y <= currentResolution.Size.Y) - 1;
			if (pos == -1) pos = 0;
		}
		return new Tuple<ScaledResolution, List<ScaledResolution>, int>(resolutions[pos], resolutions, pos);
	}

	private void CheckIfResolutionStillMatches() {
		if (_screenSettingsManager.IsFullscreen()) return; 
		var (closestResolution, resolutions, pos) = FindClosestResolutionToSelected();
		if (_screenSettingsManager.WindowedResolution.Size != closestResolution.Size) {
			_screenSettingsManager.SetWindowed(resolutions[pos]);
			UpdateResolutionButton();
		}
	}

	private static readonly Logger Logger = LoggerFactory.GetLogger(typeof(SettingsMenu));
	private bool ProcessChangeResolution(InputEvent e) {
		if (!UiLeft.IsJustPressed && !UiRight.IsJustPressed && !UiAccept.IsJustPressed) return false;
		var (_, resolutions, pos) = FindClosestResolutionToSelected();
		if (UiLeft.IsJustPressed) {
			if (pos > 0) {
				_screenSettingsManager.SetWindowed(resolutions[pos - 1]);
				UpdateResolutionButton();
			}
		} else if (UiRight.IsJustPressed) {
			if (pos < resolutions.Count - 1) {
				_screenSettingsManager.SetWindowed(resolutions[pos + 1]);
				UpdateResolutionButton();
			}
		} else if (UiAccept.IsJustPressed) {
			_screenSettingsManager.SetWindowed(pos == resolutions.Count - 1
				? resolutions[0]
				: resolutions[pos + 1]);
			UpdateResolutionButton();
		}
		return true;
	}

	private void UpdateResolutionButton() {
		var (scaledResolution, resolutions, pos) = FindClosestResolutionToSelected();
		var prefix = pos > 0 ? "< " : "";
		var suffix = pos < resolutions.Count - 1 ? " >" : "";
		var res = scaledResolution.ToString();
		if (scaledResolution.Size == _screenSettingsManager.ScreenConfiguration.BaseResolution.Size) {
			res += " (Original)";
		} else if (scaledResolution.Base == _screenSettingsManager.ScreenConfiguration.BaseResolution.Size) {
			if (scaledResolution.IsScaleYInteger()) {
				res += " (x" + scaledResolution.Scale.Y + ")";
			}
		}
		_resolutionButton.Text = prefix + res + suffix;
	}

	public void OnInput(InputEvent e) {
		if (_redefineBox.Visible) {
			// Do nothing!
			
		} else if (UiCancel.IsEventPressed(e)) {
			EventBus.Publish(MainEvent.Back);
			GetViewport().SetInputAsHandled();
				
		} else if (_resolutionButton.HasFocus()) {
			if (ProcessChangeResolution(e)) {
				GetViewport().SetInputAsHandled();
			}
		}
	}

	public async void ShowRedefineActionPanel(RedefineActionButton redefineButton) {
		_redefineBox.Show();
		_settingsBox.Hide();
		_redefineActionName.Text = redefineButton.ActionName;
		// TODO: i18n
		_redefineActionMessage.Text = redefineButton.IsKey ? "Press key for..." : "Press button for...";
		BottomBarScene.HideAll();

		await DefaultNodeHandler.Instance.AwaitInput(e => {
			if (e.IsKey(Key.Escape)) {
				// Cancel the redefine button window
				return true;
			} else if (redefineButton.IsKey && e.IsAnyKey()) {
				RedefineKey(redefineButton, e.GetKey());
				return true;
			} else if (redefineButton.IsButton && e.IsAnyButton()) {
				RedefineButton(redefineButton, e.GetButton());
				return true;
			}
			return false;
		});
		_redefineBox.Hide();
		_settingsBox.Show();
		redefineButton.GrabFocus();
	}

	private void RedefineButton(RedefineActionButton redefineButton, JoyButton newButton) {
		if (redefineButton.InputAction.HasButton(newButton)) return;

		var otherRedefine = _gamepadControls.FirstNodeOrNull<RedefineActionButton>(r => r.InputAction.HasButton(newButton));
		if (otherRedefine != null && otherRedefine != redefineButton) {
			// Swap: set to the other the current key
			var currentButton = redefineButton.InputAction.Buttons[0];
			otherRedefine.InputAction.Update(u => u.SetButton(currentButton)).Save();
			otherRedefine.Refresh();
		}
		redefineButton.InputAction.Update(u => u.SetButton(newButton)).Save();
		redefineButton.Refresh();
	}

	private void RedefineKey(RedefineActionButton redefineButton, Key newKey) {
		if (redefineButton.InputAction.HasKey(newKey)) return;
		
		var otherRedefine = _keyboardControls.FirstNodeOrNull<RedefineActionButton>(r => r.InputAction.HasKey(newKey));
		if (otherRedefine != null && otherRedefine != redefineButton) {
			// Swap: set to the other the current key
			var currentKey = redefineButton.InputAction.Keys[0];
			otherRedefine.InputAction.Update(u => u.SetKey(currentKey)).Save();
			otherRedefine.Refresh();
		}
		redefineButton.InputAction.Update(u => u.SetKey(newKey)).Save();
		redefineButton.Refresh();
	}
}
