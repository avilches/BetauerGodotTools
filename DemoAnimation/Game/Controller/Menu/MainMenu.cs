using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Betauer;
using Betauer.Animation;
using Betauer.Animation.Tween;
using Betauer.DI;
using Betauer.Input;
using Betauer.OnReady;
using Betauer.Signal;
using Betauer.UI;
using DemoAnimation.Game.Managers;
using Godot;

namespace DemoAnimation.Game.Controller.Menu {
    public class MainMenu : Node2D {
        private const float MenuEffectTime = 1f;
        private const float FadeMainMenuEffectTime = 0.75f;

        [OnReady("MainMenu/VBoxContainer/ScrollContainer/Menu")]
        private Godot.Container _menuBase;

        [OnReady("HBoxContainer/DemoMenu")] private Control _demoMenuContainer;

        [OnReady("HBoxContainer/DemoMenu/HBox/Menu")]
        private Godot.Container _menuDemoBase;

        [OnReady("HBoxContainer/Animator")] private Control _animatorContainer;

        [OnReady("HBoxContainer/Animator/LabelToAnimate")]
        private Label _labelToAnimate;

        [OnReady("HBoxContainer/Animator/SpriteToAnimate")]
        private Sprite _logo;

        [OnReady("HBoxContainer/Animator/TextureRectToAnimate")]
        private TextureRect _texture;

        [OnReady("HBoxContainer/Animator/HBoxContainer/LineEdit")]
        private LineEdit _labelToAnimateTextField;

        [OnReady("HBoxContainer/DemoMenu/HBoxOptions/OptionButtonExit")]
        private OptionButton _optionButtonExit;

        [OnReady("HBoxContainer/DemoMenu/HBoxOptions/OptionButtonEnter")]
        private OptionButton _optionButtonEnter;

        [OnReady("HBoxContainer/DemoMenu/HBoxOptions2/Button")]
        private Button _resetMenu;

        [OnReady("HBoxContainer/DemoMenu/MenuLabel")]
        private Label _menuLabel;

        private MenuContainer _menuContainer;
        private MenuContainer _demoMenu;

        [Inject] private GameManager _gameManager { get; set; }

        [Inject] private InputAction UiAccept { get; set; }
        [Inject] private InputAction UiCancel { get; set; }
        [Inject] private InputAction UiStart { get; set; }

        private Restorer _animationsRestorer;
        private Restorer _menuRestorer;

        private readonly List<TemplateFactory> _templateFactories = Templates.GetAllTemplates()
            .OrderBy(factory => (factory.Category ?? "0") + "-" + factory.Category + "-" + factory.Name)
            .ToList();

        public override void _Ready() {
            _animationsRestorer = new MultiRestorer(_labelToAnimate, _logo, _texture).Save();
            _menuContainer = BuildMenu();
            _menuLabel.Text = "Click the options to see animations";
            _resetMenu.OnPressed(() => {
                _demoMenu.QueueFree();
                _demoMenu = BuildDemoMenu();
                _demoMenu.Start();
            });
            string oldCategory = null;
            const string animationForExit = nameof(Templates.BackOutDown);
            const string animationForEnter = nameof(Templates.BounceInRight);
            var idx = 0;
            foreach (var templateFactory in _templateFactories) {
                var name = templateFactory.Name;
                if (templateFactory.Category != oldCategory && templateFactory.Category != null &&
                    oldCategory != null) {
                    _optionButtonEnter.AddSeparator();
                    _optionButtonExit.AddSeparator();
                    oldCategory = templateFactory.Category;
                }
                _optionButtonEnter.AddItem(name, idx);
                _optionButtonExit.AddItem(name, idx);
                if (name == animationForExit) {
                    _optionButtonExit.Select(idx);
                } else if (name == animationForEnter) {
                    _optionButtonEnter.Select(idx);
                }
                idx++;
            }
            _demoMenuContainer.Visible = false;
            _animatorContainer.Visible = false;
        }

