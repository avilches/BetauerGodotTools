using System;
using Godot;
using Object = Godot.Object;
using Animation = Godot.Animation;

namespace Betauer {
    public static partial class SignalExtensions {

        public static SignalHandler OnConfirmed(this AcceptDialog target, Action action) =>
            new SignalHandler(target, SignalConstants.AcceptDialog_ConfirmedSignal, action);

        public static SignalHandler<string> OnCustomAction(this AcceptDialog target, Action<string> action) =>
            new SignalHandler<string>(target, SignalConstants.AcceptDialog_CustomActionSignal, action);

        public static SignalHandler OnAnimationFinished(this AnimatedSprite target, Action action) =>
            new SignalHandler(target, SignalConstants.AnimatedSprite_AnimationFinishedSignal, action);

        public static SignalHandler OnFrameChanged(this AnimatedSprite target, Action action) =>
            new SignalHandler(target, SignalConstants.AnimatedSprite_FrameChangedSignal, action);

        public static SignalHandler OnAnimationFinished(this AnimatedSprite3D target, Action action) =>
            new SignalHandler(target, SignalConstants.AnimatedSprite3D_AnimationFinishedSignal, action);

        public static SignalHandler OnFrameChanged(this AnimatedSprite3D target, Action action) =>
            new SignalHandler(target, SignalConstants.AnimatedSprite3D_FrameChangedSignal, action);

        public static SignalHandler OnTracksChanged(this Animation target, Action action) =>
            new SignalHandler(target, SignalConstants.Animation_TracksChangedSignal, action);

        public static SignalHandler OnRemovedFromGraph(this AnimationNode target, Action action) =>
            new SignalHandler(target, SignalConstants.AnimationNode_RemovedFromGraphSignal, action);

        public static SignalHandler OnTreeChanged(this AnimationNode target, Action action) =>
            new SignalHandler(target, SignalConstants.AnimationNode_TreeChangedSignal, action);

        public static SignalHandler OnTrianglesUpdated(this AnimationNodeBlendSpace2D target, Action action) =>
            new SignalHandler(target, SignalConstants.AnimationNodeBlendSpace2D_TrianglesUpdatedSignal, action);

        public static SignalHandler OnAdvanceConditionChanged(this AnimationNodeStateMachineTransition target, Action action) =>
            new SignalHandler(target, SignalConstants.AnimationNodeStateMachineTransition_AdvanceConditionChangedSignal, action);

        public static SignalHandler<string, string> OnAnimationChanged(this AnimationPlayer target, Action<string, string> action) =>
            new SignalHandler<string, string>(target, SignalConstants.AnimationPlayer_AnimationChangedSignal, action);

        public static SignalHandler<string> OnAnimationFinished(this AnimationPlayer target, Action<string> action) =>
            new SignalHandler<string>(target, SignalConstants.AnimationPlayer_AnimationFinishedSignal, action);

        public static SignalHandler<string> OnAnimationStarted(this AnimationPlayer target, Action<string> action) =>
            new SignalHandler<string>(target, SignalConstants.AnimationPlayer_AnimationStartedSignal, action);

        public static SignalHandler OnCachesCleared(this AnimationPlayer target, Action action) =>
            new SignalHandler(target, SignalConstants.AnimationPlayer_CachesClearedSignal, action);

        public static SignalHandler<Area> OnAreaEntered(this Area target, Action<Area> action) =>
            new SignalHandler<Area>(target, SignalConstants.Area_AreaEnteredSignal, action);

        public static SignalHandler<Area> OnAreaExited(this Area target, Action<Area> action) =>
            new SignalHandler<Area>(target, SignalConstants.Area_AreaExitedSignal, action);

        public static SignalHandler<Area, RID, int, int> OnAreaShapeEntered(this Area target, Action<Area, RID, int, int> action) =>
            new SignalHandler<Area, RID, int, int>(target, SignalConstants.Area_AreaShapeEnteredSignal, action);

        public static SignalHandler<Area, RID, int, int> OnAreaShapeExited(this Area target, Action<Area, RID, int, int> action) =>
            new SignalHandler<Area, RID, int, int>(target, SignalConstants.Area_AreaShapeExitedSignal, action);

        public static SignalHandler<Node> OnBodyEntered(this Area target, Action<Node> action) =>
            new SignalHandler<Node>(target, SignalConstants.Area_BodyEnteredSignal, action);

        public static SignalHandler<Node> OnBodyExited(this Area target, Action<Node> action) =>
            new SignalHandler<Node>(target, SignalConstants.Area_BodyExitedSignal, action);

        public static SignalHandler<Node, RID, int, int> OnBodyShapeEntered(this Area target, Action<Node, RID, int, int> action) =>
            new SignalHandler<Node, RID, int, int>(target, SignalConstants.Area_BodyShapeEnteredSignal, action);

        public static SignalHandler<Node, RID, int, int> OnBodyShapeExited(this Area target, Action<Node, RID, int, int> action) =>
            new SignalHandler<Node, RID, int, int>(target, SignalConstants.Area_BodyShapeExitedSignal, action);

        public static SignalHandler<Area2D> OnAreaEntered(this Area2D target, Action<Area2D> action) =>
            new SignalHandler<Area2D>(target, SignalConstants.Area2D_AreaEnteredSignal, action);

        public static SignalHandler<Area2D> OnAreaExited(this Area2D target, Action<Area2D> action) =>
            new SignalHandler<Area2D>(target, SignalConstants.Area2D_AreaExitedSignal, action);

        public static SignalHandler<Area2D, RID, int, int> OnAreaShapeEntered(this Area2D target, Action<Area2D, RID, int, int> action) =>
            new SignalHandler<Area2D, RID, int, int>(target, SignalConstants.Area2D_AreaShapeEnteredSignal, action);

        public static SignalHandler<Area2D, RID, int, int> OnAreaShapeExited(this Area2D target, Action<Area2D, RID, int, int> action) =>
            new SignalHandler<Area2D, RID, int, int>(target, SignalConstants.Area2D_AreaShapeExitedSignal, action);

        public static SignalHandler<Node> OnBodyEntered(this Area2D target, Action<Node> action) =>
            new SignalHandler<Node>(target, SignalConstants.Area2D_BodyEnteredSignal, action);

        public static SignalHandler<Node> OnBodyExited(this Area2D target, Action<Node> action) =>
            new SignalHandler<Node>(target, SignalConstants.Area2D_BodyExitedSignal, action);

        public static SignalHandler<Node, RID, int, int> OnBodyShapeEntered(this Area2D target, Action<Node, RID, int, int> action) =>
            new SignalHandler<Node, RID, int, int>(target, SignalConstants.Area2D_BodyShapeEnteredSignal, action);

        public static SignalHandler<Node, RID, int, int> OnBodyShapeExited(this Area2D target, Action<Node, RID, int, int> action) =>
            new SignalHandler<Node, RID, int, int>(target, SignalConstants.Area2D_BodyShapeExitedSignal, action);

