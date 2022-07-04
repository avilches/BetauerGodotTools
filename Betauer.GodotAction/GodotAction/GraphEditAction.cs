using System;
using Godot;
using System.Collections.Generic;
using Environment = Godot.Environment;
using Animation = Godot.Animation;
using Object = Godot.Object;

namespace Betauer.GodotAction {
    public class GraphEditAction : ProxyNode {

        private List<Action>? _onBeginNodeMoveAction; 
        public GraphEditAction OnBeginNodeMove(Action action, bool oneShot = false, bool deferred = false) {
            AddSignal(ref _onBeginNodeMoveAction, "_begin_node_move", nameof(_GodotSignalBeginNodeMove), action, oneShot, deferred);
            return this;
        }

        public GraphEditAction RemoveOnBeginNodeMove(Action action) {
            RemoveSignal(_onBeginNodeMoveAction, "_begin_node_move", nameof(_GodotSignalBeginNodeMove), action);
            return this;
        }

        private GraphEditAction _GodotSignalBeginNodeMove() {
            ExecuteSignal(_onBeginNodeMoveAction);
            return this;
        }

        private List<Action<Vector2, string, int>>? _onConnectionFromEmptyAction; 
        public GraphEditAction OnConnectionFromEmpty(Action<Vector2, string, int> action, bool oneShot = false, bool deferred = false) {
            AddSignal(ref _onConnectionFromEmptyAction, "connection_from_empty", nameof(_GodotSignalConnectionFromEmpty), action, oneShot, deferred);
            return this;
        }

        public GraphEditAction RemoveOnConnectionFromEmpty(Action<Vector2, string, int> action) {
            RemoveSignal(_onConnectionFromEmptyAction, "connection_from_empty", nameof(_GodotSignalConnectionFromEmpty), action);
            return this;
        }

        private GraphEditAction _GodotSignalConnectionFromEmpty(Vector2 release_position, string to, int to_slot) {
            ExecuteSignal(_onConnectionFromEmptyAction, release_position, to, to_slot);
            return this;
        }

        private List<Action<string, int, string, int>>? _onConnectionRequestAction; 
        public GraphEditAction OnConnectionRequest(Action<string, int, string, int> action, bool oneShot = false, bool deferred = false) {
            AddSignal(ref _onConnectionRequestAction, "connection_request", nameof(_GodotSignalConnectionRequest), action, oneShot, deferred);
            return this;
        }

        public GraphEditAction RemoveOnConnectionRequest(Action<string, int, string, int> action) {
            RemoveSignal(_onConnectionRequestAction, "connection_request", nameof(_GodotSignalConnectionRequest), action);
            return this;
        }

        private GraphEditAction _GodotSignalConnectionRequest(string from, int from_slot, string to, int to_slot) {
            ExecuteSignal(_onConnectionRequestAction, from, from_slot, to, to_slot);
            return this;
        }

        private List<Action<string, int, Vector2>>? _onConnectionToEmptyAction; 
        public GraphEditAction OnConnectionToEmpty(Action<string, int, Vector2> action, bool oneShot = false, bool deferred = false) {
            AddSignal(ref _onConnectionToEmptyAction, "connection_to_empty", nameof(_GodotSignalConnectionToEmpty), action, oneShot, deferred);
            return this;
        }

        public GraphEditAction RemoveOnConnectionToEmpty(Action<string, int, Vector2> action) {
            RemoveSignal(_onConnectionToEmptyAction, "connection_to_empty", nameof(_GodotSignalConnectionToEmpty), action);
            return this;
        }

        private GraphEditAction _GodotSignalConnectionToEmpty(string from, int from_slot, Vector2 release_position) {
            ExecuteSignal(_onConnectionToEmptyAction, from, from_slot, release_position);
            return this;
        }

        private List<Action>? _onCopyNodesRequestAction; 
        public GraphEditAction OnCopyNodesRequest(Action action, bool oneShot = false, bool deferred = false) {
            AddSignal(ref _onCopyNodesRequestAction, "copy_nodes_request", nameof(_GodotSignalCopyNodesRequest), action, oneShot, deferred);
            return this;
        }

