using Godot;
using Veronenger.Game.Tools;
using static Godot.Mathf;

// https://github.com/Yukitty/godot-addon-integer_resolution_handler
namespace Veronenger.Game.Tools.Resolution {
    public class ScreenManager {
        private readonly Vector2 _baseResolution;

        // disabled: while the framebuffer will be resized to match the game window, nothing will be upscaled or downscaled (this includes GUIs).
        // 2d: the framebuffer is still resized, but GUIs can be upscaled or downscaled. This can result in blurry or pixelated fonts.
        // viewport: the framebuffer is resized, but computed at the original size of the project. The whole rendering will be pixelated. You generally do not want this, unless it's part of the game style.
        private readonly SceneTree.StretchMode _stretchMode;
        private readonly SceneTree.StretchAspect _stretchAspect;
        private readonly int _stretchShrink;

        private SceneTree _tree;
        private Viewport _root;
        private int _maxScale;

        public ScreenManager(Vector2 baseResolution, SceneTree.StretchMode stretchMode,
            SceneTree.StretchAspect stretchAspect, int stretchShrink = 1) {
            _baseResolution = baseResolution;
            _stretchMode = stretchMode;
            _stretchAspect = stretchAspect;
            _stretchShrink = stretchShrink;
        }

        public void Start(GameManager gameManager, string gameManagerUpdateResolutionMethodName) {
            _tree = gameManager.GetTree();
            _root = gameManager.GetNode<Viewport>("/root");

            // Enforce minimum resolution.
            OS.MinWindowSize = _baseResolution;
            // Remove default stretch behavior.
            _tree.SetScreenStretch(SceneTree.StretchMode.Disabled, SceneTree.StretchAspect.Keep,
                _baseResolution, 1);

            var screenSize = OS.GetScreenSize();
            _maxScale = (int) Max(Floor(Min(screenSize.x / _baseResolution.x, screenSize.y / _baseResolution.y)), 1);
            _tree.Connect("screen_resized", gameManager, gameManagerUpdateResolutionMethodName);
        }

        public bool IsFullscreen() => OS.WindowFullscreen;

        public void SwapFullscreen() {
            SetFullscreen(!IsFullscreen());
        }

        public void SetFullscreen(bool fs) {
            OS.WindowFullscreen = fs;
            CenterWindow();
            UpdateResolution();
        }

        public void SetScale(float scale) {
            OS.WindowSize = _baseResolution * Min(_maxScale, scale);
            CenterWindow();
            UpdateResolution();
        }

        public void SetBorderless(bool borderless) {
            OS.WindowBorderless = borderless;
            UpdateResolution();
        }

        public void SetAll(bool fs, int scale, bool borderless) {
            if (fs) {
                OS.WindowFullscreen = true;
            } else {
                OS.WindowFullscreen = false;
                OS.WindowBorderless = borderless;
                SetScale(scale);
            }
        }

        private static void CenterWindow() {
            if (!OS.WindowFullscreen) {
                var screen_size = OS.GetScreenSize(OS.CurrentScreen);
                var window_size = OS.WindowSize;
                var centered_pos = (screen_size - window_size) / 2;
                OS.WindowPosition = centered_pos;
            }
        }

        public void UpdateResolution() {
            var windowSize = OS.WindowFullscreen ? OS.GetScreenSize() : OS.WindowSize;
            var scale = (int) Max(Floor(Min(windowSize.x / _baseResolution.x, windowSize.y / _baseResolution.y)), 1);

            var screenSize = _baseResolution;
            var viewportSize = screenSize * scale;
            var overscan = ((windowSize - viewportSize) / scale).Floor();

            switch (_stretchAspect) {
                case SceneTree.StretchAspect.KeepWidth: {
                    screenSize.y += overscan.y;
                    break;
                }
                case SceneTree.StretchAspect.KeepHeight: {
                    screenSize.x += overscan.x;
                    break;
                }
                case SceneTree.StretchAspect.Expand:
                case SceneTree.StretchAspect.Ignore: {
                    screenSize += overscan;
                    break;
                }
            }

            viewportSize = screenSize * scale;
            var margin = (windowSize - viewportSize) / 2;
            var margin2 = margin.Ceil();
            margin = margin.Floor();

            ChangeViewport(screenSize, margin, viewportSize, windowSize, margin2);
        }

        private void ChangeViewport(Vector2 screenSize, Vector2 margin, Vector2 viewportSize, Vector2 windowSize,
            Vector2 margin2) {
            switch (_stretchMode) {
                case SceneTree.StretchMode.Viewport: {
                    _root.Size = (screenSize / _stretchShrink).Floor();
                    _root.SetAttachToScreenRect(new Rect2(margin, viewportSize));
                    _root.SizeOverrideStretch = false;
                    _root.SetSizeOverride(false);
                    if (Debug.DEBUG_RESOLUTION) {
                        GD.Print("(Viewport Mode) Base resolution:", _baseResolution.x, "x", _baseResolution.y,
                            " Video resolution:", windowSize.x, "x", windowSize.y,
                            " Size:", (screenSize / _stretchShrink).Floor(), "(Screen size ", screenSize, "/",
                            _stretchShrink, " stretch shrink)");
                    }

                    break;
                }
                case SceneTree.StretchMode.Mode2d:
                case SceneTree.StretchMode.Disabled: {
                    _root.Size = (viewportSize / _stretchShrink).Floor();
                    _root.SetAttachToScreenRect(new Rect2(margin, viewportSize));
                    _root.SizeOverrideStretch = true;
                    _root.SetSizeOverride(true, (screenSize / _stretchShrink).Floor());
                    if (Debug.DEBUG_RESOLUTION) {
                        GD.Print("(2D model) Base resolution:", _baseResolution.x, "x", _baseResolution.y,
                            " Video resolution:", windowSize.x, "x", windowSize.y,
                            " Size:", (viewportSize / _stretchShrink).Floor(), " (Viewport size ", viewportSize, "/",
                            _stretchShrink, " stretch shrink)");
                        //	" Viewport rect: ", margin, " ", viewportSize);
                        // Size override:", (screen_size / stretch_shrink).floor(), "(Screen size ", screen_size,"/",_stretchShrink," stretch shrink)")
                    }

                    break;
                }
            }

            VisualServer.BlackBarsSetMargins(
                Max(0, (int) margin.x),
                Max(0, (int) margin.y),
                Max(0, (int) margin2.x),
                Max(0, (int) margin2.y));
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