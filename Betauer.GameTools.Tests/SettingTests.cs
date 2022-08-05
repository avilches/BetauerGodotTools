using Betauer.Application;
using Betauer.Application.Screen;
using Betauer.Application.Settings;
using Betauer.DI;
using Betauer.TestRunner;
using Godot;
using NUnit.Framework;

namespace Betauer.GameTools.Tests {
    [TestFixture]
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
            public SettingsContainer SettingsContainer => new SettingsContainer(SettingsFile);
            
            [Service] 
            public Setting<bool> PixelPerfect => new Setting<bool>("Section", "PixelPerfect", true);

            [Service] 
            public Setting<string> Name => new Setting<string>("Section", "Name", "Default");
            
            [Service] 
            public Setting<Resolution> Resolution => new Setting<Resolution>("Video", "Screen", Resolutions.WXGA);

            [Service] 
            public Setting<string> NoAutoSave => new Setting<string>("Video", "NoAutoSave", "DEFAULT", false);
        }

        [Service]
        internal class Basic1 {
            [Inject] public Setting<bool> PixelPerfect;
            [Inject] public Setting<string> Name;
            [Inject] public Setting<Resolution> Resolution;
            [Inject] public Setting<string> NoAutoSave;
        }

        [Test]
        public void DefaultsAndSaveTest() {
            var di = new ContainerBuilder(this);
            di.Scan<Config1>();
            di.Scan<Basic1>();
            var c = di.Build();

            var b = c.Resolve<Basic1>();
            
            // Default values are read
            Assert.That(b.PixelPerfect.SettingsContainer.Dirty, Is.False);
            
            Assert.That(b.PixelPerfect.Value, Is.EqualTo(true));
            Assert.That(b.Name.Value, Is.EqualTo("Default"));
            Assert.That(b.Resolution.Value, Is.EqualTo(Resolutions.WXGA));
            Assert.That(b.NoAutoSave.Value, Is.EqualTo("DEFAULT"));
            
            // When force saved, default values are stored
            b.PixelPerfect.SettingsContainer.Save();
            Assert.That(b.PixelPerfect.SettingsContainer.Dirty, Is.False);
            var cf = new ConfigFile();
            cf.Load(SettingsFile);
            Assert.That(cf.GetValue(b.PixelPerfect.Section, b.PixelPerfect.Name, false), Is.True);
            Assert.That(cf.GetValue(b.Name.Section, b.Name.Name, "XXX"), Is.EqualTo("Default"));
            Assert.That(cf.GetValue(b.Resolution.Section, b.Resolution.Name, "XXX"), Is.EqualTo(Resolutions.WXGA.Size));
            Assert.That(cf.GetValue(b.NoAutoSave.Section, b.NoAutoSave.Name, "XXX"), Is.EqualTo("DEFAULT"));
            
            // When changed, only the auto-saved are stored
            b.PixelPerfect.Value = false;
            cf.Load(SettingsFile);
            Assert.That(cf.GetValue(b.PixelPerfect.Section, b.PixelPerfect.Name, true), Is.False);
            Assert.That(b.PixelPerfect.SettingsContainer.Dirty, Is.False);

            b.Name.Value = "CHANGED";
            cf.Load(SettingsFile);
            Assert.That(cf.GetValue(b.Name.Section, b.Name.Name, "XXX"), Is.EqualTo("CHANGED"));
            Assert.That(b.Name.SettingsContainer.Dirty, Is.False);
            
            b.Resolution.Value = Resolutions.FULLHD;
            cf.Load(SettingsFile);
            Assert.That(cf.GetValue(b.Resolution.Section, b.Resolution.Name, "XXX"), Is.EqualTo(Resolutions.FULLHD.Size));
            Assert.That(b.Resolution.SettingsContainer.Dirty, Is.False);

            // No autosave, dirty and the value is still the old value
            b.NoAutoSave.Value = "CHANGED";
            Assert.That(b.NoAutoSave.SettingsContainer.Dirty, Is.True);
            Assert.That(cf.GetValue(b.NoAutoSave.Section, b.NoAutoSave.Name, "XXX"), Is.EqualTo("DEFAULT"));
            b.NoAutoSave.SettingsContainer.Save();
            cf.Load(SettingsFile);
            Assert.That(cf.GetValue(b.NoAutoSave.Section, b.NoAutoSave.Name, "XXX"), Is.EqualTo("CHANGED"));
            
            // Change the data from the disk
            cf.Clear();
            cf.SetValue(b.NoAutoSave.Section, b.NoAutoSave.Name, "FROM DISK");
            cf.Save(SettingsFile);
            Assert.That(b.NoAutoSave.Value, Is.EqualTo("CHANGED"));
            b.NoAutoSave.SettingsContainer.Load();
            Assert.That(b.NoAutoSave.Value, Is.EqualTo("FROM DISK"));
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
            Assert.That(b.PixelPerfect.Value, Is.EqualTo(false));
            Assert.That(b.Name.Value, Is.EqualTo("CHANGED"));
            Assert.That(b.Resolution.Value, Is.EqualTo(Resolutions.FULLHD_DIV1_875));
        }
        
        
        [Configuration]
        internal class Config2 {
            [Service(SettingsFile1)]
            public SettingsContainer SettingsContainer1 => new SettingsContainer(SettingsFile1);

            [Service(SettingsFile2)]
            public SettingsContainer SettingsContainer2 => new SettingsContainer(SettingsFile2);
            
            [Service("P1")] 
            public Setting<bool> PixelPerfect => new Setting<bool>(SettingsFile1, "Section", "PixelPerfect", true);

            [Service] 
            public Setting<string> P2 => new Setting<string>(SettingsFile2, "Section", "Name", "Default");
        }

        [Service]
        internal class Basic2 {
            [Inject] public Setting<bool> P1;
            [Inject] public Setting<string> P2;
            [Inject(SettingsFile1)] public SettingsContainer SettingsContainer1;
            [Inject(SettingsFile2)] public SettingsContainer SettingsContainer2;
        }

        [Test]
        public void MultipleSettingsFile() {
            var di = new ContainerBuilder(this);
            di.Scan<Config2>();
            di.Scan<Basic2>();
            var c = di.Build();
            var b = c.Resolve<Basic2>();
            
            Assert.That(b.P1.SettingsContainer, Is.EqualTo(b.SettingsContainer1));
            Assert.That(b.P2.SettingsContainer, Is.EqualTo(b.SettingsContainer2));
        }


    }
}