using System;

namespace Betauer.Bus; 

public class EventConsumer<TEvent> {
    public Action<TEvent>? Action { get; set; }
    public Func<bool>? UnsubscribeIfFunc { get; set; }
        
    public void Do(Action<TEvent>? action) {
        Action = action;
    }

    public void UnsubscribeIf(Func<bool> func) {
        UnsubscribeIfFunc = func;
    }

    public void Execute(TEvent @event) {
        Action.Invoke(@event);
    }

    public void Unsubscribe() {
        Action = null;
        UnsubscribeIfFunc = null;
    }

    public bool IsValid() {
        return Action != null && (UnsubscribeIfFunc == null || !UnsubscribeIfFunc.Invoke());
    }
}