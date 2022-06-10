using System;
using Godot;
using Object = Godot.Object;

namespace Betauer {
    public static partial class SignalExtensions {

        public const string Sprite3D_FrameChangedSignal = "frame_changed"; 
        public static SignalHandler OnFrameChanged(this Sprite3D target, Action action) {
            return new SignalHandler(target, Sprite3D_FrameChangedSignal, action);
        }

        public const string VisualShaderNode_EditorRefreshRequestSignal = "editor_refresh_request"; 
        public static SignalHandler OnEditorRefreshRequest(this VisualShaderNode target, Action action) {
            return new SignalHandler(target, VisualShaderNode_EditorRefreshRequestSignal, action);
        }

        public const string Path_CurveChangedSignal = "curve_changed"; 
        public static SignalHandler OnCurveChanged(this Path target, Action action) {
            return new SignalHandler(target, Path_CurveChangedSignal, action);
        }

        public const string StyleBoxTexture_TextureChangedSignal = "texture_changed"; 
        public static SignalHandler OnTextureChanged(this StyleBoxTexture target, Action action) {
            return new SignalHandler(target, StyleBoxTexture_TextureChangedSignal, action);
        }

        public const string WebRTCPeerConnection_IceCandidateCreatedSignal = "ice_candidate_created"; 
        public static SignalHandler<string, int, string> OnIceCandidateCreated(this WebRTCPeerConnection target, Action<string, int, string> action) {
            return new SignalHandler<string, int, string>(target, WebRTCPeerConnection_IceCandidateCreatedSignal, action);
        }

        public const string WebRTCPeerConnection_SessionDescriptionCreatedSignal = "session_description_created"; 
        public static SignalHandler<string, string> OnSessionDescriptionCreated(this WebRTCPeerConnection target, Action<string, string> action) {
            return new SignalHandler<string, string>(target, WebRTCPeerConnection_SessionDescriptionCreatedSignal, action);
        }

        public const string WebRTCPeerConnection_DataChannelReceivedSignal = "data_channel_received"; 
        public static SignalHandler<Godot.Object> OnDataChannelReceived(this WebRTCPeerConnection target, Action<Godot.Object> action) {
            return new SignalHandler<Godot.Object>(target, WebRTCPeerConnection_DataChannelReceivedSignal, action);
        }

        public const string ARVRController_MeshUpdatedSignal = "mesh_updated"; 
        public static SignalHandler<Mesh> OnMeshUpdated(this ARVRController target, Action<Mesh> action) {
            return new SignalHandler<Mesh>(target, ARVRController_MeshUpdatedSignal, action);
        }

        public const string ARVRController_ButtonReleaseSignal = "button_release"; 
        public static SignalHandler<int> OnButtonRelease(this ARVRController target, Action<int> action) {
            return new SignalHandler<int>(target, ARVRController_ButtonReleaseSignal, action);
        }

        public const string ARVRController_ButtonPressedSignal = "button_pressed"; 
        public static SignalHandler<int> OnButtonPressed(this ARVRController target, Action<int> action) {
            return new SignalHandler<int>(target, ARVRController_ButtonPressedSignal, action);
        }

        public const string ARVRAnchor_MeshUpdatedSignal = "mesh_updated"; 
        public static SignalHandler<Mesh> OnMeshUpdated(this ARVRAnchor target, Action<Mesh> action) {
            return new SignalHandler<Mesh>(target, ARVRAnchor_MeshUpdatedSignal, action);
        }

        public const string Curve_RangeChangedSignal = "range_changed"; 
        public static SignalHandler OnRangeChanged(this Curve target, Action action) {
            return new SignalHandler(target, Curve_RangeChangedSignal, action);
        }

        public const string Tween_TweenStepSignal = "tween_step"; 
        public static SignalHandler<Godot.Object, NodePath, float, Godot.Object> OnTweenStep(this Tween target, Action<Godot.Object, NodePath, float, Godot.Object> action) {
            return new SignalHandler<Godot.Object, NodePath, float, Godot.Object>(target, Tween_TweenStepSignal, action);
        }

        public const string Tween_TweenAllCompletedSignal = "tween_all_completed"; 
        public static SignalHandler OnTweenAllCompleted(this Tween target, Action action) {
            return new SignalHandler(target, Tween_TweenAllCompletedSignal, action);
        }

        public const string Tween_TweenCompletedSignal = "tween_completed"; 
        public static SignalHandler<Godot.Object, NodePath> OnTweenCompleted(this Tween target, Action<Godot.Object, NodePath> action) {
            return new SignalHandler<Godot.Object, NodePath>(target, Tween_TweenCompletedSignal, action);
        }

        public const string Tween_TweenStartedSignal = "tween_started"; 
        public static SignalHandler<Godot.Object, NodePath> OnTweenStarted(this Tween target, Action<Godot.Object, NodePath> action) {
            return new SignalHandler<Godot.Object, NodePath>(target, Tween_TweenStartedSignal, action);
        }

        public const string CollisionObject_MouseExitedSignal = "mouse_exited"; 
        public static SignalHandler OnMouseExited(this CollisionObject target, Action action) {
            return new SignalHandler(target, CollisionObject_MouseExitedSignal, action);
        }

        public const string CollisionObject_MouseEnteredSignal = "mouse_entered"; 
        public static SignalHandler OnMouseEntered(this CollisionObject target, Action action) {
            return new SignalHandler(target, CollisionObject_MouseEnteredSignal, action);
        }

        public const string CollisionObject_InputEventSignal = "input_event"; 
        public static SignalHandler<Node, InputEvent, Vector3, Vector3, int> OnInputEvent(this CollisionObject target, Action<Node, InputEvent, Vector3, Vector3, int> action) {
            return new SignalHandler<Node, InputEvent, Vector3, Vector3, int>(target, CollisionObject_InputEventSignal, action);
        }

        public const string AnimationNodeBlendSpace2D_TrianglesUpdatedSignal = "triangles_updated"; 
        public static SignalHandler OnTrianglesUpdated(this AnimationNodeBlendSpace2D target, Action action) {
            return new SignalHandler(target, AnimationNodeBlendSpace2D_TrianglesUpdatedSignal, action);
        }

        public const string RigidBody_BodyEnteredSignal = "body_entered"; 
        public static SignalHandler<Node> OnBodyEntered(this RigidBody target, Action<Node> action) {
            return new SignalHandler<Node>(target, RigidBody_BodyEnteredSignal, action);
        }

        public const string RigidBody_BodyShapeEnteredSignal = "body_shape_entered"; 
        public static SignalHandler<RID, Node, int, int> OnBodyShapeEntered(this RigidBody target, Action<RID, Node, int, int> action) {
            return new SignalHandler<RID, Node, int, int>(target, RigidBody_BodyShapeEnteredSignal, action);
        }

        public const string RigidBody_SleepingStateChangedSignal = "sleeping_state_changed"; 
        public static SignalHandler OnSleepingStateChanged(this RigidBody target, Action action) {
            return new SignalHandler(target, RigidBody_SleepingStateChangedSignal, action);
        }

        public const string RigidBody_BodyExitedSignal = "body_exited"; 
        public static SignalHandler<Node> OnBodyExited(this RigidBody target, Action<Node> action) {
            return new SignalHandler<Node>(target, RigidBody_BodyExitedSignal, action);
        }

        public const string RigidBody_BodyShapeExitedSignal = "body_shape_exited"; 
        public static SignalHandler<RID, Node, int, int> OnBodyShapeExited(this RigidBody target, Action<RID, Node, int, int> action) {
            return new SignalHandler<RID, Node, int, int>(target, RigidBody_BodyShapeExitedSignal, action);
        }

        public const string ProximityGroup_BroadcastSignal = "broadcast"; 
        public static SignalHandler<string, Godot.Collections.Array> OnBroadcast(this ProximityGroup target, Action<string, Godot.Collections.Array> action) {
            return new SignalHandler<string, Godot.Collections.Array>(target, ProximityGroup_BroadcastSignal, action);
        }

        public const string GraphEdit_DeleteNodesRequestSignal = "delete_nodes_request"; 
        public static SignalHandler OnDeleteNodesRequest(this GraphEdit target, Action action) {
            return new SignalHandler(target, GraphEdit_DeleteNodesRequestSignal, action);
        }

        public const string GraphEdit_CopyNodesRequestSignal = "copy_nodes_request"; 
        public static SignalHandler OnCopyNodesRequest(this GraphEdit target, Action action) {
            return new SignalHandler(target, GraphEdit_CopyNodesRequestSignal, action);
        }

        public const string GraphEdit_DuplicateNodesRequestSignal = "duplicate_nodes_request"; 
        public static SignalHandler OnDuplicateNodesRequest(this GraphEdit target, Action action) {
            return new SignalHandler(target, GraphEdit_DuplicateNodesRequestSignal, action);
        }

        public const string GraphEdit_PopupRequestSignal = "popup_request"; 
        public static SignalHandler<Vector2> OnPopupRequest(this GraphEdit target, Action<Vector2> action) {
            return new SignalHandler<Vector2>(target, GraphEdit_PopupRequestSignal, action);
        }

        public const string GraphEdit_PasteNodesRequestSignal = "paste_nodes_request"; 
        public static SignalHandler OnPasteNodesRequest(this GraphEdit target, Action action) {
            return new SignalHandler(target, GraphEdit_PasteNodesRequestSignal, action);
        }

