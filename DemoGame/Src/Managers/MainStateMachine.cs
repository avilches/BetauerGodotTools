using Betauer.Application.Monitor;
using Godot;
using Betauer.Application.Screen;
using Betauer.DI;
using Betauer.Input;
using Betauer.Core.Nodes;
using Betauer.Loader;
using Betauer.Nodes;
using Betauer.StateMachine.Async;
using Veronenger.Controller;
using Veronenger.Controller.Menu;

namespace Veronenger.Managers {
    
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
    
    public class MainResourceLoader : ResourceLoaderContainer {
        [Load("res://Assets/D1.wav")] public Resource D1;
        [Load("res://Assets/D2.wav")] public Resource D2;
        [Load("res://Assets/D3.wav")] public Resource D3;
        [Load("res://Assets/D4.wav")] public Resource D4;
        [Load("res://Assets/D5.wav")] public Resource D5;
        [Load("res://Assets/D6.wav")] public Resource D6;
        [Load("res://Assets/D7.wav")] public Resource D7;
        [Load("res://Assets/sample1.wav")] public Resource sample1;
        [Load("res://Assets/sample2.wav")] public Resource sample2;
        [Load("res://Assets/sample3.wav")] public Resource sample3;
        [Load("res://Assets/sample4.wav")] public Resource sample4;
        [Load("res://Assets/sample5.wav")] public Resource sample5;
        [Load("res://Assets/sample6.wav")] public Resource sample6;
        [Load("res://Assets/sample7.wav")] public Resource sample7;
        public override void DoOnProgress(float progress) {
            GD.Print($"{progress * 100}%");
        }

    }
    
    
    [Service]
    public partial class MainStateMachine : StateMachineNodeAsync<MainState, MainEvent> {

        [Inject] private MainMenu MainMenuScene { get; set; }
        [Inject] public BottomBar BottomBarScene { get; set; }
        [Inject] private PauseMenu PauseMenuScene { get; set; }
        [Inject] private SettingsMenu SettingsMenuScene { get; set; }
        [Inject] private Factory<ModalBoxConfirm> ModalBoxConfirm { get; set; }
        [Inject] private Theme MyTheme { get; set; }
        [Inject] private Game Game { get; set; }

        [Inject] private ScreenSettingsManager ScreenSettingsManager { get; set; }
        [Inject] private SceneTree SceneTree { get; set; }
        [Inject] private DebugOverlayManager DebugOverlayManager { get; set; }

        [Inject] private InputAction UiAccept { get; set; }
        [Inject] private InputAction UiCancel { get; set; }
        [Inject] private InputAction ControllerStart { get; set; }
        
        [Inject] private Theme DebugConsoleTheme { get; set; }

        [Inject] private Bus Bus { get; set; }

        public override void _Ready() {
            ProcessMode = ProcessModeEnum.Always;
        }

        public MainStateMachine() : base(MainState.Init) {
        }

