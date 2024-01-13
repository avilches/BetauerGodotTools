using System;
using System.Linq;
using Betauer.Application;
using Betauer.Application.Settings;
using Betauer.Input;
using Betauer.TestRunner;
using System.Collections.Generic;
using Godot;
using NUnit.Framework;

namespace Betauer.GameTools.Tests.Input; 

[TestRunner.Test]
public class InputActionTests {

    const string SettingsFile = "./action-test-settings.ini";

    [SetUpClass]
    [TestRunner.TearDown]
    public void Clear() {
        System.IO.File.Delete(SettingsFile);
        new List<string> { "Jump", "Left", "Right", "Attack" }.ForEach(stringName => {
            if (InputMap.HasAction(stringName)) {
                InputMap.EraseAction(stringName);
            }
        });
    }

    [TestRunner.Test]
    public void BuilderMockTests() {
        var empty = InputAction.Create("N").Simulator();
        Assert.That(empty.Name, Is.EqualTo("N"));
        Assert.That(empty.Keys, Is.Empty);
        Assert.That(empty.Buttons, Is.Empty);
        Assert.That(InputMap.HasAction("N"), Is.False);
        Assert.That(empty.Enabled, Is.False);
        empty.Enable();
        Assert.That(empty.Enabled, Is.True);
        Assert.That(InputMap.HasAction("N"), Is.False);
    }
    
    [TestRunner.Test]
    public void BuilderTests() {
        var reg = InputAction.Create("N")
            .Keys(Key.A)
            .Keys(Key.K, Key.Alt)
            .Mouse(MouseButton.Left)
            .Buttons(JoyButton.A)
            .Buttons(JoyButton.B, JoyButton.Back)
            .NegativeAxis(JoyAxis.LeftX)
            .Ctrl()
            .Build();
        Assert.That(reg.Name, Is.EqualTo("N"));
        Assert.That(reg.Keys, Is.EqualTo(new [] {Key.A, Key.K, Key.Alt}.ToList()));
        Assert.That(reg.Buttons, Is.EqualTo(new [] {JoyButton.A, JoyButton.B, JoyButton.Back}.ToList()));
        
        Assert.That(InputMap.HasAction("N"), Is.False);
        Assert.That(reg.Enabled, Is.False);
        
        reg.Enable();
        Assert.That(reg.Enabled, Is.True);
        Assert.That(InputMap.HasAction("N"), Is.True);

        Assert.That(InputMap.ActionGetEvents("N").OfType<InputEventKey>().Count(), Is.EqualTo(3));
        Assert.That(InputMap.ActionGetEvents("N").OfType<InputEventKey>().First(e => e.Keycode == Key.A) != null);
        Assert.That(InputMap.ActionGetEvents("N").OfType<InputEventKey>().First(e => e.Keycode == Key.K) != null);
        Assert.That(InputMap.ActionGetEvents("N").OfType<InputEventKey>().First(e => e.Keycode == Key.Alt) != null);
        
        Assert.That(InputMap.ActionGetEvents("N").OfType<InputEventJoypadButton>().Count(), Is.EqualTo(3));
        Assert.That(InputMap.ActionGetEvents("N").OfType<InputEventJoypadButton>().First(e => e.ButtonIndex == JoyButton.A) != null);
        Assert.That(InputMap.ActionGetEvents("N").OfType<InputEventJoypadButton>().First(e => e.ButtonIndex == JoyButton.B) != null);
        Assert.That(InputMap.ActionGetEvents("N").OfType<InputEventJoypadButton>().First(e => e.ButtonIndex == JoyButton.Back) != null);
        Assert.That(InputMap.ActionGetEvents("N").OfType<InputEventJoypadButton>().All(e => e.Device == -1));
        
        Assert.That(InputMap.ActionGetEvents("N").OfType<InputEventJoypadMotion>().Count(), Is.EqualTo(1));
        Assert.That(InputMap.ActionGetEvents("N").OfType<InputEventJoypadMotion>().First(e => e.Axis == JoyAxis.LeftX) != null);
        Assert.That(InputMap.ActionGetEvents("N").OfType<InputEventJoypadMotion>().First(e => e.AxisValue == -1f) != null);
        Assert.That(InputMap.ActionGetEvents("N").OfType<InputEventJoypadMotion>().First(e => e.Device == -1) != null);

        Assert.That(InputMap.ActionGetEvents("N").OfType<InputEventJoypadMotion>().Count(), Is.EqualTo(1));
        Assert.That(InputMap.ActionGetEvents("N").OfType<InputEventJoypadMotion>().First(e => e.Axis == JoyAxis.LeftX) != null);

        Assert.That(InputMap.ActionGetEvents("N").OfType<InputEventMouseButton>().Count(), Is.EqualTo(1));
        Assert.That(InputMap.ActionGetEvents("N").OfType<InputEventMouseButton>().First(e => e.ButtonIndex == MouseButton.Left) != null);
        
        // Only the mouse and key events have modifiers, so 3 for the keys + 1 for the mouse = 4
        Assert.That(InputMap.ActionGetEvents("N").OfType<InputEventWithModifiers>().Count( e => e.CtrlPressed), Is.EqualTo(4));

    }

