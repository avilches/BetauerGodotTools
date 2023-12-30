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

    public virtual void Unsubscribe() {
        Action = null;
        UnsubscribeIfFunc = null;
    }

    public virtual bool IsValid() {
        return Action != null && (UnsubscribeIfFunc == null || !UnsubscribeIfFunc.Invoke());
    }
}

public class EventConsumer<TEvent, TOut> {
    public Func<TEvent, TOut>? Action { get; set; }
    public Func<bool>? UnsubscribeIfFunc { get; set; }
        
    public void Do(Func<TEvent, TOut>? action) {
        Action = action;
    }

    public void UnsubscribeIf(Func<bool> func) {
        UnsubscribeIfFunc = func;
    }

    public TOut Execute(TEvent @event) {
        return Action != null ? Action.Invoke(@event) : throw new Exception();
    }

    public virtual void Unsubscribe() {
        Action = null;
        UnsubscribeIfFunc = null;
    }

    public virtual bool IsValid() {
        return Action != null && (UnsubscribeIfFunc == null || !UnsubscribeIfFunc.Invoke());
    }
}