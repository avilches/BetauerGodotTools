using System;
using Godot;
using Object = Godot.Object;
using Animation = Godot.Animation;
using Environment = Godot.Environment;
using Betauer.GodotAction;

namespace Betauer {
    public static partial class GodotActionExtensions {

        private const string ProxyName = "__ProxyGodotAction__";

        public static T GetOrCreateProxy<T>(Node owner) where T : Node {
            T proxy = owner.GetNodeOrNull<T>(ProxyName);
            if (proxy == null) {
                proxy = Activator.CreateInstance<T>();
                proxy.Name = ProxyName;
                owner.AddChild(proxy);
            }
            return proxy;
        }

        public static AcceptDialogAction GetProxy(this AcceptDialog target) => 
                GetOrCreateProxy<AcceptDialogAction>(target);

        public static AnimatedSpriteAction GetProxy(this AnimatedSprite target) => 
                GetOrCreateProxy<AnimatedSpriteAction>(target);

        public static AnimatedSprite3DAction GetProxy(this AnimatedSprite3D target) => 
                GetOrCreateProxy<AnimatedSprite3DAction>(target);

        public static AnimationPlayerAction GetProxy(this AnimationPlayer target) => 
                GetOrCreateProxy<AnimationPlayerAction>(target);

        public static AnimationTreeAction GetProxy(this AnimationTree target) => 
                GetOrCreateProxy<AnimationTreeAction>(target);

        public static AnimationTreePlayerAction GetProxy(this AnimationTreePlayer target) => 
                GetOrCreateProxy<AnimationTreePlayerAction>(target);

        public static AreaAction GetProxy(this Area target) => 
                GetOrCreateProxy<AreaAction>(target);

        public static Area2DAction GetProxy(this Area2D target) => 
                GetOrCreateProxy<Area2DAction>(target);

        public static ARVRAnchorAction GetProxy(this ARVRAnchor target) => 
                GetOrCreateProxy<ARVRAnchorAction>(target);

        public static ARVRCameraAction GetProxy(this ARVRCamera target) => 
                GetOrCreateProxy<ARVRCameraAction>(target);

        public static ARVRControllerAction GetProxy(this ARVRController target) => 
                GetOrCreateProxy<ARVRControllerAction>(target);

        public static ARVROriginAction GetProxy(this ARVROrigin target) => 
                GetOrCreateProxy<ARVROriginAction>(target);

        public static AspectRatioContainerAction GetProxy(this AspectRatioContainer target) => 
                GetOrCreateProxy<AspectRatioContainerAction>(target);

        public static AudioStreamPlayerAction GetProxy(this AudioStreamPlayer target) => 
                GetOrCreateProxy<AudioStreamPlayerAction>(target);

        public static AudioStreamPlayer2DAction GetProxy(this AudioStreamPlayer2D target) => 
                GetOrCreateProxy<AudioStreamPlayer2DAction>(target);

        public static AudioStreamPlayer3DAction GetProxy(this AudioStreamPlayer3D target) => 
                GetOrCreateProxy<AudioStreamPlayer3DAction>(target);

        public static BackBufferCopyAction GetProxy(this BackBufferCopy target) => 
                GetOrCreateProxy<BackBufferCopyAction>(target);

        public static BakedLightmapAction GetProxy(this BakedLightmap target) => 
                GetOrCreateProxy<BakedLightmapAction>(target);

        public static Bone2DAction GetProxy(this Bone2D target) => 
                GetOrCreateProxy<Bone2DAction>(target);

        public static BoneAttachmentAction GetProxy(this BoneAttachment target) => 
                GetOrCreateProxy<BoneAttachmentAction>(target);

        public static ButtonAction GetProxy(this Button target) => 
                GetOrCreateProxy<ButtonAction>(target);

        public static CameraAction GetProxy(this Camera target) => 
                GetOrCreateProxy<CameraAction>(target);

        public static Camera2DAction GetProxy(this Camera2D target) => 
                GetOrCreateProxy<Camera2DAction>(target);

        public static CanvasLayerAction GetProxy(this CanvasLayer target) => 
                GetOrCreateProxy<CanvasLayerAction>(target);