    [TestRunner.Test]
    public void InputActionGodotInputMap() {
        var attack = InputAction.Create("Attack").Build();
        Assert.That(attack.Enabled, Is.False);
        Assert.That(InputMap.HasAction("Attack"), Is.False);
        attack.Enable();
        Assert.That(attack.Enabled, Is.True);
        Assert.That(InputMap.HasAction("Attack"), Is.True);

        
        var left = InputAction.Create("Left").NegativeAxis(JoyAxis.LeftX).Build();
        var right = InputAction.Create("Right").PositiveAxis(JoyAxis.LeftX).Build();
        var lateral = new AxisAction("Lateral");
        lateral.Disable();
        lateral.SetNegativeAndPositive(left, right);
        
        Assert.That(left.Enabled, Is.False);
        Assert.That(right.Enabled, Is.False);
        Assert.That(InputMap.HasAction("Right"), Is.False);
        Assert.That(InputMap.HasAction("Left"), Is.False);
        
        lateral.Enable();
        Assert.That(left.Enabled, Is.True);
        Assert.That(right.Enabled, Is.True);
        Assert.That(InputMap.HasAction("Right"), Is.True);
        Assert.That(InputMap.HasAction("Left"), Is.True);
        
        lateral.Disable();
        Assert.That(left.Enabled, Is.False);
        Assert.That(right.Enabled, Is.False);
        Assert.That(InputMap.HasAction("Right"), Is.False);
        Assert.That(InputMap.HasAction("Left"), Is.False);
    }

    [TestRunner.Test]
    public void InputActionSaveTest() {
        SaveSetting<string> b = Setting.Create("attack", "", false);
        var sc = new SettingsContainer(new ConfigFileWrapper(SettingsFile));
        sc.Add(b);

        var reg = InputAction.Create("N")
            .Keys(Key.A)
            .Simulator();

        reg.SaveSetting = b;
        reg.Save();

        sc.ConfigFileWrapper.GetValue("Setting/attack", "nop");
        Assert.That(sc.ConfigFileWrapper.GetValue("Settings/attack", "nop"), Is.EqualTo(reg.Export()));
    }

    [TestRunner.Test]
    public void CreateSaveSettingTest() {
        var sc = new SettingsContainer(new ConfigFileWrapper(SettingsFile));

        var reg = InputAction.Create("N")
            .Keys(Key.A)
            .Build();

        reg.CreateSaveSetting(sc, "Setting/N");
        Assert.That(reg.SaveSetting.SaveAs, Is.EqualTo("Setting/N"));
        Assert.That(reg.SaveSetting.DefaultValue, Is.EqualTo(reg.Export()));
        Assert.That(reg.SaveSetting.AutoSave, Is.True);
        reg.Save();

        Assert.That(sc.ConfigFileWrapper.GetValue("Setting/N", "nop"), Is.EqualTo(reg.Export()));
    }

    [TestRunner.Test]
    public void CreateSaveSettingTest2() {
        var sc = new SettingsContainer(new ConfigFileWrapper(SettingsFile));

        var reg = InputAction.Create("N")
            .SaveAs("Setting/N")
            .Keys(Key.A)
            .Build();

        reg.CreateSaveSetting(sc);
        Assert.That(reg.SaveSetting.SaveAs, Is.EqualTo("Setting/N"));
        Assert.That(reg.SaveSetting.DefaultValue, Is.EqualTo(reg.Export()));
        Assert.That(reg.SaveSetting.AutoSave, Is.True);
        reg.Save();

        Assert.That(sc.ConfigFileWrapper.GetValue("Setting/N", "nop"), Is.EqualTo(reg.Export()));
    }

