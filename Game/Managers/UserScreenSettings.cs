using Betauer;
using Betauer.DI;
using Betauer.Screen;
using Godot;

namespace Veronenger.Game.Managers {
    public interface IUserScreenSettings  {
        public bool PixelPerfect { get; }
        public bool Fullscreen { get; }
        public bool VSync { get; }
        public bool Borderless { get; }
        public Resolution WindowedResolution { get; }
        public string ResourceName { get; }
    }

    public class UserScreenSettings : ApplicationConfig.UserScreenSettings, IUserScreenSettings {
        public bool PixelPerfect { get; internal set; } = DefaultPixelPerfect;
        public bool Fullscreen { get; internal set; } = DefaultFullscreen;
        public bool VSync { get; internal set; } = DefaultVSync;
        public bool Borderless { get; internal set; } = DefaultBorderless;
        public Resolution WindowedResolution { get; internal set; } = new Resolution(DefaultWindowedResolution);
        public string ResourceName { get; }

        private readonly ConfigFile _cf = new ConfigFile();

        public UserScreenSettings() {
            ResourceName = System.IO.Path.Combine(OS.GetUserDataDir(), System.IO.Path.GetFileName(Filename));
            // GD.Print("UserScreenSettings: "+_resourceName);
        }

        public UserScreenSettings Load() {
            var error = _cf.Load(ResourceName);
            if (error != Error.Ok) {
                LoggerFactory.GetLogger(typeof(UserScreenSettings)).Error($"Load \"{ResourceName}\" error: {error}");
                return this;
            }
            PixelPerfect = (bool)_cf.GetValue(VideoSection, PixelPerfectProperty, DefaultPixelPerfect);
            Fullscreen = (bool)_cf.GetValue(VideoSection, FullscreenProperty, DefaultFullscreen);
            VSync = (bool)_cf.GetValue(VideoSection, VSyncProperty, DefaultVSync);
            Borderless = (bool)_cf.GetValue(VideoSection, BorderlessProperty, DefaultBorderless);
            var sn = (Vector2)_cf.GetValue(VideoSection, WindowedResolutionProperty, DefaultWindowedResolution);
            WindowedResolution = new Resolution(sn);
            return this;
        }

        public UserScreenSettings Save() {
            _cf.SetValue(VideoSection, PixelPerfectProperty, PixelPerfect);
            _cf.SetValue(VideoSection, FullscreenProperty, Fullscreen);
            _cf.SetValue(VideoSection, VSyncProperty, VSync);
            _cf.SetValue(VideoSection, BorderlessProperty, Borderless);
            _cf.SetValue(VideoSection, WindowedResolutionProperty, WindowedResolution.Size);
            var error = _cf.Save(ResourceName);
            if (error != Error.Ok) {
                LoggerFactory.GetLogger(typeof(UserScreenSettings)).Error($"Save \"{ResourceName}\" error: {error}");
            }
            return this;
        }
    }
}