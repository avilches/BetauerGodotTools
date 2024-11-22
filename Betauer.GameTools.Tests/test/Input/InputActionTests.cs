using System;
using System.Linq;
using Betauer.Input;
using Betauer.TestRunner;
using System.Collections.Generic;
using Godot;
using NUnit.Framework;

namespace Betauer.GameTools.Tests.Input; 

[TestFixture]
public class InputActionTests {

    const string SettingsFile = "./action-test-settings.ini";

    [OneTimeSetUp]
    [TearDown]
    public void Clear() {
        System.IO.File.Delete(SettingsFile);
        new List<string> { "Jump", "Left", "Right", "Attack" }.ForEach(stringName => {
            if (InputMap.HasAction(stringName)) {
                InputMap.EraseAction(stringName);
            }
        });
    }

    [Test]
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
    
    [Test]
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

    [Test]
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

    [Test]
    public void ImportJoypadButtonTest() {
        var reg = InputAction.Create("N").Build();
        Assert.That(reg.Buttons, Is.Empty);

        reg.Update(u => u.ImportJoypad("a:2,,Button:asdas,Button:230"));
        Assert.That(reg.Buttons, Is.Empty);
        // Assert.That(reg.Export(true, true, true), Is.EqualTo(""));

        reg.AllowMultipleButtons = true;
        reg.Update(u => u.ImportJoypad("a:2,,Button:A,BUTTON:1"));
        Assert.That(reg.HasButton(JoyButton.A));
        Assert.That(reg.HasButton(JoyButton.B));
        // Assert.That(reg.Export(true, true, true), Is.EqualTo("Button:A,Button:B"));

        reg.AllowMultipleButtons = false;
        reg.Update(u => u.ImportJoypad("a:2,,Button:A,BUTTON:1"));
        Assert.That(reg.HasButton(JoyButton.A));
        Assert.That(!reg.HasButton(JoyButton.B));
        // Assert.That(reg.Export(true, true, true), Is.EqualTo("Button:A"));
    }

    [Test]
    public void ImportJoypadAxisTest() {
        var reg = InputAction.Create("N").Build();
        Assert.That(reg.Axis, Is.EqualTo(JoyAxis.Invalid));
        Assert.That(reg.DeadZone, Is.EqualTo(InputAction.DefaultDeadZone));

        reg.AllowMultipleButtons = true;
        reg.Update(u => u.ImportJoypad("a:2,,Axis:asdas"));
        Assert.That(reg.Axis, Is.EqualTo(JoyAxis.Invalid));
        Assert.That(reg.DeadZone, Is.EqualTo(InputAction.DefaultDeadZone));

        reg.Update(u => u.ImportJoypad("a:2,,Axis:230"));
        Assert.That(reg.Axis, Is.EqualTo(JoyAxis.Invalid));
        Assert.That(reg.DeadZone, Is.EqualTo(InputAction.DefaultDeadZone));

        reg.Update(u => u.ImportJoypad("a:2,,Button:A,Joyaxis:RIGHTX"));
        Assert.That(reg.Axis, Is.EqualTo(JoyAxis.RightX));
        Assert.That(reg.DeadZone, Is.EqualTo(InputAction.DefaultDeadZone));

        reg.IncludeDeadZone = true;
        reg.IncludeAxisSign = true;

        // Deadzone values
        reg.Update(u => u.ImportJoypad("a:2,,Button:A,JoyaxIS:RIGHTX,deadzone:H"));
        Assert.That(reg.Axis, Is.EqualTo(JoyAxis.RightX));
        Assert.That(reg.DeadZone, Is.EqualTo(InputAction.DefaultDeadZone));

        reg.Update(u => u.ImportJoypad("a:2,,Button:A,Joyaxis:RIGHTX,deadzone:-1"));
        Assert.That(reg.Axis, Is.EqualTo(JoyAxis.RightX));
        Assert.That(reg.DeadZone, Is.EqualTo(InputAction.DefaultDeadZone));

        reg.Update(u => u.ImportJoypad("a:2,,Button:A,Joyaxis:RIGHTX,deadzone:-0.1"));
        Assert.That(reg.Axis, Is.EqualTo(JoyAxis.RightX));
        Assert.That(reg.DeadZone, Is.EqualTo(InputAction.DefaultDeadZone));

        reg.Update(u => u.ImportJoypad("a:2,,Button:A,Joyaxis:RIGHTX,deadzone:1.1"));
        Assert.That(reg.Axis, Is.EqualTo(JoyAxis.RightX));
        Assert.That(reg.DeadZone, Is.EqualTo(InputAction.DefaultDeadZone));

        reg.Update(u => u.ImportJoypad("a:2,,Button:A,Joyaxis:RIGHTX,DEADzone:0.2"));
        Assert.That(reg.Axis, Is.EqualTo(JoyAxis.RightX));
        Assert.That(reg.DeadZone, Is.EqualTo(0.2f));

        reg.Update(u => u.ImportJoypad("a:2,,Button:A,Joyaxis:RIGHTX,axisSIGN:POSITIVE"));
        Assert.That(reg.Axis, Is.EqualTo(JoyAxis.RightX));
        Assert.That(reg.DeadZone, Is.EqualTo(0.2f));
        Assert.That(reg.AxisSign, Is.EqualTo(InputAction.AxisSignEnum.Positive));

        reg.Update(u => u.ImportJoypad("a:2,,Button:A,axisSIGN:NEGATIVE"));
        Assert.That(reg.Axis, Is.EqualTo(JoyAxis.Invalid));
        Assert.That(reg.DeadZone, Is.EqualTo(0.2f));
        Assert.That(reg.AxisSign, Is.EqualTo(InputAction.AxisSignEnum.Negative));

        reg.Update(u => u.ImportJoypad("a:2,,Joyaxis:RIGHTX,axisSIGN:Positive"));
        Assert.That(reg.Axis, Is.EqualTo(JoyAxis.RightX));
        Assert.That(reg.DeadZone, Is.EqualTo(0.2f));
        Assert.That(reg.AxisSign, Is.EqualTo(InputAction.AxisSignEnum.Positive));

        // In case of error, sign remains with its old value
        reg.Update(u => u.ImportJoypad("a:2,Joyaxis:RIGHTX,axisSIGN:1"));
        Assert.That(reg.Axis, Is.EqualTo(JoyAxis.RightX));
        Assert.That(reg.DeadZone, Is.EqualTo(0.2f));
        Assert.That(reg.AxisSign, Is.EqualTo(InputAction.AxisSignEnum.Positive));

        reg.IncludeDeadZone = false;
        reg.IncludeAxisSign = false;
        // Ignore deadzone and sign if false
        reg.Update(u => u.ImportJoypad("a:2,Joyaxis:RIGHTX,axisSIGN:Negative,DEADzone:0.5"));
        Assert.That(reg.Axis, Is.EqualTo(JoyAxis.RightX));
        Assert.That(reg.DeadZone, Is.EqualTo(0.2f));
        Assert.That(reg.AxisSign, Is.EqualTo(InputAction.AxisSignEnum.Positive));

    }

