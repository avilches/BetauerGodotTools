using System;
using System.Collections.Generic;
using Godot;

namespace Betauer.Screen {
    public class ScreenConfiguration {
        public Resolution BaseResolution { get; }

        // disabled: while the framebuffer will be resized to match the game window, nothing will be upscaled or downscaled (this includes GUIs).
        // 2d: the framebuffer is still resized, but GUIs can be upscaled or downscaled. This can result in blurry or pixelated fonts.
        // viewport: the framebuffer is resized, but computed at the original size of the project. The whole rendering will be pixelated. You generally do not want this, unless it's part of the game style.
        public SceneTree.StretchMode StretchMode { get; }
        public SceneTree.StretchAspect StretchAspect { get; }
        public float Zoom { get; }

        public ScreenConfiguration(Resolution baseResolution, SceneTree.StretchMode stretchMode,
            SceneTree.StretchAspect stretchAspect, float zoom = 1f) {
            BaseResolution = baseResolution;
            StretchMode = stretchMode;
            StretchAspect = stretchAspect;
            Zoom = zoom;
        }
    }

    public interface IScreenService : IDisposable {
        void Configure(ScreenConfiguration configuration);
        bool IsFullscreen();
        void SetFullscreen();
        void SetBorderless(bool borderless);
        void SetWindowed(Resolution resolution);
        List<ScaledResolution> GetResolutions();
    }

    public abstract class BaseResolutionService {
        protected ScreenConfiguration ScreenConfiguration;
        protected readonly SceneTree Tree;

        protected Resolution BaseResolution => ScreenConfiguration.BaseResolution;
        protected SceneTree.StretchMode StretchMode => ScreenConfiguration.StretchMode;
        protected SceneTree.StretchAspect StretchAspect => ScreenConfiguration.StretchAspect;
        protected float Zoom => ScreenConfiguration.Zoom;

        protected BaseResolutionService(SceneTree tree) {
            Tree = tree;
        }

        public virtual void Configure(ScreenConfiguration screenConfiguration) {
            ScreenConfiguration = screenConfiguration;
            DoConfigure();
        }

        public List<ScaledResolution> GetResolutions() {
            var screenSize = OS.GetScreenSize();
            List<ScaledResolution> resolutions = new List<ScaledResolution>();
            foreach (var resolution in Resolutions.All()) {
                if (resolution.x <= screenSize.x &&
                    resolution.y <= screenSize.y &&
                    resolution.x >= BaseResolution.x &&
                    resolution.y >= BaseResolution.y) {
                    resolutions.Add(new ScaledResolution(BaseResolution.Size, resolution.Size));
                }
            }
            return resolutions;
        }

        public void SetWindowed(Resolution resolution) {
            var screenSize = OS.GetScreenSize();
            if (resolution.x > screenSize.x || resolution.y > screenSize.y) {
                SetFullscreen();
                return;
            }
            if (resolution.x < BaseResolution.x || resolution.y < BaseResolution.y) {
                DoSetWindowed(BaseResolution);
            } else {
                DoSetWindowed(resolution);
            }
        }

        protected abstract void DoConfigure();
        protected abstract void DoSetWindowed(Resolution resolution);
        public abstract void SetFullscreen();

        public void CenterWindow() {
            if (OS.WindowFullscreen) return;
            OS.CenterWindow();
            // TODO why this instead of OS.CenterWindow()
            // var currentScreen = OS.CurrentScreen;
            // var screenSize = OS.GetScreenSize(currentScreen);
            // var windowSize = OS.WindowSize;
            // var centeredPos = (screenSize - windowSize) / 2;
            // OS.WindowPosition = centeredPos;
            // OS.CurrentScreen = currentScreen;
        }
    }

    public class RegularResolutionService : BaseResolutionService, IScreenService {
        private static readonly Logger Logger = LoggerFactory.GetLogger(typeof(RegularResolutionService));

        public RegularResolutionService(SceneTree tree) : base(tree) {
        }

        protected override void DoConfigure() {
            // Enforce minimum resolution.
            OS.MinWindowSize = BaseResolution.Size;
            Tree.SetScreenStretch(StretchMode, StretchAspect, BaseResolution.Size, Zoom);
        }

        public void Dispose() {
        }

        public bool IsFullscreen() => OS.WindowFullscreen;

        public override void SetFullscreen() {
            OS.WindowBorderless = false;
            OS.WindowFullscreen = true;
        }

        public void SetBorderless(bool borderless) {
            if (IsFullscreen()) {
                Logger.Debug("SetBorderless true: ignoring because IsFullScreen() true");
                return;
            }
            OS.WindowBorderless = borderless;
        }

        protected override void DoSetWindowed(Resolution resolution) {
            Logger.Debug("Set Window size to: " + resolution);
            OS.WindowFullscreen = false;
            OS.WindowSize = resolution.Size;
            CenterWindow();
        }
    }

    /**
     * https://github.com/Yukitty/godot-addon-integer_resolution_handler
     */
    public class IntegerScaleResolutionService : BaseResolutionService, IScreenService {
        private static readonly Logger Logger = LoggerFactory.GetLogger(typeof(IntegerScaleResolutionService));
        private OnResizeWindowHandler _onResizeWindowHandler;

        public IntegerScaleResolutionService(SceneTree tree) : base(tree) {
        }

        protected override void DoConfigure() {
            // Enforce minimum resolution.
            OS.MinWindowSize = BaseResolution.Size;
            // Remove default stretch behavior.
            Tree.SetScreenStretch(SceneTree.StretchMode.Disabled, SceneTree.StretchAspect.Keep, BaseResolution.Size,
                1);
            _onResizeWindowHandler = new OnResizeWindowHandler(Tree, UpdateResolution);
            UpdateResolution();
        }

        public void Dispose() {
            _onResizeWindowHandler.Dispose();
        }

        public bool IsFullscreen() => OS.WindowFullscreen;

        public override void SetFullscreen() {
            OS.WindowBorderless = false;
            OS.WindowFullscreen = true;
            UpdateResolution();
        }

        public void SetBorderless(bool borderless) {
            if (IsFullscreen()) {
                Logger.Debug("SetBorderless true: ignoring because IsFullScreen() true");
                return;
            }
            OS.WindowBorderless = borderless;
            UpdateResolution();
        }

        protected override void DoSetWindowed(Resolution resolution) {
            OS.WindowFullscreen = false;
            OS.WindowSize = resolution.Size;
            CenterWindow();
            UpdateResolution();
        }

        private void UpdateResolution() {
            var windowSize = OS.WindowFullscreen ? OS.GetScreenSize() : OS.WindowSize;
            var maxScale = Resolution.CalculateMaxScale(windowSize, BaseResolution.Size);
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
                    Logger.Debug("Mode: Viewport. Base:" + BaseResolution + " Viewport:" + windowSize.x + "x" + windowSize.y);
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