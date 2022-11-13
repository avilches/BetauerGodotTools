using System;
using System.Collections.Generic;
using System.Linq;
using Godot;

namespace Betauer.Application {
    public static partial class Project {
        
        public static void PrintSettings(params string[] settingNames) {
            Array.ForEach(settingNames, property => {
                if (ProjectSettings.HasSetting(property)) {
                    GD.Print($"- {property} = {ProjectSettings.GetSetting(property)}");
                } else {
                    GD.Print($"! {property} !");
                }
            });
        }
        
        public static void PrintOSInfo() {
            GD.Print($"executable path : {OS.GetExecutablePath()}");
            GD.Print($"cmd args        : {string.Join(" ", OS.GetCmdlineArgs())}");
            GD.Print($"cmd user args   : {string.Join(" ", OS.GetCmdlineUserArgs())}");
            GD.Print($"ThreadCallerId  : {OS.GetThreadCallerId()}");
            GD.Print($"MainThreadId    : {OS.GetMainThreadId()}");
            GD.Print($"Process id      : {OS.GetProcessId().ToString()}");
            GD.Print($"Video name      : {string.Join(" ", OS.GetVideoAdapterDriverInfo())}");
            GD.Print($"Processor name  : {OS.GetProcessorName()}");
            GD.Print($"Processors      : {OS.GetProcessorCount().ToString()}");
            GD.Print($"Unique id       : {OS.GetUniqueId()}");
            GD.Print($"Locale          : {OS.GetLocale()}/{OS.GetLocaleLanguage()}");
            GD.Print($"Features        : {string.Join(", ", FeatureFlags.GetActiveList())}");
            GD.Print($"Permissions     : {string.Join(", ", OS.GetGrantedPermissions())}");
            GD.Print($"Name host       : {OS.GetName()}");
            GD.Print($"Distribution    : {OS.GetDistributionName()}");
            GD.Print($"Version         : {OS.GetVersion()}");
            GD.Print($"Model name      : {OS.GetModelName()}");
            GD.Print($"Data dir        : {OS.GetDataDir()}");
            GD.Print($"User data dir   : {OS.GetUserDataDir()}");
            GD.Print($"Config dir      : {OS.GetConfigDir()}");
            GD.Print($"Cache dir       : {OS.GetCacheDir()}");
            GD.Print($"--verbose       : {OS.IsStdoutVerbose()}");
            GD.Print($"Debug/editor    : {OS.IsDebugBuild()}");
            GD.Print($"Standalone      : {FeatureFlags.IsExported()}");
        }
        
        public static TimeSpan Uptime => TimeSpan.FromMilliseconds(Time.GetTicksMsec());

        public static class FeatureFlags {
            public static readonly Dictionary<string, string> Description = new() {
                { "Android", "Running on Android" },
                { "HTML5", "Running on HTML5" },
                { "JavaScript", "JavaScript singleton is available" },
                { "OSX", "Running on macOS" },
                { "iOS", "Running on iOS" },
                { "UWP", "Running on UWP" },
                { "Windows", "Running on Windows" },
                { "X11", "Running on X11 (Linux/BSD desktop)" },
                { "Server", "Running on the headless server platform" },
                { "debug", "Running on a debug build (including the editor)" },
                { "release", "Running on a release build" },
                { "editor", "Running on an editor build" },
                { "standalone", "Running on a non-editor build" },
                { "64", "Running on a 64-bit build (any architecture)" },
                { "32", "Running on a 32-bit build (any architecture)" },
                { "x86_64", "Running on a 64-bit x86 build" },
                { "x86", "Running on a 32-bit x86 build" },
                { "arm64", "Running on a 64-bit ARM build" },
                { "arm", "Running on a 32-bit ARM build" },
                { "mobile", "Host OS is a mobile platform" },
                { "pc", "Host OS is a PC platform (desktop/laptop)" },
                { "web", "Host OS is a Web browser" },
                { "etc", "Textures using ETC1 compression are supported" },
                { "etc2", "Textures using ETC2 compression are supported" },
                { "s3tc", "Textures using S3TC (DXT/BC) compression are supported" },
                { "pvrtc", "Textures using PVRTC compression are supported" },
            };

            public static bool IsHtml5() => OS.HasFeature("HTML5");
            public static bool HasJavaScript() => OS.HasFeature("JavaScript");
            
            public static bool IsIOs() => OS.HasFeature("iOS");
            public static bool IsAndroid() => OS.HasFeature("Android");
            
            public static bool IsMacOs() => OS.HasFeature("OSX");
            public static bool IsWindows() => OS.HasFeature("Windows");
            public static bool IsUwp() => OS.HasFeature("UWP");
            public static bool IsLinux() => OS.HasFeature("X11");
            public static bool IsServer() => OS.HasFeature("Server");
            public static bool IsEditor() => OS.HasFeature("editor");

            public static IEnumerable<string> GetActiveList() {
                return Description.Values.Where(OS.HasFeature);
            }

            public static bool IsExported() => OS.HasFeature("standalone");

            private const string MonoUnhandledExceptionPolicyLogError = "1"; 
            private const string MonoUnhandledExceptionPolicyTerminate = "0"; 

            public static bool IsTerminateOnExceptionEnabled() =>
                ProjectSettings.GetSetting("mono/runtime/unhandled_exception_policy").ToString() == MonoUnhandledExceptionPolicyTerminate;

            public static Dictionary<string, string> GetActiveMap() {
                return Description
                    .Where(val => OS.HasFeature(val.Key))
                    .ToDictionary(val => val.Key, val => val.Value);
            }

        }
    }
}