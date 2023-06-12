using System;
using System.Threading.Tasks;
using Betauer.Application.Lifecycle;
using Betauer.Application.Lifecycle.Pool;
using Betauer.Application.Monitor;
using Betauer.Application.Screen;
using Betauer.Core;
using Betauer.DI;
using Betauer.DI.Attributes;
using Betauer.DI.Factory;
using Betauer.FSM.Async;
using Betauer.Input;
using Betauer.Input.Joypad;
using Betauer.Nodes;
using Godot;
using Veronenger.Game.HUD;
using Veronenger.Game.UI;
using Veronenger.Game.UI.Settings;

namespace Veronenger.Game; 

public enum MainState {
    SplashScreenLoading,
    
    
    SplashScreenInit,
    MainMenu,
    Settings,
    StartingGame,
    Gaming,
    SavingGame,
    GameOver,
    PauseMenu,
    ModalQuitGame,
    QuitGame,
    ModalExitDesktop,
    ExitDesktop,
}
    
public enum MainEvent {
    Back,
    Pause,
    Settings,
    StartGame,
    StartSavingGame,
    EndGame,
    ModalBoxConfirmExitDesktop,
    ModalBoxConfirmQuitGame,
    ExitDesktop
}

[Singleton]
public partial class MainStateMachine : FsmNodeAsync<MainState, MainEvent>, IInjectable {

    [Inject] private ILazy<MainMenu> MainMenuSceneFactory { get; set; }
    [Inject] private ILazy<BottomBar> BottomBarSceneFactory { get; set; }
    [Inject] private ILazy<PauseMenu> PauseMenuSceneFactory { get; set; }
    [Inject] private ILazy<SettingsMenu> SettingsMenuSceneFactory { get; set; }
    [Inject] private ILazy<ProgressScreen> ProgressScreenFactory { get; set; }
    [Inject] private HudCanvas HudCanvas { get; set; }
    [Inject] private GameLoader GameLoader { get; set; }
    [Inject] private PoolContainer<Node> PoolNodeContainer { get; set; }

    private MainMenu MainMenuScene => MainMenuSceneFactory.Get();
    private BottomBar BottomBarScene => BottomBarSceneFactory.Get();
    private PauseMenu PauseMenuScene => PauseMenuSceneFactory.Get();
    private SettingsMenu SettingsMenuScene => SettingsMenuSceneFactory.Get();
    private ProgressScreen ProgressScreenScene => ProgressScreenFactory.Get();
    
    [Inject] private ITransient<ModalBoxConfirm> ModalBoxConfirm { get; set; }
    [Inject("MyTheme")] private ResourceHolder<Theme> MyTheme { get; set; }

    [Inject] private GameViewContainer GameViewContainer { get; set; }

    [Inject] private ScreenSettingsManager ScreenSettingsManager { get; set; }
    [Inject] private SceneTree SceneTree { get; set; }
    [Inject] private DebugOverlayManager DebugOverlayManager { get; set; }

    [Inject] private InputAction UiAccept { get; set; }
    [Inject] private InputAction UiCancel { get; set; }
    [Inject] private InputAction ControllerStart { get; set; }
    [Inject] private UiActionsContainer UiActionsContainer { get; set; }
    [Inject] private JoypadPlayersMapping JoypadPlayersMapping { get; set; }
        
    [Inject("DebugConsoleTheme")] private ResourceHolder<Theme> DebugConsoleTheme { get; set; }

    [Inject] private EventBus EventBus { get; set; }

    public override void _Ready() {
        ProcessMode = ProcessModeEnum.Always;
    }

    public MainStateMachine() : base(MainState.SplashScreenLoading) {
    }

