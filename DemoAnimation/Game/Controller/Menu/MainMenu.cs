using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Betauer;
using Betauer.Animation;
using Betauer.DI;
using Betauer.Input;
using Betauer.UI;
using Godot;
using Veronenger.Game.Managers;

namespace Veronenger.Game.Controller.Menu {
    public class MainMenu : Node2D {
        private const float MenuEffectTime = 1f;
        private const float FadeMainMenuEffectTime = 0.75f;

        [OnReady("MainMenu/VBoxContainer/ScrollContainer/Menu")]
        private Godot.Container _menuBase;

        [OnReady("HBoxContainer/DemoMenu")] private Control _demoMenuContainer;
        [OnReady("HBoxContainer/DemoMenu/HBox/Menu")] private Godot.Container _menuDemoBase;

        [OnReady("HBoxContainer/Animator")] private Control _animatorContainer;
        [OnReady("HBoxContainer/Animator/LabelToAnimate")] private Label _labelToAnimate;

        [OnReady("HBoxContainer/Animator/SpriteToAnimate")] private Sprite _logo;
        [OnReady("HBoxContainer/Animator/TextureRectToAnimate")] private TextureRect _texture;

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


        private MenuController _menuController;
        private MenuController _demoMenu;

        [Inject] private GameManager _gameManager;
        private readonly Launcher _launcher = new Launcher();

        [Inject] private ActionState UiAccept;
        [Inject] private ActionState UiCancel;
        [Inject] private ActionState UiStart;

        private Restorer _animationsRestorer;
        private Restorer _menuRestorer;


        private readonly List<TemplateFactory> _templateFactories = Template.GetAllTemplates()
            .OrderBy(factory => (factory.Category ?? "0") + "-" + factory.Category + "-" + factory.Name)
            .ToList();

        protected override void Dispose(bool disposing) {
            base.Dispose(disposing);
            if (disposing) {
                _resetMenuOnPressedHandler?.Free();
            }
        }

        private SignalHandler _resetMenuOnPressedHandler;

        public override void _Ready() {
            _launcher.WithParent(this);
            _animationsRestorer = new MultiRestorer(_labelToAnimate, _logo, _texture).Save();
            _menuController = BuildMenu();
            _demoMenu = BuildDemoMenu();
            _menuLabel.Text = "Click the options to see animations";
            _resetMenuOnPressedHandler = _resetMenu.OnPressed(() => {
                _demoMenu.QueueFree();
                _demoMenu = BuildDemoMenu();
                _demoMenu.Start();

            });
            string oldCategory = null;
            const string animationForExit = nameof(Template.BackOutDown);
            const string animationForEnter = nameof(Template.BounceInRight);
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
            await _demoMenu.Start();
            await _menuController.Start();
            await _launcher.Play(Template.FadeIn, this, 0f, FadeMainMenuEffectTime).Await();
            GetTree().Root.GuiDisableInput = false;
        }

        public async Task HideMainMenu() {
            GetTree().Root.GuiDisableInput = true;
            await _launcher.Play(Template.FadeOut, this, 0f, FadeMainMenuEffectTime).Await();
            Visible = false;
            GetTree().Root.GuiDisableInput = false;
        }

        public void DisableMenus() {
            _menuRestorer = _menuController.ActiveMenu!.DisableButtons().AddFocusRestorer(_menuLabel);
        }

        public void EnableMenus() {
            _menuRestorer?.Restore();
        }

        public MenuController BuildMenu() {
            foreach (var child in _menuBase.GetChildren()) (child as Node)?.Free();

            var mainMenu = new MenuController(_menuBase);
            mainMenu
                .AddMenu("Root", (c) => {
                    _demoMenuContainer.Visible = false;
                    _animatorContainer.Visible = false;
                })
                .AddButton("Start", "Anima comparision", (ctx) => { _gameManager.LoadAnimaDemo(); })
                .AddButton("Effects", "Show all animations", async (ctx) => {
                    _demoMenuContainer.Visible = false;
                    _animatorContainer.Visible = true;
                    await ctx.Go("Effects");
                })
                .AddButton("Menus", "Demo with menus", async (ctx) => {
                    _demoMenuContainer.Visible = true;
                    _animatorContainer.Visible = false;
                })
                .AddButton("Exit", "Exit", async (ctx) => { _gameManager.TriggerModalBoxConfirmExitDesktop(); });

            var effectsMenu = mainMenu.AddMenu("Effects");
            CreateEffectsMenu(effectsMenu);
            return mainMenu;
        }

