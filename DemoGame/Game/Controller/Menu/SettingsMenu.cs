using System;
using System.Collections.Generic;
using Betauer;
using Betauer.Animation;
using Betauer.Application;
using Betauer.Application.Screen;
using Betauer.DI;
using Betauer.Input;
using Betauer.OnReady;
using Godot;
using Veronenger.Game.Controller.UI;
using Veronenger.Game.Managers;

namespace Veronenger.Game.Controller.Menu {
    public class SettingsMenu : CanvasLayer {
        [OnReady("Panel")] 
        private Panel _panel;

        [OnReady("Panel/VBoxContainer/ScrollContainer/MarginContainer/Menu/Fullscreen")]
        private ButtonWrapper _fullscreenButtonWrapper;

        [OnReady("Panel/VBoxContainer/ScrollContainer/MarginContainer/Menu/Resolution")]
        private ButtonWrapper _resolutionButton;

        [OnReady("Panel/VBoxContainer/ScrollContainer/MarginContainer/Menu/PixelPerfect")]
        private ButtonWrapper _pixelPerfectButtonWrapper;

        [OnReady("Panel/VBoxContainer/ScrollContainer/MarginContainer/Menu/Borderless")]
        private ButtonWrapper _borderlessButtonWrapper;

        [OnReady("Panel/VBoxContainer/ScrollContainer/MarginContainer/Menu/VSync")]
        private ButtonWrapper _vsyncButtonWrapper;

        [OnReady("Panel/VBoxContainer/ScrollContainer/MarginContainer/Menu/Controls")]
        private VBoxContainer _controls;

        [OnReady("Panel/VBoxContainer/ScrollContainer")]
        private ScrollContainer _scrollContainer;

        [OnReady("RedefineActionPanel")] 
        private Panel _redefineActionPanel;

        [Inject] private GameManager _gameManager;
        [Inject] private SettingsManager _settingsManager;
        [Inject] private InputManager _inputManager;

        [Inject] private ActionState UiAccept;
        [Inject] private ActionState UiCancel;
        [Inject] private ActionState UiStart;
        [Inject] private ActionState UiLeft;
        [Inject] private ActionState UiRight;

        private readonly Launcher _launcher = new Launcher();

        public override void _Ready() {
            _launcher.WithParent(this);

            ConfigureCheckboxes();
            ConfigureResolutionButton();
            ConfigureControls();

            _fullscreenButtonWrapper.Pressed = _settingsManager.SettingsFile.Fullscreen;
            _pixelPerfectButtonWrapper.Pressed = _settingsManager.SettingsFile.PixelPerfect;
            _vsyncButtonWrapper.Pressed = _settingsManager.SettingsFile.VSync;
            _borderlessButtonWrapper.Pressed = _settingsManager.SettingsFile.Borderless;
            _borderlessButtonWrapper.SetFocusDisabled(_settingsManager.SettingsFile.Fullscreen);

            _resolutionButton.SetFocusDisabled(_settingsManager.SettingsFile.Fullscreen);
            UpdateResolutionButton();

            HideSettingsMenu();
        }

        private void ConfigureCheckboxes() {
            _fullscreenButtonWrapper
                .OnFocusEntered(() => {
                    _scrollContainer.ScrollVertical = 0;
                    _gameManager.MainMenuBottomBarScene.ConfigureSettingsChangeBack();
                })
                .OnPressed(isChecked => {
                    _resolutionButton.SetFocusDisabled(isChecked);
                    _borderlessButtonWrapper.SetFocusDisabled(isChecked);
                    if (isChecked) {
                        _borderlessButtonWrapper.Pressed = false;
                    }
                    _settingsManager.SetFullscreen(isChecked);
                });
            _pixelPerfectButtonWrapper
                .OnFocusEntered(_gameManager.MainMenuBottomBarScene.ConfigureSettingsChangeBack)
                .OnPressed(isChecked => _settingsManager.SetPixelPerfect(isChecked));
            
            _borderlessButtonWrapper
                .OnFocusEntered(_gameManager.MainMenuBottomBarScene.ConfigureSettingsChangeBack)
                .OnPressed(isChecked => _settingsManager.SetBorderless(isChecked));
            
            _vsyncButtonWrapper
                .OnFocusEntered(_gameManager.MainMenuBottomBarScene.ConfigureSettingsChangeBack)
                .OnPressed(isChecked => _settingsManager.SetVSync(isChecked));
        }

