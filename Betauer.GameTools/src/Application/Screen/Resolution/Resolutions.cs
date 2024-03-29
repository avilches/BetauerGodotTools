using System;
using System.Collections.Generic;
using System.Linq;
using Godot;

namespace Betauer.Application.Screen.Resolution;
public static class Resolutions {
    private static readonly List<Resolution> _all = new();

    // 4:3 (1.33) https://www.videoproc.com/video-process/4-3-resolutions.htm 
    public static Resolution QVGA = AddToAll(new Resolution("QVGA", 320, 240));
    public static Resolution VGA = AddToAll(new Resolution("VGA", 640, 480));
    public static Resolution SVGA = AddToAll(new Resolution("SVGA", 800, 600));
    public static Resolution XGA = AddToAll(new Resolution("XGA", 1024, 768));
    public static Resolution UXGA = AddToAll(new Resolution("UXGA", 1600, 1200));
    

    public static Resolution FULLHD_DIV6 = AddToAll(new Resolution(320, 180));                  // 1920x1080 / 6
    public static Resolution FULLHD_DIV4 = AddToAll(new Resolution(480, 270));                  // 1920x1080 / 4
    public static Resolution FULLHD_DIV3 = AddToAll(new Resolution(640, 360));                  // 1920x1080 / 3
    public static Resolution FULLHD_DIV2 = AddToAll(new Resolution(960, 540));                  // 1920x1080 / 2
    public static Resolution HDplus      = AddToAll(new Resolution("HD+",     1600, 900)); // 1920x1080 / 1.2
    public static Resolution FULLHD      = AddToAll(new Resolution("Full HD", 1920, 1080));
         

    // 16:9 (1.77) https://en.wikipedia.org/wiki/16:9_aspect_ratio
    public static Resolution WSVGA       = AddToAll(new Resolution("WSVGA",   1024, 576)); // 1920x1080 / 1.875
    public static Resolution HD_DIV2     = AddToAll(new Resolution(640, 360));                  // 1280x720 / 2
    public static Resolution HD          = AddToAll(new Resolution("HD",      1280, 720)); // 1920x1080 / 1.5
    public static Resolution FWXGA       = AddToAll(new Resolution("FWXGA",   1366, 768));
    public static Resolution QHD         = AddToAll(new Resolution("QHD",     2560, 1440));// 1920x1080 * 1.33 aka "2K"
    public static Resolution UHD4K       = AddToAll(new Resolution("4K UHD",  3840, 2160));// 1920x1080 * 2 // aka 4K

    // 16:10 (1.6) https://en.wikipedia.org/wiki/16:10_aspect_ratio
    public static Resolution R1610SMALL1 = AddToAll(new Resolution(640, 400));
    public static Resolution R1610SMALL2 = AddToAll(new Resolution(960, 600));
    public static Resolution WXGA        = AddToAll(new Resolution("WXGA", 1280, 800));
    public static Resolution WXGAplus    = AddToAll(new Resolution("WXGA+", 1440, 900));
    public static Resolution WSXGAplus   = AddToAll(new Resolution("WSXGA+", 1680, 1050));
    public static Resolution WUXGA       = AddToAll(new Resolution("WUXGA", 1920, 1200));
    public static Resolution WQXGA       = AddToAll(new Resolution("WQXGA", 2560, 1600));
    public static Resolution WQUXGA      = AddToAll(new Resolution("WQUXGA", 3840, 2400));

    // 21:9 (2.333) https://en.wikipedia.org/wiki/21:9_aspect_ratio
    // 64:27 (2.370)
    public static Resolution UWDIE237_0 = AddToAll(new Resolution(2560, 1080));
    public static Resolution UWDIE237_1 = AddToAll(new Resolution(5120, 2160));
    public static Resolution UWDIE237_2 = AddToAll(new Resolution(7680, 3240));
    public static Resolution UWDIE237_3 = AddToAll(new Resolution(10240, 4320));

    // 43:18 (2.38)
    public static Resolution UWDI238_0 = AddToAll(new Resolution(3440, 1440));
    public static Resolution UWDI238_1 = AddToAll(new Resolution(5160, 2160));
    public static Resolution UWDI238_2 = AddToAll(new Resolution(6880, 2880));

    //	12:5 (2.4)
    public static Resolution UWDIE24_0 = AddToAll(new Resolution(1920, 800));
    public static Resolution UWDIE24_1 = AddToAll(new Resolution(2880, 1200));
    public static Resolution UWDIE24_2 = AddToAll(new Resolution(3840, 1600));
    public static Resolution UWDIE24_3 = AddToAll(new Resolution(4320, 1800));
    public static Resolution UWDIE24_4 = AddToAll(new Resolution(5760, 2400));
    public static Resolution UWDIE24_5 = AddToAll(new Resolution(7680, 3200));
    public static Resolution UWDIE24_6 = AddToAll(new Resolution(8640, 3600));