        public static SignalHandler<Mesh> OnMeshUpdated(this ARVRAnchor target, Action<Mesh> action) =>
            new SignalHandler<Mesh>(target, SignalConstants.ARVRAnchor_MeshUpdatedSignal, action);

        public static SignalHandler<int> OnButtonPressed(this ARVRController target, Action<int> action) =>
            new SignalHandler<int>(target, SignalConstants.ARVRController_ButtonPressedSignal, action);

        public static SignalHandler<int> OnButtonRelease(this ARVRController target, Action<int> action) =>
            new SignalHandler<int>(target, SignalConstants.ARVRController_ButtonReleaseSignal, action);

        public static SignalHandler<Mesh> OnMeshUpdated(this ARVRController target, Action<Mesh> action) =>
            new SignalHandler<Mesh>(target, SignalConstants.ARVRController_MeshUpdatedSignal, action);

        public static SignalHandler<string> OnARVRServerInterfaceAdded(Action<string> action) =>
            new SignalHandler<string>(ARVRServer.Singleton, SignalConstants.ARVRServer_ARVRServerInterfaceAddedSignal, action);

        public static SignalHandler<string> OnARVRServerInterfaceRemoved(Action<string> action) =>
            new SignalHandler<string>(ARVRServer.Singleton, SignalConstants.ARVRServer_ARVRServerInterfaceRemovedSignal, action);

        public static SignalHandler<int, string, int> OnARVRServerTrackerAdded(Action<int, string, int> action) =>
            new SignalHandler<int, string, int>(ARVRServer.Singleton, SignalConstants.ARVRServer_ARVRServerTrackerAddedSignal, action);

        public static SignalHandler<int, string, int> OnARVRServerTrackerRemoved(Action<int, string, int> action) =>
            new SignalHandler<int, string, int>(ARVRServer.Singleton, SignalConstants.ARVRServer_ARVRServerTrackerRemovedSignal, action);

        public static SignalHandler OnAudioServerBusLayoutChanged(Action action) =>
            new SignalHandler(AudioServer.Singleton, SignalConstants.AudioServer_AudioServerBusLayoutChangedSignal, action);

        public static SignalHandler OnFinished(this AudioStreamPlayer target, Action action) =>
            new SignalHandler(target, SignalConstants.AudioStreamPlayer_FinishedSignal, action);

        public static SignalHandler OnFinished(this AudioStreamPlayer2D target, Action action) =>
            new SignalHandler(target, SignalConstants.AudioStreamPlayer2D_FinishedSignal, action);

        public static SignalHandler OnFinished(this AudioStreamPlayer3D target, Action action) =>
            new SignalHandler(target, SignalConstants.AudioStreamPlayer3D_FinishedSignal, action);

        public static SignalHandler OnButtonDown(this BaseButton target, Action action) =>
            new SignalHandler(target, SignalConstants.BaseButton_ButtonDownSignal, action);

        public static SignalHandler OnButtonUp(this BaseButton target, Action action) =>
            new SignalHandler(target, SignalConstants.BaseButton_ButtonUpSignal, action);

        public static SignalHandler OnPressed(this BaseButton target, Action action) =>
            new SignalHandler(target, SignalConstants.BaseButton_PressedSignal, action);

        public static SignalHandler<bool> OnToggled(this BaseButton target, Action<bool> action) =>
            new SignalHandler<bool>(target, SignalConstants.BaseButton_ToggledSignal, action);

        public static SignalHandler<Object> OnPressed(this ButtonGroup target, Action<Object> action) =>
            new SignalHandler<Object>(target, SignalConstants.ButtonGroup_PressedSignal, action);

        public static SignalHandler<int> OnCameraServerCameraFeedAdded(Action<int> action) =>
            new SignalHandler<int>(CameraServer.Singleton, SignalConstants.CameraServer_CameraServerCameraFeedAddedSignal, action);

        public static SignalHandler<int> OnCameraServerCameraFeedRemoved(Action<int> action) =>
            new SignalHandler<int>(CameraServer.Singleton, SignalConstants.CameraServer_CameraServerCameraFeedRemovedSignal, action);

        public static SignalHandler OnDraw(this CanvasItem target, Action action) =>
            new SignalHandler(target, SignalConstants.CanvasItem_DrawSignal, action);

        public static SignalHandler OnHide(this CanvasItem target, Action action) =>
            new SignalHandler(target, SignalConstants.CanvasItem_HideSignal, action);

        public static SignalHandler OnItemRectChanged(this CanvasItem target, Action action) =>
            new SignalHandler(target, SignalConstants.CanvasItem_ItemRectChangedSignal, action);

        public static SignalHandler OnVisibilityChanged(this CanvasItem target, Action action) =>
            new SignalHandler(target, SignalConstants.CanvasItem_VisibilityChangedSignal, action);

        public static SignalHandler<InputEvent, Node, Vector3, Vector3, int> OnInputEvent(this CollisionObject target, Action<InputEvent, Node, Vector3, Vector3, int> action) =>
            new SignalHandler<InputEvent, Node, Vector3, Vector3, int>(target, SignalConstants.CollisionObject_InputEventSignal, action);

        public static SignalHandler OnMouseEntered(this CollisionObject target, Action action) =>
            new SignalHandler(target, SignalConstants.CollisionObject_MouseEnteredSignal, action);

        public static SignalHandler OnMouseExited(this CollisionObject target, Action action) =>
            new SignalHandler(target, SignalConstants.CollisionObject_MouseExitedSignal, action);

        public static SignalHandler<InputEvent, int, Node> OnInputEvent(this CollisionObject2D target, Action<InputEvent, int, Node> action) =>
            new SignalHandler<InputEvent, int, Node>(target, SignalConstants.CollisionObject2D_InputEventSignal, action);

        public static SignalHandler OnMouseEntered(this CollisionObject2D target, Action action) =>
            new SignalHandler(target, SignalConstants.CollisionObject2D_MouseEnteredSignal, action);

        public static SignalHandler OnMouseExited(this CollisionObject2D target, Action action) =>
            new SignalHandler(target, SignalConstants.CollisionObject2D_MouseExitedSignal, action);

        public static SignalHandler<Color> OnColorChanged(this ColorPicker target, Action<Color> action) =>
            new SignalHandler<Color>(target, SignalConstants.ColorPicker_ColorChangedSignal, action);

        public static SignalHandler<Color> OnPresetAdded(this ColorPicker target, Action<Color> action) =>
            new SignalHandler<Color>(target, SignalConstants.ColorPicker_PresetAddedSignal, action);

        public static SignalHandler<Color> OnPresetRemoved(this ColorPicker target, Action<Color> action) =>
            new SignalHandler<Color>(target, SignalConstants.ColorPicker_PresetRemovedSignal, action);

        public static SignalHandler<Color> OnColorChanged(this ColorPickerButton target, Action<Color> action) =>
            new SignalHandler<Color>(target, SignalConstants.ColorPickerButton_ColorChangedSignal, action);