        public const string GraphEdit_ScrollOffsetChangedSignal = "scroll_offset_changed"; 
        public static SignalHandler<Vector2> OnScrollOffsetChanged(this GraphEdit target, Action<Vector2> action) {
            return new SignalHandler<Vector2>(target, GraphEdit_ScrollOffsetChangedSignal, action);
        }

        public const string GraphEdit_NodeSelectedSignal = "node_selected"; 
        public static SignalHandler<Node> OnNodeSelected(this GraphEdit target, Action<Node> action) {
            return new SignalHandler<Node>(target, GraphEdit_NodeSelectedSignal, action);
        }

        public const string GraphEdit_BeginNodeMoveSignal = "_begin_node_move"; 
        public static SignalHandler OnBeginNodeMove(this GraphEdit target, Action action) {
            return new SignalHandler(target, GraphEdit_BeginNodeMoveSignal, action);
        }

        public const string GraphEdit_ConnectionToEmptySignal = "connection_to_empty"; 
        public static SignalHandler<string, int, Vector2> OnConnectionToEmpty(this GraphEdit target, Action<string, int, Vector2> action) {
            return new SignalHandler<string, int, Vector2>(target, GraphEdit_ConnectionToEmptySignal, action);
        }

        public const string GraphEdit_DisconnectionRequestSignal = "disconnection_request"; 
        public static SignalHandler<string, int, string, int> OnDisconnectionRequest(this GraphEdit target, Action<string, int, string, int> action) {
            return new SignalHandler<string, int, string, int>(target, GraphEdit_DisconnectionRequestSignal, action);
        }

        public const string GraphEdit_ConnectionRequestSignal = "connection_request"; 
        public static SignalHandler<string, int, string, int> OnConnectionRequest(this GraphEdit target, Action<string, int, string, int> action) {
            return new SignalHandler<string, int, string, int>(target, GraphEdit_ConnectionRequestSignal, action);
        }

        public const string GraphEdit_EndNodeMoveSignal = "_end_node_move"; 
        public static SignalHandler OnEndNodeMove(this GraphEdit target, Action action) {
            return new SignalHandler(target, GraphEdit_EndNodeMoveSignal, action);
        }

        public const string GraphEdit_ConnectionFromEmptySignal = "connection_from_empty"; 
        public static SignalHandler<string, int, Vector2> OnConnectionFromEmpty(this GraphEdit target, Action<string, int, Vector2> action) {
            return new SignalHandler<string, int, Vector2>(target, GraphEdit_ConnectionFromEmptySignal, action);
        }

        public const string GraphEdit_NodeUnselectedSignal = "node_unselected"; 
        public static SignalHandler<Node> OnNodeUnselected(this GraphEdit target, Action<Node> action) {
            return new SignalHandler<Node>(target, GraphEdit_NodeUnselectedSignal, action);
        }

        public const string AnimationPlayer_CachesClearedSignal = "caches_cleared"; 
        public static SignalHandler OnCachesCleared(this AnimationPlayer target, Action action) {
            return new SignalHandler(target, AnimationPlayer_CachesClearedSignal, action);
        }

        public const string AnimationPlayer_AnimationStartedSignal = "animation_started"; 
        public static SignalHandler<string> OnAnimationStarted(this AnimationPlayer target, Action<string> action) {
            return new SignalHandler<string>(target, AnimationPlayer_AnimationStartedSignal, action);
        }

        public const string AnimationPlayer_AnimationChangedSignal = "animation_changed"; 
        public static SignalHandler<string, string> OnAnimationChanged(this AnimationPlayer target, Action<string, string> action) {
            return new SignalHandler<string, string>(target, AnimationPlayer_AnimationChangedSignal, action);
        }

        public const string AnimationPlayer_AnimationFinishedSignal = "animation_finished"; 
        public static SignalHandler<string> OnAnimationFinished(this AnimationPlayer target, Action<string> action) {
            return new SignalHandler<string>(target, AnimationPlayer_AnimationFinishedSignal, action);
        }

        public const string AnimationNode_RemovedFromGraphSignal = "removed_from_graph"; 
        public static SignalHandler OnRemovedFromGraph(this AnimationNode target, Action action) {
            return new SignalHandler(target, AnimationNode_RemovedFromGraphSignal, action);
        }

        public const string AnimationNode_TreeChangedSignal = "tree_changed"; 
        public static SignalHandler OnTreeChanged(this AnimationNode target, Action action) {
            return new SignalHandler(target, AnimationNode_TreeChangedSignal, action);
        }

        public const string AnimatedSprite3D_FrameChangedSignal = "frame_changed"; 
        public static SignalHandler OnFrameChanged(this AnimatedSprite3D target, Action action) {
            return new SignalHandler(target, AnimatedSprite3D_FrameChangedSignal, action);
        }

        public const string AnimatedSprite3D_AnimationFinishedSignal = "animation_finished"; 
        public static SignalHandler OnAnimationFinished(this AnimatedSprite3D target, Action action) {
            return new SignalHandler(target, AnimatedSprite3D_AnimationFinishedSignal, action);
        }

        public const string PopupMenu_IndexPressedSignal = "index_pressed"; 
        public static SignalHandler<int> OnIndexPressed(this PopupMenu target, Action<int> action) {
            return new SignalHandler<int>(target, PopupMenu_IndexPressedSignal, action);
        }

        public const string PopupMenu_IdFocusedSignal = "id_focused"; 
        public static SignalHandler<int> OnIdFocused(this PopupMenu target, Action<int> action) {
            return new SignalHandler<int>(target, PopupMenu_IdFocusedSignal, action);
        }

        public const string PopupMenu_IdPressedSignal = "id_pressed"; 
        public static SignalHandler<int> OnIdPressed(this PopupMenu target, Action<int> action) {
            return new SignalHandler<int>(target, PopupMenu_IdPressedSignal, action);
        }

        public const string ColorPickerButton_PickerCreatedSignal = "picker_created"; 
        public static SignalHandler OnPickerCreated(this ColorPickerButton target, Action action) {
            return new SignalHandler(target, ColorPickerButton_PickerCreatedSignal, action);
        }

        public const string ColorPickerButton_PopupClosedSignal = "popup_closed"; 
        public static SignalHandler OnPopupClosed(this ColorPickerButton target, Action action) {
            return new SignalHandler(target, ColorPickerButton_PopupClosedSignal, action);
        }

        public const string ColorPickerButton_ColorChangedSignal = "color_changed"; 
        public static SignalHandler<Color> OnColorChanged(this ColorPickerButton target, Action<Color> action) {
            return new SignalHandler<Color>(target, ColorPickerButton_ColorChangedSignal, action);
        }

        public const string RichTextLabel_MetaClickedSignal = "meta_clicked"; 
        public static SignalHandler<object> OnMetaClicked(this RichTextLabel target, Action<object> action) {
            return new SignalHandler<object>(target, RichTextLabel_MetaClickedSignal, action);
        }

        public const string RichTextLabel_MetaHoverStartedSignal = "meta_hover_started"; 
        public static SignalHandler<object> OnMetaHoverStarted(this RichTextLabel target, Action<object> action) {
            return new SignalHandler<object>(target, RichTextLabel_MetaHoverStartedSignal, action);
        }

        public const string RichTextLabel_MetaHoverEndedSignal = "meta_hover_ended"; 
        public static SignalHandler<object> OnMetaHoverEnded(this RichTextLabel target, Action<object> action) {
            return new SignalHandler<object>(target, RichTextLabel_MetaHoverEndedSignal, action);
        }

        public const string Skeleton_SkeletonUpdatedSignal = "skeleton_updated"; 
        public static SignalHandler OnSkeletonUpdated(this Skeleton target, Action action) {
            return new SignalHandler(target, Skeleton_SkeletonUpdatedSignal, action);
        }

        public const string Tree_ItemActivatedSignal = "item_activated"; 
        public static SignalHandler OnItemActivated(this Tree target, Action action) {
            return new SignalHandler(target, Tree_ItemActivatedSignal, action);
        }

        public const string Tree_MultiSelectedSignal = "multi_selected"; 
        public static SignalHandler<TreeItem, int, bool> OnMultiSelected(this Tree target, Action<TreeItem, int, bool> action) {
            return new SignalHandler<TreeItem, int, bool>(target, Tree_MultiSelectedSignal, action);
        }

        public const string Tree_ColumnTitlePressedSignal = "column_title_pressed"; 
        public static SignalHandler<int> OnColumnTitlePressed(this Tree target, Action<int> action) {
            return new SignalHandler<int>(target, Tree_ColumnTitlePressedSignal, action);
        }

        public const string Tree_CustomPopupEditedSignal = "custom_popup_edited"; 
        public static SignalHandler<bool> OnCustomPopupEdited(this Tree target, Action<bool> action) {
            return new SignalHandler<bool>(target, Tree_CustomPopupEditedSignal, action);
        }

        public const string Tree_ItemCollapsedSignal = "item_collapsed"; 
        public static SignalHandler<TreeItem> OnItemCollapsed(this Tree target, Action<TreeItem> action) {
            return new SignalHandler<TreeItem>(target, Tree_ItemCollapsedSignal, action);
        }

        public const string Tree_ItemRmbEditedSignal = "item_rmb_edited"; 
        public static SignalHandler OnItemRmbEdited(this Tree target, Action action) {
            return new SignalHandler(target, Tree_ItemRmbEditedSignal, action);
        }

