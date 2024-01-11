using System;
using System.Collections.Generic;
using Betauer.Application;
using Betauer.Application.Settings;
using Betauer.Core;
using Betauer.DI.Attributes;
using Betauer.DI.ServiceProvider;
using Betauer.Input;
using Betauer.Input.Joypad;
using Betauer.TestRunner;
using Godot;
using NUnit.Framework;

namespace Betauer.GameTools.Tests.Input; 

[TestRunner.Test]
public class MultiPlayerContainerTests {

    const string SettingsFile = "./action-test-settings.ini";

    [SetUpClass]
    [TestRunner.TearDown]
    public void Clear() {
        System.IO.File.Delete(SettingsFile);
        InputMap.GetActions().ForEach(InputMap.EraseAction);
    }

    public class Test3MultiPlayerContainer : MultiPlayerContainer<Test3PlayerActions> {
    }

    public class Test3PlayerActions : PlayerActionsContainer {
        public InputAction Jump { get; } = InputAction.Create("Jump").Buttons(JoyButton.B).Build();
        public InputAction Left { get; } = InputAction.Create("Left").AxisName("Lateral").NegativeAxis(JoyAxis.LeftX).Build();
        public AxisAction Lateral { get; } = new AxisAction("Lateral");
        public InputAction Right { get; } = InputAction.Create("Right").AxisName("Lateral").PositiveAxis(JoyAxis.LeftX).Build();
    }


    [TestRunner.Test(Description = "Test GetPlayerActionsByJoypadId, GetPlayerActionsById")]
    public void PlayerActionsContainerFinderTests() {
        var c = new Test3MultiPlayerContainer();

        Assert.That(c.GetJoypadIdsInUse(), Is.EqualTo(new int[] {}));
        Assert.That(c.GetPlayerActionsByJoypadId(0), Is.Null);
        Assert.That(c.GetPlayerActionsById(0), Is.Null);
        Assert.That(c.IsJoypadInUse(0), Is.False);

        var p1 = c.AddPlayerActions(0);
        Assert.That(c.GetJoypadIdsInUse(), Is.EqualTo(new int[] { 0 }));
        Assert.That(c.GetPlayerActionsByJoypadId(0), Is.EqualTo(p1));
        Assert.That(c.GetPlayerActionsById(0), Is.EqualTo(p1));
        Assert.That(c.IsJoypadInUse(0), Is.True);

        var p2 = c.AddPlayerActions(2);
        Assert.That(c.GetJoypadIdsInUse(), Is.EqualTo(new int[] { 0, 2 }));
        Assert.That(c.GetPlayerActionsByJoypadId(2), Is.EqualTo(p2));
        Assert.That(c.GetPlayerActionsById(1), Is.EqualTo(p2));
        Assert.That(c.IsJoypadInUse(2), Is.True);
        
        var p4 = c.CreatePlayerActions(3, 4);
        Assert.That(c.GetJoypadIdsInUse(), Is.EqualTo(new int[] { 0, 2, 4 }));
        Assert.That(c.GetPlayerActionsByJoypadId(4), Is.EqualTo(p4));
        Assert.That(c.GetPlayerActionsById(3), Is.EqualTo(p4));
        Assert.That(c.IsJoypadInUse(4), Is.True);

        Assert.That(c.GetPlayerActionsByJoypadId(10), Is.Null);
        Assert.That(c.IsJoypadInUse(1), Is.False);

    }

    [TestRunner.Test(Description = "Test auto incremental player id creation")]
    public void PlayerActionsContainerErrorDuplicateJoypadTest() {
        var c = new Test3MultiPlayerContainer();
        Assert.That(c.GetNextPlayerId(), Is.EqualTo(0));
        Assert.That(c.AddPlayerActions(0).PlayerId, Is.EqualTo(0));
        Assert.That(c.GetNextPlayerId(), Is.EqualTo(1));
        Assert.That(c.AddPlayerActions(3).PlayerId, Is.EqualTo(1));
        Assert.That(c.GetNextPlayerId(), Is.EqualTo(2));
        Assert.That(c.AddPlayerActions(5).PlayerId, Is.EqualTo(2));

        var e1 = Assert.Throws<Exception>(() => c.AddPlayerActions(3));
        Assert.That(e1.Message, Is.EqualTo("Player 1 already have the joypad 3"));
        
        var e2 = Assert.Throws<Exception>(() => c.CreatePlayerActions(10, 5));
        Assert.That(e2.Message, Is.EqualTo("Player 2 already have the joypad 5"));
        
        // If we skip the playerId 3 creating the player 4 instead...
        Assert.That(c.GetNextPlayerId(), Is.EqualTo(3));
        Assert.That(c.CreatePlayerActions(4, 6).PlayerId, Is.EqualTo(4));
        // ... the next playerId will be 3...
        Assert.That(c.GetNextPlayerId(), Is.EqualTo(3));
        Assert.That(c.AddPlayerActions(7).PlayerId, Is.EqualTo(3));
        // ... and the next playerId will be 5
        Assert.That(c.GetNextPlayerId(), Is.EqualTo(5));
        Assert.That(c.AddPlayerActions(8).PlayerId, Is.EqualTo(5));
        
        // If the player 4 is removed...
        c.RemovePlayerActions(4);
        // ... the next playerId will be 4
        Assert.That(c.GetNextPlayerId(), Is.EqualTo(4));
        Assert.That(c.AddPlayerActions(6).PlayerId, Is.EqualTo(4));
    }