        private void ConfigureResolutionButton() {
            _resolutionButton.OnFocusEntered(() => {
                UpdateResolutionButton();
                _gameManager.MainMenuBottomBarScene.ConfigureSettingsResolution();
            });
            _resolutionButton.OnFocusExited(UpdateResolutionButton);
            _resolutionButton.OnInputEvent(ctx => {
                List<ScaledResolution> resolutions = _settingsManager.GetResolutions();
                Resolution resolution = _settingsManager.SettingsFile.WindowedResolution;
                var pos = resolutions.FindIndex(scaledResolution => scaledResolution.Size == resolution.Size);
                pos = pos == -1 ? 0 : pos;
                
                if (UiLeft.IsActionPressed(ctx.InputEvent)) {
                    if (pos > 0) {
                        _settingsManager.SetWindowed(resolutions[pos - 1]);
                        UpdateResolutionButton();
                        return true;
                    }
                } else if (UiRight.IsActionPressed(ctx.InputEvent)) {
                    if (pos < resolutions.Count - 1) {
                        _settingsManager.SetWindowed(resolutions[pos + 1]);
                        UpdateResolutionButton();
                        return true;
                    }
                } else if (UiAccept.IsActionPressed(ctx.InputEvent)) {
                    _settingsManager.SetWindowed(pos == resolutions.Count - 1
                        ? resolutions[0]
                        : resolutions[pos + 1]);
                    UpdateResolutionButton();
                    return true;
                }
                return false;
            });
        }

        private void UpdateResolutionButton() {
            List<ScaledResolution> resolutions = _settingsManager.GetResolutions();
            Resolution resolution = _settingsManager.SettingsFile.WindowedResolution;
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

        private void ConfigureControls() {
            if (_controls.GetChildCount() < _inputManager.ConfigurableActionList.Count)
                throw new Exception("Please, clone and add more RedefineActionButton nodes in Controls container");

            while (_controls.GetChildCount() > _inputManager.ConfigurableActionList.Count)
                _controls.RemoveChild(_controls.GetChild(0));

            var x = 0;
            _controls.GetChildren<RedefineActionButton>().ForEach(button => {
                var action = _inputManager.ConfigurableActionList[x];
                button
                    .OnPressed(() => {
                        ShowRedefineActionPanel(button);
                    })
                    .OnFocusEntered(_gameManager.MainMenuBottomBarScene.ConfigureSettingsChangeBack);
                button.ActionHint.Labels(null, action.Name).Button(action);
                button.ActionState = action;
                x++;
            });
            _controls.GetChild<ButtonWrapper>(_controls.GetChildCount() - 1).OnFocusEntered(() => {
                _gameManager.MainMenuBottomBarScene.ConfigureSettingsChangeBack();
                _scrollContainer.ScrollVertical = int.MaxValue;
            });
        }

        private RedefineActionButton? _redefineButtonSelected;

        public bool IsRedefineAction() {
            return _redefineButtonSelected != null;
        }

        public void ShowRedefineActionPanel(RedefineActionButton button) {
            _redefineButtonSelected = button;
            _scrollContainer.Hide();
            _redefineActionPanel.Show();
        }

        public override void _Input(InputEvent @event) {
            if (_redefineButtonSelected == null) return;
            var e = new EventWrapper(@event);
            if ((e.IsAnyKey() || e.IsAnyButton()) && e.Released) {
                if (!e.IsKey(KeyList.Escape)) {
                    if (e.IsAnyKey()) {
                        // TODO use redefine key? 
                        _redefineButtonSelected.ActionState.ClearKeys().AddKey((KeyList)e.Key);
                        _settingsManager.SaveControls();
                        Console.WriteLine("New key " + e.KeyString);
                    } else if (e.IsAnyButton()) {
                        _redefineButtonSelected.ActionState.ClearButtons().AddButton((JoystickList)e.Button);
                        _settingsManager.SaveControls();
                        _inputManager.RedefineButton(_redefineButtonSelected.ActionState, e.Button);
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
            if (UiCancel.JustPressed && !IsRedefineAction()) {
                _gameManager.TriggerBack();
            }
        }
    }
}