        public const string Tree_ItemEditedSignal = "item_edited"; 
        public static SignalHandler OnItemEdited(this Tree target, Action action) {
            return new SignalHandler(target, Tree_ItemEditedSignal, action);
        }

        public const string Tree_EmptyTreeRmbSelectedSignal = "empty_tree_rmb_selected"; 
        public static SignalHandler<Vector2> OnEmptyTreeRmbSelected(this Tree target, Action<Vector2> action) {
            return new SignalHandler<Vector2>(target, Tree_EmptyTreeRmbSelectedSignal, action);
        }

        public const string Tree_NothingSelectedSignal = "nothing_selected"; 
        public static SignalHandler OnNothingSelected(this Tree target, Action action) {
            return new SignalHandler(target, Tree_NothingSelectedSignal, action);
        }

        public const string Tree_ItemDoubleClickedSignal = "item_double_clicked"; 
        public static SignalHandler OnItemDoubleClicked(this Tree target, Action action) {
            return new SignalHandler(target, Tree_ItemDoubleClickedSignal, action);
        }

        public const string Tree_EmptyRmbSignal = "empty_rmb"; 
        public static SignalHandler<Vector2> OnEmptyRmb(this Tree target, Action<Vector2> action) {
            return new SignalHandler<Vector2>(target, Tree_EmptyRmbSignal, action);
        }

        public const string Tree_ItemRmbSelectedSignal = "item_rmb_selected"; 
        public static SignalHandler<Vector2> OnItemRmbSelected(this Tree target, Action<Vector2> action) {
            return new SignalHandler<Vector2>(target, Tree_ItemRmbSelectedSignal, action);
        }

        public const string Tree_ItemSelectedSignal = "item_selected"; 
        public static SignalHandler OnItemSelected(this Tree target, Action action) {
            return new SignalHandler(target, Tree_ItemSelectedSignal, action);
        }

        public const string Tree_CellSelectedSignal = "cell_selected"; 
        public static SignalHandler OnCellSelected(this Tree target, Action action) {
            return new SignalHandler(target, Tree_CellSelectedSignal, action);
        }

        public const string Tree_ButtonPressedSignal = "button_pressed"; 
        public static SignalHandler<TreeItem, int, int> OnButtonPressed(this Tree target, Action<TreeItem, int, int> action) {
            return new SignalHandler<TreeItem, int, int>(target, Tree_ButtonPressedSignal, action);
        }

        public const string Tree_ItemCustomButtonPressedSignal = "item_custom_button_pressed"; 
        public static SignalHandler OnItemCustomButtonPressed(this Tree target, Action action) {
            return new SignalHandler(target, Tree_ItemCustomButtonPressedSignal, action);
        }

        public const string TextEdit_BreakpointToggledSignal = "breakpoint_toggled"; 
        public static SignalHandler<int> OnBreakpointToggled(this TextEdit target, Action<int> action) {
            return new SignalHandler<int>(target, TextEdit_BreakpointToggledSignal, action);
        }

        public const string TextEdit_TextChangedSignal = "text_changed"; 
        public static SignalHandler OnTextChanged(this TextEdit target, Action action) {
            return new SignalHandler(target, TextEdit_TextChangedSignal, action);
        }

        public const string TextEdit_SymbolLookupSignal = "symbol_lookup"; 
        public static SignalHandler<string, int, int> OnSymbolLookup(this TextEdit target, Action<string, int, int> action) {
            return new SignalHandler<string, int, int>(target, TextEdit_SymbolLookupSignal, action);
        }

        public const string TextEdit_CursorChangedSignal = "cursor_changed"; 
        public static SignalHandler OnCursorChanged(this TextEdit target, Action action) {
            return new SignalHandler(target, TextEdit_CursorChangedSignal, action);
        }

        public const string TextEdit_InfoClickedSignal = "info_clicked"; 
        public static SignalHandler<int, string> OnInfoClicked(this TextEdit target, Action<int, string> action) {
            return new SignalHandler<int, string>(target, TextEdit_InfoClickedSignal, action);
        }

        public const string TextEdit_RequestCompletionSignal = "request_completion"; 
        public static SignalHandler OnRequestCompletion(this TextEdit target, Action action) {
            return new SignalHandler(target, TextEdit_RequestCompletionSignal, action);
        }

        public const string SplitContainer_DraggedSignal = "dragged"; 
        public static SignalHandler<int> OnDragged(this SplitContainer target, Action<int> action) {
            return new SignalHandler<int>(target, SplitContainer_DraggedSignal, action);
        }

        public const string Spatial_GameplayEnteredSignal = "gameplay_entered"; 
        public static SignalHandler OnGameplayEntered(this Spatial target, Action action) {
            return new SignalHandler(target, Spatial_GameplayEnteredSignal, action);
        }

        public const string Spatial_VisibilityChangedSignal = "visibility_changed"; 
        public static SignalHandler OnVisibilityChanged(this Spatial target, Action action) {
            return new SignalHandler(target, Spatial_VisibilityChangedSignal, action);
        }

        public const string Spatial_GameplayExitedSignal = "gameplay_exited"; 
        public static SignalHandler OnGameplayExited(this Spatial target, Action action) {
            return new SignalHandler(target, Spatial_GameplayExitedSignal, action);
        }

        public const string VideoPlayer_FinishedSignal = "finished"; 
        public static SignalHandler OnFinished(this VideoPlayer target, Action action) {
            return new SignalHandler(target, VideoPlayer_FinishedSignal, action);
        }

        public const string OptionButton_ItemFocusedSignal = "item_focused"; 
        public static SignalHandler<int> OnItemFocused(this OptionButton target, Action<int> action) {
            return new SignalHandler<int>(target, OptionButton_ItemFocusedSignal, action);
        }

        public const string OptionButton_ItemSelectedSignal = "item_selected"; 
        public static SignalHandler<int> OnItemSelected(this OptionButton target, Action<int> action) {
            return new SignalHandler<int>(target, OptionButton_ItemSelectedSignal, action);
        }

        public const string VisualShaderNodeInput_InputTypeChangedSignal = "input_type_changed"; 
        public static SignalHandler OnInputTypeChanged(this VisualShaderNodeInput target, Action action) {
            return new SignalHandler(target, VisualShaderNodeInput_InputTypeChangedSignal, action);
        }

        public const string Range_ValueChangedSignal = "value_changed"; 
        public static SignalHandler<float> OnValueChanged(this Range target, Action<float> action) {
            return new SignalHandler<float>(target, Range_ValueChangedSignal, action);
        }

        public const string Range_ChangedSignal = "changed"; 
        public static SignalHandler OnChanged(this Range target, Action action) {
            return new SignalHandler(target, Range_ChangedSignal, action);
        }

        public const string MenuButton_AboutToShowSignal = "about_to_show"; 
        public static SignalHandler OnAboutToShow(this MenuButton target, Action action) {
            return new SignalHandler(target, MenuButton_AboutToShowSignal, action);
        }

        public const string BaseButton_ButtonDownSignal = "button_down"; 
        public static SignalHandler OnButtonDown(this BaseButton target, Action action) {
            return new SignalHandler(target, BaseButton_ButtonDownSignal, action);
        }

        public const string BaseButton_ToggledSignal = "toggled"; 
        public static SignalHandler<bool> OnToggled(this BaseButton target, Action<bool> action) {
            return new SignalHandler<bool>(target, BaseButton_ToggledSignal, action);
        }

        public const string BaseButton_PressedSignal = "pressed"; 
        public static SignalHandler OnPressed(this BaseButton target, Action action) {
            return new SignalHandler(target, BaseButton_PressedSignal, action);
        }

        public const string BaseButton_ButtonUpSignal = "button_up"; 
        public static SignalHandler OnButtonUp(this BaseButton target, Action action) {
            return new SignalHandler(target, BaseButton_ButtonUpSignal, action);
        }

        public const string ItemList_ItemActivatedSignal = "item_activated"; 
        public static SignalHandler<int> OnItemActivated(this ItemList target, Action<int> action) {
            return new SignalHandler<int>(target, ItemList_ItemActivatedSignal, action);
        }

        public const string ItemList_MultiSelectedSignal = "multi_selected"; 
        public static SignalHandler<int, bool> OnMultiSelected(this ItemList target, Action<int, bool> action) {
            return new SignalHandler<int, bool>(target, ItemList_MultiSelectedSignal, action);
        }

        public const string ItemList_NothingSelectedSignal = "nothing_selected"; 
        public static SignalHandler OnNothingSelected(this ItemList target, Action action) {
            return new SignalHandler(target, ItemList_NothingSelectedSignal, action);
        }

        public const string ItemList_RmbClickedSignal = "rmb_clicked"; 
        public static SignalHandler<Vector2> OnRmbClicked(this ItemList target, Action<Vector2> action) {
            return new SignalHandler<Vector2>(target, ItemList_RmbClickedSignal, action);
        }

        public const string ItemList_ItemRmbSelectedSignal = "item_rmb_selected"; 
        public static SignalHandler<int, Vector2> OnItemRmbSelected(this ItemList target, Action<int, Vector2> action) {
            return new SignalHandler<int, Vector2>(target, ItemList_ItemRmbSelectedSignal, action);
        }

