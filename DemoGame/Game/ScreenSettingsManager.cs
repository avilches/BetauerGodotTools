using System.Collections.Generic;
using Betauer.Application.Screen;
using Betauer.Application.Screen.Resolution;
using Betauer.Application.Settings;
using Betauer.DI.Attributes;
using Godot;

namespace Veronenger.Game;

public class ScreenSettingsManager {
    private const bool DontSave = false;
    
    [Inject] public Settings Settings { get; set; }

    private readonly ScreenConfig _initialScreenConfig;
    public ScreenController ScreenController => _service ??= new ScreenController(_initialScreenConfig);

    private ScreenController? _service;
    
    public SaveSetting<bool> PixelPerfectSetting => Settings.PixelPerfect;                
    public SaveSetting<bool> FullscreenSetting => Settings.Fullscreen;                  
    public SaveSetting<bool> VSyncSetting => Settings.VSync;                       
    public SaveSetting<bool> BorderlessSetting => Settings.Borderless;                  
    public SaveSetting<Vector2I> WindowedResolutionSetting => Settings.WindowedResolution;       

    public bool PixelPerfect => PixelPerfectSetting.Value;
    public bool Fullscreen => FullscreenSetting.Value;
    public bool VSync => VSyncSetting.Value;
    public bool Borderless => BorderlessSetting.Value;
    public Resolution WindowedResolution => new Resolution(WindowedResolutionSetting.Value);

    public ScreenSettingsManager(ScreenConfig initialScreenConfig) {
        _initialScreenConfig = initialScreenConfig;
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

    public bool IsFullscreen() => ScreenController.IsFullscreen();
    public List<ScaledResolution> GetResolutions() => ScreenController.GetResolutions();

    public void SetPixelPerfect(bool pixelPerfect, bool save = true) {
        // TODO Godot 4
        // var strategy = pixelPerfect
            // ? ScreenService.ScreenStrategyKey.WindowSize // IntegerScale
            // : ScreenService.ScreenStrategyKey.ViewportSize;
        // ScreenService.SetStrategy(strategy);
        if (save) {
            PixelPerfectSetting.Value = pixelPerfect;
            ForceSave(PixelPerfectSetting);
        }
    }

    public void SetBorderless(bool borderless, bool save = true) {
        // TODO Godot 4
        // ScreenService.SetBorderless(borderless);
        if (save) {
            BorderlessSetting.Value = borderless;
            ForceSave(BorderlessSetting);
        }
    }

    public void SetVSync(bool vsync, bool save = true) {
        // TODO Godot 4: allow more VSync modes
        DisplayServer.WindowSetVsyncMode(vsync ? DisplayServer.VSyncMode.Enabled : DisplayServer.VSyncMode.Disabled);
        if (save) {
            VSyncSetting.Value = vsync;
            ForceSave(VSyncSetting);
        }
    }

    public void SetFullscreen(bool fs, bool save = true) {
        if (fs) {
            ScreenController.SetFullscreen();
        } else {
            SetWindowed(WindowedResolution, false); // Never save resolution because it has not changed
            CenterWindow();
        }
        if (save) {
            FullscreenSetting.Value = fs;
            ForceSave(FullscreenSetting);
        }
    }

    private static void ForceSave(SaveSetting setting) {
        // Only force save if the setting is not auto-saved
        if (setting is SaveSetting { AutoSave: false } saveSetting) saveSetting.SettingsContainer!.Save();
    }

    public void SetWindowed(Betauer.Application.Screen.Resolution.Resolution resolution, bool save = true) {
        ScreenController.SetWindowed(resolution);
        if (save) {
            WindowedResolutionSetting.Value = resolution.Size;
            ForceSave(WindowedResolutionSetting);
        }
    }

    public void CenterWindow() {
        ScreenController.CenterWindow();
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