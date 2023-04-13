using Betauer.Application.Settings;
using Godot;

namespace Betauer.Input;

public interface IAction {
    public string Name { get; }
    public float Strength { get; }
    public float RawStrength { get; }
    JoyAxis Axis { get; }
    bool IsEvent(InputEvent inputEvent);
    void Enable(bool enabled);

    public void UnsetInputActionsContainer();
    public void SetInputActionsContainer(InputActionsContainer inputActionsContainer);

    public SaveSetting<string>? SaveSetting { get; set; }
    public void Parse(string values, bool reset);
    public void ResetToDefaults();
    public string AsString();
    public void Load();
    public void Save();

    public void SimulatePress(float strength);
    public void SimulateRelease();
    public void ClearJustStates();


}