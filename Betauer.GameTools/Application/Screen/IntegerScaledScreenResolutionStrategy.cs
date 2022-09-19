using System;
using System.Collections.Generic;
using Godot;

namespace Betauer.Application.Screen {
    /**
     * https://github.com/Yukitty/godot-addon-integer_resolution_handler
     */
    public class IntegerScaledScreenResolutionStrategy : BaseScreenResolutionService, IScreenStrategy, IScreenResizeHandler {
        private static readonly Logger Logger = LoggerFactory.GetLogger(typeof(IntegerScaledScreenResolutionStrategy));

        public IntegerScaledScreenResolutionStrategy(SceneTree tree) : base(tree) {
        }

        public void Enable(ScreenConfiguration screenConfiguration) {
            ScreenConfiguration = screenConfiguration;
            // Viewport means no interpolation when stretching, which it doesn't matter for bitmap graphics
            // because the image is scaled by x1 x2... so, viewport means fonts will shown worse

            // Mode2D shows betters fonts
            Tree.SetScreenStretch(SceneTree.StretchMode.Mode2d, SceneTree.StretchAspect.Keep, BaseResolution.Size,
                1);
            ScaleResolutionViewport();
        }

        public void Disable() {
        }

        public void OnScreenResized() {
            ScaleResolutionViewport();
        }

        public List<ScaledResolution> GetResolutions() {
            return BaseResolution.ExpandResolutionByWith(AspectRatios);
        }

        public override void SetFullscreen() {
            if (!OS.WindowFullscreen) {
                if (!FeatureFlags.IsMacOs()) {
                    OS.WindowBorderless = false;
                }
                OS.WindowFullscreen = true;
            }
            ScaleResolutionViewport();
        }

        protected override void DoSetBorderless(bool borderless) {
            if (!FeatureFlags.IsMacOs()) {
                if (OS.WindowBorderless == borderless) return;
                OS.WindowBorderless = borderless;
                ScaleResolutionViewport();
            }
        }

        protected override void DoSetWindowed(Resolution resolution) {
            if (OS.WindowFullscreen) OS.WindowFullscreen = false;
            OS.WindowSize = resolution.Size;
            ScaleResolutionViewport();
        }

        private void ScaleResolutionViewport() {
            var windowSize = OS.WindowFullscreen ? OS.GetScreenSize() : OS.WindowSize;
            var maxScale = Resolution.CalculateMaxScale(BaseResolution.Size, windowSize);
            var screenSize = BaseResolution.Size;
            var viewportSize = screenSize * maxScale;
            var overScan = ((windowSize - viewportSize) / maxScale).Floor();

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

            viewportSize = screenSize * maxScale;
            var margin = (windowSize - viewportSize) / 2;
            var margin2 = margin.Ceil();
            margin = margin.Floor();

            ChangeViewport(screenSize, margin, viewportSize, windowSize, margin2);
        }

        private void ChangeViewport(Vector2 screenSize, Vector2 margin, Vector2 viewportSize, Vector2 windowSize,
            Vector2 margin2) {
            Viewport rootViewport = Tree.Root;
            switch (StretchMode) {
                case SceneTree.StretchMode.Viewport: {
                    rootViewport.Size = (screenSize / Zoom).Floor();
                    rootViewport.SetAttachToScreenRect(new Rect2(margin, viewportSize));
                    rootViewport.SizeOverrideStretch = false;
                    rootViewport.SetSizeOverride(false);
                    break;
                }
                case SceneTree.StretchMode.Mode2d:
                case SceneTree.StretchMode.Disabled: {
                    rootViewport.Size = (viewportSize / Zoom).Floor();
                    rootViewport.SetAttachToScreenRect(new Rect2(margin, viewportSize));
                    rootViewport.SizeOverrideStretch = true;
                    rootViewport.SetSizeOverride(true, (screenSize / Zoom).Floor());
                    break;
                }
                default:
                    throw new ArgumentOutOfRangeException();
            }
            Logger.Debug($"IntegerScaled: {StretchMode}/{StretchAspect} | WindowSize {windowSize.x}x{windowSize.y} | Viewport {screenSize.x}x{screenSize.y}");

            VisualServer.BlackBarsSetMargins(
                Mathf.Max(0, (int)margin.x),
                Mathf.Max(0, (int)margin.y),
                Mathf.Max(0, (int)margin2.x),
                Mathf.Max(0, (int)margin2.y));
        }
    }
}