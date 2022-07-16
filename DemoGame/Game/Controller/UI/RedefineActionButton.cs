using Betauer;
using Betauer.DI;
using Betauer.Input;
using Betauer.OnReady;

namespace Veronenger.Game.Controller.UI {
    public class RedefineActionButton : ButtonWrapper {
        [OnReady("ActionHint")] public ActionHint ActionHint;
        
        public ActionState ActionState;
    }
    
}
