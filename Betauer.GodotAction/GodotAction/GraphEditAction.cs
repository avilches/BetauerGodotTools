using System;
using Godot;
using System.Collections.Generic;
using Environment = Godot.Environment;
using Animation = Godot.Animation;
using Object = Godot.Object;

namespace Betauer.GodotAction {
    public class GraphEditAction : GraphEdit {

        private List<Action<float>>? _onProcessActions; 
        private List<Action<float>>? _onPhysicsProcessActions; 
        private List<Action<InputEvent>>? _onInputActions; 
        private List<Action<InputEvent>>? _onUnhandledInputActions; 
        private List<Action<InputEventKey>>? _onUnhandledKeyInputActions;

        public GraphEditAction OnProcess(Action<float> action) {
            _onProcessActions ??= new List<Action<float>>(1);
            _onProcessActions.Add(action);
            SetProcess(true);
            return this;
        }
        public GraphEditAction OnPhysicsProcess(Action<float> action) {
            _onPhysicsProcessActions ??= new List<Action<float>>(1);
            _onPhysicsProcessActions.Add(action);
            SetPhysicsProcess(true);
            return this;
        }

        public GraphEditAction OnInput(Action<InputEvent> action) {
            _onInputActions ??= new List<Action<InputEvent>>(1);
            _onInputActions.Add(action);
            SetProcessInput(true);
            return this;
        }

        public GraphEditAction OnUnhandledInput(Action<InputEvent> action) {
            _onUnhandledInputActions ??= new List<Action<InputEvent>>(1);
            _onUnhandledInputActions.Add(action);
            SetProcessUnhandledInput(true);
            return this;
        }

        public GraphEditAction OnUnhandledKeyInput(Action<InputEventKey> action) {
            _onUnhandledKeyInputActions ??= new List<Action<InputEventKey>>(1);
            _onUnhandledKeyInputActions.Add(action);
            SetProcessUnhandledKeyInput(true);
            return this;
        }

        public GraphEditAction RemoveOnProcess(Action<float> action) {
            _onProcessActions?.Remove(action);
            return this;
        }

        public GraphEditAction RemoveOnPhysicsProcess(Action<float> action) {
            _onPhysicsProcessActions?.Remove(action);
            return this;
        }

        public GraphEditAction RemoveOnInput(Action<InputEvent> action) {
            _onInputActions?.Remove(action);
            return this;
        }

        public GraphEditAction RemoveOnUnhandledInput(Action<InputEvent> action) {
            _onUnhandledInputActions?.Remove(action);
            return this;
        }

        public GraphEditAction RemoveOnUnhandledKeyInput(Action<InputEventKey> action) {
            _onUnhandledKeyInputActions?.Remove(action);
            return this;
        }

        public override void _Process(float delta) {
            if (_onProcessActions == null || _onProcessActions.Count == 0) {
                SetProcess(false);
                return;
            }
            for (var i = 0; i < _onProcessActions.Count; i++) _onProcessActions[i].Invoke(delta);
        }

        public override void _PhysicsProcess(float delta) {
            if (_onPhysicsProcessActions == null || _onPhysicsProcessActions.Count == 0) {
                SetPhysicsProcess(false);
                return;
            }
            for (var i = 0; i < _onPhysicsProcessActions.Count; i++) _onPhysicsProcessActions[i].Invoke(delta);
        }

        public override void _Input(InputEvent @event) {
            if (_onInputActions == null || _onInputActions?.Count == 0) {
                SetProcessInput(false);
                return;
            }
            for (var i = 0; i < _onInputActions.Count; i++) _onInputActions[i].Invoke(@event);
        }

        public override void _UnhandledInput(InputEvent @event) {
            if (_onUnhandledInputActions == null || _onUnhandledInputActions.Count == 0) {
                SetProcessUnhandledInput(false);
                return;
            }
            for (var i = 0; i < _onUnhandledInputActions.Count; i++) _onUnhandledInputActions[i].Invoke(@event);
        }

        public override void _UnhandledKeyInput(InputEventKey @event) {
            if (_onUnhandledKeyInputActions == null || _onUnhandledKeyInputActions.Count == 0) {
                SetProcessUnhandledKeyInput(false);
                return;
            }
            for (var i = 0; i < _onUnhandledKeyInputActions.Count; i++) _onUnhandledKeyInputActions[i].Invoke(@event);
        }

        private List<Action>? _onBeginNodeMoveAction; 
        public GraphEditAction OnBeginNodeMove(Action action) {
            if (_onBeginNodeMoveAction == null || _onBeginNodeMoveAction.Count == 0) {
                _onBeginNodeMoveAction ??= new List<Action>(); 
                Connect("_begin_node_move", this, nameof(ExecuteBeginNodeMove));
            }
            _onBeginNodeMoveAction.Add(action);
            return this;
        }
        public GraphEditAction RemoveOnBeginNodeMove(Action action) {
            if (_onBeginNodeMoveAction == null || _onBeginNodeMoveAction.Count == 0) return this;
            _onBeginNodeMoveAction.Remove(action); 
            if (_onBeginNodeMoveAction.Count == 0) {
                Disconnect("_begin_node_move", this, nameof(ExecuteBeginNodeMove));
            }
            return this;
        }
        private void ExecuteBeginNodeMove() {
            if (_onBeginNodeMoveAction == null || _onBeginNodeMoveAction.Count == 0) return;
            for (var i = 0; i < _onBeginNodeMoveAction.Count; i++) _onBeginNodeMoveAction[i].Invoke();
        }
        

