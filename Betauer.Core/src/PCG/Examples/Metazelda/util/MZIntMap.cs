using System.Collections.Generic;

namespace Betauer.Core.PCG.Examples.Metazelda.util;

public class MZIntMap<V> : Dictionary<int,V> {
    //private static readonly long serialVersionUID = 1L;

    public int NewInt() {
        int k = Count;
        while (ContainsKey(k)) k++;
        return k;
    }
}