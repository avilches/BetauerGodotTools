using System.Linq;
using Betauer.Application.Settings;
using Betauer.DI;
using Betauer.Input;
using Betauer.TestRunner;
using Godot;
using NUnit.Framework;

namespace Betauer.GameTools.Tests {
    [TestFixture]
    public class InputActionTests : Node {

        [Test]
        public void BuilderTests() {
            var empty = InputAction.Create("N").Build();
            Assert.That(empty.IsConfigurable(), Is.False);
            Assert.That(empty.Name, Is.EqualTo("N"));
            Assert.That(empty.Keys, Is.Empty);
            Assert.That(empty.Buttons, Is.Empty);

            var reg = InputAction.Create("N")
                .Keys(KeyList.A)
                .Keys(KeyList.K, KeyList.Alt)
                .Buttons(JoystickList.Button0)
                .Buttons(JoystickList.Button1, JoystickList.Button2)
                .Build();
            Assert.That(reg.IsConfigurable(), Is.False);
            Assert.That(reg.Name, Is.EqualTo("N"));
            Assert.That(reg.Keys, Is.EqualTo(new [] {KeyList.A, KeyList.K, KeyList.Alt}.ToList()));
            Assert.That(reg.Buttons, Is.EqualTo(new [] {JoystickList.Button0, JoystickList.Button1, JoystickList.Button2}.ToList()));

            Assert.That(InputAction.Create("N").Configurable().Build().IsConfigurable(), Is.True);
            Assert.That(InputAction.Create("N").SettingsContainer("X").Build().IsConfigurable(), Is.True);
            Assert.That(InputAction.Create("N").SettingsSection("D").Build().IsConfigurable(), Is.True);
        } 

        [Configuration]
        internal class InputWithDefaultContainer {
            [Service(Name = "X1")] private InputAction Jump => InputAction.Create("Jump").Build();
            [Service(Name = "X2")] private InputAction Attack => InputAction.Create("Attack").Build();
        }

        [Service]
        internal class Service1 {
            [Inject(Name = "X1")] public InputAction Jump;
            [Inject] public InputAction X2;
        }

        [Test(Description = "No InputActionsContainer, it creates a Default instance. Check for action names")]
        public void DefaultInputActionContainerTests() {
            var di = new ContainerBuilder(this);
            di.Scan<InputWithDefaultContainer>();
            di.Scan<Service1>();
            var c = di.Build();
            var s = c.Resolve<Service1>();
            Assert.That(c.Contains<InputActionsContainer>(), Is.True);
            Assert.That(s.Jump.InputActionsContainer, Is.Not.Null);
            Assert.That(s.Jump.InputActionsContainer, Is.EqualTo(s.X2.InputActionsContainer));

            Assert.That(s.Jump.InputActionsContainer.ActionList.Count, Is.EqualTo(2));
            Assert.That(s.Jump.InputActionsContainer.ConfigurableActionList.Count, Is.EqualTo(0));
            Assert.That(s.Jump.InputActionsContainer.FindAction<InputAction>("Jump"), Is.EqualTo(s.Jump));
            Assert.That(s.Jump.InputActionsContainer.FindAction<InputAction>("Attack"), Is.EqualTo(s.X2));
        }
        
        internal class MyInputActionsContainer : InputActionsContainer {
        }

        [Configuration]
        internal class InputWithContainer {
            [Service] public InputActionsContainer InputActionsContainer => new MyInputActionsContainer();
            [Service] private InputAction Jump => InputAction.Create("Jump").Build();
        }

        [Service]
        internal class Service2 {
            [Inject] public InputActionsContainer InputActionsContainer;
            [Inject] public InputAction Jump;
        }

        [Test(Description = "Use a custom InputActionsContainer")]
        public void InputActionContainerTests() {
            var di = new ContainerBuilder(this);
            di.Scan<InputWithContainer>();
            di.Scan<Service2>();
            var c = di.Build();
            var s = c.Resolve<Service2>();
            Assert.That(s.InputActionsContainer, Is.TypeOf<MyInputActionsContainer>());
            Assert.That(s.InputActionsContainer, Is.EqualTo(s.Jump.InputActionsContainer));

            Assert.That(s.InputActionsContainer.ActionList.Count, Is.EqualTo(1));
            Assert.That(s.InputActionsContainer.ConfigurableActionList.Count, Is.EqualTo(0));
            Assert.That(s.InputActionsContainer.FindAction<InputAction>("Jump"), Is.EqualTo(s.Jump));
        }


