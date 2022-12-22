using System;
using Betauer.DI.ServiceProvider;
using Betauer.Core.Nodes;
using Betauer.Core.Signal;
using Betauer.DI;
using Betauer.OnReady;
using Godot;
using Container = Betauer.DI.Container;
using Object = Godot.Object;

namespace Betauer.Application;

/**
 * A Container that listen for nodes added to the tree and inject services inside of them + process the OnReady tag
 */
public class GodotContainer {
    private const string MetaInjected = "__injected";

    private readonly Node _owner;
    private readonly bool _addSingletonNodesToTree;
    private readonly bool _injectPropertiesOnReady = true;
    private readonly Container _container = new();
    private Action<ContainerBuilder>? _containerConfig = null;

    public GodotContainer(Node owner, bool addSingletonNodesToTree = true, bool injectPropertiesOnReady = true) {
        _owner = owner;
        _addSingletonNodesToTree = addSingletonNodesToTree;
        _injectPropertiesOnReady = injectPropertiesOnReady;
    }

    public GodotContainer Start(Action<ContainerBuilder>? containerConfig = null) {
        _containerConfig = containerConfig;
        if (_owner.GetTree() != null) StartContainer();
        else _owner.OnTreeEntered(StartContainer, true);
        return this;
    }

    private void StartContainer() {
        if (_injectPropertiesOnReady) {
            OnReadyScanner.ConfigureAutoInject(_owner.GetTree());
        }
        CreateContainer();
        _owner.GetTree().OnNodeAdded(InjectIfNotInjected);
    }

    private void CreateContainer() {
        _container.OnCreated += OnServiceCreated;
        var containerBuilder = _container.CreateBuilder().Scan(_owner.GetType().Assembly);
        _containerConfig?.Invoke(containerBuilder);
        containerBuilder.Build();
    }

    private void InjectIfNotInjected(Node node) {
        if (node.GetScript().AsGodotObject() is CSharpScript && !IsInjected(node)) _container.InjectServices(node);
    }

    private void OnServiceCreated(Lifetime lifetime, object instance) {
        if (instance is Object o)
            SetAlreadyInjected(o); // This avoid nodes are injected twice if they are added to the tree later
        if (_addSingletonNodesToTree && lifetime == Lifetime.Singleton && instance is Node node &&
            node.GetParent() == null) {
            _owner.GetTree().Root.AddChildDeferred(node);
        }
    }
    
    private static void SetAlreadyInjected(Object node) => node.SetMeta(MetaInjected, true);
    private static bool IsInjected(Object node) => node.HasMeta(MetaInjected);

}