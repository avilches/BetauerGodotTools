using System.Collections.Generic;

namespace Betauer.Core.PCG.LockTree.Metazelda.util;

public class MZIntMap<V> : Dictionary<int,V> {
    //private static readonly long serialVersionUID = 1L;

    public int NewInt() {
        int k = Count;
        while (ContainsKey(k)) k++;
        return k;
    }
}