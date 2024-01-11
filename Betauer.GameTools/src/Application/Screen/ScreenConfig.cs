using System.Collections.Generic;
using System.Linq;
using Betauer.Application.Screen.Resolution;
using Godot;

namespace Betauer.Application.Screen; 

public class ScreenConfig {
    public Resolution.Resolution BaseResolution { get; set; }
    public bool IsResizeable { get; set; }
    public List<Resolution.Resolution> Resolutions { get; set; }
    public List<AspectRatio> AspectRatios { get; set; }

    // disabled: while the framebuffer will be resized to match the game window, nothing will be upscaled or downscaled (this includes GUIs).
    // 2d: the framebuffer is still resized, but GUIs can be upscaled or downscaled. This can result in blurry or pixelated fonts.
    // viewport: the framebuffer is resized, but computed at the original size of the project. The whole rendering will be pixelated. You generally do not want this, unless it's part of the game style.
    public Window.ContentScaleModeEnum ScaleMode;
    public Window.ContentScaleAspectEnum ScaleAspect;
    public float ScaleFactor { get; set; }

    public ScreenConfig(
        Resolution.Resolution baseResolution, Window.ContentScaleModeEnum scaleMode,
        Window.ContentScaleAspectEnum scaleAspect, List<Resolution.Resolution>? resolutions = null, bool isResizeable = false,
        float scaleFactor = 1f) {
        
        BaseResolution = baseResolution;
        ScaleMode = scaleMode;
        ScaleAspect = scaleAspect;
        Resolutions = resolutions ?? Resolution.Resolutions.GetAll(Resolution.AspectRatios.Ratio16_9);
        AspectRatios = Resolutions.Select(r => r.AspectRatio).Distinct().ToList();
        IsResizeable = isResizeable;
        ScaleFactor = scaleFactor;
    }
}