    [Test]
    public void ImportKeysTest() {
        var reg = InputAction.Create("N").Build();

        reg.AllowMultipleKeys = true;
        reg.IncludeModifiers = true;

        reg.Update(u => u.ImportKeys("a:2,,Key:asdas,Key:2"));
        Assert.That(reg.Keys, Is.Empty);
        // Assert.That(reg.Export(true), Is.EqualTo(""));

        reg.Update(u => u.ImportKeys("a:2,,KEY:A,Key:76"));
        Assert.That(reg.HasKey(Key.A));
        Assert.That(reg.HasKey(Key.L));
        // Assert.That(reg.Export(true), Is.EqualTo("Key:A,Key:L"));

        reg.AllowMultipleKeys = false;
        reg.Update(u => u.ImportKeys("a:2,,key:A,key:76"));
        Assert.That(reg.HasKey(Key.A));
        Assert.That(!reg.HasKey(Key.L));
        // Assert.That(reg.Export(true), Is.EqualTo("Key:A"));

        reg.Update(u => {
            u.ClearModifiers();
            u.ImportKeys("a:2,,ctrl,alt,meta,shift");
        });
        Assert.That(reg.Ctrl, Is.True);
        Assert.That(reg.Alt, Is.True);
        Assert.That(reg.Meta, Is.True);
        Assert.That(reg.Shift, Is.True);

        reg.IncludeModifiers = false;
        reg.Update(u => {
            u.ClearModifiers();
            u.ImportKeys("a:2,,ctrl,alt,meta,shift");
        });
        Assert.That(reg.Ctrl, Is.False);
        Assert.That(reg.Alt, Is.False);
        Assert.That(reg.Meta, Is.False);
        Assert.That(reg.Shift, Is.False);
    }

    [Test]
    public void ImportMouseTest() {
        var reg = InputAction.Create("N").Build();
        Assert.That(reg.MouseButton == MouseButton.None);
        
        reg.IncludeModifiers = true;
        reg.Update(u => u.ImportMouse("MOUSE:LEFT"));
        Assert.That(reg.MouseButton == MouseButton.Left);
        
        reg.Update(u => u.ImportMouse("mouse:right"));
        Assert.That(reg.MouseButton == MouseButton.Right);
        
        reg.Update(u => u.ImportMouse("mouse:xx"));
        Assert.That(reg.MouseButton == MouseButton.None);
        
        reg.Update(u => u.ImportMouse("mouse:3"));
        Assert.That(reg.MouseButton == MouseButton.Middle);

        reg.IncludeModifiers = false;
        reg.Update(u => {
            u.ClearModifiers();
            u.ImportMouse("a:2,,ctrl,alt,meta,shift");
        });
        Assert.That(reg.Ctrl, Is.False);
        Assert.That(reg.Alt, Is.False);
        Assert.That(reg.Meta, Is.False);
        Assert.That(reg.Shift, Is.False);

        reg.IncludeModifiers = true;
        reg.Update(u => {
            u.ClearModifiers();
            u.ImportMouse("a:2,,ctrl,alt,meta,shift");
        });
        Assert.That(reg.Ctrl, Is.True);
        Assert.That(reg.Alt, Is.True);
        Assert.That(reg.Meta, Is.True);
        Assert.That(reg.Shift, Is.True);
    }