        public const string ItemList_ItemSelectedSignal = "item_selected"; 
        public static SignalHandler<int> OnItemSelected(this ItemList target, Action<int> action) {
            return new SignalHandler<int>(target, ItemList_ItemSelectedSignal, action);
        }

        public const string ColorPicker_PresetRemovedSignal = "preset_removed"; 
        public static SignalHandler<Color> OnPresetRemoved(this ColorPicker target, Action<Color> action) {
            return new SignalHandler<Color>(target, ColorPicker_PresetRemovedSignal, action);
        }

        public const string ColorPicker_PresetAddedSignal = "preset_added"; 
        public static SignalHandler<Color> OnPresetAdded(this ColorPicker target, Action<Color> action) {
            return new SignalHandler<Color>(target, ColorPicker_PresetAddedSignal, action);
        }

        public const string ColorPicker_ColorChangedSignal = "color_changed"; 
        public static SignalHandler<Color> OnColorChanged(this ColorPicker target, Action<Color> action) {
            return new SignalHandler<Color>(target, ColorPicker_ColorChangedSignal, action);
        }

        public const string GraphNode_RaiseRequestSignal = "raise_request"; 
        public static SignalHandler OnRaiseRequest(this GraphNode target, Action action) {
            return new SignalHandler(target, GraphNode_RaiseRequestSignal, action);
        }

        public const string GraphNode_CloseRequestSignal = "close_request"; 
        public static SignalHandler OnCloseRequest(this GraphNode target, Action action) {
            return new SignalHandler(target, GraphNode_CloseRequestSignal, action);
        }

        public const string GraphNode_DraggedSignal = "dragged"; 
        public static SignalHandler<Vector2, Vector2> OnDragged(this GraphNode target, Action<Vector2, Vector2> action) {
            return new SignalHandler<Vector2, Vector2>(target, GraphNode_DraggedSignal, action);
        }

        public const string GraphNode_SlotUpdatedSignal = "slot_updated"; 
        public static SignalHandler<int> OnSlotUpdated(this GraphNode target, Action<int> action) {
            return new SignalHandler<int>(target, GraphNode_SlotUpdatedSignal, action);
        }

        public const string GraphNode_OffsetChangedSignal = "offset_changed"; 
        public static SignalHandler OnOffsetChanged(this GraphNode target, Action action) {
            return new SignalHandler(target, GraphNode_OffsetChangedSignal, action);
        }

        public const string GraphNode_ResizeRequestSignal = "resize_request"; 
        public static SignalHandler<Vector2> OnResizeRequest(this GraphNode target, Action<Vector2> action) {
            return new SignalHandler<Vector2>(target, GraphNode_ResizeRequestSignal, action);
        }

        public const string ButtonGroup_PressedSignal = "pressed"; 
        public static SignalHandler<Godot.Object> OnPressed(this ButtonGroup target, Action<Godot.Object> action) {
            return new SignalHandler<Godot.Object>(target, ButtonGroup_PressedSignal, action);
        }

        public const string ScrollBar_ScrollingSignal = "scrolling"; 
        public static SignalHandler OnScrolling(this ScrollBar target, Action action) {
            return new SignalHandler(target, ScrollBar_ScrollingSignal, action);
        }

        public const string Popup_PopupHideSignal = "popup_hide"; 
        public static SignalHandler OnPopupHide(this Popup target, Action action) {
            return new SignalHandler(target, Popup_PopupHideSignal, action);
        }

        public const string Popup_AboutToShowSignal = "about_to_show"; 
        public static SignalHandler OnAboutToShow(this Popup target, Action action) {
            return new SignalHandler(target, Popup_AboutToShowSignal, action);
        }

        public const string AnimationNodeStateMachineTransition_AdvanceConditionChangedSignal = "advance_condition_changed"; 
        public static SignalHandler OnAdvanceConditionChanged(this AnimationNodeStateMachineTransition target, Action action) {
            return new SignalHandler(target, AnimationNodeStateMachineTransition_AdvanceConditionChangedSignal, action);
        }

        public const string Control_MouseExitedSignal = "mouse_exited"; 
        public static SignalHandler OnMouseExited(this Control target, Action action) {
            return new SignalHandler(target, Control_MouseExitedSignal, action);
        }

        public const string Control_GuiInputSignal = "gui_input"; 
        public static SignalHandler<InputEvent> OnGuiInput(this Control target, Action<InputEvent> action) {
            return new SignalHandler<InputEvent>(target, Control_GuiInputSignal, action);
        }

        public const string Control_ModalClosedSignal = "modal_closed"; 
        public static SignalHandler OnModalClosed(this Control target, Action action) {
            return new SignalHandler(target, Control_ModalClosedSignal, action);
        }

        public const string Control_FocusEnteredSignal = "focus_entered"; 
        public static SignalHandler OnFocusEntered(this Control target, Action action) {
            return new SignalHandler(target, Control_FocusEnteredSignal, action);
        }

        public const string Control_ResizedSignal = "resized"; 
        public static SignalHandler OnResized(this Control target, Action action) {
            return new SignalHandler(target, Control_ResizedSignal, action);
        }

        public const string Control_MinimumSizeChangedSignal = "minimum_size_changed"; 
        public static SignalHandler OnMinimumSizeChanged(this Control target, Action action) {
            return new SignalHandler(target, Control_MinimumSizeChangedSignal, action);
        }

        public const string Control_MouseEnteredSignal = "mouse_entered"; 
        public static SignalHandler OnMouseEntered(this Control target, Action action) {
            return new SignalHandler(target, Control_MouseEnteredSignal, action);
        }

        public const string Control_SizeFlagsChangedSignal = "size_flags_changed"; 
        public static SignalHandler OnSizeFlagsChanged(this Control target, Action action) {
            return new SignalHandler(target, Control_SizeFlagsChangedSignal, action);
        }

        public const string Control_FocusExitedSignal = "focus_exited"; 
        public static SignalHandler OnFocusExited(this Control target, Action action) {
            return new SignalHandler(target, Control_FocusExitedSignal, action);
        }

        public const string Timer_TimeoutSignal = "timeout"; 
        public static SignalHandler OnTimeout(this Timer target, Action action) {
            return new SignalHandler(target, Timer_TimeoutSignal, action);
        }

        public const string GridMap_CellSizeChangedSignal = "cell_size_changed"; 
        public static SignalHandler<Vector3> OnCellSizeChanged(this GridMap target, Action<Vector3> action) {
            return new SignalHandler<Vector3>(target, GridMap_CellSizeChangedSignal, action);
        }

        public const string GDScriptFunctionState_CompletedSignal = "completed"; 
        public static SignalHandler<object> OnCompleted(this GDScriptFunctionState target, Action<object> action) {
            return new SignalHandler<object>(target, GDScriptFunctionState_CompletedSignal, action);
        }

        public const string Animation_TracksChangedSignal = "tracks_changed"; 
        public static SignalHandler OnTracksChanged(this Godot.Animation target, Action action) {
            return new SignalHandler(target, Animation_TracksChangedSignal, action);
        }

        public const string SceneTreeTimer_TimeoutSignal = "timeout"; 
        public static SignalHandler OnTimeout(this SceneTreeTimer target, Action action) {
            return new SignalHandler(target, SceneTreeTimer_TimeoutSignal, action);
        }

        public const string AudioStreamPlayer2D_FinishedSignal = "finished"; 
        public static SignalHandler OnFinished(this AudioStreamPlayer2D target, Action action) {
            return new SignalHandler(target, AudioStreamPlayer2D_FinishedSignal, action);
        }

        public const string AudioStreamPlayer3D_FinishedSignal = "finished"; 
        public static SignalHandler OnFinished(this AudioStreamPlayer3D target, Action action) {
            return new SignalHandler(target, AudioStreamPlayer3D_FinishedSignal, action);
        }

        public const string AudioStreamPlayer_FinishedSignal = "finished"; 
        public static SignalHandler OnFinished(this AudioStreamPlayer target, Action action) {
            return new SignalHandler(target, AudioStreamPlayer_FinishedSignal, action);
        }

        public const string SceneTree_ConnectedToServerSignal = "connected_to_server"; 
        public static SignalHandler OnConnectedToServer(this SceneTree target, Action action) {
            return new SignalHandler(target, SceneTree_ConnectedToServerSignal, action);
        }

        public const string SceneTree_NodeConfigurationWarningChangedSignal = "node_configuration_warning_changed"; 
        public static SignalHandler<Node> OnNodeConfigurationWarningChanged(this SceneTree target, Action<Node> action) {
            return new SignalHandler<Node>(target, SceneTree_NodeConfigurationWarningChangedSignal, action);
        }

        public const string SceneTree_ConnectionFailedSignal = "connection_failed"; 
        public static SignalHandler OnConnectionFailed(this SceneTree target, Action action) {
            return new SignalHandler(target, SceneTree_ConnectionFailedSignal, action);
        }

        public const string SceneTree_PhysicsFrameSignal = "physics_frame"; 
        public static SignalHandler OnPhysicsFrame(this SceneTree target, Action action) {
            return new SignalHandler(target, SceneTree_PhysicsFrameSignal, action);
        }

        public const string SceneTree_ScreenResizedSignal = "screen_resized"; 
        public static SignalHandler OnScreenResized(this SceneTree target, Action action) {
            return new SignalHandler(target, SceneTree_ScreenResizedSignal, action);
        }

