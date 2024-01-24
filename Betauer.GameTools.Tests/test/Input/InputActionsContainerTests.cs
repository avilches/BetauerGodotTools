using System.Linq;
using Betauer.Application;
using Betauer.Application.Settings;
using Betauer.Core;
using Betauer.DI.Attributes;
using Betauer.Input;
using Betauer.TestRunner;
using Godot;
using NUnit.Framework;

namespace Betauer.GameTools.Tests.Input; 

[TestRunner.Test]
public class InputActionContainerTests {

    const string SettingsFile = "./action-test-settings.ini";

    [SetUpClass]
    [TestRunner.TearDown]
    public void Clear() {
        System.IO.File.Delete(SettingsFile);
        InputMap.GetActions().ForEach(InputMap.EraseAction);
    }

    [TestRunner.Test]
    public void InputActionFindByName() {
        var jump = InputAction.Create("Jump").Build();
        var left = InputAction.Create("Left").NegativeAxis(JoyAxis.LeftX).Build();
        var right = InputAction.Create("Right").PositiveAxis(JoyAxis.LeftX).Build();
        var lateral = new AxisAction("Lateral");
        lateral.SetNegativeAndPositive(left, right);
        
        var c = new InputActionsContainer();
        c.Add(jump);
        c.Add(left);
        c.Add(right);
        c.Add(lateral);
        
        Assert.That(c.InputActions.Count, Is.EqualTo(3));
        Assert.That(c.AxisActions.Count, Is.EqualTo(1));


        Assert.That(right.AxisAction, Is.EqualTo(lateral));
        Assert.That(left.AxisAction, Is.EqualTo(lateral));        
        Assert.That(lateral.Negative, Is.EqualTo(left));
        Assert.That(lateral.Positive, Is.EqualTo(right));

        Assert.That(c.GetInputAction("Jump"), Is.EqualTo(jump));
        Assert.That(c.GetAxisAction("Lateral"), Is.EqualTo(lateral));
        Assert.That(c.GetInputAction("Right"), Is.EqualTo(right));
        Assert.That(c.GetInputAction("Left"), Is.EqualTo(left));
    }

    [TestRunner.Test(Description = "Manual binding sets the InputAction.AxisAction and AxisActionName properties")]
    public void AxisActionManualBinding() {
        var left = InputAction.Create("Left").NegativeAxis(JoyAxis.LeftX).Build();
        var right = InputAction.Create("Right").PositiveAxis(JoyAxis.LeftX).Build();
        var lateral = new AxisAction("Lateral");
        lateral.SetNegativeAndPositive(left, right);
        Assert.That(lateral.Name, Is.EqualTo("Lateral"));

        Assert.That(right.AxisAction, Is.EqualTo(lateral));
        Assert.That(left.AxisAction, Is.EqualTo(lateral));        

        Assert.That(right.AxisName, Is.EqualTo(lateral.Name));
        Assert.That(left.AxisName, Is.EqualTo(lateral.Name));
        
        var c = new InputActionsContainer();
        c.Add(lateral);
        Assert.That(c.InputActions.Count, Is.EqualTo(2));
        Assert.That(c.AxisActions.Count, Is.EqualTo(1));
        
    }

    [TestRunner.Test(Description = "AxisAction links positive and negative by name, adding the input actions fist, then the axis action")]
    public void AsisActionBindingRightLeftFirst() {
        var left = InputAction.Create("Left").AxisName("Lateral").NegativeAxis(JoyAxis.LeftX).Build();
        var right = InputAction.Create("Right").AxisName("Lateral").PositiveAxis(JoyAxis.LeftX).Build();
        var lateral = new AxisAction("Lateral");
        var c = new InputActionsContainer();
        c.Add(left);
        c.Add(right);
        c.Add(lateral);
        Assert.That(c.InputActions.Count, Is.EqualTo(2));
        Assert.That(c.AxisActions.Count, Is.EqualTo(1));
        Assert.That(right.AxisAction, Is.EqualTo(lateral));
        Assert.That(left.AxisAction, Is.EqualTo(lateral));        
        Assert.That(lateral.Negative, Is.EqualTo(left));
        Assert.That(lateral.Positive, Is.EqualTo(right));
    }

    [TestRunner.Test(Description = "AxisAction links positive and negative by name, adding the axis action first, then the input actions")]
    public void AsisActionBindingLateralFirst() {
        var left = InputAction.Create("Left").AxisName("Lateral").NegativeAxis(JoyAxis.LeftX).Build();
        var right = InputAction.Create("Right").AxisName("Lateral").PositiveAxis(JoyAxis.LeftX).Build();
        var lateral = new AxisAction("Lateral");
        var c = new InputActionsContainer();
        c.Add(lateral);
        c.Add(left);
        c.Add(right);
        Assert.That(c.InputActions.Count, Is.EqualTo(2));
        Assert.That(c.AxisActions.Count, Is.EqualTo(1));
        Assert.That(right.AxisAction, Is.EqualTo(lateral));
        Assert.That(left.AxisAction, Is.EqualTo(lateral));        
        Assert.That(lateral.Negative, Is.EqualTo(left));
        Assert.That(lateral.Positive, Is.EqualTo(right));
    }
    
