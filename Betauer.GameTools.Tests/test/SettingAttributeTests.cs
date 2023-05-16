using Betauer.Application;
using Betauer.Application.Screen;
using Betauer.Application.Settings;
using Betauer.Application.Settings.Attributes;
using Betauer.DI.Attributes;
using Betauer.DI.Exceptions;
using Betauer.TestRunner;
using Godot;
using NUnit.Framework;
using Container = Betauer.DI.Container;

namespace Betauer.GameTools.Tests; 
  
[TestRunner.Test]
public partial class SettingAttributeTests : Node {

    const string SettingsFile = "./test-settings.ini";
    const string SettingsFile1 = "./test-settings-1.ini";
    const string SettingsFile2 = "./test-settings-2.ini";
        
    [SetUpClass]
    [TestRunner.SetUp]
    public void Clear() {
        System.IO.File.Delete(SettingsFile);
        System.IO.File.Delete(SettingsFile1);
        System.IO.File.Delete(SettingsFile2);
    }

    [Configuration]
    [Setting<string>("ServiceName", SaveAs = "Section/NoAutoSave", Default = "Default", AutoSave = false)]
    internal class ErrorConfigWithNoContainer {
    }

    [TestRunner.Test(Description = "Error if container not found (no name)")]
    public void ErrorConfigWithNoContainerTest() {
        var di = new Container.Builder();
        
        Assert.Throws<InvalidAttributeException>(() =>di.Scan<ErrorConfigWithNoContainer>());
    }

    [Configuration]
    [SettingsContainer("WrongName")]
    [Setting<string>("ServiceName", SaveAs = "Section/NoAutoSave", Default = "Default", AutoSave = false)]
    internal class ErrorConfigWithContainerNotFoundByName {
        [Singleton]
        public SettingsContainer SettingsContainer => new SettingsContainer(new ConfigFileWrapper(SettingsFile));
    }

    [TestRunner.Test(Description = "Error if container not found (wrong name)")]
    public void ErrorConfigWithContainerNotFoundByNameTest() {
        var di = new Container.Builder();
        di.Scan<ErrorConfigWithContainerNotFoundByName>();
        Assert.Throws<ServiceNotFoundException>(() => di.Build());
    }


    [Configuration]
    [SettingsContainer("MySettingsContainer")]
    [Setting<bool>("BoolSetting", SaveAs = "Section/PixelPerfect", Default = true, AutoSave = true)]
    [Setting<string>("StringSetting", SaveAs = "Section/Name", Default = "Default", AutoSave = true)]
    [Setting<string>("NoAutoSave", SaveAs = "Video/NoAutoSave", Default = "DEFAULT")]
    [Setting<string>("NoEnabled", SaveAs = "Disabled/PropertyDisabled", Default = "DEFAULT", AutoSave = true, Enabled = false)]
    internal class ConfigWithSettingContainer {
        [Singleton]
        public SettingsContainer MySettingsContainer => new SettingsContainer(new ConfigFileWrapper(SettingsFile));
    }
 
    [Singleton]
    internal class Service1 {
        [Inject] public SettingsContainer MySettingsContainer { get; set; }
        [Inject] public SaveSetting<bool> BoolSetting { get; set; }
        [Inject] public SaveSetting<string> StringSetting { get; set; }
        [Inject] public SaveSetting<string> NoAutoSave { get; set; }
        [Inject] public SaveSetting<string> NoEnabled { get; set; }
    }

