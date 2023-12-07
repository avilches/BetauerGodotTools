using System;
using System.Collections.Generic;

namespace Betauer.SourceGenerators; 

public static class Notifications {
    public static Dictionary<string, string[]> Names = new() {
        { "Godot.CanvasItem", new string[] {
                "NotificationTransformChanged",
                "NotificationLocalTransformChanged",
                "NotificationDraw",
                "NotificationVisibilityChanged",
                "NotificationEnterCanvas",
                "NotificationExitCanvas",
                "NotificationWorld2DChanged" }
        },
        { "Godot.Container", new string[] {
                "NotificationPreSortChildren",
                "NotificationSortChildren" }
        },
        { "Godot.Control", new string[] {
                "NotificationResized",
                "NotificationMouseEnter",
                "NotificationMouseExit",
                "NotificationMouseEnterSelf",
                "NotificationMouseExitSelf",
                "NotificationFocusEnter",
                "NotificationFocusExit",
                "NotificationThemeChanged",
                "NotificationScrollBegin",
                "NotificationScrollEnd",
                "NotificationLayoutDirectionChanged" }
        },
        { "Godot.Node3D", new string[] {
                "NotificationTransformChanged",
                "NotificationEnterWorld",
                "NotificationExitWorld",
                "NotificationVisibilityChanged",
                "NotificationLocalTransformChanged" }
        },
        { "Godot.Skeleton3D", new string[] {
                "NotificationUpdateSkeleton" }
        },
        { "Godot.Window", new string[] {
                "NotificationVisibilityChanged",
                "NotificationThemeChanged" }
        },
    };
}
