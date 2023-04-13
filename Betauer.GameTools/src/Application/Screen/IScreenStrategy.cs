using System.Collections.Generic;

namespace Betauer.Application.Screen; 

public interface IScreenStrategy {
    bool IsFullscreen();
    void SetFullscreen();
    void SetBorderless(bool borderless);
    void SetWindowed(Resolution resolution);
    List<ScaledResolution> GetResolutions();
    void Disable();
    void Apply();
    void SetScreenService(ScreenService screenService);
    void CenterWindow();
}