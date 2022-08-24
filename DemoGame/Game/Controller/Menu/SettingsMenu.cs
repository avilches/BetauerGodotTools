using System;
using System.Collections.Generic;
using System.Linq;
using Betauer;
using Betauer.Animation;
using Betauer.Application;
using Betauer.Application.Screen;
using Betauer.DI;
using Betauer.Input;
using Betauer.Nodes;
using Betauer.OnReady;
using Betauer.Signal;
using Godot;
using Veronenger.Game.Controller.UI;
using Veronenger.Game.Managers;

namespace Veronenger.Game.Controller.Menu {
    public class SettingsMenu : CanvasLayer {
        [OnReady("Panel")] 
        private Panel _panel;

        [OnReady("Panel/VBoxContainer/ScrollContainer/MarginContainer/Menu/Fullscreen")]
        private CheckButton _fullscreenButtonWrapper;

        [OnReady("Panel/VBoxContainer/ScrollContainer/MarginContainer/Menu/Resolution")]
        private Button _resolutionButton;

        [OnReady("Panel/VBoxContainer/ScrollContainer/MarginContainer/Menu/PixelPerfect")]
        private CheckButton _pixelPerfectButtonWrapper;

        [OnReady("Panel/VBoxContainer/ScrollContainer/MarginContainer/Menu/Borderless")]
        private CheckButton _borderlessButtonWrapper;

        [OnReady("Panel/VBoxContainer/ScrollContainer/MarginContainer/Menu/VSync")]
        private CheckButton _vsyncButtonWrapper;

        [OnReady("Panel/VBoxContainer/ScrollContainer/MarginContainer/Menu/Controls")]
        private VBoxContainer _controls;

        [OnReady("Panel/VBoxContainer/ScrollContainer")]
        private ScrollContainer _scrollContainer;

        [OnReady("RedefineActionPanel")] 
        private Panel _redefineActionPanel;

        [Inject] private GameManager _gameManager { get; set; }
        [Inject] private ScreenSettingsManager _screenSettingsManager { get; set; }
        [Inject] private InputActionsContainer _inputActionsContainer { get; set; }

        [Inject] private InputAction UiAccept { get; set; }
        [Inject] private InputAction UiCancel { get; set; }
        [Inject] private InputAction UiStart { get; set; }
        [Inject] private InputAction UiLeft { get; set; }
        [Inject] private InputAction UiRight { get; set; }

        private readonly Launcher _launcher = new Launcher();

        public override void _Ready() {
            _launcher.WithParent(this);

            ConfigureScreenSettingsButtons();
            ConfigureControls();

            _fullscreenButtonWrapper.Pressed = _screenSettingsManager.Fullscreen;
            _pixelPerfectButtonWrapper.Pressed = _screenSettingsManager.PixelPerfect;
            _vsyncButtonWrapper.Pressed = _screenSettingsManager.VSync;
            _borderlessButtonWrapper.Pressed = _screenSettingsManager.Borderless;
            _borderlessButtonWrapper.SetFocusDisabled(_screenSettingsManager.Fullscreen);

            _resolutionButton.SetFocusDisabled(_screenSettingsManager.Fullscreen);
            UpdateResolutionButton();

            HideSettingsMenu();
        }

        private void ConfigureScreenSettingsButtons() {
            _fullscreenButtonWrapper
                .OnFocusEntered(() => {
                    _scrollContainer.ScrollVertical = 0;
                    _gameManager.MainMenuBottomBarScene.ConfigureSettingsChangeBack();
                });
            _fullscreenButtonWrapper.OnToggled(isChecked => {
                _resolutionButton.SetFocusDisabled(isChecked);
                _borderlessButtonWrapper.SetFocusDisabled(isChecked);
                if (isChecked) {
                    _borderlessButtonWrapper.Pressed = false;
                }
                _screenSettingsManager.SetFullscreen(isChecked);
                CheckIfResolutionStillMatches();
            });
            _resolutionButton.OnFocusEntered(() => {
                UpdateResolutionButton();
                _gameManager.MainMenuBottomBarScene.ConfigureSettingsResolution();
            });
            _resolutionButton.OnFocusExited(UpdateResolutionButton);
            _pixelPerfectButtonWrapper.OnFocusEntered(_gameManager.MainMenuBottomBarScene.ConfigureSettingsChangeBack);
            _pixelPerfectButtonWrapper.OnToggled(isChecked => {
                _screenSettingsManager.SetPixelPerfect(isChecked);
                CheckIfResolutionStillMatches();
            });

            _borderlessButtonWrapper.OnFocusEntered(_gameManager.MainMenuBottomBarScene.ConfigureSettingsChangeBack);
            _borderlessButtonWrapper.OnToggled(isChecked => _screenSettingsManager.SetBorderless(isChecked));

            _vsyncButtonWrapper.OnFocusEntered(_gameManager.MainMenuBottomBarScene.ConfigureSettingsChangeBack);
            _vsyncButtonWrapper.OnToggled(isChecked => _screenSettingsManager.SetVSync(isChecked));
        }

