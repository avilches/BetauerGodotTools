using System;
using System.Collections.Generic;
using Betauer.Application;
using Betauer.Application.Screen;
using Betauer.Application.Settings;
using Betauer.Core;
using Betauer.DI;
using Betauer.DI.Attributes;
using Betauer.DI.Exceptions;
using Betauer.Input;
using Betauer.TestRunner;
using Godot;
using NUnit.Framework;
using Container = Betauer.DI.Container;

namespace Betauer.GameTools.Tests; 
  
[TestRunner.Test(Only = true)]
public partial class SettingTests : Node {

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

    [TestRunner.Test]
    public void MemoryTest() {
        var imm = Setting.Memory("I");
            
        Assert.That(imm.Value, Is.EqualTo("I"));
        imm.Value = "X";
        Assert.That(imm.Value, Is.EqualTo("X"));
    }

    [TestRunner.Test]
    public void FailIfNoContainer() {
        var sc = new SettingsContainer(SettingsFile);
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
        var sc = new SettingsContainer(SettingsFile);
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
        
        var sc = new SettingsContainer(SettingsFile);
        noSection.SetSettingsContainer(sc);
        propWithSlash.SetSettingsContainer(sc);

        sc.Save();

        var cf = new ConfigFileWrapper().SetFilePath(SettingsFile).Load();
        Assert.That(cf.GetValue<string>(noSection.SaveAs, "_"), Is.EqualTo("A"));
        Assert.That(cf.GetValue<string>(propWithSlash.SaveAs, "_"), Is.EqualTo("B"));
    }

    [TestRunner.Test]
    public void SaveNoAutoSaveBeforeEvenUseTest() {
        var saved = Setting.Create("Section/prop", "B", false);
        var sc = new SettingsContainer(SettingsFile);
        saved.SetSettingsContainer(sc);

        sc.Save();

        var cf = new ConfigFileWrapper().SetFilePath(SettingsFile).Load();
        Assert.That(cf.GetValue<string>(saved.SaveAs, "_"), Is.EqualTo("B"));
    }

    [TestRunner.Test]
    public void RegularAutoSaveTest() {
        var sc = new SettingsContainer(SettingsFile);
        var saved = Setting.Create("Section/NoAutoSave", "Default", false);
        saved.SetSettingsContainer(sc);

        const string changed = "XXXX";
        saved.Value = changed;
        Assert.That(saved.Value, Is.EqualTo(changed));

        var cf = new ConfigFileWrapper().SetFilePath(SettingsFile).Load();
        Assert.That(cf.GetValue<string>(saved.SaveAs, "NOT SAVED"), Is.EqualTo("NOT SAVED"));
        Assert.That(cf.GetValue<string>("Section", "NoAutoSave", "NOT SAVED"), Is.EqualTo("NOT SAVED"));

        saved.SettingsContainer.Save();
        cf.Load();
        Assert.That(cf.GetValue<string>( saved.SaveAs, "_"), Is.EqualTo(changed));
        Assert.That(cf.GetValue<string>( "Section", "NoAutoSave", "_"), Is.EqualTo(changed));
    } 

    [TestRunner.Test]
    public void RegularNoAutoSaveTest() {
        var sc = new SettingsContainer(SettingsFile);
        var saved = Setting.Create("Section/NoAutoSave", "Default", true);
        saved.SetSettingsContainer(sc);

        var cf = new ConfigFileWrapper().SetFilePath(SettingsFile);
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
    public void DisabledTest() {
        var sc = new SettingsContainer(SettingsFile);
        var savedDisabled = Setting.Create("Section/SavedDisabled", "Default", true, false);
            
        // A disabled setting works like a memory setting: it can be write and read, but data is not persisted
        
        // Read without container
        Assert.That(savedDisabled.Value, Is.EqualTo("Default"));
        // Write without container
        savedDisabled.Value = "New1";
        Assert.That(savedDisabled.Value, Is.EqualTo("New1"));

        // Setting a container doesn't make any difference
        savedDisabled.SetSettingsContainer(sc);
        Assert.That(savedDisabled.Value, Is.EqualTo("New1"));
        Assert.That(savedDisabled.SettingsContainer, Is.EqualTo(sc));
        savedDisabled.Value = "New2";
        Assert.That(savedDisabled.Value, Is.EqualTo("New2"));

        var cf = new ConfigFileWrapper().SetFilePath(SettingsFile).Load();
        Assert.That(cf.GetValue<string>(savedDisabled.SaveAs, "NOT FOUND"), Is.EqualTo("NOT FOUND"));
        
        
        // Now, enable the setting again
        savedDisabled.Enabled = true;

        // Ensure that the value is still the same
        cf.Load();
        Assert.That(cf.GetValue<string>(savedDisabled.SaveAs, "NOT FOUND"), Is.EqualTo("NOT FOUND"));
        
        savedDisabled.Value = "New3";
        cf.Load();
        Assert.That(cf.GetValue<string>(savedDisabled.SaveAs, "NOT FOUND"), Is.EqualTo("New3"));
    }

    [TestRunner.Test]
    public void DefaultValueTest() {
        var sc = new SettingsContainer(SettingsFile);
        var saved = Setting.Create("Section/SavedDisabled", "Default");
        saved.SetSettingsContainer(sc);
        
        Assert.That(saved.Value, Is.EqualTo("Default"));
        saved.DefaultValue = "Default2";
        Assert.That(saved.Value, Is.EqualTo("Default2"));

    }

}
