using System;
using System.Collections.Generic;
using Betauer.Signal;
using Godot;

namespace Betauer.Application.Screen {
    public class ScreenService {
        private readonly SceneTree _tree;

        private readonly Dictionary<ScreenStrategyKey, IScreenStrategy> _strategies = new();
        public IScreenStrategy? ScreenStrategyImpl { get; private set;  }
        public ScreenConfiguration ScreenConfiguration { get; private set; }
        public ScreenStrategyKey StrategyKey { get; private set; } = ScreenStrategyKey.ViewportSize;

        public enum ScreenStrategyKey {
            WindowSize = 0,
            ViewportSize = 1,
            IntegerScale = 2,
        }

        public ScreenService(SceneTree tree, ScreenConfiguration initialScreenConfiguration, ScreenStrategyKey? strategyKey = ScreenStrategyKey.ViewportSize) {
            _tree = tree;
            _strategies[ScreenStrategyKey.WindowSize] = new FixedViewportStrategy(_tree);
            _strategies[ScreenStrategyKey.ViewportSize] = new ResizeViewportStrategy(_tree);
            _strategies[ScreenStrategyKey.IntegerScale] = new ResizeIntegerScaledStrategy(_tree);
            SetScreenConfiguration(initialScreenConfiguration, strategyKey);
            tree.OnScreenResized(OnScreenResized);
        }

        private void OnScreenResized() {
            if (ScreenConfiguration.IsResizeable && ScreenStrategyImpl is IScreenResizeHandler handler) handler.OnScreenResized();
        }

        public void SetScreenConfiguration(ScreenConfiguration screenConfiguration, ScreenStrategyKey? strategyKey = null) {
            ScreenConfiguration = screenConfiguration ?? throw new ArgumentNullException(nameof(screenConfiguration));
            StrategyKey = strategyKey ?? StrategyKey;
            ReconfigureService();
        }

        public void SetStrategy(ScreenStrategyKey screenStrategyKey) {
            if (StrategyKey != screenStrategyKey) {
                StrategyKey = screenStrategyKey;
                ReconfigureService();
            }
        }

        private void ReconfigureService() {
            ScreenStrategyImpl?.Disable(); // can be null, but only the first time
            ScreenStrategyImpl = _strategies[StrategyKey];
            ScreenStrategyImpl.SetScreenConfiguration(ScreenConfiguration);
            
            OS.WindowResizable = ScreenConfiguration.IsResizeable;
        }
        
        public string GetStateAsString() => ScreenStrategyImpl.GetStateAsString();

        public bool IsFullscreen() => ScreenStrategyImpl.IsFullscreen();
        public void SetFullscreen() => ScreenStrategyImpl.SetFullscreen();
        public void SetBorderless(bool borderless) => ScreenStrategyImpl.SetBorderless(borderless);
        public void SetWindowed(Resolution resolution) => ScreenStrategyImpl.SetWindowed(resolution);
        public List<ScaledResolution> GetResolutions() => ScreenStrategyImpl.GetResolutions();

        public void CenterWindow() => ScreenStrategyImpl.CenterWindow();
    }

    public interface IScreenResizeHandler {
        void OnScreenResized();
    }
}