        public static CanvasModulateAction GetProxy(this CanvasModulate target) => 
                GetOrCreateProxy<CanvasModulateAction>(target);

        public static CenterContainerAction GetProxy(this CenterContainer target) => 
                GetOrCreateProxy<CenterContainerAction>(target);

        public static CheckBoxAction GetProxy(this CheckBox target) => 
                GetOrCreateProxy<CheckBoxAction>(target);

        public static CheckButtonAction GetProxy(this CheckButton target) => 
                GetOrCreateProxy<CheckButtonAction>(target);

        public static ClippedCameraAction GetProxy(this ClippedCamera target) => 
                GetOrCreateProxy<ClippedCameraAction>(target);

        public static CollisionPolygonAction GetProxy(this CollisionPolygon target) => 
                GetOrCreateProxy<CollisionPolygonAction>(target);

        public static CollisionPolygon2DAction GetProxy(this CollisionPolygon2D target) => 
                GetOrCreateProxy<CollisionPolygon2DAction>(target);

        public static CollisionShapeAction GetProxy(this CollisionShape target) => 
                GetOrCreateProxy<CollisionShapeAction>(target);

        public static CollisionShape2DAction GetProxy(this CollisionShape2D target) => 
                GetOrCreateProxy<CollisionShape2DAction>(target);

        public static ColorPickerAction GetProxy(this ColorPicker target) => 
                GetOrCreateProxy<ColorPickerAction>(target);

        public static ColorPickerButtonAction GetProxy(this ColorPickerButton target) => 
                GetOrCreateProxy<ColorPickerButtonAction>(target);

        public static ColorRectAction GetProxy(this ColorRect target) => 
                GetOrCreateProxy<ColorRectAction>(target);

        public static ConeTwistJointAction GetProxy(this ConeTwistJoint target) => 
                GetOrCreateProxy<ConeTwistJointAction>(target);

        public static ConfirmationDialogAction GetProxy(this ConfirmationDialog target) => 
                GetOrCreateProxy<ConfirmationDialogAction>(target);

        public static ContainerAction GetProxy(this Container target) => 
                GetOrCreateProxy<ContainerAction>(target);

        public static ControlAction GetProxy(this Control target) => 
                GetOrCreateProxy<ControlAction>(target);

        public static CPUParticlesAction GetProxy(this CPUParticles target) => 
                GetOrCreateProxy<CPUParticlesAction>(target);

        public static CPUParticles2DAction GetProxy(this CPUParticles2D target) => 
                GetOrCreateProxy<CPUParticles2DAction>(target);

        public static CSGBoxAction GetProxy(this CSGBox target) => 
                GetOrCreateProxy<CSGBoxAction>(target);

        public static CSGCombinerAction GetProxy(this CSGCombiner target) => 
                GetOrCreateProxy<CSGCombinerAction>(target);

        public static CSGCylinderAction GetProxy(this CSGCylinder target) => 
                GetOrCreateProxy<CSGCylinderAction>(target);

        public static CSGMeshAction GetProxy(this CSGMesh target) => 
                GetOrCreateProxy<CSGMeshAction>(target);

        public static CSGPolygonAction GetProxy(this CSGPolygon target) => 
                GetOrCreateProxy<CSGPolygonAction>(target);

        public static CSGSphereAction GetProxy(this CSGSphere target) => 
                GetOrCreateProxy<CSGSphereAction>(target);

        public static CSGTorusAction GetProxy(this CSGTorus target) => 
                GetOrCreateProxy<CSGTorusAction>(target);

        public static DampedSpringJoint2DAction GetProxy(this DampedSpringJoint2D target) => 
                GetOrCreateProxy<DampedSpringJoint2DAction>(target);

        public static DirectionalLightAction GetProxy(this DirectionalLight target) => 
                GetOrCreateProxy<DirectionalLightAction>(target);

        public static FileDialogAction GetProxy(this FileDialog target) => 
                GetOrCreateProxy<FileDialogAction>(target);

