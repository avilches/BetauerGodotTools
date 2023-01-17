using System;
using System.Collections.Generic;
using Betauer.Core.Signal;
using Godot;

namespace Betauer.Application.Screen; 

public class ScreenService {
    public SceneTree SceneTree;

    public IScreenStrategy ScreenStrategy => ScreenConfiguration.Strategy;
    public ScreenConfiguration ScreenConfiguration { get; private set; }


    public ScreenService(SceneTree sceneTree, ScreenConfiguration screenConfiguration, IScreenStrategy screenStrategy) {
        SceneTree = sceneTree;
        SceneTree.Root.OnSizeChanged(OnScreenResized);
        SetScreenConfiguration(screenConfiguration);
    }

    private void OnScreenResized() {
        if (ScreenConfiguration.IsResizeable && ScreenStrategy is IScreenResizeHandler handler) handler.OnScreenResized();
    }

    public void SetScreenConfiguration(ScreenConfiguration screenConfiguration) {
        ScreenConfiguration = screenConfiguration ?? throw new ArgumentNullException(nameof(screenConfiguration));
        ReconfigureService();
    }

    public void SetStrategy(IScreenStrategy screenStrategy) {
        ScreenConfiguration.Strategy = screenStrategy;
        ReconfigureService();
    }

    public void Apply() {
        ScreenStrategy.Apply();
    }

    private void ReconfigureService() {
        ScreenStrategy.Disable(); // can be null, but only the first time
        ScreenStrategy.SetScreenService(this);
            
        DisplayServer.WindowSetFlag(DisplayServer.WindowFlags.ResizeDisabled, !ScreenConfiguration.IsResizeable);
    }
        
    public bool IsFullscreen() => ScreenStrategy.IsFullscreen();
    public void SetFullscreen() => ScreenStrategy.SetFullscreen();
    public void SetBorderless(bool borderless) => ScreenStrategy.SetBorderless(borderless);
    public void SetWindowed(Resolution resolution) => ScreenStrategy.SetWindowed(resolution);
    public List<ScaledResolution> GetResolutions() => ScreenStrategy.GetResolutions();

    public void CenterWindow() => ScreenStrategy.CenterWindow();
}

public interface IScreenResizeHandler {
    void OnScreenResized();
}