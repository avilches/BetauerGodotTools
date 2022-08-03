using Betauer.Application;
using Betauer.Application.Screen;
using Betauer.DI;
using Betauer.TestRunner;
using Godot;
using NUnit.Framework;

namespace Betauer.GameTools.Tests {
    [TestFixture]
    [Only]
    public class SettingTests : Node {

        const string SettingsFile = "./test-settings.ini";
        const string SettingsFile1 = "./test-settings-1.ini";
        const string SettingsFile2 = "./test-settings-2.ini";
        [SetUp]
        public void SetUp() {
            System.IO.File.Delete(SettingsFile);
            System.IO.File.Delete(SettingsFile1);
            System.IO.File.Delete(SettingsFile2);
        }

        [TearDown]
        public void TearDown() {
            System.IO.File.Delete(SettingsFile);
            System.IO.File.Delete(SettingsFile1);
            System.IO.File.Delete(SettingsFile2);
        }

        [Configuration]
        internal class Config1 {
            [Service]
            public SettingContainer SettingContainer => new SettingContainer(SettingsFile);
            
            [Service] 
            public Setting<bool> PixelPerfect => new Setting<bool>("Section", "PixelPerfect", true);

            [Service] 
            public Setting<string> Name => new Setting<string>("Section", "Name", "Default");
            
            [Service] 
            public Setting<Resolution> Resolution => new Setting<Resolution>("Video", "Screen", Resolutions.WXGA);
        }

        [Service]
        internal class Basic1 {
            [Inject] public Setting<bool> PixelPerfect;
            [Inject] public Setting<string> Name;
            [Inject] public Setting<Resolution> Resolution;
        }

        [Test]
        public void DefaultsAndSaveTest() {
            var di = new ContainerBuilder(this);
            di.Scan<Config1>();
            di.Scan<Basic1>();
            var c = di.Build();

            var b = c.Resolve<Basic1>();
            
            // Default values are read
            Assert.That(b.PixelPerfect.Get(), Is.EqualTo(true));
            Assert.That(b.Name.Get(), Is.EqualTo("Default"));
            Assert.That(b.Resolution.Get(), Is.EqualTo(Resolutions.WXGA));
            
            // When saved, default values are stored
            b.PixelPerfect.SettingContainer.Save();
            var cf = new ConfigFile();
            cf.Load(SettingsFile);
            Assert.That(cf.GetValue(b.PixelPerfect.Section, b.PixelPerfect.Name, false), Is.True);
            Assert.That(cf.GetValue(b.Name.Section, b.Name.Name, "XXX"), Is.EqualTo("Default"));
            Assert.That(cf.GetValue(b.Resolution.Section, b.Resolution.Name, "XXX"), Is.EqualTo(Resolutions.WXGA.Size));
            
            // When changed, values are auto-saved and the new values are stored
            b.PixelPerfect.Set(false);
            b.Name.Set("CHANGED");
            b.Resolution.Set(Resolutions.FULLHD);
            cf.Load(SettingsFile);
            Assert.That(cf.GetValue(b.PixelPerfect.Section, b.PixelPerfect.Name, true), Is.False);
            Assert.That(cf.GetValue(b.Name.Section, b.Name.Name, "XXX"), Is.EqualTo("CHANGED"));
            Assert.That(cf.GetValue(b.Resolution.Section, b.Resolution.Name, "XXX"), Is.EqualTo(Resolutions.FULLHD.Size));
        }
        
        [Test]
        public void Load() {
            var cf = new ConfigFile();
            cf.SetValue("Section", "PixelPerfect", false);
            cf.SetValue("Section", "Name", "CHANGED");
            cf.SetValue("Video", "Screen", Resolutions.FULLHD_DIV1_875.Size);
            cf.Save(SettingsFile);
            cf.Clear();
            cf.Dispose();
            
            var di = new ContainerBuilder(this);
            di.Scan<Config1>();
            di.Scan<Basic1>();
            var c = di.Build();
            var b = c.Resolve<Basic1>();
            
            // Stored values are read
            Assert.That(b.PixelPerfect.Get(), Is.EqualTo(false));
            Assert.That(b.Name.Get(), Is.EqualTo("CHANGED"));
            Assert.That(b.Resolution.Get(), Is.EqualTo(Resolutions.FULLHD_DIV1_875));
        }
        
        
        [Configuration]
        internal class Config2 {
            [Service(SettingsFile1)]
            public SettingContainer SettingContainer1 => new SettingContainer(SettingsFile1);

            [Service(SettingsFile2)]
            public SettingContainer SettingContainer2 => new SettingContainer(SettingsFile2);
            
            [Service("P1")] 
            public Setting<bool> PixelPerfect => new Setting<bool>(SettingsFile1, "Section", "PixelPerfect", true);

            [Service] 
            public Setting<string> P2 => new Setting<string>(SettingsFile2, "Section", "Name", "Default");
        }

        [Service]
        internal class Basic2 {
            [Inject] public Setting<bool> P1;
            [Inject] public Setting<string> P2;
            [Inject(SettingsFile1)] public SettingContainer SettingContainer1;
            [Inject(SettingsFile2)] public SettingContainer SettingContainer2;
        }

        [Test]
        public void MultipleSettingsFile() {
            var di = new ContainerBuilder(this);
            di.Scan<Config2>();
            di.Scan<Basic2>();
            var c = di.Build();
            var b = c.Resolve<Basic2>();
            
            Assert.That(b.P1.SettingContainer, Is.EqualTo(b.SettingContainer1));
            Assert.That(b.P2.SettingContainer, Is.EqualTo(b.SettingContainer2));
        }


    }
}