using System;
using Godot;
using System.Collections.Generic;
using Environment = Godot.Environment;
using Animation = Godot.Animation;
using Object = Godot.Object;

namespace Betauer.GodotAction {
    public class GraphEditAction : Node {
        public GraphEditAction() {
            SetProcess(false);
            SetPhysicsProcess(false);
            SetProcessInput(false);
            SetProcessUnhandledInput(false);
            SetProcessUnhandledKeyInput(false);
        }

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
        public GraphEditAction OnBeginNodeMove(Action action, bool oneShot = false, bool deferred = false) {
            if (_onBeginNodeMoveAction == null || _onBeginNodeMoveAction.Count == 0) {
                _onBeginNodeMoveAction ??= new List<Action>(); 
                GetParent().Connect("_begin_node_move", this, nameof(_GodotSignalBeginNodeMove));
            }
            _onBeginNodeMoveAction.Add(action);
            return this;
        }
        public GraphEditAction RemoveOnBeginNodeMove(Action action) {
            if (_onBeginNodeMoveAction == null || _onBeginNodeMoveAction.Count == 0) return this;
            _onBeginNodeMoveAction.Remove(action); 
            if (_onBeginNodeMoveAction.Count == 0) {
                GetParent().Disconnect("_begin_node_move", this, nameof(_GodotSignalBeginNodeMove));
            }
            return this;
        }
        private void _GodotSignalBeginNodeMove() {
            if (_onBeginNodeMoveAction == null || _onBeginNodeMoveAction.Count == 0) return;
            for (var i = 0; i < _onBeginNodeMoveAction.Count; i++) _onBeginNodeMoveAction[i].Invoke();
        }
        

        private List<Action<Vector2, string, int>>? _onConnectionFromEmptyAction; 
        public GraphEditAction OnConnectionFromEmpty(Action<Vector2, string, int> action, bool oneShot = false, bool deferred = false) {
            if (_onConnectionFromEmptyAction == null || _onConnectionFromEmptyAction.Count == 0) {
                _onConnectionFromEmptyAction ??= new List<Action<Vector2, string, int>>(); 
                GetParent().Connect("connection_from_empty", this, nameof(_GodotSignalConnectionFromEmpty));
            }
            _onConnectionFromEmptyAction.Add(action);
            return this;
        }
        public GraphEditAction RemoveOnConnectionFromEmpty(Action<Vector2, string, int> action) {
            if (_onConnectionFromEmptyAction == null || _onConnectionFromEmptyAction.Count == 0) return this;
            _onConnectionFromEmptyAction.Remove(action); 
            if (_onConnectionFromEmptyAction.Count == 0) {
                GetParent().Disconnect("connection_from_empty", this, nameof(_GodotSignalConnectionFromEmpty));
            }
            return this;
        }
        private void _GodotSignalConnectionFromEmpty(Vector2 release_position, string to, int to_slot) {
            if (_onConnectionFromEmptyAction == null || _onConnectionFromEmptyAction.Count == 0) return;
            for (var i = 0; i < _onConnectionFromEmptyAction.Count; i++) _onConnectionFromEmptyAction[i].Invoke(release_position, to, to_slot);
        }
        

        private List<Action<string, int, string, int>>? _onConnectionRequestAction; 
        public GraphEditAction OnConnectionRequest(Action<string, int, string, int> action, bool oneShot = false, bool deferred = false) {
            if (_onConnectionRequestAction == null || _onConnectionRequestAction.Count == 0) {
                _onConnectionRequestAction ??= new List<Action<string, int, string, int>>(); 
                GetParent().Connect("connection_request", this, nameof(_GodotSignalConnectionRequest));
            }
            _onConnectionRequestAction.Add(action);
            return this;
        }
        public GraphEditAction RemoveOnConnectionRequest(Action<string, int, string, int> action) {
            if (_onConnectionRequestAction == null || _onConnectionRequestAction.Count == 0) return this;
            _onConnectionRequestAction.Remove(action); 
            if (_onConnectionRequestAction.Count == 0) {
                GetParent().Disconnect("connection_request", this, nameof(_GodotSignalConnectionRequest));
            }
            return this;
        }
        private void _GodotSignalConnectionRequest(string from, int from_slot, string to, int to_slot) {
            if (_onConnectionRequestAction == null || _onConnectionRequestAction.Count == 0) return;
            for (var i = 0; i < _onConnectionRequestAction.Count; i++) _onConnectionRequestAction[i].Invoke(from, from_slot, to, to_slot);
        }
        

