# Betauer Godot Tools
The Betauer Godot Tools is a modular framework to develop Godot games. It's divided in different and independent 
projects that can be imported in your games easily.

It include examples 
with different (and working!) ways to include the Betauer Godot Tools using relative paths to the source code (slower to build, but it can be debugged) or reference to dlls (faster to build, can't be debugged). It also resolved a very common problem: how to exclude unit tests from the game build. Finally, it include a set of Makefiles and scripts to build the games from command line. 
                            
## Projects
- [Betauer.Core](Betauer.Core): common Godot extensions and other small tools needed by all of the Betauer projects. 
The most important one is the Logger: a fast and configurable logger for Godot.
- [Betauer.TestRunner](Betauer.TestRunner): a tool to run [NUnit](https://nunit.org/) tests inside Godot, so you can test your nodes and your code easily.
- [Betauer.DI](Betauer.DI): yet, another Dependency Injection Container for C#.
This is a very simple implementation integrated with Godot, so you can do this kind of stuff;
```C#
// If a class inhertis from Node, it will be added to the SceneTree.Root like an autoload
[Singleton] public class ScoreManager : Node {}
[Singleton(Name = "Easy"] public class EasySceneManager : ISceneManager {}
[Singleton(Name = "Hard"] public class HardSceneManager : ISceneManager {}
[Configuration] public class ExposeStuff {
    [Singleton] public SaveGameManager SaveGameManager => new SaveGameManager();
    [Transient] public EnemyController Enemy => new EnemyController();
}
```
      
And use the singletons later in your scene objects like this:
```C#
// You can attach this class to your nodes or scenes, [OnReady] and [Inject] will be resolved at runtime
public class Player: KinematicBody2D {
    [OnReady("Body/AttackArea") Area2D _attackArea;
    [Inject] ScoreManager _scoreManager;
    [Inject(Name="Easy"] ISceneManager _scoreManager;
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
- [Betauer.StateMachine](Betauer.StateMachine): yet, a StateMachine implementation with a stack, compatible with `async`/`await`. Ready to be used in your menus (like this [example](DemoGame/Game/Managers/GameManager.cs)) or your player (like this other [example](DemoGame/Game/Character/Player/PlayerStateMachine.cs)).
The state machine code is well tested, you can check the tests in the [Betauer.StateMachine.Tests](Betauer.StateMachine.Tests) project.
- [Betauer.Animation](Betauer.Animation): a port to C# of the well known [Anima library](https://github.com/ceceppa/anima). It include up to 77 different animations from https://animate.style. This is a better way to use Tween to create complex and reusable animation your scene elements. It can be used to create effects or to move and schedule rotations or translation in your platforms or characters.
```C#
// This one of the 77 animations called `RollOutLeft`
return TemplateBuilder.Create()
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
- [Betauer.GameTools](Betauer.GameTools): this a set of small tools to create Godot games:
  - Bus: it's hard to handle collisions with signals. With the Bus, enemies, player and other objects can use a shared bus to subscribe and receive events with only the collision that affect to them. No more check and filter if the body shape of the collision is you or another object.
  - InputManager: handle with the Godot `InputMap` or the events is hard. This set of tools help you to create one class per action that can be injected in your
  nodes using dependency injection.
  - SettingsManager: a helper to load and save settings
  - ScreeService: a helper to change resolution with pixel perfect scale.
  - More stuff like a flipper, a loader or a timer.
- [Betauer.GodotAction](Betauer.GodotAction): Godot doesn't allow (yet) to 
subscribe easily to signals using C# lambdas. And it also forces to extend
Godot classes and override `_Process`,`_PhysicsProcess`,`_Input`,`_UnhandledInput`,`_UnhandledKeyInput` methods, which can be cumbersome to create a new class just to implement one method.
This project extends all the 524 Godot C# classes and allows to use all the signals and method with lambdas:
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
- [SourceGenerator](SourceGenerator): a standalone Godot project with a single script to write all the classes and signal extensions from the [Betauer.GodotAction](Betauer.GodotAction) project.


## Build

There is a Makefile script to clean, build (from scratch), run tests and generate classes
```bash
make clean 
make generate 
make build
make test
```
Or you can just do everything with just `make clean generate test build`.

Warning: it expects to have a Osx Godot located in the folder `/Applications/Godot_mono.app`. Maybe you need to update the Makefile to change that.

There is also a GitHub action to run the tests in every commit. Check it out here: [.github/workflows/godot-ci.yaml](.github/workflows/godot-ci.yaml) 

## Demo game

To be documented
