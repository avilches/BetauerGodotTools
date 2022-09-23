using System;
using System.Linq;
using System.Threading.Tasks;
using Betauer;
using Betauer.Animation.Tween;
using Betauer.Application.Monitor;
using Godot;
using Betauer.Application.Screen;
using Betauer.DI;
using Betauer.Input;
using Betauer.Loader;
using Betauer.Nodes;
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

        [Load("res://Scenes/Menu/MainMenu.tscn")]
        private MainMenu _mainMenuScene;

        [Load("res://Scenes/Menu/MainMenuBottomBar.tscn")]
        public MainMenuBottomBar MainMenuBottomBarScene;

        [Load("res://Scenes/Menu/PauseMenu.tscn")]
        private PauseMenu _pauseMenuScene;

        [Load("res://Scenes/Menu/SettingsMenu.tscn")]
        private SettingsMenu _settingsMenuScene;

        [Load("res://Scenes/Menu/ModalBoxConfirm.tscn")]
        private Func<ModalBoxConfirm> CreateModalBoxConfirm;

        [Load("res://Assets/UI/my_theme.tres")]
        private Theme MyTheme;

        private Node _currentGameScene;
        private Node2D _playerScene;

        [Inject] private StageManager StageManager { get; set; }
        [Inject] private ScreenSettingsManager ScreenSettingsManager { get; set; }
        [Inject] private SceneTree SceneTree { get; set; }
        [Inject] private MainResourceLoader MainResourceLoader { get; set; }
        [Inject] private DebugOverlay DebugOverlay { get; set; }

        [Inject] private InputAction UiAccept { get; set; }
        [Inject] private InputAction UiCancel { get; set; }
        [Inject] private InputAction ControllerStart { get; set; }

        public override void _Ready() {
            PauseMode = PauseModeEnum.Process;
        }

        public GameManager() : base(State.Init, "GameManager") {
            CreateState(State.Init)
                .Execute(async (ctx) => {
                    MainResourceLoader.OnProgress += context => {
                        // GD.Print(context.LoadPercent.ToString("P") + " = " + context.LoadedSize + " / " +
                        // context.TotalSize + " resource " + context.ResourceLoadedPercent.ToString("P") + " = " +
                        // context.ResourceLoadedSize + " / " + context.ResourceSize + " " + context.ResourcePath);
                    };
                    await MainResourceLoader.From(this).Load();
                    ScreenSettingsManager.Setup();
                    ConfigureDebugOverlays();
                    // Never pause the pause, settings and the state machine, because they will not work!
                    _settingsMenuScene.PauseMode = _pauseMenuScene.PauseMode = PauseModeEnum.Process;

                    SceneTree.Root.AddChild(_pauseMenuScene);
                    SceneTree.Root.AddChild(_settingsMenuScene);
                    SceneTree.Root.AddChild(_mainMenuScene);
                    SceneTree.Root.AddChild(MainMenuBottomBarScene);
                    ConfigureStates();
                    return ctx.Set(State.MainMenu);
                }).Build();
        }

        private void ConfigureDebugOverlays() {
            DebugOverlay.Panel.Theme = MyTheme;
            DebugOverlay.MonitorFpsAndMemory();
            DebugOverlay.MonitorObjectRunnerSize();
            DebugOverlay.CreateMonitor().WithPrefix("Tweens w/callbacks").Show(() => DefaultTweenCallbackManager.Instance.ActionsByTween.Count.ToString());
            DebugOverlay.CreateMonitor().WithPrefix("Objects w/signals").Show(() => DefaultSignalManager.Instance.SignalsByObject.Count.ToString());
            DebugOverlay.CreateMonitor().Show(() => ScreenSettingsManager.GetStateAsString());
            // SceneTree.Root.AddChild(CreateSignalManagerDebugOverlay());
        }

        private DebugOverlay CreateSignalManagerDebugOverlay() {
            var debugOverlay = new DebugOverlay();
            debugOverlay.Panel.Theme = MyTheme;
            debugOverlay.CreateMonitor().Show(() => {
                var txt = "";
                foreach (var objectSignals in DefaultSignalManager.Instance.SignalsByObject.Values) {
                    txt += objectSignals.Emitter.ToStringSafe() + " (" + objectSignals.Signals.Count + "): " +
                           string.Join(", ", objectSignals.Signals.Select(s => s.Signal)) + "\n";
                }

                return txt;
            });
            return debugOverlay;
        }

        private void ConfigureStates() {
            #if DEBUG
            this.OnInput((e) => {
                if (e.IsKeyPressed(KeyList.Q)) {
                    _settingsMenuScene.Scale -= new Vector2(0.1f, 0.1f);
                }
                if (e.IsKeyPressed(KeyList.W)) {
                    _settingsMenuScene.Scale = new Vector2(1, 1);
                }
                if (e.IsKeyPressed(KeyList.E)) {
                    _settingsMenuScene.Scale += new Vector2(0.1f, 0.1f);
                }
                if (e.IsKeyPressed(KeyList.Key1)) {
                    ScreenSettingsManager.ScreenConfiguration.StretchAspect = SceneTree.StretchAspect.Expand;
                    ScreenSettingsManager.SetScreenConfiguration(ScreenSettingsManager.ScreenConfiguration);
                    // ScreenSettingsManager.SetWindowed(ScreenSettingsManager.WindowedResolution);
                }
                if (e.IsKeyPressed(KeyList.Key2)) {
                    ScreenSettingsManager.ScreenConfiguration.StretchAspect = SceneTree.StretchAspect.Keep;
                    ScreenSettingsManager.SetScreenConfiguration(ScreenSettingsManager.ScreenConfiguration);
                    // ScreenSettingsManager.SetWindowed(ScreenSettingsManager.WindowedResolution);
                }
                if (e.IsKeyPressed(KeyList.Key3)) {
                    ScreenSettingsManager.ScreenConfiguration.StretchAspect = SceneTree.StretchAspect.KeepHeight;
                    ScreenSettingsManager.SetScreenConfiguration(ScreenSettingsManager.ScreenConfiguration);
                    // ScreenSettingsManager.SetWindowed(ScreenSettingsManager.WindowedResolution);
                }
                if (e.IsKeyPressed(KeyList.Key4)) {
                    ScreenSettingsManager.ScreenConfiguration.StretchAspect = SceneTree.StretchAspect.KeepWidth;
                    ScreenSettingsManager.SetScreenConfiguration(ScreenSettingsManager.ScreenConfiguration);
                    // ScreenSettingsManager.SetWindowed(ScreenSettingsManager.WindowedResolution);
                }
                if (e.IsKeyPressed(KeyList.Key6)) {
                    ScreenSettingsManager.ScreenConfiguration.StretchMode = SceneTree.StretchMode.Viewport;
                    ScreenSettingsManager.SetScreenConfiguration(ScreenSettingsManager.ScreenConfiguration);
                    // ScreenSettingsManager.SetWindowed(ScreenSettingsManager.WindowedResolution);
                }
                if (e.IsKeyPressed(KeyList.Key7)) {
                    ScreenSettingsManager.ScreenConfiguration.StretchMode = SceneTree.StretchMode.Mode2d;
                    ScreenSettingsManager.SetScreenConfiguration(ScreenSettingsManager.ScreenConfiguration);
                    // ScreenSettingsManager.SetWindowed(ScreenSettingsManager.WindowedResolution);
                }
                if (e.IsKeyPressed(KeyList.Key8)) {
                    ScreenSettingsManager.SetScreenConfiguration(ScreenSettingsManager.ScreenConfiguration, ScreenService.ScreenStrategyKey.IntegerScale);
                    // ScreenSettingsManager.SetWindowed(ScreenSettingsManager.WindowedResolution);
                }
                if (e.IsKeyPressed(KeyList.Key9)) {               
                    ScreenSettingsManager.SetScreenConfiguration(ScreenSettingsManager.ScreenConfiguration, ScreenService.ScreenStrategyKey.ViewportSize);
                    // ScreenSettingsManager.SetWindowed(ScreenSettingsManager.WindowedResolution);
                }
                if (e.IsKeyPressed(KeyList.Key0)) {
                    ScreenSettingsManager.SetScreenConfiguration(ScreenSettingsManager.ScreenConfiguration, ScreenService.ScreenStrategyKey.WindowSize);
                    // ScreenSettingsManager.SetWindowed(ScreenSettingsManager.WindowedResolution);
                }
            }, PauseModeEnum.Process);
            #endif
            
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
                if (ControllerStart.IsEventJustPressed(e)) {
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