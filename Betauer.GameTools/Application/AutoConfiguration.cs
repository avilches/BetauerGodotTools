using System;
using Betauer.DI.ServiceProvider;
using Betauer.Core.Nodes;
using Betauer.Core.Signal;
using Betauer.DI;
using Betauer.OnReady;
using Godot;
using Container = Betauer.DI.Container;
using Object = Godot.Object;

namespace Betauer.Application {
    /**
     * A Container that listen for nodes added to the tree and inject services inside of them + process the OnReady tag
     */
    public class AutoConfiguration {
        private readonly Node _owner;
        private readonly bool _addSingletonNodesToTree;
        private readonly bool _autoInjectOnReady = true;
        private readonly Container _container = new();
        private Action<ContainerBuilder>? _containerConfig = null;

        public AutoConfiguration(Node owner, bool addSingletonNodesToTree = true, bool autoInjectOnReady = true) {
            _owner = owner;
            _addSingletonNodesToTree = addSingletonNodesToTree;
            _autoInjectOnReady = autoInjectOnReady;
        }
        
        public AutoConfiguration Start(Action<ContainerBuilder>? containerConfig = null) {
            _containerConfig = containerConfig;
            if (_owner.GetTree() != null) StartContainer();
            else _owner.OnTreeEntered(StartContainer, true);
            return this;
        }

        private const string MetaInjected = "__injected";
        private static void SetAlreadyInjected(Object node) => node.SetMeta(MetaInjected, true);
        private static bool IsInjected(Object node) => node.HasMeta(MetaInjected);

        private void StartContainer() {
            OnReadyScanner.ConfigureAutoInject(_owner.GetTree());
            _owner.ProcessMode = Node.ProcessModeEnum.Always;
            _container.OnCreated += (lifetime, instance) => {
                if (instance is Object o) SetAlreadyInjected(o);
                if (instance is Node node) {
                    if (string.IsNullOrWhiteSpace(node.Name)) node.Name = node.GetType().Name; // This is useful to debug in Remote mode
                    if (_addSingletonNodesToTree && lifetime == Lifetime.Singleton && node.GetParent() == null) {
                        _owner.GetTree().Root.AddChildDeferred(node);
                    }
                }
            };

            var containerBuilder = _container.CreateBuilder().Scan(_owner.GetType().Assembly);
            _containerConfig?.Invoke(containerBuilder);
            containerBuilder.Build();

            _owner.GetTree().OnNodeAdded(node => {
                if (node.GetScript().AsGodotObject() is not CSharpScript script) return;
                
                // Process all the [OnReady] fields
                OnReadyScanner.ScanAndInject(node);
                
                // If the Node has been created by Godot (through instantiating a scene) and it's added to the tree,
                // the services will be injected.
                // But if the Node has been created by the application through the Container, the node will already have
                // all the dependencies injected. To avoid inject the dependencies twice (by the container and here when
                // it's added to the tree), 
                if (!IsInjected(node)) {
                    // All nodes created by the container are marked as injected in the OnCreated event above.
                    // So, if a node is not marked as injected is because the node has been created by Godot when
                    // instantiating a scene, so let's inject the services here
                    _container.InjectServices(node);
                }
            });
        }
    }
}