        private List<Action<string, int, Vector2>>? _onConnectionToEmptyAction; 
        public GraphEditAction OnConnectionToEmpty(Action<string, int, Vector2> action, bool oneShot = false, bool deferred = false) {
            if (_onConnectionToEmptyAction == null || _onConnectionToEmptyAction.Count == 0) {
                _onConnectionToEmptyAction ??= new List<Action<string, int, Vector2>>(); 
                GetParent().Connect("connection_to_empty", this, nameof(_GodotSignalConnectionToEmpty));
            }
            _onConnectionToEmptyAction.Add(action);
            return this;
        }
        public GraphEditAction RemoveOnConnectionToEmpty(Action<string, int, Vector2> action) {
            if (_onConnectionToEmptyAction == null || _onConnectionToEmptyAction.Count == 0) return this;
            _onConnectionToEmptyAction.Remove(action); 
            if (_onConnectionToEmptyAction.Count == 0) {
                GetParent().Disconnect("connection_to_empty", this, nameof(_GodotSignalConnectionToEmpty));
            }
            return this;
        }
        private void _GodotSignalConnectionToEmpty(string from, int from_slot, Vector2 release_position) {
            if (_onConnectionToEmptyAction == null || _onConnectionToEmptyAction.Count == 0) return;
            for (var i = 0; i < _onConnectionToEmptyAction.Count; i++) _onConnectionToEmptyAction[i].Invoke(from, from_slot, release_position);
        }
        

        private List<Action>? _onCopyNodesRequestAction; 
        public GraphEditAction OnCopyNodesRequest(Action action, bool oneShot = false, bool deferred = false) {
            if (_onCopyNodesRequestAction == null || _onCopyNodesRequestAction.Count == 0) {
                _onCopyNodesRequestAction ??= new List<Action>(); 
                GetParent().Connect("copy_nodes_request", this, nameof(_GodotSignalCopyNodesRequest));
            }
            _onCopyNodesRequestAction.Add(action);
            return this;
        }
        public GraphEditAction RemoveOnCopyNodesRequest(Action action) {
            if (_onCopyNodesRequestAction == null || _onCopyNodesRequestAction.Count == 0) return this;
            _onCopyNodesRequestAction.Remove(action); 
            if (_onCopyNodesRequestAction.Count == 0) {
                GetParent().Disconnect("copy_nodes_request", this, nameof(_GodotSignalCopyNodesRequest));
            }
            return this;
        }
        private void _GodotSignalCopyNodesRequest() {
            if (_onCopyNodesRequestAction == null || _onCopyNodesRequestAction.Count == 0) return;
            for (var i = 0; i < _onCopyNodesRequestAction.Count; i++) _onCopyNodesRequestAction[i].Invoke();
        }
        

        private List<Action>? _onDeleteNodesRequestAction; 
        public GraphEditAction OnDeleteNodesRequest(Action action, bool oneShot = false, bool deferred = false) {
            if (_onDeleteNodesRequestAction == null || _onDeleteNodesRequestAction.Count == 0) {
                _onDeleteNodesRequestAction ??= new List<Action>(); 
                GetParent().Connect("delete_nodes_request", this, nameof(_GodotSignalDeleteNodesRequest));
            }
            _onDeleteNodesRequestAction.Add(action);
            return this;
        }
        public GraphEditAction RemoveOnDeleteNodesRequest(Action action) {
            if (_onDeleteNodesRequestAction == null || _onDeleteNodesRequestAction.Count == 0) return this;
            _onDeleteNodesRequestAction.Remove(action); 
            if (_onDeleteNodesRequestAction.Count == 0) {
                GetParent().Disconnect("delete_nodes_request", this, nameof(_GodotSignalDeleteNodesRequest));
            }
            return this;
        }
        private void _GodotSignalDeleteNodesRequest() {
            if (_onDeleteNodesRequestAction == null || _onDeleteNodesRequestAction.Count == 0) return;
            for (var i = 0; i < _onDeleteNodesRequestAction.Count; i++) _onDeleteNodesRequestAction[i].Invoke();
        }
        

        private List<Action<string, int, string, int>>? _onDisconnectionRequestAction; 
        public GraphEditAction OnDisconnectionRequest(Action<string, int, string, int> action, bool oneShot = false, bool deferred = false) {
            if (_onDisconnectionRequestAction == null || _onDisconnectionRequestAction.Count == 0) {
                _onDisconnectionRequestAction ??= new List<Action<string, int, string, int>>(); 
                GetParent().Connect("disconnection_request", this, nameof(_GodotSignalDisconnectionRequest));
            }
            _onDisconnectionRequestAction.Add(action);
            return this;
        }
        public GraphEditAction RemoveOnDisconnectionRequest(Action<string, int, string, int> action) {
            if (_onDisconnectionRequestAction == null || _onDisconnectionRequestAction.Count == 0) return this;
            _onDisconnectionRequestAction.Remove(action); 
            if (_onDisconnectionRequestAction.Count == 0) {
                GetParent().Disconnect("disconnection_request", this, nameof(_GodotSignalDisconnectionRequest));
            }
            return this;
        }
        private void _GodotSignalDisconnectionRequest(string from, int from_slot, string to, int to_slot) {
            if (_onDisconnectionRequestAction == null || _onDisconnectionRequestAction.Count == 0) return;
            for (var i = 0; i < _onDisconnectionRequestAction.Count; i++) _onDisconnectionRequestAction[i].Invoke(from, from_slot, to, to_slot);
        }
        

