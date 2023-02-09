using System.Collections.Generic;
using Betauer.Application.Monitor;
using Betauer.Application.Settings;
using Betauer.DI;
using Godot;
using Container = Betauer.DI.Container;

namespace Betauer.Application.Screen;
public class ScreenSettingsManager {
    [Inject] protected Container Container { get; set; }
    [Inject] protected SceneTree SceneTree { get; set; }
    [Inject(Nullable = true)] protected DebugOverlayManager? DebugOverlayManager { get; set; }

    private const bool DontSave = false;

    private readonly ScreenConfiguration _initialScreenConfiguration;
    public ScreenService ScreenService => _service ??= new ScreenService(SceneTree, _initialScreenConfiguration, FixedViewportStrategy.Instance);
    public ScreenConfiguration ScreenConfiguration => ScreenService.ScreenConfiguration;

    private ScreenService? _service;
    
    private ISetting<bool> _pixelPerfect;
    private ISetting<bool> _fullscreen;
    private ISetting<bool> _vSync;
    private ISetting<bool> _borderless;
    private ISetting<Resolution> _windowedResolution;

    public bool PixelPerfect => _pixelPerfect.Value;
    public bool Fullscreen => _fullscreen.Value;
    public bool VSync => _vSync.Value;
    public bool Borderless => _borderless.Value;
    public Resolution WindowedResolution => _windowedResolution.Value;

    public ScreenSettingsManager(ScreenConfiguration initialScreenConfiguration) {
        _initialScreenConfiguration = initialScreenConfiguration;
    }

    [PostInject]
    private void ConfigureSettings() {
        _pixelPerfect = Container.ResolveOr<ISetting<bool>>("Settings.Screen.PixelPerfect",
            () => Setting<bool>.Memory(false));

        _fullscreen = Container.ResolveOr<ISetting<bool>>("Settings.Screen.Fullscreen",
            () => Setting<bool>.Memory(AppTools.GetWindowFullscreen()));

        _vSync = Container.ResolveOr<ISetting<bool>>("Settings.Screen.VSync",
            () => Setting<bool>.Memory(AppTools.GetWindowVsync()));

        _borderless = Container.ResolveOr<ISetting<bool>>("Settings.Screen.Borderless",
            () => Setting<bool>.Memory(AppTools.GetWindowBorderless()));

        _windowedResolution = Container.ResolveOr<ISetting<Resolution>>("Settings.Screen.WindowedResolution",
            () => Setting<Resolution>.Memory(_initialScreenConfiguration.BaseResolution));

    }

    [PostInject]
    private void ConfigureCommands() {
        DebugOverlayManager?.DebugConsole.AddScreenSettingsCommand(this);
    }
    
    public void Setup() {
        SetPixelPerfect(PixelPerfect, DontSave);
        SetVSync(VSync, DontSave);
        if (Fullscreen) {
            SetFullscreen(true, DontSave);
        } else {
            SetWindowed(WindowedResolution, DontSave);
            SetBorderless(Borderless, DontSave);
        }
    }

    public void SetScreenConfiguration(ScreenConfiguration screenConfiguration) {
        ScreenService.SetScreenConfiguration(screenConfiguration);
    }

    public void SetStrategy(IScreenStrategy strategy) {
        ScreenService.SetStrategy(strategy);
    }

    public bool IsFullscreen() => ScreenService.IsFullscreen();
    public List<ScaledResolution> GetResolutions() => ScreenService.GetResolutions();

    public void SetPixelPerfect(bool pixelPerfect, bool save = true) {
        // TODO Godot 4
        // var strategy = pixelPerfect
            // ? ScreenService.ScreenStrategyKey.WindowSize // IntegerScale
            // : ScreenService.ScreenStrategyKey.ViewportSize;
        // ScreenService.SetStrategy(strategy);
        if (save) _pixelPerfect.Value = pixelPerfect;
    }

    public void SetBorderless(bool borderless, bool save = true) {
        // TODO Godot 4
        // ScreenService.SetBorderless(borderless);
        if (save) _borderless.Value = borderless;
    }

    public void SetVSync(bool vsync, bool save = true) {
        // TODO Godot 4: allow more VSync modes
        // DisplayServer.WindowSetVsyncMode(vsync ? DisplayServer.VSyncMode.Enabled : DisplayServer.VSyncMode.Disabled);
        if (save) _vSync.Value = vsync;
    }

    public void SetFullscreen(bool fs, bool save = true) {
        if (fs) {
            ScreenService.SetFullscreen();
        } else {
            SetWindowed(WindowedResolution, false); // Never save resolution because it has not changed
            CenterWindow();
        }
        if (save) _fullscreen.Value = fs;
    }

    public void SetWindowed(Resolution resolution, bool save = true) {
        ScreenService.SetWindowed(resolution);
        if (save) _windowedResolution.Value = resolution;
    }

    public void CenterWindow() {
        ScreenService.CenterWindow();
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
}