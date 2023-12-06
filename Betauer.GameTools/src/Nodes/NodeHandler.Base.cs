using Godot;

namespace Betauer.Nodes;

public abstract class BaseNodeHandler : BaseHandler {
    protected readonly Node Node;

    protected BaseNodeHandler(Node node, string? name = null) : base(name) {
        Node = node;
    }

    public override bool IsEnabled {
        get {
            if (base.IsEnabled == false) return false;
            var tree = Node.GetTree();
            return tree != null && ShouldProcess(tree.Paused, Node.ProcessMode);
        }
    }

    public override bool IsDestroyed => base.IsDestroyed || GodotObject.IsInstanceValid(Node) == false;

    public static bool ShouldProcess(bool pause, Node.ProcessModeEnum processMode) {
        if (processMode == Node.ProcessModeEnum.Inherit) return !pause;
        return processMode == Node.ProcessModeEnum.Always ||
               (pause && processMode == Node.ProcessModeEnum.WhenPaused) ||
               (!pause && processMode == Node.ProcessModeEnum.Pausable);
    }
}