        private List<Action>? _onDrawAction; 
        public GraphEditAction OnDraw(Action action, bool oneShot = false, bool deferred = false) {
            if (_onDrawAction == null || _onDrawAction.Count == 0) {
                _onDrawAction ??= new List<Action>(); 
                GetParent().Connect("draw", this, nameof(_GodotSignalDraw));
            }
            _onDrawAction.Add(action);
            return this;
        }
        public GraphEditAction RemoveOnDraw(Action action) {
            if (_onDrawAction == null || _onDrawAction.Count == 0) return this;
            _onDrawAction.Remove(action); 
            if (_onDrawAction.Count == 0) {
                GetParent().Disconnect("draw", this, nameof(_GodotSignalDraw));
            }
            return this;
        }
        private void _GodotSignalDraw() {
            if (_onDrawAction == null || _onDrawAction.Count == 0) return;
            for (var i = 0; i < _onDrawAction.Count; i++) _onDrawAction[i].Invoke();
        }
        

        private List<Action>? _onDuplicateNodesRequestAction; 
        public GraphEditAction OnDuplicateNodesRequest(Action action, bool oneShot = false, bool deferred = false) {
            if (_onDuplicateNodesRequestAction == null || _onDuplicateNodesRequestAction.Count == 0) {
                _onDuplicateNodesRequestAction ??= new List<Action>(); 
                GetParent().Connect("duplicate_nodes_request", this, nameof(_GodotSignalDuplicateNodesRequest));
            }
            _onDuplicateNodesRequestAction.Add(action);
            return this;
        }
        public GraphEditAction RemoveOnDuplicateNodesRequest(Action action) {
            if (_onDuplicateNodesRequestAction == null || _onDuplicateNodesRequestAction.Count == 0) return this;
            _onDuplicateNodesRequestAction.Remove(action); 
            if (_onDuplicateNodesRequestAction.Count == 0) {
                GetParent().Disconnect("duplicate_nodes_request", this, nameof(_GodotSignalDuplicateNodesRequest));
            }
            return this;
        }
        private void _GodotSignalDuplicateNodesRequest() {
            if (_onDuplicateNodesRequestAction == null || _onDuplicateNodesRequestAction.Count == 0) return;
            for (var i = 0; i < _onDuplicateNodesRequestAction.Count; i++) _onDuplicateNodesRequestAction[i].Invoke();
        }
        

        private List<Action>? _onEndNodeMoveAction; 
        public GraphEditAction OnEndNodeMove(Action action, bool oneShot = false, bool deferred = false) {
            if (_onEndNodeMoveAction == null || _onEndNodeMoveAction.Count == 0) {
                _onEndNodeMoveAction ??= new List<Action>(); 
                GetParent().Connect("_end_node_move", this, nameof(_GodotSignalEndNodeMove));
            }
            _onEndNodeMoveAction.Add(action);
            return this;
        }
        public GraphEditAction RemoveOnEndNodeMove(Action action) {
            if (_onEndNodeMoveAction == null || _onEndNodeMoveAction.Count == 0) return this;
            _onEndNodeMoveAction.Remove(action); 
            if (_onEndNodeMoveAction.Count == 0) {
                GetParent().Disconnect("_end_node_move", this, nameof(_GodotSignalEndNodeMove));
            }
            return this;
        }
        private void _GodotSignalEndNodeMove() {
            if (_onEndNodeMoveAction == null || _onEndNodeMoveAction.Count == 0) return;
            for (var i = 0; i < _onEndNodeMoveAction.Count; i++) _onEndNodeMoveAction[i].Invoke();
        }
        

        private List<Action>? _onFocusEnteredAction; 
        public GraphEditAction OnFocusEntered(Action action, bool oneShot = false, bool deferred = false) {
            if (_onFocusEnteredAction == null || _onFocusEnteredAction.Count == 0) {
                _onFocusEnteredAction ??= new List<Action>(); 
                GetParent().Connect("focus_entered", this, nameof(_GodotSignalFocusEntered));
            }
            _onFocusEnteredAction.Add(action);
            return this;
        }
        public GraphEditAction RemoveOnFocusEntered(Action action) {
            if (_onFocusEnteredAction == null || _onFocusEnteredAction.Count == 0) return this;
            _onFocusEnteredAction.Remove(action); 
            if (_onFocusEnteredAction.Count == 0) {
                GetParent().Disconnect("focus_entered", this, nameof(_GodotSignalFocusEntered));
            }
            return this;
        }
        private void _GodotSignalFocusEntered() {
            if (_onFocusEnteredAction == null || _onFocusEnteredAction.Count == 0) return;
            for (var i = 0; i < _onFocusEnteredAction.Count; i++) _onFocusEnteredAction[i].Invoke();
        }
        

