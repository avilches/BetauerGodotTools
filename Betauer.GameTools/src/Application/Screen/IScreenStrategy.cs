using System.Collections.Generic;
using Betauer.Application.Screen.Resolution;

namespace Betauer.Application.Screen; 

public interface IScreenStrategy {
    bool IsFullscreen();
    void SetFullscreen();
    void SetBorderless(bool borderless);
    void SetWindowed(Resolution.Resolution resolution);
    List<ScaledResolution> GetResolutions();
    void Disable();
    void Apply();
    void SetScreenConfig(ScreenConfig screenConfig);
    void CenterWindow();
}