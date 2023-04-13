using System;
using System.Collections.Generic;
using System.Linq;
using Betauer.Core;
using Godot;

namespace Betauer.Application; 

public static partial class Project {

    public static string[] GetSettings(params string[] settingNames) {
        var maxLength = settingNames.Max(s => s.Length);
        return settingNames.Select(property =>
            ProjectSettings.HasSetting(property) ? 
                $"- {property.PadRight(maxLength)} = {ProjectSettings.GetSetting(property).ToString()}":
                $"! {property}"
        ).ToArray();
    }

    public static string[] GetOSInfo() => new[] {
        $"executable path : {OS.GetExecutablePath()}",
        $"cmd args        : {string.Join(" ", OS.GetCmdlineArgs())}",
        $"cmd user args   : {string.Join(" ", OS.GetCmdlineUserArgs())}",
        $"ThreadCallerId  : {OS.GetThreadCallerId()}",
        $"MainThreadId    : {OS.GetMainThreadId()}",
        $"Process id      : {OS.GetProcessId().ToString()}",
        $"Video name      : {string.Join(" ", OS.GetVideoAdapterDriverInfo())}",
        $"Processor name  : {OS.GetProcessorName()}",
        $"Processors      : {OS.GetProcessorCount().ToString()}",
        $"Unique id       : {OS.GetUniqueId()}",
        $"Locale          : {OS.GetLocale()}/{OS.GetLocaleLanguage()}",
        $"Features        : {string.Join(", ", FeatureFlags.GetActiveList())}",
        $"Permissions     : {string.Join(", ", OS.GetGrantedPermissions())}",
        $"Name host       : {OS.GetName()}",
        $"Distribution    : {OS.GetDistributionName()}",
        $"Version         : {OS.GetVersion()}",
        $"Model name      : {OS.GetModelName()}",
        $"Data dir        : {OS.GetDataDir()}",
        $"User data dir   : {OS.GetUserDataDir()}",
        $"Config dir      : {OS.GetConfigDir()}",
        $"Cache dir       : {OS.GetCacheDir()}",
        $"--verbose       : {OS.IsStdOutVerbose()}",
        $"Debug/editor    : {OS.IsDebugBuild()}",
        $"Standalone      : {FeatureFlags.IsExported()}",
    };

    public static TimeSpan Uptime => TimeSpan.FromMilliseconds(Time.GetTicksMsec());

    public static void SetSetting<T>(string key, T value) {
        var variantValue = Variant.From(value);
        ProjectSettings.SetSetting(key, variantValue);
    }
    public static T GetSetting<[MustBeVariant] T>(string key, T @default = default) {
        if (!ProjectSettings.HasSetting(key)) return @default;
        var variantValue = ProjectSettings.GetSetting(key);
        return variantValue.As<T>();
    }
        
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

        public static Dictionary<string, string> GetActiveMap() {
            return Description
                .Where(val => OS.HasFeature(val.Key))
                .ToDictionary(val => val.Key, val => val.Value);
        }

    }
}