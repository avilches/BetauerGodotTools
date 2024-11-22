using System;
using System.Linq;
using Betauer.Application;
using Betauer.Application.Settings;
using Betauer.Core;
using Betauer.DI;
using Betauer.DI.Attributes;
using Betauer.DI.ServiceProvider;
using Betauer.Input;
using Betauer.Input.Joypad;
using Betauer.TestRunner;
using Godot;
using NUnit.Framework;

namespace Betauer.GameTools.Tests.Input; 

[TestFixture]
public class MultiPlayerContainerTests {

    const string SettingsFile = "./action-test-settings.ini";

    [OneTimeSetUp]
    [TearDown]
    public void Clear() {
        System.IO.File.Delete(SettingsFile);
        InputMap.GetActions().ForEach(InputMap.EraseAction);
    }

    public class Test3MultiPlayerContainer : MultiPlayerContainer<Test3PlayerActions> {
    }

    public class Test3PlayerActions : PlayerActionsContainer {
        public InputAction Jump { get; } = InputAction.Create("Jump").Buttons(JoyButton.B).Keys(Key.A).Build();
        public InputAction Left { get; } = InputAction.Create("Left").AxisName("Lateral").NegativeAxis(JoyAxis.LeftX).Build();
        public AxisAction Lateral { get; } = new AxisAction("Lateral");
        public InputAction Right { get; } = InputAction.Create("Right").AxisName("Lateral").PositiveAxis(JoyAxis.LeftX).Build();
    }


    [Test(Description = "Test GetPlayerActionsByJoypadId, GetPlayerActionsById")]
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