        private List<Action<Vector2, string, int>>? _onConnectionFromEmptyAction; 
        public GraphEditAction OnConnectionFromEmpty(Action<Vector2, string, int> action) {
            if (_onConnectionFromEmptyAction == null || _onConnectionFromEmptyAction.Count == 0) {
                _onConnectionFromEmptyAction ??= new List<Action<Vector2, string, int>>(); 
                Connect("connection_from_empty", this, nameof(ExecuteConnectionFromEmpty));
            }
            _onConnectionFromEmptyAction.Add(action);
            return this;
        }
        public GraphEditAction RemoveOnConnectionFromEmpty(Action<Vector2, string, int> action) {
            if (_onConnectionFromEmptyAction == null || _onConnectionFromEmptyAction.Count == 0) return this;
            _onConnectionFromEmptyAction.Remove(action); 
            if (_onConnectionFromEmptyAction.Count == 0) {
                Disconnect("connection_from_empty", this, nameof(ExecuteConnectionFromEmpty));
            }
            return this;
        }
        private void ExecuteConnectionFromEmpty(Vector2 release_position, string to, int to_slot) {
            if (_onConnectionFromEmptyAction == null || _onConnectionFromEmptyAction.Count == 0) return;
            for (var i = 0; i < _onConnectionFromEmptyAction.Count; i++) _onConnectionFromEmptyAction[i].Invoke(release_position, to, to_slot);
        }
        

        private List<Action<string, int, string, int>>? _onConnectionRequestAction; 
        public GraphEditAction OnConnectionRequest(Action<string, int, string, int> action) {
            if (_onConnectionRequestAction == null || _onConnectionRequestAction.Count == 0) {
                _onConnectionRequestAction ??= new List<Action<string, int, string, int>>(); 
                Connect("connection_request", this, nameof(ExecuteConnectionRequest));
            }
            _onConnectionRequestAction.Add(action);
            return this;
        }
        public GraphEditAction RemoveOnConnectionRequest(Action<string, int, string, int> action) {
            if (_onConnectionRequestAction == null || _onConnectionRequestAction.Count == 0) return this;
            _onConnectionRequestAction.Remove(action); 
            if (_onConnectionRequestAction.Count == 0) {
                Disconnect("connection_request", this, nameof(ExecuteConnectionRequest));
            }
            return this;
        }
        private void ExecuteConnectionRequest(string from, int from_slot, string to, int to_slot) {
            if (_onConnectionRequestAction == null || _onConnectionRequestAction.Count == 0) return;
            for (var i = 0; i < _onConnectionRequestAction.Count; i++) _onConnectionRequestAction[i].Invoke(from, from_slot, to, to_slot);
        }
        

        private List<Action<string, int, Vector2>>? _onConnectionToEmptyAction; 
        public GraphEditAction OnConnectionToEmpty(Action<string, int, Vector2> action) {
            if (_onConnectionToEmptyAction == null || _onConnectionToEmptyAction.Count == 0) {
                _onConnectionToEmptyAction ??= new List<Action<string, int, Vector2>>(); 
                Connect("connection_to_empty", this, nameof(ExecuteConnectionToEmpty));
            }
            _onConnectionToEmptyAction.Add(action);
            return this;
        }
        public GraphEditAction RemoveOnConnectionToEmpty(Action<string, int, Vector2> action) {
            if (_onConnectionToEmptyAction == null || _onConnectionToEmptyAction.Count == 0) return this;
            _onConnectionToEmptyAction.Remove(action); 
            if (_onConnectionToEmptyAction.Count == 0) {
                Disconnect("connection_to_empty", this, nameof(ExecuteConnectionToEmpty));
            }
            return this;
        }
        private void ExecuteConnectionToEmpty(string from, int from_slot, Vector2 release_position) {
            if (_onConnectionToEmptyAction == null || _onConnectionToEmptyAction.Count == 0) return;
            for (var i = 0; i < _onConnectionToEmptyAction.Count; i++) _onConnectionToEmptyAction[i].Invoke(from, from_slot, release_position);
        }
        

        private List<Action>? _onCopyNodesRequestAction; 
        public GraphEditAction OnCopyNodesRequest(Action action) {
            if (_onCopyNodesRequestAction == null || _onCopyNodesRequestAction.Count == 0) {
                _onCopyNodesRequestAction ??= new List<Action>(); 
                Connect("copy_nodes_request", this, nameof(ExecuteCopyNodesRequest));
            }
            _onCopyNodesRequestAction.Add(action);
            return this;
        }
        public GraphEditAction RemoveOnCopyNodesRequest(Action action) {
            if (_onCopyNodesRequestAction == null || _onCopyNodesRequestAction.Count == 0) return this;
            _onCopyNodesRequestAction.Remove(action); 
            if (_onCopyNodesRequestAction.Count == 0) {
                Disconnect("copy_nodes_request", this, nameof(ExecuteCopyNodesRequest));
            }
            return this;
        }
        private void ExecuteCopyNodesRequest() {
            if (_onCopyNodesRequestAction == null || _onCopyNodesRequestAction.Count == 0) return;
            for (var i = 0; i < _onCopyNodesRequestAction.Count; i++) _onCopyNodesRequestAction[i].Invoke();
        }
        