    public void PostInject() {
#if DEBUG
        this.OnInput((e) => {
            if (e.IsKeyPressed(Key.Q)) {
                // _settingsMenuScene.Scale -= new Vector2(0.05f, 0.05f);
                // Engine.TimeScale -= 0.05f;
            }
            if (e.IsKeyPressed(Key.W)) {
                // _settingsMenuScene.Scale = new Vector2(1, 1);
                // Engine.TimeScale = 1;
            }
            if (e.IsKeyPressed(Key.E)) {
                // Engine.TimeScale += 0.05f;
                // _settingsMenuScene.Scale += new Vector2(0.05f, 0.05f);
            }
        }, ProcessModeEnum.Always);
#endif

        var splashScreen = SceneTree.GetMainScene<SplashScreen>();
        splashScreen.Layer = int.MaxValue;
        
        State(MainState.SplashScreenLoading)
            .Enter(async () => {
                splashScreen.Start();
                await GameLoader.LoadMainResources();
                splashScreen.ResourcesLoaded();
            })
            .If(() => true).Set(MainState.SplashScreenInit)
            .Build();
            
        var modalResponse = false;
        var endSplash = false;
        var splashScreenWorking = true;

        On(MainEvent.ModalBoxConfirmExitDesktop).Push(MainState.ModalExitDesktop);
        On(MainEvent.ExitDesktop).Set(MainState.ExitDesktop);
        On(MainEvent.ModalBoxConfirmQuitGame).Push(MainState.ModalQuitGame);
        State(MainState.SplashScreenInit)
            .Enter(() => {
                UiActionsContainer.OnNewUiJoypad += (deviceId) => {
                    // Console.WriteLine("New joypad for the ui " + deviceId);
                };
                JoypadPlayersMapping.OnPlayerMappingConnectionChanged += (playerMapping) => {
                    // TODO: Launch the settings window
                    // Console.WriteLine("OnPlayerMappingConnectionChanged: " + playerMapping);
                };
                UiActionsContainer.Start();
                ConfigureCanvasLayers();
                ConfigureDebugOverlays();
                HudCanvas.Configure();
                ScreenSettingsManager.Setup();
                OnTransition += args => BottomBarScene.UpdateState(args.To);
                GameLoader.OnLoadResourceProgress += (rp) => ProgressScreenScene.Loading(rp.TotalPercent);
                splashScreenWorking = false;
            })
            .OnInput(e => {
                if (splashScreenWorking) return;
                if (!endSplash && (e.IsAnyKey() || e.IsAnyButton() || e.IsAnyClick()) && e.IsJustPressed()) {
                    if (e is InputEventJoypadButton button) {
                        UiActionsContainer.SetJoypad(button.Device);
                    }
                    splashScreen.QueueFree();
                    endSplash = true;
                }
            })
            .If(() => endSplash).Set(MainState.MainMenu)
            .Build();
            
        State(MainState.MainMenu)
            .OnInput(e => MainMenuScene.OnInput(e))
            .On(MainEvent.StartGame).Set(MainState.StartingGame)
            .On(MainEvent.Settings).Push(MainState.Settings)
            .Suspend(() => MainMenuScene.DisableMenus())
            .Awake(() => MainMenuScene.EnableMenus())
            .Enter(async () => await MainMenuScene.ShowMenu())
            .Build();

        State(MainState.Settings)
            .OnInput(e => SettingsMenuScene.OnInput(e))
            .On(MainEvent.Back).Pop()
            .Enter(() => SettingsMenuScene.ShowSettingsMenu())
            .Exit(() => SettingsMenuScene.HideSettingsMenu())
            .Build();

        State(MainState.StartingGame).Enter((Func<Task>)(async () => {
                await MainMenuScene.HideMainMenu();
                GameViewContainer.CreateGame();
                SceneTree.Root.AddChild(GameViewContainer.CurrentGame);
                await GameViewContainer.CurrentGame.StartNewGame();
            }))
            .If(() => true).Set(MainState.Gaming)
            .Build();

        State(MainState.Gaming)
            .On(MainEvent.EndGame).Set(MainState.GameOver)
            .On(MainEvent.StartSavingGame).Push(MainState.SavingGame)
            .OnInput(e => {
                if (ControllerStart.IsEventJustPressed(e)) {
                    Send(MainEvent.Pause);
                    GetViewport().SetInputAsHandled();
                }
            })
            .On(MainEvent.Back).Pop()
            .On(MainEvent.Pause).Push(MainState.PauseMenu)
            .Build();

        State(MainState.SavingGame)
            .Enter(() => {
                SceneTree.Paused = true;
                ProgressScreenFactory.Get().ShowSaving();
            })
            .Exit(() => {
                ProgressScreenFactory.Get().Visible = false;
                SceneTree.Paused = false;
            })
            .On(MainEvent.Back).Pop()
            .Build();

        State(MainState.GameOver).Enter((Func<Task>)(async () => {
                await GameViewContainer.CurrentGame!.End(true);
                GameViewContainer.RemoveGame();
            }))
            .If(() => true).Set(MainState.MainMenu)
            .Build();
            
        State(MainState.PauseMenu)
            .OnInput((e) => PauseMenuScene.OnInput(e))
            .On(MainEvent.Back).Pop()
            .On(MainEvent.Settings).Push(MainState.Settings)
            .Suspend(() => PauseMenuScene.DisableMenus())
            .Awake(() => PauseMenuScene.EnableMenus())
            .Enter(async () => {
                SceneTree.Paused = true;
                await PauseMenuScene.ShowPauseMenu();
            })
            .Exit(() => {
                PauseMenuScene.EnableMenus();
                PauseMenuScene.HidePauseMenu();
                SceneTree.Paused = false;
            })
            .Build();

        State(MainState.ModalQuitGame)
            .On(MainEvent.Back).Pop()
            .Execute(async () => {
                var modalBoxConfirm = ShowModalBox("Quit game?", "Any progress not saved will be lost");
                modalResponse = await modalBoxConfirm.AwaitResult();
            })
            .If(() => modalResponse).Set(MainState.QuitGame)
            .If(() => !modalResponse).Pop()
            .Build();
                
        State(MainState.QuitGame).Enter((Func<Task>)(async () => {
                await GameViewContainer.CurrentGame!.End(true);
                GameViewContainer.RemoveGame();
            }))
            .If(() => true).Set(MainState.MainMenu)
            .Build();
                

        State(MainState.ModalExitDesktop)
            .On(MainEvent.Back).Pop()
            .Execute(async () => {
                var modalBoxConfirm = ShowModalBox("Exit game?");
                modalBoxConfirm.FadeBackgroundOut(1, 0.5f);
                modalResponse = await modalBoxConfirm.AwaitResult();
            })
            .If(() => modalResponse).Set(MainState.ExitDesktop)
            .If(() => !modalResponse).Pop()
            .Build();
                
                
        State(MainState.ExitDesktop)
            .Enter(() => SceneTree.QuitSafely())
            .Build();
    }

