using Betauer.Application.Lifecycle;
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
using Veronenger.Game.Platform.World;
using Veronenger.Game.RTS.World;
using Veronenger.Game.UI;
using Veronenger.Game.UI.Settings;

namespace Veronenger.Game; 

public enum MainState {
    Init,
    
    
    SplashScreen,
    MainMenu,
    Settings,
    StartingGamePlatform,
    StartingGameRts,
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
    StartGameRts,
    StartGamePlatform,
    StartSavingGame,
    EndGame,
    ModalBoxConfirmExitDesktop,
    ModalBoxConfirmQuitGame,
    ExitDesktop
}

public interface IMain {
    public void Send(MainEvent e, int weight = 0);
}

[Singleton<IMain>(Flags = "Autoload")]
public partial class Main : FsmNodeAsync<MainState, MainEvent>, IMain, IInjectable {

    [Inject] private ILazy<MainMenu> MainMenuSceneLazy { get; set; }
    [Inject] private ILazy<BottomBar> BottomBarLazy { get; set; }
    [Inject] private ILazy<PauseMenu> PauseMenuLazy { get; set; }
    [Inject] private ILazy<SettingsMenu> SettingsMenuLazy { get; set; }
    [Inject] private ILazy<ProgressScreen> ProgressScreenLazy { get; set; }
    [Inject] private GameLoader GameLoader { get; set; }

    private MainMenu MainMenuScene => MainMenuSceneLazy.Get();
    private BottomBar BottomBarScene => BottomBarLazy.Get();
    private PauseMenu PauseMenuScene => PauseMenuLazy.Get();
    private SettingsMenu SettingsMenuScene => SettingsMenuLazy.Get();
    private ProgressScreen ProgressScreenScene => ProgressScreenLazy.Get();
    
    [Inject] private ITransient<ModalBoxConfirm> ModalBoxConfirmFactory { get; set; }
    [Inject("DebugOverlayTheme")] private ResourceHolder<Theme> DebugOverlayTheme { get; set; }
    [Inject("DebugConsoleTheme")] private ResourceHolder<Theme> DebugConsoleTheme { get; set; }

    [Inject] private Betauer.DI.Container Container { get; set; }
    [Inject] private ITransient<PlatformGameView> PlatformGameView { get; set; }
    [Inject] private ITransient<RtsGameView> RtsGameView { get; set; }

    [Inject] private ScreenSettingsManager ScreenSettingsManager { get; set; }
    [Inject] private SceneTree SceneTree { get; set; }
    [Inject] private DebugOverlayManager DebugOverlayManager { get; set; }

    [Inject] private InputAction UiAccept { get; set; }
    [Inject] private InputAction UiCancel { get; set; }
    [Inject] private InputAction ControllerStart { get; set; }
    [Inject] private UiActionsContainer UiActionsContainer { get; set; }
    [Inject] private JoypadPlayersMapping JoypadPlayersMapping { get; set; }
        

    public override void _Ready() {
        ProcessMode = ProcessModeEnum.Always;
    }

    public Main() : base(MainState.Init) {
    }

