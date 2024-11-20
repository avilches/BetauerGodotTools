namespace Betauer.Core.PCG.Examples.ThirdPartyCode.Metazelda;

/**
* Represents a single key or lock within the lock-and-key puzzle.
* <p>
* Each MZSymbol has a 'value'. Two MZSymbols are equivalent iff they have the same
* 'value'.
*/
public class MZSymbol {
    /**
 * MZSymbol map with special meanings.
 * <p>
 * Certain items (such as (int) MZSymbolValue.Start, (int) MZSymbolValue.Goal, MZSymbolValue.Boss) serve no purpose in the puzzle
 * other than as markers for the client of the library to place special game
 * objects.
 * <p>
 * The MZSymbolValue.SwitchOn and MZSymbolValue.SwitchOff symbols do not appear in rooms, only in
 * {@link MZCondition}s and {@link MZEdge}s.
 * <p>
 * The MZSymbolValue.Switch item does not give the player the MZSymbolValue.Switch symbol, instead the
 * player may choose to either
 * <ul>
 * <li>lose the MZSymbolValue.SwitchOff symbol (if they have it), and gain the MZSymbolValue.SwitchOn
 *      symbol; or
 * <li>lose the MZSymbolValue.SwitchOn symbol (if they have it), and gain the MZSymbolValue.SwitchOff
 *      symbol.
 * <ul>
 * <p>
 */
    public enum MZSymbolValue {
        Start = -1,
        Goal = -2,
        Boss = -3,
        SwitchOn = -4,     // used as a condition (lock)
        SwitchOff = -5,    // used as a condition (lock)
        Switch = -6        // used as an item (key) within a room
    }

    protected readonly int value;
    protected readonly string name;
    
    /**
 * Creates a MZSymbol with the given value.
 *
 * @param value value to give the MZSymbol
 */
    public MZSymbol(int value) {
        this.value = value;
        
        if (value == (int) MZSymbolValue.Start)
            name = "Start";
        else if (value == (int) MZSymbolValue.Goal)
            name = "Goal";
        else if (value == (int) MZSymbolValue.Boss)
            name = "Boss";
        else if (value == (int) MZSymbolValue.SwitchOn)
            name = "On";
        else if (value == (int) MZSymbolValue.SwitchOff)
            name = "Off";
        else if (value == (int) MZSymbolValue.Switch)
            name = "SW";
        else if (value >= 0 && value < 26)
            name = ((char)((int)'A' + value)).ToString();
        else
            name = value.ToString();
    }
    
    public override bool Equals(object other) {
        if (other.GetType() == typeof(MZSymbol)) {
            return value == ((MZSymbol)other).value;
        } else {
            return true;
            //return base.Equals(other);
        }
    }
    
    public static bool Equals(MZSymbol a, MZSymbol b) {
        if (a == b) return true;
        if (b == null) return a.Equals(b);
        return b.Equals(a);
    }
    
    public override int GetHashCode() {
        return value;
    }
    
    /**
 * @return the value of the MZSymbol
 */
    public int GetValue() {
        return value;
    }
    
    /**
 * @return whether the symbol is the special (int) MZSymbolValue.Start symbol
 */
    public bool IsStart() {
        return value == (int) MZSymbolValue.Start;
    }
    
    /**
 * @return whether the symbol is the special (int) MZSymbolValue.Goal symbol
 */
    public bool IsGoal() {
        return value == (int) MZSymbolValue.Goal;
    }
    
    /**
 * @return whether the symbol is the special MZSymbolValue.Boss symbol
 */
    public bool IsBoss() {
        return value == (int) MZSymbolValue.Boss;
    }
    
    /**
 * @return whether the symbol is the special MZSymbolValue.Switch symbol
 */
    public bool IsSwitch() {
        return value == (int) MZSymbolValue.Switch;
    }
    
    /**
 * @return whether the symbol is a key
 */
    public bool IsKey() {
        return value >= 0;
    }
    
    /**
 * @return whether the symbol is one of the special MZSymbolValue.Switch_{On,Off} symbols
 */
    public bool IsSwitchState() {
        return value == (int) MZSymbolValue.SwitchOn || value == (int) MZSymbolValue.SwitchOff;
    }
    
    //@Override
    public override string ToString() {
        return name;
    }
    
}