        public const string SceneTree_NetworkPeerDisconnectedSignal = "network_peer_disconnected"; 
        public static SignalHandler<int> OnNetworkPeerDisconnected(this SceneTree target, Action<int> action) {
            return new SignalHandler<int>(target, SceneTree_NetworkPeerDisconnectedSignal, action);
        }

        public const string SceneTree_NetworkPeerConnectedSignal = "network_peer_connected"; 
        public static SignalHandler<int> OnNetworkPeerConnected(this SceneTree target, Action<int> action) {
            return new SignalHandler<int>(target, SceneTree_NetworkPeerConnectedSignal, action);
        }

        public const string SceneTree_NodeRemovedSignal = "node_removed"; 
        public static SignalHandler<Node> OnNodeRemoved(this SceneTree target, Action<Node> action) {
            return new SignalHandler<Node>(target, SceneTree_NodeRemovedSignal, action);
        }

        public const string SceneTree_NodeAddedSignal = "node_added"; 
        public static SignalHandler<Node> OnNodeAdded(this SceneTree target, Action<Node> action) {
            return new SignalHandler<Node>(target, SceneTree_NodeAddedSignal, action);
        }

        public const string SceneTree_FilesDroppedSignal = "files_dropped"; 
        public static SignalHandler<string[], int> OnFilesDropped(this SceneTree target, Action<string[], int> action) {
            return new SignalHandler<string[], int>(target, SceneTree_FilesDroppedSignal, action);
        }

        public const string SceneTree_IdleFrameSignal = "idle_frame"; 
        public static SignalHandler OnIdleFrame(this SceneTree target, Action action) {
            return new SignalHandler(target, SceneTree_IdleFrameSignal, action);
        }

        public const string SceneTree_ServerDisconnectedSignal = "server_disconnected"; 
        public static SignalHandler OnServerDisconnected(this SceneTree target, Action action) {
            return new SignalHandler(target, SceneTree_ServerDisconnectedSignal, action);
        }

        public const string SceneTree_NodeRenamedSignal = "node_renamed"; 
        public static SignalHandler<Node> OnNodeRenamed(this SceneTree target, Action<Node> action) {
            return new SignalHandler<Node>(target, SceneTree_NodeRenamedSignal, action);
        }

        public const string SceneTree_TreeChangedSignal = "tree_changed"; 
        public static SignalHandler OnTreeChanged(this SceneTree target, Action action) {
            return new SignalHandler(target, SceneTree_TreeChangedSignal, action);
        }

        public const string SceneTree_GlobalMenuActionSignal = "global_menu_action"; 
        public static SignalHandler<object, object> OnGlobalMenuAction(this SceneTree target, Action<object, object> action) {
            return new SignalHandler<object, object>(target, SceneTree_GlobalMenuActionSignal, action);
        }

        public const string Sprite_FrameChangedSignal = "frame_changed"; 
        public static SignalHandler OnFrameChanged(this Sprite target, Action action) {
            return new SignalHandler(target, Sprite_FrameChangedSignal, action);
        }

        public const string Sprite_TextureChangedSignal = "texture_changed"; 
        public static SignalHandler OnTextureChanged(this Sprite target, Action action) {
            return new SignalHandler(target, Sprite_TextureChangedSignal, action);
        }

        public const string MeshInstance2D_TextureChangedSignal = "texture_changed"; 
        public static SignalHandler OnTextureChanged(this MeshInstance2D target, Action action) {
            return new SignalHandler(target, MeshInstance2D_TextureChangedSignal, action);
        }

        public const string Skeleton2D_BoneSetupChangedSignal = "bone_setup_changed"; 
        public static SignalHandler OnBoneSetupChanged(this Skeleton2D target, Action action) {
            return new SignalHandler(target, Skeleton2D_BoneSetupChangedSignal, action);
        }

        public const string TileMap_SettingsChangedSignal = "settings_changed"; 
        public static SignalHandler OnSettingsChanged(this TileMap target, Action action) {
            return new SignalHandler(target, TileMap_SettingsChangedSignal, action);
        }

        public const string AnimatedSprite_FrameChangedSignal = "frame_changed"; 
        public static SignalHandler OnFrameChanged(this AnimatedSprite target, Action action) {
            return new SignalHandler(target, AnimatedSprite_FrameChangedSignal, action);
        }

        public const string AnimatedSprite_AnimationFinishedSignal = "animation_finished"; 
        public static SignalHandler OnAnimationFinished(this AnimatedSprite target, Action action) {
            return new SignalHandler(target, AnimatedSprite_AnimationFinishedSignal, action);
        }

        public const string TouchScreenButton_ReleasedSignal = "released"; 
        public static SignalHandler OnReleased(this TouchScreenButton target, Action action) {
            return new SignalHandler(target, TouchScreenButton_ReleasedSignal, action);
        }

        public const string TouchScreenButton_PressedSignal = "pressed"; 
        public static SignalHandler OnPressed(this TouchScreenButton target, Action action) {
            return new SignalHandler(target, TouchScreenButton_PressedSignal, action);
        }

        public const string Area_AreaExitedSignal = "area_exited"; 
        public static SignalHandler<Area> OnAreaExited(this Area target, Action<Area> action) {
            return new SignalHandler<Area>(target, Area_AreaExitedSignal, action);
        }

        public const string Area_AreaShapeExitedSignal = "area_shape_exited"; 
        public static SignalHandler<RID, Area, int, int> OnAreaShapeExited(this Area target, Action<RID, Area, int, int> action) {
            return new SignalHandler<RID, Area, int, int>(target, Area_AreaShapeExitedSignal, action);
        }

        public const string Area_BodyEnteredSignal = "body_entered"; 
        public static SignalHandler<Node> OnBodyEntered(this Area target, Action<Node> action) {
            return new SignalHandler<Node>(target, Area_BodyEnteredSignal, action);
        }

        public const string Area_BodyShapeEnteredSignal = "body_shape_entered"; 
        public static SignalHandler<RID, Node, int, int> OnBodyShapeEntered(this Area target, Action<RID, Node, int, int> action) {
            return new SignalHandler<RID, Node, int, int>(target, Area_BodyShapeEnteredSignal, action);
        }

        public const string Area_AreaEnteredSignal = "area_entered"; 
        public static SignalHandler<Area> OnAreaEntered(this Area target, Action<Area> action) {
            return new SignalHandler<Area>(target, Area_AreaEnteredSignal, action);
        }

        public const string Area_AreaShapeEnteredSignal = "area_shape_entered"; 
        public static SignalHandler<RID, Area, int, int> OnAreaShapeEntered(this Area target, Action<RID, Area, int, int> action) {
            return new SignalHandler<RID, Area, int, int>(target, Area_AreaShapeEnteredSignal, action);
        }

        public const string Area_BodyExitedSignal = "body_exited"; 
        public static SignalHandler<Node> OnBodyExited(this Area target, Action<Node> action) {
            return new SignalHandler<Node>(target, Area_BodyExitedSignal, action);
        }

        public const string Area_BodyShapeExitedSignal = "body_shape_exited"; 
        public static SignalHandler<RID, Node, int, int> OnBodyShapeExited(this Area target, Action<RID, Node, int, int> action) {
            return new SignalHandler<RID, Node, int, int>(target, Area_BodyShapeExitedSignal, action);
        }

        public const string VisibilityNotifier_CameraExitedSignal = "camera_exited"; 
        public static SignalHandler<Camera> OnCameraExited(this VisibilityNotifier target, Action<Camera> action) {
            return new SignalHandler<Camera>(target, VisibilityNotifier_CameraExitedSignal, action);
        }

        public const string VisibilityNotifier_ScreenEnteredSignal = "screen_entered"; 
        public static SignalHandler OnScreenEntered(this VisibilityNotifier target, Action action) {
            return new SignalHandler(target, VisibilityNotifier_ScreenEnteredSignal, action);
        }

        public const string VisibilityNotifier_CameraEnteredSignal = "camera_entered"; 
        public static SignalHandler<Camera> OnCameraEntered(this VisibilityNotifier target, Action<Camera> action) {
            return new SignalHandler<Camera>(target, VisibilityNotifier_CameraEnteredSignal, action);
        }

        public const string VisibilityNotifier_ScreenExitedSignal = "screen_exited"; 
        public static SignalHandler OnScreenExited(this VisibilityNotifier target, Action action) {
            return new SignalHandler(target, VisibilityNotifier_ScreenExitedSignal, action);
        }

        public const string MultiMeshInstance2D_TextureChangedSignal = "texture_changed"; 
        public static SignalHandler OnTextureChanged(this MultiMeshInstance2D target, Action action) {
            return new SignalHandler(target, MultiMeshInstance2D_TextureChangedSignal, action);
        }

        public const string CollisionObject2D_MouseExitedSignal = "mouse_exited"; 
        public static SignalHandler OnMouseExited(this CollisionObject2D target, Action action) {
            return new SignalHandler(target, CollisionObject2D_MouseExitedSignal, action);
        }

        public const string CollisionObject2D_MouseEnteredSignal = "mouse_entered"; 
        public static SignalHandler OnMouseEntered(this CollisionObject2D target, Action action) {
            return new SignalHandler(target, CollisionObject2D_MouseEnteredSignal, action);
        }

        public const string CollisionObject2D_InputEventSignal = "input_event"; 
        public static SignalHandler<Node, InputEvent, int> OnInputEvent(this CollisionObject2D target, Action<Node, InputEvent, int> action) {
            return new SignalHandler<Node, InputEvent, int>(target, CollisionObject2D_InputEventSignal, action);
        }

