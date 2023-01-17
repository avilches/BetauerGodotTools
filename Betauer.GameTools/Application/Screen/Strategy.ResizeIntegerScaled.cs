using System;
using System.Collections.Generic;
using Godot;

namespace Betauer.Application.Screen; 

/**
     * https://github.com/Yukitty/godot-addon-integer_resolution_handler
     *
     * 
     * 
     */
public class ResizeIntegerScaledStrategy : BaseScreenResolutionService, IScreenStrategy {
    public static readonly ResizeIntegerScaledStrategy Instance = new();

    public List<ScaledResolution> GetResolutions() {
        return BaseResolution.ExpandResolutionByWith(BaseResolution, AspectRatios);
    }
        
    protected override void DoApply() {
        // Viewport means no interpolation when stretching, which it doesn't matter for bitmap graphics
        // because the image is scaled by x1 x2... so, viewport means fonts will shown worse
        // Mode2D shows betters fonts
        // TODO Godot 4
            
        // Tree.Root.ContentScaleMode = Window.ContentScaleModeEnum.Disabled;
        // Tree.Root.ContentScaleAspect = Window.ContentScaleAspectEnum.Ignore;
        // Tree.Root.ContentScaleFactor = 1;
        // Tree.Root.ContentScaleSize = BaseResolution.Size;
        /*
        var rootViewport = Tree.Root;
        var windowSize = DisplayServer.WindowGetSize();
        var scale = Resolution.CalculateMaxScale(BaseResolution.Size, windowSize);
        var screenSize = BaseResolution.Size;
        var viewportSize = screenSize * scale;
        var overScan = ((windowSize - viewportSize) / scale); // .Floor();

        switch (StretchAspect) {
            case Window.ContentScaleAspectEnum.KeepWidth: {
                screenSize.y += overScan.y;
                break;
            }
            case Window.ContentScaleAspectEnum.KeepHeight: {
                screenSize.x += overScan.x;
                break;
            }
            case Window.ContentScaleAspectEnum.Expand:
            case Window.ContentScaleAspectEnum.Ignore: {
                screenSize += overScan;
                break;
            }
            case Window.ContentScaleAspectEnum.Keep:
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }

        viewportSize = screenSize * scale;
        var margin = (windowSize - viewportSize) / 2;
        var margin2 = margin; // .Ceil();
        margin = margin; //.Floor();
        var attachToScreenRect = new Rect2(margin, viewportSize);
        
        if (StretchMode == Window.ContentScaleModeEnum.Viewport) {
            rootViewport.Size = (screenSize / (int)Zoom); // .Floor();
            rootViewport.SetAttachToScreenRect(attachToScreenRect);
            rootViewport.SizeOverrideStretch = false;
            rootViewport.SetSizeOverride(false);
        } else {
            // CanvasItems || Disabled
            rootViewport.Size = (viewportSize / Zoom).Floor();
            rootViewport.SetAttachToScreenRect(attachToScreenRect);
            rootViewport.SizeOverrideStretch = true;
            rootViewport.SetSizeOverride(true, (screenSize / Zoom).Floor());
        }
        VisualServer.BlackBarsSetMargins(
            Mathf.Max(0, (int)margin.x),
            Mathf.Max(0, (int)margin.y),
            Mathf.Max(0, (int)margin2.x),
            Mathf.Max(0, (int)margin2.y));

        _state = $"ResizeIntegerScaled {StretchMode}/{StretchAspect} | Zoom {Zoom} | WindowSize {windowSize.x}x{windowSize.y} | Viewport {screenSize.x}x{screenSize.y}";
        */
    }
}