        public static SignalHandler OnPickerCreated(this ColorPickerButton target, Action action) =>
            new SignalHandler(target, SignalConstants.ColorPickerButton_PickerCreatedSignal, action);

        public static SignalHandler OnPopupClosed(this ColorPickerButton target, Action action) =>
            new SignalHandler(target, SignalConstants.ColorPickerButton_PopupClosedSignal, action);

        public static SignalHandler OnSortChildren(this Container target, Action action) =>
            new SignalHandler(target, SignalConstants.Container_SortChildrenSignal, action);

        public static SignalHandler OnFocusEntered(this Control target, Action action) =>
            new SignalHandler(target, SignalConstants.Control_FocusEnteredSignal, action);

        public static SignalHandler OnFocusExited(this Control target, Action action) =>
            new SignalHandler(target, SignalConstants.Control_FocusExitedSignal, action);

        public static SignalHandler<InputEvent> OnGuiInput(this Control target, Action<InputEvent> action) =>
            new SignalHandler<InputEvent>(target, SignalConstants.Control_GuiInputSignal, action);

        public static SignalHandler OnMinimumSizeChanged(this Control target, Action action) =>
            new SignalHandler(target, SignalConstants.Control_MinimumSizeChangedSignal, action);

        public static SignalHandler OnModalClosed(this Control target, Action action) =>
            new SignalHandler(target, SignalConstants.Control_ModalClosedSignal, action);

        public static SignalHandler OnMouseEntered(this Control target, Action action) =>
            new SignalHandler(target, SignalConstants.Control_MouseEnteredSignal, action);

        public static SignalHandler OnMouseExited(this Control target, Action action) =>
            new SignalHandler(target, SignalConstants.Control_MouseExitedSignal, action);

        public static SignalHandler OnResized(this Control target, Action action) =>
            new SignalHandler(target, SignalConstants.Control_ResizedSignal, action);

        public static SignalHandler OnSizeFlagsChanged(this Control target, Action action) =>
            new SignalHandler(target, SignalConstants.Control_SizeFlagsChangedSignal, action);

        public static SignalHandler OnRangeChanged(this Curve target, Action action) =>
            new SignalHandler(target, SignalConstants.Curve_RangeChangedSignal, action);

        public static SignalHandler<string> OnDirSelected(this FileDialog target, Action<string> action) =>
            new SignalHandler<string>(target, SignalConstants.FileDialog_DirSelectedSignal, action);

        public static SignalHandler<string> OnFileSelected(this FileDialog target, Action<string> action) =>
            new SignalHandler<string>(target, SignalConstants.FileDialog_FileSelectedSignal, action);

        public static SignalHandler<string[]> OnFilesSelected(this FileDialog target, Action<string[]> action) =>
            new SignalHandler<string[]>(target, SignalConstants.FileDialog_FilesSelectedSignal, action);

        public static SignalHandler<object> OnCompleted(this GDScriptFunctionState target, Action<object> action) =>
            new SignalHandler<object>(target, SignalConstants.GDScriptFunctionState_CompletedSignal, action);

        public static SignalHandler OnBeginNodeMove(this GraphEdit target, Action action) =>
            new SignalHandler(target, SignalConstants.GraphEdit_BeginNodeMoveSignal, action);

        public static SignalHandler<Vector2, string, int> OnConnectionFromEmpty(this GraphEdit target, Action<Vector2, string, int> action) =>
            new SignalHandler<Vector2, string, int>(target, SignalConstants.GraphEdit_ConnectionFromEmptySignal, action);

        public static SignalHandler<string, int, string, int> OnConnectionRequest(this GraphEdit target, Action<string, int, string, int> action) =>
            new SignalHandler<string, int, string, int>(target, SignalConstants.GraphEdit_ConnectionRequestSignal, action);

        public static SignalHandler<string, int, Vector2> OnConnectionToEmpty(this GraphEdit target, Action<string, int, Vector2> action) =>
            new SignalHandler<string, int, Vector2>(target, SignalConstants.GraphEdit_ConnectionToEmptySignal, action);

        public static SignalHandler OnCopyNodesRequest(this GraphEdit target, Action action) =>
            new SignalHandler(target, SignalConstants.GraphEdit_CopyNodesRequestSignal, action);

        public static SignalHandler OnDeleteNodesRequest(this GraphEdit target, Action action) =>
            new SignalHandler(target, SignalConstants.GraphEdit_DeleteNodesRequestSignal, action);

        public static SignalHandler<string, int, string, int> OnDisconnectionRequest(this GraphEdit target, Action<string, int, string, int> action) =>
            new SignalHandler<string, int, string, int>(target, SignalConstants.GraphEdit_DisconnectionRequestSignal, action);

        public static SignalHandler OnDuplicateNodesRequest(this GraphEdit target, Action action) =>
            new SignalHandler(target, SignalConstants.GraphEdit_DuplicateNodesRequestSignal, action);

        public static SignalHandler OnEndNodeMove(this GraphEdit target, Action action) =>
            new SignalHandler(target, SignalConstants.GraphEdit_EndNodeMoveSignal, action);

        public static SignalHandler<Node> OnNodeSelected(this GraphEdit target, Action<Node> action) =>
            new SignalHandler<Node>(target, SignalConstants.GraphEdit_NodeSelectedSignal, action);

        public static SignalHandler<Node> OnNodeUnselected(this GraphEdit target, Action<Node> action) =>
            new SignalHandler<Node>(target, SignalConstants.GraphEdit_NodeUnselectedSignal, action);

        public static SignalHandler OnPasteNodesRequest(this GraphEdit target, Action action) =>
            new SignalHandler(target, SignalConstants.GraphEdit_PasteNodesRequestSignal, action);

        public static SignalHandler<Vector2> OnPopupRequest(this GraphEdit target, Action<Vector2> action) =>
            new SignalHandler<Vector2>(target, SignalConstants.GraphEdit_PopupRequestSignal, action);

        public static SignalHandler<Vector2> OnScrollOffsetChanged(this GraphEdit target, Action<Vector2> action) =>
            new SignalHandler<Vector2>(target, SignalConstants.GraphEdit_ScrollOffsetChangedSignal, action);

        public static SignalHandler OnCloseRequest(this GraphNode target, Action action) =>
            new SignalHandler(target, SignalConstants.GraphNode_CloseRequestSignal, action);

        public static SignalHandler<Vector2, Vector2> OnDragged(this GraphNode target, Action<Vector2, Vector2> action) =>
            new SignalHandler<Vector2, Vector2>(target, SignalConstants.GraphNode_DraggedSignal, action);

        public static SignalHandler OnOffsetChanged(this GraphNode target, Action action) =>
            new SignalHandler(target, SignalConstants.GraphNode_OffsetChangedSignal, action);

        public static SignalHandler OnRaiseRequest(this GraphNode target, Action action) =>
            new SignalHandler(target, SignalConstants.GraphNode_RaiseRequestSignal, action);

        public static SignalHandler<Vector2> OnResizeRequest(this GraphNode target, Action<Vector2> action) =>
            new SignalHandler<Vector2>(target, SignalConstants.GraphNode_ResizeRequestSignal, action);

