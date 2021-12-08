using Godot;

namespace Tools.Animation {
    // Got from https://www.icode.com/c-function-for-a-bezier-curve/
    public class BezierCurve {
        float p0x;
        float p0y;
        float cx;
        float cy;
        float bx;
        float by;
        float ax;
        float ay;

        private BezierCurve(float p0x, float p0y, float cx, float cy, float bx, float by, float ax, float ay) {
            this.p0x = p0x;
            this.p0y = p0y;
            this.cx = cx;
            this.cy = cy;
            this.bx = bx;
            this.by = by;
            this.ax = ax;
            this.ay = ay;
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

        public static BezierCurve Create(float p0x, float p0y, float p1x, float p1y, float p2x, float p2y, float p3x, float p3y) {
            var cx = 3 * (p1x - p0x);
            var cy = 3 * (p1y - p0y);
            var bx = 3 * (p2x - p1x) - cx;
            var by = 3 * (p2y - p1y) - cy;
            var ax = p3x - p0x - cx - bx;

            var ay = p3y - p0y - cy - by;
            return new BezierCurve(p0x, p0y, cx, cy, bx, by, ax, ay);
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