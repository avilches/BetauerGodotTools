using System;
using Godot;

namespace Betauer.DI {
    /**
     * A Container that listen for nodes added to the tree and inject services inside of them + process the OnReady tag
     */
    public class GodotContainer : Node {
        public ContainerBuilder? Builder;

        private Container? _container;

        public GodotContainer() {
            PauseMode = PauseModeEnum.Process;
        }

        public GodotContainer(Container container) {
            SetContainer(container);
        }

        public void SetContainer(Container container) {
            _container = container;
            Builder = container.CreateBuilder();
        }

        public void AutoConfigure() {
            _container = new Container(this);
            Builder = _container.CreateBuilder();
            Builder.Static<Func<SceneTree>>(GetTree);
            Builder.Scan().Build();
        }

        public override void _EnterTree() {
            EnableInjection();
        }

        public override void _ExitTree() {
            DisableInjection();
        }

        public void EnableInjection() {
            if (!GetTree().IsConnected(SignalConstants.SceneTree_NodeAddedSignal, this, nameof(_GodotSignalNodeAdded)))
                GetTree().Connect(SignalConstants.SceneTree_NodeAddedSignal, this, nameof(_GodotSignalNodeAdded));
        }

        public void DisableInjection() {
            if (GetTree().IsConnected(SignalConstants.SceneTree_NodeAddedSignal, this, nameof(_GodotSignalNodeAdded)))
                GetTree().Disconnect(SignalConstants.SceneTree_NodeAddedSignal, this, nameof(_GodotSignalNodeAdded));
        }

        private const string MetaInjected = "__injected";

        // Method called by Godot
        private void _GodotSignalNodeAdded(Node node) {
            if (node.GetScript() is CSharpScript script) {
                _container.LoadOnReadyNodes(node);
                if (!node.HasMeta(MetaInjected)) {
                    _container.InjectAllFields(node);
                    node.SetMeta(MetaInjected, true);
                }
            }
        }
    }
}