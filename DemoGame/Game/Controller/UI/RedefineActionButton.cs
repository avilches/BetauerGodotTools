using Betauer.Input;
using Betauer.OnReady;
using Godot;

namespace Veronenger.Game.Controller.UI {
    public class RedefineActionButton : Button {
        [OnReady("ActionHint")] public ActionHint ActionHint;
        
        public InputAction InputAction;
    }
    
}
