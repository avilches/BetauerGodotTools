using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Betauer.Animation;
using Betauer.Application.Screen;
using Betauer.Application.Screen.Resolution;
using Betauer.Core;
using Betauer.Core.Nodes;
using Betauer.Core.Signal;
using Betauer.Core.Time;
using Betauer.DI.Attributes;
using Betauer.DI.Factory;
using Betauer.Input;
using Betauer.NodePath;
using Betauer.Nodes;
using Godot;
using Veronenger.Game.Platform;

namespace Veronenger.Game.UI.Settings; 

public partial class SettingsMenu : CanvasLayer {
	[NodePath("Panel")] private Panel _panel;
	[NodePath("Panel/SettingsBox")] private VBoxContainer _settingsBox;
	[NodePath("%Fullscreen")] private CheckButton _fullscreenButtonWrapper;
	[NodePath("%Resolution")] private Button _resolutionButton;
	[NodePath("%Resolutions")] private ItemList _resolutions;
	[NodePath("%PixelPerfect")] private CheckButton _pixelPerfectButtonWrapper;
	[NodePath("%Borderless")] private CheckButton _borderlessButtonWrapper;
	[NodePath("%VSync")] private CheckButton _vsyncButtonWrapper;
	[NodePath("%GamepadControls")] private VBoxContainer _gamepadControls;
	[NodePath("%KeyboardControls")] private VBoxContainer _keyboardControls;
	[NodePath("Panel/SettingsBox/ScrollContainer")] private ScrollContainer _scrollContainer;
	[NodePath("Panel/RedefineBox")] private VBoxContainer _redefineBox;
	[NodePath("Panel/RedefineBox/Message")] private Label _redefineActionMessage;
	[NodePath("%RedefineActionName")] private Label _redefineActionName;
	[NodePath("%RedefineCounter")] private Label _redefineCounterLabel;

	[Inject] private ILazy<BottomBar> BottomBarLazy { get; set; }
	[Inject] private Betauer.DI.Container Container { get; set; }
	[Inject] private ScreenSettingsManager ScreenSettingsManager { get; set; }
	[Inject] private PlatformMultiPlayerContainer PlatformMultiPlayerContainer { get; set; }
	[Inject] private InputActionsContainer PlatformPlayerActions => PlatformMultiPlayerContainer.SharedInputActionsContainer;
	private ScreenController ScreenController => ScreenSettingsManager.ScreenController;
	private ScreenConfig ScreenConfig => ScreenController.ScreenConfig;

	private BottomBar BottomBarScene => BottomBarLazy.Get();
	
	[Inject] private UiActions UiActions { get; set; }

	[Inject] private MainBus MainBus { get; set; }
	[Inject] private ITransient<RedefineActionButton> RedefineActionButton { get; set; }
	
