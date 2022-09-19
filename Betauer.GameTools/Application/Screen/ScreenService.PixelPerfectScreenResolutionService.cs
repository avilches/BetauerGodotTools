using System;
using System.Collections.Generic;
using Betauer.Signal;
using Godot;

namespace Betauer.Application.Screen {
    /**
     * https://github.com/Yukitty/godot-addon-integer_resolution_handler
     */
    public class PixelPerfectScreenResolutionService : BaseScreenResolutionService, IScreenService {
        private static readonly Logger Logger = LoggerFactory.GetLogger(typeof(PixelPerfectScreenResolutionService));
        private readonly SignalHandler _signalHandlerAction;
        private bool _enabled = false;

        public PixelPerfectScreenResolutionService(SceneTree tree) : base(tree) {
            _signalHandlerAction = Tree.OnScreenResized(SignalHandler);
        }

        public void SignalHandler() {
            if (_enabled) ScaleResolutionViewport();
        }
        
        protected override void OnDispose(bool disposing) {
            _signalHandlerAction.Disconnect();
        }

        public void Enable(ScreenConfiguration screenConfiguration) {
            ScreenConfiguration = screenConfiguration;
            // Viewport means no interpolation when stretching, which it doesn't matter for bitmap graphics
            // because the image is scaled by x1 x2... so, viewport means fonts will shown worse

            // Mode2D shows betters fonts
            Tree.SetScreenStretch(SceneTree.StretchMode.Mode2d, SceneTree.StretchAspect.Keep, BaseResolution.Size,
                1);
            ScaleResolutionViewport();
            _enabled = true;
        }

        public void Disable() {
            _enabled = false;
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
            CenterWindow();
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
                    Logger.Debug("Mode: Viewport. Base:" + BaseResolution + " Viewport:" + windowSize.x + "x" +
                                 windowSize.y);
                    break;
                }
                case SceneTree.StretchMode.Mode2d:
                case SceneTree.StretchMode.Disabled: {
                    rootViewport.Size = (viewportSize / Zoom).Floor();
                    rootViewport.SetAttachToScreenRect(new Rect2(margin, viewportSize));
                    rootViewport.SizeOverrideStretch = true;
                    rootViewport.SetSizeOverride(true, (screenSize / Zoom).Floor());
                    Logger.Debug("Mode: 2D. Base:" + BaseResolution + " Viewport:" + windowSize.x + "x" + windowSize.y);
                    break;
                }
                default:
                    throw new ArgumentOutOfRangeException();
            }

            VisualServer.BlackBarsSetMargins(
                Mathf.Max(0, (int)margin.x),
                Mathf.Max(0, (int)margin.y),
                Mathf.Max(0, (int)margin2.x),
                Mathf.Max(0, (int)margin2.y));
        }

        public void LoadProjectSettings() {
            var SETTING_BASE_WIDTH = "display/window/integer_resolution_handler/base_width";
            var SETTING_BASE_HEIGHT = "display/window/integer_resolution_handler/base_height";

            Vector2 base_resolution = Vector2.Zero;
            if (ProjectSettings.HasSetting(SETTING_BASE_WIDTH) && ProjectSettings.HasSetting(SETTING_BASE_HEIGHT)) {
                base_resolution.x = ProjectSettings.GetSetting(SETTING_BASE_WIDTH).ToString().ToInt();
                base_resolution.y = ProjectSettings.GetSetting(SETTING_BASE_HEIGHT).ToString().ToInt();
            }

            /*
            match ProjectSettings.GetSetting("display/window/stretch/mode"):
            "2d":
            stretch_mode = SceneTree.STRETCH_MODE_2D
            "viewport":
            stretch_mode = SceneTree.STRETCH_MODE_VIEWPORT
            _:
            stretch_mode = SceneTree.STRETCH_MODE_DISABLED

            match ProjectSettings.GetSetting("display/window/stretch/aspect"):
            "keep":
            stretch_aspect = SceneTree.STRETCH_ASPECT_KEEP
            "keep_height":
            stretch_aspect = SceneTree.STRETCH_ASPECT_KEEP_HEIGHT
            "keep_width":
            stretch_aspect = SceneTree.STRETCH_ASPECT_KEEP_WIDTH
            "expand":
            stretch_aspect = SceneTree.STRETCH_ASPECT_EXPAND
            _:
            stretch_aspect = SceneTree.STRETCH_ASPECT_IGNORE
            */
        }
    }
}