        public static SignalHandler<int> OnSlotUpdated(this GraphNode target, Action<int> action) =>
            new SignalHandler<int>(target, SignalConstants.GraphNode_SlotUpdatedSignal, action);

        public static SignalHandler<Vector3> OnCellSizeChanged(this GridMap target, Action<Vector3> action) =>
            new SignalHandler<Vector3>(target, SignalConstants.GridMap_CellSizeChangedSignal, action);

        public static SignalHandler<byte[], string[], int, int> OnRequestCompleted(this HTTPRequest target, Action<byte[], string[], int, int> action) =>
            new SignalHandler<byte[], string[], int, int>(target, SignalConstants.HTTPRequest_RequestCompletedSignal, action);

        public static SignalHandler<bool, int> OnInputJoyConnectionChanged(Action<bool, int> action) =>
            new SignalHandler<bool, int>(Input.Singleton, SignalConstants.Input_InputJoyConnectionChangedSignal, action);

        public static SignalHandler<int> OnItemActivated(this ItemList target, Action<int> action) =>
            new SignalHandler<int>(target, SignalConstants.ItemList_ItemActivatedSignal, action);

        public static SignalHandler<Vector2, int> OnItemRmbSelected(this ItemList target, Action<Vector2, int> action) =>
            new SignalHandler<Vector2, int>(target, SignalConstants.ItemList_ItemRmbSelectedSignal, action);

        public static SignalHandler<int> OnItemSelected(this ItemList target, Action<int> action) =>
            new SignalHandler<int>(target, SignalConstants.ItemList_ItemSelectedSignal, action);

        public static SignalHandler<int, bool> OnMultiSelected(this ItemList target, Action<int, bool> action) =>
            new SignalHandler<int, bool>(target, SignalConstants.ItemList_MultiSelectedSignal, action);

        public static SignalHandler OnNothingSelected(this ItemList target, Action action) =>
            new SignalHandler(target, SignalConstants.ItemList_NothingSelectedSignal, action);

        public static SignalHandler<Vector2> OnRmbClicked(this ItemList target, Action<Vector2> action) =>
            new SignalHandler<Vector2>(target, SignalConstants.ItemList_RmbClickedSignal, action);

        public static SignalHandler<string> OnTextChanged(this LineEdit target, Action<string> action) =>
            new SignalHandler<string>(target, SignalConstants.LineEdit_TextChangedSignal, action);

        public static SignalHandler<string> OnTextChangeRejected(this LineEdit target, Action<string> action) =>
            new SignalHandler<string>(target, SignalConstants.LineEdit_TextChangeRejectedSignal, action);

        public static SignalHandler<string> OnTextEntered(this LineEdit target, Action<string> action) =>
            new SignalHandler<string>(target, SignalConstants.LineEdit_TextEnteredSignal, action);

        public static SignalHandler<bool, string> OnRequestPermissionsResult(this MainLoop target, Action<bool, string> action) =>
            new SignalHandler<bool, string>(target, SignalConstants.MainLoop_RequestPermissionsResultSignal, action);

        public static SignalHandler OnAboutToShow(this MenuButton target, Action action) =>
            new SignalHandler(target, SignalConstants.MenuButton_AboutToShowSignal, action);

        public static SignalHandler OnTextureChanged(this MeshInstance2D target, Action action) =>
            new SignalHandler(target, SignalConstants.MeshInstance2D_TextureChangedSignal, action);

        public static SignalHandler OnTextureChanged(this MultiMeshInstance2D target, Action action) =>
            new SignalHandler(target, SignalConstants.MultiMeshInstance2D_TextureChangedSignal, action);

        public static SignalHandler OnConnectedToServer(this MultiplayerAPI target, Action action) =>
            new SignalHandler(target, SignalConstants.MultiplayerAPI_ConnectedToServerSignal, action);

        public static SignalHandler OnConnectionFailed(this MultiplayerAPI target, Action action) =>
            new SignalHandler(target, SignalConstants.MultiplayerAPI_ConnectionFailedSignal, action);

        public static SignalHandler<int> OnNetworkPeerConnected(this MultiplayerAPI target, Action<int> action) =>
            new SignalHandler<int>(target, SignalConstants.MultiplayerAPI_NetworkPeerConnectedSignal, action);

        public static SignalHandler<int> OnNetworkPeerDisconnected(this MultiplayerAPI target, Action<int> action) =>
            new SignalHandler<int>(target, SignalConstants.MultiplayerAPI_NetworkPeerDisconnectedSignal, action);

        public static SignalHandler<int, byte[]> OnNetworkPeerPacket(this MultiplayerAPI target, Action<int, byte[]> action) =>
            new SignalHandler<int, byte[]>(target, SignalConstants.MultiplayerAPI_NetworkPeerPacketSignal, action);

        public static SignalHandler OnServerDisconnected(this MultiplayerAPI target, Action action) =>
            new SignalHandler(target, SignalConstants.MultiplayerAPI_ServerDisconnectedSignal, action);

        public static SignalHandler OnConnectionFailed(this NetworkedMultiplayerPeer target, Action action) =>
            new SignalHandler(target, SignalConstants.NetworkedMultiplayerPeer_ConnectionFailedSignal, action);

        public static SignalHandler OnConnectionSucceeded(this NetworkedMultiplayerPeer target, Action action) =>
            new SignalHandler(target, SignalConstants.NetworkedMultiplayerPeer_ConnectionSucceededSignal, action);

        public static SignalHandler<int> OnPeerConnected(this NetworkedMultiplayerPeer target, Action<int> action) =>
            new SignalHandler<int>(target, SignalConstants.NetworkedMultiplayerPeer_PeerConnectedSignal, action);

        public static SignalHandler<int> OnPeerDisconnected(this NetworkedMultiplayerPeer target, Action<int> action) =>
            new SignalHandler<int>(target, SignalConstants.NetworkedMultiplayerPeer_PeerDisconnectedSignal, action);

        public static SignalHandler OnServerDisconnected(this NetworkedMultiplayerPeer target, Action action) =>
            new SignalHandler(target, SignalConstants.NetworkedMultiplayerPeer_ServerDisconnectedSignal, action);

        public static SignalHandler OnTextureChanged(this NinePatchRect target, Action action) =>
            new SignalHandler(target, SignalConstants.NinePatchRect_TextureChangedSignal, action);

        public static SignalHandler OnReady(this Node target, Action action) =>
            new SignalHandler(target, SignalConstants.Node_ReadySignal, action);

        public static SignalHandler OnRenamed(this Node target, Action action) =>
            new SignalHandler(target, SignalConstants.Node_RenamedSignal, action);

        public static SignalHandler OnTreeEntered(this Node target, Action action) =>
            new SignalHandler(target, SignalConstants.Node_TreeEnteredSignal, action);

        public static SignalHandler OnTreeExited(this Node target, Action action) =>
            new SignalHandler(target, SignalConstants.Node_TreeExitedSignal, action);