        public static Generic6DOFJointAction GetProxy(this Generic6DOFJoint target) => 
                GetOrCreateProxy<Generic6DOFJointAction>(target);

        public static GIProbeAction GetProxy(this GIProbe target) => 
                GetOrCreateProxy<GIProbeAction>(target);

        public static GraphEditAction GetProxy(this GraphEdit target) => 
                GetOrCreateProxy<GraphEditAction>(target);

        public static GraphNodeAction GetProxy(this GraphNode target) => 
                GetOrCreateProxy<GraphNodeAction>(target);

        public static GridContainerAction GetProxy(this GridContainer target) => 
                GetOrCreateProxy<GridContainerAction>(target);

        public static GridMapAction GetProxy(this GridMap target) => 
                GetOrCreateProxy<GridMapAction>(target);

        public static GrooveJoint2DAction GetProxy(this GrooveJoint2D target) => 
                GetOrCreateProxy<GrooveJoint2DAction>(target);

        public static HBoxContainerAction GetProxy(this HBoxContainer target) => 
                GetOrCreateProxy<HBoxContainerAction>(target);

        public static HingeJointAction GetProxy(this HingeJoint target) => 
                GetOrCreateProxy<HingeJointAction>(target);

        public static HScrollBarAction GetProxy(this HScrollBar target) => 
                GetOrCreateProxy<HScrollBarAction>(target);

        public static HSeparatorAction GetProxy(this HSeparator target) => 
                GetOrCreateProxy<HSeparatorAction>(target);

        public static HSliderAction GetProxy(this HSlider target) => 
                GetOrCreateProxy<HSliderAction>(target);

        public static HSplitContainerAction GetProxy(this HSplitContainer target) => 
                GetOrCreateProxy<HSplitContainerAction>(target);

        public static HTTPRequestAction GetProxy(this HTTPRequest target) => 
                GetOrCreateProxy<HTTPRequestAction>(target);

        public static ImmediateGeometryAction GetProxy(this ImmediateGeometry target) => 
                GetOrCreateProxy<ImmediateGeometryAction>(target);

        public static InterpolatedCameraAction GetProxy(this InterpolatedCamera target) => 
                GetOrCreateProxy<InterpolatedCameraAction>(target);

        public static ItemListAction GetProxy(this ItemList target) => 
                GetOrCreateProxy<ItemListAction>(target);

        public static KinematicBodyAction GetProxy(this KinematicBody target) => 
                GetOrCreateProxy<KinematicBodyAction>(target);

        public static KinematicBody2DAction GetProxy(this KinematicBody2D target) => 
                GetOrCreateProxy<KinematicBody2DAction>(target);

        public static LabelAction GetProxy(this Label target) => 
                GetOrCreateProxy<LabelAction>(target);

        public static Light2DAction GetProxy(this Light2D target) => 
                GetOrCreateProxy<Light2DAction>(target);

        public static LightOccluder2DAction GetProxy(this LightOccluder2D target) => 
                GetOrCreateProxy<LightOccluder2DAction>(target);

        public static Line2DAction GetProxy(this Line2D target) => 
                GetOrCreateProxy<Line2DAction>(target);

        public static LineEditAction GetProxy(this LineEdit target) => 
                GetOrCreateProxy<LineEditAction>(target);

        public static LinkButtonAction GetProxy(this LinkButton target) => 
                GetOrCreateProxy<LinkButtonAction>(target);

        public static ListenerAction GetProxy(this Listener target) => 
                GetOrCreateProxy<ListenerAction>(target);

        public static Listener2DAction GetProxy(this Listener2D target) => 
                GetOrCreateProxy<Listener2DAction>(target);

        public static MarginContainerAction GetProxy(this MarginContainer target) => 
                GetOrCreateProxy<MarginContainerAction>(target);

        public static MenuButtonAction GetProxy(this MenuButton target) => 
                GetOrCreateProxy<MenuButtonAction>(target);

        public static MeshInstanceAction GetProxy(this MeshInstance target) => 
                GetOrCreateProxy<MeshInstanceAction>(target);

        public static MeshInstance2DAction GetProxy(this MeshInstance2D target) => 
                GetOrCreateProxy<MeshInstance2DAction>(target);

