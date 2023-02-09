using System;
using Godot;
using Godot.Collections;
using Object = Godot.Object;
using Array = Godot.Collections.Array;

namespace Betauer.Core; 

public static class VariantHelper {
    public static Variant CreateFrom<T>(T value) {
        if (typeof(T) != typeof(object)) return Variant.From(value);
        return value switch {
            bool valueBool => valueBool,
            char valueChar => valueChar,
            sbyte valueSbyte => valueSbyte,
            short valueShort => valueShort,
            int valueInt => valueInt,
            long valueLong => valueLong,
            byte valueByte => valueByte,
            ushort valueUshort => valueUshort,
            uint valueUint => valueUint,
            ulong valueUlong => valueUlong,
            float valueFloat => valueFloat,
            double valueDouble => valueDouble,
            string valueString => valueString,
            Vector2 valueVector2 => valueVector2,
            Vector2i valueVector2i => valueVector2i,
            Rect2 valueRect2 => valueRect2,
            Rect2i valueRect2i => valueRect2i,
            Transform2D valueTransform2D => valueTransform2D,
            Vector3 valueVector3 => valueVector3,
            Vector3i valueVector3i => valueVector3i,
            Vector4 valueVector4 => valueVector4,
            Vector4i valueVector4i => valueVector4i,
            Basis valueBasis => valueBasis,
            Quaternion valueQuaternion => valueQuaternion,
            Transform3D valueTransform3D => valueTransform3D,
            Projection valueProjection => valueProjection,
            AABB valueAABB => valueAABB,
            Color valueColor => valueColor,
            Plane valuePlane => valuePlane,
            Callable valueCallable => valueCallable,
            Godot.Signal valueSignal => valueSignal,
            byte[] valueArrayByte => valueArrayByte.AsSpan(),
            int[] valueArrayInt => valueArrayInt.AsSpan(),
            long[] valueArrayLong => valueArrayLong.AsSpan(),
            float[] valueArrayFloat => valueArrayFloat.AsSpan(),
            double[] valueArrayDouble => valueArrayDouble.AsSpan(),
            string[] valueArrayString => valueArrayString.AsSpan(),
            Vector2[] valueArrayVector2 => valueArrayVector2.AsSpan(),
            Vector3[] valueArrayVector3 => valueArrayVector3.AsSpan(),
            Color[] valueArrayColor => valueArrayColor.AsSpan(),
            Object[] valueObject => valueObject,
            StringName[] valueArrayStringName => valueArrayStringName.AsSpan(),
            NodePath[] valueArrayNodePath => valueArrayNodePath.AsSpan(),
            RID[] valueArrayRid => valueArrayRid.AsSpan(),
            Object valueObject => valueObject,
            StringName valueStringName => valueStringName,
            NodePath valueNodePath => valueNodePath,
            RID valueRid => valueRid,
            Dictionary valueDictionary => valueDictionary,
            Array valueArray => valueArray,
            _ => throw new Exception("CreateFrom<T>: Unknown variant for type: "+typeof(T).Name)
        };
    }

    public static T ConvertTo<[MustBeVariant] T>(Variant value) {
        return typeof(T) == typeof(object) ? (T)ConvertTo(value) : value.As<T>();
    }

