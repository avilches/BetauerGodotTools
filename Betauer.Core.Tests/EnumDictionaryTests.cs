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
        public void BasicNullableTest() {
            var list = new EnumDictionary<State, string>();

            Assert.That(list.Count, Is.EqualTo(2));

            Assert.That(list[State.Start], Is.Null);
            Assert.That(list[State.End], Is.Null);
            Assert.That(list.Contains(new KeyValuePair<State, string>(State.Start, null)), Is.True);
            Assert.That(list.Contains(new KeyValuePair<State, string>(State.End, null)), Is.True);

            list[State.Start] = "Z";
            Assert.That(list.ContainsKey(State.Start), Is.True);
            Assert.That(list[State.Start], Is.EqualTo("Z"));
            Assert.That(list.Contains(new KeyValuePair<State, string>(State.Start, "Z")), Is.True);
            Assert.That(list.Contains(new KeyValuePair<State, string>(State.Start, "x")), Is.False);

            list.Add(new KeyValuePair<State, string>(State.Start, "A"));
            Assert.That(list.ContainsKey(State.Start), Is.True);
            Assert.That(list[State.Start], Is.EqualTo("A"));
            Assert.That(list.Contains(new KeyValuePair<State, string>(State.Start, "A")), Is.True);
            Assert.That(list.Contains(new KeyValuePair<State, string>(State.Start, "x")), Is.False);

            list.Add(State.End, "B");
            Assert.That(list.ContainsKey(State.End), Is.True);
            Assert.That(list[State.End], Is.EqualTo("B"));
            Assert.That(list.Contains(new KeyValuePair<State, string>(State.End, "B")), Is.True);
            Assert.That(list.Contains(new KeyValuePair<State, string>(State.End, "x")), Is.False);

            Assert.That(list.Keys.ToArray(), Is.EqualTo(new[] { State.Start, State.End }));
            Assert.That(list.Values.ToArray(), Is.EqualTo(new[] { "A", "B"}));
            
            var x = 0;
            foreach (var pair in list) {
                if (x == 0) {
                    Assert.That(pair.Key, Is.EqualTo(State.Start));
                    Assert.That(pair.Value, Is.EqualTo("A"));
                } else if (x == 1) {
                    Assert.That(pair.Key, Is.EqualTo(State.End));
                    Assert.That(pair.Value, Is.EqualTo("B"));
                }
                x++;
            }

            list.Remove(State.Start);
            Assert.That(list.ContainsKey(State.Start), Is.False);
            Assert.That(list[State.Start], Is.EqualTo(null));

            list.Clear();
            Assert.That(list.ContainsKey(State.End), Is.False);
            Assert.That(list[State.End], Is.EqualTo(null));

            list.Fill(e => "");
            Assert.That(list.Values.ToArray(), Is.EqualTo(new[] { "", "" }));
        }

        [Test]
        public void ToIntTest() {
            var list = new EnumDictionary<State, string>();
            list.Fill(e => e.ToInt().ToString());
            Assert.That(list.Values.ToArray(), Is.EqualTo(new[] { "2", "10" }));
        }
    }
}