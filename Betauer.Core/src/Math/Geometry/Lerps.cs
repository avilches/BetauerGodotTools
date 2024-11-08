using System;
using Godot;

namespace Betauer.Core.Math.Geometry;

public static partial class Lerps {

    public static Variant LerpVariant(Variant op1, Variant op2, float t) {
        if (op1.VariantType != op2.VariantType) throw new Exception($"LerpVariant: types are not equal: {op1.VariantType}  {op2.VariantType}");
        if (op1.VariantType == Variant.Type.Bool) return Lerp(op1.AsBool(), op2.AsBool(), t);
        if (op1.VariantType == Variant.Type.Int) return Lerp(op1.AsInt64(), op2.AsInt64(), t);
        if (op1.VariantType == Variant.Type.Float) return Lerp(op1.AsDouble(), op2.AsDouble(), t);
        if (op1.VariantType == Variant.Type.Vector2) return op1.AsVector2().Lerp(op2.AsVector2(), t);
        if (op1.VariantType == Variant.Type.Vector2I) return op1.AsVector2I().Lerp(op2.AsVector2I(), t);
        if (op1.VariantType == Variant.Type.Rect2) return op1.AsRect2().Lerp(op2.AsRect2(), t);
        if (op1.VariantType == Variant.Type.Rect2I) return op1.AsRect2I().Lerp(op2.AsRect2I(), t);
        if (op1.VariantType == Variant.Type.Vector3) return op1.AsVector3().Lerp(op2.AsVector3(), t);
        if (op1.VariantType == Variant.Type.Vector3I) return op1.AsVector3I().Lerp(op2.AsVector3I(), t);
        if (op1.VariantType == Variant.Type.Transform2D) return op1.AsTransform2D().InterpolateWith(op2.AsTransform2D(), t);
        if (op1.VariantType == Variant.Type.Vector4) return op1.AsVector4().Lerp(op2.AsVector4(), t);
        if (op1.VariantType == Variant.Type.Vector4I) return op1.AsVector4I().Lerp(op2.AsVector4I(), t);
        if (op1.VariantType == Variant.Type.Plane) return op1.AsPlane().Lerp(op2.AsPlane(), t);
        if (op1.VariantType == Variant.Type.Quaternion) return op1.AsQuaternion().Slerp(op2.AsQuaternion(), t);
        if (op1.VariantType == Variant.Type.Aabb) return op1.AsAabb().Lerp(op2.AsAabb(), t);
        if (op1.VariantType == Variant.Type.Basis) return op1.AsBasis().Lerp(op2.AsBasis(), t);
        if (op1.VariantType == Variant.Type.Transform3D) return op1.AsTransform3D().InterpolateWith(op2.AsTransform3D(), t);
        if (op1.VariantType == Variant.Type.Color) return op1.AsColor().Lerp(op2.AsColor(), t);
        throw new Exception($"LerpVariant: {op1.VariantType} type not implemented");
    }