        public static SignalHandler OnTreeExiting(this Node target, Action action) =>
            new SignalHandler(target, SignalConstants.Node_TreeExitingSignal, action);

        public static SignalHandler OnScriptChanged(this Object target, Action action) =>
            new SignalHandler(target, SignalConstants.Object_ScriptChangedSignal, action);

        public static SignalHandler<int> OnItemFocused(this OptionButton target, Action<int> action) =>
            new SignalHandler<int>(target, SignalConstants.OptionButton_ItemFocusedSignal, action);

        public static SignalHandler<int> OnItemSelected(this OptionButton target, Action<int> action) =>
            new SignalHandler<int>(target, SignalConstants.OptionButton_ItemSelectedSignal, action);

        public static SignalHandler OnCurveChanged(this Path target, Action action) =>
            new SignalHandler(target, SignalConstants.Path_CurveChangedSignal, action);

        public static SignalHandler OnAboutToShow(this Popup target, Action action) =>
            new SignalHandler(target, SignalConstants.Popup_AboutToShowSignal, action);

        public static SignalHandler OnPopupHide(this Popup target, Action action) =>
            new SignalHandler(target, SignalConstants.Popup_PopupHideSignal, action);

        public static SignalHandler<int> OnIdFocused(this PopupMenu target, Action<int> action) =>
            new SignalHandler<int>(target, SignalConstants.PopupMenu_IdFocusedSignal, action);

        public static SignalHandler<int> OnIdPressed(this PopupMenu target, Action<int> action) =>
            new SignalHandler<int>(target, SignalConstants.PopupMenu_IdPressedSignal, action);

        public static SignalHandler<int> OnIndexPressed(this PopupMenu target, Action<int> action) =>
            new SignalHandler<int>(target, SignalConstants.PopupMenu_IndexPressedSignal, action);

        public static SignalHandler<string, Godot.Collections.Array> OnBroadcast(this ProximityGroup target, Action<string, Godot.Collections.Array> action) =>
            new SignalHandler<string, Godot.Collections.Array>(target, SignalConstants.ProximityGroup_BroadcastSignal, action);

        public static SignalHandler OnChanged(this Range target, Action action) =>
            new SignalHandler(target, SignalConstants.Range_ChangedSignal, action);

        public static SignalHandler<float> OnValueChanged(this Range target, Action<float> action) =>
            new SignalHandler<float>(target, SignalConstants.Range_ValueChangedSignal, action);

        public static SignalHandler OnChanged(this Resource target, Action action) =>
            new SignalHandler(target, SignalConstants.Resource_ChangedSignal, action);

        public static SignalHandler<object> OnMetaClicked(this RichTextLabel target, Action<object> action) =>
            new SignalHandler<object>(target, SignalConstants.RichTextLabel_MetaClickedSignal, action);

        public static SignalHandler<object> OnMetaHoverEnded(this RichTextLabel target, Action<object> action) =>
            new SignalHandler<object>(target, SignalConstants.RichTextLabel_MetaHoverEndedSignal, action);

        public static SignalHandler<object> OnMetaHoverStarted(this RichTextLabel target, Action<object> action) =>
            new SignalHandler<object>(target, SignalConstants.RichTextLabel_MetaHoverStartedSignal, action);

        public static SignalHandler<Node> OnBodyEntered(this RigidBody target, Action<Node> action) =>
            new SignalHandler<Node>(target, SignalConstants.RigidBody_BodyEnteredSignal, action);

        public static SignalHandler<Node> OnBodyExited(this RigidBody target, Action<Node> action) =>
            new SignalHandler<Node>(target, SignalConstants.RigidBody_BodyExitedSignal, action);

        public static SignalHandler<Node, RID, int, int> OnBodyShapeEntered(this RigidBody target, Action<Node, RID, int, int> action) =>
            new SignalHandler<Node, RID, int, int>(target, SignalConstants.RigidBody_BodyShapeEnteredSignal, action);

        public static SignalHandler<Node, RID, int, int> OnBodyShapeExited(this RigidBody target, Action<Node, RID, int, int> action) =>
            new SignalHandler<Node, RID, int, int>(target, SignalConstants.RigidBody_BodyShapeExitedSignal, action);

        public static SignalHandler OnSleepingStateChanged(this RigidBody target, Action action) =>
            new SignalHandler(target, SignalConstants.RigidBody_SleepingStateChangedSignal, action);

        public static SignalHandler<Node> OnBodyEntered(this RigidBody2D target, Action<Node> action) =>
            new SignalHandler<Node>(target, SignalConstants.RigidBody2D_BodyEnteredSignal, action);

        public static SignalHandler<Node> OnBodyExited(this RigidBody2D target, Action<Node> action) =>
            new SignalHandler<Node>(target, SignalConstants.RigidBody2D_BodyExitedSignal, action);

        public static SignalHandler<Node, RID, int, int> OnBodyShapeEntered(this RigidBody2D target, Action<Node, RID, int, int> action) =>
            new SignalHandler<Node, RID, int, int>(target, SignalConstants.RigidBody2D_BodyShapeEnteredSignal, action);

        public static SignalHandler<Node, RID, int, int> OnBodyShapeExited(this RigidBody2D target, Action<Node, RID, int, int> action) =>
            new SignalHandler<Node, RID, int, int>(target, SignalConstants.RigidBody2D_BodyShapeExitedSignal, action);

        public static SignalHandler OnSleepingStateChanged(this RigidBody2D target, Action action) =>
            new SignalHandler(target, SignalConstants.RigidBody2D_SleepingStateChangedSignal, action);

        public static SignalHandler OnConnectedToServer(this SceneTree target, Action action) =>
            new SignalHandler(target, SignalConstants.SceneTree_ConnectedToServerSignal, action);

        public static SignalHandler OnConnectionFailed(this SceneTree target, Action action) =>
            new SignalHandler(target, SignalConstants.SceneTree_ConnectionFailedSignal, action);

        public static SignalHandler<string[], int> OnFilesDropped(this SceneTree target, Action<string[], int> action) =>
            new SignalHandler<string[], int>(target, SignalConstants.SceneTree_FilesDroppedSignal, action);

        public static SignalHandler<object, object> OnGlobalMenuAction(this SceneTree target, Action<object, object> action) =>
            new SignalHandler<object, object>(target, SignalConstants.SceneTree_GlobalMenuActionSignal, action);

        public static SignalHandler OnIdleFrame(this SceneTree target, Action action) =>
            new SignalHandler(target, SignalConstants.SceneTree_IdleFrameSignal, action);

        public static SignalHandler<int> OnNetworkPeerConnected(this SceneTree target, Action<int> action) =>
            new SignalHandler<int>(target, SignalConstants.SceneTree_NetworkPeerConnectedSignal, action);

        public static SignalHandler<int> OnNetworkPeerDisconnected(this SceneTree target, Action<int> action) =>
            new SignalHandler<int>(target, SignalConstants.SceneTree_NetworkPeerDisconnectedSignal, action);

