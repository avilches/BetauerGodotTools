using System;
using System.Collections.Generic;
using NUnit.Framework;
using Tools;

namespace Veronenger.Tests {
    [TestFixture]
    public class CollectionsTest {
        [SetUp]
        public void Setup() {
        }

        [Test]
        public void SimpleLinkedListForEach() {
            SimpleLinkedList<int> p = new SimpleLinkedList<int>();
            Assert.That(p.ToArray(), Is.EqualTo(new int[] { }));
            Assert.That(p.Count, Is.EqualTo(0));

            foreach (var i in p) {
                Assert.That(false);
            }

            p.Add(1);

            List<int> list = new List<int>();
            foreach (var i in p) {
                list.Add(i);
            }
            Assert.That(list.ToArray(), Is.EqualTo(new int[] { 1 }));

            p.Add(2);
            p.Add(3);

            list.Clear();
            foreach (var i in p) {
                list.Add(i);
            }
            Assert.That(list.ToArray(), Is.EqualTo(new int[] { 1, 2, 3 }));
        }

        [Test]
        public void SimpleLinkedList() {
            SimpleLinkedList<int> p = new SimpleLinkedList<int>();
            Assert.That(p.ToArray(), Is.EqualTo(new int[] {}));
            Assert.That(p.Count, Is.EqualTo(0));

            p.Add(1);
            Assert.That(p.ToArray(), Is.EqualTo(new int[] { 1 }));
            Assert.That(p.Count, Is.EqualTo(1));

            p.Add(2);
            Assert.That(p.ToArray(), Is.EqualTo(new int[] { 1, 2 }));
            Assert.That(p.Count, Is.EqualTo(2));

            p.Add(3);
            Assert.That(p.ToArray(), Is.EqualTo(new int[] { 1, 2, 3 }));
            Assert.That(p.Count, Is.EqualTo(3));

            p.RemoveEnd();
            p.RemoveEnd();
            p.RemoveEnd();

            Assert.That(p.ToArray(), Is.EqualTo(new int[] { }));
            Assert.That(p.Count, Is.EqualTo(0));

            p.Add(1);
            p.Add(2);
            p.Add(3);
            p.RemoveStart();
            p.RemoveStart();
            p.RemoveStart();

            Assert.That(p.ToArray(), Is.EqualTo(new int[] { }));
            Assert.That(p.Count, Is.EqualTo(0));

            p.Add(1);
            p.Add(2);
            p.AddStart(1);
            p.AddStart(0);
            p.Add(3);

            Assert.That(p.ToArray(), Is.EqualTo(new int[] { 0, 1, 1, 2, 3 }));
            Assert.That(p.Count, Is.EqualTo(5));

            p.Add(3);
            p.RemoveStart();
            p.RemoveEnd();

            Assert.That(p.ToArray(), Is.EqualTo(new int[] { 1, 1, 2, 3 }));
            Assert.That(p.Count, Is.EqualTo(4));

            p.Clear();


            p.Add(1);
            Assert.That(p.ToArray(), Is.EqualTo(new int[] { 1 }));
            Assert.That(p.Count, Is.EqualTo(1));

            p.Add(2);
            Assert.That(p.ToArray(), Is.EqualTo(new int[] { 1, 2 }));
            Assert.That(p.Count, Is.EqualTo(2));

            p.Add(3);
            Assert.That(p.ToArray(), Is.EqualTo(new int[] { 1, 2, 3 }));
            Assert.That(p.Count, Is.EqualTo(3));

            p.RemoveEnd();
            p.RemoveEnd();
            p.RemoveEnd();

            Assert.That(p.ToArray(), Is.EqualTo(new int[] { }));
            Assert.That(p.Count, Is.EqualTo(0));

            p.Add(1);
            p.Add(2);
            p.Add(3);
            p.RemoveStart();
            p.RemoveStart();
            p.RemoveStart();

            Assert.That(p.ToArray(), Is.EqualTo(new int[] { }));
            Assert.That(p.Count, Is.EqualTo(0));

            p.Add(1);
            p.Add(2);
            p.AddStart(1);
            p.AddStart(0);
            p.Add(3);

            Assert.That(p.ToArray(), Is.EqualTo(new int[] { 0, 1, 1, 2, 3 }));
            Assert.That(p.Count, Is.EqualTo(5));

            p.Add(3);
            p.RemoveStart();
            p.RemoveEnd();

            Assert.That(p.ToArray(), Is.EqualTo(new int[] { 1, 1, 2, 3 }));
            Assert.That(p.Count, Is.EqualTo(4));


        }
    }
}