        private List<Action>? _onFocusExitedAction; 
        public GraphEditAction OnFocusExited(Action action, bool oneShot = false, bool deferred = false) {
            if (_onFocusExitedAction == null || _onFocusExitedAction.Count == 0) {
                _onFocusExitedAction ??= new List<Action>(); 
                GetParent().Connect("focus_exited", this, nameof(_GodotSignalFocusExited));
            }
            _onFocusExitedAction.Add(action);
            return this;
        }
        public GraphEditAction RemoveOnFocusExited(Action action) {
            if (_onFocusExitedAction == null || _onFocusExitedAction.Count == 0) return this;
            _onFocusExitedAction.Remove(action); 
            if (_onFocusExitedAction.Count == 0) {
                GetParent().Disconnect("focus_exited", this, nameof(_GodotSignalFocusExited));
            }
            return this;
        }
        private void _GodotSignalFocusExited() {
            if (_onFocusExitedAction == null || _onFocusExitedAction.Count == 0) return;
            for (var i = 0; i < _onFocusExitedAction.Count; i++) _onFocusExitedAction[i].Invoke();
        }
        

        private List<Action<InputEvent>>? _onGuiInputAction; 
        public GraphEditAction OnGuiInput(Action<InputEvent> action, bool oneShot = false, bool deferred = false) {
            if (_onGuiInputAction == null || _onGuiInputAction.Count == 0) {
                _onGuiInputAction ??= new List<Action<InputEvent>>(); 
                GetParent().Connect("gui_input", this, nameof(_GodotSignalGuiInput));
            }
            _onGuiInputAction.Add(action);
            return this;
        }
        public GraphEditAction RemoveOnGuiInput(Action<InputEvent> action) {
            if (_onGuiInputAction == null || _onGuiInputAction.Count == 0) return this;
            _onGuiInputAction.Remove(action); 
            if (_onGuiInputAction.Count == 0) {
                GetParent().Disconnect("gui_input", this, nameof(_GodotSignalGuiInput));
            }
            return this;
        }
        private void _GodotSignalGuiInput(InputEvent @event) {
            if (_onGuiInputAction == null || _onGuiInputAction.Count == 0) return;
            for (var i = 0; i < _onGuiInputAction.Count; i++) _onGuiInputAction[i].Invoke(@event);
        }
        

        private List<Action>? _onHideAction; 
        public GraphEditAction OnHide(Action action, bool oneShot = false, bool deferred = false) {
            if (_onHideAction == null || _onHideAction.Count == 0) {
                _onHideAction ??= new List<Action>(); 
                GetParent().Connect("hide", this, nameof(_GodotSignalHide));
            }
            _onHideAction.Add(action);
            return this;
        }
        public GraphEditAction RemoveOnHide(Action action) {
            if (_onHideAction == null || _onHideAction.Count == 0) return this;
            _onHideAction.Remove(action); 
            if (_onHideAction.Count == 0) {
                GetParent().Disconnect("hide", this, nameof(_GodotSignalHide));
            }
            return this;
        }
        private void _GodotSignalHide() {
            if (_onHideAction == null || _onHideAction.Count == 0) return;
            for (var i = 0; i < _onHideAction.Count; i++) _onHideAction[i].Invoke();
        }
        

        private List<Action>? _onItemRectChangedAction; 
        public GraphEditAction OnItemRectChanged(Action action, bool oneShot = false, bool deferred = false) {
            if (_onItemRectChangedAction == null || _onItemRectChangedAction.Count == 0) {
                _onItemRectChangedAction ??= new List<Action>(); 
                GetParent().Connect("item_rect_changed", this, nameof(_GodotSignalItemRectChanged));
            }
            _onItemRectChangedAction.Add(action);
            return this;
        }
        public GraphEditAction RemoveOnItemRectChanged(Action action) {
            if (_onItemRectChangedAction == null || _onItemRectChangedAction.Count == 0) return this;
            _onItemRectChangedAction.Remove(action); 
            if (_onItemRectChangedAction.Count == 0) {
                GetParent().Disconnect("item_rect_changed", this, nameof(_GodotSignalItemRectChanged));
            }
            return this;
        }
        private void _GodotSignalItemRectChanged() {
            if (_onItemRectChangedAction == null || _onItemRectChangedAction.Count == 0) return;
            for (var i = 0; i < _onItemRectChangedAction.Count; i++) _onItemRectChangedAction[i].Invoke();
        }
        

        private List<Action>? _onMinimumSizeChangedAction; 
        public GraphEditAction OnMinimumSizeChanged(Action action, bool oneShot = false, bool deferred = false) {
            if (_onMinimumSizeChangedAction == null || _onMinimumSizeChangedAction.Count == 0) {
                _onMinimumSizeChangedAction ??= new List<Action>(); 
                GetParent().Connect("minimum_size_changed", this, nameof(_GodotSignalMinimumSizeChanged));
            }
            _onMinimumSizeChangedAction.Add(action);
            return this;
        }
        public GraphEditAction RemoveOnMinimumSizeChanged(Action action) {
            if (_onMinimumSizeChangedAction == null || _onMinimumSizeChangedAction.Count == 0) return this;
            _onMinimumSizeChangedAction.Remove(action); 
            if (_onMinimumSizeChangedAction.Count == 0) {
                GetParent().Disconnect("minimum_size_changed", this, nameof(_GodotSignalMinimumSizeChanged));
            }
            return this;
        }
        private void _GodotSignalMinimumSizeChanged() {
            if (_onMinimumSizeChangedAction == null || _onMinimumSizeChangedAction.Count == 0) return;
            for (var i = 0; i < _onMinimumSizeChangedAction.Count; i++) _onMinimumSizeChangedAction[i].Invoke();
        }
        

