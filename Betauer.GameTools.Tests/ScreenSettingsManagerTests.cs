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

        [Configuration]
        internal class ScreenSettingsManagerConfig {
            internal ScreenConfiguration InitialScreenConfiguration =
                new ScreenConfiguration(
                    Resolutions.FULLHD,
                    SceneTree.StretchMode.Mode2d, 
                    SceneTree.StretchAspect.KeepHeight);
            [Service] private ScreenSettingsManager ScreenSettingsManager => 
                new ScreenSettingsManager(InitialScreenConfiguration);
        }

        [Service]
        internal class Service1 {
            [Inject] public ScreenSettingsManager ScreenSettingsManager;
        }

        [Test]
        public void ScreenSettingsManagerNeedsSettingToGetTheDefaults() {
            var di = new ContainerBuilder(this);
            di.Static(GetTree());
            di.Scan<ScreenSettingsManagerConfig>();
            var e = Assert.Throws<KeyNotFoundException>(() => di.Build());
            Assert.That(e.Message.Contains("Service not found. Alias: Settings.Screen."), Is.True);
        }

        [Configuration]
        internal class ScreenSettingsConfig {
            [Service]
            public SettingsContainer SettingsContainer => new SettingsContainer(SettingsFile);

            // [Setting(Section = "Video", Name = "PixelPerfect", Default = false)]
            [Service("Settings.Screen.PixelPerfect")]
            public Setting<bool> PixelPerfect => new Setting<bool>("Video", "PixelPerfect", false);

            // [Setting(Section = "Video", Name = "Fullscreen", Default = true)]
            [Service("Settings.Screen.Fullscreen")]
            public Setting<bool> Fullscreen => new Setting<bool>("Video", "Fullscreen", true);

            // [Setting(Section = "Video", Name = "VSync", Default = false)]
            [Service("Settings.Screen.VSync")] public Setting<bool> VSync => new Setting<bool>("Video", "VSync", false);

            // [Setting(Section = "Video", Name = "Borderless", Default = false)]
            [Service("Settings.Screen.Borderless")]
            public Setting<bool> Borderless => new Setting<bool>("Video", "Borderless", false);

            // [Setting(Section = "Video", Name = "WindowedResolution")]
            [Service("Settings.Screen.WindowedResolution")]
            public Setting<Resolution> WindowedResolution =>
                new Setting<Resolution>("Video", "WindowedResolution", Resolutions.FULLHD_DIV3);
        }


        [Test]
        public void BasicTest() {
            var di = new ContainerBuilder(this);
            di.Static(GetTree());
            di.Scan<ScreenSettingsManagerConfig>();
            di.Scan<Service1>();
            di.Scan<ScreenSettingsConfig>();
            var c = di.Build();
            var s = c.Resolve<Service1>();

            Assert.That(s.ScreenSettingsManager.Fullscreen, Is.True);
            Assert.That(s.ScreenSettingsManager.PixelPerfect, Is.False);
            Assert.That(s.ScreenSettingsManager.VSync, Is.False);
            Assert.That(s.ScreenSettingsManager.WindowedResolution, Is.EqualTo(Resolutions.FULLHD_DIV3));
            
            s.ScreenSettingsManager.Setup();
            
            s.ScreenSettingsManager.SetFullscreen(false);
            s.ScreenSettingsManager.SetPixelPerfect(true);
            s.ScreenSettingsManager.SetVSync(true);
            s.ScreenSettingsManager.SetWindowed(Resolutions.WXGA);
            
            Assert.That(s.ScreenSettingsManager.Fullscreen, Is.False);
            Assert.That(s.ScreenSettingsManager.PixelPerfect, Is.True);
            Assert.That(s.ScreenSettingsManager.VSync, Is.True);
            Assert.That(s.ScreenSettingsManager.WindowedResolution, Is.EqualTo(Resolutions.WXGA));
            var cf = new ConfigFile();
            cf.Load(SettingsFile);
            Assert.That(cf.GetValue("Video", "Fullscreen", "X"), Is.False);
            Assert.That(cf.GetValue("Video", "PixelPerfect", "X"), Is.True);
            Assert.That(cf.GetValue("Video", "VSync", "X"), Is.True);
            Assert.That(cf.GetValue("Video", "WindowedResolution", "X"), Is.EqualTo(Resolutions.WXGA.Size));
            
            s.ScreenSettingsManager.SetFullscreen(true, false);
            s.ScreenSettingsManager.SetPixelPerfect(false, false);
            s.ScreenSettingsManager.SetVSync(false, false);
            s.ScreenSettingsManager.SetWindowed(Resolutions.FULLHD_DIV1_875, false);
            
            Assert.That(s.ScreenSettingsManager.Fullscreen, Is.False);
            Assert.That(s.ScreenSettingsManager.PixelPerfect, Is.True);
            Assert.That(s.ScreenSettingsManager.VSync, Is.True);
            Assert.That(s.ScreenSettingsManager.WindowedResolution, Is.EqualTo(Resolutions.WXGA));
            cf.Load(SettingsFile);
            Assert.That(cf.GetValue("Video", "Fullscreen", "X"), Is.False);
            Assert.That(cf.GetValue("Video", "PixelPerfect", "X"), Is.True);
            Assert.That(cf.GetValue("Video", "VSync", "X"), Is.True);
            Assert.That(cf.GetValue("Video", "WindowedResolution", "X"), Is.EqualTo(Resolutions.WXGA.Size));

        }
    }
}