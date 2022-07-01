using System;
using Godot;
using System.Collections.Generic;
using Environment = Godot.Environment;
using Animation = Godot.Animation;
using Object = Godot.Object;

namespace Betauer.GodotAction {
    public class GraphEditAction : ProxyNode {

        private List<Action>? _onBeginNodeMoveAction; 
        public void OnBeginNodeMove(Action action, bool oneShot = false, bool deferred = false) =>
            AddSignal(ref _onBeginNodeMoveAction, "_begin_node_move", nameof(_GodotSignalBeginNodeMove), action, oneShot, deferred);

        public void RemoveOnBeginNodeMove(Action action) =>
            RemoveSignal(_onBeginNodeMoveAction, "_begin_node_move", nameof(_GodotSignalBeginNodeMove), action);

        private void _GodotSignalBeginNodeMove() =>
            ExecuteSignal(_onBeginNodeMoveAction);
        

        private List<Action<Vector2, string, int>>? _onConnectionFromEmptyAction; 
        public void OnConnectionFromEmpty(Action<Vector2, string, int> action, bool oneShot = false, bool deferred = false) =>
            AddSignal(ref _onConnectionFromEmptyAction, "connection_from_empty", nameof(_GodotSignalConnectionFromEmpty), action, oneShot, deferred);

        public void RemoveOnConnectionFromEmpty(Action<Vector2, string, int> action) =>
            RemoveSignal(_onConnectionFromEmptyAction, "connection_from_empty", nameof(_GodotSignalConnectionFromEmpty), action);

        private void _GodotSignalConnectionFromEmpty(Vector2 release_position, string to, int to_slot) =>
            ExecuteSignal(_onConnectionFromEmptyAction, release_position, to, to_slot);
        

        private List<Action<string, int, string, int>>? _onConnectionRequestAction; 
        public void OnConnectionRequest(Action<string, int, string, int> action, bool oneShot = false, bool deferred = false) =>
            AddSignal(ref _onConnectionRequestAction, "connection_request", nameof(_GodotSignalConnectionRequest), action, oneShot, deferred);

        public void RemoveOnConnectionRequest(Action<string, int, string, int> action) =>
            RemoveSignal(_onConnectionRequestAction, "connection_request", nameof(_GodotSignalConnectionRequest), action);

        private void _GodotSignalConnectionRequest(string from, int from_slot, string to, int to_slot) =>
            ExecuteSignal(_onConnectionRequestAction, from, from_slot, to, to_slot);
        

        private List<Action<string, int, Vector2>>? _onConnectionToEmptyAction; 
        public void OnConnectionToEmpty(Action<string, int, Vector2> action, bool oneShot = false, bool deferred = false) =>
            AddSignal(ref _onConnectionToEmptyAction, "connection_to_empty", nameof(_GodotSignalConnectionToEmpty), action, oneShot, deferred);

        public void RemoveOnConnectionToEmpty(Action<string, int, Vector2> action) =>
            RemoveSignal(_onConnectionToEmptyAction, "connection_to_empty", nameof(_GodotSignalConnectionToEmpty), action);

        private void _GodotSignalConnectionToEmpty(string from, int from_slot, Vector2 release_position) =>
            ExecuteSignal(_onConnectionToEmptyAction, from, from_slot, release_position);
        

        private List<Action>? _onCopyNodesRequestAction; 
        public void OnCopyNodesRequest(Action action, bool oneShot = false, bool deferred = false) =>
            AddSignal(ref _onCopyNodesRequestAction, "copy_nodes_request", nameof(_GodotSignalCopyNodesRequest), action, oneShot, deferred);

        public void RemoveOnCopyNodesRequest(Action action) =>
            RemoveSignal(_onCopyNodesRequestAction, "copy_nodes_request", nameof(_GodotSignalCopyNodesRequest), action);

        private void _GodotSignalCopyNodesRequest() =>
            ExecuteSignal(_onCopyNodesRequestAction);
        

        private List<Action>? _onDeleteNodesRequestAction; 
        public void OnDeleteNodesRequest(Action action, bool oneShot = false, bool deferred = false) =>
            AddSignal(ref _onDeleteNodesRequestAction, "delete_nodes_request", nameof(_GodotSignalDeleteNodesRequest), action, oneShot, deferred);

