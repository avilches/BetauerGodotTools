using Betauer.DI.Attributes;
using Betauer.Input;

namespace Veronenger.Character.InputActions;

[Transient]
public class MultiplePlayerActionsContainer : JoypadContainer, ICharacterHandler {
    
    [Inject] public InputActionsContainer PlayerActionsContainer { get; set; }
    
    public AxisAction Lateral { get; private set; }
    public AxisAction Vertical { get; private set; }
    public InputAction Jump { get; private set; }
    public InputAction Attack { get; private set; }
    public InputAction Float { get; private set; }
    public InputAction Drop { get; private set; }
    public InputAction NextItem { get; private set; }
    public InputAction PrevItem { get; private set; }
    public InputAction Left { get; private set; }
    public InputAction Right { get; private set; }
    public InputAction Up { get; private set; }
    public InputAction Down { get; private set; }

    protected override InputActionsContainer Source => PlayerActionsContainer;
}