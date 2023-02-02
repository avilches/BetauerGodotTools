using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Betauer.Animation;
using Betauer.Application.Screen;
using Betauer.DI;
using Betauer.Input;
using Betauer.Core.Nodes;
using Betauer.OnReady;
using Betauer.Core.Restorer;
using Betauer.Core.Signal;
using Godot;
using Veronenger.Managers;

namespace Veronenger.UI; 

public partial class SettingsMenu : CanvasLayer {
	[OnReady("Panel")] 
	private Panel _panel;

	[OnReady("Panel/SettingsBox")] 
	private VBoxContainer _settingsBox;

	[OnReady("Panel/SettingsBox/ScrollContainer/MarginContainer/Menu/Fullscreen")]
	private CheckButton _fullscreenButtonWrapper;

	[OnReady("Panel/SettingsBox/ScrollContainer/MarginContainer/Menu/Resolution")]
	private Button _resolutionButton;

	[OnReady("Panel/SettingsBox/ScrollContainer/MarginContainer/Menu/PixelPerfect")]
	private CheckButton _pixelPerfectButtonWrapper;

	[OnReady("Panel/SettingsBox/ScrollContainer/MarginContainer/Menu/Borderless")]
	private CheckButton _borderlessButtonWrapper;

	[OnReady("Panel/SettingsBox/ScrollContainer/MarginContainer/Menu/VSync")]
	private CheckButton _vsyncButtonWrapper;

	[OnReady("Panel/SettingsBox/ScrollContainer/MarginContainer/Menu/GamepadControls")]
	private VBoxContainer _gamepadControls;

	[OnReady("Panel/SettingsBox/ScrollContainer/MarginContainer/Menu/KeyboardControls")]
	private VBoxContainer _keyboardControls;

	[OnReady("Panel/SettingsBox/ScrollContainer")]
	private ScrollContainer _scrollContainer;

	[OnReady("Panel/RedefineBox")] 
	private VBoxContainer _redefineBox;

	[OnReady("Panel/RedefineBox/Message")] 
	private Label _redefineActionMessage;

	[OnReady("Panel/RedefineBox/ActionName")] 
	private Label _redefineActionName;

	[Inject] private MainStateMachine MainStateMachine { get; set; }
	[Inject] private ScreenSettingsManager _screenSettingsManager { get; set; }

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
			.OnFocusEntered(() => {
				_scrollContainer.ScrollVertical = 0;
				MainStateMachine.BottomBarScene.ConfigureSettingsChangeBack();
			});
		_fullscreenButtonWrapper.OnToggled(isChecked => {
			_resolutionButton.SetFocusDisabled(isChecked);
			_borderlessButtonWrapper.SetFocusDisabled(isChecked);
			if (isChecked) {
				_borderlessButtonWrapper.ButtonPressed = false;
			}
			_screenSettingsManager.SetFullscreen(isChecked);
			CheckIfResolutionStillMatches();
		});
		_resolutionButton.OnFocusEntered(() => {
			UpdateResolutionButton();
			MainStateMachine.BottomBarScene.ConfigureSettingsResolution();
		});
		_resolutionButton.OnFocusExited(UpdateResolutionButton);
		_pixelPerfectButtonWrapper.OnFocusEntered(MainStateMachine.BottomBarScene.ConfigureSettingsChangeBack);
		_pixelPerfectButtonWrapper.OnPressed(() => {
			_screenSettingsManager.SetPixelPerfect(_pixelPerfectButtonWrapper.ButtonPressed);
			CheckIfResolutionStillMatches();
		});

		_borderlessButtonWrapper.OnFocusEntered(MainStateMachine.BottomBarScene.ConfigureSettingsChangeBack);
		_borderlessButtonWrapper.OnToggled(isChecked => _screenSettingsManager.SetBorderless(isChecked));

		_vsyncButtonWrapper.OnFocusEntered(MainStateMachine.BottomBarScene.ConfigureSettingsChangeBack);
		_vsyncButtonWrapper.OnToggled(isChecked => _screenSettingsManager.SetVSync(isChecked));
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
			
