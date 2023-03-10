using System;
using System.Collections.Generic;
using System.Linq;
using Betauer.Application.Settings;
using Betauer.DI;
using Betauer.DI.Exceptions;
using Betauer.DI.ServiceProvider;
using Betauer.Input;
using Betauer.TestRunner;
using Godot;
using NUnit.Framework;
using Container = Betauer.DI.Container;

namespace Betauer.GameTools.Tests {
    [TestFixture]
    public partial class InputActionTests : Node {

        const string SettingsFile = "./action-test-settings.ini";

        [SetUp]
        [TearDown]
        public void Clear() {
            System.IO.File.Delete(SettingsFile);
        }

        [Test]
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

        [Test]
        public void ImportExport() {
            SaveSetting<string> b = Setting<string>.Persistent(null, "attack", "");
            var sc = new SettingsContainer(SettingsFile);
            sc.Add(b);
            
            var jump = InputAction.Configurable("ImportExportAction").AsSimulator();
            jump.SetSaveSettings(b).Load();
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

        [Test]
        public void UpdateRollback() {
            SaveSetting<string> b = Setting<string>.Persistent(null, "attack", "");
            var sc = new SettingsContainer(SettingsFile);
            sc.Add(b);
            
            var jump = InputAction.Configurable("UpdateRollback").AsSimulator();
            jump.SetSaveSettings(b).Load();
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

        [Test]
        public void ManualConfigurationWithSettingTest() {
            var attack = InputAction.Create("ManualAttack").AsSimulator();
            Assert.That(attack.SaveSetting, Is.Null);

            var jump = InputAction.Configurable("ManualJump").AsSimulator();
            Assert.That(jump.SaveSetting, Is.Null);

            SaveSetting<string> b = Setting<string>.Persistent(null, "attack","button:A,button:B,key:H,key:F");
            Assert.That(b.Section, Is.EqualTo("Settings")); // default value when section is null
            var sc = new SettingsContainer(SettingsFile);
            sc.Add(b);
            jump.SetSaveSettings(b).Load();
            Assert.That(jump.SaveSetting, Is.EqualTo(b));
            
            Assert.That(jump.Buttons, Is.EqualTo(new [] {JoyButton.A, JoyButton.B}.ToList()));
            Assert.That(jump.Keys, Is.EqualTo(new [] {Key.H, Key.F}.ToList()));
            
            var c = new InputActionsContainer();
            c.Add(jump);
            // Assert.That(jump.InputActionsContainer, Is.EqualTo(c));
            c.Add(attack);
            // Assert.That(attack.InputActionsContainer, Is.EqualTo(c));

            Assert.That(c.InputActionList.Count, Is.EqualTo(2));
            Assert.That(c.FindAction("ManualJump"), Is.EqualTo(jump));
            Assert.That(c.FindAction("ManualAttack"), Is.EqualTo(attack));
        }

        [Configuration]
        internal class InputWithoutInputActionsContainer {
            [Service]
            private InputAction Jump => InputAction.Create("Jump").AsSimulator();
        }

        [Test(Description = "Error if there is no InputActionsContainer")]
        public void InputWithoutInputActionsContainerTest() {
            var di = new Container.Builder();
            di.Scan<InputWithoutInputActionsContainer>();
            Assert.Throws<ServiceNotFoundException>(() => di.Build());
        }
        
        [Configuration]
        internal class ConfigurableInputWithContainerButWithoutSettingContainer {
            [Service] public InputActionsContainer InputActionsContainer => new InputActionsContainer();
            [Service] private InputAction JumpConfigurable => InputAction.Configurable("Jump").AsSimulator();
        }

        [Test(Description = "Error if there is not a SettingContainer when a Configurable() action is used")]
        public void ConfigurableInputWithContainerButWithoutSettingContainerTest() {
            var di = new Container.Builder();
            di.Register(Provider.Singleton<SceneTree>(GetTree));
            di.Scan<ConfigurableInputWithContainerButWithoutSettingContainer>();
            var e = Assert.Throws<ServiceNotFoundException>(() => di.Build());
            Assert.That(e.Message, Contains.Substring(nameof(SettingsContainer)));
        }

        internal partial class MyInputActionsContainer : InputActionsContainer {
        }

        [Configuration]
        internal class InputWithContainer {
            [Service] public InputActionsContainer InputActionsContainer => new MyInputActionsContainer();
            [Service] private InputAction Jump => InputAction.Create("Jump").AsSimulator();
        }

        [Test(Description = "Use a custom InputActionsContainer")]
        public void InputActionContainerTests() {
            var di = new Container.Builder();
            di.Register(Provider.Singleton<SceneTree>(GetTree));
            di.Scan<InputWithContainer>();
            var c = di.Build();
            var s = c.Resolve<InputActionsContainer>();
            var jump = c.Resolve<InputAction>("Jump");
            Assert.That(s, Is.TypeOf<MyInputActionsContainer>());
            // Assert.That(s, Is.EqualTo(jump.InputActionsContainer));

            Assert.That(s.InputActionList.Count, Is.EqualTo(1));
            Assert.That(s.FindAction("Jump"), Is.EqualTo(jump));
        }

        [Configuration]
        internal class ConfigurableInputWithContainerAndSettings {
            [Service] public InputActionsContainer InputActionsContainer => new();

            [Service] 
            public SettingsContainer SettingsContainer => new("settings1.ini");
            [Service(Name="Other")] 
            public SettingsContainer Other => new("settings2.ini");
            
            [Service] private InputAction JumpConfigurable => InputAction.Configurable("Jump").AsSimulator();

            [Service] private InputAction JumpConfigurableWithSetting => InputAction.Configurable("Jump2")
                .SettingsContainer(nameof(Other))
                .SettingsSection("Controls2")
                .AsSimulator();
            
            [Service] private InputAction NoConfigurable => InputAction.Create("Jump3").AsSimulator();
        }

        [Service]
        internal class Service4 {
            [Inject] public InputActionsContainer InputActionsContainer { get; set; }
            [Inject] public SettingsContainer SettingsContainer { get; set; }
            [Inject(Name="Other")] public SettingsContainer SettingsContainerOther { get; set; }
            
            [Inject] public InputAction JumpConfigurable { get; set; }
            [Inject] public InputAction JumpConfigurableWithSetting { get; set; }
            [Inject] public InputAction NoConfigurable { get; set; }
        }

        [Test(Description = "Configurable with different setting container")]
        public void ConfigurableInputWithContainerAndSettingsTests() {
            var di = new Container.Builder();
            di.Scan<ConfigurableInputWithContainerAndSettings>();
            di.Scan<Service4>();
            di.Register(Provider.Singleton<SceneTree>(GetTree));
            var c = di.Build();
            var s = c.Resolve<Service4>();

            Assert.That(s.JumpConfigurable.SaveSetting.Section, Is.EqualTo("Controls"));
            Assert.That(s.JumpConfigurable.SaveSetting.Name, Is.EqualTo("Jump"));

            Assert.That(s.NoConfigurable.SaveSetting, Is.Null);

            Assert.That(s.InputActionsContainer.FindAction("Jump"), Is.EqualTo(s.JumpConfigurable));
            Assert.That(s.InputActionsContainer.FindAction("Jump2"), Is.EqualTo(s.JumpConfigurableWithSetting));
            Assert.That(s.InputActionsContainer.FindAction("Jump3"), Is.EqualTo(s.NoConfigurable));
            Assert.That(s.InputActionsContainer.InputActionList.Count, Is.EqualTo(3));
            
            Assert.That(s.JumpConfigurable.SaveSetting.SettingsContainer, Is.EqualTo(s.SettingsContainer));
            Assert.That(s.JumpConfigurableWithSetting.SaveSetting.Section, Is.EqualTo("Controls2"));
            Assert.That(s.JumpConfigurableWithSetting.SaveSetting.SettingsContainer, Is.EqualTo(s.SettingsContainerOther));
        }
        
        [Configuration]
        internal class MultipleInputActionContainer {
            [Service] public SettingsContainer SettingsContainer => new SettingsContainer("settings1.ini");
            
            [Service] 
            public InputActionsContainer InputActionsContainer => new InputActionsContainer();
            [Service(Name="Other")] 
            public InputActionsContainer InputActionsContainer2 => new InputActionsContainer();

            [Service] private InputAction Jump => InputAction.Create("Jump")
                .AsSimulator();

            [Service] private InputAction JumpOther => InputAction.Create("Other", "JumpOther")
                .AsSimulator();
        }

        [Service]
        internal class Service5 {
            [Inject] public InputActionsContainer InputActionsContainer { get; set; }
            [Inject(Name="Other")] public InputActionsContainer InputActionsContainer2 { get; set; }
            [Inject] public SettingsContainer SettingsContainer { get; set; }
            
            [Inject] public InputAction Jump { get; set; }
            [Inject] public InputAction JumpOther { get; set; }
        }

        [Test(Description = "Configurable with different input action container")]
        public void MultipleInputActionContainerTest() {
            var di = new Container.Builder();
            di.Scan<MultipleInputActionContainer>();
            di.Scan<Service5>();
            di.Register(Provider.Singleton<SceneTree>(GetTree));
            var c = di.Build();
            var s = c.Resolve<Service5>();
            
            // Assert.That(s.Jump.InputActionsContainer, Is.EqualTo(s.InputActionsContainer));
            // Assert.That(s.JumpOther.InputActionsContainer, Is.EqualTo(s.InputActionsContainer2));

            // s.InputActionsContainer.RemoveSetup();
            // s.InputActionsContainer2.RemoveSetup();
            // s.InputActionsContainer.Setup();
            // Assert.That(InputMap.HasAction("Jump"));
            // Assert.That(!InputMap.HasAction("JumpOther"));
            // s.InputActionsContainer.RemoveSetup();
            //
            // s.InputActionsContainer.RemoveSetup();
            // s.InputActionsContainer2.RemoveSetup();
            // s.InputActionsContainer2.Setup();
            // Assert.That(!InputMap.HasAction("Jump"));
            // Assert.That(InputMap.HasAction("JumpOther"));

        }
    }
}