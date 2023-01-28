using System;

namespace Betauer.StateMachine; 

public class Condition<TStateKey, TEventKey> 
    where TStateKey : Enum
    where TEventKey : Enum {
        
    private readonly Func<bool> _predicate;
    private readonly Func<CommandContext<TStateKey, TEventKey>, Command<TStateKey, TEventKey>>? _execute;
    private readonly Command<TStateKey, TEventKey> _result;

    internal Condition(Func<bool> predicate, Func<CommandContext<TStateKey, TEventKey>, Command<TStateKey, TEventKey>> execute) {
        _predicate = predicate;
        _execute = execute;
    }

    internal Condition(Func<bool> predicate, Command<TStateKey, TEventKey> result) {
        _predicate = predicate;
        _result = result;
    }

    internal bool IsPredicateTrue() => _predicate();

    internal Command<TStateKey, TEventKey> GetResult(CommandContext<TStateKey, TEventKey> ctx) =>
        _execute?.Invoke(ctx) ?? _result;

}