        public override void _Input(InputEvent @event) {
            if (@event is InputEventKey) {
                _labelToAnimate.Text = _labelToAnimateTextField.Text;
            }
        }

        public async Task ShowMenu() {
            GetTree().Root.GuiDisableInput = true;
            Visible = true;
            var modulate = Colors.White;
            modulate.a = 0;
            Modulate = modulate;
            await _menuContainer.Start();
            await Templates.FadeIn.Play(this, 0f, FadeMainMenuEffectTime).AwaitFinished();
            GetTree().Root.GuiDisableInput = false;
        }

        public async Task HideMainMenu() {
            GetTree().Root.GuiDisableInput = true;
            await Templates.FadeOut.Play(this, 0f, FadeMainMenuEffectTime).AwaitFinished();
            Visible = false;
            GetTree().Root.GuiDisableInput = false;
        }

        public void DisableMenus() {
            _menuRestorer = _menuContainer.ActiveMenu!.DisableButtons();
        }

        public void EnableMenus() {
            _menuRestorer?.Restore();
        }

        public MenuContainer BuildMenu() {
            foreach (var child in _menuBase.GetChildren()) (child as Node)?.Free();

            var mainMenu = new MenuContainer(_menuBase);
            var root = mainMenu.GetStartMenu().OnShow(() => {
                _demoMenuContainer.Visible = false;
                _animatorContainer.Visible = false;
            });

            root.AddButton("Start", "Anima comparision").OnPressed(() => { _gameManager.LoadAnimaDemo(); });
            root.AddButton("Effects", "Show all animations").OnPressed(async () => {
                _demoMenuContainer.Visible = false;
                _animatorContainer.Visible = true;
                await mainMenu.Go("Effects");
            });
            root.AddButton("Menus", "Demo with menus").OnPressed(async () => {
                _demoMenuContainer.Visible = true;
                _animatorContainer.Visible = false;
                _demoMenu?.QueueFree();
                _demoMenu = BuildDemoMenu();
                await mainMenu.Go("Menu");
                await _demoMenu.Start();
            });
            root.AddButton("Exit", "Exit").OnPressed(() => { _gameManager.TriggerModalBoxConfirmExitDesktop(); });

            var effectsMenu = mainMenu.AddMenu("Effects");
            CreateEffectsMenu(effectsMenu);

            var demoMenuMenu = mainMenu.AddMenu("Menu");
            demoMenuMenu.AddButton("Back", "Back").OnPressed(() => _menuContainer.Back());
            
            return mainMenu;
        }

        public MenuContainer BuildDemoMenu() {
            foreach (var child in _menuDemoBase.GetChildren()) (child as Node)?.Free();

            var mainMenu = new MenuContainer(_menuDemoBase);
            mainMenu.ConfigureGoTransition(GoGoodbyeAnimation, GoNewMenuAnimation);
            mainMenu.ConfigureBackTransition(BackGoodbyeAnimation, BackNewMenuAnimation);

            var root = mainMenu.GetStartMenu();
            root.AddButton("S", "Start new game").OnPressed(() => {
                mainMenu.Go("Other", "1");
            }, true);
            root.AddButton("C", "Continue").OnPressed(() => mainMenu.Go("Other", "2"));
            
            var second = mainMenu.AddMenu("Other");
            second.AddButton("1", "Come back").OnPressed(() => mainMenu.Back());
            second.AddButton("2", "Exit to main menu").OnPressed(() => mainMenu.Back());
            return mainMenu;
        }