        public const string VisibilityNotifier2D_ScreenEnteredSignal = "screen_entered"; 
        public static SignalHandler OnScreenEntered(this VisibilityNotifier2D target, Action action) {
            return new SignalHandler(target, VisibilityNotifier2D_ScreenEnteredSignal, action);
        }

        public const string VisibilityNotifier2D_ViewportEnteredSignal = "viewport_entered"; 
        public static SignalHandler<Viewport> OnViewportEntered(this VisibilityNotifier2D target, Action<Viewport> action) {
            return new SignalHandler<Viewport>(target, VisibilityNotifier2D_ViewportEnteredSignal, action);
        }

        public const string VisibilityNotifier2D_ScreenExitedSignal = "screen_exited"; 
        public static SignalHandler OnScreenExited(this VisibilityNotifier2D target, Action action) {
            return new SignalHandler(target, VisibilityNotifier2D_ScreenExitedSignal, action);
        }

        public const string VisibilityNotifier2D_ViewportExitedSignal = "viewport_exited"; 
        public static SignalHandler<Viewport> OnViewportExited(this VisibilityNotifier2D target, Action<Viewport> action) {
            return new SignalHandler<Viewport>(target, VisibilityNotifier2D_ViewportExitedSignal, action);
        }

        public const string UndoRedo_VersionChangedSignal = "version_changed"; 
        public static SignalHandler OnVersionChanged(this UndoRedo target, Action action) {
            return new SignalHandler(target, UndoRedo_VersionChangedSignal, action);
        }

        public const string MultiplayerAPI_ConnectedToServerSignal = "connected_to_server"; 
        public static SignalHandler OnConnectedToServer(this MultiplayerAPI target, Action action) {
            return new SignalHandler(target, MultiplayerAPI_ConnectedToServerSignal, action);
        }

        public const string MultiplayerAPI_ConnectionFailedSignal = "connection_failed"; 
        public static SignalHandler OnConnectionFailed(this MultiplayerAPI target, Action action) {
            return new SignalHandler(target, MultiplayerAPI_ConnectionFailedSignal, action);
        }

        public const string MultiplayerAPI_NetworkPeerPacketSignal = "network_peer_packet"; 
        public static SignalHandler<int, byte[]> OnNetworkPeerPacket(this MultiplayerAPI target, Action<int, byte[]> action) {
            return new SignalHandler<int, byte[]>(target, MultiplayerAPI_NetworkPeerPacketSignal, action);
        }

        public const string MultiplayerAPI_NetworkPeerDisconnectedSignal = "network_peer_disconnected"; 
        public static SignalHandler<int> OnNetworkPeerDisconnected(this MultiplayerAPI target, Action<int> action) {
            return new SignalHandler<int>(target, MultiplayerAPI_NetworkPeerDisconnectedSignal, action);
        }

        public const string MultiplayerAPI_NetworkPeerConnectedSignal = "network_peer_connected"; 
        public static SignalHandler<int> OnNetworkPeerConnected(this MultiplayerAPI target, Action<int> action) {
            return new SignalHandler<int>(target, MultiplayerAPI_NetworkPeerConnectedSignal, action);
        }

        public const string MultiplayerAPI_ServerDisconnectedSignal = "server_disconnected"; 
        public static SignalHandler OnServerDisconnected(this MultiplayerAPI target, Action action) {
            return new SignalHandler(target, MultiplayerAPI_ServerDisconnectedSignal, action);
        }

        public const string Resource_ChangedSignal = "changed"; 
        public static SignalHandler OnChanged(this Resource target, Action action) {
            return new SignalHandler(target, Resource_ChangedSignal, action);
        }

        public const string MainLoop_RequestPermissionsResultSignal = "_request_permissions_result"; 
        public static SignalHandler<string, bool> OnRequestPermissionsResult(this MainLoop target, Action<string, bool> action) {
            return new SignalHandler<string, bool>(target, MainLoop_RequestPermissionsResultSignal, action);
        }

        public const string Node_RenamedSignal = "renamed"; 
        public static SignalHandler OnRenamed(this Node target, Action action) {
            return new SignalHandler(target, Node_RenamedSignal, action);
        }

        public const string Node_ReadySignal = "ready"; 
        public static SignalHandler OnReady(this Node target, Action action) {
            return new SignalHandler(target, Node_ReadySignal, action);
        }

        public const string Node_TreeEnteredSignal = "tree_entered"; 
        public static SignalHandler OnTreeEntered(this Node target, Action action) {
            return new SignalHandler(target, Node_TreeEnteredSignal, action);
        }

        public const string Node_TreeExitingSignal = "tree_exiting"; 
        public static SignalHandler OnTreeExiting(this Node target, Action action) {
            return new SignalHandler(target, Node_TreeExitingSignal, action);
        }

        public const string Node_TreeExitedSignal = "tree_exited"; 
        public static SignalHandler OnTreeExited(this Node target, Action action) {
            return new SignalHandler(target, Node_TreeExitedSignal, action);
        }

        public const string NetworkedMultiplayerPeer_ConnectionFailedSignal = "connection_failed"; 
        public static SignalHandler OnConnectionFailed(this NetworkedMultiplayerPeer target, Action action) {
            return new SignalHandler(target, NetworkedMultiplayerPeer_ConnectionFailedSignal, action);
        }

        public const string NetworkedMultiplayerPeer_ConnectionSucceededSignal = "connection_succeeded"; 
        public static SignalHandler OnConnectionSucceeded(this NetworkedMultiplayerPeer target, Action action) {
            return new SignalHandler(target, NetworkedMultiplayerPeer_ConnectionSucceededSignal, action);
        }

        public const string NetworkedMultiplayerPeer_PeerDisconnectedSignal = "peer_disconnected"; 
        public static SignalHandler<int> OnPeerDisconnected(this NetworkedMultiplayerPeer target, Action<int> action) {
            return new SignalHandler<int>(target, NetworkedMultiplayerPeer_PeerDisconnectedSignal, action);
        }

        public const string NetworkedMultiplayerPeer_PeerConnectedSignal = "peer_connected"; 
        public static SignalHandler<int> OnPeerConnected(this NetworkedMultiplayerPeer target, Action<int> action) {
            return new SignalHandler<int>(target, NetworkedMultiplayerPeer_PeerConnectedSignal, action);
        }

        public const string NetworkedMultiplayerPeer_ServerDisconnectedSignal = "server_disconnected"; 
        public static SignalHandler OnServerDisconnected(this NetworkedMultiplayerPeer target, Action action) {
            return new SignalHandler(target, NetworkedMultiplayerPeer_ServerDisconnectedSignal, action);
        }

        public const string Object_ScriptChangedSignal = "script_changed"; 
        public static SignalHandler OnScriptChanged(this Object target, Action action) {
            return new SignalHandler(target, Object_ScriptChangedSignal, action);
        }

        public const string LineEdit_TextEnteredSignal = "text_entered"; 
        public static SignalHandler<string> OnTextEntered(this LineEdit target, Action<string> action) {
            return new SignalHandler<string>(target, LineEdit_TextEnteredSignal, action);
        }

        public const string LineEdit_TextChangedSignal = "text_changed"; 
        public static SignalHandler<string> OnTextChanged(this LineEdit target, Action<string> action) {
            return new SignalHandler<string>(target, LineEdit_TextChangedSignal, action);
        }

        public const string LineEdit_TextChangeRejectedSignal = "text_change_rejected"; 
        public static SignalHandler<string> OnTextChangeRejected(this LineEdit target, Action<string> action) {
            return new SignalHandler<string>(target, LineEdit_TextChangeRejectedSignal, action);
        }

        public const string RigidBody2D_BodyEnteredSignal = "body_entered"; 
        public static SignalHandler<Node> OnBodyEntered(this RigidBody2D target, Action<Node> action) {
            return new SignalHandler<Node>(target, RigidBody2D_BodyEnteredSignal, action);
        }

        public const string RigidBody2D_BodyShapeEnteredSignal = "body_shape_entered"; 
        public static SignalHandler<RID, Node, int, int> OnBodyShapeEntered(this RigidBody2D target, Action<RID, Node, int, int> action) {
            return new SignalHandler<RID, Node, int, int>(target, RigidBody2D_BodyShapeEnteredSignal, action);
        }

        public const string RigidBody2D_SleepingStateChangedSignal = "sleeping_state_changed"; 
        public static SignalHandler OnSleepingStateChanged(this RigidBody2D target, Action action) {
            return new SignalHandler(target, RigidBody2D_SleepingStateChangedSignal, action);
        }

        public const string RigidBody2D_BodyExitedSignal = "body_exited"; 
        public static SignalHandler<Node> OnBodyExited(this RigidBody2D target, Action<Node> action) {
            return new SignalHandler<Node>(target, RigidBody2D_BodyExitedSignal, action);
        }

        public const string RigidBody2D_BodyShapeExitedSignal = "body_shape_exited"; 
        public static SignalHandler<RID, Node, int, int> OnBodyShapeExited(this RigidBody2D target, Action<RID, Node, int, int> action) {
            return new SignalHandler<RID, Node, int, int>(target, RigidBody2D_BodyShapeExitedSignal, action);
        }

