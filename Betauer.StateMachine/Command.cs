using System;
using System.Collections.Generic;

namespace Betauer.StateMachine; 

public enum CommandType {
    Init,
    Set,
    Push,
    PopPush,
    Pop,
    Stay,
    SendEvent
}    
        
public readonly struct Command<TStateKey, TEventKey>
    where TStateKey : Enum
    where TEventKey : Enum {
    
    internal static Command<TStateKey, TEventKey> CreateInit(TStateKey stateKey) => new(CommandType.Init, stateKey, default, -1);
    
    internal static Command<TStateKey, TEventKey> CreateSet(TStateKey stateKey) => new(CommandType.Set, stateKey, default, -1);
    internal static Command<TStateKey, TEventKey> CreatePush(TStateKey stateKey) => new(CommandType.Push, stateKey, default, -1);
    internal static Command<TStateKey, TEventKey> CreatePopPush(TStateKey stateKey) => new(CommandType.PopPush, stateKey, default, -1);
    internal static Command<TStateKey, TEventKey> CreatePop() => new(CommandType.Pop, default, default, -1);
    internal static Command<TStateKey, TEventKey> CreateStay() => new(CommandType.Stay, default, default, -1);

    internal static Command<TStateKey, TEventKey> CreateSendEvent(TEventKey eventKey, int weight) => new(CommandType.SendEvent, default, eventKey, weight);

    public readonly CommandType Type;
    public readonly TStateKey StateKey;
    public readonly TEventKey EventKey;
    public readonly int Weight;

    private Command(CommandType type, TStateKey stateKey, TEventKey eventKey, int weight) {
        Type = type;
        StateKey = stateKey;
        EventKey = eventKey;
        Weight = weight;
    }

    public bool IsInit() => Type == CommandType.Init;
    public bool IsSet() => Type == CommandType.Set;
    public bool IsPush() => Type == CommandType.Push;
    public bool IsPop() => Type == CommandType.Pop;
    public bool IsPopPush() => Type == CommandType.PopPush;
    public bool IsStay() => Type == CommandType.Stay;
    
    public bool IsSendEvent() => Type == CommandType.SendEvent;

    public bool IsState(TStateKey stateKey) => EqualityComparer<TStateKey>.Default.Equals(StateKey, stateKey);
    
    public bool IsStayOrSet(TStateKey stateKey) => IsStay() || (IsSet() && IsState(stateKey));


}