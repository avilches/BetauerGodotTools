using Betauer.Application;
using Betauer.Application.Settings;
using Betauer.DI.Attributes;
using Betauer.DI.Exceptions;
using Betauer.Input;
using Betauer.TestRunner;
using Godot;
using NUnit.Framework;
using Betauer.Application.Settings.Attributes;
using Betauer.Input.Attributes;
using Container = Betauer.DI.Container;

namespace Betauer.GameTools.Tests;

[TestRunner.Test]
public partial class InputActionAttributeTests : Node {
    const string SettingsFile = "./action-test-settings.ini";

    [SetUpClass]
    [TestRunner.TearDown]
    public void Clear() {
        System.IO.File.Delete(SettingsFile);
    }

    [Configuration]
    internal class InputWithoutInputActionsContainer {
        [InputAction] private InputAction Jump => InputAction.Create("Jump").AsSimulator();
    }

    [TestRunner.Test(Description = "Error if there is no InputActionsContainer")]
    public void InputWithoutInputActionsContainerTest() {
        Assert.Throws<InvalidAttributeException>(() => new Container().Build(di => di.Scan<InputWithoutInputActionsContainer>()));
    }

    
    
    [InputActionsContainer("MyInputActionsContainer")]
    [Configuration]
    internal class ConfigurableInputWithContainerButWithoutSettingContainer {
        [Singleton] public InputActionsContainer MyInputActionsContainer => new InputActionsContainer();
        
        [InputAction(SaveAs = "Jump")]
        private InputAction JumpConfigurable => InputAction.Create().AsSimulator();
    }

    [TestRunner.Test(Description = "Error if there is not a SettingContainer when a Configurable() action is used")]
    public void ConfigurableInputWithContainerButWithoutSettingContainerTest() {
        Assert.Throws<InvalidAttributeException>(() => new Container().Build(di=>di.Scan<ConfigurableInputWithContainerButWithoutSettingContainer>()));
    }

    
    
    
    [InputActionsContainer("MyInputActionsContainer")]
    [Configuration]
    internal class NoNeedForSettingContainer {
        [Singleton] public InputActionsContainer MyInputActionsContainer => new InputActionsContainer();
        
        [InputAction]
        private InputAction JumpConfigurable => InputAction.Create().AsSimulator();
    }

    [TestRunner.Test(Description = "A non configurable input action does not need a SettingContainer")]
    public void NoNeedForSettingContainerTest() {
        var c = new Container();
        c.Build(di => { di.Scan<NoNeedForSettingContainer>(); });
        var jump = c.Resolve<InputAction>("JumpConfigurable");
        Assert.That(jump.SaveSetting, Is.Null);
    }


    [InputActionsContainer("MyInputActionsContainer")]
    [Configuration]
    internal class InputActionServiceNameConfig {
        [Singleton] public InputActionsContainer MyInputActionsContainer => new InputActionsContainer();
        
        [InputAction]
        private InputAction Action1 => InputAction.Create().AsSimulator();

        [InputAction("ActionOtherName2")]
        private InputAction Action2 => InputAction.Create().AsSimulator();

        [InputAction("ActionOtherName3")]
        private InputAction Action3 => InputAction.Create("ActionName").AsSimulator();
    }

    [TestRunner.Test(Description = "Test service names and action names")]
    public void ServiceNameTests() {
        var c = new Container();
        c.Build(di => { di.Scan<InputActionServiceNameConfig>(); });

        var ac1 = c.Resolve<InputAction>("Action1");
        Assert.That(ac1.Name, Is.EqualTo("Action1"));

        var ac2 = c.Resolve<InputAction>("ActionOtherName2");
        Assert.That(ac2.Name, Is.EqualTo("ActionOtherName2"));
        
        var ac3 = c.Resolve<InputAction>("ActionOtherName3");
        Assert.That(ac3.Name, Is.EqualTo("ActionName"));

        Assert.That(ac1, Is.EqualTo(ac1.InputActionsContainer!.FindAction<InputAction>("Action1")));
        Assert.That(ac2, Is.EqualTo(ac1.InputActionsContainer!.FindAction<InputAction>("ActionOtherName2")));
        Assert.That(ac3, Is.EqualTo(ac1.InputActionsContainer!.FindAction<InputAction>("ActionName")));
    }


    
    
    
    [InputActionsContainer("MyInputActionsContainer")]
    [Configuration]
    internal class AxisActionServiceNameConfig {
        [Singleton] public InputActionsContainer MyInputActionsContainer => new InputActionsContainer();
        
        [AxisAction] private AxisAction Lateral => AxisAction.Create().Build();
        
        [InputAction(AxisName = "Lateral")]
        private InputAction Right => InputAction.Create().PositiveAxis(JoyAxis.LeftX).AsSimulator();

        [InputAction("MyLeft", AxisName = "Lateral")]
        private InputAction Left => InputAction.Create().NegativeAxis(JoyAxis.LeftX).AsSimulator();
    }

    [TestRunner.Test(Description = "Test service names and action names for AxisActions")]
    public void AxisActionServiceNameConfigTests() {
        var c = new Container();
        c.Build(di => { di.Scan<AxisActionServiceNameConfig>(); });
        var right = c.Resolve<InputAction>("Right");
        var left = c.Resolve<InputAction>("MyLeft");
        var axisAction = c.Resolve<AxisAction>("Lateral");
        
        var inputActionsContainer = c.Resolve<InputActionsContainer>("MyInputActionsContainer");
        Assert.That(inputActionsContainer.FindAction<AxisAction>("Lateral"), Is.EqualTo(axisAction));
        Assert.That(inputActionsContainer.FindAction<InputAction>("Right"), Is.EqualTo(right));
        Assert.That(inputActionsContainer.FindAction<InputAction>("MyLeft"), Is.EqualTo(left));

        Assert.That(right.AxisName, Is.EqualTo("Lateral"));
        Assert.That(left.AxisName, Is.EqualTo("Lateral"));
        Assert.That(axisAction.Name, Is.EqualTo("Lateral"));

        Assert.That(right.AxisAction, Is.EqualTo(axisAction));
        Assert.That(left.AxisAction, Is.EqualTo(axisAction));        
        Assert.That(axisAction.Negative, Is.EqualTo(left));
        Assert.That(axisAction.Positive, Is.EqualTo(right));
    }
    
    
    
    
    
