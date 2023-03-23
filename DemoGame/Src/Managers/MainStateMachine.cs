using System;
using Betauer.Application.Monitor;
using Godot;
using Betauer.Application.Screen;
using Betauer.Core;
using Betauer.DI;
using Betauer.Input;
using Betauer.Core.Nodes;
using Betauer.DI.Factory;
using Betauer.Nodes;
using Betauer.FSM.Async;
using Veronenger.Config;
using Veronenger.UI;

namespace Veronenger.Managers; 

public enum MainState {
    SplashScreenLoading,
    
    
    Init,
    MainMenu,
    Settings,
    StartingGame,
    Gaming,
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
    EndGame,
    ModalBoxConfirmExitDesktop,
    ModalBoxConfirmQuitGame,
    ExitDesktop
}

[Singleton]
public partial class MainStateMachine : FsmNodeAsync<MainState, MainEvent>, IInjectable {

    [Inject] private IFactory<MainMenu> MainMenuSceneFactory { get; set; }
    [Inject] private IFactory<BottomBar> BottomBarSceneFactory { get; set; }
    [Inject] private IFactory<PauseMenu> PauseMenuSceneFactory { get; set; }
    [Inject] private IFactory<SettingsMenu> SettingsMenuSceneFactory { get; set; }
    [Inject] private IFactory<HUD> HudSceneFactory { get; set; }
    [Inject] private GameLoaderContainer GameLoaderContainer { get; set; }

    private MainMenu MainMenuScene => MainMenuSceneFactory.Get();
    private BottomBar BottomBarScene => BottomBarSceneFactory.Get();
    private PauseMenu PauseMenuScene => PauseMenuSceneFactory.Get();
    private SettingsMenu SettingsMenuScene => SettingsMenuSceneFactory.Get();
    private HUD HudScene => HudSceneFactory.Get();
    
    [Inject] private IFactory<ModalBoxConfirm> ModalBoxConfirm { get; set; }
    [Inject] private Theme MyTheme { get; set; }
    [Inject] private Game Game { get; set; }

    [Inject] private ScreenSettingsManager ScreenSettingsManager { get; set; }
    [Inject] private SceneTree SceneTree { get; set; }
    [Inject] private DebugOverlayManager DebugOverlayManager { get; set; }

    [Inject] private InputAction UiAccept { get; set; }
    [Inject] private InputAction UiCancel { get; set; }
    [Inject] private InputAction ControllerStart { get; set; }
        
    [Inject] private Theme DebugConsoleTheme { get; set; }

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

        EventBus.Subscribe(evt => Send(evt)).UnsubscribeIf(Predicates.IsInvalid(this));
        
        var splashScreen = SceneTree.GetMainScene<SplashScreenNode>();
        splashScreen.Layer = int.MaxValue;
        
        State(MainState.SplashScreenLoading)
            .Enter(async () => {
                // GameLoaderContainer.OnLoadResourceProgress += (rp) => Console.WriteLine($"{rp.ResourcePercent:P} {rp.Resource}"); 
                Console.WriteLine($"Main load:{(await GameLoaderContainer.LoadMainResources()).TotalMilliseconds}ms");
                splashScreen.Stop();
            })
            .If(() => true).Set(MainState.Init)
            .Build();
            
        var modalResponse = false;
        var endSplash = false;

        On(MainEvent.ModalBoxConfirmExitDesktop).Push(MainState.ModalExitDesktop);
        On(MainEvent.ExitDesktop).Set(MainState.ExitDesktop);
        On(MainEvent.ModalBoxConfirmQuitGame).Push(MainState.ModalQuitGame);
        State(MainState.Init)
            .Enter(() => {
                ConfigureCanvasLayers();
                ConfigureDebugOverlays();
                ScreenSettingsManager.Setup();
                OnTransition += args => BottomBarScene.UpdateState(args.To);
                GameLoaderContainer.OnLoadResourceProgress += BottomBarScene.OnLoadResourceProgress;
            })
            .OnInput(e => {
                if (!endSplash && (e.IsAnyKey() || e.IsAnyButton() || e.IsAnyClick()) && e.IsJustPressed()) {
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

        State(MainState.StartingGame)
            .Enter(async () => {
                await MainMenuScene.HideMainMenu();
                await Game.Start();
            })
            .If(() => true).Set(MainState.Gaming)
            .Build();

        State(MainState.Gaming)
            .On(MainEvent.EndGame).Set(MainState.GameOver)
            .OnInput(e => {
                if (ControllerStart.IsEventJustPressed(e)) {
                    Send(MainEvent.Pause);
                    GetViewport().SetInputAsHandled();
                }
            })
            .On(MainEvent.Back).Pop()
            .On(MainEvent.Pause).Push(MainState.PauseMenu)
            .Build();

        State(MainState.GameOver)
            .Enter(async () => await Game.End())
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
                
        State(MainState.QuitGame)
            .Enter(() => Game.End())
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
        SettingsMenuScene.Layer = CanvasLayerConstants.SettingsMenu;
        BottomBarScene.Layer = CanvasLayerConstants.BottomBar;
        HudScene.Layer = CanvasLayerConstants.HudScene;
        HudScene.Visible = false;

        OnlyInPause(PauseMenuScene);
        NeverPause(SettingsMenuScene, BottomBarScene);
    }

    private void NeverPause(params Node[] nodes) => nodes.ForEach(n=> n.ProcessMode = ProcessModeEnum.Always);
    private void OnlyInPause(params Node[] nodes) => nodes.ForEach(n=> n.ProcessMode = ProcessModeEnum.WhenPaused);

    private void ConfigureDebugOverlays() {
        DebugOverlayManager.OverlayContainer.Theme = MyTheme;
        DebugOverlayManager.DebugConsole.Theme =  DebugConsoleTheme;;
    }

    private ModalBoxConfirm ShowModalBox(string title, string subtitle = null) {
        var modalBoxConfirm = ModalBoxConfirm.Get();
        modalBoxConfirm.Layer = CanvasLayerConstants.ModalBox;
        modalBoxConfirm.Title(title, subtitle);
        modalBoxConfirm.ProcessMode = ProcessModeEnum.Always;
        SceneTree.Root.AddChild(modalBoxConfirm);
        modalBoxConfirm.AwaitResult().ContinueWith(task => modalBoxConfirm.QueueFree());
        return modalBoxConfirm;
    }
}