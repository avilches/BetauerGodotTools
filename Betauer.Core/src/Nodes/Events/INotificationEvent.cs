using System;

namespace Betauer.Core.Nodes.Events;

public interface INotificationEvent : IProcessEvent, IPhysicsProcessEvent {

	// NotificationEnterTree disabled. There is already a Godot.Node.TreeEntered event name and "_EnterTree" virtual method name
	// NotificationExitTree disabled. There is already a Godot.Node.TreeExiting event name and "_ExitTree" virtual method name
	// NotificationReady disabled. There is already a Godot.Node.Ready event name and "_Ready" virtual method name

	/// <summary>
	/// <para>Event called when the object is initialized, before its script is attached. Used internally.</para>
	/// </summary>
	public event Action OnPostinitialize;
	/// <summary>
	/// <para>Event called when the object is about to be deleted. Can act as the deconstructor of some programming languages.</para>
	/// </summary>
	public event Action OnPredelete;
    /// <summary>
    /// <para>Event called when the node is paused.</para>
    /// </summary>
    public event Action OnPaused;
    /// <summary>
    /// <para>Event called when the node is unpaused.</para>
    /// </summary>
    public event Action OnUnpaused;
    /// <summary>
    /// <para>Event called when a node is set as a child of another node.</para>
    /// <para><b>Note:</b> This doesn't mean that a node entered the <see cref="T:Godot.SceneTree" />.</para>
    /// </summary>
    public event Action OnParented;
    /// <summary>
    /// <para>Event called when a node is unparented (parent removed it from the list of children).</para>
    /// </summary>
    public event Action OnUnparented;
    /// <summary>
    /// <para>Event called by scene owner when its scene is instantiated.</para>
    /// </summary>
    public event Action OnSceneInstantiated;
    /// <summary>
    /// <para>Event called when a drag operation begins. All nodes receive this notification, not only the dragged one.</para>
    /// <para>Can be triggered either by dragging a <see cref="T:Godot.Control" /> that provides drag data (see <see cref="M:Godot.Control._GetDragData(Godot.Vector2)" />) or using <see cref="M:Godot.Control.ForceDrag(Godot.Variant,Godot.Control)" />.</para>
    /// <para>Use <see cref="M:Godot.Viewport.GuiGetDragData" /> to get the dragged data.</para>
    /// </summary>
    public event Action OnDragBegin;
    /// <summary>
    /// <para>Event called when a drag operation ends.</para>
    /// <para>Use <see cref="M:Godot.Viewport.GuiIsDragSuccessful" /> to check if the drag succeeded.</para>
    /// </summary>
    public event Action OnDragEnd;
    /// <summary>
    /// <para>Event called when the node's name or one of its parents' name is changed. This notification is <i>not</i> received when the node is removed from the scene tree to be added to another parent later on.</para>
    /// </summary>
    public event Action OnPathRenamed;
    /// <summary>
    /// <para>Event called when the list of children is changed. This happens when child nodes are added, moved or removed.</para>
    /// </summary>
    public event Action OnChildOrderChanged;
    /// <summary>
    /// <para>Event called every frame when the internal process flag is set (see <see cref="M:Godot.Node.SetProcessInternal(System.Boolean)" />).</para>
    /// </summary>
    public event Action OnInternalProcess;
    /// <summary>
    /// <para>Event called every frame when the internal physics process flag is set (see <see cref="M:Godot.Node.SetPhysicsProcessInternal(System.Boolean)" />).</para>
    /// </summary>
    public event Action OnInternalPhysicsProcess;
    /// <summary>
    /// <para>Event called when the node is ready, just before <see cref="F:Godot.Node.NotificationReady" /> is received. Unlike the latter, it's sent every time the node enters tree, instead of only once.</para>
    /// </summary>
    public event Action OnPostEnterTree;
    /// <summary>
    /// <para>Event called when the node is disabled. See <see cref="F:Godot.Node.ProcessModeEnum.Disabled" />.</para>
    /// </summary>
    public event Action OnDisabled;
    /// <summary>
    /// <para>Event called when the node is enabled again after being disabled. See <see cref="F:Godot.Node.ProcessModeEnum.Disabled" />.</para>
    /// </summary>
    public event Action OnEnabled;
    /// <summary>
    /// <para>Event called when other nodes in the tree may have been removed/replaced and node pointers may require re-caching.</para>
    /// </summary>
    public event Action OnNodeRecacheRequested;
    /// <summary>
    /// <para>Event called right before the scene with the node is saved in the editor. This notification is only sent in the Godot editor and will not occur in exported projects.</para>
    /// </summary>
    public event Action OnEditorPreSave;
    /// <summary>
    /// <para>Event called right after the scene with the node is saved in the editor. This notification is only sent in the Godot editor and will not occur in exported projects.</para>
    /// </summary>
    public event Action OnEditorPostSave;
    /// <summary>
    /// <para>Event called from the OS when the mouse enters the game window.</para>
    /// <para>Implemented on desktop and web platforms.</para>
    /// </summary>
    public event Action OnWMMouseEnter;
    /// <summary>
    /// <para>Event called from the OS when the mouse leaves the game window.</para>
    /// <para>Implemented on desktop and web platforms.</para>
    /// </summary>
    public event Action OnWMMouseExit;
    /// <summary>
    /// <para>Event called when the node's parent <see cref="T:Godot.Window" /> is focused. This may be a change of focus between two windows of the same engine instance, or from the OS desktop or a third-party application to a window of the game (in which case <see cref="F:Godot.Node.NotificationApplicationFocusIn" /> is also emitted).</para>
    /// <para>A <see cref="T:Godot.Window" /> node receives this notification when it is focused.</para>
    /// </summary>
    public event Action OnWMWindowFocusIn;
    /// <summary>
    /// <para>Event called when the node's parent <see cref="T:Godot.Window" /> is defocused. This may be a change of focus between two windows of the same engine instance, or from a window of the game to the OS desktop or a third-party application (in which case <see cref="F:Godot.Node.NotificationApplicationFocusOut" /> is also emitted).</para>
    /// <para>A <see cref="T:Godot.Window" /> node receives this notification when it is defocused.</para>
    /// </summary>
    public event Action OnWMWindowFocusOut;
    /// <summary>
    /// <para>Event called from the OS when a close request is sent (e.g. closing the window with a "Close" button or Alt + F4).</para>
    /// <para>Implemented on desktop platforms.</para>
    /// </summary>
    public event Action OnWMCloseRequest;
    /// <summary>
    /// <para>Event called from the OS when a go back request is sent (e.g. pressing the "Back" button on Android).</para>
    /// <para>Specific to the Android platform.</para>
    /// </summary>
    public event Action OnWMGoBackRequest;
    /// <summary>
    /// <para>Event called from the OS when the window is resized.</para>
    /// </summary>
    public event Action OnWMSizeChanged;
    /// <summary>
    /// <para>Event called from the OS when the screen's DPI has been changed. Only implemented on macOS.</para>
    /// </summary>
    public event Action OnWMDpiChange;
    /// <summary>
    /// <para>Event called when the mouse enters the viewport.</para>
    /// </summary>
    public event Action OnVpMouseEnter;
    /// <summary>
    /// <para>Event called when the mouse leaves the viewport.</para>
    /// </summary>
    public event Action OnVpMouseExit;
    /// <summary>
    /// <para>Event called from the OS when the application is exceeding its allocated memory.</para>
    /// <para>Specific to the iOS platform.</para>
    /// </summary>
    public event Action OnOsMemoryWarning;
    /// <summary>
    /// <para>Event called when translations may have changed. Can be triggered by the user changing the locale. Can be used to respond to language changes, for example to change the UI strings on the fly. Useful when working with the built-in translation support, like <see cref="M:Godot.GodotObject.Tr(Godot.StringName,Godot.StringName)" />.</para>
    /// </summary>
    public event Action OnTranslationChanged;
    /// <summary>
    /// <para>Event called from the OS when a request for "About" information is sent.</para>
    /// <para>Specific to the macOS platform.</para>
    /// </summary>
    public event Action OnWMAbout;
    /// <summary>
    /// <para>Event called from Godot's crash handler when the engine is about to crash.</para>
    /// <para>Implemented on desktop platforms if the crash handler is enabled.</para>
    /// </summary>
    public event Action OnCrash;
    /// <summary>
    /// <para>Event called from the OS when an update of the Input Method Engine occurs (e.g. change of IME cursor position or composition string).</para>
    /// <para>Specific to the macOS platform.</para>
    /// </summary>
    public event Action OnOsImeUpdate;
    /// <summary>
    /// <para>Event called from the OS when the application is resumed.</para>
    /// <para>Specific to the Android platform.</para>
    /// </summary>
    public event Action OnApplicationResumed;
    /// <summary>
    /// <para>Event called from the OS when the application is paused.</para>
    /// <para>Specific to the Android platform.</para>
    /// </summary>
    public event Action OnApplicationPaused;
    /// <summary>
    /// <para>Event called from the OS when the application is focused, i.e. when changing the focus from the OS desktop or a thirdparty application to any open window of the Godot instance.</para>
    /// <para>Implemented on desktop platforms.</para>
    /// </summary>
    public event Action OnApplicationFocusIn;
    /// <summary>
    /// <para>Event called from the OS when the application is defocused, i.e. when changing the focus from any open window of the Godot instance to the OS desktop or a thirdparty application.</para>
    /// <para>Implemented on desktop platforms.</para>
    /// </summary>
    public event Action OnApplicationFocusOut;
    /// <summary>
    /// <para>Event called when text server is changed.</para>
    /// </summary>
    public event Action OnTextServerChanged;
}