    private static Resolution AddToAll(Resolution resolution) {
        _all.Add(resolution);
        _all.Sort(Resolution.ComparisonByHeight);
        return resolution;
    }

    public static List<Resolution> GetAll(params AspectRatio[] aspectRatios) =>
        aspectRatios.Length switch {
            0 => new List<Resolution>(_all),
            1 => _all.FindAll(aspectRatios[0].Matches),
            _ => _all.FindAll(resolution => aspectRatios.Any(ratio => ratio.Matches(resolution)))
        };

    /// <summary>
    /// Using the "from" resolution as base, create all possible resolutions:
    /// - Scaling (x1, x2, x3) from "from" resolution up to "maxSize" (or the screen size)
    /// - In all the aspectRatios, keeping the height and changing the width
    /// </summary>
    /// <param name="from"></param>
    /// <param name="maxSize"></param>
    /// <param name="aspectRatios"></param>
    /// <returns></returns>
    public static List<ScaledResolution> ExpandResolutionByWith(this Resolution from, Resolution baseResolution, IEnumerable<AspectRatio> aspectRatios, Resolution? maxSize = null) {
        if (maxSize == null) maxSize = new Resolution(DisplayServer.ScreenGetSize());
        var maxScale = Resolution.CalculateMaxScale(from.Size, maxSize.Size);
        List<ScaledResolution> resolutions = new List<ScaledResolution>();
        for (var scale = 1; scale <= maxScale; scale++) {
            var scaledResolution = new ScaledResolution(baseResolution.Size, from.Size * scale);
            foreach (var aspectRatio in aspectRatios) {
                if (aspectRatio.Matches(from.AspectRatio)) {
                    // Add the baseResolution x scale
                    resolutions.Add(scaledResolution);
                } else {
                    // TODO: This is only with landscapes (it keeps the height)
                    if (aspectRatio.Ratio > scaledResolution.AspectRatio.Ratio) {
                        // Convert the resolution to a wider aspect ratio, keeping the height and adding more width
                        // So, if base is 1920x1080 = 1,77777 16:9
                        // to 21:9 => 2,3333
                        // x=1080*2,333=2520
                        // 2520x1080 = 2,3333 21:9
                        var newWidth = scaledResolution.Y * aspectRatio.Ratio;
                        if (newWidth <= maxSize.X) {
                            var newResolution = new Vector2I((int)newWidth, scaledResolution.Y);
                            var scaledResolutionUpdated = new ScaledResolution(baseResolution.Size, newResolution);
                            resolutions.Add(scaledResolutionUpdated);
                        }
                    } else {
                        // Convert the resolution to a stretcher aspect ratio, keeping the width and adding more height
                        // So, if base is 1920x1080 = 1,77777 16:9
                        // to 4:3 => 1,333
                        // y=1920/1,333=823
                        // 1920x1440 = 1,3333 3:4
                        var newHeight = scaledResolution.X / aspectRatio.Ratio;
                        if (newHeight <= maxSize.Y) {
                            var newResolution = new Vector2I(scaledResolution.X, (int)newHeight);
                            var scaledResolutionUpdated = new ScaledResolution(baseResolution.Size, newResolution);
                            resolutions.Add(scaledResolutionUpdated);
                        }
                    }
                }
            }
        }
        return resolutions;
    }

    public static IEnumerable<Resolution> Clamp(this IEnumerable<Resolution> resolutions, Vector2I min, Vector2I max) {
        if (min.X > max.X) throw new Exception($"Impossible to clamp: min.X {min.X} > max.X {max.X}");
        if (min.Y > max.Y) throw new Exception($"Impossible to clamp: min.Y {min.Y} > max.Y {max.Y}");
        return resolutions.Where(resolution => resolution.X >= min.X &&
                                               resolution.X <= max.X &&
                                               resolution.Y >= min.Y &&
                                               resolution.Y <= max.Y);

    }

    public static IEnumerable<ScaledResolution> ExpandResolutions(this IEnumerable<Resolution> resolutions, Resolution baseResolution, IEnumerable<AspectRatio> aspectRatios, Resolution? maxSize = null) {
        return resolutions
            .Select(resolution => resolution.ExpandResolutionByWith(baseResolution, aspectRatios, maxSize))
            .SelectMany(list => list)
            .Distinct(ScaledResolution.ComparerByBaseSize)
            .OrderBy(x => x, Resolution.ComparerByHeight)
            .ToList();
    }
}