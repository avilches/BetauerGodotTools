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
            ProjectSettings.HasSetting(property) ? $"- {property.PadRight(maxLength)} = {ProjectSettings.GetSetting(property).ToString()}" : $"! {property}"
        ).ToArray();
    }

    public static string[] GetOsInfo() => new[] {
        $"--verbose         : {OS.IsStdOutVerbose()}",
        $"Debug/editor      : {OS.IsDebugBuild()}",
        $"executable path   : {OS.GetExecutablePath()}",
        $"cmd args          : {string.Join(" ", OS.GetCmdlineArgs())}",
        $"cmd user args     : {string.Join(" ", OS.GetCmdlineUserArgs())}",
        $"ThreadCallerId    : {OS.GetThreadCallerId()}",
        $"MainThreadId      : {OS.GetMainThreadId()}",
        $"Process id        : {OS.GetProcessId().ToString()}",
        $"Video name        : {string.Join(" ", OS.GetVideoAdapterDriverInfo())}",
        $"Processor name    : {OS.GetProcessorName()}",
        $"Processors        : {OS.GetProcessorCount().ToString()}",
        $"Device unique id  : {OS.GetUniqueId()}",
        $"Device model name : {OS.GetModelName()}",
        $"Locale            : {OS.GetLocale()}/{OS.GetLocaleLanguage()}",
        $"Enabled features  : {string.Join(", ", FeatureFlags.GetEnabledList())}",
        $"Disabled features : {string.Join(", ", FeatureFlags.GetDisabledList())}",
        $"Permissions       : {string.Join(", ", OS.GetGrantedPermissions())}",
        $"OS Name           : {OS.GetName()}",
        $"OS Distribution   : {OS.GetDistributionName()}",
        $"OS Version        : {OS.GetVersion()}",
        $"Is sandboxed?     : {OS.IsSandboxed()}",
        $"System Camera     : {OS.GetSystemDir(OS.SystemDir.Dcim)}",
        $"System Desktop    : {OS.GetSystemDir(OS.SystemDir.Desktop)}",
        $"System Documents  : {OS.GetSystemDir(OS.SystemDir.Documents)}",
        $"System Music      : {OS.GetSystemDir(OS.SystemDir.Music)}",
        $"System Downloads  : {OS.GetSystemDir(OS.SystemDir.Downloads)}",
        $"System Movies     : {OS.GetSystemDir(OS.SystemDir.Movies)}",
        $"System Pictures   : {OS.GetSystemDir(OS.SystemDir.Pictures)}",
        $"System Ringtones  : {OS.GetSystemDir(OS.SystemDir.Ringtones)}",
        $"Data dir          : {OS.GetDataDir()}",
        $"User data dir     : {OS.GetUserDataDir()}",
        $"Config dir        : {OS.GetConfigDir()}",
        $"Cache dir         : {OS.GetCacheDir()}",
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

        // updated to Godot 4.4
        // from https://docs.godotengine.org/en/stable/tutorials/export/feature_tags.html

        public static readonly Dictionary<string, string> Description = new() {
            { "android", "Running on Android (but not within a Web browser)" },
            { "bsd", "Running on *BSD (but not within a Web browser)" },
            { "linux", "Running on Linux (but not within a Web browser)" },
            { "macos", "Running on macOS (but not within a Web browser)" },
            { "ios", "Running on iOS (but not within a Web browser)" },
            { "windows", "Running on Windows" },
            { "linuxbsd", "Running on Linux or *BSD" },
            { "debug", "Running on a debug build (including the editor)" },
            { "release", "Running on a release build" },
            { "editor", "Running on an editor build" },
            { "template", "Running on a non-editor (export template) build" },

            { "double", "Running on a double-precision build" },
            { "single", "Running on a single-precision build" },
            { "64", "Running on a 64-bit build (any architecture)" },
            { "32", "Running on a 32-bit build (any architecture)" },
            { "x86_64", "Running on a 64-bit x86 build" },
            { "x86_32", "Running on a 32-bit x86 build" },
            { "x86", "Running on an x86 build (any bitness)" },
            { "arm64", "Running on a 64-bit ARM build" },
            { "arm32", "Running on a 32-bit ARM build" },
            { "arm", "Running on an ARM build (any bitness)" },
            { "rv64", "Running on a 64-bit RISC-V build" },
            { "riscv", "Running on a RISC-V build (any bitness)" },
            { "ppc64", "Running on a 64-bit PowerPC build" },
            { "ppc32", "Running on a 32-bit PowerPC build" },
            { "ppc", "Running on a PowerPC build (any bitness)" },
            { "wasm64", "Running on a 64-bit WebAssembly build (not yet possible)" },
            { "wasm32", "Running on a 32-bit WebAssembly build" },
            { "wasm", "Running on a WebAssembly build (any bitness)" },

            { "mobile", "Host OS is a mobile platform" },
            { "pc", "Host OS is a PC platform (desktop/laptop)" },
            { "web", "Host OS is a Web browser" },
            { "web_android", "Host OS is a Web browser running on Android" },
            { "web_ios", "Host OS is a Web browser running on iOS" },
            { "web_linuxbsd", "Host OS is a Web browser running on Linux or *BSD" },
            { "web_macos", "Host OS is a Web browser running on macOS" },
            { "web_windows", "Host OS is a Web browser running on Windows" },

            { "etc", "Textures using ETC1 compression are supported" },
            { "etc2", "Textures using ETC2 compression are supported" },
            { "s3tc", "Textures using S3TC (DXT/BC) compression are supported" },
            { "movie", "Movie Maker mode is active" },
        };

        public static bool IsAndroid() => OS.HasFeature("android");
        public static bool IsBsd() => OS.HasFeature("bsd");
        public static bool IsLinux() => OS.HasFeature("linux");
        public static bool IsMacos() => OS.HasFeature("macos");
        public static bool IsIos() => OS.HasFeature("ios");
        public static bool IsWindows() => OS.HasFeature("windows");
        public static bool IsLinuxbsd() => OS.HasFeature("linuxbsd");
        public static bool IsDebug() => OS.HasFeature("debug");
        public static bool IsRelease() => OS.HasFeature("release");
        public static bool IsEditor() => OS.HasFeature("editor");
        public static bool IsTemplate() => OS.HasFeature("template");

        public static IEnumerable<string> GetEnabledList() {
            return Description.Keys.Where(OS.HasFeature);
        }

        public static IEnumerable<string> GetDisabledList() {
            return Description.Keys.Where(k => !OS.HasFeature(k));
        }

        public static Dictionary<string, bool> GetFeaturesTags() {
            return Description
                .ToDictionary(val => val.Key, val => OS.HasFeature(val.Key));
        }
    }
}