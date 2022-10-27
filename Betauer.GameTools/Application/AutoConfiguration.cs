using System;
using System.Threading.Tasks;
using Betauer.Application.Monitor;
using Betauer.DI;
using Betauer.DI.ServiceProvider;
using Betauer.Memory;
using Betauer.Nodes;
using Betauer.OnReady;
using Betauer.Signal;
using Betauer.Time;
using Godot;
using Container = Betauer.DI.Container;

namespace Betauer.Application {
    /**
     * A Container that listen for nodes added to the tree and inject services inside of them + process the OnReady tag
     */
    public abstract class AutoConfiguration : Node {
        protected readonly Container Container;
        protected readonly MainLoopNotificationsHandler MainLoopNotificationsHandler;
        protected readonly DebugOverlayManager DebugOverlayManagerInstance;

        [Service] public Consumer Consumer => DefaultObjectWatcherTask.Instance;
        [Service] public SceneTree SceneTree => GetTree();
        [Service] public MainLoopNotificationsHandler MainLoopNotificationsHandlerFactory => MainLoopNotificationsHandler;
        [Service] public DebugOverlayManager DebugOverlayManager => DebugOverlayManagerInstance;
        [Service] public DebugOverlay DefaultDebugOverlay => DebugOverlayManagerInstance.CreateOverlay();
        
        [Service(Lifetime.Transient)]
        public GodotStopwatch GodotStopwatch => new(GetTree());

        private bool _addSingletonNodesToTree = true;
        private float _objectWatcherTimer = 10f;
        private bool _isReady = false;

        public void EnableAddSingletonNodesToTree(bool enabled) => _addSingletonNodesToTree = enabled;
        public void SetObjectWatcherTimer(float watchTimer) => _objectWatcherTimer = watchTimer;


        protected AutoConfiguration() {
            Container = new Container();
            MainLoopNotificationsHandler = new MainLoopNotificationsHandler();
            DebugOverlayManagerInstance = new DebugOverlayManager();
            AddChild(DefaultNodeHandler.Instance);
        }

        public override void _EnterTree() {
            // It can't be called before _EnterTree because the SceneTree is exposed as a service using GetTree()
            // It needs to be called in _EnterTree because the _Ready() is called after the the main scene _Ready(), so the main
            // scene will not have services injected.
            StartContainer();

            MainLoopNotificationsHandler.OnWmQuitRequest += LoggerFactory.EnableAutoFlush;
            TaskScheduler.UnobservedTaskException += (o, args) => {
                // This event logs errors in non-awaited Task. It needs
                var e = args.Exception;
                GD.PrintErr($"{StringTools.FastFormatDateTime(DateTime.Now)} [Error] TaskScheduler.UnobservedTaskException:\n{e}");
                if (FeatureFlags.IsTerminateOnExceptionEnabled()) {
                    SceneTree.Notification(MainLoop.NotificationWmQuitRequest);
                }
            };
            AppDomain.CurrentDomain.UnhandledException += (o, args) => {
                // This event logs errors in _Input/_Ready or any other method called from Godot (async or non-async)
                // but it only works if runtime/unhandled_exception_policy is "0" (terminate),
                // so the quit is not really needed
                // If unhandled_exception_policy is "1" (LogError), the error is not logged neither this event is called
                var e = args.ExceptionObject;
                GD.PrintErr($"{StringTools.FastFormatDateTime(DateTime.Now)} [Error] AppDomain.CurrentDomain.UnhandledException:\n{e}");
            };
            DefaultObjectWatcherTask.Instance.Start(GetTree(), _objectWatcherTimer);
            PauseMode = PauseModeEnum.Process;
            this.OnReady(() => _isReady = true, true);
            
            DebugOverlayManager.DebugConsole.AddNodeHandlerInfoCommand();
            DebugOverlayManager.DebugConsole.AddSignalManagerCommand();
        }

        private void StartContainer() {
            if (_addSingletonNodesToTree) {
                Container.OnCreated += (lifetime, o) => {
                    if (o is Node node) {
                        MarkNodeAsAlreadyInjected(node);
                        if (lifetime == Lifetime.Singleton && node.GetParent() == null) {
                            if (string.IsNullOrWhiteSpace(node.Name)) node.Name = node.GetType().Name; // This is useful to debug in Remote mode
                            if (_isReady) GetTree().Root.AddChild(node);
                            else GetTree().Root.AddChildDeferred(node);
                        }
                    }
                };
            }
            Container.CreateBuilder()
                .Scan(GetType().Assembly)
                .ScanConfiguration(this)
                .Build()
                .InjectServices(this);
            GetTree().OnNodeAdded(_GodotSignalNodeAdded);
        }
        
        // Method called by Godot
        private void _GodotSignalNodeAdded(Node node) {
            if (node.GetScript() is CSharpScript) {
                OnReadyScanner.ScanAndInject(node);
                if (IsNodeInjected(node)) {
                    Container.InjectServices(node);
                }
            }
        }

        private const string MetaInjected = "__injected";
        private static void MarkNodeAsAlreadyInjected(Node node) => node.SetMeta(MetaInjected, true);
        private static bool IsNodeInjected(Node node) => !node.HasMeta(MetaInjected);

        public override void _Notification(int what) {
            MainLoopNotificationsHandler.Execute(what);
        }
    }
}