        public static SignalHandler<Node> OnNodeAdded(this SceneTree target, Action<Node> action) =>
            new SignalHandler<Node>(target, SignalConstants.SceneTree_NodeAddedSignal, action);

        public static SignalHandler<Node> OnNodeConfigurationWarningChanged(this SceneTree target, Action<Node> action) =>
            new SignalHandler<Node>(target, SignalConstants.SceneTree_NodeConfigurationWarningChangedSignal, action);

        public static SignalHandler<Node> OnNodeRemoved(this SceneTree target, Action<Node> action) =>
            new SignalHandler<Node>(target, SignalConstants.SceneTree_NodeRemovedSignal, action);

        public static SignalHandler<Node> OnNodeRenamed(this SceneTree target, Action<Node> action) =>
            new SignalHandler<Node>(target, SignalConstants.SceneTree_NodeRenamedSignal, action);

        public static SignalHandler OnPhysicsFrame(this SceneTree target, Action action) =>
            new SignalHandler(target, SignalConstants.SceneTree_PhysicsFrameSignal, action);

        public static SignalHandler OnScreenResized(this SceneTree target, Action action) =>
            new SignalHandler(target, SignalConstants.SceneTree_ScreenResizedSignal, action);

        public static SignalHandler OnServerDisconnected(this SceneTree target, Action action) =>
            new SignalHandler(target, SignalConstants.SceneTree_ServerDisconnectedSignal, action);

        public static SignalHandler OnTreeChanged(this SceneTree target, Action action) =>
            new SignalHandler(target, SignalConstants.SceneTree_TreeChangedSignal, action);

        public static SignalHandler OnTimeout(this SceneTreeTimer target, Action action) =>
            new SignalHandler(target, SignalConstants.SceneTreeTimer_TimeoutSignal, action);

        public static SignalHandler OnScrolling(this ScrollBar target, Action action) =>
            new SignalHandler(target, SignalConstants.ScrollBar_ScrollingSignal, action);

        public static SignalHandler OnScrollEnded(this ScrollContainer target, Action action) =>
            new SignalHandler(target, SignalConstants.ScrollContainer_ScrollEndedSignal, action);

        public static SignalHandler OnScrollStarted(this ScrollContainer target, Action action) =>
            new SignalHandler(target, SignalConstants.ScrollContainer_ScrollStartedSignal, action);

        public static SignalHandler OnSkeletonUpdated(this Skeleton target, Action action) =>
            new SignalHandler(target, SignalConstants.Skeleton_SkeletonUpdatedSignal, action);

        public static SignalHandler OnBoneSetupChanged(this Skeleton2D target, Action action) =>
            new SignalHandler(target, SignalConstants.Skeleton2D_BoneSetupChangedSignal, action);

        public static SignalHandler OnGameplayEntered(this Spatial target, Action action) =>
            new SignalHandler(target, SignalConstants.Spatial_GameplayEnteredSignal, action);

        public static SignalHandler OnGameplayExited(this Spatial target, Action action) =>
            new SignalHandler(target, SignalConstants.Spatial_GameplayExitedSignal, action);

        public static SignalHandler OnVisibilityChanged(this Spatial target, Action action) =>
            new SignalHandler(target, SignalConstants.Spatial_VisibilityChangedSignal, action);

        public static SignalHandler<int> OnDragged(this SplitContainer target, Action<int> action) =>
            new SignalHandler<int>(target, SignalConstants.SplitContainer_DraggedSignal, action);

        public static SignalHandler OnFrameChanged(this Sprite target, Action action) =>
            new SignalHandler(target, SignalConstants.Sprite_FrameChangedSignal, action);

        public static SignalHandler OnTextureChanged(this Sprite target, Action action) =>
            new SignalHandler(target, SignalConstants.Sprite_TextureChangedSignal, action);

        public static SignalHandler OnFrameChanged(this Sprite3D target, Action action) =>
            new SignalHandler(target, SignalConstants.Sprite3D_FrameChangedSignal, action);

        public static SignalHandler OnTextureChanged(this StyleBoxTexture target, Action action) =>
            new SignalHandler(target, SignalConstants.StyleBoxTexture_TextureChangedSignal, action);

        public static SignalHandler OnPrePopupPressed(this TabContainer target, Action action) =>
            new SignalHandler(target, SignalConstants.TabContainer_PrePopupPressedSignal, action);

        public static SignalHandler<int> OnTabChanged(this TabContainer target, Action<int> action) =>
            new SignalHandler<int>(target, SignalConstants.TabContainer_TabChangedSignal, action);

        public static SignalHandler<int> OnTabSelected(this TabContainer target, Action<int> action) =>
            new SignalHandler<int>(target, SignalConstants.TabContainer_TabSelectedSignal, action);

        public static SignalHandler<int> OnRepositionActiveTabRequest(this Tabs target, Action<int> action) =>
            new SignalHandler<int>(target, SignalConstants.Tabs_RepositionActiveTabRequestSignal, action);

        public static SignalHandler<int> OnRightButtonPressed(this Tabs target, Action<int> action) =>
            new SignalHandler<int>(target, SignalConstants.Tabs_RightButtonPressedSignal, action);

        public static SignalHandler<int> OnTabChanged(this Tabs target, Action<int> action) =>
            new SignalHandler<int>(target, SignalConstants.Tabs_TabChangedSignal, action);

        public static SignalHandler<int> OnTabClicked(this Tabs target, Action<int> action) =>
            new SignalHandler<int>(target, SignalConstants.Tabs_TabClickedSignal, action);

        public static SignalHandler<int> OnTabClose(this Tabs target, Action<int> action) =>
            new SignalHandler<int>(target, SignalConstants.Tabs_TabCloseSignal, action);

        public static SignalHandler<int> OnTabHover(this Tabs target, Action<int> action) =>
            new SignalHandler<int>(target, SignalConstants.Tabs_TabHoverSignal, action);

        public static SignalHandler<int> OnBreakpointToggled(this TextEdit target, Action<int> action) =>
            new SignalHandler<int>(target, SignalConstants.TextEdit_BreakpointToggledSignal, action);

        public static SignalHandler OnCursorChanged(this TextEdit target, Action action) =>
            new SignalHandler(target, SignalConstants.TextEdit_CursorChangedSignal, action);

        public static SignalHandler<string, int> OnInfoClicked(this TextEdit target, Action<string, int> action) =>
            new SignalHandler<string, int>(target, SignalConstants.TextEdit_InfoClickedSignal, action);

        public static SignalHandler OnRequestCompletion(this TextEdit target, Action action) =>
            new SignalHandler(target, SignalConstants.TextEdit_RequestCompletionSignal, action);

        public static SignalHandler<int, int, string> OnSymbolLookup(this TextEdit target, Action<int, int, string> action) =>
            new SignalHandler<int, int, string>(target, SignalConstants.TextEdit_SymbolLookupSignal, action);

        public static SignalHandler OnTextChanged(this TextEdit target, Action action) =>
            new SignalHandler(target, SignalConstants.TextEdit_TextChangedSignal, action);

