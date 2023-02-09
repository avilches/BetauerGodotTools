using Betauer.TestRunner;
using Godot;
using NUnit.Framework;
using Vector2 = Godot.Vector2;
using Vector3 = Godot.Vector3;

namespace Betauer.Core.Tests; 

[TestFixture]
public class VariantHelperTests {
    [Test]
    public void SumVariantInt() {
        Assert.That(VariantHelper.Add(1, 2).As<int>(), Is.EqualTo(3));
        Assert.That(VariantHelper.Subtract(1, 2).As<int>(), Is.EqualTo(-1));
    }

    [Test]
    public void SumVariantFloat() {
        Assert.That(VariantHelper.Add(1f, 2f).As<float>(), Is.EqualTo(3f).Within(0.0000001f));
        Assert.That(VariantHelper.Subtract(1f, 2f).As<float>(), Is.EqualTo(-1f).Within(0.0000001f));
    }

    [Test]
    public void SumVariantDouble() {
        Assert.That(VariantHelper.Add(1d, 2d).As<float>(), Is.EqualTo(3d).Within(0.0000001f));
        Assert.That(VariantHelper.Subtract(1d, 2d).As<float>(), Is.EqualTo(-1d).Within(0.0000001f));
    }

    [Test]
    public void SumVariantVector2() {
        Assert.That(VariantHelper.Add(Vector2.One, Vector2.One).As<Vector2>(), Is.EqualTo(new Vector2(2f, 2f)));
        Assert.That(VariantHelper.Subtract(Vector2.One, Vector2.One).As<Vector2>(), Is.EqualTo(Vector2.Zero));
    }

    [Test]
    public void SumVariantVector3() {
        Assert.That(VariantHelper.Add(Vector3.One, Vector3.One).As<Vector3>(), Is.EqualTo(new Vector3(2f, 2f, 2f)));
        Assert.That(VariantHelper.Subtract(Vector3.One, Vector3.One).As<Vector3>(), Is.EqualTo(Vector3.Zero));
    }

    [Test]
    public void SumVariantColor() {
        Assert.That(VariantHelper.Add(
                new Color(0.1f, 0.2f, 0.3f, 0.4f), new Color(0.1f, 0.2f, 0.3f, 0.4f)).As<Color>(),
            Is.EqualTo(new Color(0.2f, 0.4f, 0.6f, 0.8f)));
        Assert.That(VariantHelper.Subtract(
                new Color(0.1f, 0.1f, 0.1f, 0.1f), new Color(0.1f, 0.1f, 0.1f, 0.1f)).As<Color>(),
            Is.EqualTo(new Color(0f, 0f, 0f, 0f)));
    }

    [Test]
    public void CreateFromAndConvertToTest() {
        AssertVariant(true);
        AssertVariant(false);

        AssertVariant(char.MaxValue);
        AssertVariant(char.MinValue);

        AssertVariant(sbyte.MaxValue);
        AssertVariant(sbyte.MinValue);

        AssertVariant(short.MaxValue);
        AssertVariant(short.MinValue);

        AssertVariant(int.MaxValue);
        AssertVariant(int.MinValue);

        AssertVariant(long.MaxValue);
        AssertVariant(long.MinValue);

        AssertVariant(byte.MaxValue);
        AssertVariant(byte.MinValue);

        AssertVariant(ushort.MaxValue);
        AssertVariant(ushort.MinValue);

        AssertVariant(uint.MaxValue);
        AssertVariant(uint.MinValue);

        AssertVariant((ulong)long.MaxValue);
        AssertVariant(ulong.MinValue);

        AssertVariant(float.MaxValue);
        AssertVariant(float.MinValue);

        AssertVariant(double.MaxValue);
        AssertVariant(double.MinValue);

        AssertVariant(new Vector2(2, 3));
        AssertVariant(new Vector2i(2, 3));
        AssertVariant(new Rect2(1, 2, 3, 4));
        AssertVariant(new Rect2i(1, 2, 3, 4));
        AssertVariant(new Transform2D(1, 2, 3, 4, 5, 6));
        AssertVariant(new Vector3(1, 2, 3));
        AssertVariant(new Vector3i(1, 2, 3));
        AssertVariant(new Vector4(1, 2, 3, 4));
        AssertVariant(new Vector4i(1, 2, 3, 4));
        AssertVariant(new Basis(new Vector3(1, 2, 3), 2));
        AssertVariant(new Quaternion(1, 2, 3, 4));
        AssertVariant(new Transform3D(new Basis(new Vector3(1, 2, 3), 2), new Vector3(1, 2, 3)));
        // AssertVariant(new Projection(new Transform3D(new Basis(new Vector3(1, 2, 3), 2), new Vector3(1, 2, 3))));
        AssertVariant(new AABB(1, 2, 3, 4, 5, 6));
        AssertVariant(new Color(1, 2, 3, 4));
        AssertVariant(new Plane(1, 2, 3, 4));
        
        Assert.That(VariantHelper.ConvertTo(new Variant()), Is.Null);
    }

    public static void AssertVariant<T>(T value) {
        AssertVariantFromGodotSharp(value);
        AssertVariantFromHelperObject(value);
        AssertVariantFromHelperTyped(value);
    }

    public static void AssertVariantFromGodotSharp<T>(T value) {
        var variant = Variant.From(value);

        T asT = variant.As<T>();
        Assert.That(value, Is.EqualTo(asT));
        
        T t = VariantHelper.ConvertTo<T>(variant);
        Assert.That(value, Is.EqualTo(t));

        object o = VariantHelper.ConvertTo(variant);
        Assert.That(value, Is.EqualTo(o));

    }

    public static void AssertVariantFromHelperTyped<T>(T value) {
        var variant = VariantHelper.CreateFrom(value);

        T asT = variant.As<T>();
        Assert.That(value, Is.EqualTo(asT));
        
        T t = VariantHelper.ConvertTo<T>(variant);
        Assert.That(value, Is.EqualTo(t));

        object o = VariantHelper.ConvertTo(variant);
        Assert.That(value, Is.EqualTo(o));

    }

    public static void AssertVariantFromHelperObject<T>(T value) {
        var variant = VariantHelper.CreateFrom<object>(value);

        T asT = variant.As<T>();
        Assert.That(value, Is.EqualTo(asT));
        
        T t = VariantHelper.ConvertTo<T>(variant);
        Assert.That(value, Is.EqualTo(t));

        object o = VariantHelper.ConvertTo(variant);
        Assert.That(value, Is.EqualTo(o));

    }
}