    public static object? ConvertTo(Variant op1) {
        return op1.VariantType switch {
            Variant.Type.Nil => null,
            Variant.Type.Bool => op1.AsBool(),
            Variant.Type.Int => op1.AsInt64(),
            Variant.Type.Float => op1.AsDouble(),
            Variant.Type.String => op1.AsString(),
            Variant.Type.Vector2 => op1.AsVector2(),
            Variant.Type.Vector2i => op1.AsVector2i(),
            Variant.Type.Rect2 => op1.AsRect2(),
            Variant.Type.Rect2i => op1.AsRect2i(),
            Variant.Type.Vector3 => op1.AsVector3(),
            Variant.Type.Vector3i => op1.AsVector3i(),
            Variant.Type.Transform2d => op1.AsTransform2D(),
            Variant.Type.Vector4 => op1.AsVector4(),
            Variant.Type.Vector4i => op1.AsVector4i(),
            Variant.Type.Plane => op1.AsPlane(),
            Variant.Type.Quaternion => op1.AsQuaternion(),
            Variant.Type.Aabb => op1.AsAABB(),
            Variant.Type.Basis => op1.AsBasis(),
            Variant.Type.Transform3d => op1.AsTransform3D(),
            Variant.Type.Projection => op1.AsProjection(),
            Variant.Type.Color => op1.AsColor(),
            Variant.Type.StringName => op1.AsStringName(),
            Variant.Type.NodePath => op1.AsNodePath(),
            Variant.Type.Rid => op1.AsRID(),
            Variant.Type.Object => op1.AsGodotObject(),
            Variant.Type.Callable => op1.AsCallable(),
            Variant.Type.Signal => op1.AsSignal(),
            Variant.Type.Dictionary => op1.AsGodotDictionary(),
            Variant.Type.Array => op1.AsGodotArray(),
            Variant.Type.PackedByteArray => op1.AsByteArray(),
            Variant.Type.PackedInt32Array => op1.AsInt32Array(),
            Variant.Type.PackedInt64Array => op1.AsInt64Array(),
            Variant.Type.PackedFloat32Array => op1.AsFloat32Array(),
            Variant.Type.PackedFloat64Array => op1.AsFloat64Array(),
            Variant.Type.PackedStringArray => op1.AsStringArray(),
            Variant.Type.PackedVector2Array => op1.AsVector2Array(),
            Variant.Type.PackedVector3Array => op1.AsVector3Array(),
            Variant.Type.PackedColorArray => op1.AsColorArray(),
            _ => throw new Exception("ConvertTo: Unknown variant type: "+op1.VariantType)
        };
    }

    public static Variant Add<[MustBeVariant] T>(T op1, T op2) {
        return op1 switch {
            char fromChar when op2 is char toChar => (fromChar + toChar),
            sbyte fromSbyte when op2 is sbyte toSbyte => (fromSbyte + toSbyte),
            short fromShort when op2 is short toShort => (fromShort + toShort),
            int fromInt when op2 is int toInt => (fromInt + toInt),
            long fromLong when op2 is long toLong => (fromLong + toLong),
            byte fromByte when op2 is byte toByte => (fromByte + toByte),
            ushort fromUshort when op2 is ushort toUshort => (fromUshort + toUshort),
            uint fromUint when op2 is uint toUint => (fromUint + toUint),
            ulong fromUlong when op2 is ulong toUlong => (fromUlong + toUlong),
            float fromFloat when op2 is float toFloat => (fromFloat + toFloat),
            double fromDouble when op2 is double toDouble => (fromDouble + toDouble),
            Vector2 fromVector2 when op2 is Vector2 toVector2 => (fromVector2 + toVector2),
            Vector2i fromVector2i when op2 is Vector2i toVector2i => (fromVector2i + toVector2i),
            Vector3 fromVector3 when op2 is Vector3 toVector3 => (fromVector3 + toVector3),
            Vector3i fromVector3i when op2 is Vector3i toVector3i => (fromVector3i + toVector3i),
            Vector4 fromVector4 when op2 is Vector4 toVector4 => (fromVector4 + toVector4),
            Vector4i fromVector4i when op2 is Vector4i toVector4i => (fromVector4i + toVector4i),
            Quaternion fromQuaternion when op2 is Quaternion toQuaternion => (fromQuaternion + toQuaternion),
            Color fromColor when op2 is Color toColor => (fromColor + toColor),
            _ => throw new Exception($"Sum Variant: {op1?.GetType().Name} not implemented")
        };
    }

    public static Variant Subtract<[MustBeVariant] T>(T op1, T op2) {
        return op1 switch {
            char fromChar when op2 is char toChar => (fromChar - toChar),
            sbyte fromSbyte when op2 is sbyte toSbyte => (fromSbyte - toSbyte),
            short fromShort when op2 is short toShort => (fromShort - toShort),
            int fromInt when op2 is int toInt => (fromInt - toInt),
            long fromLong when op2 is long toLong => (fromLong - toLong),
            byte fromByte when op2 is byte toByte => (fromByte - toByte),
            ushort fromUshort when op2 is ushort toUshort => (fromUshort - toUshort),
            uint fromUint when op2 is uint toUint => (fromUint - toUint),
            ulong fromUlong when op2 is ulong toUlong => (fromUlong - toUlong),
            float fromFloat when op2 is float toFloat => (fromFloat - toFloat),
            double fromDouble when op2 is double toDouble => (fromDouble - toDouble),
            Vector2 fromVector2 when op2 is Vector2 toVector2 => (fromVector2 - toVector2),
            Vector2i fromVector2i when op2 is Vector2i toVector2i => (fromVector2i - toVector2i),
            Vector3 fromVector3 when op2 is Vector3 toVector3 => (fromVector3 - toVector3),
            Vector3i fromVector3i when op2 is Vector3i toVector3i => (fromVector3i - toVector3i),
            Vector4 fromVector4 when op2 is Vector4 toVector4 => (fromVector4 - toVector4),
            Vector4i fromVector4i when op2 is Vector4i toVector4i => (fromVector4i - toVector4i),
            Quaternion fromQuaternion when op2 is Quaternion toQuaternion => (fromQuaternion - toQuaternion),
            Color fromColor when op2 is Color toColor => (fromColor - toColor),
            _ => throw new Exception($"Subtract Variant: {op1?.GetType().Name} not implemented")
        };
    }