        public GraphEditAction RemoveOnCopyNodesRequest(Action action) {
            RemoveSignal(_onCopyNodesRequestAction, "copy_nodes_request", nameof(_GodotSignalCopyNodesRequest), action);
            return this;
        }

        private GraphEditAction _GodotSignalCopyNodesRequest() {
            ExecuteSignal(_onCopyNodesRequestAction);
            return this;
        }

        private List<Action>? _onDeleteNodesRequestAction; 
        public GraphEditAction OnDeleteNodesRequest(Action action, bool oneShot = false, bool deferred = false) {
            AddSignal(ref _onDeleteNodesRequestAction, "delete_nodes_request", nameof(_GodotSignalDeleteNodesRequest), action, oneShot, deferred);
            return this;
        }

        public GraphEditAction RemoveOnDeleteNodesRequest(Action action) {
            RemoveSignal(_onDeleteNodesRequestAction, "delete_nodes_request", nameof(_GodotSignalDeleteNodesRequest), action);
            return this;
        }

        private GraphEditAction _GodotSignalDeleteNodesRequest() {
            ExecuteSignal(_onDeleteNodesRequestAction);
            return this;
        }

        private List<Action<string, int, string, int>>? _onDisconnectionRequestAction; 
        public GraphEditAction OnDisconnectionRequest(Action<string, int, string, int> action, bool oneShot = false, bool deferred = false) {
            AddSignal(ref _onDisconnectionRequestAction, "disconnection_request", nameof(_GodotSignalDisconnectionRequest), action, oneShot, deferred);
            return this;
        }

        public GraphEditAction RemoveOnDisconnectionRequest(Action<string, int, string, int> action) {
            RemoveSignal(_onDisconnectionRequestAction, "disconnection_request", nameof(_GodotSignalDisconnectionRequest), action);
            return this;
        }

        private GraphEditAction _GodotSignalDisconnectionRequest(string from, int from_slot, string to, int to_slot) {
            ExecuteSignal(_onDisconnectionRequestAction, from, from_slot, to, to_slot);
            return this;
        }

        private List<Action>? _onDrawAction; 
        public GraphEditAction OnDraw(Action action, bool oneShot = false, bool deferred = false) {
            AddSignal(ref _onDrawAction, "draw", nameof(_GodotSignalDraw), action, oneShot, deferred);
            return this;
        }

        public GraphEditAction RemoveOnDraw(Action action) {
            RemoveSignal(_onDrawAction, "draw", nameof(_GodotSignalDraw), action);
            return this;
        }

        private GraphEditAction _GodotSignalDraw() {
            ExecuteSignal(_onDrawAction);
            return this;
        }

        private List<Action>? _onDuplicateNodesRequestAction; 
        public GraphEditAction OnDuplicateNodesRequest(Action action, bool oneShot = false, bool deferred = false) {
            AddSignal(ref _onDuplicateNodesRequestAction, "duplicate_nodes_request", nameof(_GodotSignalDuplicateNodesRequest), action, oneShot, deferred);
            return this;
        }

        public GraphEditAction RemoveOnDuplicateNodesRequest(Action action) {
            RemoveSignal(_onDuplicateNodesRequestAction, "duplicate_nodes_request", nameof(_GodotSignalDuplicateNodesRequest), action);
            return this;
        }

        private GraphEditAction _GodotSignalDuplicateNodesRequest() {
            ExecuteSignal(_onDuplicateNodesRequestAction);
            return this;
        }

        private List<Action>? _onEndNodeMoveAction; 
        public GraphEditAction OnEndNodeMove(Action action, bool oneShot = false, bool deferred = false) {
            AddSignal(ref _onEndNodeMoveAction, "_end_node_move", nameof(_GodotSignalEndNodeMove), action, oneShot, deferred);
            return this;
        }

        public GraphEditAction RemoveOnEndNodeMove(Action action) {
            RemoveSignal(_onEndNodeMoveAction, "_end_node_move", nameof(_GodotSignalEndNodeMove), action);
            return this;
        }

