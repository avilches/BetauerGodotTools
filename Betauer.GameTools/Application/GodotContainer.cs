using System;
using Betauer.DI.ServiceProvider;
using Betauer.Core.Nodes;
using Betauer.Core.Signal;
using Betauer.DI;
using Betauer.NodePath;
using Godot;
using Container = Betauer.DI.Container;

namespace Betauer.Application;

/**
 * A Container that listen for nodes added to the tree and inject services inside of them + process the NodePath tag
 */
public class GodotContainer {
    private readonly Node _owner;
    private readonly bool _addSingletonNodesToTree;
    private readonly bool _injectPropertiesOnNodeAddedToTree = true;
    private readonly Container _container = new();
    private Action<Container.Builder>? _containerConfig = null;

    public GodotContainer(Node owner, bool addSingletonNodesToTree = true, bool injectPropertiesOnNodeAddedToTree = true) {
        _owner = owner;
        _addSingletonNodesToTree = addSingletonNodesToTree;
        _injectPropertiesOnNodeAddedToTree = injectPropertiesOnNodeAddedToTree;
    }

    public GodotContainer Start(Action<Container.Builder>? containerConfig = null) {
        _containerConfig = containerConfig;
        if (_owner.IsInsideTree()) StartContainer();
        else _owner.Connect(Node.SignalName.TreeEntered, Callable.From(StartContainer), SignalTools.SignalFlags(true));
        return this;
    }

    private void StartContainer() {
        if (_injectPropertiesOnNodeAddedToTree) {
            NodePathScanner.ConfigureAutoInject(_owner.GetTree());
        }
        CreateContainer();
    }

    private void CreateContainer() {
        if (_addSingletonNodesToTree) _container.OnCreated += AddNodeSingletonsToTree;
        _container.OnPostInject += MarkObjectAsInjected; // Avoid double injection 
        _owner.GetTree().NodeAdded += InjectIfNotInjected; // Auto Inject nodes when they are added to the tree
        var containerBuilder = _container.CreateBuilder();
        _containerConfig?.Invoke(containerBuilder);
        containerBuilder.Build();
    }

    private static void MarkObjectAsInjected(object instance) {
        // This avoid nodes are injected twice if they are added to the tree later
        if (instance is GodotObject o) SetAlreadyInjected(o); 
    }

    private void InjectIfNotInjected(Node node) {
        if (node.GetScript().AsGodotObject() is CSharpScript && !IsInjected(node)) _container.InjectServices(node);
    }

    private void AddNodeSingletonsToTree(Lifetime lifetime, object instance) {
        if (lifetime == Lifetime.Singleton && instance is Node node && node.GetParent() == null) {
            _owner.GetViewport().AddChildDeferred(node);
        }
    }
    
    private static readonly StringName MetaDiInjected = "__di_injected";
    private static void SetAlreadyInjected(GodotObject node) => node.SetMeta(MetaDiInjected, true);
    private static bool IsInjected(GodotObject node) => node.HasMeta(MetaDiInjected);

}