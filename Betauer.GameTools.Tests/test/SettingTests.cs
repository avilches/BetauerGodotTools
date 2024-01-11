using System;
using Betauer.Application;
using Betauer.Application.Settings;
using Betauer.TestRunner;
using Godot;
using NUnit.Framework;

namespace Betauer.GameTools.Tests; 
  
[TestRunner.Test]
public partial class SettingTests : Node {

    const string SettingsFile = "./test-settings.ini";
        
    [SetUpClass]
    [TestRunner.SetUp]
    public void Clear() {
        System.IO.File.Delete(SettingsFile);
    }

    [TestRunner.Test]
    public void MemoryTest() {
        var imm = Setting.Memory("I");
            
        Assert.That(imm.Value, Is.EqualTo("I"));
        imm.Value = "X";
        Assert.That(imm.Value, Is.EqualTo("X"));
    }

    [TestRunner.Test]
    public void FailIfNoContainer() {
        // var sc = new SettingsContainer(new ConfigFileWrapper(SettingsFile));
        var saved = Setting.Create("Section/NoAutoSave", "Default", false);

        // Read without container
        Assert.Throws<NullReferenceException>(() => {
            var x = saved.Value;
        });

        // Write without container
        Assert.Throws<NullReferenceException>(() => saved.Value = "FAIL");
    }

    [TestRunner.Test]
    public void WorksIfContainer() {
        var sc = new SettingsContainer(new ConfigFileWrapper(SettingsFile));
        var saved = Setting.Create("Section/NoAutoSave", "Default", false);
        saved.SetSettingsContainer(sc);
        Assert.That(saved.Value, Is.EqualTo("Default"));
        Assert.That(saved.SettingsContainer, Is.EqualTo(sc));

    }

    [TestRunner.Test]
    public void SectionNameTest() {
        // Property without section
        var noSection = Setting.Create("property", "A");
        Assert.That(noSection.SaveAs, Is.EqualTo("Settings/property"));

        // Property with slash
        var propWithSlash = Setting.Create("Section/prop/other", "B");
        Assert.That(propWithSlash.SaveAs, Is.EqualTo("Section/prop/other"));
        
        var sc = new SettingsContainer(new ConfigFileWrapper(SettingsFile));
        noSection.SetSettingsContainer(sc);
        propWithSlash.SetSettingsContainer(sc);

        sc.Save();

        var cf = new ConfigFileWrapper(SettingsFile).Load();
        Assert.That(cf.GetValue<string>(noSection.SaveAs, "_"), Is.EqualTo("A"));
        Assert.That(cf.GetValue<string>(propWithSlash.SaveAs, "_"), Is.EqualTo("B"));
    }

    [TestRunner.Test]
    public void SaveNoAutoSaveBeforeEvenUseTest() {
        var saved = Setting.Create("Section/prop", "B", false);
        var sc = new SettingsContainer(new ConfigFileWrapper(SettingsFile));
        saved.SetSettingsContainer(sc);

        sc.Save();

        var cf = new ConfigFileWrapper(SettingsFile).Load();
        Assert.That(cf.GetValue<string>(saved.SaveAs, "_"), Is.EqualTo("B"));
    }

    [TestRunner.Test]
    public void RegularAutoSaveTest() {
        var sc = new SettingsContainer(new ConfigFileWrapper(SettingsFile));
        var saved = Setting.Create("Section/NoAutoSave", "Default", false);
        saved.SetSettingsContainer(sc);

        const string changed = "XXXX";
        saved.Value = changed;
        Assert.That(saved.Value, Is.EqualTo(changed));

        var cf = new ConfigFileWrapper(SettingsFile).Load();
        Assert.That(cf.GetValue<string>(saved.SaveAs, "NOT SAVED"), Is.EqualTo("NOT SAVED"));
        Assert.That(cf.GetValue<string>("Section", "NoAutoSave", "NOT SAVED"), Is.EqualTo("NOT SAVED"));

        saved.SettingsContainer.Save();
        cf.Load();
        Assert.That(cf.GetValue<string>( saved.SaveAs, "_"), Is.EqualTo(changed));
        Assert.That(cf.GetValue<string>( "Section", "NoAutoSave", "_"), Is.EqualTo(changed));
    } 

    [TestRunner.Test]
    public void RegularNoAutoSaveTest() {
        var sc = new SettingsContainer(new ConfigFileWrapper(SettingsFile));
        var saved = Setting.Create("Section/NoAutoSave", "Default", true);
        saved.SetSettingsContainer(sc);

        var cf = new ConfigFileWrapper(SettingsFile);
        cf.Load();
        Assert.That(cf.GetValue<string>(saved.SaveAs, "NOT SAVED"), Is.EqualTo("NOT SAVED"));
        Assert.That(cf.GetValue<string>("Section", "NoAutoSave", "NOT SAVED"), Is.EqualTo("NOT SAVED"));

        const string changed = "XXXX";
        saved.Value = changed;
        Assert.That(saved.Value, Is.EqualTo(changed));

        cf.Load();
        Assert.That(cf.GetValue<string>( saved.SaveAs, "_"), Is.EqualTo(changed));
        Assert.That(cf.GetValue<string>( "Section", "NoAutoSave", "_"), Is.EqualTo(changed));
    } 

    [TestRunner.Test]
    public void DefaultValueTest() {
        var sc = new SettingsContainer(new ConfigFileWrapper(SettingsFile));
        var saved = Setting.Create("Section/SavedDisabled", "Default");
        saved.SetSettingsContainer(sc);
        
        Assert.That(saved.Value, Is.EqualTo("Default"));
    }

    [TestRunner.Test]
    public void SharedSettingNameTest() {
        var sc = new SettingsContainer(new ConfigFileWrapper(SettingsFile));
        var saved1 = Setting.Create("Section/SavedDisabled", "Default");
        var saved2 = Setting.Create("Section/SavedDisabled", "Pepe");
        saved1.SetSettingsContainer(sc);
        saved2.SetSettingsContainer(sc);

        saved1.Value = "Nuevo";
        Assert.That(saved1.Value, Is.EqualTo("Nuevo"));
        Assert.That(saved2.Value, Is.EqualTo("Nuevo"));
        sc.Save();

        var cf = new ConfigFileWrapper(SettingsFile);
        cf.Load();
        Assert.That(cf.GetValue<string>("Section/SavedDisabled", "NOT SAVED"), Is.EqualTo("Nuevo"));

    }

}
