using System;
using System.Collections.Generic;
using System.Linq;
using Betauer.TestRunner;

namespace Betauer.Core.Tests;

using NUnit.Framework;

[TestFixture]
public class AttributesContainerTests {
    private record TestObject(string name) {
        public string Name { get; } = name;
    }

    [Test]
    public void Cleanup_RemovesAttributesOfCollectedObjects() {
        // Arrange
        var container = AttributesContainer.Singleton;
        CreateOrphanObjectWithAttributes();

        ForceGc();
        container.Cleanup();

        // Assert
        Assert.That(container.Attributes, Is.Empty, "Attributes should be empty after cleanup");
        Assert.That(container.References, Is.Empty, "References should be empty after cleanup");
        return;

        void CreateOrphanObjectWithAttributes() {
            var obj = new TestObject("test");
            obj.Attributes().Set("key1", "value1");
            obj.Attributes().Set("key2", "value2");

            // Verificamos que se añadieron los atributos
            Assert.That(AttributesContainer.Singleton.Attributes.Count, Is.EqualTo(2));
            Assert.That(AttributesContainer.Singleton.References.Count, Is.EqualTo(1));

            // Eliminamos todas las referencias al objeto
            obj = null;
        }
    }

    [Test]
    public void SetAttribute_FailsNoReferenceTypes() {
        Assert.Throws<ArgumentException>(() => AttributesContainer.Singleton.SetAttribute((object)12, "key1", "value1"));
        Assert.Throws<ArgumentException>(() => AttributesContainer.Singleton.SetAttribute("a", "key1", "value1"));
    }

    [Test]
    public void ClearAttributes_RemovesReferenceWhenEmpty() {
        // Arrange
        var container = AttributesContainer.Singleton;
        CreateObjectWithAttributesAndRemove();

        ForceGc();
        container.Cleanup();

        // Assert
        Assert.That(container.Attributes.Count, Is.EqualTo(0), "Attributes should be empty");
        Assert.That(container.References.Count, Is.EqualTo(0), "References should be empty");
        return;

        void CreateObjectWithAttributesAndRemove() {
            var obj = new TestObject("test");
            obj.Attributes().Set("key1", "value1");
            obj.Attributes().Set("key2", "value2");

            // Verificamos que se añadieron los atributos
            Assert.That(AttributesContainer.Singleton.Attributes.Count, Is.EqualTo(2));
            Assert.That(AttributesContainer.Singleton.References.Count, Is.EqualTo(1));

            obj.Attributes().Remove("key1");
            obj.Attributes().Remove("key2");

            // Verificamos que se borraron los atributos
            Assert.That(AttributesContainer.Singleton.Attributes.Count, Is.EqualTo(0));
            Assert.That(AttributesContainer.Singleton.References.Count, Is.EqualTo(1));

            // Eliminamos todas las referencias al objeto
            obj = null;
        }
    }

    [Test]
    public void SetAttribute_ValidatesTypes() {
        var container = AttributesContainer.Singleton;
        var validObject = new TestObject("test");

        // Test valid object (should work)
        Assert.DoesNotThrow(() => container.SetAttribute(validObject, "key", "value"));

        // Test value types (should fail)
        Assert.Throws<ArgumentException>(() => container.SetAttribute((object)null, "key", "value"),
            "Should reject boxed int");
        Assert.Throws<ArgumentException>(() => container.SetAttribute((object)12, "key", "value"),
            "Should reject boxed int");
        Assert.Throws<ArgumentException>(() => container.SetAttribute((object)true, "key", "value"),
            "Should reject boxed bool");
        Assert.Throws<ArgumentException>(() => container.SetAttribute((object)3.14, "key", "value"),
            "Should reject boxed double");

        // Test string (should fail)
        Assert.Throws<ArgumentException>(() => container.SetAttribute("test string", "key", "value"),
            "Should reject string");
        Assert.Throws<ArgumentException>(() => container.SetAttribute((object)"test string", "key", "value"),
            "Should reject boxed string");

        // Test delegates (should fail)
        Action action = () => Console.WriteLine("test");
        Func<int> func = () => 42;
        Assert.Throws<ArgumentException>(() => container.SetAttribute(action, "key", "value"),
            "Should reject Action delegate");
        Assert.Throws<ArgumentException>(() => container.SetAttribute(func, "key", "value"),
            "Should reject Func delegate");
        Assert.Throws<ArgumentException>(() => container.SetAttribute((Delegate)action, "key", "value"),
            "Should reject explicit Delegate");
        Assert.Throws<ArgumentException>(() => container.SetAttribute((object)action, "key", "value"),
            "Should reject boxed delegate");
    }

