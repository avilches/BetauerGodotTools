using System.Linq;
using Betauer.Application;
using Betauer.Application.Settings;
using Betauer.Core;
using Betauer.DI.Attributes;
using Betauer.DI.ServiceProvider;
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
        public InputAction Attack { get; } = InputAction.Create("Attack").Buttons(JoyButton.B).JoypadId(2).Build();
        public InputAction Left { get; } = InputAction.Create("Left").AxisName("Lateral").NegativeAxis(JoyAxis.LeftX).Build();
        public AxisAction Lateral { get; } = AxisAction.Create("Lateral").SaveAs("Setting/Lateral").Build();
        public InputAction Right { get; } = InputAction.Create("Right").AxisName("Lateral").PositiveAxis(JoyAxis.LeftX).Build();
    }

    [TestRunner.Test(Description = "Regular load from instance. No SettingContainer")]
    public void InputActionLoadInstanceTest() {
        var t = new Test1();
        t.AddFromInstanceProperties(t);

        Assert.That(t.Jump.SaveSetting, Is.Null);
        Assert.That(t.Attack.SaveSetting, Is.Null);
        Assert.That(t.Left.SaveSetting, Is.Null);
        Assert.That(t.Right.SaveSetting, Is.Null);
        Assert.That(t.Lateral.SaveSetting, Is.Null);
        
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

    [Singleton]
    public class Test2 : SaveSettingsContainerAware {
        public InputAction Jump { get; } = InputAction.Create("Jump").Buttons(JoyButton.B).Keys(Key.A).SaveAs("Setting/Jump").Build();
        public InputAction Attack { get; } = InputAction.Create("Attack").Buttons(JoyButton.B).JoypadId(2).Build();
        public InputAction Left { get; } = InputAction.Create("Left").AxisName("Lateral").NegativeAxis(JoyAxis.LeftX).Build();
        public AxisAction Lateral { get; } = AxisAction.Create("Lateral").SaveAs("Setting/Lateral").Build();
        public InputAction Right { get; } = InputAction.Create("Right").AxisName("Lateral").PositiveAxis(JoyAxis.LeftX).Build();
        
        [Inject] public SettingsContainer SettingsContainer { get; set;  }
    }

    [TestRunner.Test(Description = "SettingContainer injected, only the Jump and Lateral actions will have a SaveSetting")]
    public void InputActionLoadInstanceContainerTest() {
        var sc = new SettingsContainer(new ConfigFileWrapper(SettingsFile));
        var c = new Betauer.DI.Container().Build(b => {
            b.Scan<Test2>();
            b.Register(Provider.Static(sc));
        });
        var t = c.Resolve<Test2>();

        var iac = new InputActionsContainer();
        iac.AddFromInstanceProperties(t);
        Assert.That(iac.InputActions.Count, Is.EqualTo(4));
        Assert.That(iac.AxisActions.Count, Is.EqualTo(1));
        
        Assert.That(t.Attack.SaveSetting, Is.Null);
        Assert.That(t.Left.SaveSetting, Is.Null);
        Assert.That(t.Right.SaveSetting, Is.Null);
        
        Assert.That(t.Jump.SaveSetting.SaveAs, Is.EqualTo("Setting/Jump"));
        Assert.That(t.Jump.SaveSetting.DefaultValue, Is.EqualTo(t.Jump.AsString()));
        Assert.That(t.Jump.SaveSetting.AutoSave, Is.True);

        Assert.That(t.Lateral.SaveSetting.SaveAs, Is.EqualTo("Setting/Lateral"));
        Assert.That(t.Lateral.SaveSetting.DefaultValue, Is.EqualTo(t.Lateral.AsString()));
        Assert.That(t.Lateral.SaveSetting.AutoSave, Is.True);
    }
}
