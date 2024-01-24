using System;
using System.Collections.Generic;
using System.Linq;
using Betauer.Application.Settings;
using Betauer.Core;
using Betauer.Input.Handler;

namespace Betauer.Input;

/*
Caso 1: solo hay un jugador, 
se usa InputActionsContainer o SingleJoypadActionsContainer con su SettingsContainer. Solo se crea una vez y solo hay un conjunto de input cada uno con su saveSetting para siempre


Caso 2: hay multiples jugadores, todos comparten el mismo mapeo de botones, salvo el primer jugador que puede usar teclado y raton
Se crea un PlayerActionsContainer (que herada de InputActionsContainer) por cada jugador y se van creando. Cada uno tiene su joystyickId y su playerId.
A medida que se crean mas y mas, se van creando mas InputAction, y en cada uno se añade un nuevo SaveSetting, que se añade al SettingContainer. Cuando se
deja de usar un jugado y se elimina el PlayerActionsContainer del multiplayer, se eliminan todos los InputAction pero no se eliminar los SaveSetting. ESTO ES UN ERROR.
Una de dos, o se borran los SaveSetting cuando se borran los InputAction, o se reciclan.

Al compartir el mapeo, cuando se redefine un boton del jugador 1, se debe reflejar en el resto. Ahora se puede simular haciendo un Load() en el resto de jugadores
que tengan un InputAction con el mismo SaveAs que el que se ha modificado. Esto habria que hacerlo automaticamente.

tareas:
- refrescar el resto de valores cuando el player 1 se redefine
- reciclar los SaveSetting


Caso 3: solo hay 2 jugadores y cada uno tiene sus mapeos de botones diferentes (por ejemplo uno de lucha). Tiene que haber dos InputActionsContainer con diferentes SaveSetting, 
se les tiene que añadir a los SaveSetting un sufijo, como "_P1" y "_P2". Esto se puede hacer con un Suffix en el SaveSettingContainerAware.

 */

public partial class InputActionsContainer {
    
    public SettingsContainer? SettingsContainer { get; private set; }

    /// <summary>
    /// This method allows you to call Load() and Save() method to update the actions from/to this SettingContainer.
    /// The current value of the actions will be used as the default value of the SaveSetting of each action, used by RestoreDefaults() method.
    /// </summary>
    /// <param name="settingsContainer"></param>
    public void ConfigureSaveSettings(SettingsContainer settingsContainer) {
        if (SettingsContainer != null) throw new Exception("SettingsContainer already configured");
        SettingsContainer = settingsContainer;
        AxisActions.ForEach(TryAddSaveSettings);
        InputActions.ForEach(TryAddSaveSettings);
    }

    public void RestoreDefaults() {
        if (SettingsContainer == null) return;
        InputActions.ForEach(inputAction => {
            var saveSetting = FindSaveSetting(inputAction);
            if (saveSetting == null) return;
            inputAction.Update(u => {
                u.ImportJoypad(saveSetting.DefaultValue);
                u.ImportKeys(saveSetting.DefaultValue);
                u.ImportMouse(saveSetting.DefaultValue);
            });
        });
        AxisActions.ForEach(axisAction => {
            var saveSetting = FindSaveSetting(axisAction);
            if (saveSetting == null) return;
            axisAction.Import(saveSetting.DefaultValue, true);
        });
    }

    /// <summary>
    /// Load from the SettingsContainer all the actions that have a SaveAs attribute. The SettingsContainer must be configured first.
    /// The reload flag allows you to reload the ini file from disk before loading the actions (it just calls to SettingsContainer.Load()
    /// </summary>
    public void Load(bool reload = true) {
        if (SettingsContainer == null) return;
        if (reload) SettingsContainer.Load();
        InputActions.ForEach(inputAction => {
            var saveSetting = FindSaveSetting(inputAction);
            if (saveSetting == null) return;
            inputAction.Update(u => {
                u.ImportJoypad(saveSetting.Value);
                u.ImportKeys(saveSetting.Value);
                u.ImportMouse(saveSetting.Value);
            });
        });
        AxisActions.ForEach(axisAction => {
            var saveSetting = FindSaveSetting(axisAction);
            if (saveSetting == null) return;
            axisAction.Import(saveSetting.Value, false);
        });
    }

    /// <summary>
    /// Dumps all the actions that have a SaveAs attribute to the SettingsContainer. If flush is true, the SettingsContainer is saved to disk, but pay attention
    /// if the SettingsContainer has any other settings than the actions, they will be saved too. 
    /// </summary>
    public void Save(bool flush = true) {
        if (SettingsContainer == null) return;
        InputActions.ForEach(inputAction => {
            var saveSetting = FindSaveSetting(inputAction);
            if (saveSetting == null) return;
            saveSetting.Value = inputAction.Export();
        });
        AxisActions.ForEach(axisAction => {
            var saveSetting = FindSaveSetting(axisAction);
            if (saveSetting == null) return;
            saveSetting.Value = axisAction.Export();
        });
        if (flush) SettingsContainer.Save();
    }

    public SaveSetting<string>? FindSaveSetting(InputAction inputAction) {
        return SettingsContainer?.Find(inputAction.SaveAs) as SaveSetting<string>;
    }

    public SaveSetting<string>? FindSaveSetting(AxisAction axisAction) {
        return SettingsContainer?.Find(axisAction.SaveAs) as SaveSetting<string>;
    }

    private void TryAddSaveSettings(InputAction? inputAction) {
        if (SettingsContainer == null || inputAction?.SaveAs == null || FindSaveSetting(inputAction) != null) return;
        var saveSetting = Setting.Create(inputAction.SaveAs, inputAction.Export(), false);
        SettingsContainer.Add(saveSetting);
    }

    private void TryAddSaveSettings(AxisAction? axisAction) {
        if (SettingsContainer == null || axisAction?.SaveAs == null || FindSaveSetting(axisAction) != null) return;
        var saveSetting = Setting.Create(axisAction.SaveAs, axisAction.Export(), false);
        SettingsContainer.Add(saveSetting);
    }

    private void TryRemoveSaveSettings(InputAction? inputAction) {
        if (SettingsContainer == null || inputAction?.SaveAs == null) return;
        var saveSetting = FindSaveSetting(inputAction);
        if (saveSetting != null) SettingsContainer.Remove(saveSetting);
    }

    private void TryRemoveSaveSettings(AxisAction? axisAction) {
        if (SettingsContainer == null || axisAction?.SaveAs == null) return;
        var saveSetting = FindSaveSetting(axisAction);
        if (saveSetting != null) SettingsContainer.Remove(saveSetting);
    }
}