    [TestRunner.Test]
    public void ConfigWithSettingContainerTest() {
        var di = new Container.Builder();
        di.Scan<ConfigWithSettingContainer>();
        di.Scan<Service1>();
        var c = di.Build();

        var b = c.Resolve<Service1>();

        // Check the SettingContainer
        Assert.That(b.BoolSetting.SettingsContainer, Is.EqualTo(b.MySettingsContainer));
        Assert.That(b.StringSetting.SettingsContainer, Is.EqualTo(b.MySettingsContainer));
        Assert.That(b.NoAutoSave.SettingsContainer, Is.EqualTo(b.MySettingsContainer));
        Assert.That(b.NoEnabled.SettingsContainer, Is.EqualTo(b.MySettingsContainer));

        // Read with no settings save, the default values are used
        Assert.That(b.BoolSetting.Value, Is.True);
        Assert.That(b.StringSetting.Value, Is.EqualTo("Default"));
        Assert.That(b.NoAutoSave.Value, Is.EqualTo("DEFAULT"));
        Assert.That(b.NoEnabled.Value, Is.EqualTo("DEFAULT"));
        Assert.That(b.MySettingsContainer.Dirty, Is.False);
            
        // When force saved, default values are stored, except the no enabled
        b.BoolSetting.SettingsContainer.Save();
        Assert.That(b.MySettingsContainer.Dirty, Is.False);
        var cf = new ConfigFile();
        cf.Load(SettingsFile);
        Assert.That(cf.GetTypedValue(b.BoolSetting.SaveAs, false), Is.True);
        Assert.That(cf.GetTypedValue(b.StringSetting.SaveAs, "XXX"), Is.EqualTo("Default"));
        Assert.That(cf.GetTypedValue(b.NoAutoSave.SaveAs, "XXX"), Is.EqualTo("DEFAULT"));
        Assert.That(cf.GetTypedValue(b.NoEnabled.SaveAs, "XXX"), Is.EqualTo("XXX")); // not written to file

        b.NoEnabled.Value = "NEW VALUE";
        Assert.That(b.NoEnabled.Value, Is.EqualTo("NEW VALUE"));
        Assert.That(b.MySettingsContainer.Dirty, Is.False);
        cf.Load(SettingsFile);
        Assert.That(cf.GetTypedValue( b.NoEnabled.SaveAs, "XXX"), Is.EqualTo("XXX")); // not present in file
            
        // When changed, only the auto-saved are stored
        b.BoolSetting.Value = false;
        Assert.That(b.MySettingsContainer.Dirty, Is.False);
        cf.Load(SettingsFile);
        Assert.That(cf.GetTypedValue(b.BoolSetting.SaveAs, true), Is.False);

        b.StringSetting.Value = "CHANGED";
        Assert.That(b.MySettingsContainer.Dirty, Is.False);
        cf.Load(SettingsFile);
        Assert.That(cf.GetTypedValue<string>(b.StringSetting.SaveAs, "XXX"), Is.EqualTo("CHANGED"));
            
        // No autosave, dirty and the value is still the old value
        b.NoAutoSave.Value = "CHANGED";
        Assert.That(b.MySettingsContainer.Dirty, Is.True);
            
        Assert.That(cf.GetTypedValue(b.NoAutoSave.SaveAs, "XXX"), Is.EqualTo("DEFAULT"));
        b.NoAutoSave.SettingsContainer.Save();
        cf.Load(SettingsFile);
        Assert.That(cf.GetTypedValue(b.NoAutoSave.SaveAs, "XXX"), Is.EqualTo("CHANGED"));
            
        // Change the data from the disk
        cf.Clear();
        cf.SetTypedValue(b.NoAutoSave.SaveAs, "FROM DISK");
        cf.SetTypedValue(b.NoEnabled.SaveAs, "FROM DISK");
        cf.Save(SettingsFile);
        Assert.That(b.NoAutoSave.Value, Is.EqualTo("CHANGED"));
        Assert.That(b.NoEnabled.Value, Is.EqualTo("NEW VALUE"));
        b.NoAutoSave.SettingsContainer.Load();
        Assert.That(b.NoAutoSave.Value, Is.EqualTo("FROM DISK"));
        Assert.That(b.NoEnabled.Value, Is.EqualTo("NEW VALUE")); // still the same, no matter the load
    }
        
    [TestRunner.Test]
    public void ConfigWithSettingContainerLoadTest() {
        var cf = new ConfigFile();
        cf.SetTypedValue("Section", "PixelPerfect", false);
        cf.SetTypedValue("Section", "Name", "CHANGED");
        cf.Save(SettingsFile);
        cf.Clear();
        cf.Dispose();
            
        var di = new Container.Builder();
        di.Scan<ConfigWithSettingContainer>();
        di.Scan<Service1>();
        var c = di.Build();
        var b = c.Resolve<Service1>();
            
        // Stored values are read
        Assert.That(b.BoolSetting.Value, Is.False);
        Assert.That(b.StringSetting.Value, Is.EqualTo("CHANGED"));
    }
    
    
    [Configuration]
    [SettingsContainer("MySettingsContainer")]
    [Setting<Resolution>("Resolution", SaveAs = "Section/Screen", DefaultAsString = "1280x800", AutoSave = true)]
    internal class ConfigWithSettingContainer2 {
        [Singleton]
        public SettingsContainer MySettingsContainer => new SettingsContainer(new ConfigFileWrapper(SettingsFile));
    }
 
    [Singleton]
    internal class Service2 {
        [Inject] public SaveSetting<Resolution> Resolution { get; set; }
    }

    [TestRunner.Test]
    public void ConfigWithTransformerTest() {
        var di = new Container.Builder();
        di.Scan<ConfigWithSettingContainer2>();
        di.Scan<Service2>();
        var c = di.Build();

        var b = c.Resolve<Service2>();
        
        Assert.That(b.Resolution.Value, Is.EqualTo(Resolutions.WXGA));

        b.Resolution.Value = Resolutions.UWDI238_0;
        
        var cf = new ConfigFile();
        cf.Load(SettingsFile);
        Assert.That(cf.GetTypedValue(b.Resolution.SaveAs, new Vector2I(2,3)), Is.EqualTo(Resolutions.UWDI238_0.Size));
        
    }

    [Configuration]
    [SettingsContainer("MySettingsContainer")]
    [Setting<Resolution>("OtherName", SaveAs = "Section/Screen", DefaultAsString = "1280x800")]
    internal class ConfigWithSettingContainer3 {
        [Singleton]
        public SettingsContainer MySettingsContainer => new SettingsContainer(new ConfigFileWrapper(SettingsFile));
    }
 
    [Singleton]
    internal class Service3 {
        [Inject("OtherName")] public SaveSetting<Resolution> Resolution { get; set; }
    }

    [TestRunner.Test]
    public void InjectionNameTest() {
        var di = new Container.Builder();
        di.Scan<ConfigWithSettingContainer3>();
        di.Scan<Service3>();
        var c = di.Build();
        var b = c.Resolve<Service3>();
        Assert.That(b.Resolution.Value, Is.EqualTo(Resolutions.WXGA));
        
    }


}
