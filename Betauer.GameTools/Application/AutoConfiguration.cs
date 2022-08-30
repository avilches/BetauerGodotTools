using System;
using Betauer.DI;
using Betauer.Memory;
using Betauer.Nodes;
using Betauer.OnReady;
using Betauer.Signal;
using Godot;
using Container = Betauer.DI.Container;

namespace Betauer.Application {

    /**
     * A Container that listen for nodes added to the tree and inject services inside of them + process the OnReady tag
     */
    public abstract class AutoConfiguration : Node {
        protected Container Container;

        [Service] public ObjectWatcherNode ObjectWatcherNode => new ObjectWatcherNode();
        [Service] public NodeHandler NodeHandler => DefaultNodeHandler.Instance;
        [Service] public SceneTree SceneTree => SceneTreeHolder.SceneTree;

        public bool AddSingletonNodesToTree = true;
        
        public override void _EnterTree() {
            SceneTreeHolder.SceneTree = GetTree();
            PauseMode = PauseModeEnum.Process;
            Container = new Container();
            if (AddSingletonNodesToTree) {
                Container.OnCreate += (lifetime, o) => {
                    if (lifetime == Lifetime.Singleton && o is Node node) AddChild(node);
                };
            }
            Container.CreateBuilder()
                .Scan(GetType().Assembly)
                .ScanConfiguration(this)
                .Build();
            GetTree().OnNodeAdded(_GodotSignalNodeAdded);
        }

        private const string MetaInjected = "__injected";

        // Method called by Godot
        private void _GodotSignalNodeAdded(Node node) {
            if (node.GetScript() is CSharpScript) {
                OnReadyScanner.ScanAndInject(node);
                if (!node.HasMeta(MetaInjected)) {
                    Container.InjectServices(node);
                    node.SetMeta(MetaInjected, true);
                }
            }
        }
    }
}