        private SceneTreeTween? _effectsMenuSceneTreeTween;
        private Action? _effectsMenuOnFinish;
        private void CreateEffectsMenu(Betauer.UI.Menu effectsMenu) {
            string oldCategory = null;

            effectsMenu.AddButton("Back", "Back").OnPressed(() => _menuContainer.Back());
            foreach (var templateFactory in _templateFactories) {
                var name = templateFactory.Name;
                if (templateFactory.Category != oldCategory && templateFactory.Category != null) {
                    effectsMenu.AddHSeparator();
                    effectsMenu.AddNode(new Label {
                        Text = templateFactory.Category
                    });
                    oldCategory = templateFactory.Category;
                }

                Button button = effectsMenu.AddButton(name, name);
                button.OnPressed(async () => {
                    if (_effectsMenuSceneTreeTween != null) {
                        _effectsMenuSceneTreeTween.Kill();
                        _effectsMenuOnFinish();
                    }
                    var buttonRestorer = button.CreateRestorer().Save();
                    button.Disabled = true;
                    var animation = Templates.Get(name)!;
                    var targets = new Node[] {
                            button, _labelToAnimate, _texture, _logo
                        }.Where(node => animation.IsCompatibleWith(node));
                    _effectsMenuOnFinish = () => {
                        button.Disabled = false;
                        buttonRestorer.Restore();
                        _animationsRestorer.Restore();
                        _effectsMenuSceneTreeTween = null;
                        _effectsMenuOnFinish = null;
                    };
                    _effectsMenuSceneTreeTween = animation.Play(targets, 0.2f, 0f, MenuEffectTime);
                    _effectsMenuSceneTreeTween.AwaitFinished().OnCompleted(_effectsMenuOnFinish);
                });
            }
        }


        private SceneTreeTween _sceneTreeTweenDimEffect;
        public void DimOut() {
            _sceneTreeTweenDimEffect?.Kill();
            _sceneTreeTweenDimEffect = Templates.FadeOut.Play(this, 0f, 1f);
        }

        public void RollbackDimOut() {
            _sceneTreeTweenDimEffect?.Kill();
            _sceneTreeTweenDimEffect = null;
            Modulate = Colors.White;
        }

        public async Task BackMenu() {
            await _menuContainer.Back();
        }

        private const float AllMenuEffectTime = 0.5f;
        private const float DelayPerTarget = 0.05f;

        private async Task GoGoodbyeAnimation(MenuTransition transition) {
            var effect = _optionButtonExit.GetItemText(_optionButtonExit.GetSelectedId());
            if (effect == null) return;
            _menuLabel.Text = "Exit menu: playing " + effect + "...";
            var children = transition.FromMenu.GetVisibleControl();
            var delayPerTarget = DelayPerTarget * children.Count > AllMenuEffectTime
                ? AllMenuEffectTime / children.Count
                : DelayPerTarget;
            await Templates.Get(effect)!.Play(children, delayPerTarget, 0f, MenuEffectTime).AwaitFinished();
            _menuLabel.Text = "";
        }

        private async Task GoNewMenuAnimation(MenuTransition transition) {
            var effect = _optionButtonEnter.GetItemText(_optionButtonEnter.GetSelectedId());
            if (effect == null) return;
            _menuLabel.Text = "Enter menu: playing " + effect + "...";
            var children = transition.ToMenu.GetChildren();
            await MenuAnimation(children, effect);
        }

        private async Task BackGoodbyeAnimation(MenuTransition transition) {
            var effect = _optionButtonExit.GetItemText(_optionButtonExit.GetSelectedId());
            if (effect == null) return;
            _menuLabel.Text = "Exit menu: playing " + effect + "...";
            var children = transition.FromMenu.GetVisibleControl();
            await MenuAnimation(children, effect);
        }

        private async Task BackNewMenuAnimation(MenuTransition transition) {
            var effect = _optionButtonEnter.GetItemText(_optionButtonEnter.GetSelectedId());
            if (effect == null) return;
            _menuLabel.Text = "Enter menu: playing " + effect + "...";
            var children = transition.ToMenu.GetChildren();
            await MenuAnimation(children, effect);
        }

        private async Task MenuAnimation(List<Control> children, string effect) {
            var animation = Templates.Get(effect)!;
            children = children.Where(node => animation.IsCompatibleWith(node)).ToList();
            await animation.Play(children, DelayPerTarget, 0f, MenuEffectTime, AllMenuEffectTime).AwaitFinished();
            _menuLabel.Text = "";
        }
    }
}