using System.Collections.Generic;
using System.Linq;
using Betauer.Core.Nodes.Property;
using Godot;

namespace Betauer.Core.Restorer; 

public static class RestoreExtensions {
    // CanvasItem : Node
    public static readonly StringName[] CanvasItemProperties = {
        CanvasItem.PropertyName.Visible,
        CanvasItem.PropertyName.Modulate,
        CanvasItem.PropertyName.SelfModulate,
        CanvasItem.PropertyName.ShowBehindParent,
        CanvasItem.PropertyName.TopLevel,
        CanvasItem.PropertyName.ClipChildren,
        CanvasItem.PropertyName.LightMask,
        CanvasItem.PropertyName.VisibilityLayer,
        CanvasItem.PropertyName.ZIndex,
        CanvasItem.PropertyName.ZAsRelative,
        CanvasItem.PropertyName.YSortEnabled,
        CanvasItem.PropertyName.TextureFilter,
        CanvasItem.PropertyName.TextureRepeat,
        CanvasItem.PropertyName.Material,
        CanvasItem.PropertyName.UseParentMaterial,
    };

    // Node2D : CanvasItem : Node
    public static readonly StringName[] Node2DProperties = new[] {
        Node2D.PropertyName.Transform,
    }.Concat(CanvasItemProperties).ToArray();

    // Sprite2D : Node2D : CanvasItem : Node
    public static readonly StringName[] SpriteProperties = new[] {
        Sprite2D.PropertyName.Texture,
        Sprite2D.PropertyName.Centered,
        Sprite2D.PropertyName.Offset,
        Sprite2D.PropertyName.FlipH,
        Sprite2D.PropertyName.FlipV,
        Sprite2D.PropertyName.Hframes,
        Sprite2D.PropertyName.Vframes,
        Sprite2D.PropertyName.Frame,
        Sprite2D.PropertyName.FrameCoords,
        Sprite2D.PropertyName.RegionEnabled,
        Sprite2D.PropertyName.RegionRect,
        Sprite2D.PropertyName.RegionFilterClipEnabled,
    }.Concat(Node2DProperties).ToArray();
    
    // Control : CanvasItem : Node
    public static readonly StringName[] ControlProperties = new [] {
        Control.PropertyName.Position,
        Control.PropertyName.Scale,
        Control.PropertyName.Rotation,
        Control.PropertyName.PivotOffset,
        Control.PropertyName.FocusMode,
        Control.PropertyName.TooltipText,
    }.Concat(CanvasItemProperties).ToArray();

    // BaseButton : Control : CanvasItem : Node
    public static readonly StringName[] BaseButtonProperties = new [] {
        BaseButton.PropertyName.Disabled
    }.Concat(ControlProperties).ToArray();

    // CollisionObject2D : Node2D : CanvasItem : Node
    private static readonly StringName[] CollisionObject2DProperties = new [] {
        CollisionObject2D.PropertyName.DisableMode,
        CollisionObject2D.PropertyName.CollisionLayer,
        CollisionObject2D.PropertyName.CollisionMask,
        CollisionObject2D.PropertyName.CollisionPriority,
        CollisionObject2D.PropertyName.InputPickable,
    }.Concat(Node2DProperties).ToArray();

    // CharacterBody2D : CollisionObject2D : Node2D : CanvasItem : Node
    private static readonly StringName[] Character2DProperties = new [] {
        CharacterBody2D.PropertyName.MotionMode,
        CharacterBody2D.PropertyName.UpDirection,
        CharacterBody2D.PropertyName.Velocity,
        CharacterBody2D.PropertyName.SlideOnCeiling,
        CharacterBody2D.PropertyName.MaxSlides,
        CharacterBody2D.PropertyName.WallMinSlideAngle,
        CharacterBody2D.PropertyName.FloorStopOnSlope,
        CharacterBody2D.PropertyName.FloorConstantSpeed,
        CharacterBody2D.PropertyName.FloorBlockOnWall,
        CharacterBody2D.PropertyName.FloorMaxAngle,
        CharacterBody2D.PropertyName.FloorSnapLength,
        CharacterBody2D.PropertyName.PlatformOnLeave,
        CharacterBody2D.PropertyName.PlatformFloorLayers,
        CharacterBody2D.PropertyName.PlatformWallLayers,
        CharacterBody2D.PropertyName.SafeMargin,
    }.Concat(CollisionObject2DProperties).ToArray();
    
