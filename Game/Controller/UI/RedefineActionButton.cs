using System;
using Betauer;
using Betauer.Animation;
using Betauer.DI;
using Godot;

namespace Veronenger.Game.Controller.UI {
    public class RedefineActionButton : ButtonWrapper {
        [OnReady("ActionHint")] public ActionHint ActionHint;
    }
}
