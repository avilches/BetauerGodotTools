using System;
using System.Collections.Generic;
using System.Linq;
using Betauer.Memory;
using Betauer.SignalHandler;
using Godot;

namespace Betauer.Application.Screen {
    public class ScreenConfiguration {
        public Resolution BaseResolution { get; }
        public List<Resolution> Resolutions { get; }

        // disabled: while the framebuffer will be resized to match the game window, nothing will be upscaled or downscaled (this includes GUIs).
        // 2d: the framebuffer is still resized, but GUIs can be upscaled or downscaled. This can result in blurry or pixelated fonts.
        // viewport: the framebuffer is resized, but computed at the original size of the project. The whole rendering will be pixelated. You generally do not want this, unless it's part of the game style.
        public SceneTree.StretchMode StretchMode { get; }
        public SceneTree.StretchAspect StretchAspect { get; }
        public float Zoom { get; }

        public ScreenConfiguration(Resolution baseResolution, SceneTree.StretchMode stretchMode,
            SceneTree.StretchAspect stretchAspect, List<Resolution>? resolutions = null, float zoom = 1f) {
            BaseResolution = baseResolution;
            StretchMode = stretchMode;
            StretchAspect = stretchAspect;
            Resolutions = resolutions ?? Application.Screen.Resolutions.All(AspectRatios.Ratio16_9);
            Zoom = zoom;
        }
    }

    public class ScreenService {
        private readonly SceneTree _tree;
        private readonly Dictionary<Strategy, IScreenService> _strategies = new Dictionary<Strategy, IScreenService>();
        private ScreenConfiguration _currentScreenConfiguration;
        private IScreenService? _currentService;
        private Strategy _currentStrategy = Strategy.Unknown;

        public enum Strategy {
            Unknown = 0,
            PixelPerfectScale = 1,
            FitToScreen = 2,
        }

        public ScreenService(SceneTree tree, ScreenConfiguration currentScreenConfiguration, Strategy? strategy = null) {
            _tree = tree;
            _currentScreenConfiguration = currentScreenConfiguration;
            _strategies[Strategy.FitToScreen] = new FitToScreenResolutionService(_tree);
            _strategies[Strategy.PixelPerfectScale] = new PixelPerfectScreenResolutionService(_tree);
            if (strategy.HasValue) SetStrategy(strategy.Value);
        }

        public void SetScreenConfiguration(ScreenConfiguration screenConfiguration, Strategy? strategy = null) {
            if (_currentScreenConfiguration != screenConfiguration || (strategy.HasValue && strategy.Value != strategy)) {
                _currentScreenConfiguration = screenConfiguration;
                _currentStrategy = strategy ?? _currentStrategy;
                ConfigureService();
            }
        }

        public void SetStrategy(Strategy strategy) {
            if (_currentStrategy != strategy) {
                _currentStrategy = strategy;
                ConfigureService();
            }
        }

        private void ConfigureService() {
            _currentService?.Disable(); // can be null, but only the first time
            _currentService = _strategies[_currentStrategy];
            _currentService.Enable(_currentScreenConfiguration);
        }

        public void Dispose() => _currentService?.Dispose();

        public bool IsFullscreen() => _currentService.IsFullscreen();
        public void SetFullscreen() => _currentService.SetFullscreen();
        public void SetBorderless(bool borderless) => _currentService.SetBorderless(borderless);
        public void SetWindowed(Resolution resolution) => _currentService.SetWindowed(resolution);
        public List<ScaledResolution> GetResolutions() => _currentService.GetResolutions();

        public void CenterWindow() => _currentService.CenterWindow();
    }

    public interface IScreenService : IDisposable {
        bool IsFullscreen();
        void SetFullscreen();
        void SetBorderless(bool borderless);
        void SetWindowed(Resolution resolution);
        List<ScaledResolution> GetResolutions();
        void Disable();
        void Enable(ScreenConfiguration screenConfiguration);
        void CenterWindow();
    }

    public abstract class BaseScreenResolutionService : DisposableObject {
        protected readonly SceneTree Tree;
        protected ScreenConfiguration ScreenConfiguration;
        protected Resolution BaseResolution => ScreenConfiguration.BaseResolution;

        protected List<Resolution> Resolutions => ScreenConfiguration.Resolutions;

        // protected ICollection<AspectRatio> AspectRatios => _screenConfiguration.AspectRatios;
        protected SceneTree.StretchMode StretchMode => ScreenConfiguration.StretchMode;
        protected SceneTree.StretchAspect StretchAspect => ScreenConfiguration.StretchAspect;
        protected float Zoom => ScreenConfiguration.Zoom;

        protected BaseScreenResolutionService(SceneTree tree) {
            Tree = tree;
        }

        public bool IsFullscreen() => OS.WindowFullscreen;
        public abstract void SetFullscreen();

        public void SetBorderless(bool borderless) {
            if (IsFullscreen()) return;
            DoSetBorderless(borderless);
        }

        protected abstract void DoSetBorderless(bool borderless);

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

        protected abstract void DoSetWindowed(Resolution resolution);


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

    public class FitToScreenResolutionService : BaseScreenResolutionService, IScreenService {
        private static readonly Logger Logger = LoggerFactory.GetLogger(typeof(FitToScreenResolutionService));

        public FitToScreenResolutionService(SceneTree tree) : base(tree) {
        }

