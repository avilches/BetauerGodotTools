using System.Collections.Generic;
using System.Linq;
using Godot;

namespace Betauer.Application.Screen {
    public static class Resolutions {
        private static readonly List<Resolution> _all = new List<Resolution>();

        // 16:9 https://en.wikipedia.org/wiki/16:9_aspect_ratio
        public static Resolution FULLHD_DIV6 = AddToAll(new Resolution(320, 180)); // 1920x1080 / 6
        public static Resolution FULLHD_DIV4 = AddToAll(new Resolution(480, 270)); // 1920x1080 / 4
        public static Resolution FULLHD_DIV2 = AddToAll(new Resolution(960, 540)); // 1920x1080 / 2
        public static Resolution FULLHD = AddToAll(new Resolution(1920, 1080));
        public static Resolution FULLHD_2K = AddToAll(new Resolution(2560, 1440)); // 1920x1080 * 1.33 aka "2K"
        public static Resolution FULLHD_4K = AddToAll(new Resolution(3840, 2160)); // 1920x1080 * 2 // aka 4K

        public static Resolution FULLHD_DIV1_875 = AddToAll(new Resolution(1024, 576)); // 1920x1080 / 1.875
        public static Resolution FULLHD_DIV1_5 = AddToAll(new Resolution(1280, 720)); // 1920x1080 / 1.5
        public static Resolution FULLHD_DIV1_2 = AddToAll(new Resolution(1600, 900)); // 1920x1080 / 1.2
        public static Resolution FULLHD_DIV3 = AddToAll(new Resolution(640, 360)); // 1920x1080 / 3

        // 16:10 https://en.wikipedia.org/wiki/16:10_aspect_ratio
        public static Resolution R1610SMALL1 = AddToAll(new Resolution(640, 400));
        public static Resolution R1610SMALL2 = AddToAll(new Resolution(960, 600));
        public static Resolution WXGA = AddToAll(new Resolution(1280, 800));
        public static Resolution WXGAplus = AddToAll(new Resolution(1440, 900));
        public static Resolution WSXGAplus = AddToAll(new Resolution(1680, 1050));
        public static Resolution WUXGA = AddToAll(new Resolution(1920, 1200));
        public static Resolution WQXGA = AddToAll(new Resolution(2560, 1600));
        public static Resolution WQUXGA = AddToAll(new Resolution(3840, 2400));

        // 21:9 https://en.wikipedia.org/wiki/21:9_aspect_ratio
        // 64:27	2.370
        public static Resolution UWDIE237_0 = AddToAll(new Resolution(2560, 1080));
        public static Resolution UWDIE237_1 = AddToAll(new Resolution(5120, 2160));
        public static Resolution UWDIE237_2 = AddToAll(new Resolution(7680, 3240));
        public static Resolution UWDIE237_3 = AddToAll(new Resolution(10240, 4320));

        // 43:18	2.38
        public static Resolution UWDI238_0 = AddToAll(new Resolution(3440, 1440));
        public static Resolution UWDI238_1 = AddToAll(new Resolution(5160, 2160));
        public static Resolution UWDI238_2 = AddToAll(new Resolution(6880, 2880));

        //	12:5	2.4
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
        public static List<ScaledResolution> ExpandResolutionByWith(this Resolution from, IEnumerable<AspectRatio> aspectRatios, Resolution? maxSize = null) {
            if (maxSize == null) maxSize = new Resolution(OS.GetScreenSize());
            var maxScale = Resolution.CalculateMaxScale(from.Size, maxSize.Size);
            List<ScaledResolution> resolutions = new List<ScaledResolution>();
            for (var scale = 1; scale <= maxScale; scale++) {
                var scaledResolution = new ScaledResolution(from.Size, from.Size * scale);
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
                            var newWidth = scaledResolution.y * aspectRatio.Ratio;
                            if (newWidth <= maxSize.x) {
                                var newResolution = new Vector2(newWidth, scaledResolution.y);
                                var scaledResolutionUpdated = new ScaledResolution(from.Size, newResolution);
                                resolutions.Add(scaledResolutionUpdated);
                            }
                        } else {
                            // Convert the resolution to a stretcher aspect ratio, keeping the width and adding more height
                            // So, if base is 1920x1080 = 1,77777 16:9
                            // to 4:3 => 1,333
                            // y=1920/1,333=823
                            // 1920x1440 = 1,3333 3:4
                            var newHeight = scaledResolution.x / aspectRatio.Ratio;
                            if (newHeight <= maxSize.y) {
                                var newResolution = new Vector2(scaledResolution.x, newHeight);
                                var scaledResolutionUpdated = new ScaledResolution(from.Size, newResolution);
                                resolutions.Add(scaledResolutionUpdated);
                            }
                        }
                    }
                }
            }
            return resolutions;
        }

        public static IEnumerable<Resolution> Clamp(this IEnumerable<Resolution> resolutions, Vector2 min) {
            return Clamp(resolutions, min, OS.GetScreenSize());
        }

        public static IEnumerable<Resolution> Clamp(this IEnumerable<Resolution> resolutions, Vector2 min, Vector2 max) {
            return resolutions.Where(resolution => resolution.x >= min.x &&
                                                   resolution.x <= max.x &&
                                                   resolution.y >= min.y &&
                                                   resolution.y <= max.y);

        }

        public static IEnumerable<ScaledResolution> ExpandResolutions(this IEnumerable<Resolution> resolutions, Resolution baseResolution, IEnumerable<AspectRatio> aspectRatios, Resolution? maxSize = null) {
            return resolutions
                .Select(resolution => new ScaledResolution(baseResolution.Size, resolution.Size).ExpandResolutionByWith(aspectRatios, maxSize))
                .SelectMany(list => list)
                .Distinct(ScaledResolution.ComparerByBaseSize)
                .OrderBy(x => x, Resolution.ComparerByHeight)
                .ToList();
        }
        
    }
}