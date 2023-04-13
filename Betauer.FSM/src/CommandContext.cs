using System;

namespace Betauer.FSM; 

public class CommandContext<TStateKey, TEventKey> 
    where TStateKey : Enum
    where TEventKey : Enum {
        
    internal CommandContext() {
    }

    public Command<TStateKey, TEventKey> Send(TEventKey e, int weight = 0) => Command<TStateKey, TEventKey>.CreateSendEvent(e, weight); 

    public Command<TStateKey, TEventKey> Set(TStateKey state) => Command<TStateKey, TEventKey>.CreateSet(state); 
    public Command<TStateKey, TEventKey> Push(TStateKey state) => Command<TStateKey, TEventKey>.CreatePush(state); 
    public Command<TStateKey, TEventKey> PopPush(TStateKey state) => Command<TStateKey, TEventKey>.CreatePopPush(state); 
    public Command<TStateKey, TEventKey> Pop() => Command<TStateKey, TEventKey>.CreatePop(); 
    public Command<TStateKey, TEventKey> Stay() => Command<TStateKey, TEventKey>.CreateStay(); 
}