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

        [Service] public Consumer Consumer => DefaultObjectWatcherRunner.Instance;
        [Service] public NodeHandler NodeHandler => DefaultNodeHandler.Instance;
        [Service] public SceneTree SceneTree => SceneTreeHolder.SceneTree;

        private bool _addSingletonNodesToTree = true;
        private float _watchTimer = 10f;

        public void EnableAddSingletonNodesToTree(bool enabled) => _addSingletonNodesToTree = enabled;
        public void SetWatchTimer(float watchTimer) => _watchTimer = watchTimer;

        // It needs to be in _EnterTree because the _Ready() is called after the the main scene _Ready(), so the main
        // scene will not have
        private bool _ready = false;
        public override void _EnterTree() {
            SceneTreeHolder.SceneTree = GetTree();
            DefaultObjectWatcherRunner.Instance.Start(_watchTimer);
            PauseMode = PauseModeEnum.Process;
            Container = new Container();
            this.OnReady(() => _ready = true, true);
            if (_addSingletonNodesToTree) {
                Container.OnCreate += (lifetime, o) => {
                    if (lifetime == Lifetime.Singleton && o is Node node) {
                        if (_ready) GetTree().Root.AddChild(node);
                        else GetTree().Root.CallDeferred("add_child", node);
                    }
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
        
        /**
         * Detect quit app (by ALT+F4, Command+Q or user menu)
         */
        public override void _Notification(int what) {
            if (what == MainLoop.NotificationWmQuitRequest) {
                LoggerFactory.EnableAutoFlush();
                DefaultObjectWatcherRunner.Instance.Consume(true);
            }
        }
    }
}