    public static Variant AddVariant(Variant op1, Variant op2, float t) {
        if (op1.VariantType != op2.VariantType) throw new Exception($"AddVariant: types are not equal: {op1.VariantType}  {op2.VariantType}");
        if (op1.VariantType == Variant.Type.Bool) return (op1.AsBool() ? 1 : 0) + (op2.AsBool() ? 1 : 0);
        if (op1.VariantType == Variant.Type.Int) return op1.AsInt64() + op2.AsInt64(); 
        if (op1.VariantType == Variant.Type.Float) return op1.AsDouble() + op2.AsDouble(); 
        if (op1.VariantType == Variant.Type.Vector2) return op1.AsVector2() + op2.AsVector2(); 
        if (op1.VariantType == Variant.Type.Vector2i) return op1.AsVector2i() + op2.AsVector2i(); 
        if (op1.VariantType == Variant.Type.Rect2) {
            var op1Rect = op1.AsRect2();
            var op2Rect = op2.AsRect2();
            return new Rect2(op1Rect.Position + op2Rect.Position, op1Rect.Size + op2Rect.Size);
        }
        if (op1.VariantType == Variant.Type.Rect2i) {
            var op1Rect = op1.AsRect2i();
            var op2Rect = op2.AsRect2i();
            return new Rect2i(op1Rect.Position + op2Rect.Position, op1Rect.Size + op2Rect.Size);
        }
        if (op1.VariantType == Variant.Type.Vector3) return op1.AsVector3() + op2.AsVector3();
        if (op1.VariantType == Variant.Type.Vector3i) return op1.AsVector3i() + op2.AsVector3i();
        if (op1.VariantType == Variant.Type.Transform2d) return op1.AsTransform2D() * op2.AsTransform2D();
        if (op1.VariantType == Variant.Type.Vector4) return op1.AsVector4() + op2.AsVector4(); 
        if (op1.VariantType == Variant.Type.Vector4i) return op1.AsVector4i() + op2.AsVector4i();
        if (op1.VariantType == Variant.Type.Plane) {
            var op1Plane = op1.AsPlane();
            var op2Plane = op2.AsPlane();
            return new Plane(op1Plane.Normal + op2Plane.Normal, op1Plane.D + op2Plane.D);
        } 
        if (op1.VariantType == Variant.Type.Quaternion) return op1.AsQuaternion() + op2.AsQuaternion(); 
        if (op1.VariantType == Variant.Type.Aabb) {
            var op1Aabb = op1.AsAABB();
            var op2Aabb = op2.AsAABB();
            return new AABB(op1Aabb.Position + op2Aabb.Position, op1Aabb.Size + op2Aabb.Size);
        } 
        if (op1.VariantType == Variant.Type.Transform3d) return op1.AsTransform3D() * op2.AsTransform3D(); 
        if (op1.VariantType == Variant.Type.Color) return op1.AsColor() + op2.AsColor();
        throw new Exception($"AddVariant: {op1.VariantType} not implemented");
    }

