using System;
using Betauer.Core;
using Betauer.DI.ServiceProvider;
using Betauer.Core.Nodes;
using Godot;
using Container = Betauer.DI.Container;

namespace Betauer.Application;

/**
 * A tool to allow auto-inject services in nodes when they are added to the tree for the first time
 */
public static class GodotContainerInjector {

    private static readonly StringName MetaDiInjected = "__di_injected";
    
    public static class Flags {
        public const string Autoload = "Autoload";

        public static void ValidateAutoloadFlag(Provider provider) {
            if (!provider.RealType.IsSubclassOf(typeof(Node))) {
                throw new Exception($"Error in {Lifetime.Singleton}:{provider.RealType.GetTypeName()}. The {nameof(Autoload)} flag can only be used with Nodes, but {provider.RealType} is not a Node.");
            }
        }
    }
    
    public static Container.Builder InjectOnEnterTree(this Container.Builder builder, SceneTree sceneTree, bool autoAddNodeSingletonsToTree = false) {
        var container = builder.Container;

        container.OnValidate += (provider) => {
            if (provider.GetFlag(Flags.Autoload)) Flags.ValidateAutoloadFlag(provider);
        };
        
        // Auto add singleton Node to the scene tree root if they have the "Autoload" flag enabled
        container.OnCreated += providerResolved => {
            if (providerResolved is { Instance: Node node } && 
                (autoAddNodeSingletonsToTree || providerResolved.GetFlag(Flags.Autoload)) && 
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