using System;
using System.Linq;
using Betauer.Application;
using Betauer.Application.Settings;
using Betauer.Input;
using Betauer.TestRunner;
using Godot;
using NUnit.Framework;
namespace Betauer.GameTools.Tests; 

[TestRunner.Test]
public partial class InputActionTests : Node {

    const string SettingsFile = "./action-test-settings.ini";

    [SetUpClass]
    [TestRunner.TearDown]
    public void Clear() {
        System.IO.File.Delete(SettingsFile);
    }

    [TestRunner.Test]
    public void BuilderTests() {
        var empty = InputAction.Create("N").AsSimulator();
        Assert.That(empty.Name, Is.EqualTo("N"));
        Assert.That(empty.Keys, Is.Empty);
        Assert.That(empty.Buttons, Is.Empty);

        var reg = InputAction.Create("N")
            .Keys(Key.A)
            .Keys(Key.K, Key.Alt)
            .Buttons(JoyButton.A)
            .Buttons(JoyButton.B, JoyButton.Back)
            .AsSimulator();
        Assert.That(reg.Name, Is.EqualTo("N"));
        Assert.That(reg.Keys, Is.EqualTo(new [] {Key.A, Key.K, Key.Alt}.ToList()));
        Assert.That(reg.Buttons, Is.EqualTo(new [] {JoyButton.A, JoyButton.B, JoyButton.Back}.ToList()));
    }

    [TestRunner.Test]
    public void InputActionImportExportTests() {
        SaveSetting<string> b = Setting.Create( "attack", "");
        var sc = new SettingsContainer(new ConfigFileWrapper(SettingsFile));
        b.SetSettingsContainer(sc);
            
        var jump = InputAction.Create().AsSimulator();
        jump.SaveSetting = b;
        jump.Load();
        Assert.That(jump.Buttons, Is.Empty);
        Assert.That(jump.Keys, Is.Empty);
        Assert.That(jump.Axis, Is.EqualTo(JoyAxis.Invalid));

        // Configure and save
        jump.Update(u => {
            u.AddKeys(Key.A, Key.Exclam);
            u.AddButtons(JoyButton.Paddle1, JoyButton.X);
            u.SetAxis(JoyAxis.RightX);
        });
        Assert.That(jump.Buttons, Is.EqualTo(new [] {JoyButton.Paddle1, JoyButton.X}.ToList()));
        Assert.That(jump.Keys, Is.EqualTo(new [] {Key.A, Key.Exclam}.ToList()));
        Assert.That(jump.Axis, Is.EqualTo(JoyAxis.RightX));
        jump.Save();

        // Delete
        jump.Update(u => {
            u.ClearKeys().ClearButtons();
            u.SetAxis(JoyAxis.Invalid);
        });
            
        // Its changed
        Assert.That(jump.Buttons, Is.Empty);
        Assert.That(jump.Keys, Is.Empty);
        Assert.That(jump.Axis, Is.EqualTo(JoyAxis.Invalid));

        // But it load again, data is recovered
        jump.Load();
        Assert.That(jump.Buttons, Is.EqualTo(new [] {JoyButton.Paddle1, JoyButton.X}.ToList()));
        Assert.That(jump.Keys, Is.EqualTo(new [] {Key.A, Key.Exclam}.ToList()));
        Assert.That(jump.Axis, Is.EqualTo(JoyAxis.RightX));
    }
    
    [TestRunner.Test]
    public void UpdateRollback() {
        SaveSetting<string> b = Setting.Create( "attack", "");
        var sc = new SettingsContainer(new ConfigFileWrapper(SettingsFile));
        b.SetSettingsContainer(sc);
            
        var jump = InputAction.Create().AsSimulator();
        jump.SaveSetting = b;
        jump.Load();
        Assert.That(jump.Buttons, Is.Empty);
        Assert.That(jump.Keys, Is.Empty);
        Assert.That(jump.Axis, Is.EqualTo(JoyAxis.Invalid));

        // Configure and save
        jump.Update(u => {
            u.AddKeys(Key.A, Key.Exclam);
            u.AddButtons(JoyButton.Paddle1, JoyButton.X);
            u.SetAxis(JoyAxis.RightX);
        });
        Assert.That(jump.Buttons, Is.EqualTo(new [] {JoyButton.Paddle1, JoyButton.X}.ToList()));
        Assert.That(jump.Keys, Is.EqualTo(new [] {Key.A, Key.Exclam}.ToList()));
        Assert.That(jump.Axis, Is.EqualTo(JoyAxis.RightX));
        jump.Save();

        // Delete but fail
        jump.Update(u => {
            u.ClearKeys().ClearButtons();
            u.SetAxis(JoyAxis.Invalid);
            throw new Exception();
        });

        // Data is recovered
        Assert.That(jump.Buttons, Is.EqualTo(new [] {JoyButton.Paddle1, JoyButton.X}.ToList()));
        Assert.That(jump.Keys, Is.EqualTo(new [] {Key.A, Key.Exclam}.ToList()));
        Assert.That(jump.Axis, Is.EqualTo(JoyAxis.RightX));
    }

