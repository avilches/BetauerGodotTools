using NUnit.Framework;
using System;
using Betauer.TestRunner;

namespace Betauer.Core.Tests;

[TestFixture]
public class IndexMinPriorityQueueTests {
    private IndexMinPriorityQueue<int> _queue;
    private const int MaxSize = 10;

    [SetUp]
    public void Setup() {
        _queue = new IndexMinPriorityQueue<int>(MaxSize);
    }

    [Test]
    public void NewQueue_IsEmpty() {
        Assert.That(_queue.IsEmpty(), Is.True);
        Assert.That(_queue.Size, Is.EqualTo(0));
    }

    [Test]
    public void Insert_SingleElement_QueueNotEmpty() {
        _queue.Insert(0, 42);
        
        Assert.That(_queue.IsEmpty(), Is.False);
        Assert.That(_queue.Size, Is.EqualTo(1));
        Assert.That(_queue.Contains(0), Is.True);
        Assert.That(_queue.KeyAt(0), Is.EqualTo(42));
    }

    [Test]
    public void Insert_MultipleElements_CorrectMinimum() {
        _queue.Insert(0, 3);
        _queue.Insert(1, 5);
        _queue.Insert(2, 1);
        _queue.Insert(3, 4);

        Assert.That(_queue.Size, Is.EqualTo(4));
        Assert.That(_queue.MinKey(), Is.EqualTo(1));
        Assert.That(_queue.MinIndex(), Is.EqualTo(2));
    }

    [Test]
    public void DeleteMin_RemovesMinimumElement() {
        _queue.Insert(0, 3);
        _queue.Insert(1, 5);
        _queue.Insert(2, 1);
        _queue.Insert(3, 4);

        int minIndex = _queue.DeleteMin();
        
        Assert.That(minIndex, Is.EqualTo(2));
        Assert.That(_queue.Size, Is.EqualTo(3));
        Assert.That(_queue.Contains(2), Is.False);
        Assert.That(_queue.MinKey(), Is.EqualTo(3));
    }

    [Test]
    public void ChangeKey_UpdatesKeyCorrectly() {
        _queue.Insert(0, 3);
        _queue.Insert(1, 5);
        _queue.Insert(2, 1);

        _queue.ChangeKey(1, 0); // Make index 1 the new minimum

        Assert.That(_queue.MinIndex(), Is.EqualTo(1));
        Assert.That(_queue.MinKey(), Is.EqualTo(0));
    }

    [Test]
    public void DecreaseKey_UpdatesPriorityCorrectly() {
        _queue.Insert(0, 3);
        _queue.Insert(1, 5);
        _queue.Insert(2, 4);

        _queue.DecreaseKey(1, 2); // Decrease 5 to 2

        Assert.That(_queue.MinIndex(), Is.EqualTo(1));
        Assert.That(_queue.MinKey(), Is.EqualTo(2));
    }

    [Test]
    public void IncreaseKey_UpdatesPriorityCorrectly() {
        _queue.Insert(0, 3);
        _queue.Insert(1, 1);
        _queue.Insert(2, 4);

        _queue.IncreaseKey(1, 5); // Increase 1 to 5

        Assert.That(_queue.MinIndex(), Is.EqualTo(0));
        Assert.That(_queue.MinKey(), Is.EqualTo(3));
    }

    [Test]
    public void Delete_RemovesElementCorrectly() {
        _queue.Insert(0, 3);
        _queue.Insert(1, 1);
        _queue.Insert(2, 4);

        _queue.Delete(1);

        Assert.That(_queue.Contains(1), Is.False);
        Assert.That(_queue.Size, Is.EqualTo(2));
        Assert.That(_queue.MinKey(), Is.EqualTo(3));
    }

    [Test]
    public void DeleteAll_QueueBecomesEmpty() {
        _queue.Insert(0, 3);
        _queue.Insert(1, 1);
        _queue.Insert(2, 4);

        _queue.Delete(0);
        _queue.Delete(1);
        _queue.Delete(2);

        Assert.That(_queue.IsEmpty(), Is.True);
        Assert.That(_queue.Size, Is.EqualTo(0));
    }

    [Test]
    public void Insert_WithString_WorksCorrectly() {
        var stringQueue = new IndexMinPriorityQueue<string>(MaxSize);
        
        stringQueue.Insert(0, "banana");
        stringQueue.Insert(1, "apple");
        stringQueue.Insert(2, "cherry");

        Assert.That(stringQueue.MinKey(), Is.EqualTo("apple"));
        Assert.That(stringQueue.MinIndex(), Is.EqualTo(1));
    }

    [Test]
    public void Contains_ReturnsFalseForNonExistentIndex() {
        _queue.Insert(0, 1);
        
        Assert.That(_queue.Contains(1), Is.False);
        Assert.That(_queue.Contains(5), Is.False);
    }

    [Test]
    public void Operations_MaintainHeapInvariant() {
        // Insert elements in non-sorted order
        _queue.Insert(0, 5);
        _queue.Insert(1, 3);
        _queue.Insert(2, 7);
        _queue.Insert(3, 1);
        _queue.Insert(4, 4);

        // Verify min element is extracted in order
        Assert.That(_queue.DeleteMin(), Is.EqualTo(3)); // Index with value 1
        Assert.That(_queue.DeleteMin(), Is.EqualTo(1)); // Index with value 3
        Assert.That(_queue.DeleteMin(), Is.EqualTo(4)); // Index with value 4
        Assert.That(_queue.DeleteMin(), Is.EqualTo(0)); // Index with value 5
        Assert.That(_queue.DeleteMin(), Is.EqualTo(2)); // Index with value 7
        
        Assert.That(_queue.IsEmpty(), Is.True);
    }
}