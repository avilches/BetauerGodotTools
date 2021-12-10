using Godot;

namespace Tools.Animation {
    public abstract class Easing {
        public string Name { get; }
        public static Easing LinearIn = new GodotEasing(Tween.TransitionType.Linear, Tween.EaseType.In);
        public static Easing LinearOut = new GodotEasing(Tween.TransitionType.Linear, Tween.EaseType.Out);
        public static Easing LinearInOut = new GodotEasing(Tween.TransitionType.Linear, Tween.EaseType.InOut);

        public static Easing SineIn = new GodotEasing(Tween.TransitionType.Sine, Tween.EaseType.In);
        public static Easing SineOut = new GodotEasing(Tween.TransitionType.Sine, Tween.EaseType.Out);
        public static Easing SineInOut = new GodotEasing(Tween.TransitionType.Sine, Tween.EaseType.InOut);

        public static Easing QuintIn = new GodotEasing(Tween.TransitionType.Quint, Tween.EaseType.In);
        public static Easing QuintOut = new GodotEasing(Tween.TransitionType.Quint, Tween.EaseType.Out);
        public static Easing QuintInOut = new GodotEasing(Tween.TransitionType.Quint, Tween.EaseType.InOut);

        public static Easing QuartIn = new GodotEasing(Tween.TransitionType.Quart, Tween.EaseType.In);
        public static Easing QuartOut = new GodotEasing(Tween.TransitionType.Quart, Tween.EaseType.Out);
        public static Easing QuartInOut = new GodotEasing(Tween.TransitionType.Quart, Tween.EaseType.InOut);

        public static Easing QuadIn = new GodotEasing(Tween.TransitionType.Quad, Tween.EaseType.In);
        public static Easing QuadOut = new GodotEasing(Tween.TransitionType.Quad, Tween.EaseType.Out);
        public static Easing QuadInOut = new GodotEasing(Tween.TransitionType.Quad, Tween.EaseType.InOut);

        public static Easing ExpoIn = new GodotEasing(Tween.TransitionType.Expo, Tween.EaseType.In);
        public static Easing ExpoOut = new GodotEasing(Tween.TransitionType.Expo, Tween.EaseType.Out);
        public static Easing ExpoInOut = new GodotEasing(Tween.TransitionType.Expo, Tween.EaseType.InOut);

        public static Easing ElasticIn = new GodotEasing(Tween.TransitionType.Elastic, Tween.EaseType.In);
        public static Easing ElasticOut = new GodotEasing(Tween.TransitionType.Elastic, Tween.EaseType.Out);
        public static Easing ElasticInOut = new GodotEasing(Tween.TransitionType.Elastic, Tween.EaseType.InOut);

        public static Easing CubicIn = new GodotEasing(Tween.TransitionType.Cubic, Tween.EaseType.In);
        public static Easing CubicOut = new GodotEasing(Tween.TransitionType.Cubic, Tween.EaseType.Out);
        public static Easing CubicInOut = new GodotEasing(Tween.TransitionType.Cubic, Tween.EaseType.InOut);

        public static Easing CircIn = new GodotEasing(Tween.TransitionType.Circ, Tween.EaseType.In);
        public static Easing CircOut = new GodotEasing(Tween.TransitionType.Circ, Tween.EaseType.Out);
        public static Easing CircInOut = new GodotEasing(Tween.TransitionType.Circ, Tween.EaseType.InOut);

        public static Easing BounceIn = new GodotEasing(Tween.TransitionType.Bounce, Tween.EaseType.In);
        public static Easing BounceOut = new GodotEasing(Tween.TransitionType.Bounce, Tween.EaseType.Out);
        public static Easing BounceInOut = new GodotEasing(Tween.TransitionType.Bounce, Tween.EaseType.InOut);

        public static Easing BackIn = new GodotEasing(Tween.TransitionType.Back, Tween.EaseType.In);
        public static Easing BackOut = new GodotEasing(Tween.TransitionType.Back, Tween.EaseType.Out);
        public static Easing BackInOut = new GodotEasing(Tween.TransitionType.Back, Tween.EaseType.InOut);

        protected Easing(string name) {
            Name = name;
        }
    }

    public class GodotEasing : Easing {
        public readonly Tween.EaseType EaseType;
        public readonly Tween.TransitionType TransitionType;

        internal GodotEasing(Tween.TransitionType transitionType, Tween.EaseType easeType) :
            base($"{transitionType}{easeType}") {
            TransitionType = transitionType;
            EaseType = easeType;
        }
    }

    // Got from https://www.icode.com/c-function-for-a-bezier-curve/
    public class BezierCurve : Easing {
        public readonly float p0x;
        public readonly float p0y;
        public readonly float cx;
        public readonly float cy;
        public readonly float bx;
        public readonly float by;
        public readonly float ax;
        public readonly float ay;
        public readonly string Name;

        public BezierCurve(string name, float p0x, float p0y, float cx, float cy, float bx, float by, float ax,
            float ay) : base(name){
            Name = name;
            this.p0x = p0x;
            this.p0y = p0y;
            this.cx = cx;
            this.cy = cy;
            this.bx = bx;
            this.by = by;
            this.ax = ax;
            this.ay = ay;
        }

        public override string ToString() {
            return Name;
        }

        public Vector2 GetPoint(float t) {
            var cube = t * t * t;
            var square = t * t;
            var resX = (ax * cube) + (bx * square) + (cx * t) + p0x;
            var resY = (ay * cube) + (by * square) + (cy * t) + p0y;
            return new Vector2(resX, resY);
        }

        public float GetY(float t) => ay * t * t * t + by * t * t + cy * t + p0y;

        public static BezierCurve Create(Vector2 p1, Vector2 p2) {
            return Create(0, 0, p1.x, p1.y, p2.x, p2.y, 1, 1);
        }

        public static BezierCurve Create(Vector2 p0, Vector2 p1, Vector2 p2, Vector2 p3) {
            return Create(p0.x, p0.y, p1.x, p1.y, p2.x, p2.y, p3.x, p3.y);
        }

        public static BezierCurve Create(float p1x, float p1y, float p2x, float p2y) {
            return Create(0, 0, p1x, p1y, p2x, p2y, 1, 1);
        }

        public static BezierCurve Create(
            float p0x, float p0y, float p1x, float p1y,
            float p2x, float p2y, float p3x, float p3y) {
            var cx = 3 * (p1x - p0x);
            var cy = 3 * (p1y - p0y);
            var bx = 3 * (p2x - p1x) - cx;
            var by = 3 * (p2y - p1y) - cy;
            var ax = p3x - p0x - cx - bx;
            var ay = p3y - p0y - cy - by;
            var name = $"({p0x},{p0y}) ({p1x},{p1y}) ({p2x},{p2y}) ({p3x},{p3y})";
            return new BezierCurve(name, p0x, p0y, cx, cy, bx, by, ax, ay);
        }

        // Slower version using Godot LinearInterpolate function.
        // public static float CubicBezier(Vector2 p0, Vector2 p1, Vector2 p2, Vector2 p3, float t) {
        //     var q0 = p0.LinearInterpolate(p1, t);
        //     var q1 = p1.LinearInterpolate(p2, t);
        //     var q2 = p2.LinearInterpolate(p3, t);
        //     var r0 = q0.LinearInterpolate(q1, t);
        //     var r1 = q1.LinearInterpolate(q2, t);
        //
        //     var s = r0.LinearInterpolate(r1, t);
        //     return s.y;
        // }
    }
}