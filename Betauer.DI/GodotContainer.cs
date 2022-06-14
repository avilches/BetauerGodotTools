using System;
using Godot;

namespace Betauer.DI {
    /**
     * A Container that listen for nodes added to the tree and inject services inside of them + process the OnReady tag
     */
    public class GodotContainer : Node {
        private Container _container;

        public void SetContainer(Container container) {
            _container = container;
        }

        public void CreateAndScan() {
            var builder = new ContainerBuilder(this);
            builder.Static<Func<SceneTree>>(GetTree);
            builder.Scan();
            SetContainer(builder.Build());
        }

        public override void _EnterTree() {
            EnableInjection();
        }

        public override void _ExitTree() {
            DisableInjection();
        }

        public void EnableInjection() {
            if (!GetTree().IsConnected(SignalConstants.SceneTree_NodeAddedSignal, this, nameof(GodotExecuteNodeAdded)))
                GetTree().Connect(SignalConstants.SceneTree_NodeAddedSignal, this, nameof(GodotExecuteNodeAdded));
        }

        public void DisableInjection() {
            if (GetTree().IsConnected(SignalConstants.SceneTree_NodeAddedSignal, this, nameof(GodotExecuteNodeAdded)))
                GetTree().Disconnect(SignalConstants.SceneTree_NodeAddedSignal, this, nameof(GodotExecuteNodeAdded));
        }

        private const string MetaInjected = "__injected";

        public void GodotExecuteNodeAdded(Node node) {
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