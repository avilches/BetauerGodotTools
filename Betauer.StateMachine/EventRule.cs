using System;

namespace Betauer.StateMachine; 

public class EventRule<TStateKey, TEventKey> 
    where TStateKey : Enum
    where TEventKey : Enum {
        
    private readonly Func<CommandContext<TStateKey, TEventKey>, Command<TStateKey, TEventKey>>? _execute;
    private readonly Command<TStateKey, TEventKey> _result;

    internal EventRule(Func<CommandContext<TStateKey, TEventKey>, Command<TStateKey, TEventKey>> execute) {
        _execute = execute;
    }

    internal EventRule(Command<TStateKey, TEventKey> result) {
        _result = result;
    }

    internal Command<TStateKey, TEventKey> GetResult(CommandContext<TStateKey, TEventKey> ctx) =>
        _execute?.Invoke(ctx) ?? _result;

}