        [PostInject]
        private void Configure() {
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
            
            Bus.Subscribe(Enqueue).RemoveIfInvalid(this);
            var modalResponse = false;
            var splashScreen = SceneTree.GetMainScene<SplashScreenNode>();
            splashScreen.Layer = int.MaxValue;

            var endSplash = false;
            State(MainState.Init)
                .Enter(() => {
                    MainMenuScene.Layer = CanvasLayerConstants.MainMenu;
                    PauseMenuScene.Layer = CanvasLayerConstants.PauseMenu;
                    SettingsMenuScene.Layer = CanvasLayerConstants.SettingsMenu;
                    BottomBarScene.Layer = CanvasLayerConstants.BottomBar;
                    // Never pause the pause, settings and the state machine, because they will not work!
                    SettingsMenuScene.ProcessMode = ProcessModeEnum.Always;
                    PauseMenuScene.ProcessMode = ProcessModeEnum.WhenPaused;

                    ScreenSettingsManager.Setup();
                    ConfigureDebugOverlays();

                    AddOnTransition((args) => BottomBarScene.UpdateState(args.To));
                })
                .Execute(async () => {
                    var test = new MainResourceLoader();
                    GD.Print("Loading start....");
                    await test.Load();
                    GD.Print("Loading end");
                    endSplash = true;
                    splashScreen.QueueFree();
                })
                .OnInput((e) => {
                    if ((e.IsAnyKey() || e.IsAnyButton() || e.IsAnyClick()) && e.IsJustPressed()) {
                        splashScreen.QueueFree();
                        endSplash = true;
                    }
                })
                .If(() => endSplash).Set(MainState.MainMenu)
                .Build();
            
            State(MainState.MainMenu)
                .OnInput(e => MainMenuScene.OnInput(e))
                .On(MainEvent.StartGame).Then(context=> context.Set(MainState.StartingGame))
                .On(MainEvent.Settings).Then(context=> context.Push(MainState.Settings))
                .Suspend(() => MainMenuScene.DisableMenus())
                .Awake(() => MainMenuScene.EnableMenus())
                .Enter(async () => await MainMenuScene.ShowMenu())
                .Build();

            State(MainState.Settings)
                .OnInput(e => SettingsMenuScene.OnInput(e))
                .On(MainEvent.Back).Then(context => context.Pop())
                .Enter(() => SettingsMenuScene.ShowSettingsMenu())
                .Exit(() => SettingsMenuScene.HideSettingsMenu())
                .Build();

            State(MainState.StartingGame)
                .Enter(async () => {
                    await MainMenuScene.HideMainMenu();
                    Game.Start();
                })
                .If(() => true).Set(MainState.Gaming)
                .Build();

            State(MainState.Gaming)
                .OnInput(e => {
                    if (ControllerStart.IsEventJustPressed(e)) {
                        Enqueue(MainEvent.Pause);
                        GetViewport().SetInputAsHandled();
                    }
                })
                .On(MainEvent.Back).Then(context=> context.Pop())
                .On(MainEvent.Pause).Then(context=> context.Push(MainState.PauseMenu))
                .Exit(() => Game.End())
                .Build();
            
            On(MainEvent.EndGame).Then(ctx => ctx.Set(MainState.MainMenu));

            State(MainState.PauseMenu)
                .OnInput(e => PauseMenuScene.OnInput(e))
                .On(MainEvent.Back).Then(context=> context.Pop())
                .On(MainEvent.Settings).Then(context=> context.Push(MainState.Settings))
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

            On(MainEvent.ModalBoxConfirmQuitGame).Then(context=> context.Push(MainState.ModalQuitGame));
            State(MainState.ModalQuitGame)
                .On(MainEvent.Back).Then(context=> context.Pop())
                .Execute(async () => {
                    var modalBoxConfirm = ShowModalBox("Quit game?", "Any progress not saved will be lost");
                    modalResponse = await modalBoxConfirm.AwaitResult();
                })
                .If(() => modalResponse).Set(MainState.MainMenu)
                .If(() => !modalResponse).Pop()
                .Build();
                

            On(MainEvent.ModalBoxConfirmExitDesktop).Then(context=> context.Push(MainState.ModalExitDesktop));
            State(MainState.ModalExitDesktop)
                .On(MainEvent.Back).Then(context=> context.Pop())
                .Execute(async () => {
                    var modalBoxConfirm = ShowModalBox("Exit game?");
                    modalBoxConfirm.FadeBackgroundOut(1, 0.5f);
                    modalResponse = await modalBoxConfirm.AwaitResult();
                })
                .If(() => modalResponse).Set(MainState.ExitDesktop)
                .If(() => !modalResponse).Pop()
                .Build();
                
                
            On(MainEvent.ExitDesktop).Then(context=> context.Set(MainState.ExitDesktop));
            State(MainState.ExitDesktop)
                .Enter(() => SceneTree.QuitSafely())
                .Build();
        }

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
}