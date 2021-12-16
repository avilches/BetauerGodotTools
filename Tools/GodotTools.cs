using System;
using System.Collections;
using System.Collections.Generic;
using Godot;
using static Godot.Mathf;

namespace Tools {
    public static class GodotExtension {
        public static T FindFirstChild<T>(this Node parent) where T : class {
            var count = parent.GetChildCount();
            for (var i = 0; i < count; i++) {
                if (parent.GetChild(i) is T result) {
                    return result;
                }
            }

            return null;
        }

        public static List<T> GetChildrenFilter<T>(this Node parent) where T : class {
            return Filter<T>(parent.GetChildren());
        }

        public static List<T> Filter<T>(IList nodes) where T : class {
            var children = new List<T>();
            foreach (var node in nodes) {
                if (node is T nodeTyped) children.Add(nodeTyped);
            }
            return children;
        }

        public static void DisableAllNotifications(this Node node) {
            node.SetProcess(false);
            node.SetProcessInput(false);
            node.SetProcessUnhandledInput(false);
            node.SetProcessUnhandledKeyInput(false);
            node.SetPhysicsProcess(false);
        }
    }

    public static class GodotTools {
        public static object SumVariant(object op1, object op2) {
            if (op1 is float fromFloat && op2 is float toFloat)
                return fromFloat + toFloat;

            if (op1 is int fromInt && op2 is int toInt)
                return fromInt + toInt;

            if (op1 is Color fromColor && op2 is Color toColor)
                return fromColor + toColor;

            if (op1 is Vector2 fromVector2 && op2 is Vector2 toVector2)
                return fromVector2 + toVector2;

            if (op1 is Vector3 fromVector3 && op2 is Vector3 toVector3)
                return fromVector3 + toVector3;

            throw new Exception("Sum Variant " + op1.GetType().Name + " + " + op2.GetType().Name + " not implemented");
        }

        /*
         * Missing: Transform3D (there is no C# class for it, why the tween.cpp can handle it?
         */
        public static object LerpVariant(object op1, object op2, float t) {
            switch (op1) {
                case float fromFloat when op2 is float toFloat:
                    return Mathf.Lerp(fromFloat, toFloat, t);

                case int fromInt when op2 is int toInt:
                    return (int)Mathf.Lerp(fromInt, toInt, t);

                case bool fromBool when op2 is bool toBool:
                    return Lerp(fromBool, toBool, t);

                case Vector2 fromVector2 when op2 is Vector2 toVector2:
                    return Lerp(fromVector2, toVector2, t);

                case Rect2 fromRect2 when op2 is Rect2 toRect2:
                    return Lerp(fromRect2, toRect2, t);

                case Transform2D fromTransform2D when op2 is Transform2D toTransform2D:
                    return Lerp(fromTransform2D, toTransform2D, t);

                case Vector3 fromVector3 when op2 is Vector3 toVector3:
                    return Lerp(fromVector3, toVector3, t);

                case Color fromColor when op2 is Color toColor:
                    return Lerp(fromColor, toColor, t);

                case AABB fromAabb when op2 is AABB toAabb:
                    return Lerp(fromAabb, toAabb, t);

                case Basis fromBasis when op2 is Basis toBasis:
                    return Lerp(fromBasis, toBasis, t);
                default:
                    throw new Exception("Lerp from " + op1.GetType().Name + " to " + op2.GetType().Name +
                                        " not implemented");
            }
        }

        public static bool Lerp(bool from, bool to, float t) {
            // TODO: is this correct? not tested. Make a test comparing Tween node and this
            return Mathf.Lerp(from ? 1 : 0, to ? 1 : 0, t) > 0.5;
        }

        public static Vector2 Lerp(Vector2 from, Vector2 to, float t) {
            /*
			APPLY_EQUATION(x);
			APPLY_EQUATION(y);
             */
            return new Vector2(
                Mathf.Lerp(from.x, to.y, t),
                Mathf.Lerp(from.y, to.y, t));
        }

        private static Rect2 Lerp(Rect2 fromRect2, Rect2 toRect2, float t) {
            /*
			APPLY_EQUATION(position.x);
			APPLY_EQUATION(position.y);
			APPLY_EQUATION(size.x);
			APPLY_EQUATION(size.y);
             */
            // TODO: is this correct? not tested. Make a test comparing Tween node and this
            return new Rect2(
                Lerp(fromRect2.Position, toRect2.Position, t),
                Lerp(fromRect2.Size, toRect2.Size, t));
        }

        public static Transform2D Lerp(Transform2D from, Transform2D to, float t) {
            /*
			APPLY_EQUATION(elements[0][0]);
			APPLY_EQUATION(elements[0][1]);
			APPLY_EQUATION(elements[1][0]);
			APPLY_EQUATION(elements[1][1]);
			APPLY_EQUATION(elements[2][0]);
			APPLY_EQUATION(elements[2][1]);
             */
            // TODO: is this correct? not tested. Make a test comparing Tween node and this
            return new Transform2D(
                Lerp(from.x, to.x, t),
                Lerp(from.y, to.y, t),
                Lerp(from.origin, to.origin, t)
            );
        }

