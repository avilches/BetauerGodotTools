using Betauer.Application.Monitor;
using Betauer.Application.Screen;
using Betauer.DI;
using Betauer.DI.ServiceProvider;
using Betauer.Tools.Logging;
using Betauer.Core.Memory;
using Betauer.Core.Nodes;
using Betauer.OnReady;
using Betauer.Time;
using Godot;
using Container = Betauer.DI.Container;

namespace Betauer.Application {
    /**
     * A Container that listen for nodes added to the tree and inject services inside of them + process the OnReady tag
     */
    public abstract class AutoConfiguration : Node {
        protected readonly Container Container;
        protected readonly MainLoopNotificationsHandler MainLoopNotificationsHandlerInstance;
        private readonly Options _options;

        public class Options {
            public bool AddSingletonNodesToTree = true;
            public float ObjectWatcherTimer = 10f;
        }

        private class Configuration {
            private readonly AutoConfiguration _outer;

            public Configuration(AutoConfiguration outer) {
                _outer = outer;
            }
            
            [Service] public SceneTree SceneTree => _outer.GetTree();
            [Service] public MainLoopNotificationsHandler MainLoopNotificationsHandler => _outer.MainLoopNotificationsHandlerInstance;
            [Service] public DebugOverlayManager DebugOverlayManager => new();
            [Service(Lifetime.Transient)] public GodotStopwatch GodotStopwatch => new(_outer.GetTree());
        }

        protected AutoConfiguration(Options? options = null) {
            _options = options ?? new Options();
            Container = new Container();

            MainLoopNotificationsHandlerInstance = new MainLoopNotificationsHandler();
            MainLoopNotificationsHandlerInstance.OnWmQuitRequest += () => {
                LoggerFactory.SetAutoFlush(true);
                GD.Print($"[WmQuitRequest] Uptime: {Project.Uptime.TotalMinutes:0} min {Project.Uptime.Seconds:00} sec");
            };
            
            AddChild(DefaultNodeHandler.Instance);
        }

        public override void _EnterTree() {
            // Container can't be built before _EnterTree because the SceneTree is exposed as a service using GetTree()
            // It needs to be called in _EnterTree because the _Ready() is called after the the main scene _Ready(), so the main
            // scene will not have services injected.
            StartContainer();

            DefaultObjectWatcherTask.Instance.Start(GetTree(), _options.ObjectWatcherTimer);
            PauseMode = PauseModeEnum.Process;

            AddConsoleCommands(Container.Resolve<DebugOverlayManager>().DebugConsole);
        }

        /// <summary>
        /// Virtual method so it can be overriden to add more commands.
        /// Just call to base.AddConsoleCommands(debugConsole) to ensure the default commands are still added.
        /// </summary>
        /// <param name="debugConsole"></param>
        public virtual void AddConsoleCommands(DebugConsole debugConsole) {
            debugConsole.AddHelpCommand();
            debugConsole.AddEngineTimeScaleCommand();
            debugConsole.AddEngineTargetFpsCommand();
            debugConsole.AddClearConsoleCommand();
            debugConsole.AddQuitCommand();
            debugConsole.AddNodeHandlerInfoCommand();
            debugConsole.AddSignalManagerCommand();
            debugConsole.AddShowAllCommand();
            debugConsole.AddSystemInfoCommand();
            ScreenSettingsManager? screenSettings = Container.ResolveOr<ScreenSettingsManager>(() => null);
            if (screenSettings != null) {
                debugConsole.AddScreenSettingsManagerMonitor(screenSettings);
            }
        }

        private void StartContainer() {
            Container.OnCreated += (lifetime, o) => {
                if (o is Node node) {
                    MarkNodeAsAlreadyInjected(node);
                    if (string.IsNullOrWhiteSpace(node.Name)) node.Name = node.GetType().Name; // This is useful to debug in Remote mode
                    if (_options.AddSingletonNodesToTree && 
                        lifetime == Lifetime.Singleton && 
                        node.GetParent() == null) {
                        GetTree().Root.AddChildDeferred(node);
                    }
                }
            };
            var containerBuilder = Container.CreateBuilder();
            OnBuildContainer(containerBuilder);
            containerBuilder.Build().InjectServices(this);
            GetTree().Connect("node_added", this, nameof(_GodotSignalNodeAdded));
        }

        /// <summary>
        /// Virtual method so it can be overriden to do more stuff with the container before it finishes.
        /// Call to base.OnBuildContainer(builder) to scan the current assembly and use the current class as configuration. 
        /// </summary>
        /// <param name="???"></param>
        public virtual void OnBuildContainer(ContainerBuilder builder) {
            builder
                .Scan(GetType().Assembly)
                .ScanConfiguration(new Configuration(this));
            
        }

        // Method called by Godot when a Node is added to the tree
        private void _GodotSignalNodeAdded(Node node) {
            if (node.GetScript() is CSharpScript) {
                OnReadyScanner.ScanAndInject(node);
                // If the Node has been created by Godot (through instantiating a scene) and it's added to the tree,
                // the services will be injected.
                // But if the Node has been created by the application through the Container, the node will already have
                // all the dependencies injected. To avoid inject the dependencies twice (by the container and here when
                // it's added to the tree), 
                
                if (IsNodeMarkedInjected(node)) {
                    // A Node could be
                    Container.InjectServices(node);
                }
            }
        }

        private const string MetaInjected = "__injected";
        private static void MarkNodeAsAlreadyInjected(Node node) => node.SetMeta(MetaInjected, true);
        private static bool IsNodeMarkedInjected(Node node) => !node.HasMeta(MetaInjected);

        public override void _Notification(int what) {
            MainLoopNotificationsHandlerInstance.Execute(what);
        }
    }
}