        private List<Action>? _onDeleteNodesRequestAction; 
        public GraphEditAction OnDeleteNodesRequest(Action action) {
            if (_onDeleteNodesRequestAction == null || _onDeleteNodesRequestAction.Count == 0) {
                _onDeleteNodesRequestAction ??= new List<Action>(); 
                Connect("delete_nodes_request", this, nameof(ExecuteDeleteNodesRequest));
            }
            _onDeleteNodesRequestAction.Add(action);
            return this;
        }
        public GraphEditAction RemoveOnDeleteNodesRequest(Action action) {
            if (_onDeleteNodesRequestAction == null || _onDeleteNodesRequestAction.Count == 0) return this;
            _onDeleteNodesRequestAction.Remove(action); 
            if (_onDeleteNodesRequestAction.Count == 0) {
                Disconnect("delete_nodes_request", this, nameof(ExecuteDeleteNodesRequest));
            }
            return this;
        }
        private void ExecuteDeleteNodesRequest() {
            if (_onDeleteNodesRequestAction == null || _onDeleteNodesRequestAction.Count == 0) return;
            for (var i = 0; i < _onDeleteNodesRequestAction.Count; i++) _onDeleteNodesRequestAction[i].Invoke();
        }
        

        private List<Action<string, int, string, int>>? _onDisconnectionRequestAction; 
        public GraphEditAction OnDisconnectionRequest(Action<string, int, string, int> action) {
            if (_onDisconnectionRequestAction == null || _onDisconnectionRequestAction.Count == 0) {
                _onDisconnectionRequestAction ??= new List<Action<string, int, string, int>>(); 
                Connect("disconnection_request", this, nameof(ExecuteDisconnectionRequest));
            }
            _onDisconnectionRequestAction.Add(action);
            return this;
        }
        public GraphEditAction RemoveOnDisconnectionRequest(Action<string, int, string, int> action) {
            if (_onDisconnectionRequestAction == null || _onDisconnectionRequestAction.Count == 0) return this;
            _onDisconnectionRequestAction.Remove(action); 
            if (_onDisconnectionRequestAction.Count == 0) {
                Disconnect("disconnection_request", this, nameof(ExecuteDisconnectionRequest));
            }
            return this;
        }
        private void ExecuteDisconnectionRequest(string from, int from_slot, string to, int to_slot) {
            if (_onDisconnectionRequestAction == null || _onDisconnectionRequestAction.Count == 0) return;
            for (var i = 0; i < _onDisconnectionRequestAction.Count; i++) _onDisconnectionRequestAction[i].Invoke(from, from_slot, to, to_slot);
        }
        

        private List<Action>? _onDrawAction; 
        public GraphEditAction OnDraw(Action action) {
            if (_onDrawAction == null || _onDrawAction.Count == 0) {
                _onDrawAction ??= new List<Action>(); 
                Connect("draw", this, nameof(ExecuteDraw));
            }
            _onDrawAction.Add(action);
            return this;
        }
        public GraphEditAction RemoveOnDraw(Action action) {
            if (_onDrawAction == null || _onDrawAction.Count == 0) return this;
            _onDrawAction.Remove(action); 
            if (_onDrawAction.Count == 0) {
                Disconnect("draw", this, nameof(ExecuteDraw));
            }
            return this;
        }
        private void ExecuteDraw() {
            if (_onDrawAction == null || _onDrawAction.Count == 0) return;
            for (var i = 0; i < _onDrawAction.Count; i++) _onDrawAction[i].Invoke();
        }
        

        private List<Action>? _onDuplicateNodesRequestAction; 
        public GraphEditAction OnDuplicateNodesRequest(Action action) {
            if (_onDuplicateNodesRequestAction == null || _onDuplicateNodesRequestAction.Count == 0) {
                _onDuplicateNodesRequestAction ??= new List<Action>(); 
                Connect("duplicate_nodes_request", this, nameof(ExecuteDuplicateNodesRequest));
            }
            _onDuplicateNodesRequestAction.Add(action);
            return this;
        }
        public GraphEditAction RemoveOnDuplicateNodesRequest(Action action) {
            if (_onDuplicateNodesRequestAction == null || _onDuplicateNodesRequestAction.Count == 0) return this;
            _onDuplicateNodesRequestAction.Remove(action); 
            if (_onDuplicateNodesRequestAction.Count == 0) {
                Disconnect("duplicate_nodes_request", this, nameof(ExecuteDuplicateNodesRequest));
            }
            return this;
        }
        private void ExecuteDuplicateNodesRequest() {
            if (_onDuplicateNodesRequestAction == null || _onDuplicateNodesRequestAction.Count == 0) return;
            for (var i = 0; i < _onDuplicateNodesRequestAction.Count; i++) _onDuplicateNodesRequestAction[i].Invoke();
        }
        

