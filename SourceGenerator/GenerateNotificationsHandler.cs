using System;
using System.Collections.Generic;
using System.Linq;
using Godot;
using File = System.IO.File;
using Path = System.IO.Path;

namespace Generator {
    public class GenerateNotificationHandler {
        private static string GodotActionsExtensionsFile(string name) =>
            "../Betauer.GameTools/Application/" + name + "NotificationsHandler.cs";

        public static void WriteNotifications(GodotClass clazz) {
            var body = GenerateBodyClass(clazz);
            Console.WriteLine($"Generated {Path.GetFullPath(GodotActionsExtensionsFile(clazz.ClassName))}");
            File.WriteAllText(GodotActionsExtensionsFile(clazz.ClassName), body);
        }

        private static string GenerateBodyClass(GodotClass clazz) {
            return $@"using System;
using Godot;

namespace Betauer.Application {{
    public class {clazz.ClassName}NotificationsHandler {{

{string.Join("\n", GenerateEvents(clazz))}

        public void Execute(int what) {{
            switch (what) {{
{string.Join("\n", GenerateSwitchCases(clazz))}
            }}
        }}  

    }}
}}";
        }

        private static IEnumerable<string> GenerateEvents(GodotClass clazz) {
            return ClassDB.ClassGetIntegerConstantList(clazz.ClassName)
                .Where(n => n.StartsWith("NOTIFICATION"))
                .Select(n => 
$"        public event Action {NotificationEventName(n)};");
        }

        private static string NotificationEventName(string s) {
            return "On"+s.ToLower().CamelCase().Remove(0, "Notification".Length);
        }

        private static string NotificationConstant(string s) {
            return s.ToLower().CamelCase();
        }

        private static IEnumerable<string> GenerateSwitchCases(GodotClass clazz) {
            return ClassDB.ClassGetIntegerConstantList(clazz.ClassName)
                .Where(n => n.StartsWith("NOTIFICATION"))
                .Select(n => 
$@"                case {clazz.ClassName}.{NotificationConstant(n)}:
                    {NotificationEventName(n)}?.Invoke();
                    break;");
        }
        
        
    }
}