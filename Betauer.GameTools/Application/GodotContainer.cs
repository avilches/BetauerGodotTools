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
    private Action<ContainerBuilder>? _containerConfig = null;

    public GodotContainer(Node owner, bool addSingletonNodesToTree = true, bool injectPropertiesOnNodeAddedToTree = true) {
        _owner = owner;
        _addSingletonNodesToTree = addSingletonNodesToTree;
        _injectPropertiesOnNodeAddedToTree = injectPropertiesOnNodeAddedToTree;
    }

    public GodotContainer Start(Action<ContainerBuilder>? containerConfig = null) {
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
        _owner.GetTree().NodeAdded += InjectIfNotInjected;
    }

    private void CreateContainer() {
        _container.OnCreated += OnServiceCreated;
        var containerBuilder = _container.CreateBuilder();
        _containerConfig?.Invoke(containerBuilder);
        containerBuilder.Build();
    }

    private void InjectIfNotInjected(Node node) {
        if (node.GetScript().AsGodotObject() is CSharpScript && !IsInjected(node)) _container.InjectServices(node);
    }

    private void OnServiceCreated(Lifetime lifetime, object instance) {
        if (instance is GodotObject o)
            SetAlreadyInjected(o); // This avoid nodes are injected twice if they are added to the tree later
        if (_addSingletonNodesToTree && 
            lifetime == Lifetime.Singleton && 
            instance is Node node && 
            node.GetParent() == null) {
            _owner.GetViewport().AddChildDeferred(node);
        }
    }
    
    private static readonly StringName MetaDiInjected = "__di_injected";
    private static void SetAlreadyInjected(GodotObject node) => node.SetMeta(MetaDiInjected, true);
    private static bool IsInjected(GodotObject node) => node.HasMeta(MetaDiInjected);

}