    [InputActionsContainer("MyInputActionsContainer")]
    [Configuration]
    internal class AxisActionServiceNameWithNameConfig {
        [Singleton] public InputActionsContainer MyInputActionsContainer => new InputActionsContainer();
        
        [AxisAction] private AxisAction Lateral => AxisAction.Create("lat").Build();
        
        [InputAction(AxisName = "lat")]
        private InputAction Right => InputAction.Create("r").PositiveAxis(JoyAxis.LeftX).AsSimulator();

        [InputAction("MyLeft", AxisName = "lat")]
        private InputAction Left => InputAction.Create("l").NegativeAxis(JoyAxis.LeftX).AsSimulator();
    }

    [TestRunner.Test(Description = "Test service names and action names for AxisActions")]
    public void AxisActionServiceNameWithNameConfigTests() {
        var c = new Container();
        c.Build(di => { di.Scan<AxisActionServiceNameWithNameConfig>(); });
        var right = c.Resolve<InputAction>("Right");
        var left = c.Resolve<InputAction>("MyLeft");
        var axisAction = c.Resolve<AxisAction>("Lateral");

        var inputActionsContainer = c.Resolve<InputActionsContainer>("MyInputActionsContainer");
        Assert.That(inputActionsContainer.FindAction<AxisAction>("lat"), Is.EqualTo(axisAction));
        Assert.That(inputActionsContainer.FindAction<InputAction>("r"), Is.EqualTo(right));
        Assert.That(inputActionsContainer.FindAction<InputAction>("l"), Is.EqualTo(left));

        Assert.That(right.AxisName, Is.EqualTo("lat"));
        Assert.That(left.AxisName, Is.EqualTo("lat"));
        Assert.That(axisAction.Name, Is.EqualTo("lat"));

        Assert.That(right.AxisAction, Is.EqualTo(axisAction));
        Assert.That(left.AxisAction, Is.EqualTo(axisAction));        
        Assert.That(axisAction.AsString(), Is.EqualTo("Reverse:False"));
        Assert.That(axisAction.Negative, Is.EqualTo(left));
        Assert.That(axisAction.Positive, Is.EqualTo(right));
    }
    
    
    
    
    
    
    [InputActionsContainer("MyInputActionsContainer")]
    [SettingsContainer("MySettingContainer")]
    [Configuration]
    internal class ConfigurableInputs {
        [Singleton] public InputActionsContainer MyInputActionsContainer => new InputActionsContainer();
        [Singleton] public SettingsContainer MySettingContainer => new SettingsContainer(new ConfigFileWrapper(SettingsFile));
        
        [InputAction(SaveAs = "Controls/Jump")]
        private InputAction Jump1 => InputAction.Create().AsSimulator();

        [InputAction(SaveAs = "Jump2")]
        private InputAction Jump2 => InputAction.Create().AsSimulator();

        [InputAction(AxisName = "Lateral")]
        private InputAction Right => InputAction.Create("r").PositiveAxis(JoyAxis.LeftX).AsSimulator();

        [InputAction(AxisName = "Lateral")]
        private InputAction Left => InputAction.Create("l").NegativeAxis(JoyAxis.LeftX).AsSimulator();
        
        [AxisAction(SaveAs = "Controls/Lateral")]
        private AxisAction Lateral => AxisAction.Create().ReverseAxis(true).Build();

    }

    [TestRunner.Test(Description = "Input and AxisAction with configurable settings")]
    public void ConfigurableInputsTests() {
        var c = new Container();
        c.Build(di => { di.Scan<ConfigurableInputs>(); });
        var jump1 = c.Resolve<InputAction>("Jump1");
        Assert.That(jump1.SaveSetting.SaveAs, Is.EqualTo("Controls/Jump"));
        Assert.That(jump1.SaveSetting.AutoSave, Is.True);
        Assert.That(jump1.SaveSetting.SettingsContainer, Is.EqualTo(c.Resolve<SettingsContainer>("MySettingContainer")));

        // No section creates "Settings" section
        var jump2 = c.Resolve<InputAction>("Jump2");
        Assert.That(jump2.SaveSetting.SaveAs, Is.EqualTo("Settings/Jump2"));
        Assert.That(jump2.SaveSetting.AutoSave, Is.True);
        Assert.That(jump2.SaveSetting.SettingsContainer, Is.EqualTo(c.Resolve<SettingsContainer>("MySettingContainer")));

        var right = c.Resolve<InputAction>("Right");
        var left = c.Resolve<InputAction>("Left");

        var axisAction = c.Resolve<AxisAction>("Lateral");
        Assert.That(axisAction.SaveSetting.SettingsContainer, Is.EqualTo(c.Resolve<SettingsContainer>("MySettingContainer")));
        Assert.That(axisAction.SaveSetting.SaveAs, Is.EqualTo("Controls/Lateral"));
        Assert.That(axisAction.AsString(), Is.EqualTo("Reverse:True"));
        Assert.That(axisAction.SaveSetting.AutoSave, Is.True);
        Assert.That(axisAction.Negative, Is.EqualTo(left));
        Assert.That(axisAction.Positive, Is.EqualTo(right));
    }
}