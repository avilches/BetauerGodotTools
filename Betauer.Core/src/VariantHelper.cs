using System;
using Godot;
using Godot.Collections;
using Array = Godot.Collections.Array;

namespace Betauer.Core; 

public static class VariantHelper {
    public static bool IsNull(this Variant variant) => variant.VariantType == Variant.Type.Nil;
    public static bool IsBool(this Variant variant) => variant.VariantType == Variant.Type.Bool;
    public static bool IsInt64(this Variant variant) => variant.VariantType == Variant.Type.Int;
    public static bool IsSingle(this Variant variant) => variant.VariantType == Variant.Type.Float;
    public static bool IsString(this Variant variant) => variant.VariantType == Variant.Type.String;
    public static bool IsVector2(this Variant variant) => variant.VariantType == Variant.Type.Vector2;
    public static bool IsVector2I(this Variant variant) => variant.VariantType == Variant.Type.Vector2I;
    public static bool IsRect2(this Variant variant) => variant.VariantType == Variant.Type.Rect2;
    public static bool IsRect2I(this Variant variant) => variant.VariantType == Variant.Type.Rect2I;
    public static bool IsVector3(this Variant variant) => variant.VariantType == Variant.Type.Vector3;
    public static bool IsVector3I(this Variant variant) => variant.VariantType == Variant.Type.Vector3I;
    public static bool IsTransform2D(this Variant variant) => variant.VariantType == Variant.Type.Transform2D;
    public static bool IsVector4(this Variant variant) => variant.VariantType == Variant.Type.Vector4;
    public static bool IsVector4I(this Variant variant) => variant.VariantType == Variant.Type.Vector4I;
    public static bool IsPlane(this Variant variant) => variant.VariantType == Variant.Type.Plane;
    public static bool IsQuaternion(this Variant variant) => variant.VariantType == Variant.Type.Quaternion;
    public static bool IsAabb(this Variant variant) => variant.VariantType == Variant.Type.Aabb;
    public static bool IsBasis(this Variant variant) => variant.VariantType == Variant.Type.Basis;
    public static bool IsTransform3D(this Variant variant) => variant.VariantType == Variant.Type.Transform3D;
    public static bool IsProjection(this Variant variant) => variant.VariantType == Variant.Type.Projection;
    public static bool IsColor(this Variant variant) => variant.VariantType == Variant.Type.Color;
    public static bool IsStringName(this Variant variant) => variant.VariantType == Variant.Type.StringName;
    public static bool IsNodePath(this Variant variant) => variant.VariantType == Variant.Type.NodePath;
    public static bool IsRid(this Variant variant) => variant.VariantType == Variant.Type.Rid;
    public static bool IsGodotObject(this Variant variant) => variant.VariantType == Variant.Type.Object;
    public static bool IsCallable(this Variant variant) => variant.VariantType == Variant.Type.Callable;
    public static bool IsSignal(this Variant variant) => variant.VariantType == Variant.Type.Signal;
    public static bool IsGodotDictionary(this Variant variant) => variant.VariantType == Variant.Type.Dictionary;
    public static bool IsGodotArray(this Variant variant) => variant.VariantType == Variant.Type.Array;
    public static bool IsByteArray(this Variant variant) => variant.VariantType == Variant.Type.PackedByteArray;
    public static bool IsInt32Array(this Variant variant) => variant.VariantType == Variant.Type.PackedInt32Array;
    public static bool IsInt64Array(this Variant variant) => variant.VariantType == Variant.Type.PackedInt64Array;
    public static bool IsFloat32Array(this Variant variant) => variant.VariantType == Variant.Type.PackedFloat32Array;
    public static bool IsFloat64Array(this Variant variant) => variant.VariantType == Variant.Type.PackedFloat64Array;
    public static bool IsStringArray(this Variant variant) => variant.VariantType == Variant.Type.PackedStringArray;
    public static bool IsVector2Array(this Variant variant) => variant.VariantType == Variant.Type.PackedVector2Array;
    public static bool IsVector3Array(this Variant variant) => variant.VariantType == Variant.Type.PackedVector3Array;
    public static bool IsColorArray(this Variant variant) => variant.VariantType == Variant.Type.PackedColorArray;

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
            Vector2I valueVector2I => valueVector2I,
            Rect2 valueRect2 => valueRect2,
            Rect2I valueRect2I => valueRect2I,
            Transform2D valueTransform2D => valueTransform2D,
            Vector3 valueVector3 => valueVector3,
            Vector3I valueVector3I => valueVector3I,
            Vector4 valueVector4 => valueVector4,
            Vector4I valueVector4I => valueVector4I,
            Basis valueBasis => valueBasis,
            Quaternion valueQuaternion => valueQuaternion,
            Transform3D valueTransform3D => valueTransform3D,
            Projection valueProjection => valueProjection,
            Aabb valueAabb => valueAabb,
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
            GodotObject[] valueObject => valueObject,
            StringName[] valueArrayStringName => valueArrayStringName.AsSpan(),
            NodePath[] valueArrayNodePath => valueArrayNodePath.AsSpan(),
            Rid[] valueArrayRid => valueArrayRid.AsSpan(),
            GodotObject valueObject => valueObject,
            StringName valueStringName => valueStringName,
            NodePath valueNodePath => valueNodePath,
            Rid valueRid => valueRid,
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
            Variant.Type.Vector2I => op1.AsVector2I(),
            Variant.Type.Rect2 => op1.AsRect2(),
            Variant.Type.Rect2I => op1.AsRect2I(),
            Variant.Type.Vector3 => op1.AsVector3(),
            Variant.Type.Vector3I => op1.AsVector3I(),
            Variant.Type.Transform2D => op1.AsTransform2D(),
            Variant.Type.Vector4 => op1.AsVector4(),
            Variant.Type.Vector4I => op1.AsVector4I(),
            Variant.Type.Plane => op1.AsPlane(),
            Variant.Type.Quaternion => op1.AsQuaternion(),
            Variant.Type.Aabb => op1.AsAabb(),
            Variant.Type.Basis => op1.AsBasis(),
            Variant.Type.Transform3D => op1.AsTransform3D(),
            Variant.Type.Projection => op1.AsProjection(),
            Variant.Type.Color => op1.AsColor(),
            Variant.Type.StringName => op1.AsStringName(),
            Variant.Type.NodePath => op1.AsNodePath(),
            Variant.Type.Rid => op1.AsRid(),
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
            _ => throw new Exception($"ConvertTo({op1.VariantType}) not implement4d. Unknown variant type")
        };
    }

    public static Variant Add<[MustBeVariant] T>(T op1, T op2) {
        return op1 switch {
            // bool from when op2 is bool to => (from + to),
            char from when op2 is char to => (from + to),
            sbyte from when op2 is sbyte to => (from + to),
            short from when op2 is short to => (from + to),
            int from when op2 is int to => (from + to),
            long from when op2 is long to => (from + to),
            byte from when op2 is byte to => (from + to),
            ushort from when op2 is ushort to => (from + to),
            uint from when op2 is uint to => (from + to),
            ulong from when op2 is ulong to => (from + to),
            float from when op2 is float to => (from + to),
            double from when op2 is double to => (from + to),
            string from when op2 is string to => (from + to),
            Vector2 from when op2 is Vector2 to => (from + to),
            Vector2I from when op2 is Vector2I to => (from + to),
            // Rect2 from when op2 is Rect2 to => (from + to),
            // Rect2I from when op2 is Rect2I to => (from + to),
            // Transform2D from when op2 is Transform2D to => (from + to),
            Vector3 from when op2 is Vector3 to => (from + to),
            Vector3I from when op2 is Vector3I to => (from + to),
            Vector4 from when op2 is Vector4 to => (from + to),
            Vector4I from when op2 is Vector4I to => (from + to),
            // Basis from when op2 is Basis to => (from + to),
            Quaternion from when op2 is Quaternion to => (from + to),
            // Transform3D from when op2 is Transform3D to => (from + to),
            // Projection from when op2 is Projection to => (from + to),
            // Aabb from when op2 is Aabb to => (from + to),
            Color from when op2 is Color to => (from + to),
            // Plane from when op2 is Plane to => (from + to),
            // Callable from when op2 is Callable to => EqualsVariant(from, to),
            // Godot.Signal from when op2 is Godot.Signal to => EqualsVariant(from, to),
            // byte[]
            // int[]
            // long[]
            // float[]
            // double[]
            // string[]
            // Vector2[]
            // Vector3[]
            // Color[]
            // GodotObject[]
            // StringName[]
            // NodePath[]
            // Rid[]
            // GodotObject from when op2 is GodotObject to => (from + to),
            StringName from when op2 is StringName to => (from + to),
            NodePath from when op2 is NodePath to => (from + to),
            // Rid from when op2 is Rid to => (from + to),
            // Dictionary
            // Array
            _ => throw new Exception($"Add<{op1?.GetType().Name}>() not implemented")
        };
    }

    public static bool Equals<[MustBeVariant] T>(T op1, T op2) {
        if (op1 is null && op2 is null) return true;
        if (op1 is null && op2 is not null) return false;
        if (op1 is not null && op2 is null) return false;
        
        return op1 switch {
            bool from when op2 is bool to => (from == to),
            char from when op2 is char to => (from == to),
            sbyte from when op2 is sbyte to => (from == to),
            short from when op2 is short to => (from == to),
            int from when op2 is int to => (from == to),
            long from when op2 is long to => (from == to),
            byte from when op2 is byte to => (from == to),
            ushort from when op2 is ushort to => (from == to),
            uint from when op2 is uint to => (from == to),
            ulong from when op2 is ulong to => (from == to),
            float from when op2 is float to => (from == to),
            double from when op2 is double to => (from == to),
            string from when op2 is string to => (from == to),
            Vector2 from when op2 is Vector2 to => (from == to),
            Vector2I from when op2 is Vector2I to => (from == to),
            Rect2 from when op2 is Rect2 to => (from == to),
            Rect2I from when op2 is Rect2I to => (from == to),
            Transform2D from when op2 is Transform2D to => (from == to),
            Vector3 from when op2 is Vector3 to => (from == to),
            Vector3I from when op2 is Vector3I to => (from == to),
            Vector4 from when op2 is Vector4 to => (from == to),
            Vector4I from when op2 is Vector4I to => (from == to),
            Basis from when op2 is Basis to => (from == to),
            Quaternion from when op2 is Quaternion to => (from == to),
            Transform3D from when op2 is Transform3D to => (from == to),
            Projection from when op2 is Projection to => (from == to),
            Aabb from when op2 is Aabb to => (from == to),
            Color from when op2 is Color to => (from == to),
            Plane from when op2 is Plane to => (from == to),
            // Callable from when op2 is Callable to => EqualsVariant(from, to),
            // Godot.Signal from when op2 is Godot.Signal to => EqualsVariant(from, to),
            // byte[]
            // int[]
            // long[]
            // float[]
            // double[]
            // string[]
            // Vector2[]
            // Vector3[]
            // Color[]
            // GodotObject[]
            // StringName[]
            // NodePath[]
            // Rid[]
            GodotObject from when op2 is GodotObject to => (from == to),
            StringName from when op2 is StringName to => (from == to),
            NodePath from when op2 is NodePath to => (from == to),
            Rid from when op2 is Rid to => (from == to),
            // Dictionary
            // Array
            _ => throw new Exception($"Equals<{op1?.GetType().Name}>() not implemented")
        };
    }

    public static bool EqualsVariant(Variant op1, Variant op2) {
        if (op1.VariantType != op2.VariantType) return false;
        if (op1.IsNull()) return true;
        if (op1.IsBool()) return op1.AsBool() == op2.AsBool();
        if (op1.IsInt64()) return op1.AsInt64() == op2.AsInt64();
        if (op1.IsSingle()) return op1.AsSingle() == op2.AsSingle();
        if (op1.IsString()) return op1.AsString() == op2.AsString();
        if (op1.IsVector2()) return op1.AsVector2() == op2.AsVector2();
        if (op1.IsVector2I()) return op1.AsVector2I() == op2.AsVector2I();
        if (op1.IsRect2()) return op1.AsRect2() == op2.AsRect2();
        if (op1.IsRect2I()) return op1.AsRect2I() == op2.AsRect2I();
        if (op1.IsVector3()) return op1.AsVector3() == op2.AsVector3();
        if (op1.IsVector3I()) return op1.AsVector3I() == op2.AsVector3I();
        if (op1.IsTransform2D()) return op1.AsTransform2D() == op2.AsTransform2D();
        if (op1.IsVector4()) return op1.AsVector4() == op2.AsVector4();
        if (op1.IsVector4I()) return op1.AsVector4I() == op2.AsVector4I();
        if (op1.IsPlane()) return op1.AsPlane() == op2.AsPlane();
        if (op1.IsQuaternion()) return op1.AsQuaternion() == op2.AsQuaternion();
        if (op1.IsAabb()) return op1.AsAabb() == op2.AsAabb();
        if (op1.IsBasis()) return op1.AsBasis() == op2.AsBasis();
        if (op1.IsTransform3D()) return op1.AsTransform3D() == op2.AsTransform3D();
        if (op1.IsProjection()) return op1.AsProjection() == op2.AsProjection();
        if (op1.IsColor()) return op1.AsColor() == op2.AsColor();
        if (op1.IsStringName()) return op1.AsStringName() == op2.AsStringName();
        if (op1.IsNodePath()) return op1.AsNodePath() == op2.AsNodePath();
        if (op1.IsRid()) return op1.AsRid() == op2.AsRid();
        if (op1.IsGodotObject()) return op1.AsGodotObject() == op2.AsGodotObject();
        // if (op1.IsCallable()) return op1.AsCallable() == op2.AsCallable();
        if (op1.IsSignal()) {
            var s1 = op1.AsSignal();
            var s2 = op2.AsSignal();
            return s1.Name == s2.Name && s1.Owner == s2.Owner;
        }
        if (op1.IsGodotDictionary()) return op1.AsGodotDictionary() == op2.AsGodotDictionary();
        if (op1.IsByteArray()) return op1.AsByteArray().AsSpan().SequenceEqual(new ReadOnlySpan<byte>(op2.AsByteArray()));
        if (op1.IsInt32Array()) return op1.AsInt32Array().AsSpan().SequenceEqual(new ReadOnlySpan<int>(op2.AsInt32Array()));
        if (op1.IsInt64Array()) return op1.AsInt64Array().AsSpan().SequenceEqual(new ReadOnlySpan<long>(op2.AsInt64Array()));
        if (op1.IsFloat32Array()) return op1.AsFloat32Array().AsSpan().SequenceEqual(new ReadOnlySpan<float>(op2.AsFloat32Array()));
        if (op1.IsFloat64Array()) return op1.AsFloat64Array().AsSpan().SequenceEqual(new ReadOnlySpan<double>(op2.AsFloat64Array()));
        if (op1.IsStringArray()) return op1.AsStringArray().AsSpan().SequenceEqual(new ReadOnlySpan<string>(op2.AsStringArray()));
        if (op1.IsVector2Array()) return op1.AsVector2Array().AsSpan().SequenceEqual(new ReadOnlySpan<Vector2>(op2.AsVector2Array()));
        if (op1.IsVector3Array()) return op1.AsVector3Array().AsSpan().SequenceEqual(new ReadOnlySpan<Vector3>(op2.AsVector3Array()));
        if (op1.IsColorArray()) return op1.AsColorArray().AsSpan().SequenceEqual(new ReadOnlySpan<Color>(op2.AsColorArray()));
        throw new Exception($"EqualsVariant({op1.VariantType}) not implemented");
    }

    public static Variant Subtract<[MustBeVariant] T>(T op1, T op2) {
        return op1 switch {
            // bool from when op2 is bool to => (from - to),
            char from when op2 is char to => (from - to),
            sbyte from when op2 is sbyte to => (from - to),
            short from when op2 is short to => (from - to),
            int from when op2 is int to => (from - to),
            long from when op2 is long to => (from - to),
            byte from when op2 is byte to => (from - to),
            ushort from when op2 is ushort to => (from - to),
            uint from when op2 is uint to => (from - to),
            ulong from when op2 is ulong to => (from - to),
            float from when op2 is float to => (from - to),
            double from when op2 is double to => (from - to),
            // string from when op2 is string to => (from - to),
            Vector2 from when op2 is Vector2 to => (from - to),
            Vector2I from when op2 is Vector2I to => (from - to),
            // Rect2 from when op2 is Rect2 to => (from - to),
            // Rect2I from when op2 is Rect2I to => (from - to),
            // Transform2D from when op2 is Transform2D to => (from - to),
            Vector3 from when op2 is Vector3 to => (from - to),
            Vector3I from when op2 is Vector3I to => (from - to),
            Vector4 from when op2 is Vector4 to => (from - to),
            Vector4I from when op2 is Vector4I to => (from - to),
            // Basis from when op2 is Basis to => (from - to),
            Quaternion from when op2 is Quaternion to => (from - to),
            // Transform3D from when op2 is Transform3D to => (from - to),
            // Projection from when op2 is Projection to => (from - to),
            // Aabb from when op2 is Aabb to => (from - to),
            Color from when op2 is Color to => (from - to),
            // Plane from when op2 is Plane to => (from - to),
            // Callable from when op2 is Callable to => EqualsVariant(from, to),
            // Godot.Signal from when op2 is Godot.Signal to => EqualsVariant(from, to),
            // byte[]
            // int[]
            // long[]
            // float[]
            // double[]
            // string[]
            // Vector2[]
            // Vector3[]
            // Color[]
            // GodotObject[]
            // StringName[]
            // NodePath[]
            // Rid[]
            // GodotObject from when op2 is GodotObject to => (from - to),
            // StringName from when op2 is StringName to => (from - to),
            // NodePath from when op2 is NodePath to => (from - to),
            // Rid from when op2 is Rid to => (from - to),
            // Dictionary
            // Array
            _ => throw new Exception($"Substract<{op1?.GetType().Name}>() not implemented")
        };
    }

    public static Variant AddVariant(Variant op1, Variant op2, float t) {
        if (op1.VariantType != op2.VariantType) throw new Exception($"AddVariant: types are not equal: {op1.VariantType}  {op2.VariantType}");
        if (op1.VariantType == Variant.Type.Bool) return (op1.AsBool() ? 1 : 0) + (op2.AsBool() ? 1 : 0);
        if (op1.VariantType == Variant.Type.Int) return op1.AsInt64() + op2.AsInt64(); 
        if (op1.VariantType == Variant.Type.Float) return op1.AsDouble() + op2.AsDouble(); 
        if (op1.VariantType == Variant.Type.Vector2) return op1.AsVector2() + op2.AsVector2(); 
        if (op1.VariantType == Variant.Type.Vector2I) return op1.AsVector2I() + op2.AsVector2I(); 
        if (op1.VariantType == Variant.Type.Rect2) {
            var op1Rect = op1.AsRect2();
            var op2Rect = op2.AsRect2();
            return new Rect2(op1Rect.Position + op2Rect.Position, op1Rect.Size + op2Rect.Size);
        }
        if (op1.VariantType == Variant.Type.Rect2I) {
            var op1Rect = op1.AsRect2I();
            var op2Rect = op2.AsRect2I();
            return new Rect2I(op1Rect.Position + op2Rect.Position, op1Rect.Size + op2Rect.Size);
        }
        if (op1.VariantType == Variant.Type.Vector3) return op1.AsVector3() + op2.AsVector3();
        if (op1.VariantType == Variant.Type.Vector3I) return op1.AsVector3I() + op2.AsVector3I();
        if (op1.VariantType == Variant.Type.Transform2D) return op1.AsTransform2D() * op2.AsTransform2D();
        if (op1.VariantType == Variant.Type.Vector4) return op1.AsVector4() + op2.AsVector4(); 
        if (op1.VariantType == Variant.Type.Vector4I) return op1.AsVector4I() + op2.AsVector4I();
        if (op1.VariantType == Variant.Type.Plane) {
            var op1Plane = op1.AsPlane();
            var op2Plane = op2.AsPlane();
            return new Plane(op1Plane.Normal + op2Plane.Normal, op1Plane.D + op2Plane.D);
        } 
        if (op1.VariantType == Variant.Type.Quaternion) return op1.AsQuaternion() + op2.AsQuaternion(); 
        if (op1.VariantType == Variant.Type.Aabb) {
            var op1Aabb = op1.AsAabb();
            var op2Aabb = op2.AsAabb();
            return new Aabb(op1Aabb.Position + op2Aabb.Position, op1Aabb.Size + op2Aabb.Size);
        } 
        if (op1.VariantType == Variant.Type.Transform3D) return op1.AsTransform3D() * op2.AsTransform3D(); 
        if (op1.VariantType == Variant.Type.Color) return op1.AsColor() + op2.AsColor();
        throw new Exception($"AddVariant: {op1.VariantType} not implemented");
    }

    public static Variant SubtractVariant(Variant op1, Variant op2, float t) {
        if (op1.VariantType != op2.VariantType) throw new Exception($"SubtractVariant: types are not equal: {op1.VariantType}  {op2.VariantType}");
        if (op1.VariantType == Variant.Type.Bool) return (op1.AsBool() ? 1 : 0) - (op2.AsBool() ? 1 : 0);
        if (op1.VariantType == Variant.Type.Int) return op1.AsInt64() - op2.AsInt64(); 
        if (op1.VariantType == Variant.Type.Float) return op1.AsDouble() - op2.AsDouble(); 
        if (op1.VariantType == Variant.Type.Vector2) return op1.AsVector2() - op2.AsVector2(); 
        if (op1.VariantType == Variant.Type.Vector2I) return op1.AsVector2I() - op2.AsVector2I();
        if (op1.VariantType == Variant.Type.Rect2) {
            var op1Rect = op1.AsRect2();
            var op2Rect = op2.AsRect2();
            return new Rect2(op1Rect.Position - op2Rect.Position, op1Rect.Size - op2Rect.Size);
        }
        if (op1.VariantType == Variant.Type.Rect2I) {
            var op1Rect = op1.AsRect2I();
            var op2Rect = op2.AsRect2I();
            return new Rect2I(op1Rect.Position - op2Rect.Position, op1Rect.Size - op2Rect.Size);
        }
        if (op1.VariantType == Variant.Type.Vector3) return op1.AsVector3() - op2.AsVector3();
        if (op1.VariantType == Variant.Type.Vector3I) return op1.AsVector3I() - op2.AsVector3I();
        if (op1.VariantType == Variant.Type.Transform2D) return op1.AsTransform2D().Inverse() * op2.AsTransform2D();
        if (op1.VariantType == Variant.Type.Vector4) return op1.AsVector4() - op2.AsVector4(); 
        if (op1.VariantType == Variant.Type.Vector4I) return op1.AsVector4I() - op2.AsVector4I(); 
        if (op1.VariantType == Variant.Type.Plane) {
            var op1Plane = op1.AsPlane();
            var op2Plane = op2.AsPlane();
            return new Plane(op1Plane.Normal - op2Plane.Normal, op1Plane.D - op2Plane.D);
        } 
        if (op1.VariantType == Variant.Type.Quaternion) return op1.AsQuaternion() - op2.AsQuaternion();
        if (op1.VariantType == Variant.Type.Aabb) {
            var op1Aabb = op1.AsAabb();
            var op2Aabb = op2.AsAabb();
            return new Aabb(op1Aabb.Position - op2Aabb.Position, op1Aabb.Size - op2Aabb.Size);
        } 
        if (op1.VariantType == Variant.Type.Transform3D) return op1.AsTransform3D().Inverse() * op2.AsTransform3D(); 
        if (op1.VariantType == Variant.Type.Color) return op1.AsColor() - op2.AsColor();
        throw new Exception($"Subtract Variant: {op1.VariantType} type not implemented");
    }

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