using System.Collections.Generic;
using Betauer.Collections;
using Betauer.TestRunner;
using Godot;
using NUnit.Framework;

namespace Betauer.Tests {
    [TestFixture]
    public class CollectionsTest {
        [SetUp]
        public void Setup() {
        }

        [Test]
        public void Empty() {
            int x = 0;
            new FastUnsafeLinkedList<int>().ForEach(i => x++);
            Assert.That(x, Is.EqualTo(0));

            Assert.That(new FastUnsafeLinkedList<int>().Count, Is.EqualTo(0));
            Assert.That(new FastUnsafeLinkedList<string>().Find(i => true), Is.Null);

            FastUnsafeLinkedList<int> p = new FastUnsafeLinkedList<int>();
            p.AddStart(1);
            p.AddStart(2);
            AssertLoop(p, new int[] { 2, 1 });

        }

        [Test]
        public void FastUnsafeLinkedListForEach() {
            FastUnsafeLinkedList<int> p = new FastUnsafeLinkedList<int>();
            Assert.That(p.ToArray(), Is.EqualTo(new int[] { }));
            Assert.That(p.Count, Is.EqualTo(0));

            AssertLoop(p, new int[] { });

            p.Add(1);
            AssertLoop(p, new[] { 1 });

            p.Add(2);
            p.Add(3);

            AssertLoop(p, new[] { 1, 2, 3 });

            p.RemoveEnd();
            AssertLoop(p, new[] { 1, 2 });

            p.RemoveEnd();
            p.RemoveEnd();
            AssertLoop(p, new int[] { });
        }

        private void AssertLoop(FastUnsafeLinkedList<int> p, int[] to) {
            Assert.That(p.ToArray(), Is.EqualTo(to));
            Assert.That(p.Count, Is.EqualTo(to.Length));
            List<int> list1 = new List<int>();
            foreach (var i in p) {
                list1.Add(i);
            }
            Assert.That(list1.ToArray(), Is.EqualTo(to));


            Assert.That(new FastUnsafeLinkedList<int>(new List<int>(to)).ToArray(), Is.EqualTo(to));
        }

        [Test]
        public void FastUnsafeLinkedList() {
            FastUnsafeLinkedList<int> p = new FastUnsafeLinkedList<int>();
            AssertLoop(p, new int[] { });

            p.Add(1);
            AssertLoop(p, new int[] { 1 });

            p.Add(2);
            AssertLoop(p, new int[] { 1, 2 });

            p.Add(3);
            AssertLoop(p, new int[] { 1, 2, 3 });

            p.RemoveEnd();
            p.RemoveEnd();
            p.RemoveEnd();

            AssertLoop(p, new int[] { });

            p.Add(1);
            p.Add(2);
            p.Add(3);
            p.RemoveStart();
            p.RemoveStart();
            p.RemoveStart();

            AssertLoop(p, new int[] { });

            p.Add(1);
            p.Add(2);
            p.AddStart(1);
            p.AddStart(0);
            p.Add(3);

            AssertLoop(p, new int[] { 0, 1, 1, 2, 3 });

            p.Add(10);
            p.RemoveStart();
            p.RemoveEnd();

            AssertLoop(p, new int[] { 1, 1, 2, 3 });

            p.Clear();

            p.AddStart(1);
            AssertLoop(p, new int[] { 1 });

            p.AddStart(2);
            AssertLoop(p, new int[] { 2, 1 });

            p.AddStart(3);
            AssertLoop(p, new int[] { 3, 2, 1 });

            p.RemoveEnd();
            p.RemoveEnd();
            p.RemoveEnd();

            AssertLoop(p, new int[] { });

            p.Add(1);
            p.Add(2);
            p.Add(3);
            p.RemoveStart();
            p.RemoveStart();
            p.RemoveStart();

            AssertLoop(p, new int[] { });

            p.Add(1);
            p.Add(2);
            p.AddStart(1);
            p.AddStart(0);
            p.Add(3);

            AssertLoop(p, new int[] { 0, 1, 1, 2, 3 });

            p.Add(3);
            p.RemoveStart();
            p.RemoveEnd();

            AssertLoop(p, new int[] { 1, 1, 2, 3 });
        }
    }
}