    // RigidBody2D : CollisionObject2D : Node2D : CanvasItem : Node
    private static readonly StringName[] RigidBody2DProperties = new[] {
        RigidBody2D.PropertyName.Mass,
        RigidBody2D.PropertyName.Inertia,
        RigidBody2D.PropertyName.CenterOfMassMode,
        RigidBody2D.PropertyName.CenterOfMass,
        RigidBody2D.PropertyName.PhysicsMaterialOverride,
        RigidBody2D.PropertyName.GravityScale,
        RigidBody2D.PropertyName.CustomIntegrator,
        RigidBody2D.PropertyName.ContinuousCd,
        RigidBody2D.PropertyName.MaxContactsReported,
        RigidBody2D.PropertyName.ContactMonitor,
        RigidBody2D.PropertyName.Sleeping,
        RigidBody2D.PropertyName.CanSleep,
        RigidBody2D.PropertyName.LockRotation,
        RigidBody2D.PropertyName.Freeze,
        RigidBody2D.PropertyName.FreezeMode,
        RigidBody2D.PropertyName.LinearVelocity,
        RigidBody2D.PropertyName.LinearDampMode,
        RigidBody2D.PropertyName.LinearDamp,
        RigidBody2D.PropertyName.AngularVelocity,
        RigidBody2D.PropertyName.AngularDampMode,
        RigidBody2D.PropertyName.AngularDamp,
        RigidBody2D.PropertyName.ConstantForce,
        RigidBody2D.PropertyName.ConstantTorque,
    }.Concat(CollisionObject2DProperties).ToArray();
    
    // StaticBody2D : CollisionObject2D : Node2D : CanvasItem : Node
    private static readonly StringName[] StaticBody2DProperties = new[] {
        StaticBody2D.PropertyName.PhysicsMaterialOverride,
        StaticBody2D.PropertyName.ConstantLinearVelocity,
        StaticBody2D.PropertyName.ConstantAngularVelocity,
    }.Concat(CollisionObject2DProperties).ToArray();
    
    // Area2D : CollisionObject2D : Node2D : CanvasItem : Node
    private static readonly StringName[] Area2DProperties = new[] {
        Area2D.PropertyName.Monitorable,
        Area2D.PropertyName.Monitoring,
    }.Concat(CollisionObject2DProperties).ToArray();

    // CollisionShape2D : Node2D : CanvasItem : Node
    private static readonly StringName[] CollisionShape2DProperties = new [] {
        CollisionShape2D.PropertyName.Disabled,
        CollisionShape2D.PropertyName.OneWayCollision,
        CollisionShape2D.PropertyName.OneWayCollisionMargin,
    }.Concat(Node2DProperties).ToArray();
    
    public static FocusRestorer CreateFocusOwnerRestorer(this Control control) {
        return new FocusRestorer(control);
    }

    public static ChildFocusRestorer CreateChildFocusedRestorer(this Container container) {
        return new ChildFocusRestorer(container);
    }

    public static Restorer CreateRestorer(this Node node, params string[] properties) {
        return new PropertyNameRestorer(node, properties);
    }

    public static Restorer CreateRestorer(this Node node, params NodePath[] property) {
        return new PropertyNameRestorer(node, property);
    }

    public static Restorer CreateRestorer(this Node node, params IProperty[] property) {
        return new PropertyRestorer(node, property);
    }

    public static MultiRestorer CreateRestorer(this IEnumerable<Node> nodes, params string[] properties) {
        var multiRestorer = new MultiRestorer();
        nodes.ForEach(node => multiRestorer.Add(node.CreateRestorer(properties)));
        return multiRestorer;
    }

    public static MultiRestorer CreateRestorer(this IEnumerable<Node> nodes, params NodePath[] properties) {
        var multiRestorer = new MultiRestorer();
        nodes.ForEach(node => multiRestorer.Add(node.CreateRestorer(properties)));
        return multiRestorer;
    }

    public static MultiRestorer CreateRestorer(this IEnumerable<Node> nodes, params IProperty[] properties) {
        var multiRestorer = new MultiRestorer();
        nodes.ForEach(node => multiRestorer.Add(node.CreateRestorer(properties)));
        return multiRestorer;
    }

    public static MultiRestorer CreateRestorer(this IEnumerable<Node> nodes) {
        var multiRestorer = new MultiRestorer();
        nodes.ForEach(node => multiRestorer.Add(node.CreateRestorer()));
        return multiRestorer;
    }

    public static Restorer CreateRestorer(this Node node) {
        return node switch {
            CharacterBody2D => node.GetChildren().OfType<CollisionShape2D>().CreateRestorer().Add(new PropertyNameRestorer(node, Character2DProperties)), 
            RigidBody2D => node.GetChildren().OfType<CollisionShape2D>().CreateRestorer().Add(new PropertyNameRestorer(node, RigidBody2DProperties)), 
            StaticBody2D => node.GetChildren().OfType<CollisionShape2D>().CreateRestorer().Add(new PropertyNameRestorer(node, StaticBody2DProperties)), 
            Area2D => node.GetChildren().OfType<CollisionShape2D>().CreateRestorer().Add(new PropertyNameRestorer(node, Area2DProperties)), 
            CollisionObject2D => node.GetChildren().OfType<CollisionShape2D>().CreateRestorer().Add(new PropertyNameRestorer(node, CollisionObject2DProperties)),
            CollisionShape2D => new PropertyNameRestorer(node, CollisionShape2DProperties),
            Sprite2D => new PropertyNameRestorer(node, SpriteProperties),
            Node2D => new PropertyNameRestorer(node, Node2DProperties),
            BaseButton => new PropertyNameRestorer(node, BaseButtonProperties),
            Control => new PropertyNameRestorer(node, ControlProperties),
            _ => DummyRestorer.Instance
        };
    }
}