        public void RemoveOnDeleteNodesRequest(Action action) =>
            RemoveSignal(_onDeleteNodesRequestAction, "delete_nodes_request", nameof(_GodotSignalDeleteNodesRequest), action);

        private void _GodotSignalDeleteNodesRequest() =>
            ExecuteSignal(_onDeleteNodesRequestAction);
        

        private List<Action<string, int, string, int>>? _onDisconnectionRequestAction; 
        public void OnDisconnectionRequest(Action<string, int, string, int> action, bool oneShot = false, bool deferred = false) =>
            AddSignal(ref _onDisconnectionRequestAction, "disconnection_request", nameof(_GodotSignalDisconnectionRequest), action, oneShot, deferred);

        public void RemoveOnDisconnectionRequest(Action<string, int, string, int> action) =>
            RemoveSignal(_onDisconnectionRequestAction, "disconnection_request", nameof(_GodotSignalDisconnectionRequest), action);

        private void _GodotSignalDisconnectionRequest(string from, int from_slot, string to, int to_slot) =>
            ExecuteSignal(_onDisconnectionRequestAction, from, from_slot, to, to_slot);
        

        private List<Action>? _onDrawAction; 
        public void OnDraw(Action action, bool oneShot = false, bool deferred = false) =>
            AddSignal(ref _onDrawAction, "draw", nameof(_GodotSignalDraw), action, oneShot, deferred);

        public void RemoveOnDraw(Action action) =>
            RemoveSignal(_onDrawAction, "draw", nameof(_GodotSignalDraw), action);

        private void _GodotSignalDraw() =>
            ExecuteSignal(_onDrawAction);
        

        private List<Action>? _onDuplicateNodesRequestAction; 
        public void OnDuplicateNodesRequest(Action action, bool oneShot = false, bool deferred = false) =>
            AddSignal(ref _onDuplicateNodesRequestAction, "duplicate_nodes_request", nameof(_GodotSignalDuplicateNodesRequest), action, oneShot, deferred);

        public void RemoveOnDuplicateNodesRequest(Action action) =>
            RemoveSignal(_onDuplicateNodesRequestAction, "duplicate_nodes_request", nameof(_GodotSignalDuplicateNodesRequest), action);

        private void _GodotSignalDuplicateNodesRequest() =>
            ExecuteSignal(_onDuplicateNodesRequestAction);
        

        private List<Action>? _onEndNodeMoveAction; 
        public void OnEndNodeMove(Action action, bool oneShot = false, bool deferred = false) =>
            AddSignal(ref _onEndNodeMoveAction, "_end_node_move", nameof(_GodotSignalEndNodeMove), action, oneShot, deferred);

        public void RemoveOnEndNodeMove(Action action) =>
            RemoveSignal(_onEndNodeMoveAction, "_end_node_move", nameof(_GodotSignalEndNodeMove), action);

        private void _GodotSignalEndNodeMove() =>
            ExecuteSignal(_onEndNodeMoveAction);
        

        private List<Action>? _onFocusEnteredAction; 
        public void OnFocusEntered(Action action, bool oneShot = false, bool deferred = false) =>
            AddSignal(ref _onFocusEnteredAction, "focus_entered", nameof(_GodotSignalFocusEntered), action, oneShot, deferred);

        public void RemoveOnFocusEntered(Action action) =>
            RemoveSignal(_onFocusEnteredAction, "focus_entered", nameof(_GodotSignalFocusEntered), action);

        private void _GodotSignalFocusEntered() =>
            ExecuteSignal(_onFocusEnteredAction);
        

        private List<Action>? _onFocusExitedAction; 
        public void OnFocusExited(Action action, bool oneShot = false, bool deferred = false) =>
            AddSignal(ref _onFocusExitedAction, "focus_exited", nameof(_GodotSignalFocusExited), action, oneShot, deferred);

        public void RemoveOnFocusExited(Action action) =>
            RemoveSignal(_onFocusExitedAction, "focus_exited", nameof(_GodotSignalFocusExited), action);

        private void _GodotSignalFocusExited() =>
            ExecuteSignal(_onFocusExitedAction);
        