        public static MultiMeshInstanceAction GetProxy(this MultiMeshInstance target) => 
                GetOrCreateProxy<MultiMeshInstanceAction>(target);

        public static MultiMeshInstance2DAction GetProxy(this MultiMeshInstance2D target) => 
                GetOrCreateProxy<MultiMeshInstance2DAction>(target);

        public static NavigationAction GetProxy(this Navigation target) => 
                GetOrCreateProxy<NavigationAction>(target);

        public static Navigation2DAction GetProxy(this Navigation2D target) => 
                GetOrCreateProxy<Navigation2DAction>(target);

        public static NavigationMeshInstanceAction GetProxy(this NavigationMeshInstance target) => 
                GetOrCreateProxy<NavigationMeshInstanceAction>(target);

        public static NavigationPolygonInstanceAction GetProxy(this NavigationPolygonInstance target) => 
                GetOrCreateProxy<NavigationPolygonInstanceAction>(target);

        public static NinePatchRectAction GetProxy(this NinePatchRect target) => 
                GetOrCreateProxy<NinePatchRectAction>(target);

        public static Node2DAction GetProxy(this Node2D target) => 
                GetOrCreateProxy<Node2DAction>(target);

        public static OccluderAction GetProxy(this Occluder target) => 
                GetOrCreateProxy<OccluderAction>(target);

        public static OmniLightAction GetProxy(this OmniLight target) => 
                GetOrCreateProxy<OmniLightAction>(target);

        public static OptionButtonAction GetProxy(this OptionButton target) => 
                GetOrCreateProxy<OptionButtonAction>(target);

        public static PanelAction GetProxy(this Panel target) => 
                GetOrCreateProxy<PanelAction>(target);

        public static PanelContainerAction GetProxy(this PanelContainer target) => 
                GetOrCreateProxy<PanelContainerAction>(target);

        public static ParallaxBackgroundAction GetProxy(this ParallaxBackground target) => 
                GetOrCreateProxy<ParallaxBackgroundAction>(target);

        public static ParallaxLayerAction GetProxy(this ParallaxLayer target) => 
                GetOrCreateProxy<ParallaxLayerAction>(target);

        public static ParticlesAction GetProxy(this Particles target) => 
                GetOrCreateProxy<ParticlesAction>(target);

        public static Particles2DAction GetProxy(this Particles2D target) => 
                GetOrCreateProxy<Particles2DAction>(target);

        public static PathAction GetProxy(this Path target) => 
                GetOrCreateProxy<PathAction>(target);

        public static Path2DAction GetProxy(this Path2D target) => 
                GetOrCreateProxy<Path2DAction>(target);

        public static PathFollowAction GetProxy(this PathFollow target) => 
                GetOrCreateProxy<PathFollowAction>(target);

        public static PathFollow2DAction GetProxy(this PathFollow2D target) => 
                GetOrCreateProxy<PathFollow2DAction>(target);

        public static PhysicalBoneAction GetProxy(this PhysicalBone target) => 
                GetOrCreateProxy<PhysicalBoneAction>(target);

        public static PinJointAction GetProxy(this PinJoint target) => 
                GetOrCreateProxy<PinJointAction>(target);

        public static PinJoint2DAction GetProxy(this PinJoint2D target) => 
                GetOrCreateProxy<PinJoint2DAction>(target);

        public static Polygon2DAction GetProxy(this Polygon2D target) => 
                GetOrCreateProxy<Polygon2DAction>(target);

        public static PopupAction GetProxy(this Popup target) => 
                GetOrCreateProxy<PopupAction>(target);

        public static PopupDialogAction GetProxy(this PopupDialog target) => 
                GetOrCreateProxy<PopupDialogAction>(target);

        public static PopupMenuAction GetProxy(this PopupMenu target) => 
                GetOrCreateProxy<PopupMenuAction>(target);

        public static PopupPanelAction GetProxy(this PopupPanel target) => 
                GetOrCreateProxy<PopupPanelAction>(target);

        public static PortalAction GetProxy(this Portal target) => 
                GetOrCreateProxy<PortalAction>(target);