        public const string Area2D_AreaExitedSignal = "area_exited"; 
        public static SignalHandler<Area2D> OnAreaExited(this Area2D target, Action<Area2D> action) {
            return new SignalHandler<Area2D>(target, Area2D_AreaExitedSignal, action);
        }

        public const string Area2D_AreaShapeExitedSignal = "area_shape_exited"; 
        public static SignalHandler<RID, Area2D, int, int> OnAreaShapeExited(this Area2D target, Action<RID, Area2D, int, int> action) {
            return new SignalHandler<RID, Area2D, int, int>(target, Area2D_AreaShapeExitedSignal, action);
        }

        public const string Area2D_BodyEnteredSignal = "body_entered"; 
        public static SignalHandler<Node> OnBodyEntered(this Area2D target, Action<Node> action) {
            return new SignalHandler<Node>(target, Area2D_BodyEnteredSignal, action);
        }

        public const string Area2D_BodyShapeEnteredSignal = "body_shape_entered"; 
        public static SignalHandler<RID, Node, int, int> OnBodyShapeEntered(this Area2D target, Action<RID, Node, int, int> action) {
            return new SignalHandler<RID, Node, int, int>(target, Area2D_BodyShapeEnteredSignal, action);
        }

        public const string Area2D_AreaEnteredSignal = "area_entered"; 
        public static SignalHandler<Area2D> OnAreaEntered(this Area2D target, Action<Area2D> action) {
            return new SignalHandler<Area2D>(target, Area2D_AreaEnteredSignal, action);
        }

        public const string Area2D_AreaShapeEnteredSignal = "area_shape_entered"; 
        public static SignalHandler<RID, Area2D, int, int> OnAreaShapeEntered(this Area2D target, Action<RID, Area2D, int, int> action) {
            return new SignalHandler<RID, Area2D, int, int>(target, Area2D_AreaShapeEnteredSignal, action);
        }

        public const string Area2D_BodyExitedSignal = "body_exited"; 
        public static SignalHandler<Node> OnBodyExited(this Area2D target, Action<Node> action) {
            return new SignalHandler<Node>(target, Area2D_BodyExitedSignal, action);
        }

        public const string Area2D_BodyShapeExitedSignal = "body_shape_exited"; 
        public static SignalHandler<RID, Node, int, int> OnBodyShapeExited(this Area2D target, Action<RID, Node, int, int> action) {
            return new SignalHandler<RID, Node, int, int>(target, Area2D_BodyShapeExitedSignal, action);
        }

        public const string HTTPRequest_RequestCompletedSignal = "request_completed"; 
        public static SignalHandler<int, int, string[], byte[]> OnRequestCompleted(this HTTPRequest target, Action<int, int, string[], byte[]> action) {
            return new SignalHandler<int, int, string[], byte[]>(target, HTTPRequest_RequestCompletedSignal, action);
        }

        public const string CanvasItem_ItemRectChangedSignal = "item_rect_changed"; 
        public static SignalHandler OnItemRectChanged(this CanvasItem target, Action action) {
            return new SignalHandler(target, CanvasItem_ItemRectChangedSignal, action);
        }

        public const string CanvasItem_DrawSignal = "draw"; 
        public static SignalHandler OnDraw(this CanvasItem target, Action action) {
            return new SignalHandler(target, CanvasItem_DrawSignal, action);
        }

        public const string CanvasItem_VisibilityChangedSignal = "visibility_changed"; 
        public static SignalHandler OnVisibilityChanged(this CanvasItem target, Action action) {
            return new SignalHandler(target, CanvasItem_VisibilityChangedSignal, action);
        }

        public const string CanvasItem_HideSignal = "hide"; 
        public static SignalHandler OnHide(this CanvasItem target, Action action) {
            return new SignalHandler(target, CanvasItem_HideSignal, action);
        }

        public const string NinePatchRect_TextureChangedSignal = "texture_changed"; 
        public static SignalHandler OnTextureChanged(this NinePatchRect target, Action action) {
            return new SignalHandler(target, NinePatchRect_TextureChangedSignal, action);
        }

        public const string Container_SortChildrenSignal = "sort_children"; 
        public static SignalHandler OnSortChildren(this Container target, Action action) {
            return new SignalHandler(target, Container_SortChildrenSignal, action);
        }

        public const string TabContainer_PrePopupPressedSignal = "pre_popup_pressed"; 
        public static SignalHandler OnPrePopupPressed(this TabContainer target, Action action) {
            return new SignalHandler(target, TabContainer_PrePopupPressedSignal, action);
        }

        public const string TabContainer_TabSelectedSignal = "tab_selected"; 
        public static SignalHandler<int> OnTabSelected(this TabContainer target, Action<int> action) {
            return new SignalHandler<int>(target, TabContainer_TabSelectedSignal, action);
        }

        public const string TabContainer_TabChangedSignal = "tab_changed"; 
        public static SignalHandler<int> OnTabChanged(this TabContainer target, Action<int> action) {
            return new SignalHandler<int>(target, TabContainer_TabChangedSignal, action);
        }

        public const string ScrollContainer_ScrollStartedSignal = "scroll_started"; 
        public static SignalHandler OnScrollStarted(this ScrollContainer target, Action action) {
            return new SignalHandler(target, ScrollContainer_ScrollStartedSignal, action);
        }

        public const string ScrollContainer_ScrollEndedSignal = "scroll_ended"; 
        public static SignalHandler OnScrollEnded(this ScrollContainer target, Action action) {
            return new SignalHandler(target, ScrollContainer_ScrollEndedSignal, action);
        }

        public const string AcceptDialog_ConfirmedSignal = "confirmed"; 
        public static SignalHandler OnConfirmed(this AcceptDialog target, Action action) {
            return new SignalHandler(target, AcceptDialog_ConfirmedSignal, action);
        }

        public const string AcceptDialog_CustomActionSignal = "custom_action"; 
        public static SignalHandler<string> OnCustomAction(this AcceptDialog target, Action<string> action) {
            return new SignalHandler<string>(target, AcceptDialog_CustomActionSignal, action);
        }

        public const string AudioServer_AudioServerBusLayoutChangedSignal = "bus_layout_changed"; 
        public static SignalHandler OnAudioServerBusLayoutChanged(Action action) {
            return new SignalHandler(AudioServer.Singleton, AudioServer_AudioServerBusLayoutChangedSignal, action);
        }

        public const string CameraServer_CameraServerCameraFeedRemovedSignal = "camera_feed_removed"; 
        public static SignalHandler<int> OnCameraServerCameraFeedRemoved(Action<int> action) {
            return new SignalHandler<int>(CameraServer.Singleton, CameraServer_CameraServerCameraFeedRemovedSignal, action);
        }

        public const string CameraServer_CameraServerCameraFeedAddedSignal = "camera_feed_added"; 
        public static SignalHandler<int> OnCameraServerCameraFeedAdded(Action<int> action) {
            return new SignalHandler<int>(CameraServer.Singleton, CameraServer_CameraServerCameraFeedAddedSignal, action);
        }

        public const string Viewport_SizeChangedSignal = "size_changed"; 
        public static SignalHandler OnSizeChanged(this Viewport target, Action action) {
            return new SignalHandler(target, Viewport_SizeChangedSignal, action);
        }

        public const string Viewport_GuiFocusChangedSignal = "gui_focus_changed"; 
        public static SignalHandler<Control> OnGuiFocusChanged(this Viewport target, Action<Control> action) {
            return new SignalHandler<Control>(target, Viewport_GuiFocusChangedSignal, action);
        }

        public const string FileDialog_FilesSelectedSignal = "files_selected"; 
        public static SignalHandler<string[]> OnFilesSelected(this FileDialog target, Action<string[]> action) {
            return new SignalHandler<string[]>(target, FileDialog_FilesSelectedSignal, action);
        }

        public const string FileDialog_DirSelectedSignal = "dir_selected"; 
        public static SignalHandler<string> OnDirSelected(this FileDialog target, Action<string> action) {
            return new SignalHandler<string>(target, FileDialog_DirSelectedSignal, action);
        }

        public const string FileDialog_FileSelectedSignal = "file_selected"; 
        public static SignalHandler<string> OnFileSelected(this FileDialog target, Action<string> action) {
            return new SignalHandler<string>(target, FileDialog_FileSelectedSignal, action);
        }

        public const string Tabs_TabCloseSignal = "tab_close"; 
        public static SignalHandler<int> OnTabClose(this Tabs target, Action<int> action) {
            return new SignalHandler<int>(target, Tabs_TabCloseSignal, action);
        }

        public const string Tabs_TabClickedSignal = "tab_clicked"; 
        public static SignalHandler<int> OnTabClicked(this Tabs target, Action<int> action) {
            return new SignalHandler<int>(target, Tabs_TabClickedSignal, action);
        }

        public const string Tabs_RepositionActiveTabRequestSignal = "reposition_active_tab_request"; 
        public static SignalHandler<int> OnRepositionActiveTabRequest(this Tabs target, Action<int> action) {
            return new SignalHandler<int>(target, Tabs_RepositionActiveTabRequestSignal, action);
        }

        public const string Tabs_RightButtonPressedSignal = "right_button_pressed"; 
        public static SignalHandler<int> OnRightButtonPressed(this Tabs target, Action<int> action) {
            return new SignalHandler<int>(target, Tabs_RightButtonPressedSignal, action);
        }