    [Singleton]
    public class Test1 : InputActionsContainer {
        public InputAction Jump { get; } = InputAction.Create("Jump").Buttons(JoyButton.B).Keys(Key.A).SaveAs("Setting/Jump").Build();
        public InputAction Attack { get; } = InputAction.Create("Attack").Buttons(JoyButton.B).Build();
        public InputAction Left { get; } = InputAction.Create("Left").AxisName("Lateral").NegativeAxis(JoyAxis.LeftX).Build();
        public AxisAction Lateral { get; } = AxisAction.Create("Lateral").SaveAs("Setting/Lateral").Build();
        public InputAction Right { get; } = InputAction.Create("Right").AxisName("Lateral").PositiveAxis(JoyAxis.LeftX).Build();
    }

    [TestRunner.Test(Description = "Regular load from instance. No SettingContainer")]
    public void InputActionLoadInstanceTest() {
        var t = new Test1();
        t.AddActionsFromProperties(t);
        
        t.Attack.JoypadId = 2;

        Assert.That(t.InputActions.Count, Is.EqualTo(4));
        Assert.That(t.AxisActions.Count, Is.EqualTo(1));

        Assert.That(t.Right.AxisAction, Is.EqualTo(t.Lateral));
        Assert.That(t.Left.AxisAction, Is.EqualTo(t.Lateral));        
        Assert.That(t.Lateral.Negative, Is.EqualTo(t.Left));
        Assert.That(t.Lateral.Positive, Is.EqualTo(t.Right));

        Assert.That(t.GetInputAction("Jump"), Is.EqualTo(t.Jump));
        Assert.That(t.GetAxisAction("Lateral"), Is.EqualTo(t.Lateral));
        Assert.That(t.GetInputAction("Right"), Is.EqualTo(t.Right));
        Assert.That(t.GetInputAction("Left"), Is.EqualTo(t.Left));
        
        Assert.That(InputMap.HasAction("Jump"), Is.False);
        Assert.That(InputMap.HasAction("Attack"), Is.False);
        Assert.That(InputMap.HasAction("Right"), Is.False);
        Assert.That(InputMap.HasAction("Left"), Is.False);

        t.EnableAll();
        Assert.That(InputMap.HasAction("Jump"), Is.True);
        Assert.That(InputMap.ActionGetEvents("Jump").OfType<InputEventJoypadButton>().ToList()[0].ButtonIndex, Is.EqualTo(JoyButton.B));
        Assert.That(InputMap.ActionGetEvents("Jump").OfType<InputEventKey>().ToList()[0].Keycode, Is.EqualTo(Key.A));

        Assert.That(InputMap.HasAction("Attack"), Is.True);
        Assert.That(InputMap.ActionGetEvents("Attack").OfType<InputEventJoypadButton>().ToList()[0].Device, Is.EqualTo(2));

        Assert.That(InputMap.HasAction("Right"), Is.True);
        Assert.That(InputMap.ActionGetEvents("Right").OfType<InputEventJoypadMotion>().ToList()[0].Device, Is.EqualTo(-1));
        Assert.That(InputMap.ActionGetEvents("Right").OfType<InputEventJoypadMotion>().ToList()[0].Axis, Is.EqualTo(JoyAxis.LeftX));
        Assert.That(InputMap.ActionGetEvents("Right").OfType<InputEventJoypadMotion>().ToList()[0].AxisValue, Is.EqualTo(1));
        
        Assert.That(InputMap.HasAction("Left"), Is.True);
        Assert.That(InputMap.ActionGetEvents("Left").OfType<InputEventJoypadMotion>().ToList()[0].Device, Is.EqualTo(-1));
        Assert.That(InputMap.ActionGetEvents("Right").OfType<InputEventJoypadMotion>().ToList()[0].Axis, Is.EqualTo(JoyAxis.LeftX));
        Assert.That(InputMap.ActionGetEvents("Left").OfType<InputEventJoypadMotion>().ToList()[0].AxisValue, Is.EqualTo(-1));
        
        t.DisableAll();
        Assert.That(InputMap.HasAction("Jump"), Is.False);
        Assert.That(InputMap.HasAction("Attack"), Is.False);
        Assert.That(InputMap.HasAction("Right"), Is.False);
        Assert.That(InputMap.HasAction("Left"), Is.False);
    }

    public class Test2 {
        public InputAction Jump { get; } = InputAction.Create("Jump").Buttons(JoyButton.B).Keys(Key.A).SaveAs("Setting/Jump").Build();
        public InputAction Attack { get; } = InputAction.Create("Attack").Buttons(JoyButton.B).Build();
        public InputAction Left { get; } = InputAction.Create("Left").AxisName("Lateral").NegativeAxis(JoyAxis.LeftX).Build();
        public AxisAction Lateral { get; } = AxisAction.Create("Lateral").SaveAs("Setting/Lateral").Build();
        public InputAction Right { get; } = InputAction.Create("Right").AxisName("Lateral").PositiveAxis(JoyAxis.LeftX).Build();
    }