        private GraphEditAction _GodotSignalEndNodeMove() {
            ExecuteSignal(_onEndNodeMoveAction);
            return this;
        }

        private List<Action>? _onFocusEnteredAction; 
        public GraphEditAction OnFocusEntered(Action action, bool oneShot = false, bool deferred = false) {
            AddSignal(ref _onFocusEnteredAction, "focus_entered", nameof(_GodotSignalFocusEntered), action, oneShot, deferred);
            return this;
        }

        public GraphEditAction RemoveOnFocusEntered(Action action) {
            RemoveSignal(_onFocusEnteredAction, "focus_entered", nameof(_GodotSignalFocusEntered), action);
            return this;
        }

        private GraphEditAction _GodotSignalFocusEntered() {
            ExecuteSignal(_onFocusEnteredAction);
            return this;
        }

        private List<Action>? _onFocusExitedAction; 
        public GraphEditAction OnFocusExited(Action action, bool oneShot = false, bool deferred = false) {
            AddSignal(ref _onFocusExitedAction, "focus_exited", nameof(_GodotSignalFocusExited), action, oneShot, deferred);
            return this;
        }

        public GraphEditAction RemoveOnFocusExited(Action action) {
            RemoveSignal(_onFocusExitedAction, "focus_exited", nameof(_GodotSignalFocusExited), action);
            return this;
        }

        private GraphEditAction _GodotSignalFocusExited() {
            ExecuteSignal(_onFocusExitedAction);
            return this;
        }

        private List<Action<InputEvent>>? _onGuiInputAction; 
        public GraphEditAction OnGuiInput(Action<InputEvent> action, bool oneShot = false, bool deferred = false) {
            AddSignal(ref _onGuiInputAction, "gui_input", nameof(_GodotSignalGuiInput), action, oneShot, deferred);
            return this;
        }

        public GraphEditAction RemoveOnGuiInput(Action<InputEvent> action) {
            RemoveSignal(_onGuiInputAction, "gui_input", nameof(_GodotSignalGuiInput), action);
            return this;
        }

        private GraphEditAction _GodotSignalGuiInput(InputEvent @event) {
            ExecuteSignal(_onGuiInputAction, @event);
            return this;
        }

        private List<Action>? _onHideAction; 
        public GraphEditAction OnHide(Action action, bool oneShot = false, bool deferred = false) {
            AddSignal(ref _onHideAction, "hide", nameof(_GodotSignalHide), action, oneShot, deferred);
            return this;
        }

        public GraphEditAction RemoveOnHide(Action action) {
            RemoveSignal(_onHideAction, "hide", nameof(_GodotSignalHide), action);
            return this;
        }

        private GraphEditAction _GodotSignalHide() {
            ExecuteSignal(_onHideAction);
            return this;
        }

        private List<Action>? _onItemRectChangedAction; 
        public GraphEditAction OnItemRectChanged(Action action, bool oneShot = false, bool deferred = false) {
            AddSignal(ref _onItemRectChangedAction, "item_rect_changed", nameof(_GodotSignalItemRectChanged), action, oneShot, deferred);
            return this;
        }

        public GraphEditAction RemoveOnItemRectChanged(Action action) {
            RemoveSignal(_onItemRectChangedAction, "item_rect_changed", nameof(_GodotSignalItemRectChanged), action);
            return this;
        }

        private GraphEditAction _GodotSignalItemRectChanged() {
            ExecuteSignal(_onItemRectChangedAction);
            return this;
        }

        private List<Action>? _onMinimumSizeChangedAction; 
        public GraphEditAction OnMinimumSizeChanged(Action action, bool oneShot = false, bool deferred = false) {
            AddSignal(ref _onMinimumSizeChangedAction, "minimum_size_changed", nameof(_GodotSignalMinimumSizeChanged), action, oneShot, deferred);
            return this;
        }

