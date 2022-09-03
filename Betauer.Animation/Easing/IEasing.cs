namespace Betauer.Animation.Easing {
    public interface IEasing {
        public string Name { get; }
        /// <summary>
        /// t is a number between 0 and 1
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        public float GetY(float t);
    }
}