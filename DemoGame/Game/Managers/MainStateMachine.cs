using System;
using System.Threading.Tasks;
using Betauer.Application.Monitor;
using Godot;
using Betauer.Application.Screen;
using Betauer.DI;
using Betauer.Input;
using Betauer.Loader;
using Betauer.Nodes;
using Betauer.Signal;
using Betauer.StateMachine.Async;
using Veronenger.Game.Controller.Menu;

namespace Veronenger.Game.Managers {
    
    public enum MainState {
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
    
    public enum MainTransition {
        Back,
        Pause,
        Settings,
        StartGame,
        EndGame,
        ModalBoxConfirmExitDesktop,
        ModalBoxConfirmQuitGame,
        ExitDesktop
    }
    
    [Service]
    public class MainStateMachine : StateMachineNodeAsync<MainState, MainTransition> {

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

        [Inject] private Game Game { get; set; }

        [Inject] private ScreenSettingsManager ScreenSettingsManager { get; set; }
        [Inject] private SceneTree SceneTree { get; set; }
        [Inject] private MainResourceLoader MainResourceLoader { get; set; }
        [Inject] private DebugOverlay DefaultDebugOverlay { get; set; }
        [Inject] private DebugOverlayManager DebugOverlayManager { get; set; }

        [Inject] private InputAction UiAccept { get; set; }
        [Inject] private InputAction UiCancel { get; set; }
        [Inject] private InputAction ControllerStart { get; set; }

        [Inject] private Bus Bus { get; set; }

        public override void _Ready() {
            PauseMode = PauseModeEnum.Process;
        }

        public MainStateMachine() : base(MainState.Init) {
        }

        [PostCreate]
        private void Configure() {
            #if DEBUG
            this.OnInput((e) => {
                if (e.IsKeyPressed(KeyList.Q)) {
                    // _settingsMenuScene.Scale -= new Vector2(0.05f, 0.05f);
                    // Engine.TimeScale -= 0.05f;
                }
                if (e.IsKeyPressed(KeyList.W)) {
                    // _settingsMenuScene.Scale = new Vector2(1, 1);
                    // Engine.TimeScale = 1;
                }
                if (e.IsKeyPressed(KeyList.E)) {
                    // Engine.TimeScale += 0.05f;
                    // _settingsMenuScene.Scale += new Vector2(0.05f, 0.05f);
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
            
            Bus.Subscribe(Enqueue);
            
            State(MainState.Init)
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
                    AddOnTransition((args) => MainMenuBottomBarScene.UpdateState(args.To));
                    return ctx.Set(MainState.StartingGame);
                }).Build();
            
            State(MainState.MainMenu)
                .OnInput(e => _mainMenuScene.OnInput(e))
                .On(MainTransition.StartGame, context => context.Set(MainState.StartingGame))
                .On(MainTransition.Settings, context => context.Push(MainState.Settings))
                .Suspend(() => _mainMenuScene.DisableMenus())
                .Awake(() => _mainMenuScene.EnableMenus())
                .Enter(async () => await _mainMenuScene.ShowMenu())
                .Build();

            State(MainState.Settings)
                .OnInput(e => _settingsMenuScene.OnInput(e))
                .On(MainTransition.Back, context => context.Pop())
                .Enter(() => _settingsMenuScene.ShowSettingsMenu())
                .Exit(() => _settingsMenuScene.HideSettingsMenu())
                .Build();

            State(MainState.StartingGame)
                .Enter(async () => {
                    await _mainMenuScene.HideMainMenu();
                    Game.Start();
                })
                .Execute(context => context.Set(MainState.Gaming))
                .Build();

            State(MainState.Gaming)
                .OnInput(e => {
                    if (ControllerStart.IsEventJustPressed(e)) {
                        Enqueue(MainTransition.Pause);
                        GetTree().SetInputAsHandled();
                    }
                })
                .On(MainTransition.Back, context => context.Pop())
                .On(MainTransition.Pause, context => context.Push(MainState.PauseMenu))
                .Exit(() => Game.End())
                .Build();
            
            On(MainTransition.EndGame, ctx => ctx.Set(MainState.MainMenu));

            State(MainState.PauseMenu)
                .OnInput(e => _pauseMenuScene.OnInput(e))
                .On(MainTransition.Back, context => context.Pop())
                .On(MainTransition.Settings, context => context.Push(MainState.Settings))
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

            On(MainTransition.ModalBoxConfirmQuitGame, context => context.Push(MainState.ModalQuitGame));
            State(MainState.ModalQuitGame)
                .On(MainTransition.Back, context => context.Pop())
                .Execute(async (context) => {
                    var result = await ShowModalBox("Quit game?", "Any progress not saved will be lost");
                    return result ? context.Set(MainState.MainMenu) : context.Pop();
                })
                .Build();
                

            On(MainTransition.ModalBoxConfirmExitDesktop, context => context.Push(MainState.ModalExitDesktop));
            State(MainState.ModalExitDesktop)
                .On(MainTransition.Back, context => context.Pop())
                .Enter(() => _mainMenuScene.DimOut())
                .Exit(() => _mainMenuScene.RollbackDimOut())
                .Execute(async (context) => {
                    var result = await ShowModalBox("Exit game?");
                    return result ? context.Set(MainState.ExitDesktop) : context.Pop();
                })
                .Build();
                
            On(MainTransition.ExitDesktop, context => context.Set(MainState.ExitDesktop));
            State(MainState.ExitDesktop)
                .Enter(() => SceneTree.Notification(MainLoop.NotificationWmQuitRequest))
                .Build();
        }

        private void ConfigureDebugOverlays() {
            DefaultDebugOverlay.Solid().Solid().Title("System")
                .AddMonitorFpsAndMemory()
                .AddMonitorInternals()
                .Text(ScreenSettingsManager.GetStateAsString).EndMonitor();

            DebugOverlayManager.OverlayContainer.Theme = MyTheme;
            DebugOverlayManager.DebugConsole.Theme =  MainResourceLoader.DebugConsoleTheme;;
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
    }
}