        private List<Action>? _onModalClosedAction; 
        public GraphEditAction OnModalClosed(Action action, bool oneShot = false, bool deferred = false) {
            if (_onModalClosedAction == null || _onModalClosedAction.Count == 0) {
                _onModalClosedAction ??= new List<Action>(); 
                GetParent().Connect("modal_closed", this, nameof(_GodotSignalModalClosed));
            }
            _onModalClosedAction.Add(action);
            return this;
        }
        public GraphEditAction RemoveOnModalClosed(Action action) {
            if (_onModalClosedAction == null || _onModalClosedAction.Count == 0) return this;
            _onModalClosedAction.Remove(action); 
            if (_onModalClosedAction.Count == 0) {
                GetParent().Disconnect("modal_closed", this, nameof(_GodotSignalModalClosed));
            }
            return this;
        }
        private void _GodotSignalModalClosed() {
            if (_onModalClosedAction == null || _onModalClosedAction.Count == 0) return;
            for (var i = 0; i < _onModalClosedAction.Count; i++) _onModalClosedAction[i].Invoke();
        }
        

        private List<Action>? _onMouseEnteredAction; 
        public GraphEditAction OnMouseEntered(Action action, bool oneShot = false, bool deferred = false) {
            if (_onMouseEnteredAction == null || _onMouseEnteredAction.Count == 0) {
                _onMouseEnteredAction ??= new List<Action>(); 
                GetParent().Connect("mouse_entered", this, nameof(_GodotSignalMouseEntered));
            }
            _onMouseEnteredAction.Add(action);
            return this;
        }
        public GraphEditAction RemoveOnMouseEntered(Action action) {
            if (_onMouseEnteredAction == null || _onMouseEnteredAction.Count == 0) return this;
            _onMouseEnteredAction.Remove(action); 
            if (_onMouseEnteredAction.Count == 0) {
                GetParent().Disconnect("mouse_entered", this, nameof(_GodotSignalMouseEntered));
            }
            return this;
        }
        private void _GodotSignalMouseEntered() {
            if (_onMouseEnteredAction == null || _onMouseEnteredAction.Count == 0) return;
            for (var i = 0; i < _onMouseEnteredAction.Count; i++) _onMouseEnteredAction[i].Invoke();
        }
        

        private List<Action>? _onMouseExitedAction; 
        public GraphEditAction OnMouseExited(Action action, bool oneShot = false, bool deferred = false) {
            if (_onMouseExitedAction == null || _onMouseExitedAction.Count == 0) {
                _onMouseExitedAction ??= new List<Action>(); 
                GetParent().Connect("mouse_exited", this, nameof(_GodotSignalMouseExited));
            }
            _onMouseExitedAction.Add(action);
            return this;
        }
        public GraphEditAction RemoveOnMouseExited(Action action) {
            if (_onMouseExitedAction == null || _onMouseExitedAction.Count == 0) return this;
            _onMouseExitedAction.Remove(action); 
            if (_onMouseExitedAction.Count == 0) {
                GetParent().Disconnect("mouse_exited", this, nameof(_GodotSignalMouseExited));
            }
            return this;
        }
        private void _GodotSignalMouseExited() {
            if (_onMouseExitedAction == null || _onMouseExitedAction.Count == 0) return;
            for (var i = 0; i < _onMouseExitedAction.Count; i++) _onMouseExitedAction[i].Invoke();
        }
        

        private List<Action<Node>>? _onNodeSelectedAction; 
        public GraphEditAction OnNodeSelected(Action<Node> action, bool oneShot = false, bool deferred = false) {
            if (_onNodeSelectedAction == null || _onNodeSelectedAction.Count == 0) {
                _onNodeSelectedAction ??= new List<Action<Node>>(); 
                GetParent().Connect("node_selected", this, nameof(_GodotSignalNodeSelected));
            }
            _onNodeSelectedAction.Add(action);
            return this;
        }
        public GraphEditAction RemoveOnNodeSelected(Action<Node> action) {
            if (_onNodeSelectedAction == null || _onNodeSelectedAction.Count == 0) return this;
            _onNodeSelectedAction.Remove(action); 
            if (_onNodeSelectedAction.Count == 0) {
                GetParent().Disconnect("node_selected", this, nameof(_GodotSignalNodeSelected));
            }
            return this;
        }
        private void _GodotSignalNodeSelected(Node node) {
            if (_onNodeSelectedAction == null || _onNodeSelectedAction.Count == 0) return;
            for (var i = 0; i < _onNodeSelectedAction.Count; i++) _onNodeSelectedAction[i].Invoke(node);
        }
        

