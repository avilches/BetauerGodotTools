using System;
using Betauer.Application;
using Betauer.Application.Settings;
using Betauer.TestRunner;
using Godot;
using NUnit.Framework;

namespace Betauer.GameTools.Tests;

[TestFixture]
public partial class SettingTests : Node {

    const string SettingsFile = "./test-settings.ini";

    [SetUp]
    public void Clear() {
        System.IO.File.Delete(SettingsFile);
    }

    [Test]
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

    [Test]
    public void DuplicatedSettingTest() {
        var sc = new SettingsContainer(new ConfigFileWrapper(SettingsFile));
        sc.Add(Setting.Create("Section/SavedDisabled", "Default"));
        Assert.Throws<Exception>(() => sc.Add(Setting.Create("Section/SavedDisabled", "Pepe")));
    }

    [Test]
    public void WorksIfContainer() {
        var sc = new SettingsContainer(new ConfigFileWrapper(SettingsFile));
        var saved = Setting.Create("Section/NoAutoSave", "Default", false);
        sc.Add(saved);
        Assert.That(saved.Value, Is.EqualTo("Default"));
        Assert.That(saved.SettingsContainer, Is.EqualTo(sc));

    }

    [Test]
    public void SectionNameTest() {
        // Property without section
        var noSection = Setting.Create("property", "A");
        Assert.That(noSection.SaveAs, Is.EqualTo("Settings/property"));

        // Property with slash
        var propWithSlash = Setting.Create("Section/prop/other", "B");
        Assert.That(propWithSlash.SaveAs, Is.EqualTo("Section/prop/other"));

        var sc = new SettingsContainer(new ConfigFileWrapper(SettingsFile));
        sc.Add(noSection);
        sc.Add(propWithSlash);

        sc.Save();

        var cf = new ConfigFileWrapper(SettingsFile).Load();
        Assert.That(cf.GetValue<string>(noSection.SaveAs, "_"), Is.EqualTo("A"));
        Assert.That(cf.GetValue<string>(propWithSlash.SaveAs, "_"), Is.EqualTo("B"));
    }

    [Test]
    public void SaveNoAutoSaveBeforeEvenUseTest() {
        var saved = Setting.Create("Section/prop", "B", false);
        var sc = new SettingsContainer(new ConfigFileWrapper(SettingsFile));
        sc.Add(saved);

        sc.Save();

        var cf = new ConfigFileWrapper(SettingsFile).Load();
        Assert.That(cf.GetValue<string>(saved.SaveAs, "_"), Is.EqualTo("B"));
    }

    [Test]
    public void RegularAutoSaveTest() {
        var sc = new SettingsContainer(new ConfigFileWrapper(SettingsFile));
        var saved = Setting.Create("Section/NoAutoSave", "Default", false);
        sc.Add(saved);

        const string changed = "XXXX";
        saved.Value = changed;
        Assert.That(saved.Value, Is.EqualTo(changed));

        var cf = new ConfigFileWrapper(SettingsFile).Load();
        Assert.That(cf.GetValue<string>(saved.SaveAs, "NOT SAVED"), Is.EqualTo("NOT SAVED"));
        Assert.That(cf.GetValue<string>("Section", "NoAutoSave", "NOT SAVED"), Is.EqualTo("NOT SAVED"));

        saved.SettingsContainer.Save();
        cf.Load();
        Assert.That(cf.GetValue<string>(saved.SaveAs, "_"), Is.EqualTo(changed));
        Assert.That(cf.GetValue<string>("Section", "NoAutoSave", "_"), Is.EqualTo(changed));
    }

    [Test]
    public void RegularNoAutoSaveTest() {
        var sc = new SettingsContainer(new ConfigFileWrapper(SettingsFile));
        var saved = Setting.Create("Section/NoAutoSave", "Default", true);
        sc.Add(saved);

        var cf = new ConfigFileWrapper(SettingsFile);
        cf.Load();
        Assert.That(cf.GetValue<string>(saved.SaveAs, "NOT SAVED"), Is.EqualTo("NOT SAVED"));
        Assert.That(cf.GetValue<string>("Section", "NoAutoSave", "NOT SAVED"), Is.EqualTo("NOT SAVED"));

        const string changed = "XXXX";
        saved.Value = changed;
        Assert.That(saved.Value, Is.EqualTo(changed));

        cf.Load();
        Assert.That(cf.GetValue<string>(saved.SaveAs, "_"), Is.EqualTo(changed));
        Assert.That(cf.GetValue<string>("Section", "NoAutoSave", "_"), Is.EqualTo(changed));
    }

    [Test]
    public void DefaultValueTest() {
        var sc = new SettingsContainer(new ConfigFileWrapper(SettingsFile));
        var saved = Setting.Create("Section/SavedDisabled", "Default");
        sc.Add(saved);

        Assert.That(saved.Value, Is.EqualTo("Default"));
    }

    public class Test1 {
        public SaveSetting<string> Saved1 { get; } = Setting.Create("Section/B", "Default");
        public SaveSetting<bool> Saved2 { get; } = Setting.Create("Section/A", true);
    }

    [Test]
    public void LoadInstanceTest() {
        var sc = new SettingsContainer(new ConfigFileWrapper(SettingsFile));
        var t = new Test1();
        sc.AddFromInstanceProperties(t);
        Assert.That(sc.Settings.Contains(t.Saved1));
        Assert.That(sc.Settings.Contains(t.Saved1));
    }
}
