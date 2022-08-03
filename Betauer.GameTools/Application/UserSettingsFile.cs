using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Reflection;
using Betauer.Application.Screen;
using Betauer.Input;
using Godot;
using File = System.IO.File;

namespace Betauer.Application {
    public interface IUserSettings {
        public bool PixelPerfect { get; }
        public bool Fullscreen { get; }
        public bool VSync { get; }
        public bool Borderless { get; }
        public Resolution WindowedResolution { get; }
        public string SettingsPathFile { get;  }
    }

    public class SettingsFile : IUserSettings {

        public const string ControlsSection = "Controls";

        // [Setting(Section = "Video", Name = "PixelPerfect", Default = false)]
        public bool PixelPerfect { get; internal set; }
        
        // [Setting(Section = "Video", Name = "Fullscreen", Default = true)]
        public bool Fullscreen { get; internal set; }
        
        // [Setting(Section = "Video", Name = "VSync", Default = false)]
        public bool VSync { get; internal set; }
        
        // [Setting(Section = "Video", Name = "Borderless", Default = false)]
        public bool Borderless { get; internal set; }

        // [Setting(Section = "Video", Name = "WindowedResolution")]
        public Resolution WindowedResolution { get; internal set; }
        
        public ICollection<ActionState> ActionList { get; }
        public string SettingsPathFile { get; }

        private readonly IUserSettings _defaults;
        private readonly ConfigFileWrapper _cf;
        private static readonly Logger Logger = LoggerFactory.GetLogger(typeof(SettingsFile));
        
        public SettingsFile(IUserSettings defaults, ICollection<ActionState> actionList) {
            SettingsPathFile = defaults.SettingsPathFile;
            _cf = new ConfigFileWrapper().SetFilePath(SettingsPathFile);
            _defaults = defaults;
            PixelPerfect = defaults.PixelPerfect;
            Fullscreen = defaults.Fullscreen;
            VSync = defaults.VSync;
            Borderless = defaults.Borderless;
            WindowedResolution = defaults.WindowedResolution;
            ActionList = actionList;
        }

        public SettingsFile Load() {
            if (Logger.IsEnabled(TraceLevel.Debug)) {
                var content = File.Exists(SettingsPathFile) ? File.ReadAllText(SettingsPathFile) : "(file not found)";
                Logger.Debug("Loading from " + SettingsPathFile + "\n" + content);
            }
            _cf.Load();
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
            ActionList.ToList().ForEach(actionState => {
                _cf.SetValue(ControlsSection, actionState.GetPropertyNameForKeys(), actionState.ExportKeys());
                _cf.SetValue(ControlsSection, actionState.GetPropertyNameForButtons(), actionState.ExportButtons());
            });
            _cf.Save();
            if (Logger.IsEnabled(TraceLevel.Debug)) {
                Logger.Debug("Saving to " + SettingsPathFile + "\n" + File.ReadAllText(SettingsPathFile));
            }
            return this;
        }
    }
}