        public static SignalHandler OnSettingsChanged(this TileMap target, Action action) =>
            new SignalHandler(target, SignalConstants.TileMap_SettingsChangedSignal, action);

        public static SignalHandler OnTimeout(this Timer target, Action action) =>
            new SignalHandler(target, SignalConstants.Timer_TimeoutSignal, action);

        public static SignalHandler OnPressed(this TouchScreenButton target, Action action) =>
            new SignalHandler(target, SignalConstants.TouchScreenButton_PressedSignal, action);

        public static SignalHandler OnReleased(this TouchScreenButton target, Action action) =>
            new SignalHandler(target, SignalConstants.TouchScreenButton_ReleasedSignal, action);

        public static SignalHandler<int, int, TreeItem> OnButtonPressed(this Tree target, Action<int, int, TreeItem> action) =>
            new SignalHandler<int, int, TreeItem>(target, SignalConstants.Tree_ButtonPressedSignal, action);

        public static SignalHandler OnCellSelected(this Tree target, Action action) =>
            new SignalHandler(target, SignalConstants.Tree_CellSelectedSignal, action);

        public static SignalHandler<int> OnColumnTitlePressed(this Tree target, Action<int> action) =>
            new SignalHandler<int>(target, SignalConstants.Tree_ColumnTitlePressedSignal, action);

        public static SignalHandler<bool> OnCustomPopupEdited(this Tree target, Action<bool> action) =>
            new SignalHandler<bool>(target, SignalConstants.Tree_CustomPopupEditedSignal, action);

        public static SignalHandler<Vector2> OnEmptyRmb(this Tree target, Action<Vector2> action) =>
            new SignalHandler<Vector2>(target, SignalConstants.Tree_EmptyRmbSignal, action);

        public static SignalHandler<Vector2> OnEmptyTreeRmbSelected(this Tree target, Action<Vector2> action) =>
            new SignalHandler<Vector2>(target, SignalConstants.Tree_EmptyTreeRmbSelectedSignal, action);

        public static SignalHandler OnItemActivated(this Tree target, Action action) =>
            new SignalHandler(target, SignalConstants.Tree_ItemActivatedSignal, action);

        public static SignalHandler<TreeItem> OnItemCollapsed(this Tree target, Action<TreeItem> action) =>
            new SignalHandler<TreeItem>(target, SignalConstants.Tree_ItemCollapsedSignal, action);

        public static SignalHandler OnItemCustomButtonPressed(this Tree target, Action action) =>
            new SignalHandler(target, SignalConstants.Tree_ItemCustomButtonPressedSignal, action);

        public static SignalHandler OnItemDoubleClicked(this Tree target, Action action) =>
            new SignalHandler(target, SignalConstants.Tree_ItemDoubleClickedSignal, action);

        public static SignalHandler OnItemEdited(this Tree target, Action action) =>
            new SignalHandler(target, SignalConstants.Tree_ItemEditedSignal, action);

        public static SignalHandler OnItemRmbEdited(this Tree target, Action action) =>
            new SignalHandler(target, SignalConstants.Tree_ItemRmbEditedSignal, action);

        public static SignalHandler<Vector2> OnItemRmbSelected(this Tree target, Action<Vector2> action) =>
            new SignalHandler<Vector2>(target, SignalConstants.Tree_ItemRmbSelectedSignal, action);

        public static SignalHandler OnItemSelected(this Tree target, Action action) =>
            new SignalHandler(target, SignalConstants.Tree_ItemSelectedSignal, action);

        public static SignalHandler<int, TreeItem, bool> OnMultiSelected(this Tree target, Action<int, TreeItem, bool> action) =>
            new SignalHandler<int, TreeItem, bool>(target, SignalConstants.Tree_MultiSelectedSignal, action);

        public static SignalHandler OnNothingSelected(this Tree target, Action action) =>
            new SignalHandler(target, SignalConstants.Tree_NothingSelectedSignal, action);

        public static SignalHandler OnTweenAllCompleted(this Tween target, Action action) =>
            new SignalHandler(target, SignalConstants.Tween_TweenAllCompletedSignal, action);

        public static SignalHandler<Object, NodePath> OnTweenCompleted(this Tween target, Action<Object, NodePath> action) =>
            new SignalHandler<Object, NodePath>(target, SignalConstants.Tween_TweenCompletedSignal, action);

        public static SignalHandler<Object, NodePath> OnTweenStarted(this Tween target, Action<Object, NodePath> action) =>
            new SignalHandler<Object, NodePath>(target, SignalConstants.Tween_TweenStartedSignal, action);

        public static SignalHandler<Object, float, NodePath, Object> OnTweenStep(this Tween target, Action<Object, float, NodePath, Object> action) =>
            new SignalHandler<Object, float, NodePath, Object>(target, SignalConstants.Tween_TweenStepSignal, action);

        public static SignalHandler OnVersionChanged(this UndoRedo target, Action action) =>
            new SignalHandler(target, SignalConstants.UndoRedo_VersionChangedSignal, action);

        public static SignalHandler OnFinished(this VideoPlayer target, Action action) =>
            new SignalHandler(target, SignalConstants.VideoPlayer_FinishedSignal, action);

        public static SignalHandler<Control> OnGuiFocusChanged(this Viewport target, Action<Control> action) =>
            new SignalHandler<Control>(target, SignalConstants.Viewport_GuiFocusChangedSignal, action);

        public static SignalHandler OnSizeChanged(this Viewport target, Action action) =>
            new SignalHandler(target, SignalConstants.Viewport_SizeChangedSignal, action);

        public static SignalHandler<Camera> OnCameraEntered(this VisibilityNotifier target, Action<Camera> action) =>
            new SignalHandler<Camera>(target, SignalConstants.VisibilityNotifier_CameraEnteredSignal, action);

        public static SignalHandler<Camera> OnCameraExited(this VisibilityNotifier target, Action<Camera> action) =>
            new SignalHandler<Camera>(target, SignalConstants.VisibilityNotifier_CameraExitedSignal, action);

        public static SignalHandler OnScreenEntered(this VisibilityNotifier target, Action action) =>
            new SignalHandler(target, SignalConstants.VisibilityNotifier_ScreenEnteredSignal, action);

        public static SignalHandler OnScreenExited(this VisibilityNotifier target, Action action) =>
            new SignalHandler(target, SignalConstants.VisibilityNotifier_ScreenExitedSignal, action);

        public static SignalHandler OnScreenEntered(this VisibilityNotifier2D target, Action action) =>
            new SignalHandler(target, SignalConstants.VisibilityNotifier2D_ScreenEnteredSignal, action);

        public static SignalHandler OnScreenExited(this VisibilityNotifier2D target, Action action) =>
            new SignalHandler(target, SignalConstants.VisibilityNotifier2D_ScreenExitedSignal, action);

        public static SignalHandler<Viewport> OnViewportEntered(this VisibilityNotifier2D target, Action<Viewport> action) =>
            new SignalHandler<Viewport>(target, SignalConstants.VisibilityNotifier2D_ViewportEnteredSignal, action);

