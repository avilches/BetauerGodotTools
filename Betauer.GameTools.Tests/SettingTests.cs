using System.Collections.Generic;
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
        internal class ConfigWithContainer {
            [Service]
            public SettingsContainer SettingsContainer => new SettingsContainer(SettingsFile);
            
            [Service] 
            public Setting<bool> BoolSetting => new Setting<bool>("Section", "PixelPerfect", true);

            [Service] 
            public Setting<string> StringSetting => new Setting<string>("Section", "Name", "Default");
            
            [Service] 
            public Setting<Resolution> Resolution => new Setting<Resolution>("Video", "Screen", Resolutions.WXGA);

            [Service] 
            public Setting<string> NoAutoSave => new Setting<string>("Video", "NoAutoSave", "DEFAULT", false);

            [Service] 
            public Setting<string> NoEnabled => new Setting<string>("Disabled", "PropertyDisabled", "DEFAULT", true, false);
        }

        [Service]
        internal class Service1 {
            [Inject] public SettingsContainer SettingsContainerByType;
            [Inject] public Setting<bool> BoolSetting;
            [Inject] public Setting<string> StringSetting;
            [Inject] public Setting<Resolution> Resolution;
            [Inject] public Setting<string> NoAutoSave;
            [Inject] public Setting<string> NoEnabled;
        }

        [Test]
        public void DefaultsAndSaveTest() {
            var di = new ContainerBuilder(this);
            di.Scan<ConfigWithContainer>();
            di.Scan<Service1>();
            var c = di.Build();

            var b = c.Resolve<Service1>();

            // Check the SettingContainer
            Assert.That(b.BoolSetting.SettingsContainer, Is.EqualTo(b.SettingsContainerByType));
            Assert.That(b.StringSetting.SettingsContainer, Is.EqualTo(b.SettingsContainerByType));
            Assert.That(b.Resolution.SettingsContainer, Is.EqualTo(b.SettingsContainerByType));
            Assert.That(b.NoAutoSave.SettingsContainer, Is.EqualTo(b.SettingsContainerByType));
            Assert.That(b.NoEnabled.SettingsContainer, Is.EqualTo(b.SettingsContainerByType));

            // Read with no settings save, the default values are used
            
            Assert.That(b.BoolSetting.Value, Is.EqualTo(true));
            Assert.That(b.StringSetting.Value, Is.EqualTo("Default"));
            Assert.That(b.Resolution.Value, Is.EqualTo(Resolutions.WXGA));
            Assert.That(b.NoAutoSave.Value, Is.EqualTo("DEFAULT"));
            Assert.That(b.NoEnabled.Value, Is.EqualTo("DEFAULT"));
            Assert.That(b.SettingsContainerByType.Dirty, Is.False);
            
            // When force saved, default values are stored, except the no enabled
            b.BoolSetting.SettingsContainer.Save();
            Assert.That(b.SettingsContainerByType.Dirty, Is.False);
            var cf = new ConfigFile();
            cf.Load(SettingsFile);
            Assert.That(cf.GetValue(b.BoolSetting.Section, b.BoolSetting.Name, false), Is.True);
            Assert.That(cf.GetValue(b.StringSetting.Section, b.StringSetting.Name, "XXX"), Is.EqualTo("Default"));
            Assert.That(cf.GetValue(b.Resolution.Section, b.Resolution.Name, "XXX"), Is.EqualTo(Resolutions.WXGA.Size));
            Assert.That(cf.GetValue(b.NoAutoSave.Section, b.NoAutoSave.Name, "XXX"), Is.EqualTo("DEFAULT"));
            Assert.That(cf.GetValue(b.NoEnabled.Section, b.NoEnabled.Name, "XXX"), Is.EqualTo("XXX")); // not written to file

            b.NoEnabled.Value = "NEW VALUE";
            Assert.That(b.NoEnabled.Value, Is.EqualTo("NEW VALUE"));
            Assert.That(b.SettingsContainerByType.Dirty, Is.False);
            cf.Load(SettingsFile);
            Assert.That(cf.GetValue(b.NoEnabled.Section, b.NoEnabled.Name, "XXX"), Is.EqualTo("XXX")); // not present in file
            
            // When changed, only the auto-saved are stored
            b.BoolSetting.Value = false;
            Assert.That(b.SettingsContainerByType.Dirty, Is.False);
            cf.Load(SettingsFile);
            Assert.That(cf.GetValue(b.BoolSetting.Section, b.BoolSetting.Name, true), Is.False);

            b.StringSetting.Value = "CHANGED";
            Assert.That(b.SettingsContainerByType.Dirty, Is.False);
            cf.Load(SettingsFile);
            Assert.That(cf.GetValue(b.StringSetting.Section, b.StringSetting.Name, "XXX"), Is.EqualTo("CHANGED"));
            
            b.Resolution.Value = Resolutions.FULLHD;
            Assert.That(b.SettingsContainerByType.Dirty, Is.False);
            cf.Load(SettingsFile);
            Assert.That(cf.GetValue(b.Resolution.Section, b.Resolution.Name, "XXX"), Is.EqualTo(Resolutions.FULLHD.Size));

            // No autosave, dirty and the value is still the old value
            b.NoAutoSave.Value = "CHANGED";
            Assert.That(b.SettingsContainerByType.Dirty, Is.True);
            
            Assert.That(cf.GetValue(b.NoAutoSave.Section, b.NoAutoSave.Name, "XXX"), Is.EqualTo("DEFAULT"));
            b.NoAutoSave.SettingsContainer.Save();
            cf.Load(SettingsFile);
            Assert.That(cf.GetValue(b.NoAutoSave.Section, b.NoAutoSave.Name, "XXX"), Is.EqualTo("CHANGED"));
            
            // Change the data from the disk
            cf.Clear();
            cf.SetValue(b.NoAutoSave.Section, b.NoAutoSave.Name, "FROM DISK");
            cf.SetValue(b.NoEnabled.Section, b.NoEnabled.Name, "FROM DISK");
            cf.Save(SettingsFile);
            Assert.That(b.NoAutoSave.Value, Is.EqualTo("CHANGED"));
            Assert.That(b.NoEnabled.Value, Is.EqualTo("NEW VALUE"));
            b.NoAutoSave.SettingsContainer.Load();
            Assert.That(b.NoAutoSave.Value, Is.EqualTo("FROM DISK"));
            Assert.That(b.NoEnabled.Value, Is.EqualTo("NEW VALUE")); // still the same, no matter the load
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
            di.Scan<ConfigWithContainer>();
            di.Scan<Service1>();
            var c = di.Build();
            var b = c.Resolve<Service1>();
            
            // Stored values are read
            Assert.That(b.BoolSetting.Value, Is.EqualTo(false));
            Assert.That(b.StringSetting.Value, Is.EqualTo("CHANGED"));
            Assert.That(b.Resolution.Value, Is.EqualTo(Resolutions.FULLHD_DIV1_875));
        }
        
        
        [Configuration]
        internal class ConfigWithMultipleContainer {
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
        public void MultipleSettingsFileTest() {
            var di = new ContainerBuilder(this);
            di.Scan<ConfigWithMultipleContainer>();
            di.Scan<Basic2>();
            var c = di.Build();
            var b = c.Resolve<Basic2>();
            
            Assert.That(b.P1.SettingsContainer, Is.EqualTo(b.SettingsContainer1));
            Assert.That(b.P2.SettingsContainer, Is.EqualTo(b.SettingsContainer2));
        }

        [Configuration]
        internal class ConfigWithAnonymousContainer {
            [Service] 
            public Setting<string> P1 => new Setting<string>("Section", "Name", "Default");
            [Service] 
            public Setting<string> P2 => new Setting<string>("Section", "Name", "Default");
        }

        [Service]
        internal class Basic3 {
            [Inject] public Setting<string> P1;
            [Inject] public Setting<string> P2;
        }

        [Test(Description = "Anonymous container test")]
        public void AnonymousContainerTest() {
            var di = new ContainerBuilder(this);
            di.Scan<ConfigWithAnonymousContainer>();
            di.Scan<Basic3>();
            var c = di.Build();
            var b = c.Resolve<Basic3>();

            Assert.That(c.Contains<SettingsContainer>(), Is.False);
            Assert.That(b.P1.SettingsContainer, Is.TypeOf<SettingsContainer>());
            Assert.That(b.P1.SettingsContainer, Is.EqualTo(b.P2.SettingsContainer));
        }

        [Configuration]
        internal class SettingContainerNotFound {
            [Service] 
            public Setting<string> P1 => new Setting<string>("NOT FOUND", "Section", "Name", "Default");
        }

        [Test(Description = "SettingContainer name not found")]
        public void SettingContainerNotFoundTest() {
            var di = new ContainerBuilder(this);
            di.Scan<SettingContainerNotFound>();
            di.Scan<Basic3>();
            Assert.Throws<KeyNotFoundException>(() => di.Build());
        }


    }
}