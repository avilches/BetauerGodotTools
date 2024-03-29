# Betauer Godot Tools

> **Warning**
> This readme is very out of date. If you find this library useful and you are considering use it, just text me and I'll try to update this and create some kind of documentation/tutorial.

The Betauer Godot Tools is a framework to develop Godot games using C#. It's divided in different and independent projects that can be imported in your games easily.


![Integration tests](https://github.com/avilches/BetauerGodotTools/actions/workflows/godot-ci.yaml/badge.svg)

## Projects
- [Betauer.Core](Betauer.Core): common Godot extensions and other small tools needed by all of the Betauer projects. The most important one is the Logger: a fast and configurable logger for Godot. It also have some reflection tools
  needed by dependency injection.
- [Betauer.TestRunner](Betauer.TestRunner): a tool to run [NUnit](https://nunit.org/) tests inside Godot, so you can test your nodes and your code easily inside Godot.
- [Betauer.FSM](Betauer.FSM): yet another FSM implementation for C#. With a stack, compatible with `async`/`await` and compatible with Godot. Ready to be used in a game menu (like this [example](DemoGame/Game/Managers/MainFSM.cs)) or your player (like this other [example](DemoGame/Game/Character/Player/PlayerFSM.cs)).
  The state machine code is well tested, you can check the tests in the [Betauer.FSM.Tests](Betauer.FSM.Tests) project.
- [Betauer.GameTools](Betauer.GameTools): this a set of small tools to create Godot games:
    - Bus: it's hard to handle collisions with signals. With the Bus, enemies, player and other objects can use a shared bus to subscribe and receive events with only the collision that affect to them. No more check and filter if the body shape of the collision is you or another object.
    - InputManager: handle with the Godot `InputMap` or the events is hard. This set of tools help you to create one class per action that can be injected in your
      nodes using dependency injection.
       ````C#
      [Configuration]
      public class Actions {
          [Service] public InputActionsContainer InputActionsContainer => new();
          
          [Service]
          private InputAction UiUp => InputAction.Create("ui_up")
              .KeepProjectSettings()
              .NegativeAxis(1, "ui_down")
              .DeadZone(0.5f)
              .Build();
  
              [Service]
          private InputAction Attack => InputAction.Configurable("Attack")
              .Keys(KeyList.C)
              .Mouse(ButtonList.Left)
              .Buttons(JoystickList.XboxB)
              .Build();
      }
      ```` 
    - SettingsManager: a helper to load and save settings where every setting is defined as a service, and it can be injected anywhere in your code.
      ````C#
      [Configuration]
      public class Settings {
          [Service] public SettingsContainer SettingsContainer => new(AppTools.GetUserFile("settings.ini"));
  
          [Service("Settings.Screen.PixelPerfect")]
          public ISetting<bool> PixelPerfect => Setting<bool>.Save("Video", "PixelPerfect", false);
  
          [Service("Settings.Difficult")]
          public ISetting<int> Difficult =>  Setting<bool>.Save("Game", "Difficult", 1);
  
          [Service("Settings.Screen.WindowedResolution")]
          public ISetting<Resolution> WindowedResolution =>
              Setting<Resolution>.Save("Video", "WindowedResolution", ApplicationConfig.Configuration.BaseResolution);
      }    
      ````
    - ScreenService: a helper to change resolution with pixel perfect scale.
    - More stuff like a flipper, a loader or a timer.
    - Handle signals and _process or _input methods with lambdas. Godot doesn't allow (yet) to subscribe easily to signals using C# lambdas. And it also forces to extend Godot classes and override `_Process`, `_PhysicsProcess`, `_Input`, `_UnhandledInput` and `_UnhandledKeyInput` methods, which can be cumbersome to create a new class just to implement one method.

      ```C#
      var button = new ButtonAction();
      // Lambdas instead of signals
      button.OnReady(() => button.Visible = true);
      button.OnFocusEntered(() => GD.Print("focus_entered signal"));
      button.OnFocusExited(() => GD.Print("focus_exited signal"));
      // Lambdas instead of method overriding
      button.OnProcess((float delta) => GD.Print("_Process(delta)"));  
      button.OnPhysicsProcess((float delta) => GD.Print("_PhysicsProcess(delta)"));  
      button.OnInput((InputEvent inputEvent) => GD.Print("_Input(inputEvent)"));  
      ```
- [Betauer.DI](Betauer.DI): yet, another Dependency Injection Container for C#.
  This is a very simple implementation integrated with Godot, so you can do this kind of stuff;
```C#
// The lifecycle of these classes will be controlled by the container 
// If a class inhertis from Node, it will be added to the SceneTree.Root like an autoload
[Service] public class ScoreManager : Node {}
[Service(Name = "Easy"] public class EasySceneManager : ISceneManager {}
[Service(Name = "Hard"] public class HardSceneManager : ISceneManager {
    [Inject] ScoreManager ScoreManager { get; set; }
}

[Configuration] public class ExposeStuff {
    [Service] public SaveGameManager SaveGameManager => new SaveGameManager();
    // Injecting a lifetime service will create a new instance every time is injected
    [Service(Lifetime.Transient)] public EnemyController Enemy => new EnemyController();
}
```

And use the singletons later in your scene objects like this:
```C#
// You can attach this class to your nodes or scenes, [OnReady] and [Inject] will be resolved at runtime
public class Player: KinematicBody2D {
    [OnReady("Body/AttackArea") Area2D _attackArea;
    [Inject] ScoreManager _scoreManager { get; set; }
    [Inject(Name="Easy"] ISceneManager _sceneManager { get; set; }
}
```
As an extra feature, you will have a `GetTree()` method available in any class just injecting `Func<SceneTree>` as an attribute:
```C#
[Singleton]
public class YourClass {
    [Inject] private Func<SceneTree> GetTree;
    public void YourMethod() {
        // This works
        GetTree().Root.GuiDisableInput = true;
    }
}
```
With just adding the container as Autoload. Dependency injection is well tested, you can check the tests in the [Betauer.DI.Tests](Betauer.DI.Tests) project.
- [Betauer.Animation](Betauer.Animation): a port to C# of the well known [Anima library](https://github.com/ceceppa/anima). It include up to 77 different animations from https://animate.style. This is a lot better way to use Tween to create complex and reusable animations for your scene elements. It can be used to create effects or to move and schedule rotations or translation in your platforms or characters.
```C#
// This one of the 77 animations called `RollOutLeft`
return Sequence.Create()
    .SetDuration(0.5f)
    .AnimateRelativeKeys(Property.PositionBySizeX)
       .KeyframeOffset(0.00f, 0.0f, null, node => node.SetRotateOriginToCenter())
       .KeyframeOffset(1.00f, -1.0f)
    .EndAnimate()
    .Parallel()
    .AnimateKeys(Property.Rotate2D)
       .KeyframeTo(0.00f, 0)
       .KeyframeTo(1.00f, 120)
    .EndAnimate()
    .Parallel()
    .AnimateKeys(Property.Opacity)
       .KeyframeTo(0.00f, 1.0f)
       .KeyframeTo(1.00f, 0.0f)
    .EndAnimate()
    .BuildTemplate();
```
You can check out or start to use any of the [animations](Betauer.Animation/Template.cs), or create your own ones.

- [SourceGenerator](SourceGenerator): a standalone Godot project with a single script to write all the classes and signal extensions from the [Betauer.GodotAction](Betauer.GodotAction) project.

It also includes examples with different (and working) ways to include the Betauer Godot Tools using relative paths to the source code (slower to build, but it can be debugged) or reference to dlls (faster to build, can't be debugged). It also resolved a very common problem: how to exclude unit tests from the game build. Finally, it include a set of Makefiles and scripts to build the games from command line.

## Build

There is a Makefile script to clean, build (from scratch), run tests and generate classes
```bash
make clean 
make generate 
make test
make build/debug
make build/release
```
Or you can do everything with just `make export/dll`. All dlls will be copied into the `./export/releases/<version>/Debug` and `.../ExporRelease` folders.

Warning: it expects to have a Osx Godot located in the folder `/Applications/Godot_mono.app`. Maybe you need to update the Makefile to change that.

There is also a GitHub action to run the tests in every commit. Check it out here: [.github/workflows/godot-ci.yaml](.github/workflows/godot-ci.yaml)

## Demo game

To be documented