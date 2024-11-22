using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Godot;
using File = System.IO.File;
using Path = System.IO.Path;

namespace Generator; 

public static class GenerateNotificationsMetadata {
    private static string CreateConstantsLongFileName() =>
        "../Betauer.SourceGenerators.Tests/test/Notifications.ConstantValues.cs";

    private static string CreateNotificationNamesFileName() =>
        "../Betauer.SourceGenerators/src/Notifications.Names.cs";

    public static void Write(List<GodotClass> classes) {
        var bodyConstantsLong = GenerateNotificationConstantsValues(classes);
        Console.WriteLine($"Generated {Path.GetFullPath(CreateConstantsLongFileName())}");
        File.WriteAllText(CreateConstantsLongFileName(), bodyConstantsLong);

        var bodyStringNames = GenerateNotificationStringNamesClass(classes);
        Console.WriteLine($"Generated {Path.GetFullPath(CreateNotificationNamesFileName())}");
        File.WriteAllText(CreateNotificationNamesFileName(), bodyStringNames);
    }

    private static string GenerateNotificationConstantsValues(List<GodotClass> classes) {
        var godotClasses = classes.Where(c => c is { IsValid: true, IsStatic: false, IsAbstract: false, IsNode: true }).ToArray();
        var content = new StringBuilder($@"using Godot;
using Betauer.Core.Nodes.Events;
using Betauer.TestRunner;

namespace Betauer.SourceGenerators.Tests; 

/**
 * Godot version: {Engine.GetVersionInfo()["string"].ToString()}
 * Date: {DateTime.Now:yyyy-MM-dd HH:mm:ss}
 */

[TestFixture]
public class NotificationTest {{

    [Test]
    public void NotificationTests() {{
        {string.Join("\n        ", godotClasses.Where(c => c.Notifications.Length > 0).Select(c => $"var instance{c.ClassName} = new {c.ClassName}NotificationTest();"))}
    }}
}}
");
        foreach (var godotClass in godotClasses.Where(c => c.Notifications.Length > 0)) {
            var parent = godotClass.ClassName;
            if (parent == "CanvasItem") {
                parent = "Node2D /* CanvasItem can not be inherit from */";
            }
            content.AppendLine($@"
[Notifications(Process = false, PhysicsProcess = false)]
public partial class {godotClass.ClassName}NotificationTest : {parent} {{
    public override partial void _Notification(int what);
    public override void _Ready() {{
        {string.Join("\n        ", godotClass.Notifications.Select(n => $"On{n.Replace("Notification", "")} += () => {{ }};"))}
    }}
}}");
        }
        return content.ToString();
    }

    private static string GenerateNotificationStringNamesClass(List<GodotClass> classes) {
        return $@"using System;
using System.Collections.Generic;

namespace Betauer.SourceGenerators; 

public static class Notifications {{
    public static Dictionary<string, string[]> Names = new() {{
        {string.Join("\n        ", classes.Where(c => c.Notifications.Length > 0).Select(GenerateNotificationNames))}
    }};
}}
";
    }

    private static string GenerateNotificationLongValues(GodotClass clazz) {
        return $@"{{ ""Godot.{clazz.ClassName}"", new long[] {{
                {string.Join(",\n                ", clazz.Notifications.Select(n => clazz.ClassName + "." + n))} }}
        }},";
    }

    private static string GenerateNotificationNames(GodotClass clazz) {
        return $@"{{ ""Godot.{clazz.ClassName}"", new string[] {{
                {string.Join(",\n                ", clazz.Notifications.Select(n => $@"""{n}"""))} }}
        }},";
    }
}