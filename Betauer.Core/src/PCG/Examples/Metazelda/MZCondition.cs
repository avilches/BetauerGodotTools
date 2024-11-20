using System;

namespace Betauer.Core.PCG.Examples.Metazelda;

/**
 * Used to represent {@link MZRoom}s' preconditions.
 * <p>
 * A MZRoom's precondition can be considered the set of MZSymbols from the other
 * MZRooms that the player must have collected to be able to reach this room. For
 * instance, if the MZRoom is behind a locked door, the precondition for the
 * MZRoom includes the key for that lock.
 * <p>
 * In practice, since there is always a time ordering on the collection of keys,
 * this can be implemented as a count of the number of keys the player must have
 * (the 'keyLevel').
 * <p>
 * The state of the {@link MZDungeon}'s switch is also recorded in the MZCondition.
 * A MZRoom behind a link that requires the switch to be flipped into a particular
 * state will have a precondition that includes the switch's state.
 * <p>
 * A MZCondition is 'satisfied' when the player has all the keys it requires and
 * when the dungeon's switch is in the state that it requires.
 * <p>
 * A MZCondition x implies a MZCondition y if and only if y is satisfied whenever x
 * is.
 */
public class MZCondition {

    /**
     * A type to represent the required state of a switch for the MZCondition to
     * be satisfied.
     */
    public enum SwitchState
    {
        /**
         * The switch may be in any state.
         */
        Either,
        /**
         * The switch must be off.
         */
        Off,
        /**
         * The switch must be on.
         */
        On
    }

    protected int keyLevel;
    protected SwitchState switchState;
    protected bool initialized = false;

    /**
     * Create a MZCondition that is always satisfied.
     */
    public MZCondition() {
        keyLevel = 0;
        switchState = SwitchState.Either;
    }
    
    /**
     * Creates a MZCondition that requires the player to have a particular
     * {@link MZSymbol}.
     * 
     * @param e the symbol that the player must have for the MZCondition to be
     *          satisfied
     */
    public MZCondition(MZSymbol e) {
        if (e.GetValue() == (int) MZSymbol.MZSymbolValue.SwitchOff) {
            keyLevel = 0;
            switchState = SwitchState.Off;
        } else if (e.GetValue() == (int) MZSymbol.MZSymbolValue.SwitchOn) {
            keyLevel = 0;
            switchState = SwitchState.On;
        } else {
            keyLevel = e.GetValue()+1;
            switchState = SwitchState.Either;
        }
    }
    
    /**
     * Creates a MZCondition from another MZCondition (copy it).
     * 
     * @param other the other MZCondition
     */
    public MZCondition(MZCondition other) {
        keyLevel = other.keyLevel;
        switchState = other.switchState;
    }
    
    /**
     * Creates a MZCondition that requires the switch to be in a particular state.
     * 
     * @param switchState   the required state for the switch to be in
     */
    public MZCondition(SwitchState switchState) {
        keyLevel = 0;
        this.switchState = switchState;
    }
    
    public override bool Equals(object other) {
        if (other.GetType() == typeof(MZCondition)) {
            MZCondition o = (MZCondition)other;
            return keyLevel == o.keyLevel && switchState == o.switchState;
        } else {
            return base.Equals(other);
        }
    }
    
    private void Add(MZSymbol sym) {
        if (sym.GetValue() == (int) MZSymbol.MZSymbolValue.SwitchOff) {
            switchState = SwitchState.Off;
        } else if (sym.GetValue() == (int) MZSymbol.MZSymbolValue.SwitchOn) {
            switchState = SwitchState.On;
        } else {
            keyLevel = Math.Max(keyLevel, sym.GetValue()+1);
        }
    }

    private void Add(MZCondition cond) {
        if (switchState == SwitchState.Either) {
            switchState = cond.switchState;
        }
        keyLevel = Math.Max(keyLevel, cond.keyLevel);
    }
    
    /**
     * Creates a new MZCondition that requires this MZCondition to be satisfied and
     * requires another {@link MZSymbol} to be obtained as well.
     * 
     * @param sym   the Added symbol the player must have for the new MZCondition
     *              to be satisfied
     * @return      the new MZCondition
     */
    public MZCondition And(MZSymbol sym) {
        MZCondition result = new MZCondition(this);
        result.Add(sym);
        return result;
    }
    
