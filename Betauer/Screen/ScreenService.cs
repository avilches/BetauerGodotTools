using System;
using Godot;
using NUnit.Framework;

namespace Betauer.Screen {
    public interface IScreenService {
        void Configure(ScreenConfiguration configuration);

        bool IsFullscreen();
        void SetFullscreen();
        int GetScale();
        int GetMaxScale();
        void SetBorderless(bool borderless);
        void SetWindowed(int scale);

        void CenterWindow();

        void OnScreenResized();
    }

    // https://github.com/Yukitty/godot-addon-integer_resolution_handler
    public class ScreenIntegerResolutionService : IScreenService {
        private static readonly Logger Logger = LoggerFactory.GetLogger(typeof(ScreenIntegerResolutionService));

        private readonly SceneTree _tree;

        private ScreenConfiguration _screenConfiguration;

        public ScreenIntegerResolutionService(SceneTree sceneTree) {
            _tree = sceneTree;
        }

        private Vector2 BaseResolution => _screenConfiguration.BaseResolution;
        private SceneTree.StretchMode StretchMode => _screenConfiguration.StretchMode;
        private SceneTree.StretchAspect StretchAspect => _screenConfiguration.StretchAspect;
        private int StretchShrink => _screenConfiguration.StretchShrink;

        private int _maxScale;
        private int _scale = -1;

        public void Configure(ScreenConfiguration screenConfiguration) {
            _screenConfiguration = screenConfiguration;

            // Enforce minimum resolution.
            OS.MinWindowSize = BaseResolution;

            // Remove default stretch behavior.
            _tree.SetScreenStretch(SceneTree.StretchMode.Disabled, SceneTree.StretchAspect.Keep, BaseResolution, 1);
            CalculateMaxScaleCurrentScreen();
        }

        private void CalculateMaxScaleCurrentScreen() {
            var screenSize = OS.GetScreenSize();
            _maxScale = (int)Mathf.Max(Mathf.Floor(Mathf.Min(screenSize.x / BaseResolution.x, screenSize.y / BaseResolution.y)),
                1);
        }

        public bool IsFullscreen() => OS.WindowFullscreen;

        public void SetFullscreen() {
            CalculateMaxScaleCurrentScreen();
            OS.WindowBorderless = false;
            OS.WindowFullscreen = true;
            if (_scale == -1) _scale = _maxScale;
            ResolutionUpdated();
        }

        public int GetScale() {
            return _scale;
        }

        public int GetMaxScale() {
            return _maxScale;
        }

        public void SetBorderless(bool borderless) {
            if (IsFullscreen()) return;
            OS.WindowBorderless = borderless;
            ResolutionUpdated();
        }

        public void SetWindowed(int newScale) {
            CalculateMaxScaleCurrentScreen();
            newScale = newScale > _maxScale || newScale < 1 ? 1 : newScale;
            OS.WindowFullscreen = false;
            OS.WindowSize = BaseResolution * newScale;
            CenterWindow();
            ResolutionUpdated();
        }

        public void CenterWindow() {
            if (!OS.WindowFullscreen) {
                var currentScreen = OS.CurrentScreen;
                var screenSize = OS.GetScreenSize(currentScreen);
                var windowSize = OS.WindowSize;
                var centeredPos = (screenSize - windowSize) / 2;
                OS.WindowPosition = centeredPos;
                OS.CurrentScreen = currentScreen;
            }
        }

        public void OnScreenResized() {
            ResolutionUpdated();
        }

        private void ResolutionUpdated() {
            var windowSize = OS.WindowFullscreen ? OS.GetScreenSize() : OS.WindowSize;
            var scale = (int)Mathf.Max(Mathf.Floor(Mathf.Min(windowSize.x / BaseResolution.x, windowSize.y / BaseResolution.y)), 1);
            if (!OS.WindowFullscreen) {
                _scale = scale;
            }

            var screenSize = BaseResolution;
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

            ChangeViewport(screenSize, margin, viewportSize, windowSize, margin2);
        }

        private void ChangeViewport(Vector2 screenSize, Vector2 margin, Vector2 viewportSize, Vector2 windowSize,
            Vector2 margin2) {
            Viewport rootViewport = _tree.Root;
            switch (StretchMode) {
                case SceneTree.StretchMode.Viewport: {
                    rootViewport.Size = (screenSize / StretchShrink).Floor();
                    rootViewport.SetAttachToScreenRect(new Rect2(margin, viewportSize));
                    rootViewport.SizeOverrideStretch = false;
                    rootViewport.SetSizeOverride(false);
                    Logger.Debug("(Viewport Mode) Base resolution:", BaseResolution.x, "x", BaseResolution.y,
                        " Video resolution:", windowSize.x, "x", windowSize.y,
                        " Size:", (screenSize / StretchShrink).Floor(), "(Screen size ", screenSize, "/",
                        StretchShrink, " stretch shrink)");
                    break;
                }
                case SceneTree.StretchMode.Mode2d:
                case SceneTree.StretchMode.Disabled: {
                    rootViewport.Size = (viewportSize / StretchShrink).Floor();
                    rootViewport.SetAttachToScreenRect(new Rect2(margin, viewportSize));
                    rootViewport.SizeOverrideStretch = true;
                    rootViewport.SetSizeOverride(true, (screenSize / StretchShrink).Floor());
                    Logger.Debug("(2D model) Base resolution:", BaseResolution.x, "x", BaseResolution.y,
                        " Video resolution:", windowSize.x, "x", windowSize.y,
                        " Size:", (viewportSize / StretchShrink).Floor(), " (Viewport size ", viewportSize, "/",
                        StretchShrink, " stretch shrink)");
                    //	" Viewport rect: ", margin, " ", viewportSize);
                    // Size override:", (screen_size / stretch_shrink).floor(), "(Screen size ", screen_size,"/",_stretchShrink," stretch shrink)")
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