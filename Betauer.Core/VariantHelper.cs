using System;
using Godot;
using Godot.Collections;
using Object = Godot.Object;
using Array = Godot.Collections.Array;

namespace Betauer.Core {
    public static class VariantHelper {
        public static Variant CreateFrom<T>(T value) {
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
                SignalInfo valueSignalInfo => valueSignalInfo,
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

        public static T ConvertTo<T>(Variant value) {
            var t = typeof(T);
            if (t == typeof(object)) return (T)ConvertTo(value);
            if (t == typeof(bool)) return (T)(object)value.AsBool();
            if (t == typeof(char)) return (T)(object)value.AsChar();
            if (t == typeof(sbyte)) return (T)(object)value.AsSByte();
            if (t == typeof(short)) return (T)(object)value.AsInt16();
            if (t == typeof(int)) return (T)(object)value.AsInt32();
            if (t == typeof(long)) return (T)(object)value.AsInt64();
            if (t == typeof(byte)) return (T)(object)value.AsByte();
            if (t == typeof(ushort)) return (T)(object)value.AsUInt16();
            if (t == typeof(uint)) return (T)(object)value.AsUInt32();
            if (t == typeof(ulong)) return (T)(object)value.AsUInt64();
            if (t == typeof(float)) return (T)(object)value.AsSingle();
            if (t == typeof(double)) return (T)(object)value.AsDouble();
            if (t == typeof(string)) return (T)(object)value.AsString();
            if (t == typeof(Vector2)) return (T)(object)value.AsVector2();
            if (t == typeof(Vector2i)) return (T)(object)value.AsVector2i();
            if (t == typeof(Rect2)) return (T)(object)value.AsRect2();
            if (t == typeof(Rect2i)) return (T)(object)value.AsRect2i();
            if (t == typeof(Transform2D)) return (T)(object)value.AsTransform2D();
            if (t == typeof(Vector3)) return (T)(object)value.AsVector3();
            if (t == typeof(Vector3i)) return (T)(object)value.AsVector3i();
            if (t == typeof(Vector4)) return (T)(object)value.AsVector4();
            if (t == typeof(Vector4i)) return (T)(object)value.AsVector4i();
            if (t == typeof(Basis)) return (T)(object)value.AsBasis();
            if (t == typeof(Quaternion)) return (T)(object)value.AsQuaternion();
            if (t == typeof(Transform3D)) return (T)(object)value.AsTransform3D();
            if (t == typeof(Projection)) return (T)(object)value.AsProjection();
            if (t == typeof(AABB)) return (T)(object)value.AsAABB();
            if (t == typeof(Color)) return (T)(object)value.AsColor();
            if (t == typeof(Plane)) return (T)(object)value.AsPlane();
            if (t == typeof(Callable)) return (T)(object)value.AsCallable();
            if (t == typeof(SignalInfo)) return (T)(object)value.AsSignalInfo();
            if (t == typeof(Span<byte>)) return (T)(object)value.AsByteArray();
            if (t == typeof(Span<int>)) return (T)(object)value.AsInt32Array();
            if (t == typeof(Span<long>)) return (T)(object)value.AsInt64Array();
            if (t == typeof(Span<float>)) return (T)(object)value.AsFloat32Array();
            if (t == typeof(Span<double>)) return (T)(object)value.AsFloat64Array();
            if (t == typeof(Span<string>)) return (T)(object)value.AsStringArray();
            if (t == typeof(Span<Vector2>)) return (T)(object)value.AsVector2Array();
            if (t == typeof(Span<Vector3>)) return (T)(object)value.AsVector3Array();
            if (t == typeof(Span<Color>)) return (T)(object)value.AsColorArray();
            if (t == typeof(Object[])) return (T)(object)value.AsGodotObjectArray<Object>();
            if (t == typeof(Span<StringName>)) return (T)(object)value.AsSystemArrayOfStringName();
            if (t == typeof(Span<NodePath>)) return (T)(object)value.AsSystemArrayOfNodePath();
            if (t == typeof(Span<RID>)) return (T)(object)value.AsSystemArrayOfRID();
            if (t == typeof(Object)) return (T)(object)value.AsGodotObject();
            if (t == typeof(StringName)) return (T)(object)value.AsStringName();
            if (t == typeof(NodePath)) return (T)(object)value.AsNodePath();
            if (t == typeof(RID)) return (T)(object)value.AsRID();
            if (t == typeof(Dictionary)) return (T)(object)value.AsGodotDictionary();
            if (t == typeof(Array)) return (T)(object)value.AsGodotArray();
            // if (t == typeof(Dictionary<TKey, TValue>>) return (T)(object)value.AsGodotDictionary<TKey, TValue>());
            // if (t == typeof(Godot.Collections.Array<T>>)) return (T)(object)value.AsGodotArray();
            throw new Exception("ConverTo<T>: Unknown variant for type: " + typeof(T).Name);
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
                Variant.Type.Signal => op1.AsSignalInfo(),
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
                _ => throw new Exception("ConverTo: Unknown variant type: "+op1.VariantType)
            };
        }