        public GraphEditAction RemoveOnMinimumSizeChanged(Action action) {
            RemoveSignal(_onMinimumSizeChangedAction, "minimum_size_changed", nameof(_GodotSignalMinimumSizeChanged), action);
            return this;
        }

        private GraphEditAction _GodotSignalMinimumSizeChanged() {
            ExecuteSignal(_onMinimumSizeChangedAction);
            return this;
        }

        private List<Action>? _onModalClosedAction; 
        public GraphEditAction OnModalClosed(Action action, bool oneShot = false, bool deferred = false) {
            AddSignal(ref _onModalClosedAction, "modal_closed", nameof(_GodotSignalModalClosed), action, oneShot, deferred);
            return this;
        }

        public GraphEditAction RemoveOnModalClosed(Action action) {
            RemoveSignal(_onModalClosedAction, "modal_closed", nameof(_GodotSignalModalClosed), action);
            return this;
        }

        private GraphEditAction _GodotSignalModalClosed() {
            ExecuteSignal(_onModalClosedAction);
            return this;
        }

        private List<Action>? _onMouseEnteredAction; 
        public GraphEditAction OnMouseEntered(Action action, bool oneShot = false, bool deferred = false) {
            AddSignal(ref _onMouseEnteredAction, "mouse_entered", nameof(_GodotSignalMouseEntered), action, oneShot, deferred);
            return this;
        }

        public GraphEditAction RemoveOnMouseEntered(Action action) {
            RemoveSignal(_onMouseEnteredAction, "mouse_entered", nameof(_GodotSignalMouseEntered), action);
            return this;
        }

        private GraphEditAction _GodotSignalMouseEntered() {
            ExecuteSignal(_onMouseEnteredAction);
            return this;
        }

        private List<Action>? _onMouseExitedAction; 
        public GraphEditAction OnMouseExited(Action action, bool oneShot = false, bool deferred = false) {
            AddSignal(ref _onMouseExitedAction, "mouse_exited", nameof(_GodotSignalMouseExited), action, oneShot, deferred);
            return this;
        }

        public GraphEditAction RemoveOnMouseExited(Action action) {
            RemoveSignal(_onMouseExitedAction, "mouse_exited", nameof(_GodotSignalMouseExited), action);
            return this;
        }

        private GraphEditAction _GodotSignalMouseExited() {
            ExecuteSignal(_onMouseExitedAction);
            return this;
        }

        private List<Action<Node>>? _onNodeSelectedAction; 
        public GraphEditAction OnNodeSelected(Action<Node> action, bool oneShot = false, bool deferred = false) {
            AddSignal(ref _onNodeSelectedAction, "node_selected", nameof(_GodotSignalNodeSelected), action, oneShot, deferred);
            return this;
        }

        public GraphEditAction RemoveOnNodeSelected(Action<Node> action) {
            RemoveSignal(_onNodeSelectedAction, "node_selected", nameof(_GodotSignalNodeSelected), action);
            return this;
        }

        private GraphEditAction _GodotSignalNodeSelected(Node node) {
            ExecuteSignal(_onNodeSelectedAction, node);
            return this;
        }

        private List<Action<Node>>? _onNodeUnselectedAction; 
        public GraphEditAction OnNodeUnselected(Action<Node> action, bool oneShot = false, bool deferred = false) {
            AddSignal(ref _onNodeUnselectedAction, "node_unselected", nameof(_GodotSignalNodeUnselected), action, oneShot, deferred);
            return this;
        }

        public GraphEditAction RemoveOnNodeUnselected(Action<Node> action) {
            RemoveSignal(_onNodeUnselectedAction, "node_unselected", nameof(_GodotSignalNodeUnselected), action);
            return this;
        }

        private GraphEditAction _GodotSignalNodeUnselected(Node node) {
            ExecuteSignal(_onNodeUnselectedAction, node);
            return this;
        }

        private List<Action>? _onPasteNodesRequestAction; 
        public GraphEditAction OnPasteNodesRequest(Action action, bool oneShot = false, bool deferred = false) {
            AddSignal(ref _onPasteNodesRequestAction, "paste_nodes_request", nameof(_GodotSignalPasteNodesRequest), action, oneShot, deferred);
            return this;
        }

