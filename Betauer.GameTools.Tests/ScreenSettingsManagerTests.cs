using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Betauer.Application.Screen;
using Betauer.Application.Settings;
using Betauer.DI;
using Betauer.Input;
using Betauer.TestRunner;
using Godot;
using NUnit.Framework;

namespace Betauer.GameTools.Tests {
    [TestFixture]
    public class ScreenSettingsManagerTests : Node {

        const string SettingsFile = "./test-screen-settings.ini";

        [SetUp]
        [TearDown]
        public void Clear() {
            System.IO.File.Delete(SettingsFile);
        }

        internal static ScreenConfiguration InitialScreenConfiguration =
            new ScreenConfiguration(
                Resolutions.FULLHD,
                SceneTree.StretchMode.Mode2d, 
                SceneTree.StretchAspect.KeepHeight);

        [Configuration]
        internal class ScreenSettingsManagerConfig {
            [Service] private ScreenSettingsManager ScreenSettingsManager => 
                new ScreenSettingsManager(InitialScreenConfiguration);
        }

        [Configuration]
        internal class ScreenSettingsSavedConfig {
            [Service]
            public SettingsContainer SettingsContainer => new SettingsContainer(SettingsFile);

            // [Setting(Section = "Video", Name = "PixelPerfect", Default = false)]
            [Service("Settings.Screen.PixelPerfect")]
            public ISetting<bool> PixelPerfect => Setting<bool>.Save("Video", "PixelPerfect", false);

            // [Setting(Section = "Video", Name = "Fullscreen", Default = true)]
            [Service("Settings.Screen.Fullscreen")]
            public ISetting<bool> Fullscreen => Setting<bool>.Save("Video", "Fullscreen", true);

            // [Setting(Section = "Video", Name = "VSync", Default = false)]
            [Service("Settings.Screen.VSync")]
            public ISetting<bool> VSync => Setting<bool>.Save("Video", "VSync", false);

            // [Setting(Section = "Video", Name = "Borderless", Default = false)]
            [Service("Settings.Screen.Borderless")]
            public ISetting<bool> Borderless => Setting<bool>.Save("Video", "Borderless", false);

            // [Setting(Section = "Video", Name = "WindowedResolution")]
            [Service("Settings.Screen.WindowedResolution")]
            public ISetting<Resolution> WindowedResolution =>
                Setting<Resolution>.Save("Video", "WindowedResolution", Resolutions.FULLHD_DIV3);
        }

        [Test]
        public void SaveSettingTest() {
            var di = new ContainerBuilder(this);
            di.Static(GetTree());
            di.Scan<ScreenSettingsManagerConfig>();
            di.Scan<ScreenSettingsSavedConfig>();
            var c = di.Build();
            var s = c.Resolve<ScreenSettingsManager>();

            Assert.That(s.Fullscreen, Is.True);
            Assert.That(s.PixelPerfect, Is.False);
            Assert.That(s.VSync, Is.False);
            Assert.That(s.WindowedResolution, Is.EqualTo(Resolutions.FULLHD_DIV3));
            
            s.Setup();
            s.SetFullscreen(false);
            s.SetPixelPerfect(true);
            s.SetVSync(true);
            s.SetWindowed(Resolutions.WXGA);
            
            Assert.That(s.Fullscreen, Is.False);
            Assert.That(s.PixelPerfect, Is.True);
            Assert.That(s.VSync, Is.True);
            Assert.That(s.WindowedResolution, Is.EqualTo(Resolutions.WXGA));
            var cf = new ConfigFile();
            cf.Load(SettingsFile);
            Assert.That(cf.GetValue("Video", "Fullscreen", "X"), Is.False);
            Assert.That(cf.GetValue("Video", "PixelPerfect", "X"), Is.True);
            Assert.That(cf.GetValue("Video", "VSync", "X"), Is.True);
            Assert.That(cf.GetValue("Video", "WindowedResolution", "X"), Is.EqualTo(Resolutions.WXGA.Size));
            
            s.SetFullscreen(true, false);
            s.SetPixelPerfect(false, false);
            s.SetVSync(false, false);
            s.SetWindowed(Resolutions.FULLHD_DIV1_875, false);
            
            Assert.That(s.Fullscreen, Is.False);
            Assert.That(s.PixelPerfect, Is.True);
            Assert.That(s.VSync, Is.True);
            Assert.That(s.WindowedResolution, Is.EqualTo(Resolutions.WXGA));
            cf.Load(SettingsFile);
            Assert.That(cf.GetValue("Video", "Fullscreen", "X"), Is.False);
            Assert.That(cf.GetValue("Video", "PixelPerfect", "X"), Is.True);
            Assert.That(cf.GetValue("Video", "VSync", "X"), Is.True);
            Assert.That(cf.GetValue("Video", "WindowedResolution", "X"), Is.EqualTo(Resolutions.WXGA.Size));
        }
        
        [Test]
        public void MemorySettingTest() {
            var di = new ContainerBuilder(this);
            di.Static(GetTree());
            di.Scan<ScreenSettingsManagerConfig>();
            var c = di.Build();
            var s = c.Resolve<ScreenSettingsManager>();

            Assert.That(s.Fullscreen, Is.False);
            Assert.That(s.PixelPerfect, Is.False);
            Assert.That(s.VSync, Is.True);
            Assert.That(s.WindowedResolution, Is.EqualTo(InitialScreenConfiguration.BaseResolution));
            
            s.Setup();
            
            s.SetFullscreen(true);
            s.SetPixelPerfect(true);
            s.SetVSync(false);
            s.SetWindowed(Resolutions.WXGA);
            
            Assert.That(s.Fullscreen, Is.True);
            Assert.That(s.PixelPerfect, Is.True);
            Assert.That(s.VSync, Is.False);
            Assert.That(s.WindowedResolution, Is.EqualTo(Resolutions.WXGA));
        }

     
    }
}