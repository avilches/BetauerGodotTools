using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Godot;
using Betauer;
using Betauer.Animation;
using Betauer.DI;
using Betauer.Screen;
using Veronenger.Game.Controller;
using Veronenger.Game.Controller.Menu;

namespace Veronenger.Game.Managers {
    /**
     * GameManager es Node para estar en autoload y recibir eventos
     * Crea automaticamente a los otros managers (Manager = siempre cargado)
     * Los Controller son scripts de objetos de le escena que se cargan y se registran en los managers
     *
     * Los Manager actuan de intermediarios entre objetos que no se conocen entre si. Por ejemplo: las death zones,
     * plataformas o stages se añaden en sus managers, que escucha a las señales que estos objetos producen.
     * Por otro lado, el jugador se registra en estos mismos manager para escuchar estas señales, sin llegar a saber
     * donde estan realmente estos objetos (plataformas o areas).
     *
     */
    [Singleton]
    public class GameManager {
        private static readonly Logger Logger = LoggerFactory.GetLogger(typeof(GameManager));

        private MainMenu _mainMenuScene;
        private PauseMenu _pauseMenuScene;
        private OptionsMenu _optionsMenuScene;
        private Node _currentGameScene;
        private Node2D _playerScene;

        public bool IsGamePaused() => CurrentState == State.PauseMenu;
        public bool IsGaming() => CurrentState == State.Gaming;
        public bool IsModal() => CurrentState == State.Modal;
        public bool IsMainMenu() => CurrentState == State.MainMenu;
        public bool IsOptions() => CurrentState == State.Options;

        public readonly Launcher Launcher = new Launcher();

        [Inject] private StageManager _stageManager;
        [Inject] private ScreenManager _screenManager;
        [Inject] private Func<SceneTree> GetTree;
        [Inject] private ResourceManager _resourceManager;

        public enum State {
            Loading,
            MainMenu,
            Gaming,
            PauseMenu,
            Options,
            Modal
        }

        private readonly Stack<State> _states = new Stack<State>();
        public State CurrentState => _states.Peek();

        public void OnFinishLoad(SplashScreenController splashScreen) {
            _screenManager.Start(ApplicationConfig.Configuration);
            _states.Push(State.Loading);
            splashScreen.QueueFree();
            Launcher.WithParent(GetTree().Root);
            _mainMenuScene = _resourceManager.CreateMainMenu();
            _pauseMenuScene = _resourceManager.CreatePauseMenu();
            _optionsMenuScene = _resourceManager.CreateOptionsMenu();
            
            // Never pause the pause menu and the options menu!
            _optionsMenuScene.PauseMode = _pauseMenuScene.PauseMode = Node.PauseModeEnum.Process;
            GetTree().Root.AddChild(_pauseMenuScene);
            GetTree().Root.AddChild(_optionsMenuScene);

            GetTree().Root.AddChild(_mainMenuScene); // Main menu shows itself in Ready
            _states.Push(State.MainMenu);
        }

        public async void StartGame() {
            await _mainMenuScene.HideMainMenu();
            _currentGameScene = _resourceManager.CreateWorld1();
            await AddSceneDeferred(_currentGameScene);
            AddPlayerToScene(_currentGameScene);
            _states.Push(State.Gaming);
        }

        public async void ShowPauseMenu() {
            GetTree().Paused = true;
            await _pauseMenuScene.ShowPauseMenu();
            _states.Push(State.PauseMenu);
        }

        public async void ShowOptionsMenu() {
            await _optionsMenuScene.ShowOptionsMenu();
            _states.Push(State.Options);
        }

        public void CloseOptionsMenu() {
            _optionsMenuScene.HideOptionsMenu();
            _states.Pop();
            if (IsMainMenu()) {
                _mainMenuScene.FocusOptions();
            } else if (IsGamePaused()) {
                _pauseMenuScene.FocusOptions();
            }
        }

        public async Task<bool> ModalBoxConfirmExitDesktop() {
            return await ShowModalBox("Exit game?");
        }

        public async Task<bool> ModalBoxConfirmQuitGame() {
            return await ShowModalBox("Quit game?", "Any progress not saved will be lost");
        }

        private async Task<bool> ShowModalBox(string title, string subtitle = null) {
            ModalBoxConfirm modalBoxConfirm = _resourceManager.CreateModalBoxConfirm();
            modalBoxConfirm.Title(title, subtitle);
            modalBoxConfirm.PauseMode = Node.PauseModeEnum.Process;
            GetTree().Root.AddChild(modalBoxConfirm);
            _states.Push(State.Modal);
            var result = await modalBoxConfirm.AwaitResult();
            _states.Pop();
            modalBoxConfirm.QueueFree();
            return result;
        }

        public void ClosePauseMenu() {
            _pauseMenuScene.HidePauseMenu();
            GetTree().Paused = false;
            _states.Pop();
        }

        public async Task ExitGameAndBackToMainMenu() {
            _currentGameScene.PrintStrayNodes();
            _currentGameScene.QueueFree();
            _currentGameScene = null;
            _states.Pop();
            await _mainMenuScene.ShowMenu();
        }

        public async void QueueChangeSceneWithPlayer(string sceneName) {
            _states.Push(State.Loading);
            _stageManager.ClearTransition();
            _currentGameScene.QueueFree();

            var nextScene = ResourceLoader.Load<PackedScene>(sceneName).Instance();
            await AddSceneDeferred(nextScene);
            AddPlayerToScene(nextScene);
            _currentGameScene = nextScene;
            _states.Pop();
        }

        private void AddPlayerToScene(Node nextScene) {
            _playerScene = _resourceManager.CreatePlayer();
            nextScene.AddChild(_playerScene);
            var position2D = nextScene.FindChild<Node2D>("PositionPlayer");
            if (position2D == null) {
                throw new Exception("Node PositionPlayer not found when loading scene " + nextScene.Filename);
            }
            _playerScene.GlobalPosition = position2D.GlobalPosition;
        }

        private async Task AddSceneDeferred(Node scene) {
            await GetTree().AwaitIdleFrame();
            GetTree().Root.AddChild(scene);
        }

        public async Task LoadAnimaDemo() {
            var nextScene = ResourceLoader.Load<PackedScene>("demos/AnimationsPreview.tscn").Instance();
            _currentGameScene = nextScene;
            await AddSceneDeferred(_currentGameScene);

            _screenManager.ChangeScreenConfiguration(ApplicationConfig.AnimaDemoConfiguration,
                ScreenService.Strategy.FitToScreen);
            _screenManager.CenterWindow();
        }
    }
}