    [TestRunner.Test]
    public void ImportJoypadButtonTest() {
        var reg = InputAction.Create("N").Build();
        Assert.That(reg.Buttons, Is.Empty);

        reg.Update(u => u.ImportJoypad("a:2,,Button:asdas,Button:230", true));
        Assert.That(reg.Buttons, Is.Empty);
        // Assert.That(reg.Export(true, true, true), Is.EqualTo(""));

        reg.Update(u => u.ImportJoypad("a:2,,Button:A,BUTTON:1", true));
        Assert.That(reg.HasButton(JoyButton.A));
        Assert.That(reg.HasButton(JoyButton.B));
        // Assert.That(reg.Export(true, true, true), Is.EqualTo("Button:A,Button:B"));

        // No multiple
        reg.Update(u => u.ImportJoypad("a:2,,Button:A,BUTTON:1", false));
        Assert.That(reg.HasButton(JoyButton.A));
        Assert.That(!reg.HasButton(JoyButton.B));
        // Assert.That(reg.Export(true, true, true), Is.EqualTo("Button:A"));
    }

    [TestRunner.Test]
    public void ImportJoypadAxisTest() {
        var reg = InputAction.Create("N").Build();
        Assert.That(reg.Axis, Is.EqualTo(JoyAxis.Invalid));
        Assert.That(reg.DeadZone, Is.EqualTo(InputAction.DefaultDeadZone));

        reg.Update(u => u.ImportJoypad("a:2,,Axis:asdas", true, true, true));
        Assert.That(reg.Axis, Is.EqualTo(JoyAxis.Invalid));
        Assert.That(reg.DeadZone, Is.EqualTo(InputAction.DefaultDeadZone));

        reg.Update(u => u.ImportJoypad("a:2,,Axis:230", true, true, true));
        Assert.That(reg.Axis, Is.EqualTo(JoyAxis.Invalid));
        Assert.That(reg.DeadZone, Is.EqualTo(InputAction.DefaultDeadZone));

        reg.Update(u => u.ImportJoypad("a:2,,Button:A,Joyaxis:RIGHTX", true, true, true));
        Assert.That(reg.Axis, Is.EqualTo(JoyAxis.RightX));
        Assert.That(reg.DeadZone, Is.EqualTo(InputAction.DefaultDeadZone));

        // Deadzone values
        reg.Update(u => u.ImportJoypad("a:2,,Button:A,JoyaxIS:RIGHTX,deadzone:H", true, true, true));
        Assert.That(reg.Axis, Is.EqualTo(JoyAxis.RightX));
        Assert.That(reg.DeadZone, Is.EqualTo(InputAction.DefaultDeadZone));

        reg.Update(u => u.ImportJoypad("a:2,,Button:A,Joyaxis:RIGHTX,deadzone:-1", true, true, true));
        Assert.That(reg.Axis, Is.EqualTo(JoyAxis.RightX));
        Assert.That(reg.DeadZone, Is.EqualTo(InputAction.DefaultDeadZone));

        reg.Update(u => u.ImportJoypad("a:2,,Button:A,Joyaxis:RIGHTX,deadzone:-0.1", true, true, true));
        Assert.That(reg.Axis, Is.EqualTo(JoyAxis.RightX));
        Assert.That(reg.DeadZone, Is.EqualTo(InputAction.DefaultDeadZone));

        reg.Update(u => u.ImportJoypad("a:2,,Button:A,Joyaxis:RIGHTX,deadzone:1.1", true, true, true));
        Assert.That(reg.Axis, Is.EqualTo(JoyAxis.RightX));
        Assert.That(reg.DeadZone, Is.EqualTo(InputAction.DefaultDeadZone));

        reg.Update(u => u.ImportJoypad("a:2,,Button:A,Joyaxis:RIGHTX,DEADzone:0.2", true, true, true));
        Assert.That(reg.Axis, Is.EqualTo(JoyAxis.RightX));
        Assert.That(reg.DeadZone, Is.EqualTo(0.2f));

        reg.Update(u => u.ImportJoypad("a:2,,Button:A,Joyaxis:RIGHTX,axisSIGN:POSITIVE", true, true, true));
        Assert.That(reg.Axis, Is.EqualTo(JoyAxis.RightX));
        Assert.That(reg.DeadZone, Is.EqualTo(0.2f));
        Assert.That(reg.AxisSign, Is.EqualTo(InputAction.AxisSignEnum.Positive));

        reg.Update(u => u.ImportJoypad("a:2,,Button:A,axisSIGN:NEGATIVE", true, true, true));
        Assert.That(reg.Axis, Is.EqualTo(JoyAxis.Invalid));
        Assert.That(reg.DeadZone, Is.EqualTo(0.2f));
        Assert.That(reg.AxisSign, Is.EqualTo(InputAction.AxisSignEnum.Negative));

        reg.Update(u => u.ImportJoypad("a:2,,Joyaxis:RIGHTX,axisSIGN:Positive", true, true, true));
        Assert.That(reg.Axis, Is.EqualTo(JoyAxis.RightX));
        Assert.That(reg.DeadZone, Is.EqualTo(0.2f));
        Assert.That(reg.AxisSign, Is.EqualTo(InputAction.AxisSignEnum.Positive));

        // In case of error, sign remains with its old value
        reg.Update(u => u.ImportJoypad("a:2,Joyaxis:RIGHTX,axisSIGN:1", true, true, true));
        Assert.That(reg.Axis, Is.EqualTo(JoyAxis.RightX));
        Assert.That(reg.DeadZone, Is.EqualTo(0.2f));
        Assert.That(reg.AxisSign, Is.EqualTo(InputAction.AxisSignEnum.Positive));

        // Ignore deadzone and sign if false
        reg.Update(u => u.ImportJoypad("a:2,Joyaxis:RIGHTX,axisSIGN:Negative,DEADzone:0.5", true, false, false));
        Assert.That(reg.Axis, Is.EqualTo(JoyAxis.RightX));
        Assert.That(reg.DeadZone, Is.EqualTo(0.2f));
        Assert.That(reg.AxisSign, Is.EqualTo(InputAction.AxisSignEnum.Positive));

    }

