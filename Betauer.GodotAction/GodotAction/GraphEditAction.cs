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
            if (_onProcessActions == null) {
                SetProcess(false);
                return;
            }
            for (var i = 0; i < _onProcessActions.Count; i++) _onProcessActions[i].Invoke(delta);
        }

        public override void _PhysicsProcess(float delta) {
            if (_onPhysicsProcessActions == null) {
                SetPhysicsProcess(true);
                return;
            }
            for (var i = 0; i < _onPhysicsProcessActions.Count; i++) _onPhysicsProcessActions[i].Invoke(delta);
        }

        public override void _Input(InputEvent @event) {
            if (_onInputActions == null) {
                SetProcessInput(true);
                return;
            }
            for (var i = 0; i < _onInputActions.Count; i++) _onInputActions[i].Invoke(@event);
        }

        public override void _UnhandledInput(InputEvent @event) {
            if (_onUnhandledInputActions == null) {
                SetProcessUnhandledInput(true);
                return;
            }
            for (var i = 0; i < _onUnhandledInputActions.Count; i++) _onUnhandledInputActions[i].Invoke(@event);
        }

        public override void _UnhandledKeyInput(InputEventKey @event) {
            if (_onUnhandledKeyInputActions == null) {
                SetProcessUnhandledKeyInput(true);
                return;
            }
            for (var i = 0; i < _onUnhandledKeyInputActions.Count; i++) _onUnhandledKeyInputActions[i].Invoke(@event);
        }

        private Action? _onBeginNodeMoveAction; 
        public GraphEditAction OnBeginNodeMove(Action action) {
            if (_onBeginNodeMoveAction == null) 
                Connect("_begin_node_move", this, nameof(ExecuteBeginNodeMove));
            _onBeginNodeMoveAction = action;
            return this;
        }
        public GraphEditAction RemoveOnBeginNodeMove() {
            if (_onBeginNodeMoveAction == null) return this; 
            Disconnect("_begin_node_move", this, nameof(ExecuteBeginNodeMove));
            _onBeginNodeMoveAction = null;
            return this;
        }
        private void ExecuteBeginNodeMove() =>
            _onBeginNodeMoveAction?.Invoke();
        

        private Action<Vector2, string, int>? _onConnectionFromEmptyAction; 
        public GraphEditAction OnConnectionFromEmpty(Action<Vector2, string, int> action) {
            if (_onConnectionFromEmptyAction == null) 
                Connect("connection_from_empty", this, nameof(ExecuteConnectionFromEmpty));
            _onConnectionFromEmptyAction = action;
            return this;
        }
        public GraphEditAction RemoveOnConnectionFromEmpty() {
            if (_onConnectionFromEmptyAction == null) return this; 
            Disconnect("connection_from_empty", this, nameof(ExecuteConnectionFromEmpty));
            _onConnectionFromEmptyAction = null;
            return this;
        }
        private void ExecuteConnectionFromEmpty(Vector2 release_position, string to, int to_slot) =>
            _onConnectionFromEmptyAction?.Invoke(release_position, to, to_slot);
        

        private Action<string, int, string, int>? _onConnectionRequestAction; 
        public GraphEditAction OnConnectionRequest(Action<string, int, string, int> action) {
            if (_onConnectionRequestAction == null) 
                Connect("connection_request", this, nameof(ExecuteConnectionRequest));
            _onConnectionRequestAction = action;
            return this;
        }
        public GraphEditAction RemoveOnConnectionRequest() {
            if (_onConnectionRequestAction == null) return this; 
            Disconnect("connection_request", this, nameof(ExecuteConnectionRequest));
            _onConnectionRequestAction = null;
            return this;
        }
        private void ExecuteConnectionRequest(string from, int from_slot, string to, int to_slot) =>
            _onConnectionRequestAction?.Invoke(from, from_slot, to, to_slot);
        

        private Action<string, int, Vector2>? _onConnectionToEmptyAction; 
        public GraphEditAction OnConnectionToEmpty(Action<string, int, Vector2> action) {
            if (_onConnectionToEmptyAction == null) 
                Connect("connection_to_empty", this, nameof(ExecuteConnectionToEmpty));
            _onConnectionToEmptyAction = action;
            return this;
        }
        public GraphEditAction RemoveOnConnectionToEmpty() {
            if (_onConnectionToEmptyAction == null) return this; 
            Disconnect("connection_to_empty", this, nameof(ExecuteConnectionToEmpty));
            _onConnectionToEmptyAction = null;
            return this;
        }
        private void ExecuteConnectionToEmpty(string from, int from_slot, Vector2 release_position) =>
            _onConnectionToEmptyAction?.Invoke(from, from_slot, release_position);
        

        private Action? _onCopyNodesRequestAction; 
        public GraphEditAction OnCopyNodesRequest(Action action) {
            if (_onCopyNodesRequestAction == null) 
                Connect("copy_nodes_request", this, nameof(ExecuteCopyNodesRequest));
            _onCopyNodesRequestAction = action;
            return this;
        }
        public GraphEditAction RemoveOnCopyNodesRequest() {
            if (_onCopyNodesRequestAction == null) return this; 
            Disconnect("copy_nodes_request", this, nameof(ExecuteCopyNodesRequest));
            _onCopyNodesRequestAction = null;
            return this;
        }
        private void ExecuteCopyNodesRequest() =>
            _onCopyNodesRequestAction?.Invoke();
        

        private Action? _onDeleteNodesRequestAction; 
        public GraphEditAction OnDeleteNodesRequest(Action action) {
            if (_onDeleteNodesRequestAction == null) 
                Connect("delete_nodes_request", this, nameof(ExecuteDeleteNodesRequest));
            _onDeleteNodesRequestAction = action;
            return this;
        }
        public GraphEditAction RemoveOnDeleteNodesRequest() {
            if (_onDeleteNodesRequestAction == null) return this; 
            Disconnect("delete_nodes_request", this, nameof(ExecuteDeleteNodesRequest));
            _onDeleteNodesRequestAction = null;
            return this;
        }
        private void ExecuteDeleteNodesRequest() =>
            _onDeleteNodesRequestAction?.Invoke();
        

        private Action<string, int, string, int>? _onDisconnectionRequestAction; 
        public GraphEditAction OnDisconnectionRequest(Action<string, int, string, int> action) {
            if (_onDisconnectionRequestAction == null) 
                Connect("disconnection_request", this, nameof(ExecuteDisconnectionRequest));
            _onDisconnectionRequestAction = action;
            return this;
        }
        public GraphEditAction RemoveOnDisconnectionRequest() {
            if (_onDisconnectionRequestAction == null) return this; 
            Disconnect("disconnection_request", this, nameof(ExecuteDisconnectionRequest));
            _onDisconnectionRequestAction = null;
            return this;
        }
        private void ExecuteDisconnectionRequest(string from, int from_slot, string to, int to_slot) =>
            _onDisconnectionRequestAction?.Invoke(from, from_slot, to, to_slot);
        

        private Action? _onDrawAction; 
        public GraphEditAction OnDraw(Action action) {
            if (_onDrawAction == null) 
                Connect("draw", this, nameof(ExecuteDraw));
            _onDrawAction = action;
            return this;
        }
        public GraphEditAction RemoveOnDraw() {
            if (_onDrawAction == null) return this; 
            Disconnect("draw", this, nameof(ExecuteDraw));
            _onDrawAction = null;
            return this;
        }
        private void ExecuteDraw() =>
            _onDrawAction?.Invoke();
        

        private Action? _onDuplicateNodesRequestAction; 
        public GraphEditAction OnDuplicateNodesRequest(Action action) {
            if (_onDuplicateNodesRequestAction == null) 
                Connect("duplicate_nodes_request", this, nameof(ExecuteDuplicateNodesRequest));
            _onDuplicateNodesRequestAction = action;
            return this;
        }
        public GraphEditAction RemoveOnDuplicateNodesRequest() {
            if (_onDuplicateNodesRequestAction == null) return this; 
            Disconnect("duplicate_nodes_request", this, nameof(ExecuteDuplicateNodesRequest));
            _onDuplicateNodesRequestAction = null;
            return this;
        }
        private void ExecuteDuplicateNodesRequest() =>
            _onDuplicateNodesRequestAction?.Invoke();
        

        private Action? _onEndNodeMoveAction; 
        public GraphEditAction OnEndNodeMove(Action action) {
            if (_onEndNodeMoveAction == null) 
                Connect("_end_node_move", this, nameof(ExecuteEndNodeMove));
            _onEndNodeMoveAction = action;
            return this;
        }
        public GraphEditAction RemoveOnEndNodeMove() {
            if (_onEndNodeMoveAction == null) return this; 
            Disconnect("_end_node_move", this, nameof(ExecuteEndNodeMove));
            _onEndNodeMoveAction = null;
            return this;
        }
        private void ExecuteEndNodeMove() =>
            _onEndNodeMoveAction?.Invoke();
        

        private Action? _onFocusEnteredAction; 
        public GraphEditAction OnFocusEntered(Action action) {
            if (_onFocusEnteredAction == null) 
                Connect("focus_entered", this, nameof(ExecuteFocusEntered));
            _onFocusEnteredAction = action;
            return this;
        }
        public GraphEditAction RemoveOnFocusEntered() {
            if (_onFocusEnteredAction == null) return this; 
            Disconnect("focus_entered", this, nameof(ExecuteFocusEntered));
            _onFocusEnteredAction = null;
            return this;
        }
        private void ExecuteFocusEntered() =>
            _onFocusEnteredAction?.Invoke();
        

        private Action? _onFocusExitedAction; 
        public GraphEditAction OnFocusExited(Action action) {
            if (_onFocusExitedAction == null) 
                Connect("focus_exited", this, nameof(ExecuteFocusExited));
            _onFocusExitedAction = action;
            return this;
        }
        public GraphEditAction RemoveOnFocusExited() {
            if (_onFocusExitedAction == null) return this; 
            Disconnect("focus_exited", this, nameof(ExecuteFocusExited));
            _onFocusExitedAction = null;
            return this;
        }
        private void ExecuteFocusExited() =>
            _onFocusExitedAction?.Invoke();
        

        private Action<InputEvent>? _onGuiInputAction; 
        public GraphEditAction OnGuiInput(Action<InputEvent> action) {
            if (_onGuiInputAction == null) 
                Connect("gui_input", this, nameof(ExecuteGuiInput));
            _onGuiInputAction = action;
            return this;
        }
        public GraphEditAction RemoveOnGuiInput() {
            if (_onGuiInputAction == null) return this; 
            Disconnect("gui_input", this, nameof(ExecuteGuiInput));
            _onGuiInputAction = null;
            return this;
        }
        private void ExecuteGuiInput(InputEvent @event) =>
            _onGuiInputAction?.Invoke(@event);
        

        private Action? _onHideAction; 
        public GraphEditAction OnHide(Action action) {
            if (_onHideAction == null) 
                Connect("hide", this, nameof(ExecuteHide));
            _onHideAction = action;
            return this;
        }
        public GraphEditAction RemoveOnHide() {
            if (_onHideAction == null) return this; 
            Disconnect("hide", this, nameof(ExecuteHide));
            _onHideAction = null;
            return this;
        }
        private void ExecuteHide() =>
            _onHideAction?.Invoke();
        

        private Action? _onItemRectChangedAction; 
        public GraphEditAction OnItemRectChanged(Action action) {
            if (_onItemRectChangedAction == null) 
                Connect("item_rect_changed", this, nameof(ExecuteItemRectChanged));
            _onItemRectChangedAction = action;
            return this;
        }
        public GraphEditAction RemoveOnItemRectChanged() {
            if (_onItemRectChangedAction == null) return this; 
            Disconnect("item_rect_changed", this, nameof(ExecuteItemRectChanged));
            _onItemRectChangedAction = null;
            return this;
        }
        private void ExecuteItemRectChanged() =>
            _onItemRectChangedAction?.Invoke();
        

        private Action? _onMinimumSizeChangedAction; 
        public GraphEditAction OnMinimumSizeChanged(Action action) {
            if (_onMinimumSizeChangedAction == null) 
                Connect("minimum_size_changed", this, nameof(ExecuteMinimumSizeChanged));
            _onMinimumSizeChangedAction = action;
            return this;
        }
        public GraphEditAction RemoveOnMinimumSizeChanged() {
            if (_onMinimumSizeChangedAction == null) return this; 
            Disconnect("minimum_size_changed", this, nameof(ExecuteMinimumSizeChanged));
            _onMinimumSizeChangedAction = null;
            return this;
        }
        private void ExecuteMinimumSizeChanged() =>
            _onMinimumSizeChangedAction?.Invoke();
        

        private Action? _onModalClosedAction; 
        public GraphEditAction OnModalClosed(Action action) {
            if (_onModalClosedAction == null) 
                Connect("modal_closed", this, nameof(ExecuteModalClosed));
            _onModalClosedAction = action;
            return this;
        }
        public GraphEditAction RemoveOnModalClosed() {
            if (_onModalClosedAction == null) return this; 
            Disconnect("modal_closed", this, nameof(ExecuteModalClosed));
            _onModalClosedAction = null;
            return this;
        }
        private void ExecuteModalClosed() =>
            _onModalClosedAction?.Invoke();
        

        private Action? _onMouseEnteredAction; 
        public GraphEditAction OnMouseEntered(Action action) {
            if (_onMouseEnteredAction == null) 
                Connect("mouse_entered", this, nameof(ExecuteMouseEntered));
            _onMouseEnteredAction = action;
            return this;
        }
        public GraphEditAction RemoveOnMouseEntered() {
            if (_onMouseEnteredAction == null) return this; 
            Disconnect("mouse_entered", this, nameof(ExecuteMouseEntered));
            _onMouseEnteredAction = null;
            return this;
        }
        private void ExecuteMouseEntered() =>
            _onMouseEnteredAction?.Invoke();
        

        private Action? _onMouseExitedAction; 
        public GraphEditAction OnMouseExited(Action action) {
            if (_onMouseExitedAction == null) 
                Connect("mouse_exited", this, nameof(ExecuteMouseExited));
            _onMouseExitedAction = action;
            return this;
        }
        public GraphEditAction RemoveOnMouseExited() {
            if (_onMouseExitedAction == null) return this; 
            Disconnect("mouse_exited", this, nameof(ExecuteMouseExited));
            _onMouseExitedAction = null;
            return this;
        }
        private void ExecuteMouseExited() =>
            _onMouseExitedAction?.Invoke();
        

        private Action<Node>? _onNodeSelectedAction; 
        public GraphEditAction OnNodeSelected(Action<Node> action) {
            if (_onNodeSelectedAction == null) 
                Connect("node_selected", this, nameof(ExecuteNodeSelected));
            _onNodeSelectedAction = action;
            return this;
        }
        public GraphEditAction RemoveOnNodeSelected() {
            if (_onNodeSelectedAction == null) return this; 
            Disconnect("node_selected", this, nameof(ExecuteNodeSelected));
            _onNodeSelectedAction = null;
            return this;
        }
        private void ExecuteNodeSelected(Node node) =>
            _onNodeSelectedAction?.Invoke(node);
        

        private Action<Node>? _onNodeUnselectedAction; 
        public GraphEditAction OnNodeUnselected(Action<Node> action) {
            if (_onNodeUnselectedAction == null) 
                Connect("node_unselected", this, nameof(ExecuteNodeUnselected));
            _onNodeUnselectedAction = action;
            return this;
        }
        public GraphEditAction RemoveOnNodeUnselected() {
            if (_onNodeUnselectedAction == null) return this; 
            Disconnect("node_unselected", this, nameof(ExecuteNodeUnselected));
            _onNodeUnselectedAction = null;
            return this;
        }
        private void ExecuteNodeUnselected(Node node) =>
            _onNodeUnselectedAction?.Invoke(node);
        

        private Action? _onPasteNodesRequestAction; 
        public GraphEditAction OnPasteNodesRequest(Action action) {
            if (_onPasteNodesRequestAction == null) 
                Connect("paste_nodes_request", this, nameof(ExecutePasteNodesRequest));
            _onPasteNodesRequestAction = action;
            return this;
        }
        public GraphEditAction RemoveOnPasteNodesRequest() {
            if (_onPasteNodesRequestAction == null) return this; 
            Disconnect("paste_nodes_request", this, nameof(ExecutePasteNodesRequest));
            _onPasteNodesRequestAction = null;
            return this;
        }
        private void ExecutePasteNodesRequest() =>
            _onPasteNodesRequestAction?.Invoke();
        

        private Action<Vector2>? _onPopupRequestAction; 
        public GraphEditAction OnPopupRequest(Action<Vector2> action) {
            if (_onPopupRequestAction == null) 
                Connect("popup_request", this, nameof(ExecutePopupRequest));
            _onPopupRequestAction = action;
            return this;
        }
        public GraphEditAction RemoveOnPopupRequest() {
            if (_onPopupRequestAction == null) return this; 
            Disconnect("popup_request", this, nameof(ExecutePopupRequest));
            _onPopupRequestAction = null;
            return this;
        }
        private void ExecutePopupRequest(Vector2 position) =>
            _onPopupRequestAction?.Invoke(position);
        

        private Action? _onReadyAction; 
        public GraphEditAction OnReady(Action action) {
            if (_onReadyAction == null) 
                Connect("ready", this, nameof(ExecuteReady));
            _onReadyAction = action;
            return this;
        }
        public GraphEditAction RemoveOnReady() {
            if (_onReadyAction == null) return this; 
            Disconnect("ready", this, nameof(ExecuteReady));
            _onReadyAction = null;
            return this;
        }
        private void ExecuteReady() =>
            _onReadyAction?.Invoke();
        

        private Action? _onRenamedAction; 
        public GraphEditAction OnRenamed(Action action) {
            if (_onRenamedAction == null) 
                Connect("renamed", this, nameof(ExecuteRenamed));
            _onRenamedAction = action;
            return this;
        }
        public GraphEditAction RemoveOnRenamed() {
            if (_onRenamedAction == null) return this; 
            Disconnect("renamed", this, nameof(ExecuteRenamed));
            _onRenamedAction = null;
            return this;
        }
        private void ExecuteRenamed() =>
            _onRenamedAction?.Invoke();
        

        private Action? _onResizedAction; 
        public GraphEditAction OnResized(Action action) {
            if (_onResizedAction == null) 
                Connect("resized", this, nameof(ExecuteResized));
            _onResizedAction = action;
            return this;
        }
        public GraphEditAction RemoveOnResized() {
            if (_onResizedAction == null) return this; 
            Disconnect("resized", this, nameof(ExecuteResized));
            _onResizedAction = null;
            return this;
        }
        private void ExecuteResized() =>
            _onResizedAction?.Invoke();
        

        private Action? _onScriptChangedAction; 
        public GraphEditAction OnScriptChanged(Action action) {
            if (_onScriptChangedAction == null) 
                Connect("script_changed", this, nameof(ExecuteScriptChanged));
            _onScriptChangedAction = action;
            return this;
        }
        public GraphEditAction RemoveOnScriptChanged() {
            if (_onScriptChangedAction == null) return this; 
            Disconnect("script_changed", this, nameof(ExecuteScriptChanged));
            _onScriptChangedAction = null;
            return this;
        }
        private void ExecuteScriptChanged() =>
            _onScriptChangedAction?.Invoke();
        

        private Action<Vector2>? _onScrollOffsetChangedAction; 
        public GraphEditAction OnScrollOffsetChanged(Action<Vector2> action) {
            if (_onScrollOffsetChangedAction == null) 
                Connect("scroll_offset_changed", this, nameof(ExecuteScrollOffsetChanged));
            _onScrollOffsetChangedAction = action;
            return this;
        }
        public GraphEditAction RemoveOnScrollOffsetChanged() {
            if (_onScrollOffsetChangedAction == null) return this; 
            Disconnect("scroll_offset_changed", this, nameof(ExecuteScrollOffsetChanged));
            _onScrollOffsetChangedAction = null;
            return this;
        }
        private void ExecuteScrollOffsetChanged(Vector2 ofs) =>
            _onScrollOffsetChangedAction?.Invoke(ofs);
        

        private Action? _onSizeFlagsChangedAction; 
        public GraphEditAction OnSizeFlagsChanged(Action action) {
            if (_onSizeFlagsChangedAction == null) 
                Connect("size_flags_changed", this, nameof(ExecuteSizeFlagsChanged));
            _onSizeFlagsChangedAction = action;
            return this;
        }
        public GraphEditAction RemoveOnSizeFlagsChanged() {
            if (_onSizeFlagsChangedAction == null) return this; 
            Disconnect("size_flags_changed", this, nameof(ExecuteSizeFlagsChanged));
            _onSizeFlagsChangedAction = null;
            return this;
        }
        private void ExecuteSizeFlagsChanged() =>
            _onSizeFlagsChangedAction?.Invoke();
        

        private Action? _onTreeEnteredAction; 
        public GraphEditAction OnTreeEntered(Action action) {
            if (_onTreeEnteredAction == null) 
                Connect("tree_entered", this, nameof(ExecuteTreeEntered));
            _onTreeEnteredAction = action;
            return this;
        }
        public GraphEditAction RemoveOnTreeEntered() {
            if (_onTreeEnteredAction == null) return this; 
            Disconnect("tree_entered", this, nameof(ExecuteTreeEntered));
            _onTreeEnteredAction = null;
            return this;
        }
        private void ExecuteTreeEntered() =>
            _onTreeEnteredAction?.Invoke();
        

        private Action? _onTreeExitedAction; 
        public GraphEditAction OnTreeExited(Action action) {
            if (_onTreeExitedAction == null) 
                Connect("tree_exited", this, nameof(ExecuteTreeExited));
            _onTreeExitedAction = action;
            return this;
        }
        public GraphEditAction RemoveOnTreeExited() {
            if (_onTreeExitedAction == null) return this; 
            Disconnect("tree_exited", this, nameof(ExecuteTreeExited));
            _onTreeExitedAction = null;
            return this;
        }
        private void ExecuteTreeExited() =>
            _onTreeExitedAction?.Invoke();
        

        private Action? _onTreeExitingAction; 
        public GraphEditAction OnTreeExiting(Action action) {
            if (_onTreeExitingAction == null) 
                Connect("tree_exiting", this, nameof(ExecuteTreeExiting));
            _onTreeExitingAction = action;
            return this;
        }
        public GraphEditAction RemoveOnTreeExiting() {
            if (_onTreeExitingAction == null) return this; 
            Disconnect("tree_exiting", this, nameof(ExecuteTreeExiting));
            _onTreeExitingAction = null;
            return this;
        }
        private void ExecuteTreeExiting() =>
            _onTreeExitingAction?.Invoke();
        

        private Action? _onVisibilityChangedAction; 
        public GraphEditAction OnVisibilityChanged(Action action) {
            if (_onVisibilityChangedAction == null) 
                Connect("visibility_changed", this, nameof(ExecuteVisibilityChanged));
            _onVisibilityChangedAction = action;
            return this;
        }
        public GraphEditAction RemoveOnVisibilityChanged() {
            if (_onVisibilityChangedAction == null) return this; 
            Disconnect("visibility_changed", this, nameof(ExecuteVisibilityChanged));
            _onVisibilityChangedAction = null;
            return this;
        }
        private void ExecuteVisibilityChanged() =>
            _onVisibilityChangedAction?.Invoke();
        
    }
}