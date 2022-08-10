using System.Collections.Generic;
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

        const string SettingsFile = "./action-test-settings.ini";

        [SetUp]
        [TearDown]
        public void Clear() {
            System.IO.File.Delete(SettingsFile);
        }

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

        [Test]
        public void ManualConfigurationWithSettingTest() {
            var attack = InputAction.Create("ManualAttack").Build();
            Assert.That(attack.IsConfigurable(), Is.False);
            Assert.That(attack.ButtonSetting, Is.Null);
            Assert.That(attack.KeySetting, Is.Null);

            var jump = InputAction.Create("ManualJump").Configurable().Build();
            Assert.That(jump.IsConfigurable(), Is.True);
            Assert.That(jump.ButtonSetting, Is.Null);
            Assert.That(jump.KeySetting, Is.Null);

            SaveSetting<string> b = Setting<string>.Save("Controls", "attack.buttons","0,1");
            SaveSetting<string> k = Setting<string>.Save("Controls", "attack.keys", "H,F");
            var sc = new SettingsContainer(SettingsFile);
            sc.Add(b);
            sc.Add(k);
            jump.SetSettings(k, b).Load();
            Assert.That(jump.ButtonSetting, Is.EqualTo(b));
            Assert.That(jump.KeySetting, Is.EqualTo(k));
            
            Assert.That(jump.Buttons, Is.EqualTo(new [] {JoystickList.Button0, JoystickList.Button1}.ToList()));
            Assert.That(jump.Keys, Is.EqualTo(new [] {KeyList.H, KeyList.F}.ToList()));
            
            Assert.That(InputMap.HasAction("ManualJump"), Is.False);
            jump.Setup();
            Assert.That(InputMap.HasAction("ManualJump"), Is.True);

            Assert.That(InputMap.HasAction("ManualAttack"), Is.False);
            attack.Setup();
            Assert.That(InputMap.HasAction("ManualAttack"), Is.True);
            
            var c = new InputActionsContainer();
            c.Add(jump);
            Assert.That(jump.InputActionsContainer, Is.EqualTo(c));
            c.Add(attack);
            Assert.That(attack.InputActionsContainer, Is.EqualTo(c));

            Assert.That(c.ActionList.Count, Is.EqualTo(2));
            Assert.That(c.ConfigurableActionList.Count, Is.EqualTo(1));
            Assert.That(c.FindAction<InputAction>("ManualJump"), Is.EqualTo(jump));
            Assert.That(c.FindAction<InputAction>("ManualAttack"), Is.EqualTo(attack));
        }

        [Test]
        public void ManualConfigurationMultipleInputContainerTest() {
            var acc = new InputActionsContainer();
            var jump = InputAction.Create("Jump").Build();
            acc.Add(jump);
            Assert.That(jump.InputActionsContainer, Is.EqualTo(acc));
            
            var acc2 = new InputActionsContainer();
            var jumpOther = InputAction.Create("VALUE IGNORED", "JumpOther").Configurable().Build();
            acc2.Add(jumpOther);
            Assert.That(jumpOther.InputActionsContainer, Is.EqualTo(acc2));

            acc.RemoveSetup();
            acc2.RemoveSetup();
            acc.Setup();
            Assert.That(InputMap.HasAction("Jump"));
            Assert.That(!InputMap.HasAction("JumpOther"));
            acc.RemoveSetup();
            
            acc.RemoveSetup();
            acc2.RemoveSetup();
            acc2.Setup();
            Assert.That(!InputMap.HasAction("Jump"));
            Assert.That(InputMap.HasAction("JumpOther"));
        }

        [Configuration]
        internal class InputWithoutInputActionsContainer {
            [Service]
            private InputAction Jump => InputAction.Create("Jump").Build();
        }

        [Test(Description = "Error if there is no InputActionsContainer")]
        public void InputWithoutInputActionsContainerTest() {
            var di = new ContainerBuilder(this);
            di.Scan<InputWithoutInputActionsContainer>();
            Assert.Throws<KeyNotFoundException>(() => di.Build());
        }
        
        [Configuration]
        internal class ConfigurableInputWithContainerButWithoutSettingContainer {
            [Service] public InputActionsContainer InputActionsContainer => new InputActionsContainer();
            [Service] private InputAction JumpConfigurable => InputAction.Create("Jump").Configurable().Build();
        }

        [Test(Description = "Error if there is not a SettingContainer when a Configurable() action is used")]
        public void ConfigurableInputWithContainerButWithoutSettingContainerTest() {
            var di = new ContainerBuilder(this);
            di.Scan<ConfigurableInputWithContainerButWithoutSettingContainer>();
            var e = Assert.Throws<KeyNotFoundException>(() => di.Build());
            Assert.That(e.Message.Contains(nameof(SettingsContainer)));
        }

        internal class MyInputActionsContainer : InputActionsContainer {
        }

        [Configuration]
        internal class InputWithContainer {
            [Service] public InputActionsContainer InputActionsContainer => new MyInputActionsContainer();
            [Service] private InputAction Jump => InputAction.Create("Jump").Build();
        }

        [Test(Description = "Use a custom InputActionsContainer")]
        public void InputActionContainerTests() {
            var di = new ContainerBuilder(this);
            di.Scan<InputWithContainer>();
            var c = di.Build();
            var s = c.Resolve<InputActionsContainer>();
            var jump = c.Resolve<InputAction>("Jump");
            Assert.That(s, Is.TypeOf<MyInputActionsContainer>());
            Assert.That(s, Is.EqualTo(jump.InputActionsContainer));

            Assert.That(s.ActionList.Count, Is.EqualTo(1));
            Assert.That(s.ConfigurableActionList.Count, Is.EqualTo(0));
            Assert.That(s.FindAction<InputAction>("Jump"), Is.EqualTo(jump));
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
            
            [Service] private InputAction NoConfigurable => InputAction.Create("Jump3").Build();
        }

        [Service]
        internal class Service4 {
            [Inject] public InputActionsContainer InputActionsContainer;
            [Inject] public SettingsContainer SettingsContainer;
            [Inject(Name="Other")] public SettingsContainer SettingsContainerOther;
            
            [Inject] public InputAction JumpConfigurable;
            [Inject] public InputAction JumpConfigurableWithSetting;
            [Inject] public InputAction NoConfigurable;
        }

        [Test(Description = "Configurable with different setting container")]
        public void ConfigurableInputWithContainerAndSettingsTests() {
            var di = new ContainerBuilder(this);
            di.Scan<ConfigurableInputWithContainerAndSettings>();
            di.Scan<Service4>();
            var c = di.Build();
            var s = c.Resolve<Service4>();

            Assert.That(s.JumpConfigurable.ButtonSetting.Section, Is.EqualTo("Controls"));
            Assert.That(s.JumpConfigurable.ButtonSetting.Name, Is.EqualTo("Jump.Buttons"));
            Assert.That(s.JumpConfigurable.KeySetting.Section, Is.EqualTo("Controls"));
            Assert.That(s.JumpConfigurable.KeySetting.Name, Is.EqualTo("Jump.Keys"));
            Assert.That(s.JumpConfigurable.KeySetting.SettingsContainer,
                Is.EqualTo(s.JumpConfigurable.ButtonSetting.SettingsContainer));

            Assert.That(s.NoConfigurable.ButtonSetting, Is.Null);
            Assert.That(s.NoConfigurable.KeySetting, Is.Null);

            Assert.That(s.InputActionsContainer.FindAction<InputAction>("Jump"), Is.EqualTo(s.JumpConfigurable));
            Assert.That(s.InputActionsContainer.FindAction<InputAction>("Jump2"), Is.EqualTo(s.JumpConfigurableWithSetting));
            Assert.That(s.InputActionsContainer.FindAction<InputAction>("Jump3"), Is.EqualTo(s.NoConfigurable));
            Assert.That(s.InputActionsContainer.ActionList.Count, Is.EqualTo(3));
            Assert.That(s.InputActionsContainer.ConfigurableActionList.Count, Is.EqualTo(2));
            
            
            Assert.That(s.JumpConfigurable.KeySetting.SettingsContainer, Is.EqualTo(s.SettingsContainer));
            Assert.That(s.JumpConfigurable.ButtonSetting.SettingsContainer, Is.EqualTo(s.SettingsContainer));
            Assert.That(s.JumpConfigurableWithSetting.ButtonSetting.Section, Is.EqualTo("Controls2"));
            Assert.That(s.JumpConfigurableWithSetting.KeySetting.Section, Is.EqualTo("Controls2"));
            Assert.That(s.JumpConfigurableWithSetting.KeySetting.SettingsContainer, Is.EqualTo(s.SettingsContainerOther));
            Assert.That(s.JumpConfigurableWithSetting.ButtonSetting.SettingsContainer, Is.EqualTo(s.SettingsContainerOther));
        }
        
        [Configuration]
        internal class MultipleInputActionContainer {
            [Service] public SettingsContainer SettingsContainer => new SettingsContainer("settings1.ini");
            
            [Service] 
            public InputActionsContainer InputActionsContainer => new InputActionsContainer();
            [Service(Name="Other")] 
            public InputActionsContainer InputActionsContainer2 => new InputActionsContainer();

            [Service] private InputAction Jump => InputAction.Create("Jump")
                .Build();

            [Service] private InputAction JumpOther => InputAction.Create("Other", "JumpOther")
                .Build();
        }

        [Service]
        internal class Service5 {
            [Inject] public InputActionsContainer InputActionsContainer;
            [Inject(Name="Other")] public InputActionsContainer InputActionsContainer2;
            [Inject] public SettingsContainer SettingsContainer;
            
            [Inject] public InputAction Jump;
            [Inject] public InputAction JumpOther;
        }

        [Test(Description = "Configurable with different input action container")]
        public void MultipleInputActionContainerTest() {
            var di = new ContainerBuilder(this);
            di.Scan<MultipleInputActionContainer>();
            di.Scan<Service5>();
            var c = di.Build();
            var s = c.Resolve<Service5>();
            
            Assert.That(s.Jump.InputActionsContainer, Is.EqualTo(s.InputActionsContainer));
            Assert.That(s.JumpOther.InputActionsContainer, Is.EqualTo(s.InputActionsContainer2));

            s.InputActionsContainer.RemoveSetup();
            s.InputActionsContainer2.RemoveSetup();
            s.InputActionsContainer.Setup();
            Assert.That(InputMap.HasAction("Jump"));
            Assert.That(!InputMap.HasAction("JumpOther"));
            s.InputActionsContainer.RemoveSetup();
            
            s.InputActionsContainer.RemoveSetup();
            s.InputActionsContainer2.RemoveSetup();
            s.InputActionsContainer2.Setup();
            Assert.That(!InputMap.HasAction("Jump"));
            Assert.That(InputMap.HasAction("JumpOther"));

        }
    }
}