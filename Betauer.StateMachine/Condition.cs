using System;

namespace Betauer.StateMachine;

public abstract class Condition {
    public enum Type {
        Asap,
        Always,
        Lazy
    }
}
public class Condition<TStateKey, TEventKey> : Condition 
    where TStateKey : Enum
    where TEventKey : Enum {
    
        
    private readonly Type _type;
    private readonly Func<bool> _predicate;
    private readonly Func<CommandContext<TStateKey, TEventKey>, Command<TStateKey, TEventKey>>? _execute;
    private readonly Command<TStateKey, TEventKey> _result;

    internal Condition(Type type, Func<bool> predicate, Func<CommandContext<TStateKey, TEventKey>, Command<TStateKey, TEventKey>> execute) {
        _type = type;
        _predicate = predicate;
        _execute = execute;
    }

    internal Condition(Type type, Func<bool> predicate, Command<TStateKey, TEventKey> result) {
        _type = type;
        _predicate = predicate;
        _result = result;
    }

    internal bool IsPredicateTrue() => _predicate();
    internal bool IsPredicateTrue(Type type1, Type type2) => (_type == type1 || _type == type2) && _predicate();

    internal Command<TStateKey, TEventKey> GetResult(CommandContext<TStateKey, TEventKey> ctx) =>
        _execute?.Invoke(ctx) ?? _result;

}