        public static T Add<T>(T op1, T op2) {
            return op1 switch {
                char fromChar when op2 is char toChar => (T)(object)(fromChar + toChar),
                sbyte fromSbyte when op2 is sbyte toSbyte => (T)(object)(fromSbyte + toSbyte),
                short fromShort when op2 is short toShort => (T)(object)(fromShort + toShort),
                int fromInt when op2 is int toInt => (T)(object)(fromInt + toInt),
                long fromLong when op2 is long toLong => (T)(object)(fromLong + toLong),
                byte fromByte when op2 is byte toByte => (T)(object)(fromByte + toByte),
                ushort fromUshort when op2 is ushort toUshort => (T)(object)(fromUshort + toUshort),
                uint fromUint when op2 is uint toUint => (T)(object)(fromUint + toUint),
                ulong fromUlong when op2 is ulong toUlong => (T)(object)(fromUlong + toUlong),
                float fromFloat when op2 is float toFloat => (T)(object)(fromFloat + toFloat),
                double fromDouble when op2 is double toDouble => (T)(object)(fromDouble + toDouble),
                Vector2 fromVector2 when op2 is Vector2 toVector2 => (T)(object)(fromVector2 + toVector2),
                Vector2i fromVector2i when op2 is Vector2i toVector2i => (T)(object)(fromVector2i + toVector2i),
                Vector3 fromVector3 when op2 is Vector3 toVector3 => (T)(object)(fromVector3 + toVector3),
                Vector3i fromVector3i when op2 is Vector3i toVector3i => (T)(object)(fromVector3i + toVector3i),
                Vector4 fromVector4 when op2 is Vector4 toVector4 => (T)(object)(fromVector4 + toVector4),
                Vector4i fromVector4i when op2 is Vector4i toVector4i => (T)(object)(fromVector4i + toVector4i),
                Quaternion fromQuaternion when op2 is Quaternion toQuaternion => (T)(object)(fromQuaternion + toQuaternion),
                Color fromColor when op2 is Color toColor => (T)(object)(fromColor + toColor),
                _ => throw new Exception($"Sum Variant: {op1?.GetType().Name} not implemented")
            };
        }