    private void ConfigureCanvasLayers() {
        MainMenuScene.Layer = CanvasLayerConstants.MainMenu;
        PauseMenuScene.Layer = CanvasLayerConstants.PauseMenu;
        BottomBarScene.Layer = CanvasLayerConstants.BottomBar;
        HudCanvas.Layer = CanvasLayerConstants.HudScene;
        SettingsMenuScene.Layer = CanvasLayerConstants.SettingsMenu;
        ProgressScreenScene.Layer = CanvasLayerConstants.ProgressScreen;

        OnlyInPause(PauseMenuScene);
        NeverPause(SettingsMenuScene, BottomBarScene, ProgressScreenScene);
    }

    private void NeverPause(params Node[] nodes) => nodes.ForEach(n=> n.ProcessMode = ProcessModeEnum.Always);
    private void OnlyInPause(params Node[] nodes) => nodes.ForEach(n=> n.ProcessMode = ProcessModeEnum.WhenPaused);

    private void ConfigureDebugOverlays() {
        DebugOverlayManager.OverlayContainer.Theme = MyTheme.Get();
        DebugOverlayManager.DebugConsole.Theme = DebugConsoleTheme.Get();
        DebugOverlayManager.Overlay("Pool")
            .Text("Busy", () => PoolNodeContainer.BusyCount() + "").EndMonitor()
            .Text("Available", () => PoolNodeContainer.AvailableCount() + "").EndMonitor()
            .Text("Invalid", () => PoolNodeContainer.InvalidCount() + "").EndMonitor();

    }

    private ModalBoxConfirm ShowModalBox(string title, string subtitle = null) {
        var modalBoxConfirm = ModalBoxConfirm.Create();
        modalBoxConfirm.Layer = CanvasLayerConstants.ModalBox;
        modalBoxConfirm.Title(title, subtitle);
        modalBoxConfirm.ProcessMode = ProcessModeEnum.Always;
        SceneTree.Root.AddChild(modalBoxConfirm);
        modalBoxConfirm.AwaitResult().ContinueWith(task => modalBoxConfirm.QueueFree());
        return modalBoxConfirm;
    }
}