        private List<Action<InputEvent>>? _onGuiInputAction; 
        public void OnGuiInput(Action<InputEvent> action, bool oneShot = false, bool deferred = false) =>
            AddSignal(ref _onGuiInputAction, "gui_input", nameof(_GodotSignalGuiInput), action, oneShot, deferred);

        public void RemoveOnGuiInput(Action<InputEvent> action) =>
            RemoveSignal(_onGuiInputAction, "gui_input", nameof(_GodotSignalGuiInput), action);

        private void _GodotSignalGuiInput(InputEvent @event) =>
            ExecuteSignal(_onGuiInputAction, @event);
        

        private List<Action>? _onHideAction; 
        public void OnHide(Action action, bool oneShot = false, bool deferred = false) =>
            AddSignal(ref _onHideAction, "hide", nameof(_GodotSignalHide), action, oneShot, deferred);

        public void RemoveOnHide(Action action) =>
            RemoveSignal(_onHideAction, "hide", nameof(_GodotSignalHide), action);

        private void _GodotSignalHide() =>
            ExecuteSignal(_onHideAction);
        

        private List<Action>? _onItemRectChangedAction; 
        public void OnItemRectChanged(Action action, bool oneShot = false, bool deferred = false) =>
            AddSignal(ref _onItemRectChangedAction, "item_rect_changed", nameof(_GodotSignalItemRectChanged), action, oneShot, deferred);

        public void RemoveOnItemRectChanged(Action action) =>
            RemoveSignal(_onItemRectChangedAction, "item_rect_changed", nameof(_GodotSignalItemRectChanged), action);

        private void _GodotSignalItemRectChanged() =>
            ExecuteSignal(_onItemRectChangedAction);
        

        private List<Action>? _onMinimumSizeChangedAction; 
        public void OnMinimumSizeChanged(Action action, bool oneShot = false, bool deferred = false) =>
            AddSignal(ref _onMinimumSizeChangedAction, "minimum_size_changed", nameof(_GodotSignalMinimumSizeChanged), action, oneShot, deferred);

        public void RemoveOnMinimumSizeChanged(Action action) =>
            RemoveSignal(_onMinimumSizeChangedAction, "minimum_size_changed", nameof(_GodotSignalMinimumSizeChanged), action);

        private void _GodotSignalMinimumSizeChanged() =>
            ExecuteSignal(_onMinimumSizeChangedAction);
        

        private List<Action>? _onModalClosedAction; 
        public void OnModalClosed(Action action, bool oneShot = false, bool deferred = false) =>
            AddSignal(ref _onModalClosedAction, "modal_closed", nameof(_GodotSignalModalClosed), action, oneShot, deferred);

        public void RemoveOnModalClosed(Action action) =>
            RemoveSignal(_onModalClosedAction, "modal_closed", nameof(_GodotSignalModalClosed), action);

        private void _GodotSignalModalClosed() =>
            ExecuteSignal(_onModalClosedAction);
        

        private List<Action>? _onMouseEnteredAction; 
        public void OnMouseEntered(Action action, bool oneShot = false, bool deferred = false) =>
            AddSignal(ref _onMouseEnteredAction, "mouse_entered", nameof(_GodotSignalMouseEntered), action, oneShot, deferred);

        public void RemoveOnMouseEntered(Action action) =>
            RemoveSignal(_onMouseEnteredAction, "mouse_entered", nameof(_GodotSignalMouseEntered), action);

        private void _GodotSignalMouseEntered() =>
            ExecuteSignal(_onMouseEnteredAction);
        

        private List<Action>? _onMouseExitedAction; 
        public void OnMouseExited(Action action, bool oneShot = false, bool deferred = false) =>
            AddSignal(ref _onMouseExitedAction, "mouse_exited", nameof(_GodotSignalMouseExited), action, oneShot, deferred);

        public void RemoveOnMouseExited(Action action) =>
            RemoveSignal(_onMouseExitedAction, "mouse_exited", nameof(_GodotSignalMouseExited), action);

        private void _GodotSignalMouseExited() =>
            ExecuteSignal(_onMouseExitedAction);
        