        public void Enable(ScreenConfiguration screenConfiguration) {
            ScreenConfiguration = screenConfiguration;
            // Enforce minimum resolution.
            OS.MinWindowSize = BaseResolution.Size;
            if (OS.WindowSize < OS.MinWindowSize) {
                OS.WindowSize = OS.MinWindowSize;
            }
            Tree.SetScreenStretch(StretchMode, StretchAspect, BaseResolution.Size, Zoom);
        }

        public void Disable() {
        }

        protected override void OnDispose(bool disposing) {
        }

        public List<ScaledResolution> GetResolutions() {
            var screenSize = OS.GetScreenSize();
            return (from resolution in Resolutions ?? Application.Screen.Resolutions.All()
                where resolution.x <= screenSize.x && resolution.y <= screenSize.y &&
                      resolution.x >= BaseResolution.x && resolution.y >= BaseResolution.y
                select new ScaledResolution(BaseResolution.Size, resolution.Size)).ToList();
        }

        public override void SetFullscreen() {
            if (!OS.WindowFullscreen) {
                OS.WindowBorderless = false;
                OS.WindowFullscreen = true;
            }
        }

        protected override void DoSetBorderless(bool borderless) {
            if (OS.WindowBorderless == borderless) return;
            OS.WindowBorderless = borderless;
        }

        protected override void DoSetWindowed(Resolution resolution) {
            if (OS.WindowFullscreen) OS.WindowFullscreen = false;
            OS.WindowSize = resolution.Size;
            CenterWindow();
        }
    }

    /**
     * https://github.com/Yukitty/godot-addon-integer_resolution_handler
     */
    public class PixelPerfectScreenResolutionService : BaseScreenResolutionService, IScreenService {
        private static readonly Logger Logger = LoggerFactory.GetLogger(typeof(PixelPerfectScreenResolutionService));
        private readonly SignalHandlerAction _signalHandlerAction;
        private bool _enabled = false;

        public PixelPerfectScreenResolutionService(SceneTree tree) : base(tree) {
            _signalHandlerAction = Tree.OnScreenResized(SignalHandler);
        }

        public void SignalHandler() {
            if (_enabled) ScaleResolutionViewport();
        }
        
        protected override void OnDispose(bool disposing) {
            _signalHandlerAction.Free();
        }

        public void Enable(ScreenConfiguration screenConfiguration) {
            ScreenConfiguration = screenConfiguration;
            // Enforce minimum resolution.
            OS.MinWindowSize = BaseResolution.Size;
            if (OS.WindowSize < OS.MinWindowSize) {
                OS.WindowSize = OS.MinWindowSize;
            }
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
            var screenSize = OS.GetScreenSize();
            var maxScale = Resolution.CalculateMaxScale(screenSize, BaseResolution.Size);
            List<ScaledResolution> resolutions = new List<ScaledResolution>();
            HashSet<AspectRatio> otherAspectRatios = new HashSet<AspectRatio>();
            // Loop all the possible resolution just to extract all the aspect ratios...
            foreach (var resolution in Resolutions.Where(resolution => !resolution.AspectRatio.Matches(BaseResolution))) {
                otherAspectRatios.Add(resolution.AspectRatio);
            }
            for (var scale = 1; scale <= maxScale; scale++) {
                var scaledResolution = new ScaledResolution(BaseResolution.Size, BaseResolution.Size * scale);
                // Add the baseResolution x scale
                resolutions.Add(scaledResolution);
                // And add it again adapted to other aspect ratios
                foreach (var aspectRatio in otherAspectRatios) {
                    // TODO: This is only with landscapes
                    if (aspectRatio.Ratio > scaledResolution.AspectRatio.Ratio) {
                        // Convert the resolution to a wider aspect ratio, keeping the height and adding more width
                        // So, if base is 1920x1080 = 1,77777 16:9
                        // to 21:9 => 2,3333
                        // x=1080*2,333=2520
                        // 2520x1080 = 2,3333 21:9
                        var newWidth = scaledResolution.y * aspectRatio.Ratio;
                        if (newWidth <= screenSize.x) {
                            var newResolution = new Vector2(newWidth, scaledResolution.y);
                            var scaledResolutionUpdated = new ScaledResolution(BaseResolution.Size, newResolution);
                            resolutions.Add(scaledResolutionUpdated);
                        }
                    } else {
                        // Convert the resolution to a stretcher aspect ratio, keeping the width and adding more height
                        // So, if base is 1920x1080 = 1,77777 16:9
                        // to 4:3 => 1,333
                        // y=1920/1,333=823
                        // 1920x1440 = 1,3333 3:4
                        var newHeight = scaledResolution.x / aspectRatio.Ratio;
                        if (newHeight <= screenSize.y) {
                            var newResolution = new Vector2(scaledResolution.x, newHeight);
                            var scaledResolutionUpdated = new ScaledResolution(BaseResolution.Size, newResolution);
                            resolutions.Add(scaledResolutionUpdated);
                        }
                    }
                }
            }
            return resolutions;
        }

        public override void SetFullscreen() {
            if (!OS.WindowFullscreen) {
                OS.WindowBorderless = false;
                OS.WindowFullscreen = true;
            }
            ScaleResolutionViewport();
        }

        protected override void DoSetBorderless(bool borderless) {
            if (OS.WindowBorderless == borderless) return;
            OS.WindowBorderless = borderless;
            ScaleResolutionViewport();
        }

        protected override void DoSetWindowed(Resolution resolution) {
            if (OS.WindowFullscreen) OS.WindowFullscreen = false;
            OS.WindowSize = resolution.Size;
            CenterWindow();
            ScaleResolutionViewport();
        }

        private void ScaleResolutionViewport() {
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