        public GraphEditAction RemoveOnPasteNodesRequest(Action action) {
            RemoveSignal(_onPasteNodesRequestAction, "paste_nodes_request", nameof(_GodotSignalPasteNodesRequest), action);
            return this;
        }

        private GraphEditAction _GodotSignalPasteNodesRequest() {
            ExecuteSignal(_onPasteNodesRequestAction);
            return this;
        }

        private List<Action<Vector2>>? _onPopupRequestAction; 
        public GraphEditAction OnPopupRequest(Action<Vector2> action, bool oneShot = false, bool deferred = false) {
            AddSignal(ref _onPopupRequestAction, "popup_request", nameof(_GodotSignalPopupRequest), action, oneShot, deferred);
            return this;
        }

        public GraphEditAction RemoveOnPopupRequest(Action<Vector2> action) {
            RemoveSignal(_onPopupRequestAction, "popup_request", nameof(_GodotSignalPopupRequest), action);
            return this;
        }

        private GraphEditAction _GodotSignalPopupRequest(Vector2 position) {
            ExecuteSignal(_onPopupRequestAction, position);
            return this;
        }

        private List<Action>? _onReadyAction; 
        public GraphEditAction OnReady(Action action, bool oneShot = false, bool deferred = false) {
            AddSignal(ref _onReadyAction, "ready", nameof(_GodotSignalReady), action, oneShot, deferred);
            return this;
        }

        public GraphEditAction RemoveOnReady(Action action) {
            RemoveSignal(_onReadyAction, "ready", nameof(_GodotSignalReady), action);
            return this;
        }

        private GraphEditAction _GodotSignalReady() {
            ExecuteSignal(_onReadyAction);
            return this;
        }

        private List<Action>? _onRenamedAction; 
        public GraphEditAction OnRenamed(Action action, bool oneShot = false, bool deferred = false) {
            AddSignal(ref _onRenamedAction, "renamed", nameof(_GodotSignalRenamed), action, oneShot, deferred);
            return this;
        }

        public GraphEditAction RemoveOnRenamed(Action action) {
            RemoveSignal(_onRenamedAction, "renamed", nameof(_GodotSignalRenamed), action);
            return this;
        }

        private GraphEditAction _GodotSignalRenamed() {
            ExecuteSignal(_onRenamedAction);
            return this;
        }

        private List<Action>? _onResizedAction; 
        public GraphEditAction OnResized(Action action, bool oneShot = false, bool deferred = false) {
            AddSignal(ref _onResizedAction, "resized", nameof(_GodotSignalResized), action, oneShot, deferred);
            return this;
        }

        public GraphEditAction RemoveOnResized(Action action) {
            RemoveSignal(_onResizedAction, "resized", nameof(_GodotSignalResized), action);
            return this;
        }

        private GraphEditAction _GodotSignalResized() {
            ExecuteSignal(_onResizedAction);
            return this;
        }

        private List<Action>? _onScriptChangedAction; 
        public GraphEditAction OnScriptChanged(Action action, bool oneShot = false, bool deferred = false) {
            AddSignal(ref _onScriptChangedAction, "script_changed", nameof(_GodotSignalScriptChanged), action, oneShot, deferred);
            return this;
        }

        public GraphEditAction RemoveOnScriptChanged(Action action) {
            RemoveSignal(_onScriptChangedAction, "script_changed", nameof(_GodotSignalScriptChanged), action);
            return this;
        }

        private GraphEditAction _GodotSignalScriptChanged() {
            ExecuteSignal(_onScriptChangedAction);
            return this;
        }

        private List<Action<Vector2>>? _onScrollOffsetChangedAction; 
        public GraphEditAction OnScrollOffsetChanged(Action<Vector2> action, bool oneShot = false, bool deferred = false) {
            AddSignal(ref _onScrollOffsetChangedAction, "scroll_offset_changed", nameof(_GodotSignalScrollOffsetChanged), action, oneShot, deferred);
            return this;
        }