    public static Variant SubtractVariant(Variant op1, Variant op2, float t) {
        if (op1.VariantType != op2.VariantType) throw new Exception($"SubtractVariant: types are not equal: {op1.VariantType}  {op2.VariantType}");
        if (op1.VariantType == Variant.Type.Bool) return (op1.AsBool() ? 1 : 0) - (op2.AsBool() ? 1 : 0);
        if (op1.VariantType == Variant.Type.Int) return op1.AsInt64() - op2.AsInt64(); 
        if (op1.VariantType == Variant.Type.Float) return op1.AsDouble() - op2.AsDouble(); 
        if (op1.VariantType == Variant.Type.Vector2) return op1.AsVector2() - op2.AsVector2(); 
        if (op1.VariantType == Variant.Type.Vector2i) return op1.AsVector2i() - op2.AsVector2i();
        if (op1.VariantType == Variant.Type.Rect2) {
            var op1Rect = op1.AsRect2();
            var op2Rect = op2.AsRect2();
            return new Rect2(op1Rect.Position - op2Rect.Position, op1Rect.Size - op2Rect.Size);
        }
        if (op1.VariantType == Variant.Type.Rect2i) {
            var op1Rect = op1.AsRect2i();
            var op2Rect = op2.AsRect2i();
            return new Rect2i(op1Rect.Position - op2Rect.Position, op1Rect.Size - op2Rect.Size);
        }
        if (op1.VariantType == Variant.Type.Vector3) return op1.AsVector3() - op2.AsVector3();
        if (op1.VariantType == Variant.Type.Vector3i) return op1.AsVector3i() - op2.AsVector3i();
        if (op1.VariantType == Variant.Type.Transform2d) return op1.AsTransform2D().Inverse() * op2.AsTransform2D();
        if (op1.VariantType == Variant.Type.Vector4) return op1.AsVector4() - op2.AsVector4(); 
        if (op1.VariantType == Variant.Type.Vector4i) return op1.AsVector4i() - op2.AsVector4i(); 
        if (op1.VariantType == Variant.Type.Plane) {
            var op1Plane = op1.AsPlane();
            var op2Plane = op2.AsPlane();
            return new Plane(op1Plane.Normal - op2Plane.Normal, op1Plane.D - op2Plane.D);
        } 
        if (op1.VariantType == Variant.Type.Quaternion) return op1.AsQuaternion() - op2.AsQuaternion();
        if (op1.VariantType == Variant.Type.Aabb) {
            var op1Aabb = op1.AsAABB();
            var op2Aabb = op2.AsAABB();
            return new AABB(op1Aabb.Position - op2Aabb.Position, op1Aabb.Size - op2Aabb.Size);
        } 
        if (op1.VariantType == Variant.Type.Transform3d) return op1.AsTransform3D().Inverse() * op2.AsTransform3D(); 
        if (op1.VariantType == Variant.Type.Color) return op1.AsColor() - op2.AsColor();
        throw new Exception($"Subtract Variant: {op1.VariantType} type not implemented");
    }