        private List<Action<Node>>? _onNodeSelectedAction; 
        public void OnNodeSelected(Action<Node> action, bool oneShot = false, bool deferred = false) =>
            AddSignal(ref _onNodeSelectedAction, "node_selected", nameof(_GodotSignalNodeSelected), action, oneShot, deferred);

        public void RemoveOnNodeSelected(Action<Node> action) =>
            RemoveSignal(_onNodeSelectedAction, "node_selected", nameof(_GodotSignalNodeSelected), action);

        private void _GodotSignalNodeSelected(Node node) =>
            ExecuteSignal(_onNodeSelectedAction, node);
        

        private List<Action<Node>>? _onNodeUnselectedAction; 
        public void OnNodeUnselected(Action<Node> action, bool oneShot = false, bool deferred = false) =>
            AddSignal(ref _onNodeUnselectedAction, "node_unselected", nameof(_GodotSignalNodeUnselected), action, oneShot, deferred);

        public void RemoveOnNodeUnselected(Action<Node> action) =>
            RemoveSignal(_onNodeUnselectedAction, "node_unselected", nameof(_GodotSignalNodeUnselected), action);

        private void _GodotSignalNodeUnselected(Node node) =>
            ExecuteSignal(_onNodeUnselectedAction, node);
        

        private List<Action>? _onPasteNodesRequestAction; 
        public void OnPasteNodesRequest(Action action, bool oneShot = false, bool deferred = false) =>
            AddSignal(ref _onPasteNodesRequestAction, "paste_nodes_request", nameof(_GodotSignalPasteNodesRequest), action, oneShot, deferred);

        public void RemoveOnPasteNodesRequest(Action action) =>
            RemoveSignal(_onPasteNodesRequestAction, "paste_nodes_request", nameof(_GodotSignalPasteNodesRequest), action);

        private void _GodotSignalPasteNodesRequest() =>
            ExecuteSignal(_onPasteNodesRequestAction);
        

        private List<Action<Vector2>>? _onPopupRequestAction; 
        public void OnPopupRequest(Action<Vector2> action, bool oneShot = false, bool deferred = false) =>
            AddSignal(ref _onPopupRequestAction, "popup_request", nameof(_GodotSignalPopupRequest), action, oneShot, deferred);

        public void RemoveOnPopupRequest(Action<Vector2> action) =>
            RemoveSignal(_onPopupRequestAction, "popup_request", nameof(_GodotSignalPopupRequest), action);

        private void _GodotSignalPopupRequest(Vector2 position) =>
            ExecuteSignal(_onPopupRequestAction, position);
        

        private List<Action>? _onReadyAction; 
        public void OnReady(Action action, bool oneShot = false, bool deferred = false) =>
            AddSignal(ref _onReadyAction, "ready", nameof(_GodotSignalReady), action, oneShot, deferred);

        public void RemoveOnReady(Action action) =>
            RemoveSignal(_onReadyAction, "ready", nameof(_GodotSignalReady), action);

        private void _GodotSignalReady() =>
            ExecuteSignal(_onReadyAction);
        

        private List<Action>? _onRenamedAction; 
        public void OnRenamed(Action action, bool oneShot = false, bool deferred = false) =>
            AddSignal(ref _onRenamedAction, "renamed", nameof(_GodotSignalRenamed), action, oneShot, deferred);

        public void RemoveOnRenamed(Action action) =>
            RemoveSignal(_onRenamedAction, "renamed", nameof(_GodotSignalRenamed), action);

        private void _GodotSignalRenamed() =>
            ExecuteSignal(_onRenamedAction);
        

        private List<Action>? _onResizedAction; 
        public void OnResized(Action action, bool oneShot = false, bool deferred = false) =>
            AddSignal(ref _onResizedAction, "resized", nameof(_GodotSignalResized), action, oneShot, deferred);

        public void RemoveOnResized(Action action) =>
            RemoveSignal(_onResizedAction, "resized", nameof(_GodotSignalResized), action);

        private void _GodotSignalResized() =>
            ExecuteSignal(_onResizedAction);
        

        private List<Action>? _onScriptChangedAction; 
        public void OnScriptChanged(Action action, bool oneShot = false, bool deferred = false) =>
            AddSignal(ref _onScriptChangedAction, "script_changed", nameof(_GodotSignalScriptChanged), action, oneShot, deferred);

