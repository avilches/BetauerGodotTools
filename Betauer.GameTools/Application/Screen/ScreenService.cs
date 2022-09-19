using System;
using System.Collections.Generic;
using Betauer.Signal;
using Godot;

namespace Betauer.Application.Screen {
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
            tree.OnScreenResized(OnScreenResized);
        }

        private void OnScreenResized() {
            if (_currentScreenConfiguration.IsResizeable && _currentService is IScreenResizeHandler handler) handler.OnScreenResized();
        }

        public void SetScreenConfiguration(ScreenConfiguration screenConfiguration, Strategy? strategy = null) {
            if (screenConfiguration == null) throw new ArgumentNullException(nameof(screenConfiguration));
            if (_currentScreenConfiguration != screenConfiguration || (strategy.HasValue && strategy.Value != strategy)) {
                _currentScreenConfiguration = screenConfiguration;
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
            
            // Enforce minimum resolution.
            OS.MinWindowSize = _currentScreenConfiguration.DownScaledMinimumResolution.Size;
            if (OS.WindowSize < OS.MinWindowSize) {
                OS.WindowSize = OS.MinWindowSize;
            }
            OS.WindowResizable = _currentScreenConfiguration.IsResizeable;
        }

        public bool IsFullscreen() => _currentService.IsFullscreen();
        public void SetFullscreen() => _currentService.SetFullscreen();
        public void SetBorderless(bool borderless) => _currentService.SetBorderless(borderless);
        public void SetWindowed(Resolution resolution) => _currentService.SetWindowed(resolution);
        public List<ScaledResolution> GetResolutions() => _currentService.GetResolutions();

        public void CenterWindow() => _currentService.CenterWindow();
    }

    public interface IScreenService {
        bool IsFullscreen();
        void SetFullscreen();
        void SetBorderless(bool borderless);
        void SetWindowed(Resolution resolution);
        List<ScaledResolution> GetResolutions();
        void Disable();
        void Enable(ScreenConfiguration screenConfiguration);
        void CenterWindow();
    }

    public interface IScreenResizeHandler {
        void OnScreenResized();
    }
}