        private List<Action<Node>>? _onNodeUnselectedAction; 
        public GraphEditAction OnNodeUnselected(Action<Node> action, bool oneShot = false, bool deferred = false) {
            if (_onNodeUnselectedAction == null || _onNodeUnselectedAction.Count == 0) {
                _onNodeUnselectedAction ??= new List<Action<Node>>(); 
                GetParent().Connect("node_unselected", this, nameof(_GodotSignalNodeUnselected));
            }
            _onNodeUnselectedAction.Add(action);
            return this;
        }
        public GraphEditAction RemoveOnNodeUnselected(Action<Node> action) {
            if (_onNodeUnselectedAction == null || _onNodeUnselectedAction.Count == 0) return this;
            _onNodeUnselectedAction.Remove(action); 
            if (_onNodeUnselectedAction.Count == 0) {
                GetParent().Disconnect("node_unselected", this, nameof(_GodotSignalNodeUnselected));
            }
            return this;
        }
        private void _GodotSignalNodeUnselected(Node node) {
            if (_onNodeUnselectedAction == null || _onNodeUnselectedAction.Count == 0) return;
            for (var i = 0; i < _onNodeUnselectedAction.Count; i++) _onNodeUnselectedAction[i].Invoke(node);
        }
        

        private List<Action>? _onPasteNodesRequestAction; 
        public GraphEditAction OnPasteNodesRequest(Action action, bool oneShot = false, bool deferred = false) {
            if (_onPasteNodesRequestAction == null || _onPasteNodesRequestAction.Count == 0) {
                _onPasteNodesRequestAction ??= new List<Action>(); 
                GetParent().Connect("paste_nodes_request", this, nameof(_GodotSignalPasteNodesRequest));
            }
            _onPasteNodesRequestAction.Add(action);
            return this;
        }
        public GraphEditAction RemoveOnPasteNodesRequest(Action action) {
            if (_onPasteNodesRequestAction == null || _onPasteNodesRequestAction.Count == 0) return this;
            _onPasteNodesRequestAction.Remove(action); 
            if (_onPasteNodesRequestAction.Count == 0) {
                GetParent().Disconnect("paste_nodes_request", this, nameof(_GodotSignalPasteNodesRequest));
            }
            return this;
        }
        private void _GodotSignalPasteNodesRequest() {
            if (_onPasteNodesRequestAction == null || _onPasteNodesRequestAction.Count == 0) return;
            for (var i = 0; i < _onPasteNodesRequestAction.Count; i++) _onPasteNodesRequestAction[i].Invoke();
        }
        

        private List<Action<Vector2>>? _onPopupRequestAction; 
        public GraphEditAction OnPopupRequest(Action<Vector2> action, bool oneShot = false, bool deferred = false) {
            if (_onPopupRequestAction == null || _onPopupRequestAction.Count == 0) {
                _onPopupRequestAction ??= new List<Action<Vector2>>(); 
                GetParent().Connect("popup_request", this, nameof(_GodotSignalPopupRequest));
            }
            _onPopupRequestAction.Add(action);
            return this;
        }
        public GraphEditAction RemoveOnPopupRequest(Action<Vector2> action) {
            if (_onPopupRequestAction == null || _onPopupRequestAction.Count == 0) return this;
            _onPopupRequestAction.Remove(action); 
            if (_onPopupRequestAction.Count == 0) {
                GetParent().Disconnect("popup_request", this, nameof(_GodotSignalPopupRequest));
            }
            return this;
        }
        private void _GodotSignalPopupRequest(Vector2 position) {
            if (_onPopupRequestAction == null || _onPopupRequestAction.Count == 0) return;
            for (var i = 0; i < _onPopupRequestAction.Count; i++) _onPopupRequestAction[i].Invoke(position);
        }
        

        private List<Action>? _onReadyAction; 
        public GraphEditAction OnReady(Action action, bool oneShot = false, bool deferred = false) {
            if (_onReadyAction == null || _onReadyAction.Count == 0) {
                _onReadyAction ??= new List<Action>(); 
                GetParent().Connect("ready", this, nameof(_GodotSignalReady));
            }
            _onReadyAction.Add(action);
            return this;
        }
        public GraphEditAction RemoveOnReady(Action action) {
            if (_onReadyAction == null || _onReadyAction.Count == 0) return this;
            _onReadyAction.Remove(action); 
            if (_onReadyAction.Count == 0) {
                GetParent().Disconnect("ready", this, nameof(_GodotSignalReady));
            }
            return this;
        }
        private void _GodotSignalReady() {
            if (_onReadyAction == null || _onReadyAction.Count == 0) return;
            for (var i = 0; i < _onReadyAction.Count; i++) _onReadyAction[i].Invoke();
        }
        

