using Betauer.Core.Nodes;
using Betauer.Core.Nodes.Events;
using Godot;

namespace Betauer.Nodes;

[InputEvents]
[Notification(Process = false, PhysicsProcess = false)]
public partial class NodeEventHandler : Node {

    private static readonly NodeEventHandler DefaultInstanceValue = new();
    private static bool _isDefaultInstanceAdded = false;
    
    public static NodeEventHandler DefaultInstance {
        get {
            if (!_isDefaultInstanceAdded) {
                var sceneTree = (SceneTree)Engine.GetMainLoop();
                if (sceneTree != null) {
                    _isDefaultInstanceAdded = true;
                    sceneTree.Root.AddChildDeferred(DefaultInstanceValue);
                }
            }
            return DefaultInstanceValue;
        }
    }
    
    public override void _EnterTree() {
        GetParent().ChildEnteredTree += (Node _) => GetParent()?.MoveChildDeferred(this, -1);
    }

    public override partial void _Input(InputEvent e);
    public override partial void _UnhandledInput(InputEvent e);
    public override partial void _UnhandledKeyInput(InputEvent e);
    public override partial void _ShortcutInput(InputEvent inputEvent);
    public override partial void _Notification(int what);
    
    public bool IsMouseInsideScreen { get; internal set; } = false;
    public bool IsWindowFocused { get; internal set; } = true;
    public bool IsApplicationFocused { get; internal set; } = true;

    public NodeEventHandler() {
        OnWMMouseEnter += () => IsMouseInsideScreen = true;
        OnWMMouseExit += () => IsMouseInsideScreen = false;
        OnWMWindowFocusIn += () => IsWindowFocused = true;
        OnWMWindowFocusOut += () => IsWindowFocused = false;
        OnApplicationFocusIn += () => IsApplicationFocused = true;
        OnApplicationFocusOut += () => IsApplicationFocused = false;
    }
}