using Betauer.Screen;
using Godot;

namespace Betauer.Managers {
    public interface IUserScreenSettings  {
        public bool PixelPerfect { get; }
        public bool Fullscreen { get; }
        public bool VSync { get; }
        public bool Borderless { get; }
        public Resolution WindowedResolution { get; }
    }

    public class UserSettingsFile : IUserScreenSettings {
        public const string Filename = "settings.cfg";
        public const string VideoSection = "Video";
        public const string PixelPerfectProperty = "PixelPerfect";
        public const string FullscreenProperty = "Fullscreen";
        public const string VSyncProperty = "VSync";
        public const string BorderlessProperty = "Borderless";
        public const string WindowedResolutionProperty = "WindowedResolution";

        public bool PixelPerfect { get; internal set; }
        public bool Fullscreen { get; internal set; }
        public bool VSync { get; internal set; }
        public bool Borderless { get; internal set; }
        public Resolution WindowedResolution { get; internal set; }
        public string ResourceName { get; }

        private readonly ConfigFile _cf = new ConfigFile();

        private readonly IUserScreenSettings _defaults;
        public UserSettingsFile(IUserScreenSettings defaults) {
            ResourceName = System.IO.Path.Combine(OS.GetUserDataDir(), System.IO.Path.GetFileName(Filename));
            _defaults = defaults;
            PixelPerfect = defaults.PixelPerfect; 
            Fullscreen = defaults.Fullscreen;
            VSync = defaults.VSync;
            Borderless = defaults.Borderless;
            WindowedResolution = defaults.WindowedResolution;
        }

        public UserSettingsFile Load() {
            var error = _cf.Load(ResourceName);
            if (error != Error.Ok) {
                LoggerFactory.GetLogger(typeof(UserSettingsFile)).Error($"Load \"{ResourceName}\" error: {error}");
                return this;
            }
            PixelPerfect = (bool)_cf.GetValue(VideoSection, PixelPerfectProperty, _defaults.PixelPerfect);
            Fullscreen = (bool)_cf.GetValue(VideoSection, FullscreenProperty, _defaults.Fullscreen);
            VSync = (bool)_cf.GetValue(VideoSection, VSyncProperty, _defaults.VSync);
            Borderless = (bool)_cf.GetValue(VideoSection, BorderlessProperty, _defaults.Borderless);
            var sn = (Vector2)_cf.GetValue(VideoSection, WindowedResolutionProperty, _defaults.WindowedResolution.Size);
            WindowedResolution = new Resolution(sn);
            return this;
        }

        public UserSettingsFile Save() {
            _cf.SetValue(VideoSection, PixelPerfectProperty, PixelPerfect);
            _cf.SetValue(VideoSection, FullscreenProperty, Fullscreen);
            _cf.SetValue(VideoSection, VSyncProperty, VSync);
            _cf.SetValue(VideoSection, BorderlessProperty, Borderless);
            _cf.SetValue(VideoSection, WindowedResolutionProperty, WindowedResolution.Size);
            var error = _cf.Save(ResourceName);
            if (error != Error.Ok) {
                LoggerFactory.GetLogger(typeof(UserSettingsFile)).Error($"Save \"{ResourceName}\" error: {error}");
            }
            return this;
        }
    }
}