    [TestRunner.Test(Description = "Error when duplicated joypad id")]
    public void PlayerActionsContainerDuplicatedTest() {
        var c = new Test3MultiPlayerContainer();
        c.AddPlayerActions(0);
        c.AddPlayerActions(1);
        c.AddPlayerActions(2);
        var e = Assert.Throws<Exception>(() => c.AddPlayerActions(1));
        Assert.That(e.Message, Is.EqualTo("Player 1 already have the joypad 1"));
    }

    [TestRunner.Test(Description = "")]
    public void PlayerActionsContainerTest() {
        var c = new Test3MultiPlayerContainer();

        Test3PlayerActions p1 = c.AddPlayerActions(2);
        Assert.That(p1.PlayerId, Is.EqualTo(0));
        Assert.That(p1.JoypadId, Is.EqualTo(2));
        var playerId = p1.PlayerId;
        
        Assert.That(InputMap.HasAction("Jump/"+playerId), Is.True);
        Assert.That(InputMap.HasAction("Right/"+playerId), Is.True);
        Assert.That(InputMap.HasAction("Left/"+playerId), Is.True);

        Assert.That((InputMap.ActionGetEvents("Jump/" + playerId)[0] as InputEventJoypadButton)!.Device, Is.EqualTo(2));
        
        p1.Stop();
        Assert.That(InputMap.HasAction("Jump/"+playerId), Is.False);
        Assert.That(InputMap.HasAction("Right/"+playerId), Is.False);
        Assert.That(InputMap.HasAction("Left/"+playerId), Is.False);
    }

    [Singleton]
    public class Test4MultiPlayerContainer : MultiPlayerContainer<Test4PlayerActions> {
    }


    public class Test4PlayerActions : PlayerActionsContainer, SaveSettingsContainerAware {
        public InputAction Jump { get; } = InputAction.Create("Jump").Buttons(JoyButton.B).Keys(Key.A).SaveAs("Setting/Jump").Build();
        public InputAction Attack { get; } = InputAction.Create("Attack").Buttons(JoyButton.B).JoypadId(2).Build();
        public InputAction Left { get; } = InputAction.Create("Left").AxisName("Lateral").NegativeAxis(JoyAxis.LeftX).Build();
        public AxisAction Lateral { get; } = AxisAction.Create("Lateral").SaveAs("Setting/Lateral").Build();
        public InputAction Right { get; } = InputAction.Create("Right").AxisName("Lateral").PositiveAxis(JoyAxis.LeftX).Build();
        
        [Inject] public SettingsContainer SettingsContainer { get; set;  }
    }

    [TestRunner.Test(Description = "SettingContainer injected, only the Jump and Lateral actions will have a SaveSetting")]
    public void SettingContainerIntegrationTests() {
        var sc = new SettingsContainer(new ConfigFileWrapper(SettingsFile));
        var c = new Betauer.DI.Container().Build(b => {
            b.Scan<Test4MultiPlayerContainer>();
            b.Register(Provider.Static(sc));
        });
        var mpc = c.Resolve<Test4MultiPlayerContainer>();

        var t = mpc.CreatePlayerActions(3, 3);

        Assert.That(t.InputActions.Count, Is.EqualTo(4));
        Assert.That(t.AxisActions.Count, Is.EqualTo(1));
        
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


    [TestRunner.Test(Description = "SettingContainer injected, only the Jump and Lateral actions will have a SaveSetting")]
    public void SharingSettingContainerIntegrationTests() {
        var sc = new SettingsContainer(new ConfigFileWrapper(SettingsFile));
        var c = new Betauer.DI.Container().Build(b => {
            b.Scan<Test4MultiPlayerContainer>();
            b.Register(Provider.Static(sc));
        });
        var mpc = c.Resolve<Test4MultiPlayerContainer>();

        var t1 = mpc.CreatePlayerActions(0, 1);
        var t2 = mpc.CreatePlayerActions(1, 2);
        
        // t1 and t2 have have different SaveSetting but using the same SaveAs and the same SettingsContainer
        // Both have JoyButton.B
        Assert.That(t1.Jump.Buttons, Is.EqualTo(new [] { JoyButton.B }));
        Assert.That(t1.Jump.Buttons, Is.EqualTo(t2.Jump.Buttons));
        t1.Jump.Update(u => u.ClearButtons().SetButton(JoyButton.Start));
        Assert.That(t1.Jump.Buttons, Is.EqualTo(new [] { JoyButton.Start }));
        t1.Jump.Save();
        
        // EL save refresca el resto de SaveSettings que tengan el mimo nombre, pero no refrescan sus InputAction
        
        Assert.That(sc.ConfigFileWrapper.GetValue("Setting/Jump", "nop"), Is.EqualTo(t1.Jump.AsString()));
        
        // t1 is now JoyButton.Start, but t2 is not aware of this change
        Assert.That(t2.Jump.Buttons, Is.EqualTo(new [] { JoyButton.B }));
        t2.Jump.Load();
        Assert.That(t2.Jump.Buttons, Is.EqualTo(new [] { JoyButton.Start }));
        
        var t3 = mpc.CreatePlayerActions(3, 3);

    }


}