    [TestRunner.Test]
    public void ImportKeysTest() {
        var reg = InputAction.Create("N").Build();

        reg.Update(u => u.ImportKeys("a:2,,Key:asdas,Key:2", true, true));
        Assert.That(reg.Keys, Is.Empty);
        // Assert.That(reg.Export(true, true, true), Is.EqualTo(""));

        reg.Update(u => u.ImportKeys("a:2,,KEY:A,Key:76", true, true));
        Assert.That(reg.HasKey(Key.A));
        Assert.That(reg.HasKey(Key.L));
        // Assert.That(reg.Export(true, true, true), Is.EqualTo("Key:A,Key:L"));

        // No multiple
        reg.Update(u => u.ImportKeys("a:2,,key:A,key:76", false, true));
        Assert.That(reg.HasKey(Key.A));
        Assert.That(!reg.HasKey(Key.L));
        // Assert.That(reg.Export(true, true, true), Is.EqualTo("Key:A"));

        reg.Update(u => {
            u.ClearModifiers();
            u.ImportKeys("a:2,,ctrl,alt,meta,shift", true, true);
        });
        Assert.That(reg.Ctrl, Is.True);
        Assert.That(reg.Alt, Is.True);
        Assert.That(reg.Meta, Is.True);
        Assert.That(reg.Shift, Is.True);

        reg.Update(u => {
            u.ClearModifiers();
            u.ImportKeys("a:2,,ctrl,alt,meta,shift", true, false);
        });
        Assert.That(reg.Ctrl, Is.False);
        Assert.That(reg.Alt, Is.False);
        Assert.That(reg.Meta, Is.False);
        Assert.That(reg.Shift, Is.False);
    }

    [TestRunner.Test]
    public void ImportMouseTest() {
        var reg = InputAction.Create("N").Build();
        Assert.That(reg.MouseButton == MouseButton.None);
        
        reg.Update(u => u.ImportMouse("MOUSE:LEFT", true));
        Assert.That(reg.MouseButton == MouseButton.Left);
        
        reg.Update(u => u.ImportMouse("mouse:right", true));
        Assert.That(reg.MouseButton == MouseButton.Right);
        
        reg.Update(u => u.ImportMouse("mouse:xx", true));
        Assert.That(reg.MouseButton == MouseButton.None);
        
        reg.Update(u => u.ImportMouse("mouse:3", true));
        Assert.That(reg.MouseButton == MouseButton.Middle);

        reg.Update(u => {
            u.ClearModifiers();
            u.ImportMouse("a:2,,ctrl,alt,meta,shift", false);
        });
        Assert.That(reg.Ctrl, Is.False);
        Assert.That(reg.Alt, Is.False);
        Assert.That(reg.Meta, Is.False);
        Assert.That(reg.Shift, Is.False);

        reg.Update(u => {
            u.ClearModifiers();
            u.ImportMouse("a:2,,ctrl,alt,meta,shift", true);
        });
        Assert.That(reg.Ctrl, Is.True);
        Assert.That(reg.Alt, Is.True);
        Assert.That(reg.Meta, Is.True);
        Assert.That(reg.Shift, Is.True);
    }

