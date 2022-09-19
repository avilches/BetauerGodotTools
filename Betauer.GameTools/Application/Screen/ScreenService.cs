using System;
using System.Collections.Generic;
using System.Linq;
using Betauer.Memory;
using Godot;
using static Betauer.Application.Screen.AspectRatios;
using static Betauer.Application.Screen.Resolutions;

namespace Betauer.Application.Screen {
    public class ScreenConfiguration {
        public Resolution DownScaledMinimumResolution { get; }
        public Resolution BaseResolution { get; }
        public List<Resolution> Resolutions { get; }
        public List<AspectRatio> AspectRatios { get; }

        // disabled: while the framebuffer will be resized to match the game window, nothing will be upscaled or downscaled (this includes GUIs).
        // 2d: the framebuffer is still resized, but GUIs can be upscaled or downscaled. This can result in blurry or pixelated fonts.
        // viewport: the framebuffer is resized, but computed at the original size of the project. The whole rendering will be pixelated. You generally do not want this, unless it's part of the game style.
        public SceneTree.StretchMode StretchMode { get; }
        public SceneTree.StretchAspect StretchAspect { get; }
        public float Zoom { get; }

        public ScreenConfiguration(Resolution downScaledMinimumResolution, Resolution baseResolution, SceneTree.StretchMode stretchMode,
            SceneTree.StretchAspect stretchAspect, List<Resolution>? resolutions = null, float zoom = 1f) {
            DownScaledMinimumResolution = downScaledMinimumResolution;
            BaseResolution = baseResolution;
            StretchMode = stretchMode;
            StretchAspect = stretchAspect;
            Resolutions = resolutions ?? GetAll(Ratio16_9);
            AspectRatios = Resolutions.Select(r => r.AspectRatio).Distinct().ToList();
            Zoom = zoom;
        }
    }

    public class ScreenService {
        private readonly SceneTree _tree;
        private readonly Dictionary<Strategy, IScreenService> _strategies = new Dictionary<Strategy, IScreenService>();
        private ScreenConfiguration _currentScreenConfiguration;
        private IScreenService? _currentService;
        private Strategy _currentStrategy = Strategy.FitToScreen;

        public enum Strategy {
            FitToScreen = 0,
            PixelPerfectScale = 1,
        }

        public ScreenService(SceneTree tree, ScreenConfiguration initialScreenConfiguration, Strategy? strategy = Strategy.FitToScreen) {
            _tree = tree;
            _strategies[Strategy.FitToScreen] = new FitToScreenResolutionService(_tree);
            _strategies[Strategy.PixelPerfectScale] = new PixelPerfectScreenResolutionService(_tree);
            SetScreenConfiguration(initialScreenConfiguration, strategy);
        }

        public void SetScreenConfiguration(ScreenConfiguration screenConfiguration, Strategy? strategy = null) {
            if (screenConfiguration == null) throw new ArgumentNullException(nameof(screenConfiguration));
            if (_currentScreenConfiguration != screenConfiguration || (strategy.HasValue && strategy.Value != strategy)) {
                _currentScreenConfiguration = screenConfiguration;
                // Enforce minimum resolution.
                OS.MinWindowSize = _currentScreenConfiguration.DownScaledMinimumResolution.Size;
                if (OS.WindowSize < OS.MinWindowSize) {
                    OS.WindowSize = OS.MinWindowSize;
                }
                _currentStrategy = strategy ?? _currentStrategy;
                ReconfigureService();
            }
        }

        public void SetStrategy(Strategy strategy) {
            if (_currentStrategy != strategy) {
                _currentStrategy = strategy;
                ReconfigureService();
            }
        }

        private void ReconfigureService() {
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
        protected Resolution DownScaledMinimumResolution => ScreenConfiguration.DownScaledMinimumResolution;
        protected Resolution BaseResolution => ScreenConfiguration.BaseResolution;

        protected List<Resolution> Resolutions => ScreenConfiguration.Resolutions;
        protected List<AspectRatio> AspectRatios => ScreenConfiguration.AspectRatios;
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
            if (resolution.x < DownScaledMinimumResolution.x || resolution.y < DownScaledMinimumResolution.y) {
                DoSetWindowed(DownScaledMinimumResolution);
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
}