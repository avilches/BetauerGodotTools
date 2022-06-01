using System;
using Betauer;
using Betauer.Animation;
using Betauer.DI;
using Godot;

namespace Veronenger.Game.Controller.UI {
    public class RedefineActionButton : ActionButton {
        [OnReady("ActionHint")] public ActionHint ActionHint;
    }
}