        public static Position2DAction GetProxy(this Position2D target) => 
                GetOrCreateProxy<Position2DAction>(target);

        public static Position3DAction GetProxy(this Position3D target) => 
                GetOrCreateProxy<Position3DAction>(target);

        public static ProgressBarAction GetProxy(this ProgressBar target) => 
                GetOrCreateProxy<ProgressBarAction>(target);

        public static ProximityGroupAction GetProxy(this ProximityGroup target) => 
                GetOrCreateProxy<ProximityGroupAction>(target);

        public static RayCastAction GetProxy(this RayCast target) => 
                GetOrCreateProxy<RayCastAction>(target);

        public static RayCast2DAction GetProxy(this RayCast2D target) => 
                GetOrCreateProxy<RayCast2DAction>(target);

        public static ReferenceRectAction GetProxy(this ReferenceRect target) => 
                GetOrCreateProxy<ReferenceRectAction>(target);

        public static ReflectionProbeAction GetProxy(this ReflectionProbe target) => 
                GetOrCreateProxy<ReflectionProbeAction>(target);

        public static RemoteTransformAction GetProxy(this RemoteTransform target) => 
                GetOrCreateProxy<RemoteTransformAction>(target);

        public static RemoteTransform2DAction GetProxy(this RemoteTransform2D target) => 
                GetOrCreateProxy<RemoteTransform2DAction>(target);

        public static ResourcePreloaderAction GetProxy(this ResourcePreloader target) => 
                GetOrCreateProxy<ResourcePreloaderAction>(target);

        public static RichTextLabelAction GetProxy(this RichTextLabel target) => 
                GetOrCreateProxy<RichTextLabelAction>(target);

        public static RigidBodyAction GetProxy(this RigidBody target) => 
                GetOrCreateProxy<RigidBodyAction>(target);

        public static RigidBody2DAction GetProxy(this RigidBody2D target) => 
                GetOrCreateProxy<RigidBody2DAction>(target);

        public static RoomAction GetProxy(this Room target) => 
                GetOrCreateProxy<RoomAction>(target);

        public static RoomGroupAction GetProxy(this RoomGroup target) => 
                GetOrCreateProxy<RoomGroupAction>(target);

        public static RoomManagerAction GetProxy(this RoomManager target) => 
                GetOrCreateProxy<RoomManagerAction>(target);

        public static ScrollContainerAction GetProxy(this ScrollContainer target) => 
                GetOrCreateProxy<ScrollContainerAction>(target);

        public static SkeletonAction GetProxy(this Skeleton target) => 
                GetOrCreateProxy<SkeletonAction>(target);

        public static Skeleton2DAction GetProxy(this Skeleton2D target) => 
                GetOrCreateProxy<Skeleton2DAction>(target);

        public static SkeletonIKAction GetProxy(this SkeletonIK target) => 
                GetOrCreateProxy<SkeletonIKAction>(target);

        public static SliderJointAction GetProxy(this SliderJoint target) => 
                GetOrCreateProxy<SliderJointAction>(target);

        public static SoftBodyAction GetProxy(this SoftBody target) => 
                GetOrCreateProxy<SoftBodyAction>(target);

        public static SpatialAction GetProxy(this Spatial target) => 
                GetOrCreateProxy<SpatialAction>(target);

        public static SpinBoxAction GetProxy(this SpinBox target) => 
                GetOrCreateProxy<SpinBoxAction>(target);

        public static SpotLightAction GetProxy(this SpotLight target) => 
                GetOrCreateProxy<SpotLightAction>(target);

        public static SpringArmAction GetProxy(this SpringArm target) => 
                GetOrCreateProxy<SpringArmAction>(target);

        public static SpriteAction GetProxy(this Sprite target) => 
                GetOrCreateProxy<SpriteAction>(target);

        public static Sprite3DAction GetProxy(this Sprite3D target) => 
                GetOrCreateProxy<Sprite3DAction>(target);

        public static StaticBodyAction GetProxy(this StaticBody target) => 
                GetOrCreateProxy<StaticBodyAction>(target);