    [TestRunner.Test(Description = "With SettingContainer")]
    public void InputActionLoadInstanceContainerTest() {
        var sc = new SettingsContainer(new ConfigFileWrapper(SettingsFile));
        var t = new Test2();
        var iac = new InputActionsContainer();
        iac.AddActionsFromProperties(t);
        iac.ConfigureSaveSettings(sc);
        
        Assert.That(iac.InputActions.Count, Is.EqualTo(4));
        Assert.That(iac.AxisActions.Count, Is.EqualTo(1));
        Assert.That(iac.SettingsContainer!.Settings.Count, Is.EqualTo(2));
    }
    
    [TestRunner.Test(Description = "With SettingContainer REVERSE, first configure save settings, then add from instance")]
    public void InputActionLoadInstanceContainerReverseTest() {
        var sc = new SettingsContainer(new ConfigFileWrapper(SettingsFile));
        var t = new Test2();
        var iac = new InputActionsContainer();
        iac.ConfigureSaveSettings(sc);
        iac.AddActionsFromProperties(t);
        
        Assert.That(iac.InputActions.Count, Is.EqualTo(4));
        Assert.That(iac.AxisActions.Count, Is.EqualTo(1));
        Assert.That(iac.SettingsContainer!.Settings.Count, Is.EqualTo(2));
    }

    
    [TestRunner.Test(Description = "With SettingContainer, update and save")]
    public void UpdateAndSaveTest() {
        var o = new ConfigFileWrapper(SettingsFile);
        var sc = new SettingsContainer(new ConfigFileWrapper(SettingsFile));
        var t = new Test2();
        var iac = new InputActionsContainer();
        iac.ConfigureSaveSettings(sc);
        iac.AddActionsFromProperties(t);

        // If Save() for the very first time, the default values are saved
        iac.Save();
        o.Load();
        Assert.That(o.GetValue("Setting/Jump", "nop"), Is.EqualTo("Button:B,Key:A"));
        Assert.That(o.GetValue("Setting/Lateral", "nop"), Is.EqualTo("Reverse:False"));

        // When update the values and Save()
        t.Jump.Update(u=> u.ClearButtons().ClearKeys().SetMouse(MouseButton.Left));
        t.Lateral.Reverse = true;
        Assert.That(t.Jump.Export(), Is.EqualTo("Mouse:Left"));
        Assert.That(t.Lateral.Export(), Is.EqualTo("Reverse:True"));
        iac.Save();
        // The values are saved
        o.Load();
        Assert.That(o.GetValue("Setting/Jump", "nop"), Is.EqualTo("Mouse:Left"));
        Assert.That(o.GetValue("Setting/Lateral", "nop"), Is.EqualTo("Reverse:True"));
        
        // When update the values and Load()
        t.Jump.Update(u=> u.ClearMouse().SetKey(Key.K));
        t.Lateral.Reverse = false;
        Assert.That(t.Jump.Export(), Is.EqualTo("Key:K"));
        Assert.That(t.Lateral.Export(), Is.EqualTo("Reverse:False"));
        iac.Load();
        
        // The values are lost and previous values are restored
        Assert.That(t.Jump.Export(), Is.EqualTo("Mouse:Left"));
        Assert.That(t.Lateral.Export(), Is.EqualTo("Reverse:True"));
        o.Load();
        Assert.That(o.GetValue("Setting/Jump", "nop"), Is.EqualTo("Mouse:Left"));
        Assert.That(o.GetValue("Setting/Lateral", "nop"), Is.EqualTo("Reverse:True"));

        // When restore defaults, Inputs recover the default values from the SaveSetting
        // but they are not saved yet
        iac.RestoreDefaults();
        Assert.That(t.Jump.Export(), Is.EqualTo("Button:B,Key:A"));
        Assert.That(t.Lateral.Export(), Is.EqualTo("Reverse:False"));
        o.Load();
        Assert.That(o.GetValue("Setting/Jump", "nop"), Is.EqualTo("Mouse:Left"));
        Assert.That(o.GetValue("Setting/Lateral", "nop"), Is.EqualTo("Reverse:True"));

        // When restore defaults and save, default values are saved
        iac.RestoreDefaults();
        iac.Save();
        Assert.That(t.Jump.Export(), Is.EqualTo("Button:B,Key:A"));
        Assert.That(t.Lateral.Export(), Is.EqualTo("Reverse:False"));
        o.Load();
        Assert.That(o.GetValue("Setting/Jump", "nop"), Is.EqualTo("Button:B,Key:A"));
        Assert.That(o.GetValue("Setting/Lateral", "nop"), Is.EqualTo("Reverse:False"));

    }
}
