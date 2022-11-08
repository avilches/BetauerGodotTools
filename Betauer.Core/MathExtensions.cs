namespace Betauer.Core {
    public static class MathExtensions {
        /// <summary>
        /// Calculate a modulo operation, allowing negative numbers. The % operator in C# is not the
        /// modulo operator but the remainder operator.
        /// So:
        /// 5.Mod(2) == 1
        /// -5.Mod(2) == 1
        ///
        /// More info: https://stackoverflow.com/questions/11720656/modulo-operation-with-negative-numbers
        ///  
        /// </summary>
        /// <param name="x"></param>
        /// <param name="mod"></param>
        /// <returns></returns>
        public static int Mod(this int x, int mod) {
            var remainder = x % mod;
            return remainder < 0 ? remainder + mod : remainder;
        }
    }
}