	public override void _Ready() {
		ConfigureSignalEvents();
		AddInputControls();
		UpdateResolutionButtonText();
		
		_fullscreenButtonWrapper.ButtonPressed = ScreenSettingsManager.Fullscreen.Value;
		// _pixelPerfectButtonWrapper.ButtonPressed = ScreenSettingsManager.PixelPerfect;
		_vsyncButtonWrapper.ButtonPressed = ScreenSettingsManager.VSync.Value;
		_borderlessButtonWrapper.ButtonPressed = ScreenSettingsManager.Borderless.Value;
		_borderlessButtonWrapper.SetFocusDisabled(ScreenSettingsManager.Fullscreen.Value);
		_resolutionButton.SetFocusDisabled(ScreenSettingsManager.Fullscreen.Value);
		
		_resolutions.Hide();
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

	private void ConfigureSignalEvents() {
		_fullscreenButtonWrapper
			.FocusEntered += () => {
				_scrollContainer.ScrollVertical = 0;
				BottomBarScene.ChangeBack();
			};
		_fullscreenButtonWrapper.Toggled += isChecked => {
			_resolutionButton.SetFocusDisabled(isChecked);
			_borderlessButtonWrapper.SetFocusDisabled(isChecked);
			if (isChecked) {
				_borderlessButtonWrapper.ButtonPressed = false;
			}
			ScreenController.SetFullscreen(isChecked);
			CheckIfResolutionStillMatches();
		};
		_resolutionButton.Pressed += OpenResolutionList;
		
		_resolutions.FocusExited += () => _resolutions.Hide();
		_resolutions.ItemActivated += index => {
			var resolution = _resolutions.GetItemMetadata((int)index).AsVector2I();
			ScreenSettingsManager.SetWindowed(new Resolution(resolution));
			UpdateResolutionButtonText();
			CloseResolutionList();
		};

		_pixelPerfectButtonWrapper.FocusEntered += BottomBarScene.ChangeBack;
		_pixelPerfectButtonWrapper.Pressed += () => {
			// ScreenSettingsManager.SetPixelPerfect(_pixelPerfectButtonWrapper.ButtonPressed);
			CheckIfResolutionStillMatches();
		};

		_borderlessButtonWrapper.FocusEntered += BottomBarScene.ChangeBack;
		_borderlessButtonWrapper.Toggled += isChecked => ScreenController.SetBorderless(isChecked);

		_vsyncButtonWrapper.FocusEntered += BottomBarScene.ChangeBack;
		_vsyncButtonWrapper.Toggled += isChecked => ScreenController.SetVSync(isChecked);
	}

	private void AddInputControls() {
		// Remove all
		foreach (Node child in _gamepadControls.GetChildren()) child.QueueFree();
		foreach (Node child in _keyboardControls.GetChildren()) child.QueueFree();
			
		// TODO: i18n
		PlatformPlayerActions.Load();
		PlatformPlayerActions.InputActions.ForEach(inputAction => {
			var saveSetting = PlatformPlayerActions.FindSaveSetting(inputAction);
			if (saveSetting != null) {
				if (inputAction.Keys.Count > 0) {
					AddConfigureControl(inputAction.Name, inputAction, true);
				}
				if (inputAction.Buttons.Count > 0) {
					AddConfigureControl(inputAction.Name, inputAction, false);
				}
			}
		});
			
		_keyboardControls.GetChild<Button>(_gamepadControls.GetChildCount() - 1).FocusEntered += () => {
			BottomBarScene.ChangeBack();
			_scrollContainer.ScrollVertical = int.MaxValue;
		};
	}

	private void AddConfigureControl(string name, InputAction action, bool isKey) {
		var button = RedefineActionButton.Create();
		button.Pressed += () => ShowRedefineActionPanel(button);
		button.FocusEntered += BottomBarScene.ChangeBack;
		button.SetInputAction(name, action, isKey);
		if (isKey) _keyboardControls.AddChild(button);
		else _gamepadControls.AddChild(button);
	} 

	private Tuple<List<ScaledResolution>, ScaledResolution, int> FindClosestResolutionToSelected() {
		List<ScaledResolution> resolutions = ScreenController.GetResolutions();
		Resolution currentResolution = new Resolution(ScreenSettingsManager.WindowedResolution.Value);
		var pos = resolutions.FindIndex(scaledResolution => scaledResolution.Size == currentResolution.Size);
		if (pos == -1) {
			// Find the closest resolution with the same or smaller height
			pos = resolutions.Count(scaledResolution => scaledResolution.Size.Y <= currentResolution.Size.Y) - 1;
			if (pos == -1) pos = 0;
		}
		return new Tuple<List<ScaledResolution>, ScaledResolution, int>(resolutions, resolutions[pos], pos);
	}

	private void CheckIfResolutionStillMatches() {
		if (ScreenController.IsFullscreen()) return; 
		var (resolutions, closestResolution, pos) = FindClosestResolutionToSelected();
		if (ScreenSettingsManager.WindowedResolution.Value != closestResolution.Size) {
			ScreenSettingsManager.SetWindowed(resolutions[pos]);
			UpdateResolutionButtonText();
		}
	}

	private void UpdateResolutionButtonText() {
		var (resolutions, selected, selectedPosition) = FindClosestResolutionToSelected();
		_resolutionButton.Text = " "+GetResolutionFullName(selected);
	}

	private void CloseResolutionList() {
		_resolutions.Hide();
		_resolutionButton.GrabFocus();
	}

	private void OpenResolutionList() {
		_resolutions.Clear();
		var (resolutions, selected, selectedPosition) = FindClosestResolutionToSelected();

		resolutions.ForEach((scaledResolution, index) => {
			var res = GetResolutionFullName(scaledResolution);
			_resolutions.AddItem(res);
			_resolutions.SetItemMetadata(index, scaledResolution.Size);
			if (selectedPosition == index) _resolutionButton.Text = " "+res;
		});
		_resolutions.Select(selectedPosition);
		_resolutions.Show();
		_resolutions.GrabFocus();
		_resolutions.EnsureCurrentIsVisible();
	}

	private string GetResolutionFullName(ScaledResolution scaledResolution) {
		var res = new StringBuilder(scaledResolution.ToString());
		if (scaledResolution.Size == ScreenConfig.BaseResolution.Size) {
			res.Append(" (Original)");
		} else if (scaledResolution.Base == ScreenConfig.BaseResolution.Size) {
			if (scaledResolution.IsScaleYInteger()) {
				res.Append(" (x");
				res.Append(scaledResolution.Scale.Y);
				res.Append(')');
			}
		}
		return res.ToString();
	}

	public void OnInput(InputEvent e) {
		if (_redefineBox.Visible) {
			// Do nothing!
			
		} else if (UiActions.UiCancel.IsEventPressed(e)) {
			if (_resolutions.HasFocus()) {
				CloseResolutionList();
			} else {
				MainBus.Publish(MainEvent.TriggerCloseSettingsMenu);
				GetViewport().SetInputAsHandled();
			}
		}
	}

	private const int RedefineSecondsTimeout = 5;

	public async void ShowRedefineActionPanel(RedefineActionButton redefineButton) {
		_redefineBox.Show();
		_settingsBox.Hide();
		_redefineActionName.Text = redefineButton.ActionName;
		// TODO: i18n
		_redefineActionMessage.Text = redefineButton.IsKey ? "Press key for..." : "Press button for...";

		BottomBarScene.HideAll();

		var redefineOk = false;
		var redefineSeconds = RedefineSecondsTimeout;
		void UpdateCounter() =>_redefineCounterLabel.Text = $"Esc to cancel {redefineSeconds--}...";
		var scheduler = new GodotScheduler(GetTree(), 0, 1, UpdateCounter, true).Start();
		
		await NodeManager.MainInstance.AwaitInput(e => {
			if (!e.IsPressed()) return false;
			if (e.IsKey(Key.Escape)) return true; // Close the redefine button window
			if (redefineButton.IsKey && e.IsAnyKey()) {
				_redefineActionName.Text = redefineButton.ActionName + ": " + e.GetKeyString();
				RedefineKey(redefineButton, e.GetKey());
				redefineOk = true;
				return true;
			} else if (redefineButton.IsButton && e.IsAnyButton()) {
				RedefineButton(redefineButton, e.GetButton());
				redefineOk = true;
				return true;
			}
			return false;
		}, RedefineSecondsTimeout + 0.3f /* so enough time to show the counter showing "0" */);
		_redefineActionMessage.Text = "";
		_redefineCounterLabel.Text = "";
		scheduler.Stop();

		if (redefineOk) {
			PlatformPlayerActions.Save();
			PlatformMultiPlayerContainer.SyncActions(); // If there is game in progress, sync the actions
			await NodeManager.MainInstance.AwaitInput(e => e.IsPressed() && (e.IsAnyKey() || e.IsAnyButton() || e.IsAnyClick()), 1.2f);
		}
		
		_redefineBox.Hide();
		_settingsBox.Show();
		redefineButton.GrabFocus();
	}

	private void RedefineButton(RedefineActionButton redefineButton, JoyButton newButton) {
		if (redefineButton.InputAction.HasButton(newButton)) return;

		var otherRedefine = _gamepadControls.FirstNode<RedefineActionButton>(r => r.InputAction.HasButton(newButton));
		if (otherRedefine != null && otherRedefine != redefineButton) {
			// Swap: set to the other the current key
			var currentButton = redefineButton.InputAction.Buttons[0];
			otherRedefine.InputAction.Update(u => u.SetButton(currentButton));
			otherRedefine.Refresh();
		}
		redefineButton.InputAction.Update(u => u.SetButton(newButton));
		redefineButton.Refresh();
	}

	private void RedefineKey(RedefineActionButton redefineButton, Key newKey) {
		if (redefineButton.InputAction.HasKey(newKey)) return;
		
		var otherRedefine = _keyboardControls.FirstNode<RedefineActionButton>(r => r.InputAction.HasKey(newKey));
		if (otherRedefine != null && otherRedefine != redefineButton) {
			// Swap: set to the other the current key
			var currentKey = redefineButton.InputAction.Keys[0];
			otherRedefine.InputAction.Update(u => u.SetKey(currentKey));
			otherRedefine.Refresh();
		}
		redefineButton.InputAction.Update(u => u.SetKey(newKey));
		redefineButton.Refresh();
	}
}
