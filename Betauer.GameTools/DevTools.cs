using System;
using Godot;

namespace Betauer {
    public enum UnhandledExceptionPolicy {
        TerminateApplication = 0,
        LogError = 1
    }

    public static class DevTools {
        public static readonly bool IsDevelopment = OS.HasFeature("editor");

        public static void CheckErrorPolicy(SceneTree tree, UnhandledExceptionPolicy unhandledExceptionPolicyConfig) {
            var unhandledExceptionPolicySetting = GetUnhandledExceptionPolicySetting();
            if (!IsDevelopment) {
                // This is a release
                if (unhandledExceptionPolicySetting == UnhandledExceptionPolicy.TerminateApplication) {
                    // If the app can crash with just an exception, then better to crash as soon as possible
                    throw new Exception(
                        "Please, don't use export a release with the mono/unhandled_exception_policy as Terminate Application");
                }
            }
            if (unhandledExceptionPolicySetting == UnhandledExceptionPolicy.LogError) {
                GD.Print("Current unhandled exception policy: Log Error");
                if (unhandledExceptionPolicyConfig == UnhandledExceptionPolicy.TerminateApplication) {
                    WriteUnhandledExceptionPolicy(UnhandledExceptionPolicy.TerminateApplication);
                    tree.Quit();
                }
            } else if (unhandledExceptionPolicySetting == UnhandledExceptionPolicy.TerminateApplication) {
                GD.Print("Current unhandled exception policy: Terminate Application");
                if (unhandledExceptionPolicyConfig == UnhandledExceptionPolicy.LogError) {
                    WriteUnhandledExceptionPolicy(UnhandledExceptionPolicy.LogError);
                    tree.Quit();
                }
            }
        }

        private static void WriteUnhandledExceptionPolicy(UnhandledExceptionPolicy policy) {
            GD.PushWarning("Writing mono/unhandled_exception_policy=" + policy +
                           " in project.godot configuration. Restart to reload changes. Aborting...");
            ProjectSettings.SetSetting("mono/unhandled_exception_policy", (int)policy);
            ProjectSettings.Save();
        }

        private static UnhandledExceptionPolicy GetUnhandledExceptionPolicySetting() {
            var x = ProjectSettings.GetSetting("mono/unhandled_exception_policy");
            return x is int policy && policy == (int)UnhandledExceptionPolicy.LogError
                ? UnhandledExceptionPolicy.LogError
                : UnhandledExceptionPolicy.TerminateApplication;
        }

    }
}