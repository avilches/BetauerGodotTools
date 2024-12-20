# Betauer Godot Tools

> **Warning**
> This README may be outdated. If you find this library useful and plan to use it, please contact me, and I'll update the content and create proper documentation/tutorials.

> **Wow!**
> It (only) works with Godot 4! :-)

Betauer Godot Tools is a C# framework designed to facilitate the development of games using the Godot engine. It consists of various independent projects that can be easily imported into your games.

## Projects

### 1. Betauer.Tools.Logging

A logger to print messages to the Godot console, including the current process and physics frame information.
Configurable on a per-class basis with various trace levels, this tool offers optimal performance by utilizing the [ZString](https://github.com/Cysharp/ZString) library to prevent memory allocation during string concatenation and date string creation.

This library is used by all other Betauer Godot Tools projects.

```C#
using Betauer.Tools.Logging;
// Configure the logger, TraceLevels are: Off (no logs), Fatal, Error, Warning, Info, Debug, All (equivalent to Debug)
LoggerFactory.SetTraceLevel<MyClass>(TraceLevel.Error); // Recommended for production 
LoggerFactory.SetTraceLevel<MyClass>(TraceLevel.Debug); // Development 

// Use the logger
private static readonly Logger _logger = LoggerFactory.GetLogger<MyClass>();
_logger.Debug("This is a debug message"); // it will not appear if the trace level bigger than debug, like Error 
_logger.Error("This is a {0} message", "error"); // The string interpolation won't have memory allocation at all! :)
```
        
By default, Error and Fatal use `GD.PushError`, Warning uses `GD.PushWarning` and the rest of the levels use `GD.Print`. 
You can change this behavior using the `LoggerFactory.AddTextWriter(ITextWriter writer)` method with your own `ITextWriter`. 
There are some implementations already provided by the library, like `GDPrintWriter` (default), `ColoredConsoleWriter`/`ConsoleWriter` (both use Console.WriteLine(), with or without colors) and `TextWriterWrapper`.

### 2. Betauer.TestRunner

Betauer.TestRunner is a tool designed for executing tests within the Godot environment, streamlining the process of testing nodes and code inside Godot.
That means you can access to SceneTree and any GodotSharp classes in your tests. Additionally, it provides its own set of C# attributes to identify and mark your classes as test cases.

```C#
using Betauer.TestRunner;

[TestFixture]
public class YourTest {
    // Executed only once before and after all tests in this class
    [OneTimeSetUp] public void OneTimeSetUp() {}
    [TearDownClass] public void TearDownClass() {}

    // Executed before and after each test in this class
    [SetUp] public void Setup() {}
    [TearDown] public void TearDown() {}

    [Test]
    public void YourTest() {
        // you have access to any Godot class!
        Engine.TimeScale = 0.5f;
        // do your test here, using NUnit or XUnit or any other assertion library
    }
```

If your test class inherits from Node, it will be instantiated and added to the SceneTree **in every test**. Tests can use `this.AwaitProcessFrame()` or `this.AwaitPhysicsFrame()` to wait for the next frame to be processed.
```C#
using Betauer.TestRunner;

[TestFixture]
public class YouTest : Node {
    [OneTimeSetUp]
    public void Setup() {
        // This method is called once before all tests in this class
    }

    [Test]
    public void YourTest() {
        GetTree().AddChild(new Sprite2D());
        this.AwaitProcessFrame();
        // one frame later, the sprite added is added properly to tree 
    }
```

In order to run these tests in your game, you have to create a script like this and specify the assemblies to scan for test cases:

```C#
public partial class RunTests : SceneTree {
    public override async void _Initialize() {
        await ConsoleTestRunner.RunTests(this, typeof(YourClass).Assembly);
    }
}
```
      
And execute them with Godot, using this command line to run the script RunTests located in the root of your project:
```shell
godot4 -s RunTests.cs --headless
```

Note: it requires importing an additional library for asserting test results like NUnit or or XUnit though.

### 3. Betauer.Core

A set of common classes used by all other Betauer Godot Tools projects:

- Two different types of object pools. Both super simple to use and super fast. No thread-safe.
- A node property restorer. A helper to restore the state of a node's properties after a scene change.
- Signal tools: the AwaitExtensions class. A set of extension methods to allow await for any signal.
- Reusable timer tools helper. They internally use SceneTreeTimer with SceneTree.CreateTimer(), so they are affected by the `Engine.Timescale` and the `SceneTree.Pause` state (these things C# System.Diagnostics.Stopwatch can not do)
    - GodotStopwatch: to measure elapsed time since start. It has Start/Stop and Reset.
    - GodotTimeout: to execute an action after the timeout in seconds. It has Start/Stop and Reset.
    - GodotScheduler: to execute an action periodically (every x seconds). It has Start/Stop and Reset. 

And a lot of more extensions for Godot classes. They are used by the other projects, but they can be used in your own projects too.

### 4. Betauer.DI

Betauer.DI is a simple dependency injection container for C#, integrated with Godot. It allows you to manage the lifecycle of classes, inject services into your scene objects.

### 5. Betauer.Tools.FastReflection

Reflection is slow in C#. This library creates and compile lambdas to ensure a fast access to properties, methods and fields. This library is used by other Betauer Godot Tools projects, like the Dependency Injection Container (Betauer.DI)

### 6. Betauer.Bus

An extension class and some tools to simplify collision handling, layers, masks and signals. Objects like enemies and players can subscribe and receive events relevant to their collisions in a easier way than use pure Godot.

### 7. Betauer.FSM

Betauer.FSM is another Finite State Machine (FSM) implementation for C#. It is compatible with async/await, stack-based, and Godot, making it suitable for use in game menus or player character logic. The state machine code is well-tested, as demonstrated in the Betauer.FSM.Tests project.

### 8. Betauer.GameTools

Betauer.GameTools is a collection of small tools for creating Godot games, including:

- **InputManager**: Simplifies working with the Godot InputMap and events, allowing you to create one class per action that can be injected into nodes using dependency injection.
- **SettingsManager**: A helper for loading and saving settings where each setting is defined as a service and can be injected anywhere in your code.
- **ScreenService**: A helper for changing resolutions with pixel-perfect scaling.
- **Additional tools**: Features like a flipper, a loader, and a timer are also included.

### 9. Betauer.Animation

Betauer.Animation is a C# port of the well-known [Anima](https://github.com/ceceppa/anima) library. It includes up to 77 different animations from [Animate.style](https://animate.style). This provides a better way to use Tween for creating complex and reusable animations for scene elements. It can be used for effects or moving and scheduling rotations or translations in platforms or characters.

### 10. SourceGenerator

SourceGenerator is a standalone Godot project with a single script that writes the Betauer.Core/Signal/AwaitExtensions.cs and Betauer.GameTools/Application/Notifications/NotificationsHandler.cs.

### 11. DemoGame

A demo game that uses all the Betauer Godot Tools projects. It is a simple platformer with a few enemies and a few levels. It is a good example of how to use the Betauer Godot Tools projects.
It offers examples of different ways to include Betauer Godot Tools using relative paths to the source code (slower to build but debuggable) or reference to DLLs (faster to build but not debuggable). It also resolves a common problem: excluding unit tests from the game build. Lastly, it includes a set of Makefiles and scripts for building games from the command line.

## Build

There is a Makefile script to clean, build (from scratch), run tests and generate classes
```bash
make clean                  
make generate 
make build/Debug
make test
```