    /**
     * Creates a new MZCondition that requires this MZCondition and another
     * MZCondition to both be satisfied.
     * 
     * @param other the other MZCondition that must be satisfied.
     * @return      the new MZCondition
     */
    public MZCondition And(MZCondition? other) {
        if (other == null) return this;
        MZCondition result = new MZCondition(this);
        result.Add(other);
        return result;
    }
    
    /**
     * Determines whether another MZCondition is necessarily true if this one is.
     * 
     * @param other the other MZCondition
     * @return  whether the other MZCondition is implied by this one
     */
    public bool Implies(MZCondition other) {
        return keyLevel >= other.keyLevel &&
               (switchState == other.switchState ||
                other.switchState == SwitchState.Either);
    }

    /**
     * Determines whether this MZCondition implies that a particular
     * {@link MZSymbol} has been obtained.
     * 
     * @param s the MZSymbol
     * @return  whether the MZSymbol is implied by this MZCondition
     */
    public bool Implies(MZSymbol s) {
        return Implies(new MZCondition(s));
    }
    
    /**
     * Gets the single {@link MZSymbol} needed to make this MZCondition and another
     * MZCondition identical.
     * <p>
     * If {@link #and}ed to both MZConditions, the MZConditions would then imply
     * each other.
     * 
     * @param other the other MZCondition
     * @return  the MZSymbol needed to make the MZConditions identical, or null if
     *          there is no single MZSymbol that would make them identical or if
     *          they are already identical.
     */
    public MZSymbol? SingleSymbolDifference(MZCondition other) {
        // If the difference between this and other can be made up by obtaining
        // a single new symbol, this returns the symbol. If multiple or no
        // symbols are required, returns null.
        
        if (this.Equals(other)) return null;
        if (switchState == other.switchState) {
            return new MZSymbol(Math.Max(keyLevel, other.keyLevel)-1);
        } else {
            if (keyLevel != other.keyLevel) return null;
            // Multiple symbols needed        ^^^

            if (switchState != SwitchState.Either &&
                other.switchState != SwitchState.Either)
                return null;
            
            SwitchState nonEither = switchState != SwitchState.Either
                ? switchState
                : other.switchState;

            return new MZSymbol(nonEither == SwitchState.On
                ? (int) MZSymbol.MZSymbolValue.SwitchOn
                : (int) MZSymbol.MZSymbolValue.SwitchOff);
        }
    }
    
    public override string ToString() {
        String result = "";
        if (keyLevel != 0) {
            result += new MZSymbol(keyLevel-1).ToString();
        }
        if (switchState != SwitchState.Either) {
            if (!result.Equals("")) result += ",";
            result += switchState.ToSymbol().ToString();
        }
        return result;
    }
    
    /**
     * Get the number of keys that need to have been obtained for this MZCondition
     * to be satisfied.
     * 
     * @return the number of keys
     */
    public int GetKeyLevel() {
        return keyLevel;
    }
    
    /**
     * Get the state the switch is required to be in for this MZCondition to be
     * satisfied.
     */
    public SwitchState GetSwitchState() {
        return switchState;
    }
    
}

public static class Extensions
{
    /**
     * Convert this SwitchState to a {@link MZSymbol}.
     * 
     * @return  a symbol representing the required state of the switch or
     *          null if the switch may be in any state
     */
    public static MZSymbol ToSymbol(this MZCondition.SwitchState state)
    {
        switch (state)
        {
            case MZCondition.SwitchState.Off:
                return new MZSymbol((int) MZSymbol.MZSymbolValue.SwitchOff);
            case MZCondition.SwitchState.On:
                return new MZSymbol((int) MZSymbol.MZSymbolValue.SwitchOn);
            default:
                return null;
        }
    }

    /**
     * Invert the required state of the switch.
     * 
     * @return  a SwitchState with the opposite required switch state or
     *          this SwitchState if no particular state is required
     */
    public static MZCondition.SwitchState Invert(this MZCondition.SwitchState state)
    {
        switch (state)
        {
            case MZCondition.SwitchState.Off: return MZCondition.SwitchState.On;
            case MZCondition.SwitchState.On: return MZCondition.SwitchState.Off;
            default:
                return state;
        }
    }
};