    public void PostInject() {
#if DEBUG
        NodeManager.MainInstance.OnInput += e => {
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
        };
#endif

        var modalResponse = false;

        On(MainEvent.ModalBoxConfirmExitDesktop).Push(MainState.ModalExitDesktop);
        On(MainEvent.ExitDesktop).Set(MainState.ExitDesktop);
        On(MainEvent.ModalBoxConfirmQuitGame).Push(MainState.ModalQuitGame);
        
        State(MainState.Init)
            .Enter(async () => {
                var splashScreen = SceneTree.GetMainScene<SplashScreen>();
                splashScreen.StartLoadingAnimation();
                await GameLoader.LoadMainResources();
                splashScreen.StopLoadingAnimation();
                ConfigureApp();
            })
            .If(() => true).Set(MainState.SplashScreen)
            .Build();

        var endSplash = false;
        State(MainState.SplashScreen)
            .OnInput(e => {
                if ((e.IsAnyKey() || e.IsAnyButton() || e.IsAnyClick()) && e.IsJustPressed()) {
                    if (e is InputEventJoypadButton button) {
                        UiActionsContainer.SetJoypad(button.Device);
                    }
                    var splashScreen = SceneTree.GetMainScene<SplashScreen>();
                    splashScreen.QueueFree();
                    endSplash = true;
                }
            })
            .If(() => endSplash).Set(MainState.MainMenu)
            .Build();

        State(MainState.MainMenu)
            .OnInput(e => MainMenuScene.OnInput(e))
            .On(MainEvent.StartGamePlatform).Set(MainState.StartingGamePlatform)
            .On(MainEvent.StartGameRts).Set(MainState.StartingGameRts)
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

        IGameView gameView = null!;
        State(MainState.StartingGamePlatform).Enter(async () => {
                await MainMenuScene.HideMainMenu();
                gameView = PlatformGameView.Create();
                await gameView.StartNewGame();
            })
            .If(() => true).Set(MainState.Gaming)
            .Build();

        State(MainState.StartingGameRts).Enter(async () => {
                await MainMenuScene.HideMainMenu();
                gameView = RtsGameView.Create();
                await gameView.StartNewGame();
            })
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
                ProgressScreenLazy.Get().ShowSaving();
            })
            .Exit(() => {
                ProgressScreenLazy.Get().Visible = false;
                SceneTree.Paused = false;
            })
            .On(MainEvent.Back).Pop()
            .Build();

        State(MainState.GameOver).Enter(async () => {
                await gameView.End(true);
            })
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
                
        State(MainState.QuitGame).Enter(async () => {
                await gameView.End(true);
            })
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

    private void ConfigureApp() {
        UiActionsContainer.OnNewUiJoypad += (deviceId) => {
            // Console.WriteLine("New joypad for the ui " + deviceId);
        };
        JoypadPlayersMapping.OnPlayerMappingConnectionChanged += (playerMapping) => {
            // TODO: Launch the settings window
            // Console.WriteLine("OnPlayerMappingConnectionChanged: " + playerMapping);
        };
        UiActionsContainer.SetFirstJoypadConnected();
        ConfigureCanvasLayers();
        ConfigureDebugOverlays();
        ScreenSettingsManager.Setup();
        OnTransition += args => BottomBarScene.UpdateState(args.To);
        GameLoader.OnLoadResourceProgress += (rp) => ProgressScreenScene.Loading(rp.TotalPercent);
    }

    private void ConfigureDebugOverlays() {
        DebugOverlayManager.OverlayContainer.Theme = DebugOverlayTheme.Get();
        DebugOverlayManager.DebugConsole.Theme = DebugConsoleTheme.Get();
    }

    private void ConfigureCanvasLayers() {
        MainMenuScene.Layer = CanvasLayerConstants.MainMenu;
        PauseMenuScene.Layer = CanvasLayerConstants.PauseMenu;
        BottomBarScene.Layer = CanvasLayerConstants.BottomBar;
        SettingsMenuScene.Layer = CanvasLayerConstants.SettingsMenu;
        ProgressScreenScene.Layer = CanvasLayerConstants.ProgressScreen;
        OnlyInPause(PauseMenuScene);
        NeverPause(SettingsMenuScene, BottomBarScene, ProgressScreenScene);
    }

    private void NeverPause(params Node[] nodes) => nodes.ForEach(n=> n.ProcessMode = ProcessModeEnum.Always);
    private void OnlyInPause(params Node[] nodes) => nodes.ForEach(n=> n.ProcessMode = ProcessModeEnum.WhenPaused);

    private ModalBoxConfirm ShowModalBox(string title, string subtitle = null) {
        var modalBoxConfirm = ModalBoxConfirmFactory.Create();
        modalBoxConfirm.Layer = CanvasLayerConstants.ModalBox;
        modalBoxConfirm.Title(title, subtitle);
        modalBoxConfirm.ProcessMode = ProcessModeEnum.Always;
        SceneTree.Root.AddChild(modalBoxConfirm);
        modalBoxConfirm.AwaitResult().ContinueWith(task => modalBoxConfirm.QueueFree());
        return modalBoxConfirm;
    }
}