        public GraphEditAction RemoveOnScrollOffsetChanged(Action<Vector2> action) {
            RemoveSignal(_onScrollOffsetChangedAction, "scroll_offset_changed", nameof(_GodotSignalScrollOffsetChanged), action);
            return this;
        }

        private GraphEditAction _GodotSignalScrollOffsetChanged(Vector2 ofs) {
            ExecuteSignal(_onScrollOffsetChangedAction, ofs);
            return this;
        }

        private List<Action>? _onSizeFlagsChangedAction; 
        public GraphEditAction OnSizeFlagsChanged(Action action, bool oneShot = false, bool deferred = false) {
            AddSignal(ref _onSizeFlagsChangedAction, "size_flags_changed", nameof(_GodotSignalSizeFlagsChanged), action, oneShot, deferred);
            return this;
        }

        public GraphEditAction RemoveOnSizeFlagsChanged(Action action) {
            RemoveSignal(_onSizeFlagsChangedAction, "size_flags_changed", nameof(_GodotSignalSizeFlagsChanged), action);
            return this;
        }

        private GraphEditAction _GodotSignalSizeFlagsChanged() {
            ExecuteSignal(_onSizeFlagsChangedAction);
            return this;
        }

        private List<Action>? _onTreeEnteredAction; 
        public GraphEditAction OnTreeEntered(Action action, bool oneShot = false, bool deferred = false) {
            AddSignal(ref _onTreeEnteredAction, "tree_entered", nameof(_GodotSignalTreeEntered), action, oneShot, deferred);
            return this;
        }

        public GraphEditAction RemoveOnTreeEntered(Action action) {
            RemoveSignal(_onTreeEnteredAction, "tree_entered", nameof(_GodotSignalTreeEntered), action);
            return this;
        }

        private GraphEditAction _GodotSignalTreeEntered() {
            ExecuteSignal(_onTreeEnteredAction);
            return this;
        }

        private List<Action>? _onTreeExitedAction; 
        public GraphEditAction OnTreeExited(Action action, bool oneShot = false, bool deferred = false) {
            AddSignal(ref _onTreeExitedAction, "tree_exited", nameof(_GodotSignalTreeExited), action, oneShot, deferred);
            return this;
        }

        public GraphEditAction RemoveOnTreeExited(Action action) {
            RemoveSignal(_onTreeExitedAction, "tree_exited", nameof(_GodotSignalTreeExited), action);
            return this;
        }

        private GraphEditAction _GodotSignalTreeExited() {
            ExecuteSignal(_onTreeExitedAction);
            return this;
        }

        private List<Action>? _onTreeExitingAction; 
        public GraphEditAction OnTreeExiting(Action action, bool oneShot = false, bool deferred = false) {
            AddSignal(ref _onTreeExitingAction, "tree_exiting", nameof(_GodotSignalTreeExiting), action, oneShot, deferred);
            return this;
        }

        public GraphEditAction RemoveOnTreeExiting(Action action) {
            RemoveSignal(_onTreeExitingAction, "tree_exiting", nameof(_GodotSignalTreeExiting), action);
            return this;
        }

        private GraphEditAction _GodotSignalTreeExiting() {
            ExecuteSignal(_onTreeExitingAction);
            return this;
        }

        private List<Action>? _onVisibilityChangedAction; 
        public GraphEditAction OnVisibilityChanged(Action action, bool oneShot = false, bool deferred = false) {
            AddSignal(ref _onVisibilityChangedAction, "visibility_changed", nameof(_GodotSignalVisibilityChanged), action, oneShot, deferred);
            return this;
        }

        public GraphEditAction RemoveOnVisibilityChanged(Action action) {
            RemoveSignal(_onVisibilityChangedAction, "visibility_changed", nameof(_GodotSignalVisibilityChanged), action);
            return this;
        }

        private GraphEditAction _GodotSignalVisibilityChanged() {
            ExecuteSignal(_onVisibilityChangedAction);
            return this;
        }
    }
}