        public const string Tabs_TabChangedSignal = "tab_changed"; 
        public static SignalHandler<int> OnTabChanged(this Tabs target, Action<int> action) {
            return new SignalHandler<int>(target, Tabs_TabChangedSignal, action);
        }

        public const string Tabs_TabHoverSignal = "tab_hover"; 
        public static SignalHandler<int> OnTabHover(this Tabs target, Action<int> action) {
            return new SignalHandler<int>(target, Tabs_TabHoverSignal, action);
        }

        public const string VisualServer_VisualServerFramePostDrawSignal = "frame_post_draw"; 
        public static SignalHandler OnVisualServerFramePostDraw(Action action) {
            return new SignalHandler(VisualServer.Singleton, VisualServer_VisualServerFramePostDrawSignal, action);
        }

        public const string VisualServer_VisualServerFramePreDrawSignal = "frame_pre_draw"; 
        public static SignalHandler OnVisualServerFramePreDraw(Action action) {
            return new SignalHandler(VisualServer.Singleton, VisualServer_VisualServerFramePreDrawSignal, action);
        }

        public const string ARVRServer_ARVRServerTrackerRemovedSignal = "tracker_removed"; 
        public static SignalHandler<string, int, int> OnARVRServerTrackerRemoved(Action<string, int, int> action) {
            return new SignalHandler<string, int, int>(ARVRServer.Singleton, ARVRServer_ARVRServerTrackerRemovedSignal, action);
        }

        public const string ARVRServer_ARVRServerTrackerAddedSignal = "tracker_added"; 
        public static SignalHandler<string, int, int> OnARVRServerTrackerAdded(Action<string, int, int> action) {
            return new SignalHandler<string, int, int>(ARVRServer.Singleton, ARVRServer_ARVRServerTrackerAddedSignal, action);
        }

        public const string ARVRServer_ARVRServerInterfaceRemovedSignal = "interface_removed"; 
        public static SignalHandler<string> OnARVRServerInterfaceRemoved(Action<string> action) {
            return new SignalHandler<string>(ARVRServer.Singleton, ARVRServer_ARVRServerInterfaceRemovedSignal, action);
        }

        public const string ARVRServer_ARVRServerInterfaceAddedSignal = "interface_added"; 
        public static SignalHandler<string> OnARVRServerInterfaceAdded(Action<string> action) {
            return new SignalHandler<string>(ARVRServer.Singleton, ARVRServer_ARVRServerInterfaceAddedSignal, action);
        }

        public const string Input_InputJoyConnectionChangedSignal = "joy_connection_changed"; 
        public static SignalHandler<int, bool> OnInputJoyConnectionChanged(Action<int, bool> action) {
            return new SignalHandler<int, bool>(Input.Singleton, Input_InputJoyConnectionChangedSignal, action);
        }

        public const string WebXRInterface_SessionEndedSignal = "session_ended"; 
        public static SignalHandler OnSessionEnded(this WebXRInterface target, Action action) {
            return new SignalHandler(target, WebXRInterface_SessionEndedSignal, action);
        }

        public const string WebXRInterface_ReferenceSpaceResetSignal = "reference_space_reset"; 
        public static SignalHandler OnReferenceSpaceReset(this WebXRInterface target, Action action) {
            return new SignalHandler(target, WebXRInterface_ReferenceSpaceResetSignal, action);
        }

        public const string WebXRInterface_SelectstartSignal = "selectstart"; 
        public static SignalHandler<int> OnSelectstart(this WebXRInterface target, Action<int> action) {
            return new SignalHandler<int>(target, WebXRInterface_SelectstartSignal, action);
        }

        public const string WebXRInterface_SelectendSignal = "selectend"; 
        public static SignalHandler<int> OnSelectend(this WebXRInterface target, Action<int> action) {
            return new SignalHandler<int>(target, WebXRInterface_SelectendSignal, action);
        }

        public const string WebXRInterface_SqueezestartSignal = "squeezestart"; 
        public static SignalHandler<int> OnSqueezestart(this WebXRInterface target, Action<int> action) {
            return new SignalHandler<int>(target, WebXRInterface_SqueezestartSignal, action);
        }

        public const string WebXRInterface_SelectSignal = "select"; 
        public static SignalHandler<int> OnSelect(this WebXRInterface target, Action<int> action) {
            return new SignalHandler<int>(target, WebXRInterface_SelectSignal, action);
        }

        public const string WebXRInterface_SessionFailedSignal = "session_failed"; 
        public static SignalHandler<string> OnSessionFailed(this WebXRInterface target, Action<string> action) {
            return new SignalHandler<string>(target, WebXRInterface_SessionFailedSignal, action);
        }

        public const string WebXRInterface_VisibilityStateChangedSignal = "visibility_state_changed"; 
        public static SignalHandler OnVisibilityStateChanged(this WebXRInterface target, Action action) {
            return new SignalHandler(target, WebXRInterface_VisibilityStateChangedSignal, action);
        }

        public const string WebXRInterface_SqueezeendSignal = "squeezeend"; 
        public static SignalHandler<int> OnSqueezeend(this WebXRInterface target, Action<int> action) {
            return new SignalHandler<int>(target, WebXRInterface_SqueezeendSignal, action);
        }

        public const string WebXRInterface_SessionSupportedSignal = "session_supported"; 
        public static SignalHandler<string, bool> OnSessionSupported(this WebXRInterface target, Action<string, bool> action) {
            return new SignalHandler<string, bool>(target, WebXRInterface_SessionSupportedSignal, action);
        }

        public const string WebXRInterface_SqueezeSignal = "squeeze"; 
        public static SignalHandler<int> OnSqueeze(this WebXRInterface target, Action<int> action) {
            return new SignalHandler<int>(target, WebXRInterface_SqueezeSignal, action);
        }

        public const string WebXRInterface_SessionStartedSignal = "session_started"; 
        public static SignalHandler OnSessionStarted(this WebXRInterface target, Action action) {
            return new SignalHandler(target, WebXRInterface_SessionStartedSignal, action);
        }

        public const string WebSocketClient_ServerCloseRequestSignal = "server_close_request"; 
        public static SignalHandler<int, string> OnServerCloseRequest(this WebSocketClient target, Action<int, string> action) {
            return new SignalHandler<int, string>(target, WebSocketClient_ServerCloseRequestSignal, action);
        }

        public const string WebSocketClient_ConnectionEstablishedSignal = "connection_established"; 
        public static SignalHandler<string> OnConnectionEstablished(this WebSocketClient target, Action<string> action) {
            return new SignalHandler<string>(target, WebSocketClient_ConnectionEstablishedSignal, action);
        }

        public const string WebSocketClient_DataReceivedSignal = "data_received"; 
        public static SignalHandler OnDataReceived(this WebSocketClient target, Action action) {
            return new SignalHandler(target, WebSocketClient_DataReceivedSignal, action);
        }

        public const string WebSocketClient_ConnectionErrorSignal = "connection_error"; 
        public static SignalHandler OnConnectionError(this WebSocketClient target, Action action) {
            return new SignalHandler(target, WebSocketClient_ConnectionErrorSignal, action);
        }

        public const string WebSocketClient_ConnectionClosedSignal = "connection_closed"; 
        public static SignalHandler<bool> OnConnectionClosed(this WebSocketClient target, Action<bool> action) {
            return new SignalHandler<bool>(target, WebSocketClient_ConnectionClosedSignal, action);
        }

        public const string WebSocketServer_ClientCloseRequestSignal = "client_close_request"; 
        public static SignalHandler<int, int, string> OnClientCloseRequest(this WebSocketServer target, Action<int, int, string> action) {
            return new SignalHandler<int, int, string>(target, WebSocketServer_ClientCloseRequestSignal, action);
        }

        public const string WebSocketServer_DataReceivedSignal = "data_received"; 
        public static SignalHandler<int> OnDataReceived(this WebSocketServer target, Action<int> action) {
            return new SignalHandler<int>(target, WebSocketServer_DataReceivedSignal, action);
        }

        public const string WebSocketServer_ClientConnectedSignal = "client_connected"; 
        public static SignalHandler<int, string> OnClientConnected(this WebSocketServer target, Action<int, string> action) {
            return new SignalHandler<int, string>(target, WebSocketServer_ClientConnectedSignal, action);
        }

        public const string WebSocketServer_ClientDisconnectedSignal = "client_disconnected"; 
        public static SignalHandler<int, bool> OnClientDisconnected(this WebSocketServer target, Action<int, bool> action) {
            return new SignalHandler<int, bool>(target, WebSocketServer_ClientDisconnectedSignal, action);
        }

        public const string VisualScript_NodePortsChangedSignal = "node_ports_changed"; 
        public static SignalHandler<string, int> OnNodePortsChanged(this VisualScript target, Action<string, int> action) {
            return new SignalHandler<string, int>(target, VisualScript_NodePortsChangedSignal, action);
        }

        public const string VisualScriptNode_PortsChangedSignal = "ports_changed"; 
        public static SignalHandler OnPortsChanged(this VisualScriptNode target, Action action) {
            return new SignalHandler(target, VisualScriptNode_PortsChangedSignal, action);
        }

        public const string WebSocketMultiplayerPeer_PeerPacketSignal = "peer_packet"; 
        public static SignalHandler<int> OnPeerPacket(this WebSocketMultiplayerPeer target, Action<int> action) {
            return new SignalHandler<int>(target, WebSocketMultiplayerPeer_PeerPacketSignal, action);
        }
    }
}