    public static Variant LerpVariant<[MustBeVariant] T>(T op1, T op2, float t) {
        return op1 switch {
            bool fromBool when op2 is bool toBool => Lerp(fromBool, toBool, t),
            char fromChar when op2 is char toChar => Mathf.Lerp(fromChar, toChar, t),
            sbyte fromSbyte when op2 is sbyte toSbyte => Mathf.Lerp(fromSbyte, toSbyte, t),
            short fromShort when op2 is short toShort => Mathf.Lerp(fromShort, toShort, t),
            int fromInt when op2 is int toInt => Mathf.Lerp(fromInt, toInt, t),
            long fromLong when op2 is long toLong => Mathf.Lerp(fromLong, toLong, t),
            byte fromByte when op2 is byte toByte => Mathf.Lerp(fromByte, toByte, t),
            ushort fromUshort when op2 is ushort toUshort => Mathf.Lerp(fromUshort, toUshort, t),
            uint fromUint when op2 is uint toUint => Mathf.Lerp(fromUint, toUint, t),
            ulong fromUlong when op2 is ulong toUlong => Mathf.Lerp(fromUlong, toUlong, t),
            float fromFloat when op2 is float toFloat => Mathf.Lerp(fromFloat, toFloat, t),
            double fromDouble when op2 is double toDouble => Lerp(fromDouble, toDouble, t),
            Vector2 fromVector2 when op2 is Vector2 toVector2 => fromVector2.Lerp(toVector2, t),
            Vector2I fromVector2I when op2 is Vector2I toVector2I => fromVector2I.Lerp(toVector2I, t),
            Rect2 fromRect2 when op2 is Rect2 toRect2 => fromRect2.Lerp(toRect2, t),
            Rect2I fromRect2I when op2 is Rect2I toRect2I => fromRect2I.Lerp(toRect2I, t),
            Transform2D fromTransform2D when op2 is Transform2D toTransform2D => fromTransform2D.InterpolateWith(toTransform2D, t),
            Vector3 fromVector3 when op2 is Vector3 toVector3 => fromVector3.Lerp(toVector3, t),
            Vector3I fromVector3I when op2 is Vector3I toVector3I => fromVector3I.Lerp(toVector3I, t),
            Basis fromBasis when op2 is Basis toBasis => fromBasis.Lerp(toBasis, t),
            Quaternion fromQuaternion when op2 is Quaternion toQuaternion => fromQuaternion.Slerp(toQuaternion, t),
            Transform3D fromTransform3D when op2 is Transform3D toTransform3D => fromTransform3D.InterpolateWith(toTransform3D, t),
            Vector4 fromVector4 when op2 is Vector4 toVector4 => fromVector4.Lerp(toVector4, t),
            Vector4I fromVector4I when op2 is Vector4I toVector4I => fromVector4I.Lerp(toVector4I, t),
            Aabb fromAabb when op2 is Aabb toAabb => fromAabb.Lerp(toAabb, t),
            Color fromColor when op2 is Color toColor => fromColor.Lerp(toColor, t),
            Plane fromPlane when op2 is Plane toPlane => fromPlane.Lerp(toPlane, t),
            _ => throw new Exception($"LerpVariant<T>: {op1?.GetType().Name} type not implemented")
        };
    }

    public static bool Lerp(this bool from, bool to, float t) =>
        Mathf.Lerp(from ? 1 : 0, to ? 1 : 0, t) > 0.5;

    public static double Lerp(this double from, double to, float weight) =>
        from + (to - from) * weight;

    public static Vector2I Lerp(this Vector2I op1, Vector2I op2, float t) =>
        new((int)Mathf.Lerp(op1.X, op2.X, t),
            (int)Mathf.Lerp(op1.Y, op2.Y, t));

    public static Vector3I Lerp(this Vector3I op1, Vector3I op2, float t) =>
        new((int)Mathf.Lerp(op1.X, op2.X, t),
            (int)Mathf.Lerp(op1.Y, op2.Y, t),
            (int)Mathf.Lerp(op1.Z, op2.Z, t));

    public static Vector4I Lerp(this Vector4I op1, Vector4I op2, float t) =>
        new((int)Mathf.Lerp(op1.X, op2.X, t),
            (int)Mathf.Lerp(op1.Y, op2.Y, t),
            (int)Mathf.Lerp(op1.Z, op2.Z, t),
            (int)Mathf.Lerp(op1.W, op2.W, t));

    public static Rect2 Lerp(this Rect2 from, Rect2 to, float t) =>
        new(from.Position.Lerp(to.Position, t),
            from.Size.Lerp(to.Size, t));

    public static Basis Lerp(this Basis from, Basis to, float t) =>
        new(from.Row0.Lerp(to.Row0, t),
            from.Row1.Lerp(to.Row1, t),
            from.Row2.Lerp(to.Row2, t));

    public static Rect2I Lerp(this Rect2I from, Rect2I to, float t) =>
        new(from.Position.Lerp(to.Position, t),
            from.Size.Lerp(to.Size, t));

    public static Aabb Lerp(this Aabb from, Aabb to, float t) =>
        new(from.Position.Lerp(to.Position, t),
            from.Size.Lerp(to.Size, t));

    public static Plane Lerp(this Plane from, Plane to, float t) =>
        new(from.Normal.Lerp(to.Normal, t),
            Mathf.Lerp(from.D, to.D, t));
}