        private List<Action>? _onEndNodeMoveAction; 
        public GraphEditAction OnEndNodeMove(Action action) {
            if (_onEndNodeMoveAction == null || _onEndNodeMoveAction.Count == 0) {
                _onEndNodeMoveAction ??= new List<Action>(); 
                Connect("_end_node_move", this, nameof(ExecuteEndNodeMove));
            }
            _onEndNodeMoveAction.Add(action);
            return this;
        }
        public GraphEditAction RemoveOnEndNodeMove(Action action) {
            if (_onEndNodeMoveAction == null || _onEndNodeMoveAction.Count == 0) return this;
            _onEndNodeMoveAction.Remove(action); 
            if (_onEndNodeMoveAction.Count == 0) {
                Disconnect("_end_node_move", this, nameof(ExecuteEndNodeMove));
            }
            return this;
        }
        private void ExecuteEndNodeMove() {
            if (_onEndNodeMoveAction == null || _onEndNodeMoveAction.Count == 0) return;
            for (var i = 0; i < _onEndNodeMoveAction.Count; i++) _onEndNodeMoveAction[i].Invoke();
        }
        

        private List<Action>? _onFocusEnteredAction; 
        public GraphEditAction OnFocusEntered(Action action) {
            if (_onFocusEnteredAction == null || _onFocusEnteredAction.Count == 0) {
                _onFocusEnteredAction ??= new List<Action>(); 
                Connect("focus_entered", this, nameof(ExecuteFocusEntered));
            }
            _onFocusEnteredAction.Add(action);
            return this;
        }
        public GraphEditAction RemoveOnFocusEntered(Action action) {
            if (_onFocusEnteredAction == null || _onFocusEnteredAction.Count == 0) return this;
            _onFocusEnteredAction.Remove(action); 
            if (_onFocusEnteredAction.Count == 0) {
                Disconnect("focus_entered", this, nameof(ExecuteFocusEntered));
            }
            return this;
        }
        private void ExecuteFocusEntered() {
            if (_onFocusEnteredAction == null || _onFocusEnteredAction.Count == 0) return;
            for (var i = 0; i < _onFocusEnteredAction.Count; i++) _onFocusEnteredAction[i].Invoke();
        }
        

        private List<Action>? _onFocusExitedAction; 
        public GraphEditAction OnFocusExited(Action action) {
            if (_onFocusExitedAction == null || _onFocusExitedAction.Count == 0) {
                _onFocusExitedAction ??= new List<Action>(); 
                Connect("focus_exited", this, nameof(ExecuteFocusExited));
            }
            _onFocusExitedAction.Add(action);
            return this;
        }
        public GraphEditAction RemoveOnFocusExited(Action action) {
            if (_onFocusExitedAction == null || _onFocusExitedAction.Count == 0) return this;
            _onFocusExitedAction.Remove(action); 
            if (_onFocusExitedAction.Count == 0) {
                Disconnect("focus_exited", this, nameof(ExecuteFocusExited));
            }
            return this;
        }
        private void ExecuteFocusExited() {
            if (_onFocusExitedAction == null || _onFocusExitedAction.Count == 0) return;
            for (var i = 0; i < _onFocusExitedAction.Count; i++) _onFocusExitedAction[i].Invoke();
        }
        

        private List<Action<InputEvent>>? _onGuiInputAction; 
        public GraphEditAction OnGuiInput(Action<InputEvent> action) {
            if (_onGuiInputAction == null || _onGuiInputAction.Count == 0) {
                _onGuiInputAction ??= new List<Action<InputEvent>>(); 
                Connect("gui_input", this, nameof(ExecuteGuiInput));
            }
            _onGuiInputAction.Add(action);
            return this;
        }
        public GraphEditAction RemoveOnGuiInput(Action<InputEvent> action) {
            if (_onGuiInputAction == null || _onGuiInputAction.Count == 0) return this;
            _onGuiInputAction.Remove(action); 
            if (_onGuiInputAction.Count == 0) {
                Disconnect("gui_input", this, nameof(ExecuteGuiInput));
            }
            return this;
        }
        private void ExecuteGuiInput(InputEvent @event) {
            if (_onGuiInputAction == null || _onGuiInputAction.Count == 0) return;
            for (var i = 0; i < _onGuiInputAction.Count; i++) _onGuiInputAction[i].Invoke(@event);
        }
        

        private List<Action>? _onHideAction; 
        public GraphEditAction OnHide(Action action) {
            if (_onHideAction == null || _onHideAction.Count == 0) {
                _onHideAction ??= new List<Action>(); 
                Connect("hide", this, nameof(ExecuteHide));
            }
            _onHideAction.Add(action);
            return this;
        }
        public GraphEditAction RemoveOnHide(Action action) {
            if (_onHideAction == null || _onHideAction.Count == 0) return this;
            _onHideAction.Remove(action); 
            if (_onHideAction.Count == 0) {
                Disconnect("hide", this, nameof(ExecuteHide));
            }
            return this;
        }
        private void ExecuteHide() {
            if (_onHideAction == null || _onHideAction.Count == 0) return;
            for (var i = 0; i < _onHideAction.Count; i++) _onHideAction[i].Invoke();
        }
        