		_keyboardControls.GetChild<Button>(_gamepadControls.GetChildCount() - 1).OnFocusEntered(() => {
			MainStateMachine.BottomBarScene.ConfigureSettingsChangeBack();
			_scrollContainer.ScrollVertical = int.MaxValue;
		});
	}

	[Inject] private Factory<RedefineActionButton> RedefineActionButton { get; set; }

	private void AddConfigureControl(string name, InputAction action, bool isKey) {
		var button = RedefineActionButton.Get();
		button.OnPressed(() => ShowRedefineActionPanel(button));
		button.OnFocusEntered(MainStateMachine.BottomBarScene.ConfigureSettingsChangeBack);
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
			pos = resolutions.Count(scaledResolution => scaledResolution.Size.y <= currentResolution.Size.y) - 1;
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

	private bool ProcessChangeResolution(InputEvent e) {
		if (!UiLeft.IsEventJustPressed(e) && !UiRight.IsEventJustPressed(e) &&
			!UiAccept.IsEventJustPressed(e)) return false;
		var (_, resolutions, pos) = FindClosestResolutionToSelected();
		if (UiLeft.IsEventJustPressed(e)) {
			if (pos > 0) {
				_screenSettingsManager.SetWindowed(resolutions[pos - 1]);
				UpdateResolutionButton();
			}
		} else if (UiRight.IsEventJustPressed(e)) {
			if (pos < resolutions.Count - 1) {
				_screenSettingsManager.SetWindowed(resolutions[pos + 1]);
				UpdateResolutionButton();
			}
		} else if (UiAccept.IsEventJustPressed(e)) {
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
				res += " (x" + scaledResolution.Scale.y + ")";
			}
		}
		_resolutionButton.Text = prefix + res + suffix;
	}

	public void OnInput(InputEvent e) {
		// if (e.IsAnyKey()) {
		// Console.WriteLine("GetKeyString:" + e.GetKeyString() + " / Enum:" + e.GetKey()+" / Unicode: "+e.GetKeyUnicode());
		// } else if (e.IsAnyButton()) {
		// Console.WriteLine("ButtonString:" + e.GetButtonString() + " / Enum:" + e.GetButton());
		// }
		if (IsWaitingFromRedefineInputEvent()) {
			RedefineControlFromInputEvent(e);
			GetViewport().SetInputAsHandled();
				
		} else if (UiCancel.IsEventPressed(e)) {
			EventBus.Publish(MainEvent.Back);
			GetViewport().SetInputAsHandled();
				
		} else if (_resolutionButton.HasFocus()) {
			if (ProcessChangeResolution(e)) {
				GetViewport().SetInputAsHandled();
			}
		}
	}

	private RedefineActionButton? _redefineButtonSelected;

	public bool IsWaitingFromRedefineInputEvent() {
		return _redefineButtonSelected != null;
	}

	public void ShowRedefineActionPanel(RedefineActionButton button) {
		_redefineButtonSelected = button;
		_redefineBox.Show();
		_settingsBox.Hide();
		_redefineActionName.Text = button.ActionName;
		// TODO: i18n
		_redefineActionMessage.Text = button.IsKey ? "Press key for..." : "Press button for...";
		MainStateMachine.BottomBarScene.HideAll();
	}

	private void RedefineControlFromInputEvent(InputEvent e) {
		if (!e.IsKey(Key.Escape)) {
			if (_redefineButtonSelected!.IsKey && e.IsAnyKey() && !e.IsKey(Key.Escape)) {
				var otherRedefine = _keyboardControls.FirstNodeOrNull<RedefineActionButton>(r => r.InputAction.HasKey(e.GetKey()));
				if (otherRedefine != null && otherRedefine != _redefineButtonSelected) {
					// Swap: set to the other the current key
					otherRedefine.InputAction.ClearKeys().AddKey(_redefineButtonSelected!.InputAction.Keys[0]).Save().Setup();
					otherRedefine.Refresh();
				}
				_redefineButtonSelected!.InputAction.ClearKeys().AddKey(e.GetKey()).Save().Setup();
				_redefineButtonSelected.Refresh();
			} else if (_redefineButtonSelected!.IsButton && e.IsAnyButton()) {
				var otherRedefine = _gamepadControls.FirstNodeOrNull<RedefineActionButton>(r => r.InputAction.HasButton(e.GetButton()));
				if (otherRedefine != null && otherRedefine != _redefineButtonSelected) {
					// Swap: set to the other the current key
					otherRedefine.InputAction.ClearButtons().AddButton(_redefineButtonSelected!.InputAction.Buttons[0]).Save().Setup();
					otherRedefine.Refresh();
				}
				_redefineButtonSelected!.InputAction.ClearButtons().AddButton(e.GetButton()).Save().Setup();
				_redefineButtonSelected.Refresh();
			} else {
				// Ignore the event
				return;
			}
		}
		_redefineBox.Hide();
		_settingsBox.Show();
		_redefineButtonSelected!.GrabFocus();
		_redefineButtonSelected = null;
	}
}
