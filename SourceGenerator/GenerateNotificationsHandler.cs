using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Godot;
using File = System.IO.File;
using Path = System.IO.Path;

namespace Generator; 

public class GenerateNotificationHandler {
    private static string CreateFileName() =>
        "../Betauer.GameTools/Application/Notifications/NotificationsHandler.cs";

    public static void Write(params GodotClass[] classes) {
        var body = GenerateBodyClass(classes);
        Console.WriteLine($"Generated {Path.GetFullPath(CreateFileName())}");
        File.WriteAllText(CreateFileName(), body);
    }

    private static string GenerateBodyClass(GodotClass[] clazz) {
        return $@"using System;
using Godot;

namespace Betauer.Application.Notifications; 

public partial class NotificationsHandler : Node {{

{string.Join("\n", GenerateEvents(clazz))}

    public override void _EnterTree() {{
        ProcessMode = ProcessModeEnum.Always;
    }}

    public void AddTo(Viewport viewport) {{
        GetParent()?.RemoveChild(this);
            viewport.AddChild(this);
    }}

    public override void _Notification(int what) {{
        switch ((long)what) {{
{string.Join("\n", GenerateSwitchCases(clazz))}
        }}
    }}  

}}
";
    }

    private static IEnumerable<string> GenerateEvents(GodotClass[] classes) {
        return classes.SelectMany(GenerateEvents);
    }

    private static IEnumerable<string> GenerateEvents(GodotClass clazz) {
        return ClassDB.ClassGetIntegerConstantList(clazz.ClassName)
            .Where(n => FilterNotification(clazz, n))
            .Select(n =>              
                $"    public event Action {NotificationEventName(clazz, n)};");
    }
        
    private static bool FilterNotification(GodotClass clazz, string n) {
        // 0-1 GodotObject lifecycle (ctor and predelete)
        // 0-100 Node notifications (like enter tree camera becomes active)
        // 1### notifications = SO
        // 2### notifications = MainLoop (Node already include them)
        // 9### notifications = Editor
        return n.StartsWith("NOTIFICATION") && GetNotificationValue(clazz, n) is >= 1000 and < 9000;
    }

    private static string NotificationEventName(GodotClass clazz, string s) {
        return $"On{s.ToLower().CamelCase().Replace("Wm", "WM").Remove(0, "Notification".Length)}";
    }

    private static string NotificationConstantName(string s) {
        return s.ToLower().CamelCase().Replace("Wm", "WM");
    }
    private static long GetNotificationValue(GodotClass clazz, string s) {
        var name = NotificationConstantName(s);
        return (long)clazz.Type!
            .GetFields(BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy)
            .First(fi => fi.IsLiteral && !fi.IsInitOnly && fi.Name == name).GetRawConstantValue()!;
    }

    private static IEnumerable<string> GenerateSwitchCases(GodotClass[] classes) {
        return classes.SelectMany(GenerateSwitchCases);
    }

    private static IEnumerable<string> GenerateSwitchCases(GodotClass clazz) {
        return ClassDB.ClassGetIntegerConstantList(clazz.ClassName)
            .Where(n => FilterNotification(clazz, n))
            .Select(n => 
                $@"            case {clazz.ClassName}.{NotificationConstantName(n)}: // {GetNotificationValue(clazz, n)}
                {NotificationEventName(clazz, n)}?.Invoke();
                break;");
    }
        
        
}