        private List<Action>? _onRenamedAction; 
        public GraphEditAction OnRenamed(Action action, bool oneShot = false, bool deferred = false) {
            if (_onRenamedAction == null || _onRenamedAction.Count == 0) {
                _onRenamedAction ??= new List<Action>(); 
                GetParent().Connect("renamed", this, nameof(_GodotSignalRenamed));
            }
            _onRenamedAction.Add(action);
            return this;
        }
        public GraphEditAction RemoveOnRenamed(Action action) {
            if (_onRenamedAction == null || _onRenamedAction.Count == 0) return this;
            _onRenamedAction.Remove(action); 
            if (_onRenamedAction.Count == 0) {
                GetParent().Disconnect("renamed", this, nameof(_GodotSignalRenamed));
            }
            return this;
        }
        private void _GodotSignalRenamed() {
            if (_onRenamedAction == null || _onRenamedAction.Count == 0) return;
            for (var i = 0; i < _onRenamedAction.Count; i++) _onRenamedAction[i].Invoke();
        }
        

        private List<Action>? _onResizedAction; 
        public GraphEditAction OnResized(Action action, bool oneShot = false, bool deferred = false) {
            if (_onResizedAction == null || _onResizedAction.Count == 0) {
                _onResizedAction ??= new List<Action>(); 
                GetParent().Connect("resized", this, nameof(_GodotSignalResized));
            }
            _onResizedAction.Add(action);
            return this;
        }
        public GraphEditAction RemoveOnResized(Action action) {
            if (_onResizedAction == null || _onResizedAction.Count == 0) return this;
            _onResizedAction.Remove(action); 
            if (_onResizedAction.Count == 0) {
                GetParent().Disconnect("resized", this, nameof(_GodotSignalResized));
            }
            return this;
        }
        private void _GodotSignalResized() {
            if (_onResizedAction == null || _onResizedAction.Count == 0) return;
            for (var i = 0; i < _onResizedAction.Count; i++) _onResizedAction[i].Invoke();
        }
        

        private List<Action>? _onScriptChangedAction; 
        public GraphEditAction OnScriptChanged(Action action, bool oneShot = false, bool deferred = false) {
            if (_onScriptChangedAction == null || _onScriptChangedAction.Count == 0) {
                _onScriptChangedAction ??= new List<Action>(); 
                GetParent().Connect("script_changed", this, nameof(_GodotSignalScriptChanged));
            }
            _onScriptChangedAction.Add(action);
            return this;
        }
        public GraphEditAction RemoveOnScriptChanged(Action action) {
            if (_onScriptChangedAction == null || _onScriptChangedAction.Count == 0) return this;
            _onScriptChangedAction.Remove(action); 
            if (_onScriptChangedAction.Count == 0) {
                GetParent().Disconnect("script_changed", this, nameof(_GodotSignalScriptChanged));
            }
            return this;
        }
        private void _GodotSignalScriptChanged() {
            if (_onScriptChangedAction == null || _onScriptChangedAction.Count == 0) return;
            for (var i = 0; i < _onScriptChangedAction.Count; i++) _onScriptChangedAction[i].Invoke();
        }
        

        private List<Action<Vector2>>? _onScrollOffsetChangedAction; 
        public GraphEditAction OnScrollOffsetChanged(Action<Vector2> action, bool oneShot = false, bool deferred = false) {
            if (_onScrollOffsetChangedAction == null || _onScrollOffsetChangedAction.Count == 0) {
                _onScrollOffsetChangedAction ??= new List<Action<Vector2>>(); 
                GetParent().Connect("scroll_offset_changed", this, nameof(_GodotSignalScrollOffsetChanged));
            }
            _onScrollOffsetChangedAction.Add(action);
            return this;
        }
        public GraphEditAction RemoveOnScrollOffsetChanged(Action<Vector2> action) {
            if (_onScrollOffsetChangedAction == null || _onScrollOffsetChangedAction.Count == 0) return this;
            _onScrollOffsetChangedAction.Remove(action); 
            if (_onScrollOffsetChangedAction.Count == 0) {
                GetParent().Disconnect("scroll_offset_changed", this, nameof(_GodotSignalScrollOffsetChanged));
            }
            return this;
        }
        private void _GodotSignalScrollOffsetChanged(Vector2 ofs) {
            if (_onScrollOffsetChangedAction == null || _onScrollOffsetChangedAction.Count == 0) return;
            for (var i = 0; i < _onScrollOffsetChangedAction.Count; i++) _onScrollOffsetChangedAction[i].Invoke(ofs);
        }
        

        private List<Action>? _onSizeFlagsChangedAction; 
        public GraphEditAction OnSizeFlagsChanged(Action action, bool oneShot = false, bool deferred = false) {
            if (_onSizeFlagsChangedAction == null || _onSizeFlagsChangedAction.Count == 0) {
                _onSizeFlagsChangedAction ??= new List<Action>(); 
                GetParent().Connect("size_flags_changed", this, nameof(_GodotSignalSizeFlagsChanged));
            }
            _onSizeFlagsChangedAction.Add(action);
            return this;
        }
        public GraphEditAction RemoveOnSizeFlagsChanged(Action action) {
            if (_onSizeFlagsChangedAction == null || _onSizeFlagsChangedAction.Count == 0) return this;
            _onSizeFlagsChangedAction.Remove(action); 
            if (_onSizeFlagsChangedAction.Count == 0) {
                GetParent().Disconnect("size_flags_changed", this, nameof(_GodotSignalSizeFlagsChanged));
            }
            return this;
        }
        private void _GodotSignalSizeFlagsChanged() {
            if (_onSizeFlagsChangedAction == null || _onSizeFlagsChangedAction.Count == 0) return;
            for (var i = 0; i < _onSizeFlagsChangedAction.Count; i++) _onSizeFlagsChangedAction[i].Invoke();
        }
        