        [Configuration]
        internal class ConfigurableInputWithContainer {
            [Service] public InputActionsContainer InputActionsContainer => new InputActionsContainer();

            [Service] private InputAction JumpConfigurable => InputAction.Create("Jump")
                .Configurable()
                .Build();
            [Service] private InputAction NoConfigurable => InputAction.Create("Attack").Build();
        }

        [Service]
        internal class Service3 {
            [Inject] public InputActionsContainer InputActionsContainer;
            
            [Inject] public InputAction JumpConfigurable;
            [Inject] public InputAction NoConfigurable;
        }

        [Test(Description = "Check for auto Configurable() settings")]
        public void ConfigurableInputWithContainerTests() {
            var di = new ContainerBuilder(this);
            di.Scan<ConfigurableInputWithContainer>();
            di.Scan<Service3>();
            var c = di.Build();
            var s = c.Resolve<Service3>();
            Assert.That(s.JumpConfigurable.ButtonSetting.Section, Is.EqualTo("Controls"));
            Assert.That(s.JumpConfigurable.ButtonSetting.Name, Is.EqualTo("Jump.Buttons"));
            Assert.That(s.JumpConfigurable.KeySetting.Section, Is.EqualTo("Controls"));
            Assert.That(s.JumpConfigurable.KeySetting.Name, Is.EqualTo("Jump.Keys"));
            Assert.That(s.JumpConfigurable.KeySetting.SettingsContainer, Is.EqualTo(s.JumpConfigurable.ButtonSetting.SettingsContainer));

            Assert.That(s.NoConfigurable.ButtonSetting, Is.Null);
            Assert.That(s.NoConfigurable.KeySetting, Is.Null);

            Assert.That(s.InputActionsContainer.FindAction<InputAction>("Jump"), Is.EqualTo(s.JumpConfigurable));
            Assert.That(s.InputActionsContainer.FindAction<InputAction>("Attack"), Is.EqualTo(s.NoConfigurable));
            Assert.That(s.InputActionsContainer.ActionList.Count, Is.EqualTo(2));
            Assert.That(s.InputActionsContainer.ConfigurableActionList.Count, Is.EqualTo(1));
            Assert.That(s.InputActionsContainer.ConfigurableActionList[0], Is.EqualTo(s.JumpConfigurable));
        }
        
        [Configuration]
        internal class ConfigurableInputWithContainerAndSettings {
            [Service] public InputActionsContainer InputActionsContainer => new InputActionsContainer();

            [Service] public SettingsContainer SettingsContainer => new SettingsContainer("settings1.ini");
            [Service(Name="Other")] public SettingsContainer SettingsContainerOther => new SettingsContainer("settings2.ini");
            
            [Service] private InputAction JumpConfigurable => InputAction.Create("Jump")
                .Configurable()
                .Build();

            [Service] private InputAction JumpConfigurableWithSetting => InputAction.Create("Jump2")
                .SettingsContainer("Other")
                .SettingsSection("Controls2")
                .Build();
        }

        [Service]
        internal class Service4 {
            [Inject] public SettingsContainer SettingsContainer;
            [Inject(Name="Other")] public SettingsContainer SettingsContainerOther;
            
            [Inject] public InputAction JumpConfigurable;
            [Inject] public InputAction JumpConfigurableWithSetting;
        }

        [Test(Description = "Configurable with different setting container")]
        public void ConfigurableInputWithContainerAndSettingsTests() {
            var di = new ContainerBuilder(this);
            di.Scan<ConfigurableInputWithContainerAndSettings>();
            di.Scan<Service4>();
            var c = di.Build();
            var s = c.Resolve<Service4>();
            Assert.That(s.JumpConfigurable.KeySetting.SettingsContainer, Is.EqualTo(s.SettingsContainer));
            Assert.That(s.JumpConfigurable.ButtonSetting.SettingsContainer, Is.EqualTo(s.SettingsContainer));

            Assert.That(s.JumpConfigurableWithSetting.ButtonSetting.Section, Is.EqualTo("Controls2"));
            Assert.That(s.JumpConfigurableWithSetting.KeySetting.Section, Is.EqualTo("Controls2"));
            
            Assert.That(s.JumpConfigurableWithSetting.KeySetting.SettingsContainer, Is.EqualTo(s.SettingsContainerOther));
            Assert.That(s.JumpConfigurableWithSetting.ButtonSetting.SettingsContainer, Is.EqualTo(s.SettingsContainerOther));
        }
    }
}