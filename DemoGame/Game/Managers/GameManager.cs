using System;
using System.Threading.Tasks;
using Godot;
using Betauer.Animation;
using Betauer.Application;
using Betauer.Application.Screen;
using Betauer.DI;
using Betauer.Input;
using Betauer.Loader;
using Betauer.Signal;
using Betauer.StateMachine;
using Veronenger.Game.Controller.Menu;

namespace Veronenger.Game.Managers {
    [Service]
    public class GameManager : StateMachineNode<GameManager.State, GameManager.Transition> {

        public enum Transition {
            Back,
            Pause,
            Settings,
            StartGame,
            ModalBoxConfirmExitDesktop,
            ModalBoxConfirmQuitGame
        }

        public enum State {
            Init,
            MainMenu,
            Settings,
            StartingGame,
            Gaming,
            PauseMenu,
            ModalQuitGame,
            ModalExitDesktop,
            ExitDesktop,
        }

        [Scene("res://Scenes/Menu/MainMenu.tscn")]
        private MainMenu _mainMenuScene;

        [Scene("res://Scenes/Menu/MainMenuBottomBar.tscn")]
        public MainMenuBottomBar MainMenuBottomBarScene;

        [Scene("res://Scenes/Menu/PauseMenu.tscn")]
        private PauseMenu _pauseMenuScene;

        [Scene("res://Scenes/Menu/SettingsMenu.tscn")]
        private SettingsMenu _settingsMenuScene;

        [Scene("res://Scenes/Menu/ModalBoxConfirm.tscn")]
        private Func<ModalBoxConfirm> CreateModalBoxConfirm;
        
        private Node _currentGameScene;
        private Node2D _playerScene;

        private readonly Launcher _launcher = new Launcher();

        [Inject] private StageManager StageManager { get; set; }
        [Inject] private ScreenSettingsManager ScreenSettingsManager { get; set; }
        [Inject] private SceneTree SceneTree { get; set; }
        [Inject] private MainResourceLoader MainResourceLoader { get; set; }

        [Inject] private InputAction PixelPerfectInputAction { get; set; }
        [Inject] private InputAction UiAccept { get; set; }
        [Inject] private InputAction UiCancel { get; set; }
        [Inject] private InputAction UiStart { get; set; }

        public override void _Ready() {
            PauseMode = PauseModeEnum.Process;
        }

        public GameManager() : base(State.Init) {
            var builder = CreateBuilder();
            builder.State(State.Init)
                .Execute(async (ctx) => {
                    MainResourceLoader.OnProgress += context => {
                        // GD.Print(context.TotalLoadedPercent.ToString("P") + " = " + context.TotalLoadedSize + " / " +
                        // context.TotalSize + " resource " + context.ResourceLoadedPercent.ToString("P") + " = " +
                        // context.ResourceLoadedSize + " / " + context.ResourceSize + " " + context.ResourcePath);
                    };
                    await MainResourceLoader.Bind(this).Load();
                    ScreenSettingsManager.Setup();
                    // Never pause the pause, settings and the state machine, because they will not work!
                    _settingsMenuScene.PauseMode = _pauseMenuScene.PauseMode = PauseModeEnum.Process;

                    _launcher.WithParent(SceneTree.Root);
                    SceneTree.Root.AddChild(_pauseMenuScene);
                    SceneTree.Root.AddChild(_settingsMenuScene);
                    SceneTree.Root.AddChild(_mainMenuScene);
                    SceneTree.Root.AddChild(MainMenuBottomBarScene);
                    ConfigureStates();
                    return ctx.Set(State.MainMenu);
                });
            builder.Build();
        }
        
