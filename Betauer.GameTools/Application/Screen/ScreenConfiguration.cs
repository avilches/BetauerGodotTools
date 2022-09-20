using System.Collections.Generic;
using System.Linq;
using Godot;

namespace Betauer.Application.Screen {
    public class ScreenConfiguration {
        public Resolution DownScaledMinimumResolution { get; }
        public Resolution BaseResolution { get; }
        public bool IsResizeable { get;  }
        public List<Resolution> Resolutions { get; }
        public List<AspectRatio> AspectRatios { get; }

        // disabled: while the framebuffer will be resized to match the game window, nothing will be upscaled or downscaled (this includes GUIs).
        // 2d: the framebuffer is still resized, but GUIs can be upscaled or downscaled. This can result in blurry or pixelated fonts.
        // viewport: the framebuffer is resized, but computed at the original size of the project. The whole rendering will be pixelated. You generally do not want this, unless it's part of the game style.
        public SceneTree.StretchMode StretchMode;
        public SceneTree.StretchAspect StretchAspect;
        public float Zoom { get; }

        public ScreenConfiguration(Resolution downScaledMinimumResolution, Resolution baseResolution, SceneTree.StretchMode stretchMode,
            SceneTree.StretchAspect stretchAspect, List<Resolution>? resolutions = null, bool isResizeable = false, float zoom = 1f) {
            DownScaledMinimumResolution = downScaledMinimumResolution;
            BaseResolution = baseResolution;
            StretchMode = stretchMode;
            StretchAspect = stretchAspect;
            Resolutions = resolutions ?? Screen.Resolutions.GetAll(Screen.AspectRatios.Ratio16_9);
            AspectRatios = Resolutions.Select(r => r.AspectRatio).Distinct().ToList();
            IsResizeable = isResizeable;
            Zoom = zoom;
        }
    }
}