        public void RemoveOnScriptChanged(Action action) =>
            RemoveSignal(_onScriptChangedAction, "script_changed", nameof(_GodotSignalScriptChanged), action);

        private void _GodotSignalScriptChanged() =>
            ExecuteSignal(_onScriptChangedAction);
        

        private List<Action<Vector2>>? _onScrollOffsetChangedAction; 
        public void OnScrollOffsetChanged(Action<Vector2> action, bool oneShot = false, bool deferred = false) =>
            AddSignal(ref _onScrollOffsetChangedAction, "scroll_offset_changed", nameof(_GodotSignalScrollOffsetChanged), action, oneShot, deferred);

        public void RemoveOnScrollOffsetChanged(Action<Vector2> action) =>
            RemoveSignal(_onScrollOffsetChangedAction, "scroll_offset_changed", nameof(_GodotSignalScrollOffsetChanged), action);

        private void _GodotSignalScrollOffsetChanged(Vector2 ofs) =>
            ExecuteSignal(_onScrollOffsetChangedAction, ofs);
        

        private List<Action>? _onSizeFlagsChangedAction; 
        public void OnSizeFlagsChanged(Action action, bool oneShot = false, bool deferred = false) =>
            AddSignal(ref _onSizeFlagsChangedAction, "size_flags_changed", nameof(_GodotSignalSizeFlagsChanged), action, oneShot, deferred);

        public void RemoveOnSizeFlagsChanged(Action action) =>
            RemoveSignal(_onSizeFlagsChangedAction, "size_flags_changed", nameof(_GodotSignalSizeFlagsChanged), action);

        private void _GodotSignalSizeFlagsChanged() =>
            ExecuteSignal(_onSizeFlagsChangedAction);
        

        private List<Action>? _onTreeEnteredAction; 
        public void OnTreeEntered(Action action, bool oneShot = false, bool deferred = false) =>
            AddSignal(ref _onTreeEnteredAction, "tree_entered", nameof(_GodotSignalTreeEntered), action, oneShot, deferred);

        public void RemoveOnTreeEntered(Action action) =>
            RemoveSignal(_onTreeEnteredAction, "tree_entered", nameof(_GodotSignalTreeEntered), action);

        private void _GodotSignalTreeEntered() =>
            ExecuteSignal(_onTreeEnteredAction);
        

        private List<Action>? _onTreeExitedAction; 
        public void OnTreeExited(Action action, bool oneShot = false, bool deferred = false) =>
            AddSignal(ref _onTreeExitedAction, "tree_exited", nameof(_GodotSignalTreeExited), action, oneShot, deferred);

        public void RemoveOnTreeExited(Action action) =>
            RemoveSignal(_onTreeExitedAction, "tree_exited", nameof(_GodotSignalTreeExited), action);

        private void _GodotSignalTreeExited() =>
            ExecuteSignal(_onTreeExitedAction);
        

        private List<Action>? _onTreeExitingAction; 
        public void OnTreeExiting(Action action, bool oneShot = false, bool deferred = false) =>
            AddSignal(ref _onTreeExitingAction, "tree_exiting", nameof(_GodotSignalTreeExiting), action, oneShot, deferred);

        public void RemoveOnTreeExiting(Action action) =>
            RemoveSignal(_onTreeExitingAction, "tree_exiting", nameof(_GodotSignalTreeExiting), action);

        private void _GodotSignalTreeExiting() =>
            ExecuteSignal(_onTreeExitingAction);
        

        private List<Action>? _onVisibilityChangedAction; 
        public void OnVisibilityChanged(Action action, bool oneShot = false, bool deferred = false) =>
            AddSignal(ref _onVisibilityChangedAction, "visibility_changed", nameof(_GodotSignalVisibilityChanged), action, oneShot, deferred);

        public void RemoveOnVisibilityChanged(Action action) =>
            RemoveSignal(_onVisibilityChangedAction, "visibility_changed", nameof(_GodotSignalVisibilityChanged), action);

        private void _GodotSignalVisibilityChanged() =>
            ExecuteSignal(_onVisibilityChangedAction);
        
    }
}