        private List<Action>? _onItemRectChangedAction; 
        public GraphEditAction OnItemRectChanged(Action action) {
            if (_onItemRectChangedAction == null || _onItemRectChangedAction.Count == 0) {
                _onItemRectChangedAction ??= new List<Action>(); 
                Connect("item_rect_changed", this, nameof(ExecuteItemRectChanged));
            }
            _onItemRectChangedAction.Add(action);
            return this;
        }
        public GraphEditAction RemoveOnItemRectChanged(Action action) {
            if (_onItemRectChangedAction == null || _onItemRectChangedAction.Count == 0) return this;
            _onItemRectChangedAction.Remove(action); 
            if (_onItemRectChangedAction.Count == 0) {
                Disconnect("item_rect_changed", this, nameof(ExecuteItemRectChanged));
            }
            return this;
        }
        private void ExecuteItemRectChanged() {
            if (_onItemRectChangedAction == null || _onItemRectChangedAction.Count == 0) return;
            for (var i = 0; i < _onItemRectChangedAction.Count; i++) _onItemRectChangedAction[i].Invoke();
        }
        

        private List<Action>? _onMinimumSizeChangedAction; 
        public GraphEditAction OnMinimumSizeChanged(Action action) {
            if (_onMinimumSizeChangedAction == null || _onMinimumSizeChangedAction.Count == 0) {
                _onMinimumSizeChangedAction ??= new List<Action>(); 
                Connect("minimum_size_changed", this, nameof(ExecuteMinimumSizeChanged));
            }
            _onMinimumSizeChangedAction.Add(action);
            return this;
        }
        public GraphEditAction RemoveOnMinimumSizeChanged(Action action) {
            if (_onMinimumSizeChangedAction == null || _onMinimumSizeChangedAction.Count == 0) return this;
            _onMinimumSizeChangedAction.Remove(action); 
            if (_onMinimumSizeChangedAction.Count == 0) {
                Disconnect("minimum_size_changed", this, nameof(ExecuteMinimumSizeChanged));
            }
            return this;
        }
        private void ExecuteMinimumSizeChanged() {
            if (_onMinimumSizeChangedAction == null || _onMinimumSizeChangedAction.Count == 0) return;
            for (var i = 0; i < _onMinimumSizeChangedAction.Count; i++) _onMinimumSizeChangedAction[i].Invoke();
        }
        

        private List<Action>? _onModalClosedAction; 
        public GraphEditAction OnModalClosed(Action action) {
            if (_onModalClosedAction == null || _onModalClosedAction.Count == 0) {
                _onModalClosedAction ??= new List<Action>(); 
                Connect("modal_closed", this, nameof(ExecuteModalClosed));
            }
            _onModalClosedAction.Add(action);
            return this;
        }
        public GraphEditAction RemoveOnModalClosed(Action action) {
            if (_onModalClosedAction == null || _onModalClosedAction.Count == 0) return this;
            _onModalClosedAction.Remove(action); 
            if (_onModalClosedAction.Count == 0) {
                Disconnect("modal_closed", this, nameof(ExecuteModalClosed));
            }
            return this;
        }
        private void ExecuteModalClosed() {
            if (_onModalClosedAction == null || _onModalClosedAction.Count == 0) return;
            for (var i = 0; i < _onModalClosedAction.Count; i++) _onModalClosedAction[i].Invoke();
        }
        

        private List<Action>? _onMouseEnteredAction; 
        public GraphEditAction OnMouseEntered(Action action) {
            if (_onMouseEnteredAction == null || _onMouseEnteredAction.Count == 0) {
                _onMouseEnteredAction ??= new List<Action>(); 
                Connect("mouse_entered", this, nameof(ExecuteMouseEntered));
            }
            _onMouseEnteredAction.Add(action);
            return this;
        }
        public GraphEditAction RemoveOnMouseEntered(Action action) {
            if (_onMouseEnteredAction == null || _onMouseEnteredAction.Count == 0) return this;
            _onMouseEnteredAction.Remove(action); 
            if (_onMouseEnteredAction.Count == 0) {
                Disconnect("mouse_entered", this, nameof(ExecuteMouseEntered));
            }
            return this;
        }
        private void ExecuteMouseEntered() {
            if (_onMouseEnteredAction == null || _onMouseEnteredAction.Count == 0) return;
            for (var i = 0; i < _onMouseEnteredAction.Count; i++) _onMouseEnteredAction[i].Invoke();
        }
        

        private List<Action>? _onMouseExitedAction; 
        public GraphEditAction OnMouseExited(Action action) {
            if (_onMouseExitedAction == null || _onMouseExitedAction.Count == 0) {
                _onMouseExitedAction ??= new List<Action>(); 
                Connect("mouse_exited", this, nameof(ExecuteMouseExited));
            }
            _onMouseExitedAction.Add(action);
            return this;
        }
        public GraphEditAction RemoveOnMouseExited(Action action) {
            if (_onMouseExitedAction == null || _onMouseExitedAction.Count == 0) return this;
            _onMouseExitedAction.Remove(action); 
            if (_onMouseExitedAction.Count == 0) {
                Disconnect("mouse_exited", this, nameof(ExecuteMouseExited));
            }
            return this;
        }
        private void ExecuteMouseExited() {
            if (_onMouseExitedAction == null || _onMouseExitedAction.Count == 0) return;
            for (var i = 0; i < _onMouseExitedAction.Count; i++) _onMouseExitedAction[i].Invoke();
        }
        

