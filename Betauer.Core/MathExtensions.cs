namespace Betauer {
    public static class MathExtensions {
        public static int Mod(this int x, int mod) {
            var remainder = x % mod;
            return remainder < 0 ? remainder + mod : remainder;
        }
    }
}