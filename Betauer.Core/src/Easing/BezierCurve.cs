using Godot;

namespace Betauer.Core.Easing {
    // Got from https://www.icode.com/c-function-for-a-bezier-curve/

    /*
    // https://godotengine.org/qa/80633/using-curve2d-to-draw-a-bezier-curve
    // https://www.reddit.com/r/godot/comments/igkbjv/using_curve2d_to_draw_a_bezier_curve/
    // https://www.khanacademy.org/science/physics/two-dimensional-motion/two-dimensional-projectile-mot/a/what-is-2d-projectile-motion

    var p0_in = Vector2.Zero; // This isn't used for the first curve
    var p0_vertex = new Vector2(p0x, p0y); // First point of first line segment
    var p0_out = new Vector2(p1x, p1y); // Second point of first line segment
    var p1_in = new Vector2(p2x, p2y); // First point of second line segment
    var p1_vertex = new Vector2(p3x, p3y); // Second point of second line segment
    var p1_out = Vector2.One; // Not used unless another curve is added
    var curve = new Curve2D();
    curve.AddPoint(p0_vertex, p0_in, p0_out-p0_vertex);
    curve.AddPoint(p1_vertex, p1_in-p1_vertex, p1_out);
    */

    // Slower version using Godot LinearInterpolate function only for learning purposes
    // public static float CubicBezier(Vector2 p0, Vector2 p1, Vector2 p2, Vector2 p3, float t) {
    //     var q0 = p0.LinearInterpolate(p1, t);
    //     var q1 = p1.LinearInterpolate(p2, t);
    //     var q2 = p2.LinearInterpolate(p3, t);
    //     var r0 = q0.LinearInterpolate(q1, t);
    //     var r1 = q1.LinearInterpolate(q2, t);
    //
    //     var s = r0.LinearInterpolate(r1, t);
    //     return s.Y;
    // }


    public readonly struct BezierCurve : IEasing {
        public readonly float P0X;
        public readonly float P0Y;
        
        public readonly float Cx;
        public readonly float Cy;
        public readonly float Bx;
        public readonly float By;
        public readonly float Ax;
        public readonly float Ay;

        public static BezierCurve Create(Vector2 p1, Vector2 p2) {
            return Create(0, 0, p1.X, p1.Y, p2.X, p2.Y, 1, 1);
        }

        public static BezierCurve Create(Vector2 p0, Vector2 p1, Vector2 p2, Vector2 p3) {
            return Create(p0.X, p0.Y, p1.X, p1.Y, p2.X, p2.Y, p3.X, p3.Y);
        }

        public static BezierCurve Create(float p1X, float p1Y, float p2X, float p2Y) {
            return Create(0, 0, p1X, p1Y, p2X, p2Y, 1, 1);
        }

        public static BezierCurve Create(float p0X, float p0Y, 
            float p1X, float p1Y, float p2X, float p2Y, float p3X, float p3Y) {
            var cx = 3 * (p1X - p0X);
            var cy = 3 * (p1Y - p0Y);
            var bx = 3 * (p2X - p1X) - cx;
            var by = 3 * (p2Y - p1Y) - cy;
            var ax = p3X - p0X - cx - bx;
            var ay = p3Y - p0Y - cy - by;
            return new BezierCurve(p0X, p0Y, cx, cy, bx, by, ax, ay);
        }

        private BezierCurve(float p0X, float p0Y, float cx, float cy, float bx, float by, float ax, float ay) {
            P0X = p0X;
            P0Y = p0Y;
            Cx = cx;
            Cy = cy;
            Bx = bx;
            By = by;
            Ax = ax;
            Ay = ay;
        }

        public float GetY(float t) {
            var square = t * t;
            var cube = square * t;
            return Ay * cube + By * square + Cy * t + P0Y;
        }

        public Vector2 GetPoint(float t) {
            var square = t * t;
            var cube = square * t;
            var resX = (Ax * cube) + (Bx * square) + (Cx * t) + P0X;
            var resY = (Ay * cube) + (By * square) + (Cy * t) + P0Y;
            return new Vector2(resX, resY);
        }

        public bool Equals(BezierCurve other) {
            return P0X.Equals(other.P0X) && 
                   P0Y.Equals(other.P0Y) && 
                   Cx.Equals(other.Cx) && 
                   Cy.Equals(other.Cy) && 
                   Bx.Equals(other.Bx) && 
                   By.Equals(other.By) && 
                   Ax.Equals(other.Ax) && 
                   Ay.Equals(other.Ay);
        }

        public override bool Equals(object? obj) {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals((BezierCurve)obj);
        }

        public override int GetHashCode() {
            unchecked {
                var hashCode = P0X.GetHashCode();
                hashCode = (hashCode * 397) ^ P0Y.GetHashCode();
                hashCode = (hashCode * 397) ^ Cx.GetHashCode();
                hashCode = (hashCode * 397) ^ Cy.GetHashCode();
                hashCode = (hashCode * 397) ^ Bx.GetHashCode();
                hashCode = (hashCode * 397) ^ By.GetHashCode();
                hashCode = (hashCode * 397) ^ Ax.GetHashCode();
                hashCode = (hashCode * 397) ^ Ay.GetHashCode();
                return hashCode;
            }
        }
    }
}