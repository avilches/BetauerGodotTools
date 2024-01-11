using Betauer.Application.Screen;
using Betauer.Application.Screen.Resolution;
using Betauer.Application.Settings;
using Betauer.DI.Attributes;
using Betauer.Nodes;
using Godot;

namespace Veronenger.Game;

public class ScreenSettingsManager {
    [Inject] public Settings Settings { get; set; }

    public ScreenController ScreenController { get; private set; }

    public SaveSetting<bool> Fullscreen => Settings.Fullscreen;                  
    public SaveSetting<bool> VSync => Settings.VSync;                       
    public SaveSetting<bool> Borderless => Settings.Borderless;                  
    public SaveSetting<Vector2I> WindowedResolution => Settings.WindowedResolution;       

    public ScreenSettingsManager(ScreenConfig initialScreenConfig) {
        ScreenController = new ScreenController(initialScreenConfig);
    }

    public void Start() {
        ScreenController.Start();
        
        ScreenController.FullscreenSetting = Fullscreen;
        ScreenController.VSyncSetting = VSync;
        ScreenController.BorderlessSetting = Borderless;
        ScreenController.WindowedResolutionSetting = WindowedResolution;
        
        ScreenController.DoVSync(VSync.Value);
        if (Fullscreen.Value) {
            ScreenController.DoFullscreen(true);
        } else {
            ScreenController.DoWindowed(new Resolution(WindowedResolution.Value));
        }
        ScreenController.DoBorderless(Borderless.Value);
        ScreenController.CenterWindow();
        NodeManager.MainInstance.OnWMCloseRequest += Stop;
    }

    public void Stop() {
        ScreenController.Stop();
    }

    /*
    private int _currentScreen = -1;
    private Vector2 _screenSize = Vector2.Zero;

    // Detect current screen change or monitor resolution (screen size) changed
    public override void _PhysicsProcess(double delta) {
        var screenSize = OS.GetScreenSize();
        if (_currentScreen != OS.CurrentScreen || screenSize != _screenSize) {
            _screenSize = screenSize;
            _currentScreen = OS.CurrentScreen;
            // GD.Print("New currentScreen: " + _currentScreen + " " + _screenSize);
            ScreenChanged();
        }
    }

    private void ScreenChanged() {
        var screenSize = OS.GetScreenSize();
        if (!OS.WindowFullscreen) {
            // Make the application window the smallest so it can fit the screen
            var windowSize = OS.WindowSize;
            if (windowSize.X >= screenSize.X || windowSize.Y >= screenSize.Y) {
                var scaledResolution = GetResolutions()[0];
                // GD.Print("OPPPPPS BIGGER WINDOW THAN SCREEN. Changing to " + scaledResolution);
                SetWindowed(scaledResolution);
            }
        }
    }
    */
    public void SetWindowed(Resolution resolution) {
        ScreenController.SetWindowed(resolution);
        ScreenController.CenterWindow();
    }
}