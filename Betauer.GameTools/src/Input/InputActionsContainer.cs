using System.Collections.Generic;
using System.Linq;
using System.Reflection;
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
    public List<AxisAction> AxisActions { get; } = new();
    public List<InputAction> InputActions { get; } = new();
    
    private readonly HashSet<GodotInputHandler> _onInputActions = new();

    public AxisAction? GetAxisAction(string name) {
        return AxisActions.Find(action => action.Name == name);
    }

    public InputAction? GetInputAction(string name) {
        return InputActions.Find(action => action.Name == name);
    }

    public void AddActionsFromProperties(object instance) {
        var propertyInfos = instance.GetType().GetProperties();

        propertyInfos
            .Where(p => typeof(InputAction).IsAssignableFrom(p.PropertyType))
            .Select(p => p.GetValue(instance))
            .Where(i => i != null)
            .Cast<InputAction>()
            .ForEach(Add);
        
        propertyInfos
            .Where(p => typeof(AxisAction).IsAssignableFrom(p.PropertyType))
            .Select(p => p.GetValue(instance))
            .Where(i => i != null)
            .Cast<AxisAction>()
            .ForEach(Add);
    }
    
    public void Add(AxisAction axisAction) {
        if (AxisActions.Contains(axisAction)) return; // Avoid duplicates
        AxisActions.Add(axisAction);
        TryLinkAxisActionToNegativePositiveInputs(axisAction);
        TryAddSaveSettings(axisAction);
        if (axisAction.Negative != null) Add(axisAction.Negative);
        if (axisAction.Positive != null) Add(axisAction.Positive);
    }

    public void Add(InputAction inputAction) {
        if (InputActions.Contains(inputAction)) return; // Avoid duplicates
        TryAddSaveSettings(inputAction);
        InputActions.Add(inputAction);
        if (inputAction.AxisName != null) {
            if (AxisActions.Find(action => action.Name == inputAction.AxisName) is AxisAction axisAction) {
                TryLinkAxisActionToNegativePositiveInputs(axisAction);
            }
        }
        CheckInputHandler(inputAction);
    }

    private void TryLinkAxisActionToNegativePositiveInputs(AxisAction axisAction) {
        if (axisAction.Negative == null && axisAction.Positive == null) {
            var pairs = InputActions.FindAll(action => action.AxisName == axisAction.Name);
            if (pairs.Count == 2) {
                axisAction.SetNegativeAndPositive(pairs[0], pairs[1]);
                Add(pairs[0]);
                Add(pairs[1]);
            }
        }
    }

    public void Remove(InputAction inputAction) {
        if (InputActions.Remove(inputAction)) {
            CheckInputHandler(inputAction);
        }
    }

    public void Remove(AxisAction axisAction) {
        AxisActions.Remove(axisAction);
        if (axisAction.Negative != null) Remove(axisAction.Negative);
        if (axisAction.Positive != null) Remove(axisAction.Positive);
    }

    public void EnableAll(bool enable = true) {
        InputActions.ForEach(action => action.Enable(enable));
    }

    public void DisableAll() {
        InputActions.ForEach(action => action.Disable());
    }

    public void Clear() {
        InputActions.ForEach(action => {
            action.Disable();
            TryRemoveSaveSettings(action);
        });
        InputActions.Clear();
        AxisActions.ForEach(axisAction => {
            axisAction.Disable();
            TryRemoveSaveSettings(axisAction);
        });
        AxisActions.Clear();
    }

    internal void CheckInputHandler(InputAction inputAction) {
        if (inputAction.Handler is GodotInputHandler handler && handler.HasJustTimers) {
            if (inputAction.Enabled) {
                _onInputActions.Add(handler); // This is Set, so it already take care of duplicates
            } else {
                _onInputActions.Remove(handler);
            }
        }
    }

    // public override void _Input(InputEvent e) {
    //     if (_onInputActions.Count == 0) {
    //         SetProcessInput(false);
    //         return;
    //     }
    //     var span = CollectionsMarshal.AsSpan(_onInputActions);
    //     for (var i = 0; i < span.Length; i++) {
    //         var handler = span[i];
    //         handler.UpdateJustTimers();
    //     }
    // }
}