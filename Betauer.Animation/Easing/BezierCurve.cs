using Godot;

namespace Betauer.Animation.Easing {
    // Got from https://www.icode.com/c-function-for-a-bezier-curve/
    public class BezierCurve : IEasing {
        public readonly float p0x;
        public readonly float p0y;
        public readonly float cx;
        public readonly float cy;
        public readonly float bx;
        public readonly float by;
        public readonly float ax;
        public readonly float ay;
        public readonly Curve2D curve;

        public string Name { get; }

        public BezierCurve(string name,
            float p0x, float p0y, float cx, float cy, float bx, float by, float ax, float ay, Curve2D curve) {
            Name = name;
            this.p0x = p0x;
            this.p0y = p0y;
            this.cx = cx;
            this.cy = cy;
            this.bx = bx;
            this.by = by;
            this.ax = ax;
            this.ay = ay;
            this.curve = curve;
        }

        public float GetY(float t) {
            return ay * t * t * t + by * t * t + cy * t + p0y;
        }

        public Vector2 GetPoint(float t) {
            var cube = t * t * t;
            var square = t * t;
            var resX = (ax * cube) + (bx * square) + (cx * t) + p0x;
            var resY = (ay * cube) + (by * square) + (cy * t) + p0y;
            return new Vector2(resX, resY);
        }

        public override string ToString() {
            return Name;
        }

        public bool Equals(BezierCurve other) {
            return p0x.Equals(other.p0x) && p0y.Equals(other.p0y) && 
                   cx.Equals(other.cx) && cy.Equals(other.cy) && 
                   bx.Equals(other.bx) && by.Equals(other.by) && 
                   ax.Equals(other.ax) && ay.Equals(other.ay);
        }

        public override bool Equals(object? obj) {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals((BezierCurve)obj);
        }

        public override int GetHashCode() {
            unchecked {
                var hashCode = p0x.GetHashCode();
                hashCode = (hashCode * 397) ^ p0y.GetHashCode();
                hashCode = (hashCode * 397) ^ cx.GetHashCode();
                hashCode = (hashCode * 397) ^ cy.GetHashCode();
                hashCode = (hashCode * 397) ^ bx.GetHashCode();
                hashCode = (hashCode * 397) ^ by.GetHashCode();
                hashCode = (hashCode * 397) ^ ax.GetHashCode();
                hashCode = (hashCode * 397) ^ ay.GetHashCode();
                return hashCode;
            }
        }


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
            var name = $"Bezier({p0x},{p0y}) ({p1x},{p1y}) ({p2x},{p2y}) ({p3x},{p3y})";
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

            return new BezierCurve(name, p0x, p0y, cx, cy, bx, by, ax, ay, null);
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