    [Test(Description = "Test auto incremental player id creation")]
    public void PlayerActionsContainerErrorDuplicateJoypadTest() {
        var c = new Test3MultiPlayerContainer();
        Assert.That(c.GetNextPlayerId(), Is.EqualTo(0));
        Assert.That(c.AddPlayerActions(0).PlayerId, Is.EqualTo(0));
        Assert.That(c.GetNextPlayerId(), Is.EqualTo(1));
        Assert.That(c.AddPlayerActions(3).PlayerId, Is.EqualTo(1));
        Assert.That(c.GetNextPlayerId(), Is.EqualTo(2));
        Assert.That(c.AddPlayerActions(5).PlayerId, Is.EqualTo(2));

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

    [Test(Description = "Add players and change player id")]
    public void PlayerActionsContainerTest() {
        var c = new Test3MultiPlayerContainer();

        var joypadId = 2;
        Test3PlayerActions p1 = c.AddPlayerActions(joypadId);
        Assert.That(p1.PlayerId, Is.EqualTo(0));
        Assert.That(p1.JoypadId, Is.EqualTo(joypadId));
        var playerId = p1.PlayerId;
        
        Assert.That(InputMap.HasAction("Jump/P"+playerId), Is.True);
        Assert.That(InputMap.HasAction("Right/P"+playerId), Is.True);
        Assert.That(InputMap.HasAction("Left/P"+playerId), Is.True);

        Assert.That((InputMap.ActionGetEvents("Jump/P"+playerId).OfType<InputEventJoypadButton>().ToList()[0])!.Device, Is.EqualTo(joypadId));
        Assert.That((InputMap.ActionGetEvents("Left/P"+playerId).OfType<InputEventJoypadMotion>().ToList()[0])!.Device, Is.EqualTo(joypadId));

        var newPlayerId = 7;
        c.ChangePlayerId(playerId, 7);
        Assert.That(p1.PlayerId, Is.EqualTo(newPlayerId));
        Assert.That(p1.JoypadId, Is.EqualTo(2));
        Assert.That((InputMap.ActionGetEvents("Jump/P"+newPlayerId).OfType<InputEventJoypadButton>().ToList()[0])!.Device, Is.EqualTo(joypadId));
        Assert.That((InputMap.ActionGetEvents("Left/P"+newPlayerId).OfType<InputEventJoypadMotion>().ToList()[0])!.Device, Is.EqualTo(joypadId));

        Assert.That(InputMap.HasAction("Jump/P"+newPlayerId), Is.True);
        Assert.That(InputMap.HasAction("Right/P"+newPlayerId), Is.True);
        Assert.That(InputMap.HasAction("Left/P"+newPlayerId), Is.True);
        Assert.That(InputMap.HasAction("Jump/P"+playerId), Is.False);
        Assert.That(InputMap.HasAction("Right/P"+playerId), Is.False);
        Assert.That(InputMap.HasAction("Left/P"+playerId), Is.False);
        
        var p2 = c.AddPlayerActions(6);
        Assert.Throws<Exception>(() => c.ChangePlayerId(newPlayerId, p2.PlayerId));

        c.RemovePlayerActions(p1);
        Assert.That(InputMap.HasAction("Jump/P"+newPlayerId), Is.False);
        Assert.That(InputMap.HasAction("Right/P"+newPlayerId), Is.False);
        Assert.That(InputMap.HasAction("Left/P"+newPlayerId), Is.False);
    }

    [Test(Description = "ChangeKeyboard")]
    public void ChangeKeyboard() {
        var mpc = new Test3MultiPlayerContainer();

        var t1 = mpc.CreatePlayerActions(0, 1, true);
        InputIsEquals(mpc.SharedInputActions.Jump, t1.Jump);
        InputIsEquals(mpc.SharedInputActions.Left, t1.Left);
        InputIsEquals(mpc.SharedInputActions.Right, t1.Right);
        InputIsEquals(mpc.SharedInputActions.Lateral, t1.Lateral);
        
        mpc.ChangeKeyboard(0, false);
        InputIsJoypadEqualsButNoKeyboard(mpc.SharedInputActions.Jump, t1.Jump);
        InputIsJoypadEqualsButNoKeyboard(mpc.SharedInputActions.Left, t1.Left);
        InputIsEquals(mpc.SharedInputActions.Right, t1.Right);
        InputIsEquals(mpc.SharedInputActions.Lateral, t1.Lateral);
        
        mpc.ChangeKeyboard(0, true);
        InputIsEquals(mpc.SharedInputActions.Jump, t1.Jump);
        InputIsEquals(mpc.SharedInputActions.Left, t1.Left);
        InputIsEquals(mpc.SharedInputActions.Right, t1.Right);
        InputIsEquals(mpc.SharedInputActions.Lateral, t1.Lateral);
    }

    [Test(Description = "ChangeJoypad")]
    public void ChangeJoypad() {
        var mpc = new Test3MultiPlayerContainer();

        var joypadId = 1;
        var t1 = mpc.CreatePlayerActions(0, joypadId, true);
        Assert.That((InputMap.ActionGetEvents("Jump/P0").OfType<InputEventJoypadButton>().ToList()[0])!.Device, Is.EqualTo(joypadId));
        Assert.That((InputMap.ActionGetEvents("Left/P0").OfType<InputEventJoypadMotion>().ToList()[0])!.Device, Is.EqualTo(joypadId));

        var newJoypadId = 2;
        mpc.ChangeJoypad(0, newJoypadId);
        Assert.That(t1.JoypadId, Is.EqualTo(newJoypadId));
        Assert.That((InputMap.ActionGetEvents("Jump/P0").OfType<InputEventJoypadButton>().ToList()[0])!.Device, Is.EqualTo(newJoypadId));
        Assert.That((InputMap.ActionGetEvents("Left/P0").OfType<InputEventJoypadMotion>().ToList()[0])!.Device, Is.EqualTo(newJoypadId));
    }

    [Test(Description = "SyncActions")]
    public void SyncActionsTest() {
        var mpc = new Test3MultiPlayerContainer();

        var t1 = mpc.CreatePlayerActions(0, 1, true);
        InputIsEquals(mpc.SharedInputActions.Jump, t1.Jump);
        InputIsEquals(mpc.SharedInputActions.Left, t1.Left);
        InputIsEquals(mpc.SharedInputActions.Right, t1.Right);
        InputIsEquals(mpc.SharedInputActions.Lateral, t1.Lateral);

        var t2 = mpc.CreatePlayerActions(1, 2, false);
        InputIsJoypadEqualsButNoKeyboard(mpc.SharedInputActions.Jump, t2.Jump);
        InputIsJoypadEqualsButNoKeyboard(mpc.SharedInputActions.Left, t2.Left);
        InputIsEquals(mpc.SharedInputActions.Right, t2.Right);
        InputIsEquals(mpc.SharedInputActions.Lateral, t2.Lateral);
        
        // Change the shared input without syncing...
        mpc.SharedInputActions.Jump.Update(u => 
            u.ClearButtons().SetButton(JoyButton.Start).ClearKeys().SetKey(Key.B));

        Assert.That(mpc.SharedInputActions.Jump.Buttons, Is.Not.EqualTo(t1.Jump.Buttons));
        Assert.That(mpc.SharedInputActions.Jump.Buttons, Is.Not.EqualTo(t2.Jump.Buttons));
        
        // sync
        mpc.SyncActions();
        InputIsEquals(mpc.SharedInputActions.Jump, t1.Jump);
        InputIsEquals(mpc.SharedInputActions.Left, t1.Left);
        InputIsEquals(mpc.SharedInputActions.Right, t1.Right);
        InputIsEquals(mpc.SharedInputActions.Lateral, t1.Lateral);

        InputIsJoypadEqualsButNoKeyboard(mpc.SharedInputActions.Jump, t2.Jump);
        InputIsJoypadEqualsButNoKeyboard(mpc.SharedInputActions.Left, t2.Left);
        InputIsEquals(mpc.SharedInputActions.Right, t2.Right);
        InputIsEquals(mpc.SharedInputActions.Lateral, t2.Lateral);
        
        // New players have the new actions too
        var t3 = mpc.AddPlayerActions(2, true);
        InputIsEquals(mpc.SharedInputActions.Jump, t3.Jump);
        InputIsEquals(mpc.SharedInputActions.Left, t3.Left);
        InputIsEquals(mpc.SharedInputActions.Right, t3.Right);
        InputIsEquals(mpc.SharedInputActions.Lateral, t3.Lateral);
        
        var t4 = mpc.AddPlayerActions( 4, false);
        InputIsJoypadEqualsButNoKeyboard(mpc.SharedInputActions.Jump, t4.Jump);
        InputIsJoypadEqualsButNoKeyboard(mpc.SharedInputActions.Left, t4.Left);
        InputIsEquals(mpc.SharedInputActions.Right, t4.Right);
        InputIsEquals(mpc.SharedInputActions.Lateral, t4.Lateral);
    }


    private static void InputIsEquals(AxisAction a1, AxisAction a2) {
        // Assert.That(a1.Name, Is.EqualTo(a2.Name));
        Assert.That(a1.Reverse, Is.EqualTo(a2.Reverse));
    }

    private static void InputIsEquals(InputAction a1, InputAction a2) {
        // Assert.That(a1.Name, Is.EqualTo(a2.Name));
        Assert.That(a1.Buttons, Is.EqualTo(a2.Buttons));
        Assert.That(a1.AxisName, Is.EqualTo(a2.AxisName));
        Assert.That(a1.Axis, Is.EqualTo(a2.Axis));
        Assert.That(a1.AxisSign, Is.EqualTo(a2.AxisSign));
        Assert.That(a1.DeadZone, Is.EqualTo(a2.DeadZone));
        
        Assert.That(a1.Keys, Is.EqualTo(a2.Keys));
        Assert.That(a1.MouseButton, Is.EqualTo(a2.MouseButton));
        Assert.That(a1.CommandOrCtrlAutoremap, Is.EqualTo(a2.CommandOrCtrlAutoremap));
        Assert.That(a1.Ctrl, Is.EqualTo(a2.Ctrl));
        Assert.That(a1.Shift, Is.EqualTo(a2.Shift));
        Assert.That(a1.Alt, Is.EqualTo(a2.Alt));
        Assert.That(a1.Meta, Is.EqualTo(a2.Meta));
    }

    private static void InputIsJoypadEqualsButNoKeyboard(InputAction a1, InputAction a2) {
        // Assert.That(a1.Name, Is.EqualTo(a2.Name));
        Assert.That(a1.Buttons, Is.EqualTo(a2.Buttons));
        Assert.That(a1.AxisName, Is.EqualTo(a2.AxisName));
        Assert.That(a1.Axis, Is.EqualTo(a2.Axis));
        Assert.That(a1.AxisSign, Is.EqualTo(a2.AxisSign));
        Assert.That(a1.DeadZone, Is.EqualTo(a2.DeadZone));
        
        Assert.That(a2.Keys.Count, Is.EqualTo(0));
        Assert.That(a2.MouseButton, Is.EqualTo(MouseButton.None));
        Assert.That(a2.CommandOrCtrlAutoremap, Is.False);
        Assert.That(a2.Ctrl, Is.False);
        Assert.That(a2.Shift, Is.False);
        Assert.That(a2.Alt, Is.False);
        Assert.That(a2.Meta, Is.False);
    }

    [Singleton]
    public class Test4MultiPlayerContainer : MultiPlayerContainer<Test4PlayerActions>, IInjectable {
        [Inject] SettingsContainer SettingsContainer { get; set; }
        public void PostInject() {
            ConfigureSaveSettings(SettingsContainer);
        }
    }

    public class Test4PlayerActions : PlayerActionsContainer {
        public InputAction Jump { get; } = InputAction.Create("Jump").Buttons(JoyButton.B).Build();
    }

    [Test(Description = "SettingContainer injected, only the Jump and Lateral actions will have a SaveSetting")]
    public void SharingSettingContainerIntegrationTests() {
        var sc = new SettingsContainer(new ConfigFileWrapper(SettingsFile));
        var c = new Betauer.DI.Container().Build(b => {
            b.Scan<Test4MultiPlayerContainer>();
            b.Register(Provider.Static(sc));
        });
        var mpc = c.Resolve<Test4MultiPlayerContainer>();
        
        Assert.That(mpc.SharedInputActionsContainer.SettingsContainer, Is.EqualTo(sc));
    }
}