        private void ConfigureStates() {
            var builder = CreateBuilder();
            AddListener(MainMenuBottomBarScene);
            builder.State(State.MainMenu)
                .On(Transition.StartGame, context => context.Set(State.StartingGame))
                .On(Transition.Settings, context => context.Push(State.Settings))
                .Suspend(() => _mainMenuScene.DisableMenus())
                .Awake(() => _mainMenuScene.EnableMenus())
                .Enter(async () => await _mainMenuScene.ShowMenu())
                .Execute(_mainMenuScene.Execute);

            builder.State(State.Settings)
                .On(Transition.Back, context => context.Pop())
                .Enter(_settingsMenuScene.ShowSettingsMenu)
                .Execute(_settingsMenuScene.Execute)
                .Exit(_settingsMenuScene.HideSettingsMenu);

            builder.State(State.StartingGame)
                .Enter(async () => {
                    await _mainMenuScene.HideMainMenu();
                    _currentGameScene = MainResourceLoader.CreateWorld1();
                    await AddSceneDeferred(_currentGameScene);
                    AddPlayerToScene(_currentGameScene);
                })
                .Execute(context => context.Set(State.Gaming));

            // OnInput(State.Gaming, (e) => {
                // if (PixelPerfectInputAction.IsActionPressed(e)) {
                    // ScreenSettingsManager.SetPixelPerfect(!ScreenSettingsManager.PixelPerfect);
                // }
            // });
            builder.State(State.Gaming)
                .On(Transition.Back, context => context.Pop())
                .On(Transition.Pause, context => context.Push(State.PauseMenu))
                .Execute(context => {
                    if (UiStart.JustPressed()) {
                        return context.Trigger(Transition.Pause);
                    }
                    return context.None();
                })
                .Exit(() => {
                    _currentGameScene.PrintStrayNodes();
                    _currentGameScene.QueueFree();
                    _currentGameScene = null;
                });

            builder.State(State.PauseMenu)
                .On(Transition.Back, context => context.Pop())
                .On(Transition.Settings, context => context.Push(State.Settings))
                .Suspend(() => _pauseMenuScene.DisableMenus())
                .Awake(() => _pauseMenuScene.EnableMenus())
                .Enter(async () => {
                    SceneTree.Paused = true;
                    await _pauseMenuScene.ShowPauseMenu();
                })
                .Execute(_pauseMenuScene.Execute)
                .Exit(() => {
                    _pauseMenuScene.EnableMenus();
                    _pauseMenuScene.HidePauseMenu();
                    SceneTree.Paused = false;
                });

            builder.On(Transition.ModalBoxConfirmQuitGame, context => context.Push(State.ModalQuitGame));
            builder.State(State.ModalQuitGame)
                .On(Transition.Back, context => context.Pop())
                .Execute(async (context) => {
                    var result = await ShowModalBox("Quit game?", "Any progress not saved will be lost");
                    return result ? context.Set(State.MainMenu) : context.Pop();
                });

            builder.On(Transition.ModalBoxConfirmExitDesktop, context => context.Push(State.ModalExitDesktop));
            builder.State(State.ModalExitDesktop)
                .On(Transition.Back, context => context.Pop())
                .Enter(() => _mainMenuScene.DimOut())
                .Exit(() => _mainMenuScene.RollbackDimOut())
                .Execute(async (context) => {
                    var result = await ShowModalBox("Exit game?");
                    return result ? context.Push(State.ExitDesktop) : context.Pop();
                });

            builder.State(State.ExitDesktop)
                .Enter(() => SceneTree.Notification(MainLoop.NotificationWmQuitRequest));

            builder.Build();
        }

        public void TriggerStartGame() {
            Enqueue(Transition.StartGame);
        }

        public void TriggerPauseMenu() {
            Enqueue(Transition.Pause);
        }

        public void TriggerSettings() {
            Enqueue(Transition.Settings);
        }

        public void TriggerBack() {
            Enqueue(Transition.Back);
        }

        public void TriggerModalBoxConfirmExitDesktop() {
            Enqueue(Transition.ModalBoxConfirmExitDesktop);
        }

        public void TriggerModalBoxConfirmQuitGame() {
            Enqueue(Transition.ModalBoxConfirmQuitGame);
        }

        private async Task<bool> ShowModalBox(string title, string subtitle = null) {
            ModalBoxConfirm modalBoxConfirm = CreateModalBoxConfirm();
            modalBoxConfirm.Title(title, subtitle);
            modalBoxConfirm.PauseMode = PauseModeEnum.Process;
            SceneTree.Root.AddChild(modalBoxConfirm);
            var result = await modalBoxConfirm.AwaitResult();
            modalBoxConfirm.QueueFree();
            return result;
        }

        public async void QueueChangeSceneWithPlayer(string sceneName) {
            StageManager.ClearTransition();
            _currentGameScene.QueueFree();

            var nextScene = ResourceLoader.Load<PackedScene>(sceneName).Instance();
            await AddSceneDeferred(nextScene);
            AddPlayerToScene(nextScene);
            _currentGameScene = nextScene;
        }

        private void AddPlayerToScene(Node nextScene) {
            _playerScene = MainResourceLoader.CreatePlayer();
            nextScene.AddChild(_playerScene);
            var position2D = nextScene.GetNode<Node2D>("PositionPlayer");
            if (position2D == null) {
                throw new Exception("Node PositionPlayer not found when loading scene " + nextScene.Filename);
            }
            _playerScene.GlobalPosition = position2D.GlobalPosition;
        }

        private async Task AddSceneDeferred(Node scene) {
            await SceneTree.AwaitIdleFrame();
            SceneTree.Root.AddChild(scene);
        }

    }
}