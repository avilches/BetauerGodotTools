using Betauer.DI.ServiceProvider;
using Betauer.Core.Nodes;
using Betauer.DI;
using Godot;
using Container = Betauer.DI.Container;

namespace Betauer.Application;

/**
 * A tool to allow auto-inject services in nodes when they are added to the tree for the first time
 */
public static class GodotContainerInjector {

    private static readonly StringName MetaDiInjected = "__di_injected";
    
    public static Container.Builder InjectOnEnterTree(this Container.Builder builder, SceneTree sceneTree, bool autoAddNodeSingletonsToTree = false) {
        var container = builder.Container;
        // Auto add singleton Node to the scene tree root if they have the "AddToTree" flag enabled. This mimics the Autoload behaviour.
        container.OnCreated += providerResolved => {
            if (providerResolved is { Lifetime: Lifetime.Singleton, Instance: Node node } && 
                (autoAddNodeSingletonsToTree || providerResolved.GetFlag("AddToTree")) && 
                 node.GetParent() == null) {
                sceneTree.Root.AddChildDeferred(node);
            }
        };
        // Inject services in nodes when they are added to the tree. Pay attention if a node is remove from the tree and added again, this
        // will trigger the injection again. To avoid this, the MetaDiInjected flag is used: if its present, the node is not injected again.
        sceneTree.NodeAdded += (node) => {
            if (node.GetScript().AsGodotObject() is CSharpScript && !node.HasMeta(MetaDiInjected)) {
                container.InjectServices(node);
            }
        };

        // When a service is a Node and it has been injected, flag it as injected. This avoid double injection when the node is
        // removed and added to the tree again.
        container.OnPostInject += instance => {
            if (instance is Node o) o.SetMeta(MetaDiInjected, true);
        };
        return builder;
    }
}