    [TestRunner.Test]
    public void ImportModifiersTest() {
        var reg = InputAction.Create("N").Build();
        Assert.That(reg.Ctrl, Is.False);
        Assert.That(reg.Alt, Is.False);
        Assert.That(reg.Meta, Is.False);
        Assert.That(reg.Shift, Is.False);

        reg.Update(u => {
            u.ClearModifiers();
            u.ImportModifiers("a:2,,ctrl,alt,meta,shift");
        });
        Assert.That(reg.Ctrl, Is.True);
        Assert.That(reg.Alt, Is.True);
        Assert.That(reg.Meta, Is.True);
        Assert.That(reg.Shift, Is.True);
        
        reg.Update(u => {
            u.ClearModifiers();
        });
        Assert.That(reg.Ctrl, Is.False);
        Assert.That(reg.Alt, Is.False);
        Assert.That(reg.Meta, Is.False);
        Assert.That(reg.Shift, Is.False);

        reg.Update(u => {
            u.ClearModifiers();
            u.ImportModifiers("a:2,,ctrl:truE,alt:TRUE,meta:False,shift:falsE");
        });
        Assert.That(reg.Ctrl, Is.True);
        Assert.That(reg.Alt, Is.True);
        Assert.That(reg.Meta, Is.False);
        Assert.That(reg.Shift, Is.False);
    }
    
    [TestRunner.Test]
    public void ImportLateralTest() {
        var lateral = AxisAction.Create("Lateral").Build();
        lateral.Import("Reverse:True", true);
        Assert.That(lateral.Reverse, Is.EqualTo(true));

        lateral.Import("Reverse:false", true);
        Assert.That(lateral.Reverse, Is.EqualTo(false));
    }

    [TestRunner.Test]
    public void InputActionExportImport() {

        Assert.That(AxisAction.Create("Lateral").ReverseAxis(true).Build().Export(), Is.EqualTo("Reverse:True"));
        Assert.That(AxisAction.Create("Lateral").ReverseAxis(false).Build().Export(), Is.EqualTo("Reverse:False"));

        Assert.That(InputAction.Create("N").Simulator().Export(false, false, false), Is.EqualTo(""));
        Assert.That(InputAction.Create("N").Simulator().Export(true, true, true), Is.EqualTo(""));

        var reg = InputAction.Create("N")
            .Keys(Key.A)
            .Keys(Key.B)
            .Mouse(MouseButton.Left)
            .Buttons(JoyButton.A, JoyButton.B)
            .NegativeAxis(JoyAxis.LeftX)
            .DeadZone(0.2f)
            .Ctrl()
            .Meta()
            .Shift()
            .SaveAs("Setting/N")
            .Simulator();

        Assert.That(reg.Export(false, false, false), Is.EqualTo("Button:A,Button:B,JoyAxis:LeftX,Key:A,Key:B,Mouse:Left"));
        Assert.That(reg.Export(true, true, true), Is.EqualTo("Button:A,Button:B,JoyAxis:LeftX,AxisSign:Negative,DeadZone:0.2,Key:A,Key:B,Mouse:Left,Shift,Ctrl,Meta"));

        var regEmpty = InputAction.Create("N").Build();
        regEmpty.Update(u => {
            u.ImportKeys(reg.Export(true, true, true), true, true);
            u.ImportMouse(reg.Export(true, true, true), true);
            u.ImportJoypad(reg.Export(true, true, true), true, true, true);
        });

        Assert.That(regEmpty.Export(false, false, false), Is.EqualTo(reg.Export(false, false, false)));
        Assert.That(regEmpty.Export(true, true, true), Is.EqualTo(reg.Export(true, true, true)));
        
    }