    public static Variant LerpVariant(Variant op1, Variant op2, float t) {
        if (op1.VariantType != op2.VariantType) throw new Exception($"LerpVariant: types are not equal: {op1.VariantType}  {op2.VariantType}");
        if (op1.VariantType == Variant.Type.Bool) return Lerp(op1.AsBool(), op2.AsBool(), t);
        if (op1.VariantType == Variant.Type.Int) return Lerp(op1.AsInt64(), op2.AsInt64(), t); 
        if (op1.VariantType == Variant.Type.Float) return Lerp(op1.AsDouble(), op2.AsDouble(), t); 
        if (op1.VariantType == Variant.Type.Vector2) return op1.AsVector2().Lerp(op2.AsVector2(), t); 
        if (op1.VariantType == Variant.Type.Vector2i) return op1.AsVector2i().Lerp(op2.AsVector2i(), t); 
        if (op1.VariantType == Variant.Type.Rect2) return op1.AsRect2().Lerp(op2.AsRect2(), t); 
        if (op1.VariantType == Variant.Type.Rect2i) return op1.AsRect2i().Lerp(op2.AsRect2i(), t); 
        if (op1.VariantType == Variant.Type.Vector3) return op1.AsVector3().Lerp(op2.AsVector3(), t); 
        if (op1.VariantType == Variant.Type.Vector3i) return op1.AsVector3i().Lerp(op2.AsVector3i(), t); 
        if (op1.VariantType == Variant.Type.Transform2d) return op1.AsTransform2D().InterpolateWith(op2.AsTransform2D(), t); 
        if (op1.VariantType == Variant.Type.Vector4) return op1.AsVector4().Lerp(op2.AsVector4(), t); 
        if (op1.VariantType == Variant.Type.Vector4i) return op1.AsVector4i().Lerp(op2.AsVector4i(), t); 
        if (op1.VariantType == Variant.Type.Plane) return op1.AsPlane().Lerp(op2.AsPlane(), t); 
        if (op1.VariantType == Variant.Type.Quaternion) return op1.AsQuaternion().Slerp(op2.AsQuaternion(), t); 
        if (op1.VariantType == Variant.Type.Aabb) return op1.AsAABB().Lerp(op2.AsAABB(), t); 
        if (op1.VariantType == Variant.Type.Basis) return op1.AsBasis().Lerp(op2.AsBasis(), t); 
        if (op1.VariantType == Variant.Type.Transform3d) return op1.AsTransform3D().InterpolateWith(op2.AsTransform3D(), t); 
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
            Vector2i fromVector2i when op2 is Vector2i toVector2i => fromVector2i.Lerp(toVector2i, t),
            Rect2 fromRect2 when op2 is Rect2 toRect2 => fromRect2.Lerp(toRect2, t),
            Rect2i fromRect2i when op2 is Rect2i toRect2i => fromRect2i.Lerp(toRect2i, t),
            Transform2D fromTransform2D when op2 is Transform2D toTransform2D => fromTransform2D.InterpolateWith(toTransform2D, t),
            Vector3 fromVector3 when op2 is Vector3 toVector3 => fromVector3.Lerp(toVector3, t),
            Vector3i fromVector3i when op2 is Vector3i toVector3i => fromVector3i.Lerp(toVector3i, t),
            Basis fromBasis when op2 is Basis toBasis => fromBasis.Lerp(toBasis, t),
            Quaternion fromQuaternion when op2 is Quaternion toQuaternion => fromQuaternion.Slerp(toQuaternion, t),
            Transform3D fromTransform3D when op2 is Transform3D toTransform3D => fromTransform3D.InterpolateWith(toTransform3D, t),
            Vector4 fromVector4 when op2 is Vector4 toVector4 => fromVector4.Lerp(toVector4, t),
            Vector4i fromVector4i when op2 is Vector4i toVector4i => fromVector4i.Lerp(toVector4i, t),
            AABB fromAABB when op2 is AABB toAABB => fromAABB.Lerp(toAABB, t),
            Color fromColor when op2 is Color toColor => fromColor.Lerp(toColor, t),
            Plane fromPlane when op2 is Plane toPlane => fromPlane.Lerp(toPlane, t),
            _ => throw new Exception($"LerpVariant<T>: {op1?.GetType().Name} type not implemented")
        };
    }

    public static bool Lerp(this bool from, bool to, float t) => 
        Mathf.Lerp(from ? 1 : 0, to ? 1 : 0, t) > 0.5;

    public static double Lerp(this double from, double to, float weight) =>
        from + (to - from) * weight;

    public static Vector2i Lerp(this Vector2i op1, Vector2i op2, float t) =>
        new((int)Mathf.Lerp(op1.x, op2.x, t),
            (int)Mathf.Lerp(op1.y, op2.y, t));

    public static Vector3i Lerp(this Vector3i op1, Vector3i op2, float t) =>
        new((int)Mathf.Lerp(op1.x, op2.x, t),
            (int)Mathf.Lerp(op1.y, op2.y, t),
            (int)Mathf.Lerp(op1.z, op2.z, t));

    public static Vector4i Lerp(this Vector4i op1, Vector4i op2, float t) =>
        new((int)Mathf.Lerp(op1.x, op2.x, t),
            (int)Mathf.Lerp(op1.y, op2.y, t),
            (int)Mathf.Lerp(op1.z, op2.z, t),
            (int)Mathf.Lerp(op1.w, op2.w, t));

    public static Rect2 Lerp(this Rect2 from, Rect2 to, float t) =>
        new(from.Position.Lerp(to.Position, t), 
            from.Size.Lerp(to.Size, t));

    public static Basis Lerp(this Basis from, Basis to, float t) =>
        new(from.Row0.Lerp(to.Row0, t), 
            from.Row1.Lerp(to.Row1, t),
            from.Row2.Lerp(to.Row2, t));

    public static Rect2i Lerp(this Rect2i from, Rect2i to, float t) =>
        new(from.Position.Lerp(to.Position, t), 
            from.Size.Lerp(to.Size, t));

    public static AABB Lerp(this AABB from, AABB to, float t) =>
        new(from.Position.Lerp(to.Position, t), 
            from.Size.Lerp(to.Size, t));

    public static Plane Lerp(this Plane from, Plane to, float t) =>
        new(from.Normal.Lerp(to.Normal, t), 
            Mathf.Lerp(from.D, to.D, t));
}