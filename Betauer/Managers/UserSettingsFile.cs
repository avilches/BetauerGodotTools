using System;
using System.Collections.Generic;
using System.Linq;
using Betauer.Input;
using Betauer.Screen;
using Godot;
using File = System.IO.File;

namespace Betauer.Managers {
    public interface IUserSettings {
        public bool PixelPerfect { get; }
        public bool Fullscreen { get; }
        public bool VSync { get; }
        public bool Borderless { get; }
        public Resolution WindowedResolution { get; }
    }

    public class SettingsFile : IUserSettings {
        public const string Filename = "settings.cfg";

        public const string VideoSection = "Video";
        public const string PixelPerfectProperty = "PixelPerfect";
        public const string FullscreenProperty = "Fullscreen";
        public const string VSyncProperty = "VSync";
        public const string BorderlessProperty = "Borderless";
        public const string WindowedResolutionProperty = "WindowedResolution";

        public const string ControlsSection = "Controls";

        public bool PixelPerfect { get; internal set; }
        public bool Fullscreen { get; internal set; }
        public bool VSync { get; internal set; }
        public bool Borderless { get; internal set; }
        public Resolution WindowedResolution { get; internal set; }
        public ICollection<ActionState> ActionList { get; internal set; }

        private readonly string _resourceName;
        private readonly IUserSettings _defaults;
        private readonly Properties _cf;

        public SettingsFile(IUserSettings defaults, ICollection<ActionState> actionList) {
            _resourceName = System.IO.Path.Combine(OS.GetUserDataDir(), System.IO.Path.GetFileName(Filename));
            _cf = new Properties(_resourceName);
            _defaults = defaults;
            PixelPerfect = defaults.PixelPerfect;
            Fullscreen = defaults.Fullscreen;
            VSync = defaults.VSync;
            Borderless = defaults.Borderless;
            WindowedResolution = defaults.WindowedResolution;
            ActionList = actionList;
        }

        public SettingsFile Load() {
            Console.WriteLine("Loading from " + _resourceName + "\n" + File.ReadAllText(_resourceName));
            _cf.Load();
            PixelPerfect = _cf.GetValue(VideoSection, PixelPerfectProperty, _defaults.PixelPerfect);
            Fullscreen = _cf.GetValue(VideoSection, FullscreenProperty, _defaults.Fullscreen);
            VSync = _cf.GetValue(VideoSection, VSyncProperty, _defaults.VSync);
            Borderless = _cf.GetValue(VideoSection, BorderlessProperty, _defaults.Borderless);
            var sn = _cf.GetValue(VideoSection, WindowedResolutionProperty, _defaults.WindowedResolution.Size);
            WindowedResolution = new Resolution(sn);
            ActionList.ToList().ForEach(actionState => {
                var keys = _cf.GetValue(ControlsSection, actionState.GetPropertyNameForKeys(),
                    actionState.ExportKeys());
                var buttons = _cf.GetValue(ControlsSection, actionState.GetPropertyNameForButtons(),
                    actionState.ExportButtons());
                actionState.ImportKeys(keys);
                actionState.ImportButtons(buttons);
            });
            return this;
        }

        public SettingsFile Save() {
            _cf.SetValue(VideoSection, PixelPerfectProperty, PixelPerfect);
            _cf.SetValue(VideoSection, FullscreenProperty, Fullscreen);
            _cf.SetValue(VideoSection, VSyncProperty, VSync);
            _cf.SetValue(VideoSection, BorderlessProperty, Borderless);
            _cf.SetValue(VideoSection, WindowedResolutionProperty, WindowedResolution.Size);
            ActionList.ToList().ForEach(actionState => {
                _cf.SetValue(ControlsSection, actionState.GetPropertyNameForKeys(), actionState.ExportKeys());
                _cf.SetValue(ControlsSection, actionState.GetPropertyNameForButtons(), actionState.ExportButtons());
            });
            _cf.Save();
            Console.WriteLine("Saving to " + _resourceName + "\n" + File.ReadAllText(_resourceName));
            return this;
        }
    }
}