    [TestRunner.Test]
    public void InputActionImportExportTests() {
        SaveSetting<string> b = Setting.Create( "attack", "");
        var sc = new SettingsContainer(new ConfigFileWrapper(SettingsFile));
        sc.Add(b);
            
        var jump = InputAction.Simulator();
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
        Assert.That(jump.Export(), Is.EqualTo("Button:Paddle1,Button:X,JoyAxis:RightX,Key:A,Key:Exclam"));
        jump.Save();
        Assert.That(sc.ConfigFileWrapper.GetValue("Settings/attack", "nop"), Is.EqualTo(jump.Export()));


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
        SaveSetting<string> b = Setting.Create( "Controls/attack", "");
        var sc = new SettingsContainer(new ConfigFileWrapper(SettingsFile));
        sc.Add(b);
            
        var jump = InputAction.Simulator();
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
        Assert.That(sc.ConfigFileWrapper.GetValue("Controls/attack", "nop"), Is.EqualTo(jump.Export()));

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
        SaveSetting<string> b = Setting.Create( "Lateral", "Reverse:True", false);
        var sc = new SettingsContainer(new ConfigFileWrapper(SettingsFile));
        sc.Add(b);
        var lateral = AxisAction.Mock();
        Assert.That(lateral.Reverse, Is.False);
        
        // Load -> true
        lateral.SaveSetting = b;
        lateral.Load();
        Assert.That(lateral.Reverse, Is.True);

        // false -> Save -> Load -> false again
        lateral.Reverse = false;
        lateral.Save();
        Assert.That(sc.ConfigFileWrapper.GetValue("Settings/Lateral", "nop"), Is.EqualTo(lateral.Export()));
        
        Assert.That(lateral.Reverse, Is.False);
        lateral.Load();
        Assert.That(lateral.Reverse, Is.False);

        lateral.ResetToDefaults();
        Assert.That(lateral.Reverse, Is.True);
    }

    [TestRunner.Test]
    public void AxisActionSave() {
        SaveSetting<string> b = Setting.Create("move", "", false);
        var sc = new SettingsContainer(new ConfigFileWrapper(SettingsFile));
        sc.Add(b);

        var lateral = AxisAction.Mock();
        Assert.That(lateral.Reverse, Is.False);

        lateral.SaveSetting = b;
        lateral.Save();

        Assert.That(sc.ConfigFileWrapper.GetValue("Settings/move", "nop") == lateral.Export());
    }
    
    [TestRunner.Test]
    public void CreateSaveSettingAxisTest() {
        var sc = new SettingsContainer(new ConfigFileWrapper(SettingsFile));

        var reg = AxisAction.Create("N")
            .SaveAs("Setting/Lateral")
            .ReverseAxis()
            .Build();

        reg.CreateSaveSetting(sc, "Setting/Lateral");
        Assert.That(reg.SaveSetting.SaveAs, Is.EqualTo("Setting/Lateral"));
        Assert.That(reg.SaveSetting.DefaultValue, Is.EqualTo(reg.Export()));
        Assert.That(reg.SaveSetting.AutoSave, Is.True);
        reg.Save();

        Assert.That(sc.ConfigFileWrapper.GetValue("Setting/Lateral", "nop"), Is.EqualTo(reg.Export()));
    }

    [TestRunner.Test]
    public void CreateSaveSettingAxisTest2() {
        var sc = new SettingsContainer(new ConfigFileWrapper(SettingsFile));

        var reg = AxisAction.Create("N")
            .SaveAs("Setting/Lateral")
            .ReverseAxis()
            .Build();

        reg.CreateSaveSetting(sc);
        Assert.That(reg.SaveSetting.SaveAs, Is.EqualTo("Setting/Lateral"));
        Assert.That(reg.SaveSetting.DefaultValue, Is.EqualTo(reg.Export()));
        Assert.That(reg.SaveSetting.AutoSave, Is.True);
        reg.Save();

        Assert.That(sc.ConfigFileWrapper.GetValue("Setting/Lateral", "nop"), Is.EqualTo(reg.Export()));
    }


    [TestRunner.Test]
    public void ManualConfigurationWithSettingTest() {
        var attack = InputAction.Create("ManualAttack").Simulator();
        Assert.That(attack.SaveSetting, Is.Null);

        var jump = InputAction.Create("ManualJump").Simulator();
        Assert.That(jump.SaveSetting, Is.Null);

        SaveSetting<string> b = Setting.Create("attack","button:A,button:B,key:H,key:F");

        var sc = new SettingsContainer(new ConfigFileWrapper(SettingsFile));
        sc.Add(b);
        jump.SaveSetting = b;
        jump.Load();
        Assert.That(jump.SaveSetting, Is.EqualTo(b));
            
        Assert.That(jump.Buttons, Is.EqualTo(new [] {JoyButton.A, JoyButton.B}.ToList()));
        Assert.That(jump.Keys, Is.EqualTo(new [] {Key.H, Key.F}.ToList()));
    }
}