        public static SignalHandler<Viewport> OnViewportExited(this VisibilityNotifier2D target, Action<Viewport> action) =>
            new SignalHandler<Viewport>(target, SignalConstants.VisibilityNotifier2D_ViewportExitedSignal, action);

        public static SignalHandler<string, int> OnNodePortsChanged(this VisualScript target, Action<string, int> action) =>
            new SignalHandler<string, int>(target, SignalConstants.VisualScript_NodePortsChangedSignal, action);

        public static SignalHandler OnPortsChanged(this VisualScriptNode target, Action action) =>
            new SignalHandler(target, SignalConstants.VisualScriptNode_PortsChangedSignal, action);

        public static SignalHandler OnVisualServerFramePostDraw(Action action) =>
            new SignalHandler(VisualServer.Singleton, SignalConstants.VisualServer_VisualServerFramePostDrawSignal, action);

        public static SignalHandler OnVisualServerFramePreDraw(Action action) =>
            new SignalHandler(VisualServer.Singleton, SignalConstants.VisualServer_VisualServerFramePreDrawSignal, action);

        public static SignalHandler OnEditorRefreshRequest(this VisualShaderNode target, Action action) =>
            new SignalHandler(target, SignalConstants.VisualShaderNode_EditorRefreshRequestSignal, action);

        public static SignalHandler OnInputTypeChanged(this VisualShaderNodeInput target, Action action) =>
            new SignalHandler(target, SignalConstants.VisualShaderNodeInput_InputTypeChangedSignal, action);

        public static SignalHandler<Object> OnDataChannelReceived(this WebRTCPeerConnection target, Action<Object> action) =>
            new SignalHandler<Object>(target, SignalConstants.WebRTCPeerConnection_DataChannelReceivedSignal, action);

        public static SignalHandler<int, string, string> OnIceCandidateCreated(this WebRTCPeerConnection target, Action<int, string, string> action) =>
            new SignalHandler<int, string, string>(target, SignalConstants.WebRTCPeerConnection_IceCandidateCreatedSignal, action);

        public static SignalHandler<string, string> OnSessionDescriptionCreated(this WebRTCPeerConnection target, Action<string, string> action) =>
            new SignalHandler<string, string>(target, SignalConstants.WebRTCPeerConnection_SessionDescriptionCreatedSignal, action);

        public static SignalHandler<bool> OnConnectionClosed(this WebSocketClient target, Action<bool> action) =>
            new SignalHandler<bool>(target, SignalConstants.WebSocketClient_ConnectionClosedSignal, action);

        public static SignalHandler OnConnectionError(this WebSocketClient target, Action action) =>
            new SignalHandler(target, SignalConstants.WebSocketClient_ConnectionErrorSignal, action);

        public static SignalHandler<string> OnConnectionEstablished(this WebSocketClient target, Action<string> action) =>
            new SignalHandler<string>(target, SignalConstants.WebSocketClient_ConnectionEstablishedSignal, action);

        public static SignalHandler OnDataReceived(this WebSocketClient target, Action action) =>
            new SignalHandler(target, SignalConstants.WebSocketClient_DataReceivedSignal, action);

        public static SignalHandler<int, string> OnServerCloseRequest(this WebSocketClient target, Action<int, string> action) =>
            new SignalHandler<int, string>(target, SignalConstants.WebSocketClient_ServerCloseRequestSignal, action);

        public static SignalHandler<int> OnPeerPacket(this WebSocketMultiplayerPeer target, Action<int> action) =>
            new SignalHandler<int>(target, SignalConstants.WebSocketMultiplayerPeer_PeerPacketSignal, action);

        public static SignalHandler<int, int, string> OnClientCloseRequest(this WebSocketServer target, Action<int, int, string> action) =>
            new SignalHandler<int, int, string>(target, SignalConstants.WebSocketServer_ClientCloseRequestSignal, action);

        public static SignalHandler<int, string> OnClientConnected(this WebSocketServer target, Action<int, string> action) =>
            new SignalHandler<int, string>(target, SignalConstants.WebSocketServer_ClientConnectedSignal, action);

        public static SignalHandler<int, bool> OnClientDisconnected(this WebSocketServer target, Action<int, bool> action) =>
            new SignalHandler<int, bool>(target, SignalConstants.WebSocketServer_ClientDisconnectedSignal, action);

        public static SignalHandler<int> OnDataReceived(this WebSocketServer target, Action<int> action) =>
            new SignalHandler<int>(target, SignalConstants.WebSocketServer_DataReceivedSignal, action);

        public static SignalHandler OnReferenceSpaceReset(this WebXRInterface target, Action action) =>
            new SignalHandler(target, SignalConstants.WebXRInterface_ReferenceSpaceResetSignal, action);

        public static SignalHandler<int> OnSelect(this WebXRInterface target, Action<int> action) =>
            new SignalHandler<int>(target, SignalConstants.WebXRInterface_SelectSignal, action);

        public static SignalHandler<int> OnSelectend(this WebXRInterface target, Action<int> action) =>
            new SignalHandler<int>(target, SignalConstants.WebXRInterface_SelectendSignal, action);

        public static SignalHandler<int> OnSelectstart(this WebXRInterface target, Action<int> action) =>
            new SignalHandler<int>(target, SignalConstants.WebXRInterface_SelectstartSignal, action);

        public static SignalHandler OnSessionEnded(this WebXRInterface target, Action action) =>
            new SignalHandler(target, SignalConstants.WebXRInterface_SessionEndedSignal, action);

        public static SignalHandler<string> OnSessionFailed(this WebXRInterface target, Action<string> action) =>
            new SignalHandler<string>(target, SignalConstants.WebXRInterface_SessionFailedSignal, action);

        public static SignalHandler OnSessionStarted(this WebXRInterface target, Action action) =>
            new SignalHandler(target, SignalConstants.WebXRInterface_SessionStartedSignal, action);

        public static SignalHandler<string, bool> OnSessionSupported(this WebXRInterface target, Action<string, bool> action) =>
            new SignalHandler<string, bool>(target, SignalConstants.WebXRInterface_SessionSupportedSignal, action);

        public static SignalHandler<int> OnSqueeze(this WebXRInterface target, Action<int> action) =>
            new SignalHandler<int>(target, SignalConstants.WebXRInterface_SqueezeSignal, action);

        public static SignalHandler<int> OnSqueezeend(this WebXRInterface target, Action<int> action) =>
            new SignalHandler<int>(target, SignalConstants.WebXRInterface_SqueezeendSignal, action);

        public static SignalHandler<int> OnSqueezestart(this WebXRInterface target, Action<int> action) =>
            new SignalHandler<int>(target, SignalConstants.WebXRInterface_SqueezestartSignal, action);

        public static SignalHandler OnVisibilityStateChanged(this WebXRInterface target, Action action) =>
            new SignalHandler(target, SignalConstants.WebXRInterface_VisibilityStateChangedSignal, action);
    }
}