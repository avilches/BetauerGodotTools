using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;
using Betauer.Application.Lifecycle.Attributes;
using Betauer.Application.Persistent;
using Betauer.Application.Persistent.Json;
using Betauer.Application.Settings;
using Betauer.DI.Attributes;
using Betauer.Input;
using Godot;

namespace Veronenger.Game.Dungeon;

[Configuration]
[Loader("GameLoader", Tag = MainResources.GameLoaderTag)]
public class DungeonMainResources {
    [Transient<DungeonGameView>(Name = "DungeonGameView")]
    public DungeonGameView GameView => new DungeonGameView();
}

[Configuration]
[Loader("GameLoader", Tag = GameLoaderTag)]
[Scene.Transient<DungeonMap>(Name="DungeonMapFactory", Path="res://Game/Dungeon/DungeonMap.tscn")]
public class DungeonGameResources {
    public const string GameLoaderTag = "dungeon";
}


public interface IDungeonSaveObject : ISaveObject {
}

[Configuration]
public class DungeonGameConfig {
    [Singleton] public JsonGameLoader<DungeonSaveGameMetadata> DungeonGameObjectLoader() {
        var loader = new JsonGameLoader<DungeonSaveGameMetadata>();
        loader.WithJsonSerializerOptions(options => {
            options.AllowTrailingCommas = true;
            options.WriteIndented = true;
            options.Converters.Add(new JsonStringEnumConverter(JsonNamingPolicy.CamelCase));
        });
        loader.Scan<IDungeonSaveObject>();
        return loader;
    }
}

[Singleton]
public class DungeonConfig {
}

[Singleton]
public class DungeonPlayerActions : SingleJoypadActionsContainer {
    public DungeonPlayerActions() {
        AddActionsFromProperties(this);
        SetFirstConnectedJoypad();
    }

    [Inject] public SettingsContainer SettingsContainer { get; set; }

    public AxisAction Lateral { get; } = AxisAction.Create("Lateral").SaveAs("Controls/Lateral").Build();

    public AxisAction Vertical { get; } = AxisAction.Create("Vertical").SaveAs("Controls/Vertical").Build();

    public InputAction Up { get; } = InputAction.Create("Up")
        .AxisName("Vertical")
        .SaveAs("Controls/Up")
        .Keys(Key.Up)
        .Buttons(JoyButton.DpadUp)
        .NegativeAxis(JoyAxis.LeftY)
        .DeadZone(0.5f)
        .Build();

    public InputAction Down { get; } = InputAction.Create("Down")
        .AxisName("Vertical")
        .SaveAs("Controls/Down")
        .Keys(Key.Down)
        .Buttons(JoyButton.DpadDown)
        .PositiveAxis(JoyAxis.LeftY)
        .DeadZone(0.5f)
        .Build();

    public InputAction Left { get; } = InputAction.Create("Left")
        .AxisName("Lateral")
        .SaveAs("Controls/Left")
        .Keys(Key.Left)
        .Buttons(JoyButton.DpadLeft)
        .NegativeAxis(JoyAxis.LeftX)
        .DeadZone(0.5f)
        .Build();

    public InputAction Right { get; } = InputAction.Create("Right")
        .AxisName("Lateral")
        .SaveAs("Controls/Right")
        .Keys(Key.Right)
        .Buttons(JoyButton.DpadRight)
        .PositiveAxis(JoyAxis.LeftX)
        .DeadZone(0.5f)
        .Build();

    public InputAction Jump { get; } = InputAction.Create("Jump")
        .SaveAs("Controls/Jump")
        .Keys(Key.Space)
        .Buttons(JoyButton.A)
        .EnableJustTimers()
        .Build();

    public InputAction Attack { get; } = InputAction.Create("Attack")
        .SaveAs("Controls/Attack")
        .Keys(Key.C)
        .Mouse(MouseButton.Left)
        .Buttons(JoyButton.B)
        .Build();
}

