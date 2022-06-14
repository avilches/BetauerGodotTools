using Betauer.DI;
using Betauer.Input;

namespace Veronenger.Game.Controller.UI {
    public class RedefineActionButton : ButtonWrapper {
        [OnReady("ActionHint")] public ActionHint ActionHint;
        
        public ActionState ActionState;
    }
    
}