    [TestRunner.Test]
    public void AxisActionImportExportTests() {
        SaveSetting<string> b = Setting.Create( "Lateral", "Reverse:True");
        var sc = new SettingsContainer(new ConfigFileWrapper(SettingsFile));
        b.SetSettingsContainer(sc);
        var lateral = AxisAction.Mock();
        Assert.That(lateral.Reverse, Is.False);
        
        // Load -> true
        lateral.SaveSetting = b;
        lateral.Load();
        Assert.That(lateral.Reverse, Is.True);

        // false -> Save -> Load -> false again
        lateral.Reverse = false;
        lateral.Save();
        Assert.That(lateral.Reverse, Is.False);
        lateral.Load();
        Assert.That(lateral.Reverse, Is.False);

        lateral.ResetToDefaults();
        Assert.That(lateral.Reverse, Is.True);
    }

    [TestRunner.Test]
    public void InputActionFindByName() {
        var attack = InputAction.Create("ManualAttack").AsSimulator();
        var jump = InputAction.Create("ManualJump").AsSimulator();
        var left = InputAction.Create("Left").NegativeAxis(JoyAxis.LeftX).AsSimulator();
        var right = InputAction.Create("Right").PositiveAxis(JoyAxis.LeftX).AsSimulator();
        var lateral = new AxisAction("Lateral", left, right);
        
        var c = new InputActionsContainer();
        attack.SetInputActionsContainer(c);
        jump.SetInputActionsContainer(c);
        lateral.SetInputActionsContainer(c);
        
        Assert.That(jump.InputActionsContainer, Is.EqualTo(c));
        Assert.That(attack.InputActionsContainer, Is.EqualTo(c));
        Assert.That(lateral.InputActionsContainer, Is.EqualTo(c));
        Assert.That(left.InputActionsContainer, Is.EqualTo(c));
        Assert.That(right.InputActionsContainer, Is.EqualTo(c));
        

        Assert.That(right.AxisName, Is.EqualTo("Lateral"));
        Assert.That(left.AxisName, Is.EqualTo("Lateral"));
        Assert.That(lateral.Name, Is.EqualTo("Lateral"));

        Assert.That(right.AxisAction, Is.EqualTo(lateral));
        Assert.That(left.AxisAction, Is.EqualTo(lateral));        
        Assert.That(lateral.Negative, Is.EqualTo(left));
        Assert.That(lateral.Positive, Is.EqualTo(right));

        
        Assert.That(c.InputActionList.Count, Is.EqualTo(5));
        Assert.That(c.FindAction<InputAction>("ManualJump"), Is.EqualTo(jump));
        Assert.That(c.FindAction<InputAction>("ManualAttack"), Is.EqualTo(attack));
        Assert.That(c.FindAction<AxisAction>("Lateral"), Is.EqualTo(lateral));
        Assert.That(c.FindAction<InputAction>("Right"), Is.EqualTo(right));
        Assert.That(c.FindAction<InputAction>("Left"), Is.EqualTo(left));
    }
    
    [TestRunner.Test]
    public void ManualConfigurationWithSettingTest() {
        var attack = InputAction.Create("ManualAttack").AsSimulator();
        Assert.That(attack.SaveSetting, Is.Null);

        var jump = InputAction.Create("ManualJump").AsSimulator();
        Assert.That(jump.SaveSetting, Is.Null);

        SaveSetting<string> b = Setting.Create("attack","button:A,button:B,key:H,key:F");

        var sc = new SettingsContainer(new ConfigFileWrapper(SettingsFile));
        b.SetSettingsContainer(sc);
        jump.SaveSetting = b;
        jump.Load();
        Assert.That(jump.SaveSetting, Is.EqualTo(b));
            
        Assert.That(jump.Buttons, Is.EqualTo(new [] {JoyButton.A, JoyButton.B}.ToList()));
        Assert.That(jump.Keys, Is.EqualTo(new [] {Key.H, Key.F}.ToList()));
    }
}
