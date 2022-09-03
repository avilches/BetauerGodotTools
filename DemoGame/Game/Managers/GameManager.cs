using System;
using System.Threading.Tasks;
using Betauer.Animation;
using Godot;
using Betauer.Application.Screen;
using Betauer.DI;
using Betauer.Input;
using Betauer.Loader;
using Betauer.Memory;
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
            ModalBoxConfirmQuitGame,
            ExitDesktop
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

        [Scene("res://Scenes/DebugOverlay.tscn")] public DebugOverlay DebugOverlay;

        private Node _currentGameScene;
        private Node2D _playerScene;

        [Inject] private StageManager StageManager { get; set; }
        [Inject] private ScreenSettingsManager ScreenSettingsManager { get; set; }
        [Inject] private SceneTree SceneTree { get; set; }
        [Inject] private MainResourceLoader MainResourceLoader { get; set; }

        [Inject] private InputAction UiAccept { get; set; }
        [Inject] private InputAction UiCancel { get; set; }
        [Inject] private InputAction UiStart { get; set; }

        public override void _Ready() {
            PauseMode = PauseModeEnum.Process;
        }

        public GameManager() : base(State.Init) {
            CreateState(State.Init)
                .Execute(async (ctx) => {
                    MainResourceLoader.OnProgress += context => {
                        // GD.Print(context.LoadPercent.ToString("P") + " = " + context.LoadedSize + " / " +
                        // context.TotalSize + " resource " + context.ResourceLoadedPercent.ToString("P") + " = " +
                        // context.ResourceLoadedSize + " / " + context.ResourceSize + " " + context.ResourcePath);
                    };
                    await MainResourceLoader.Load(this);
                    ScreenSettingsManager.Setup();
                    ConfigureDebugOverlays();
                    // Never pause the pause, settings and the state machine, because they will not work!
                    _settingsMenuScene.PauseMode = _pauseMenuScene.PauseMode = PauseModeEnum.Process;

                    SceneTree.Root.AddChild(DebugOverlay);
                    SceneTree.Root.AddChild(_pauseMenuScene);
                    SceneTree.Root.AddChild(_settingsMenuScene);
                    SceneTree.Root.AddChild(_mainMenuScene);
                    SceneTree.Root.AddChild(MainMenuBottomBarScene);
                    ConfigureStates();
                    return ctx.Set(State.MainMenu);
                }).Build();
        }

        private void ConfigureDebugOverlays() {
            DebugOverlay.Add("ObjectWatcher").Do(() => DefaultObjectWatcherRunner.Instance.Count.ToString());
            DebugOverlay.Add("SceneTreeTween")
                .Do(() => DefaultTweenCallbackManager.Instance.ActionsPerSceneTree.Count.ToString());
        }

        private void ConfigureStates() {
            AddListener(MainMenuBottomBarScene);

            OnInput(State.MainMenu, _mainMenuScene.OnInput);
            CreateState(State.MainMenu)
                .On(Transition.StartGame, context => context.Set(State.StartingGame))
                .On(Transition.Settings, context => context.Push(State.Settings))
                .Suspend(() => _mainMenuScene.DisableMenus())
                .Awake(() => _mainMenuScene.EnableMenus())
                .Enter(async () => await _mainMenuScene.ShowMenu())
                .Build();

            OnInput(State.Settings, _settingsMenuScene.OnInput);
            CreateState(State.Settings)
                .On(Transition.Back, context => context.Pop())
                .Enter(_settingsMenuScene.ShowSettingsMenu)
                .Exit(_settingsMenuScene.HideSettingsMenu)
                .Build();

            CreateState(State.StartingGame)
                .Enter(async () => {
                    await _mainMenuScene.HideMainMenu();
                    _currentGameScene = MainResourceLoader.CreateWorld1();
                    await AddSceneDeferred(_currentGameScene);
                    AddPlayerToScene(_currentGameScene);
                })
                .Execute(context => context.Set(State.Gaming))
                .Build();

            OnInput(State.Gaming, (e) => {
                if (UiStart.IsEventJustPressed(e)) {
                    Enqueue(Transition.Pause);
                    GetTree().SetInputAsHandled();
                }
            });
            CreateState(State.Gaming)
                .On(Transition.Back, context => context.Pop())
                .On(Transition.Pause, context => context.Push(State.PauseMenu))
                .Exit(() => {
                    _currentGameScene.PrintStrayNodes();
                    _currentGameScene.QueueFree();
                    _currentGameScene = null;
                })
                .Build();
                

            OnInput(State.PauseMenu, _pauseMenuScene.OnInput);
            CreateState(State.PauseMenu)
                .On(Transition.Back, context => context.Pop())
                .On(Transition.Settings, context => context.Push(State.Settings))
                .Suspend(() => _pauseMenuScene.DisableMenus())
                .Awake(() => _pauseMenuScene.EnableMenus())
                .Enter(async () => {
                    SceneTree.Paused = true;
                    await _pauseMenuScene.ShowPauseMenu();
                })
                .Exit(() => {
                    _pauseMenuScene.EnableMenus();
                    _pauseMenuScene.HidePauseMenu();
                    SceneTree.Paused = false;
                })
                .Build();

            On(Transition.ModalBoxConfirmQuitGame, context => context.Push(State.ModalQuitGame));
            CreateState(State.ModalQuitGame)
                .On(Transition.Back, context => context.Pop())
                .Execute(async (context) => {
                    var result = await ShowModalBox("Quit game?", "Any progress not saved will be lost");
                    return result ? context.Set(State.MainMenu) : context.Pop();
                })
                .Build();
                

            On(Transition.ModalBoxConfirmExitDesktop, context => context.Push(State.ModalExitDesktop));
            CreateState(State.ModalExitDesktop)
                .On(Transition.Back, context => context.Pop())
                .Enter(() => _mainMenuScene.DimOut())
                .Exit(() => _mainMenuScene.RollbackDimOut())
                .Execute(async (context) => {
                    var result = await ShowModalBox("Exit game?");
                    return result ? context.Set(State.ExitDesktop) : context.Pop();
                })
                .Build();
                
            On(Transition.ExitDesktop, context => context.Set(State.ExitDesktop));
            CreateState(State.ExitDesktop)
                .Enter(() => SceneTree.Notification(MainLoop.NotificationWmQuitRequest))
                .Build();
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

        public void TriggerExitDesktop() {
            Enqueue(Transition.ExitDesktop);
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