        public static Vector3 Lerp(Vector3 from, Vector3 to, float t) {
            /*
			APPLY_EQUATION(x);
			APPLY_EQUATION(y);
			APPLY_EQUATION(z);
             */
            // TODO: is this correct? not tested. Make a test comparing Tween node and this
            return new Vector3(
                Mathf.Lerp(from.x, to.y, t),
                Mathf.Lerp(from.y, to.y, t),
                Mathf.Lerp(from.z, to.z, t));
        }

        public static Color Lerp(Color from, Color to, float t) {
            /*
            APPLY_EQUATION(r);
            APPLY_EQUATION(g);
            APPLY_EQUATION(b);
            APPLY_EQUATION(a);
             */
            // TODO: is this correct? not tested. Make a test comparing Tween node and this
            return new Color(
                Mathf.Lerp(from.r, to.r, t),
                Mathf.Lerp(from.g, to.g, t),
                Mathf.Lerp(from.b, to.b, t),
                Mathf.Lerp(from.a, to.a, t));
        }

        public static AABB Lerp(AABB from, AABB to, float t) {
            /*
            APPLY_EQUATION(position.x);
			APPLY_EQUATION(position.y);
			APPLY_EQUATION(position.z);
			APPLY_EQUATION(size.x);
			APPLY_EQUATION(size.y);
			APPLY_EQUATION(size.z);
             */
            // TODO: is this correct? not tested. Make a test comparing Tween node and this
            return new AABB(
                Lerp(from.Position, to.Position, t),
                Lerp(from.Size, to.Size, t));
        }

        public static Basis Lerp(Basis from, Basis to, float t) {
            /*
                APPLY_EQUATION(elements[0][0]);
                APPLY_EQUATION(elements[0][1]);
                APPLY_EQUATION(elements[0][2]);
                APPLY_EQUATION(elements[1][0]);
                APPLY_EQUATION(elements[1][1]);
                APPLY_EQUATION(elements[1][2]);
                APPLY_EQUATION(elements[2][0]);
                APPLY_EQUATION(elements[2][1]);
                APPLY_EQUATION(elements[2][2]);

             */
            // TODO: is this correct? not tested. Make a test comparing Tween node and this
            return new Basis(
                Lerp(from.Column0, to.Column0, t),
                Lerp(from.Column1, to.Column1, t),
                Lerp(from.Column2, to.Column2, t));
        }

        public static Quat Lerp(Quat from, Quat to, float t) {
            /*
            APPLY_EQUATION(x);
			APPLY_EQUATION(y);
			APPLY_EQUATION(z);
			APPLY_EQUATION(w);
             */
            // TODO: is this correct? not tested. Make a test comparing Tween node and this
            return new Quat(
                Mathf.Lerp(from.x, to.x, t),
                Mathf.Lerp(from.y, to.y, t),
                Mathf.Lerp(from.z, to.z, t),
                Mathf.Lerp(from.w, to.w, t));
        }

        /*
            APPLY_EQUATION(basis.elements[0][0]);
			APPLY_EQUATION(basis.elements[0][1]);
			APPLY_EQUATION(basis.elements[0][2]);
			APPLY_EQUATION(basis.elements[1][0]);
			APPLY_EQUATION(basis.elements[1][1]);
			APPLY_EQUATION(basis.elements[1][2]);
			APPLY_EQUATION(basis.elements[2][0]);
			APPLY_EQUATION(basis.elements[2][1]);
			APPLY_EQUATION(basis.elements[2][2]);
			APPLY_EQUATION(origin.x);
			APPLY_EQUATION(origin.y);
			APPLY_EQUATION(origin.z);
         */
        // missing Transform3D Lerp
    }

    public static class AnimationTools {
        /*
         * Alinea las plataformas como si fueran una aguja de un reloj y la gira. La primera primera plataforma
         * mantiene su posicion y las demás se van espaciando hasta llegar al radius
         */
        public static void RotateAligned(List<PhysicsBody2D> nodes, float angle, float radius,
            float initialOffset = 20) {
            var count = nodes.Count;
            var spacing = radius / count;
            for (var i = 0; i < count; i++) {
                float offset = ((spacing * i) + initialOffset);
                var newX = Sin(angle) * offset;
                var newY = Cos(angle) * offset;
                var newPos = new Vector2(newX, newY);
                nodes[i].Position = newPos;
            }
        }

        /*
         * Distribuye por el circulo de manera espaciado y las gira
         */
        public static void RotateSpaced(List<PhysicsBody2D> nodes, float angle, Vector2 radius) {
            var count = nodes.Count;
            var spacing = Tau / count;
            for (var i = 0; i < count; i++) {
                var newX = Sin(spacing * i + angle) * radius.x;
                var newY = Cos(spacing * i + angle) * radius.y;
                var newPos = new Vector2(newX, newY);
                nodes[i].Position = newPos;
            }
        }
    }
}