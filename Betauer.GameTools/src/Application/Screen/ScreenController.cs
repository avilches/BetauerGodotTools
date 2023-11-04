using System;
using System.Collections.Generic;
using Betauer.Application.Screen.Resolution;
using Godot;

namespace Betauer.Application.Screen; 

public class ScreenController {
    private static readonly SceneTree SceneTree = (SceneTree)Engine.GetMainLoop();
    private ScreenConfig _screenConfig;

    public ScreenConfig ScreenConfig {
        get => _screenConfig;
        set {
            _screenConfig = value;
            Apply();
        }
    }

    public ScreenController(ScreenConfig screenConfig) {
        ScreenConfig = screenConfig ?? throw new ArgumentNullException(nameof(screenConfig));
        Apply();
        SceneTree.Root.SizeChanged += OnScreenResized;
    }

    private void OnScreenResized() {
        if (ScreenConfig is { IsResizeable: true, Strategy: IScreenResizeHandler handler }) {
            handler.OnScreenResized();
        }
    }

    public void Apply() {
        ScreenConfig.Strategy.Apply();
    }
        
    public bool IsFullscreen() => ScreenConfig.Strategy.IsFullscreen();
    public void SetFullscreen() => ScreenConfig.Strategy.SetFullscreen();
    public void SetBorderless(bool borderless) => ScreenConfig.Strategy.SetBorderless(borderless);
    public void SetWindowed(Resolution.Resolution resolution) => ScreenConfig.Strategy.SetWindowed(resolution);
    public List<ScaledResolution> GetResolutions() => ScreenConfig.Strategy.GetResolutions();
    public void CenterWindow() => ScreenConfig.Strategy.CenterWindow();
}