    [Test]
    public void GetAttribute_ValidatesTypes() {
        var container = AttributesContainer.Singleton;
        var validObject = new TestObject("test");

        // Test valid object (should work)
        Assert.DoesNotThrow(() => container.GetAttribute(validObject, "key"));

        // Test value types (should fail)
        Assert.Throws<ArgumentException>(() => container.GetAttribute((object)null, "key"),
            "Should reject boxed int");
        Assert.Throws<ArgumentException>(() => container.GetAttribute((object)12, "key"),
            "Should reject boxed int");
        Assert.Throws<ArgumentException>(() => container.GetAttribute((object)true, "key"),
            "Should reject boxed bool");
        Assert.Throws<ArgumentException>(() => container.GetAttribute((object)3.14, "key"),
            "Should reject boxed double");

        // Test string (should fail)
        Assert.Throws<ArgumentException>(() => container.GetAttribute("test string", "key"),
            "Should reject string");
        Assert.Throws<ArgumentException>(() => container.GetAttribute((object)"test string", "key"),
            "Should reject boxed string");

        // Test delegates (should fail)
        Action action = () => Console.WriteLine("test");
        Func<int> func = () => 42;
        Assert.Throws<ArgumentException>(() => container.GetAttribute(action, "key"),
            "Should reject Action delegate");
        Assert.Throws<ArgumentException>(() => container.GetAttribute(func, "key"),
            "Should reject Func delegate");
        Assert.Throws<ArgumentException>(() => container.GetAttribute((Delegate)action, "key"),
            "Should reject explicit Delegate");
        Assert.Throws<ArgumentException>(() => container.GetAttribute((object)action, "key"),
            "Should reject boxed delegate");
    }

    [Test]
    public void OtherMethods_ValidateTypes() {
        var container = AttributesContainer.Singleton;
        var invalidString = "test string";
        Action invalidDelegate = () => Console.WriteLine("test");
        object invalidValueType = 42;

        // Test HasAttribute
        Assert.Throws<ArgumentException>(() => container.HasAttribute((object)null, "key"));
        Assert.Throws<ArgumentException>(() => container.HasAttribute(invalidString, "key"));
        Assert.Throws<ArgumentException>(() => container.HasAttribute(invalidDelegate, "key"));
        Assert.Throws<ArgumentException>(() => container.HasAttribute(invalidValueType, "key"));

        // Test RemoveAttribute
        Assert.Throws<ArgumentException>(() => container.RemoveAttribute((object)null, "key"));
        Assert.Throws<ArgumentException>(() => container.RemoveAttribute(invalidString, "key"));
        Assert.Throws<ArgumentException>(() => container.RemoveAttribute(invalidDelegate, "key"));
        Assert.Throws<ArgumentException>(() => container.RemoveAttribute(invalidValueType, "key"));

        // Test GetAttributes
        Assert.Throws<ArgumentException>(() => container.GetAttributes((object)null).ToList());
        Assert.Throws<ArgumentException>(() => container.GetAttributes(invalidString).ToList());
        Assert.Throws<ArgumentException>(() => container.GetAttributes(invalidDelegate).ToList());
        Assert.Throws<ArgumentException>(() => container.GetAttributes(invalidValueType).ToList());

        // Test ClearAttributes
        Assert.Throws<ArgumentException>(() => container.ClearAttributes((object)null));
        Assert.Throws<ArgumentException>(() => container.ClearAttributes(invalidString));
        Assert.Throws<ArgumentException>(() => container.ClearAttributes(invalidDelegate));
        Assert.Throws<ArgumentException>(() => container.ClearAttributes(invalidValueType));
    }

    [Test]
    public void ArraysAreValid() {
        var container = AttributesContainer.Singleton;
        var array = new int[] { 1, 2, 3 };
        var objectArray = new object[] { "test", 1, true };

        // Arrays should work fine as they are proper reference types
        Assert.DoesNotThrow(() => container.SetAttribute(array, "key", "value"));
        Assert.DoesNotThrow(() => container.SetAttribute(objectArray, "key", "value"));
    }

    [Test]
    public void AutoCleanup() {
        // Arrange
        var container = AttributesContainer.Singleton;
        CreateOrphanObjectWithAttributes();

        ForceGc();
        for (var n = 0; n < AttributesContainer.CleanupInterval + 1; n++) {
            container.RemoveAttribute(this, "key1");
        }

        // Assert
        Assert.That(container.Attributes, Is.Empty, "Attributes should be empty after cleanup");
        Assert.That(container.References, Is.Empty, "References should be empty after cleanup");
        return;

        void CreateOrphanObjectWithAttributes() {
            var obj = new TestObject("test");

            for (var n = 0; n < AttributesContainer.MaxSize; n++) {
                obj.Attributes().Set("key"+n, "value1");
            }

            // Verificamos que se añadieron los atributos
            Assert.That(AttributesContainer.Singleton.Attributes.Count, Is.EqualTo(AttributesContainer.MaxSize));
            Assert.That(AttributesContainer.Singleton.References.Count, Is.EqualTo(1));

            // Eliminamos todas las referencias al objeto
            obj = null;
        }
    }

    [TearDown]
    public void Cleanup() {
        AttributesContainer.Singleton.Attributes.Clear();
        AttributesContainer.Singleton.References.Clear();
    }

    private static void ForceGc() {
        // Force multiple GC collections to ensure cleanup
        for (int i = 0; i < 3; i++) {
            GC.Collect(GC.MaxGeneration, GCCollectionMode.Forced, true);
            GC.WaitForPendingFinalizers();
        }
    }
}