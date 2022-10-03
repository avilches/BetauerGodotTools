using System.Collections.Generic;
using System.Linq;
using Betauer.TestRunner;
using NUnit.Framework;

namespace Betauer.Tests {
    [TestFixture]
    public class EnumDictionaryTests {
        public enum State {
            Start = 2,
            End = 10
        }

        [Test]
        public void BasicTest() {
            var list = new EnumDictionary<State, int>();

            Assert.That(list[State.Start], Is.EqualTo(default(int)));
            Assert.That(list[State.End], Is.EqualTo(default(int)));
            Assert.That(list.Count, Is.EqualTo(2));
            Assert.That(list.ContainsKey(State.Start), Is.True);
            Assert.That(list.ContainsKey(State.End), Is.True);

            list[State.Start] = 9;
            list[State.Start] = 10;
            list.Add(State.End, 19);
            list.Add(new KeyValuePair<State, int>(State.End, 20));
            Assert.That(list[State.Start], Is.EqualTo(10));
            Assert.That(list.Contains(new KeyValuePair<State, int>(State.Start, 10)), Is.True);
            Assert.That(list.Contains(new KeyValuePair<State, int>(State.Start, 100)), Is.False);

            Assert.That(list[State.End], Is.EqualTo(20));
            Assert.That(list.Contains(new KeyValuePair<State, int>(State.End, 20)), Is.True);
            Assert.That(list.Contains(new KeyValuePair<State, int>(State.End, 200)), Is.False);

            Assert.That(list.Keys.ToArray(), Is.EqualTo(new[] { State.Start, State.End }));
            Assert.That(list.Values.ToArray(), Is.EqualTo(new[] { 10, 20 }));

            var x = 0;
            foreach (var pair in list) {
                if (x == 0) {
                    Assert.That(pair.Key, Is.EqualTo(State.Start));
                    Assert.That(pair.Value, Is.EqualTo(10));
                } else if (x == 1) {
                    Assert.That(pair.Key, Is.EqualTo(State.End));
                    Assert.That(pair.Value, Is.EqualTo(20));
                }
                x++;
            }

            list.Remove(State.Start);
            Assert.That(list[State.Start], Is.EqualTo(default(int)));

            list.Clear();
            Assert.That(list[State.End], Is.EqualTo(default(int)));

            list.Fill(e => 8);
            Assert.That(list.Values.ToArray(), Is.EqualTo(new[] { 8, 8 }));

        }
        
        [Test]
        public void ToIntTest() {
            var list = new EnumDictionary<State, int>();
            list.Fill(e => e.ToInt());
            Assert.That(list.Values.ToArray(), Is.EqualTo(new[] { 2, 10 }));
        }
    }
}