        private List<Action<Node>>? _onNodeSelectedAction; 
        public GraphEditAction OnNodeSelected(Action<Node> action) {
            if (_onNodeSelectedAction == null || _onNodeSelectedAction.Count == 0) {
                _onNodeSelectedAction ??= new List<Action<Node>>(); 
                Connect("node_selected", this, nameof(ExecuteNodeSelected));
            }
            _onNodeSelectedAction.Add(action);
            return this;
        }
        public GraphEditAction RemoveOnNodeSelected(Action<Node> action) {
            if (_onNodeSelectedAction == null || _onNodeSelectedAction.Count == 0) return this;
            _onNodeSelectedAction.Remove(action); 
            if (_onNodeSelectedAction.Count == 0) {
                Disconnect("node_selected", this, nameof(ExecuteNodeSelected));
            }
            return this;
        }
        private void ExecuteNodeSelected(Node node) {
            if (_onNodeSelectedAction == null || _onNodeSelectedAction.Count == 0) return;
            for (var i = 0; i < _onNodeSelectedAction.Count; i++) _onNodeSelectedAction[i].Invoke(node);
        }
        

        private List<Action<Node>>? _onNodeUnselectedAction; 
        public GraphEditAction OnNodeUnselected(Action<Node> action) {
            if (_onNodeUnselectedAction == null || _onNodeUnselectedAction.Count == 0) {
                _onNodeUnselectedAction ??= new List<Action<Node>>(); 
                Connect("node_unselected", this, nameof(ExecuteNodeUnselected));
            }
            _onNodeUnselectedAction.Add(action);
            return this;
        }
        public GraphEditAction RemoveOnNodeUnselected(Action<Node> action) {
            if (_onNodeUnselectedAction == null || _onNodeUnselectedAction.Count == 0) return this;
            _onNodeUnselectedAction.Remove(action); 
            if (_onNodeUnselectedAction.Count == 0) {
                Disconnect("node_unselected", this, nameof(ExecuteNodeUnselected));
            }
            return this;
        }
        private void ExecuteNodeUnselected(Node node) {
            if (_onNodeUnselectedAction == null || _onNodeUnselectedAction.Count == 0) return;
            for (var i = 0; i < _onNodeUnselectedAction.Count; i++) _onNodeUnselectedAction[i].Invoke(node);
        }
        

        private List<Action>? _onPasteNodesRequestAction; 
        public GraphEditAction OnPasteNodesRequest(Action action) {
            if (_onPasteNodesRequestAction == null || _onPasteNodesRequestAction.Count == 0) {
                _onPasteNodesRequestAction ??= new List<Action>(); 
                Connect("paste_nodes_request", this, nameof(ExecutePasteNodesRequest));
            }
            _onPasteNodesRequestAction.Add(action);
            return this;
        }
        public GraphEditAction RemoveOnPasteNodesRequest(Action action) {
            if (_onPasteNodesRequestAction == null || _onPasteNodesRequestAction.Count == 0) return this;
            _onPasteNodesRequestAction.Remove(action); 
            if (_onPasteNodesRequestAction.Count == 0) {
                Disconnect("paste_nodes_request", this, nameof(ExecutePasteNodesRequest));
            }
            return this;
        }
        private void ExecutePasteNodesRequest() {
            if (_onPasteNodesRequestAction == null || _onPasteNodesRequestAction.Count == 0) return;
            for (var i = 0; i < _onPasteNodesRequestAction.Count; i++) _onPasteNodesRequestAction[i].Invoke();
        }
        

        private List<Action<Vector2>>? _onPopupRequestAction; 
        public GraphEditAction OnPopupRequest(Action<Vector2> action) {
            if (_onPopupRequestAction == null || _onPopupRequestAction.Count == 0) {
                _onPopupRequestAction ??= new List<Action<Vector2>>(); 
                Connect("popup_request", this, nameof(ExecutePopupRequest));
            }
            _onPopupRequestAction.Add(action);
            return this;
        }
        public GraphEditAction RemoveOnPopupRequest(Action<Vector2> action) {
            if (_onPopupRequestAction == null || _onPopupRequestAction.Count == 0) return this;
            _onPopupRequestAction.Remove(action); 
            if (_onPopupRequestAction.Count == 0) {
                Disconnect("popup_request", this, nameof(ExecutePopupRequest));
            }
            return this;
        }
        private void ExecutePopupRequest(Vector2 position) {
            if (_onPopupRequestAction == null || _onPopupRequestAction.Count == 0) return;
            for (var i = 0; i < _onPopupRequestAction.Count; i++) _onPopupRequestAction[i].Invoke(position);
        }
        

