using System;
using System.Reflection;
using System.Threading.Tasks;
using Betauer.Application.Monitor;
using Godot;
using Betauer.Application.Screen;
using Betauer.DI;
using Betauer.Input;
using Betauer.Loader;
using Betauer.Core.Nodes;
using Betauer.StateMachine.Async;
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
    
    [Service]
    public class MainStateMachine : StateMachineNodeAsync<MainState, MainEvent> {

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
            }, PauseModeEnum.Process);
            #endif
            
            Bus.Subscribe(Enqueue).RemoveIfInvalid(this);
            var modalResponse = false;
            
            State(MainState.Init)
                .Execute(async () => {
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
                })
                .If(() => true).Set(MainState.StartingGame)
                .Build();
            
            State(MainState.MainMenu)
                .OnInput(e => _mainMenuScene.OnInput(e))
                .On(MainEvent.StartGame).Then(context=> context.Set(MainState.StartingGame))
                .On(MainEvent.Settings).Then(context=> context.Push(MainState.Settings))
                .Suspend(() => _mainMenuScene.DisableMenus())
                .Awake(() => _mainMenuScene.EnableMenus())
                .Enter(async () => await _mainMenuScene.ShowMenu())
                .Build();

            State(MainState.Settings)
                .OnInput(e => _settingsMenuScene.OnInput(e))
                .On(MainEvent.Back).Then(context => context.Pop())
                .Enter(() => _settingsMenuScene.ShowSettingsMenu())
                .Exit(() => _settingsMenuScene.HideSettingsMenu())
                .Build();

            State(MainState.StartingGame)
                .Enter(async () => {
                    await _mainMenuScene.HideMainMenu();
                    Game.Start();
                })
                .If(() => true).Set(MainState.Gaming)
                .Build();

            State(MainState.Gaming)
                .OnInput(e => {
                    if (ControllerStart.IsEventJustPressed(e)) {
                        Enqueue(MainEvent.Pause);
                        GetTree().SetInputAsHandled();
                    }
                })
                .On(MainEvent.Back).Then(context=> context.Pop())
                .On(MainEvent.Pause).Then(context=> context.Push(MainState.PauseMenu))
                .Exit(() => Game.End())
                .Build();
            
            On(MainEvent.EndGame).Then(ctx => ctx.Set(MainState.MainMenu));

            State(MainState.PauseMenu)
                .OnInput(e => _pauseMenuScene.OnInput(e))
                .On(MainEvent.Back).Then(context=> context.Pop())
                .On(MainEvent.Settings).Then(context=> context.Push(MainState.Settings))
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

            On(MainEvent.ModalBoxConfirmQuitGame).Then(context=> context.Push(MainState.ModalQuitGame));
            State(MainState.ModalQuitGame)
                .On(MainEvent.Back).Then(context=> context.Pop())
                .Execute(async () => {
                    modalResponse = await ShowModalBox("Quit game?", "Any progress not saved will be lost");
                })
                .If(() => modalResponse).Set(MainState.MainMenu)
                .If(() => !modalResponse).Pop()
                .Build();
                

            On(MainEvent.ModalBoxConfirmExitDesktop).Then(context=> context.Push(MainState.ModalExitDesktop));
            State(MainState.ModalExitDesktop)
                .On(MainEvent.Back).Then(context=> context.Pop())
                .Enter(() => _mainMenuScene.DimOut())
                .Exit(() => _mainMenuScene.RollbackDimOut())
                .Execute(async () => {
                    modalResponse = await ShowModalBox("Exit game?");
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