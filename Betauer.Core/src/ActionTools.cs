using System;

namespace Betauer.Core; 

    public class ActionTools {
        public static Action<TBase1>? Convert<TBase1, TSubType1>(Action<TSubType1> action) 
            where TSubType1 : TBase1 {
            return typeof(TSubType1) == typeof(TBase1)
                ? action as Action<TBase1>
                : (p1) => {
                    if (p1 is null or TSubType1) action((TSubType1)p1!);
                };
        }

        public static Action<TBase1, TBase2>? Convert<TBase1, TBase2, TSubType1, TSubType2>(Action<TSubType1, TSubType2> action) 
            where TSubType1 : TBase1 
            where TSubType2 : TBase2 {
            return typeof(TSubType1) == typeof(TBase1) &&
                   typeof(TSubType2) == typeof(TBase2)
                ? action as Action<TBase1, TBase2>
                : (p1, p2) => {
                    if (p1 is null or TSubType1 &&
                        p2 is null or TSubType2) {
                        action((TSubType1)p1!, (TSubType2)p2!);
                    }
                };
        }

        public static Action<TBase1, TBase2, TBase3>? Convert<TBase1, TBase2, TBase3, TSubType1, TSubType2, TSubType3>(Action<TSubType1, TSubType2, TSubType3> action) 
            where TSubType1 : TBase1 
            where TSubType2 : TBase2
            where TSubType3 : TBase3 {
            return typeof(TSubType1) == typeof(TBase1) &&
                   typeof(TSubType2) == typeof(TBase2) &&
                   typeof(TSubType3) == typeof(TBase3)
                ? action as Action<TBase1, TBase2, TBase3>
                : (p1, p2, p3) => {
                    if (p1 is null or TSubType1 &&
                        p2 is null or TSubType2 &&
                        p3 is null or TSubType3) {
                        action((TSubType1)p1!, (TSubType2)p2!, (TSubType3)p3!);
                    }
                };
        }
        public static Action<TBase1, TBase2, TBase3, TBase4>? Convert<TBase1, TBase2, TBase3, TBase4, TSubType1, TSubType2, TSubType3, TSubType4>(Action<TSubType1, TSubType2, TSubType3, TSubType4> action) 
            where TSubType1 : TBase1 
            where TSubType2 : TBase2
            where TSubType3 : TBase3 
            where TSubType4 : TBase4 {
            return typeof(TSubType1) == typeof(TBase1) &&
                   typeof(TSubType2) == typeof(TBase2) &&
                   typeof(TSubType3) == typeof(TBase3) &&
                   typeof(TSubType4) == typeof(TBase4)
                ? action as Action<TBase1, TBase2, TBase3, TBase4>
                : (p1, p2, p3, p4) => {
                    if (p1 is null or TSubType1 &&
                        p2 is null or TSubType2 &&
                        p3 is null or TSubType3 &&
                        p4 is null or TSubType4) {
                        action((TSubType1)p1!, (TSubType2)p2!, (TSubType3)p3!, (TSubType4)p4!);
                    }
                };
        }
    }
    