        private List<Action>? _onReadyAction; 
        public GraphEditAction OnReady(Action action) {
            if (_onReadyAction == null || _onReadyAction.Count == 0) {
                _onReadyAction ??= new List<Action>(); 
                Connect("ready", this, nameof(ExecuteReady));
            }
            _onReadyAction.Add(action);
            return this;
        }
        public GraphEditAction RemoveOnReady(Action action) {
            if (_onReadyAction == null || _onReadyAction.Count == 0) return this;
            _onReadyAction.Remove(action); 
            if (_onReadyAction.Count == 0) {
                Disconnect("ready", this, nameof(ExecuteReady));
            }
            return this;
        }
        private void ExecuteReady() {
            if (_onReadyAction == null || _onReadyAction.Count == 0) return;
            for (var i = 0; i < _onReadyAction.Count; i++) _onReadyAction[i].Invoke();
        }
        

        private List<Action>? _onRenamedAction; 
        public GraphEditAction OnRenamed(Action action) {
            if (_onRenamedAction == null || _onRenamedAction.Count == 0) {
                _onRenamedAction ??= new List<Action>(); 
                Connect("renamed", this, nameof(ExecuteRenamed));
            }
            _onRenamedAction.Add(action);
            return this;
        }
        public GraphEditAction RemoveOnRenamed(Action action) {
            if (_onRenamedAction == null || _onRenamedAction.Count == 0) return this;
            _onRenamedAction.Remove(action); 
            if (_onRenamedAction.Count == 0) {
                Disconnect("renamed", this, nameof(ExecuteRenamed));
            }
            return this;
        }
        private void ExecuteRenamed() {
            if (_onRenamedAction == null || _onRenamedAction.Count == 0) return;
            for (var i = 0; i < _onRenamedAction.Count; i++) _onRenamedAction[i].Invoke();
        }
        

        private List<Action>? _onResizedAction; 
        public GraphEditAction OnResized(Action action) {
            if (_onResizedAction == null || _onResizedAction.Count == 0) {
                _onResizedAction ??= new List<Action>(); 
                Connect("resized", this, nameof(ExecuteResized));
            }
            _onResizedAction.Add(action);
            return this;
        }
        public GraphEditAction RemoveOnResized(Action action) {
            if (_onResizedAction == null || _onResizedAction.Count == 0) return this;
            _onResizedAction.Remove(action); 
            if (_onResizedAction.Count == 0) {
                Disconnect("resized", this, nameof(ExecuteResized));
            }
            return this;
        }
        private void ExecuteResized() {
            if (_onResizedAction == null || _onResizedAction.Count == 0) return;
            for (var i = 0; i < _onResizedAction.Count; i++) _onResizedAction[i].Invoke();
        }
        

        private List<Action>? _onScriptChangedAction; 
        public GraphEditAction OnScriptChanged(Action action) {
            if (_onScriptChangedAction == null || _onScriptChangedAction.Count == 0) {
                _onScriptChangedAction ??= new List<Action>(); 
                Connect("script_changed", this, nameof(ExecuteScriptChanged));
            }
            _onScriptChangedAction.Add(action);
            return this;
        }
        public GraphEditAction RemoveOnScriptChanged(Action action) {
            if (_onScriptChangedAction == null || _onScriptChangedAction.Count == 0) return this;
            _onScriptChangedAction.Remove(action); 
            if (_onScriptChangedAction.Count == 0) {
                Disconnect("script_changed", this, nameof(ExecuteScriptChanged));
            }
            return this;
        }
        private void ExecuteScriptChanged() {
            if (_onScriptChangedAction == null || _onScriptChangedAction.Count == 0) return;
            for (var i = 0; i < _onScriptChangedAction.Count; i++) _onScriptChangedAction[i].Invoke();
        }
        

        private List<Action<Vector2>>? _onScrollOffsetChangedAction; 
        public GraphEditAction OnScrollOffsetChanged(Action<Vector2> action) {
            if (_onScrollOffsetChangedAction == null || _onScrollOffsetChangedAction.Count == 0) {
                _onScrollOffsetChangedAction ??= new List<Action<Vector2>>(); 
                Connect("scroll_offset_changed", this, nameof(ExecuteScrollOffsetChanged));
            }
            _onScrollOffsetChangedAction.Add(action);
            return this;
        }
        public GraphEditAction RemoveOnScrollOffsetChanged(Action<Vector2> action) {
            if (_onScrollOffsetChangedAction == null || _onScrollOffsetChangedAction.Count == 0) return this;
            _onScrollOffsetChangedAction.Remove(action); 
            if (_onScrollOffsetChangedAction.Count == 0) {
                Disconnect("scroll_offset_changed", this, nameof(ExecuteScrollOffsetChanged));
            }
            return this;
        }
        private void ExecuteScrollOffsetChanged(Vector2 ofs) {
            if (_onScrollOffsetChangedAction == null || _onScrollOffsetChangedAction.Count == 0) return;
            for (var i = 0; i < _onScrollOffsetChangedAction.Count; i++) _onScrollOffsetChangedAction[i].Invoke(ofs);
        }
        