        public static T Subtract<T>(T op1, T op2) {
            return op1 switch {
                char fromChar when op2 is char toChar => (T)(object)(fromChar - toChar),
                sbyte fromSbyte when op2 is sbyte toSbyte => (T)(object)(fromSbyte - toSbyte),
                short fromShort when op2 is short toShort => (T)(object)(fromShort - toShort),
                int fromInt when op2 is int toInt => (T)(object)(fromInt - toInt),
                long fromLong when op2 is long toLong => (T)(object)(fromLong - toLong),
                byte fromByte when op2 is byte toByte => (T)(object)(fromByte - toByte),
                ushort fromUshort when op2 is ushort toUshort => (T)(object)(fromUshort - toUshort),
                uint fromUint when op2 is uint toUint => (T)(object)(fromUint - toUint),
                ulong fromUlong when op2 is ulong toUlong => (T)(object)(fromUlong - toUlong),
                float fromFloat when op2 is float toFloat => (T)(object)(fromFloat - toFloat),
                double fromDouble when op2 is double toDouble => (T)(object)(fromDouble - toDouble),
                Vector2 fromVector2 when op2 is Vector2 toVector2 => (T)(object)(fromVector2 - toVector2),
                Vector2i fromVector2i when op2 is Vector2i toVector2i => (T)(object)(fromVector2i - toVector2i),
                Vector3 fromVector3 when op2 is Vector3 toVector3 => (T)(object)(fromVector3 - toVector3),
                Vector3i fromVector3i when op2 is Vector3i toVector3i => (T)(object)(fromVector3i - toVector3i),
                Vector4 fromVector4 when op2 is Vector4 toVector4 => (T)(object)(fromVector4 - toVector4),
                Vector4i fromVector4i when op2 is Vector4i toVector4i => (T)(object)(fromVector4i - toVector4i),
                Quaternion fromQuaternion when op2 is Quaternion toQuaternion => (T)(object)(fromQuaternion - toQuaternion),
                Color fromColor when op2 is Color toColor => (T)(object)(fromColor - toColor),
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

        public static T LerpVariant<T>(T op1, T op2, float t) {
            return op1 switch {
                bool fromBool when op2 is bool toBool => (T)(object)Lerp(fromBool, toBool, t),
                char fromChar when op2 is char toChar => (T)(object)Mathf.Lerp(fromChar, toChar, t),
                sbyte fromSbyte when op2 is sbyte toSbyte => (T)(object)Mathf.Lerp(fromSbyte, toSbyte, t),
                short fromShort when op2 is short toShort => (T)(object)Mathf.Lerp(fromShort, toShort, t),
                int fromInt when op2 is int toInt => (T)(object)Mathf.Lerp(fromInt, toInt, t),
                long fromLong when op2 is long toLong => (T)(object)Mathf.Lerp(fromLong, toLong, t),
                byte fromByte when op2 is byte toByte => (T)(object)Mathf.Lerp(fromByte, toByte, t),
                ushort fromUshort when op2 is ushort toUshort => (T)(object)Mathf.Lerp(fromUshort, toUshort, t),
                uint fromUint when op2 is uint toUint => (T)(object)Mathf.Lerp(fromUint, toUint, t),
                ulong fromUlong when op2 is ulong toUlong => (T)(object)Mathf.Lerp(fromUlong, toUlong, t),
                float fromFloat when op2 is float toFloat => (T)(object)Mathf.Lerp(fromFloat, toFloat, t),
                double fromDouble when op2 is double toDouble => (T)(object)Lerp(fromDouble, toDouble, t),
                Vector2 fromVector2 when op2 is Vector2 toVector2 => (T)(object)fromVector2.Lerp(toVector2, t),
                Vector2i fromVector2i when op2 is Vector2i toVector2i => (T)(object)fromVector2i.Lerp(toVector2i, t),
                Rect2 fromRect2 when op2 is Rect2 toRect2 => (T)(object)fromRect2.Lerp(toRect2, t),
                Rect2i fromRect2i when op2 is Rect2i toRect2i => (T)(object)fromRect2i.Lerp(toRect2i, t),
                Transform2D fromTransform2D when op2 is Transform2D toTransform2D => (T)(object)fromTransform2D.InterpolateWith(toTransform2D, t),
                Vector3 fromVector3 when op2 is Vector3 toVector3 => (T)(object)fromVector3.Lerp(toVector3, t),
                Vector3i fromVector3i when op2 is Vector3i toVector3i => (T)(object)fromVector3i.Lerp(toVector3i, t),
                Basis fromBasis when op2 is Basis toBasis => (T)(object)fromBasis.Lerp(toBasis, t),
                Quaternion fromQuaternion when op2 is Quaternion toQuaternion => (T)(object)fromQuaternion.Slerp(toQuaternion, t),
                Transform3D fromTransform3D when op2 is Transform3D toTransform3D => (T)(object)fromTransform3D.InterpolateWith(toTransform3D, t),
                Vector4 fromVector4 when op2 is Vector4 toVector4 => (T)(object)fromVector4.Lerp(toVector4, t),
                Vector4i fromVector4i when op2 is Vector4i toVector4i => (T)(object)fromVector4i.Lerp(toVector4i, t),
                AABB fromAABB when op2 is AABB toAABB => (T)(object)fromAABB.Lerp(toAABB, t),
                Color fromColor when op2 is Color toColor => (T)(object)fromColor.Lerp(toColor, t),
                Plane fromPlane when op2 is Plane toPlane => (T)(object)fromPlane.Lerp(toPlane, t),
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
}