        public MenuController BuildDemoMenu() {
            foreach (var child in _menuDemoBase.GetChildren()) (child as Node)?.Free();

            var mainMenu = new MenuController(_menuDemoBase);
            mainMenu.AddMenu("Root")
                .AddButton("x", "Start new game", (ctx) => { ctx.Go("1", GoGoodbyeAnimation, GoNewMenuAnimation); })
                .AddButton("x", "Continue", (ctx) => { ctx.Go("1", GoGoodbyeAnimation, GoNewMenuAnimation); })
                .AddButton("x", "Settings", (ctx) => { ctx.Go("1", GoGoodbyeAnimation, GoNewMenuAnimation); })
                .AddButton("x", "Online", (ctx) => { ctx.Go("1", GoGoodbyeAnimation, GoNewMenuAnimation); })
                .AddButton("x", "Credits", (ctx) => { ctx.Go("1", GoGoodbyeAnimation, GoNewMenuAnimation); })
                .AddButton("x", "Exit game", (ctx) => { ctx.Go("1", GoGoodbyeAnimation, GoNewMenuAnimation); });

            mainMenu.AddMenu("1")
                .AddButton("x", "Come back", (ctx) => { ctx.Back(BackGoodbyeAnimation, BackNewMenuAnimation); })
                .AddButton("x", "Exit to main menu", (ctx) => { ctx.Back(BackGoodbyeAnimation, BackNewMenuAnimation); })
                .AddButton("x", "Back", (ctx) => { ctx.Back(BackGoodbyeAnimation, BackNewMenuAnimation); })
                .AddButton("x", "Open main menu", (ctx) => { ctx.Back(BackGoodbyeAnimation, BackNewMenuAnimation); })
                .AddButton("x", "Return", (ctx) => { ctx.Back(BackGoodbyeAnimation, BackNewMenuAnimation); })
                .AddButton("x", "Quit", (ctx) => { ctx.Back(BackGoodbyeAnimation, BackNewMenuAnimation); });
            return mainMenu;
        }

        private void CreateEffectsMenu(ActionMenu effectsMenu) {
            var oldCategory = "@";
            foreach (var templateFactory in _templateFactories) {
                var name = templateFactory.Name;
                if (templateFactory.Category != oldCategory && templateFactory.Category != null) {
                    effectsMenu.AddHSeparator();
                    effectsMenu.AddNode(new Label {
                        Text = templateFactory.Category
                    });
                    oldCategory = templateFactory.Category;
                }

                effectsMenu.AddButton(name, name, async (ctx) => {
                    var buttonRestorer = ctx.ActionButton.CreateRestorer().Save();
                    ctx.ActionButton.Disabled = true;
                    var targets = new Node[] { ctx.ActionButton, _labelToAnimate, _texture, _logo };
                    await _launcher.MultiPlay(Template.Get(name), targets, 0.5f, 0f, MenuEffectTime).Await();
                    await this.AwaitIdleFrame();
                    ctx.ActionButton.Disabled = false;
                    buttonRestorer.Restore();
                    _animationsRestorer.Restore();
                });
            }
            effectsMenu.AddButton("Back", "Back", async (ctx) => { await ctx.Back(); });
        }

        public void DimOut() {
            _launcher.Play(Template.FadeOut, this, 0f, 1f).Await();
        }

        public void RollbackDimOut() {
            _launcher.RemoveAll();
            Modulate = Colors.White;
        }

        public bool IsRootMenuActive() {
            return _menuController.ActiveMenu?.Name == "Root";
        }

        public async Task BackMenu() {
            await _menuController.Back();
        }

        private const float AllMenuEffectTime = 0.5f;
        private const float DelayPerTarget = 0.05f;

        private async Task GoGoodbyeAnimation(MenuTransition transition) {
            var effect = _optionButtonExit.GetItemText(_optionButtonExit.GetSelectedId());
            if (effect == null) return;
            _menuLabel.Text = "Exit menu: playing "+effect+"...";
            var children = transition.FromMenu.GetVisibleControl();
            var delayPerTarget = DelayPerTarget * children.Count > AllMenuEffectTime
                ? AllMenuEffectTime / children.Count
                : DelayPerTarget;
            await _launcher.MultiPlay(Template.Get(effect), children, delayPerTarget, 0f, MenuEffectTime).Await();
            _menuLabel.Text = "";
        }

        private async Task GoNewMenuAnimation(MenuTransition transition) {
            var effect = _optionButtonEnter.GetItemText(_optionButtonEnter.GetSelectedId());
            if (effect == null) return;
            _menuLabel.Text = "Enter menu: playing "+effect+"...";
            var children = transition.ToMenu.GetChildren();
            var delayPerTarget = DelayPerTarget * children.Count > AllMenuEffectTime
                ? AllMenuEffectTime / children.Count
                : DelayPerTarget;
            await _launcher.MultiPlay(Template.Get(effect), children, delayPerTarget, 0f, MenuEffectTime).Await();
            _menuLabel.Text = "";
        }


        private async Task BackGoodbyeAnimation(MenuTransition transition) {
            var effect = _optionButtonExit.GetItemText(_optionButtonExit.GetSelectedId());
            if (effect == null) return;
            _menuLabel.Text = "Exit menu: playing "+effect+"...";
            var children = transition.FromMenu.GetVisibleControl();
            var delayPerTarget = DelayPerTarget * children.Count > AllMenuEffectTime
                ? AllMenuEffectTime / children.Count
                : DelayPerTarget;
            await _launcher.MultiPlay(Template.Get(effect), children, delayPerTarget, 0f, MenuEffectTime).Await();
            _menuLabel.Text = "";
        }

        private async Task BackNewMenuAnimation(MenuTransition transition) {
            var effect = _optionButtonEnter.GetItemText(_optionButtonEnter.GetSelectedId());
            if (effect == null) return;
            _menuLabel.Text = "Enter menu: playing "+effect+"...";
            var children = transition.ToMenu.GetChildren();
            var delayPerTarget = DelayPerTarget * children.Count > AllMenuEffectTime
                ? AllMenuEffectTime / children.Count
                : DelayPerTarget;
            await _launcher.MultiPlay(Template.Get(effect), children, delayPerTarget, 0f, MenuEffectTime).Await();
            _menuLabel.Text = "";
        }
    }
}