    [Test]
    public void ImportLateralTest() {
        var lateral = AxisAction.Create("Lateral").Build();
        lateral.Import("Reverse:True", true);
        Assert.That(lateral.Reverse, Is.EqualTo(true));

        lateral.Import("Reverse:false", true);
        Assert.That(lateral.Reverse, Is.EqualTo(false));
    }

    [Test]
    public void LateralExportImport() {
        Assert.That(AxisAction.Create("Lateral").ReverseAxis(true).Build().Export(), Is.EqualTo("Reverse:True"));
        Assert.That(AxisAction.Create("Lateral").ReverseAxis(false).Build().Export(), Is.EqualTo("Reverse:False"));
    }

    [Test]
    public void InputActionExportImport() {
        Assert.That(InputAction.Create("N").Simulator().Export(), Is.EqualTo(""));

        var complete = InputAction.Create("N")
            .Keys(Key.A)
            .Keys(Key.B)
            .Mouse(MouseButton.Left)
            .Buttons(JoyButton.A, JoyButton.B)
            .NegativeAxis(JoyAxis.LeftX, true)
            .DeadZone(0.2f, true)
            .Ctrl()
            .Meta()
            .Shift()
            .SaveAs("Setting/N")
            .IncludeModifiers()
            .Build();

        Assert.That(complete.Export(), Is.EqualTo("Button:A,Button:B,JoyAxis:LeftX,AxisSign:Negative,DeadZone:0.2,Key:A,Key:B,Mouse:Left,Shift,Ctrl,Meta"));

        var imported = InputAction.Create("N").Build();
        imported.IncludeAxisSign = true;
        imported.IncludeDeadZone = true;
        imported.IncludeModifiers = true;
        imported.Update(u => {
            u.ImportKeys(complete.Export());
            u.ImportMouse(complete.Export());
            u.ImportJoypad(complete.Export());
        });

        Assert.That(imported.Export(), Is.EqualTo(complete.Export()));
        
        var imported2 = InputAction.Create("N").Build();
        imported2.IncludeAxisSign = false;
        imported2.IncludeDeadZone = false;
        imported2.IncludeModifiers = false;
        imported2.Update(u => {
            u.ImportKeys(complete.Export());
            u.ImportMouse(complete.Export());
            u.ImportJoypad(complete.Export());
        });

        Assert.That(imported2.Export(), Is.EqualTo("Button:A,Button:B,JoyAxis:LeftX,Key:A,Key:B,Mouse:Left"));
        imported2.AllowMultipleButtons = false;
        imported2.AllowMultipleKeys = false;
        Assert.That(imported2.Export(), Is.EqualTo("Button:A,JoyAxis:LeftX,Key:A,Mouse:Left"));

        var imported3 = InputAction.Create("N").Build();
        imported3.IncludeAxisSign = false;
        imported3.IncludeDeadZone = false;
        imported3.IncludeModifiers = false;
        imported3.AllowMultipleButtons = false;
        imported3.AllowMultipleKeys = false;
        imported3.Update(u => {
            u.ImportKeys(complete.Export());
            u.ImportMouse(complete.Export());
            u.ImportJoypad(complete.Export());
        });

        Assert.That(imported3.Export(), Is.EqualTo("Button:A,JoyAxis:LeftX,Key:A,Mouse:Left"));
    }
    
    [Test]
    public void UpdateRollback() {
        var complete = InputAction.Create("N")
            .Keys(Key.A)
            .Keys(Key.B)
            .Mouse(MouseButton.Left)
            .Buttons(JoyButton.A, JoyButton.B)
            .NegativeAxis(JoyAxis.LeftX, true)
            .DeadZone(0.2f, true)
            .Ctrl()
            .Meta()
            .Shift()
            .SaveAs("Setting/N")
            .IncludeModifiers()
            .Build();

        var backup = complete.Export();

        // Configure and save
        complete.Update(u => {
            u.AddKeys(Key.Exclam);
            u.AddButtons(JoyButton.Paddle1, JoyButton.X);
            u.SetAxis(JoyAxis.RightX);
            u.SetDeadZone(0.8f);
            u.ClearModifiers();
            throw new Exception();
        });
        Assert.That(complete.Export(), Is.EqualTo(backup));
    }

}