        private List<Action>? _onSizeFlagsChangedAction; 
        public GraphEditAction OnSizeFlagsChanged(Action action) {
            if (_onSizeFlagsChangedAction == null || _onSizeFlagsChangedAction.Count == 0) {
                _onSizeFlagsChangedAction ??= new List<Action>(); 
                Connect("size_flags_changed", this, nameof(ExecuteSizeFlagsChanged));
            }
            _onSizeFlagsChangedAction.Add(action);
            return this;
        }
        public GraphEditAction RemoveOnSizeFlagsChanged(Action action) {
            if (_onSizeFlagsChangedAction == null || _onSizeFlagsChangedAction.Count == 0) return this;
            _onSizeFlagsChangedAction.Remove(action); 
            if (_onSizeFlagsChangedAction.Count == 0) {
                Disconnect("size_flags_changed", this, nameof(ExecuteSizeFlagsChanged));
            }
            return this;
        }
        private void ExecuteSizeFlagsChanged() {
            if (_onSizeFlagsChangedAction == null || _onSizeFlagsChangedAction.Count == 0) return;
            for (var i = 0; i < _onSizeFlagsChangedAction.Count; i++) _onSizeFlagsChangedAction[i].Invoke();
        }
        

        private List<Action>? _onTreeEnteredAction; 
        public GraphEditAction OnTreeEntered(Action action) {
            if (_onTreeEnteredAction == null || _onTreeEnteredAction.Count == 0) {
                _onTreeEnteredAction ??= new List<Action>(); 
                Connect("tree_entered", this, nameof(ExecuteTreeEntered));
            }
            _onTreeEnteredAction.Add(action);
            return this;
        }
        public GraphEditAction RemoveOnTreeEntered(Action action) {
            if (_onTreeEnteredAction == null || _onTreeEnteredAction.Count == 0) return this;
            _onTreeEnteredAction.Remove(action); 
            if (_onTreeEnteredAction.Count == 0) {
                Disconnect("tree_entered", this, nameof(ExecuteTreeEntered));
            }
            return this;
        }
        private void ExecuteTreeEntered() {
            if (_onTreeEnteredAction == null || _onTreeEnteredAction.Count == 0) return;
            for (var i = 0; i < _onTreeEnteredAction.Count; i++) _onTreeEnteredAction[i].Invoke();
        }
        

        private List<Action>? _onTreeExitedAction; 
        public GraphEditAction OnTreeExited(Action action) {
            if (_onTreeExitedAction == null || _onTreeExitedAction.Count == 0) {
                _onTreeExitedAction ??= new List<Action>(); 
                Connect("tree_exited", this, nameof(ExecuteTreeExited));
            }
            _onTreeExitedAction.Add(action);
            return this;
        }
        public GraphEditAction RemoveOnTreeExited(Action action) {
            if (_onTreeExitedAction == null || _onTreeExitedAction.Count == 0) return this;
            _onTreeExitedAction.Remove(action); 
            if (_onTreeExitedAction.Count == 0) {
                Disconnect("tree_exited", this, nameof(ExecuteTreeExited));
            }
            return this;
        }
        private void ExecuteTreeExited() {
            if (_onTreeExitedAction == null || _onTreeExitedAction.Count == 0) return;
            for (var i = 0; i < _onTreeExitedAction.Count; i++) _onTreeExitedAction[i].Invoke();
        }
        

        private List<Action>? _onTreeExitingAction; 
        public GraphEditAction OnTreeExiting(Action action) {
            if (_onTreeExitingAction == null || _onTreeExitingAction.Count == 0) {
                _onTreeExitingAction ??= new List<Action>(); 
                Connect("tree_exiting", this, nameof(ExecuteTreeExiting));
            }
            _onTreeExitingAction.Add(action);
            return this;
        }
        public GraphEditAction RemoveOnTreeExiting(Action action) {
            if (_onTreeExitingAction == null || _onTreeExitingAction.Count == 0) return this;
            _onTreeExitingAction.Remove(action); 
            if (_onTreeExitingAction.Count == 0) {
                Disconnect("tree_exiting", this, nameof(ExecuteTreeExiting));
            }
            return this;
        }
        private void ExecuteTreeExiting() {
            if (_onTreeExitingAction == null || _onTreeExitingAction.Count == 0) return;
            for (var i = 0; i < _onTreeExitingAction.Count; i++) _onTreeExitingAction[i].Invoke();
        }
        

        private List<Action>? _onVisibilityChangedAction; 
        public GraphEditAction OnVisibilityChanged(Action action) {
            if (_onVisibilityChangedAction == null || _onVisibilityChangedAction.Count == 0) {
                _onVisibilityChangedAction ??= new List<Action>(); 
                Connect("visibility_changed", this, nameof(ExecuteVisibilityChanged));
            }
            _onVisibilityChangedAction.Add(action);
            return this;
        }
        public GraphEditAction RemoveOnVisibilityChanged(Action action) {
            if (_onVisibilityChangedAction == null || _onVisibilityChangedAction.Count == 0) return this;
            _onVisibilityChangedAction.Remove(action); 
            if (_onVisibilityChangedAction.Count == 0) {
                Disconnect("visibility_changed", this, nameof(ExecuteVisibilityChanged));
            }
            return this;
        }
        private void ExecuteVisibilityChanged() {
            if (_onVisibilityChangedAction == null || _onVisibilityChangedAction.Count == 0) return;
            for (var i = 0; i < _onVisibilityChangedAction.Count; i++) _onVisibilityChangedAction[i].Invoke();
        }
        
    }
}