        private void ConfigureControls() {
            if (_controls.GetChildCount() < _inputActionsContainer.ConfigurableActionList.Count)
                throw new Exception("Please, clone and add more RedefineActionButton nodes in Controls container");

            while (_controls.GetChildCount() > _inputActionsContainer.ConfigurableActionList.Count)
                _controls.RemoveChild(_controls.GetChild(0));

            var x = 0;
            _controls.GetChildren<RedefineActionButton>().ForEach(button => {
                var action = _inputActionsContainer.ConfigurableActionList[x] as InputAction;
                button.OnPressed(() => {
                    ShowRedefineActionPanel(button);
                });
                button.OnFocusEntered(_gameManager.MainMenuBottomBarScene.ConfigureSettingsChangeBack);
                button.ActionHint.Labels(null, action.Name).InputAction(action);
                button.InputAction = action;
                x++;
            });
            _controls.GetChild<Button>(_controls.GetChildCount() - 1).OnFocusEntered(() => {
                _gameManager.MainMenuBottomBarScene.ConfigureSettingsChangeBack();
                _scrollContainer.ScrollVertical = int.MaxValue;
            });
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

        private void ProcessChangeResolution() {
            if (!UiLeft.JustPressed() && !UiRight.JustPressed() && !UiAccept.JustPressed()) return;
            var (_, resolutions, pos) = FindClosestResolutionToSelected();
            if (UiLeft.JustPressed()) {
                if (pos > 0) {
                    _screenSettingsManager.SetWindowed(resolutions[pos - 1]);
                    UpdateResolutionButton();
                }
            } else if (UiRight.JustPressed()) {
                if (pos < resolutions.Count - 1) {
                    _screenSettingsManager.SetWindowed(resolutions[pos + 1]);
                    UpdateResolutionButton();
                }
            } else if (UiAccept.JustPressed()) {
                _screenSettingsManager.SetWindowed(pos == resolutions.Count - 1
                    ? resolutions[0]
                    : resolutions[pos + 1]);
                UpdateResolutionButton();
            }
        }

        private void UpdateResolutionButton() {
            var (scaledResolution, resolutions, pos) = FindClosestResolutionToSelected();
            var prefix = pos > 0 ? "< " : "";
            var suffix = pos < resolutions.Count - 1 ? " >" : "";
            var res = scaledResolution.ToString();
            if (scaledResolution.Size == _screenSettingsManager.InitialScreenConfiguration.BaseResolution.Size) {
                res += " (Original)";
            } else if (scaledResolution.Base == _screenSettingsManager.InitialScreenConfiguration.BaseResolution.Size &&
                       scaledResolution.IsPixelPerfectScale()) {
                res += " (x" + scaledResolution.GetPixelPerfectScale() + ")";
            }
            _resolutionButton.Text = prefix + res + suffix;
        }

        public override void _Input(InputEvent @event) {
            if (IsRedefineInputActive()) RedefineButtonInput(@event);
        }

        private RedefineActionButton? _redefineButtonSelected;

        public bool IsRedefineInputActive() {
            return _redefineButtonSelected != null;
        }

        public void ShowRedefineActionPanel(RedefineActionButton button) {
            _redefineButtonSelected = button;
            _scrollContainer.Hide();
            _redefineActionPanel.Show();
        }

        private void RedefineButtonInput(InputEvent @event) {
            var e = new EventWrapper(@event);
            if ((e.IsAnyKey() || e.IsAnyButton()) && e.Released) {
                if (!e.IsKey(KeyList.Escape)) {
                    if (e.IsAnyKey()) {
                        _redefineButtonSelected.InputAction.ClearKeys().AddKey((KeyList)e.Key).Save();
                        Console.WriteLine("New key " + e.KeyString);
                    } else if (e.IsAnyButton()) {
                        _redefineButtonSelected.InputAction.ClearButtons().AddButton((JoystickList)e.Button).Save();
                        Console.WriteLine("New button " + e.Button);
                    }
                }
                _redefineButtonSelected.GrabFocus();
                GetTree().SetInputAsHandled();
                _scrollContainer.Show();
                _redefineActionPanel.Hide();
                _redefineButtonSelected = null;
            }
        }

        public void ShowSettingsMenu() {
            _panel.Show();
            _scrollContainer.Show();
            _redefineActionPanel.Hide();
            _fullscreenButtonWrapper.GrabFocus();
        }

        public void HideSettingsMenu() {
            _launcher.RemoveAll();
            _panel.Hide();
            _redefineActionPanel.Hide();
        }

        public void Execute() {
            if (_resolutionButton.HasFocus()) {
                ProcessChangeResolution();
            }
            if (UiCancel.JustPressed() && !IsRedefineInputActive()) {
                _gameManager.TriggerBack();
            }
        }
    }
}