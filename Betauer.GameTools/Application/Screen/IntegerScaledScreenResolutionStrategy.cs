using System;
using System.Collections.Generic;
using Godot;

namespace Betauer.Application.Screen {
    /**
     * https://github.com/Yukitty/godot-addon-integer_resolution_handler
     *
     * 
     * 
     */
    public class IntegerScaledScreenResolutionStrategy : BaseScreenResolutionService, IScreenStrategy {
        private static readonly Logger Logger = LoggerFactory.GetLogger(typeof(IntegerScaledScreenResolutionStrategy));
        public IntegerScaledScreenResolutionStrategy(SceneTree tree) : base(tree) {
        }

        public List<ScaledResolution> GetResolutions() {
            return BaseResolution.ExpandResolutionByWith(AspectRatios);
        }
        
        protected override void Setup() {
            // Viewport means no interpolation when stretching, which it doesn't matter for bitmap graphics
            // because the image is scaled by x1 x2... so, viewport means fonts will shown worse
            // Mode2D shows betters fonts

            // Enforce minimum resolution.
            OS.MinWindowSize = ScreenConfiguration.BaseResolution.Size;
            if (OS.WindowSize < OS.MinWindowSize) {
                OS.WindowSize = OS.MinWindowSize;
            }
            OS.WindowResizable = ScreenConfiguration.IsResizeable;
            Tree.SetScreenStretch(SceneTree.StretchMode.Disabled, SceneTree.StretchAspect.Ignore, BaseResolution.Size, 1);
            var rootViewport = Tree.Root;
            var windowSize = OS.WindowFullscreen ? OS.GetScreenSize() : OS.WindowSize;
            var scale = Resolution.CalculateMaxScale(BaseResolution.Size, windowSize);
            var screenSize = BaseResolution.Size;
            var viewportSize = screenSize * scale;
            var overScan = ((windowSize - viewportSize) / scale).Floor();

            switch (StretchAspect) {
                case SceneTree.StretchAspect.KeepWidth: {
                    screenSize.y += overScan.y;
                    break;
                }
                case SceneTree.StretchAspect.KeepHeight: {
                    screenSize.x += overScan.x;
                    break;
                }
                case SceneTree.StretchAspect.Expand:
                case SceneTree.StretchAspect.Ignore: {
                    screenSize += overScan;
                    break;
                }
                case SceneTree.StretchAspect.Keep:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            viewportSize = screenSize * scale;
            var margin = (windowSize - viewportSize) / 2;
            var margin2 = margin.Ceil();
            margin = margin.Floor();
            var attachToScreenRect = new Rect2(margin, viewportSize);
            
            if (StretchMode == SceneTree.StretchMode.Viewport) {
                rootViewport.Size = (screenSize / Zoom).Floor();
                rootViewport.SetAttachToScreenRect(attachToScreenRect);
                rootViewport.SizeOverrideStretch = false;
                rootViewport.SetSizeOverride(false);
            } else {
                // Mode2d || Disabled
                rootViewport.Size = (viewportSize / Zoom).Floor();
                rootViewport.SetAttachToScreenRect(attachToScreenRect);
                rootViewport.SizeOverrideStretch = true;
                rootViewport.SetSizeOverride(true, (screenSize / Zoom).Floor());
            }
            Logger.Debug($"{StretchMode}/{StretchAspect} | WindowSize {windowSize.x}x{windowSize.y} | Viewport {screenSize.x}x{screenSize.y} | AttachToScreenRect pos:{attachToScreenRect.Position.x}x{attachToScreenRect.Position.y} size:{attachToScreenRect.Size.x}x{attachToScreenRect.Size.y}");

            VisualServer.BlackBarsSetMargins(
                Mathf.Max(0, (int)margin.x),
                Mathf.Max(0, (int)margin.y),
                Mathf.Max(0, (int)margin2.x),
                Mathf.Max(0, (int)margin2.y));
        }
    }
}