        private List<Action>? _onTreeEnteredAction; 
        public GraphEditAction OnTreeEntered(Action action, bool oneShot = false, bool deferred = false) {
            if (_onTreeEnteredAction == null || _onTreeEnteredAction.Count == 0) {
                _onTreeEnteredAction ??= new List<Action>(); 
                GetParent().Connect("tree_entered", this, nameof(_GodotSignalTreeEntered));
            }
            _onTreeEnteredAction.Add(action);
            return this;
        }
        public GraphEditAction RemoveOnTreeEntered(Action action) {
            if (_onTreeEnteredAction == null || _onTreeEnteredAction.Count == 0) return this;
            _onTreeEnteredAction.Remove(action); 
            if (_onTreeEnteredAction.Count == 0) {
                GetParent().Disconnect("tree_entered", this, nameof(_GodotSignalTreeEntered));
            }
            return this;
        }
        private void _GodotSignalTreeEntered() {
            if (_onTreeEnteredAction == null || _onTreeEnteredAction.Count == 0) return;
            for (var i = 0; i < _onTreeEnteredAction.Count; i++) _onTreeEnteredAction[i].Invoke();
        }
        

        private List<Action>? _onTreeExitedAction; 
        public GraphEditAction OnTreeExited(Action action, bool oneShot = false, bool deferred = false) {
            if (_onTreeExitedAction == null || _onTreeExitedAction.Count == 0) {
                _onTreeExitedAction ??= new List<Action>(); 
                GetParent().Connect("tree_exited", this, nameof(_GodotSignalTreeExited));
            }
            _onTreeExitedAction.Add(action);
            return this;
        }
        public GraphEditAction RemoveOnTreeExited(Action action) {
            if (_onTreeExitedAction == null || _onTreeExitedAction.Count == 0) return this;
            _onTreeExitedAction.Remove(action); 
            if (_onTreeExitedAction.Count == 0) {
                GetParent().Disconnect("tree_exited", this, nameof(_GodotSignalTreeExited));
            }
            return this;
        }
        private void _GodotSignalTreeExited() {
            if (_onTreeExitedAction == null || _onTreeExitedAction.Count == 0) return;
            for (var i = 0; i < _onTreeExitedAction.Count; i++) _onTreeExitedAction[i].Invoke();
        }
        

        private List<Action>? _onTreeExitingAction; 
        public GraphEditAction OnTreeExiting(Action action, bool oneShot = false, bool deferred = false) {
            if (_onTreeExitingAction == null || _onTreeExitingAction.Count == 0) {
                _onTreeExitingAction ??= new List<Action>(); 
                GetParent().Connect("tree_exiting", this, nameof(_GodotSignalTreeExiting));
            }
            _onTreeExitingAction.Add(action);
            return this;
        }
        public GraphEditAction RemoveOnTreeExiting(Action action) {
            if (_onTreeExitingAction == null || _onTreeExitingAction.Count == 0) return this;
            _onTreeExitingAction.Remove(action); 
            if (_onTreeExitingAction.Count == 0) {
                GetParent().Disconnect("tree_exiting", this, nameof(_GodotSignalTreeExiting));
            }
            return this;
        }
        private void _GodotSignalTreeExiting() {
            if (_onTreeExitingAction == null || _onTreeExitingAction.Count == 0) return;
            for (var i = 0; i < _onTreeExitingAction.Count; i++) _onTreeExitingAction[i].Invoke();
        }
        

        private List<Action>? _onVisibilityChangedAction; 
        public GraphEditAction OnVisibilityChanged(Action action, bool oneShot = false, bool deferred = false) {
            if (_onVisibilityChangedAction == null || _onVisibilityChangedAction.Count == 0) {
                _onVisibilityChangedAction ??= new List<Action>(); 
                GetParent().Connect("visibility_changed", this, nameof(_GodotSignalVisibilityChanged));
            }
            _onVisibilityChangedAction.Add(action);
            return this;
        }
        public GraphEditAction RemoveOnVisibilityChanged(Action action) {
            if (_onVisibilityChangedAction == null || _onVisibilityChangedAction.Count == 0) return this;
            _onVisibilityChangedAction.Remove(action); 
            if (_onVisibilityChangedAction.Count == 0) {
                GetParent().Disconnect("visibility_changed", this, nameof(_GodotSignalVisibilityChanged));
            }
            return this;
        }
        private void _GodotSignalVisibilityChanged() {
            if (_onVisibilityChangedAction == null || _onVisibilityChangedAction.Count == 0) return;
            for (var i = 0; i < _onVisibilityChangedAction.Count; i++) _onVisibilityChangedAction[i].Invoke();
        }
        
    }
}