        public static StaticBody2DAction GetProxy(this StaticBody2D target) => 
                GetOrCreateProxy<StaticBody2DAction>(target);

        public static TabContainerAction GetProxy(this TabContainer target) => 
                GetOrCreateProxy<TabContainerAction>(target);

        public static TabsAction GetProxy(this Tabs target) => 
                GetOrCreateProxy<TabsAction>(target);

        public static TextEditAction GetProxy(this TextEdit target) => 
                GetOrCreateProxy<TextEditAction>(target);

        public static TextureButtonAction GetProxy(this TextureButton target) => 
                GetOrCreateProxy<TextureButtonAction>(target);

        public static TextureProgressAction GetProxy(this TextureProgress target) => 
                GetOrCreateProxy<TextureProgressAction>(target);

        public static TextureRectAction GetProxy(this TextureRect target) => 
                GetOrCreateProxy<TextureRectAction>(target);

        public static TileMapAction GetProxy(this TileMap target) => 
                GetOrCreateProxy<TileMapAction>(target);

        public static TimerAction GetProxy(this Timer target) => 
                GetOrCreateProxy<TimerAction>(target);

        public static ToolButtonAction GetProxy(this ToolButton target) => 
                GetOrCreateProxy<ToolButtonAction>(target);

        public static TouchScreenButtonAction GetProxy(this TouchScreenButton target) => 
                GetOrCreateProxy<TouchScreenButtonAction>(target);

        public static TreeAction GetProxy(this Tree target) => 
                GetOrCreateProxy<TreeAction>(target);

        public static TweenAction GetProxy(this Tween target) => 
                GetOrCreateProxy<TweenAction>(target);

        public static VBoxContainerAction GetProxy(this VBoxContainer target) => 
                GetOrCreateProxy<VBoxContainerAction>(target);

        public static VehicleBodyAction GetProxy(this VehicleBody target) => 
                GetOrCreateProxy<VehicleBodyAction>(target);

        public static VehicleWheelAction GetProxy(this VehicleWheel target) => 
                GetOrCreateProxy<VehicleWheelAction>(target);

        public static VideoPlayerAction GetProxy(this VideoPlayer target) => 
                GetOrCreateProxy<VideoPlayerAction>(target);

        public static ViewportAction GetProxy(this Viewport target) => 
                GetOrCreateProxy<ViewportAction>(target);

        public static ViewportContainerAction GetProxy(this ViewportContainer target) => 
                GetOrCreateProxy<ViewportContainerAction>(target);

        public static VisibilityEnablerAction GetProxy(this VisibilityEnabler target) => 
                GetOrCreateProxy<VisibilityEnablerAction>(target);

        public static VisibilityEnabler2DAction GetProxy(this VisibilityEnabler2D target) => 
                GetOrCreateProxy<VisibilityEnabler2DAction>(target);

        public static VisibilityNotifierAction GetProxy(this VisibilityNotifier target) => 
                GetOrCreateProxy<VisibilityNotifierAction>(target);

        public static VisibilityNotifier2DAction GetProxy(this VisibilityNotifier2D target) => 
                GetOrCreateProxy<VisibilityNotifier2DAction>(target);

        public static VScrollBarAction GetProxy(this VScrollBar target) => 
                GetOrCreateProxy<VScrollBarAction>(target);

        public static VSeparatorAction GetProxy(this VSeparator target) => 
                GetOrCreateProxy<VSeparatorAction>(target);

        public static VSliderAction GetProxy(this VSlider target) => 
                GetOrCreateProxy<VSliderAction>(target);

        public static VSplitContainerAction GetProxy(this VSplitContainer target) => 
                GetOrCreateProxy<VSplitContainerAction>(target);

        public static WindowDialogAction GetProxy(this WindowDialog target) => 
                GetOrCreateProxy<WindowDialogAction>(target);

        public static WorldEnvironmentAction GetProxy(this WorldEnvironment target) => 
                GetOrCreateProxy<WorldEnvironmentAction>(target);

        public static YSortAction GetProxy(this YSort target) => 
                GetOrCreateProxy<YSortAction>(target);
    }
}