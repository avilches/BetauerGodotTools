using System;
using Godot;

namespace Betauer.Core.Time; 

public static partial class TimerExtensions {
    
    public static GodotStopwatch CreateStopwatch(this SceneTree sceneTree, 
        bool processAlways = false, bool processInPhysics = false, bool ignoreTimeScale = false) =>
        new GodotStopwatch(sceneTree, processAlways, processInPhysics, ignoreTimeScale);
    
    public static GodotTimeout CreateTimeout(this SceneTree sceneTree, 
        double timeout, Action onTimeout, bool processAlways = false, bool processInPhysics = false, bool ignoreTimeScale = false) =>
        new GodotTimeout(sceneTree, timeout, onTimeout, processAlways, processInPhysics, ignoreTimeScale);

    public static GodotScheduler CreateScheduler(this SceneTree sceneTree,
        double initialDelay, double everySeconds, Action action, bool processAlways = false, bool processInPhysics = false, bool ignoreTimeScale = false) =>
        new GodotScheduler(sceneTree, initialDelay, everySeconds, action, processAlways, processInPhysics, ignoreTimeScale);

}