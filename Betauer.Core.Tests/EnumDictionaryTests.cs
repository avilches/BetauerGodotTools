using System.Collections.Generic;
using System.Linq;
using Betauer.TestRunner;
using NUnit.Framework;

namespace Betauer.Tests {
    [TestFixture]
    public class EnumDictionaryTests {
        public enum VariableState {
            Start = 2,
            End = 10
        }

        public enum FixedState {
            Start = 0,
            End = 1
        }

        [Test]
        public void VariableEnumDictionaryTest() {
            var list = EnumDictionary<VariableState, string>.Create();
            Assert.That(list, Is.TypeOf<VariableEnumDictionary<VariableState, string>>());

            Assert.That(list.Count, Is.EqualTo(2));

            Assert.That(list[VariableState.Start], Is.Null);
            Assert.That(list[VariableState.End], Is.Null);
            Assert.That(list.Contains(new KeyValuePair<VariableState, string>(VariableState.Start, null)), Is.True);
            Assert.That(list.Contains(new KeyValuePair<VariableState, string>(VariableState.End, null)), Is.True);

            list[VariableState.Start] = "Z";
            Assert.That(list.ContainsKey(VariableState.Start), Is.True);
            Assert.That(list[VariableState.Start], Is.EqualTo("Z"));
            Assert.That(list.Contains(new KeyValuePair<VariableState, string>(VariableState.Start, "Z")), Is.True);
            Assert.That(list.Contains(new KeyValuePair<VariableState, string>(VariableState.Start, "x")), Is.False);

            list.Add(new KeyValuePair<VariableState, string>(VariableState.Start, "A"));
            Assert.That(list.ContainsKey(VariableState.Start), Is.True);
            Assert.That(list[VariableState.Start], Is.EqualTo("A"));
            Assert.That(list.Contains(new KeyValuePair<VariableState, string>(VariableState.Start, "A")), Is.True);
            Assert.That(list.Contains(new KeyValuePair<VariableState, string>(VariableState.Start, "x")), Is.False);

            list.Add(VariableState.End, "B");
            Assert.That(list.ContainsKey(VariableState.End), Is.True);
            Assert.That(list[VariableState.End], Is.EqualTo("B"));
            Assert.That(list.Contains(new KeyValuePair<VariableState, string>(VariableState.End, "B")), Is.True);
            Assert.That(list.Contains(new KeyValuePair<VariableState, string>(VariableState.End, "x")), Is.False);

            Assert.That(list.Keys.ToArray(), Is.EqualTo(new[] { VariableState.Start, VariableState.End }));
            Assert.That(list.Values.ToArray(), Is.EqualTo(new[] { "A", "B"}));
            
            var x = 0;
            foreach (var pair in list) {
                if (x == 0) {
                    Assert.That(pair.Key, Is.EqualTo(VariableState.Start));
                    Assert.That(pair.Value, Is.EqualTo("A"));
                } else if (x == 1) {
                    Assert.That(pair.Key, Is.EqualTo(VariableState.End));
                    Assert.That(pair.Value, Is.EqualTo("B"));
                }
                x++;
            }

            list.Remove(VariableState.Start);
            Assert.That(list.ContainsKey(VariableState.Start), Is.False);
            Assert.That(list[VariableState.Start], Is.EqualTo(null));

            list.Clear();
            Assert.That(list.ContainsKey(VariableState.End), Is.False);
            Assert.That(list[VariableState.End], Is.EqualTo(null));

            list.Fill(e => "");
            Assert.That(list.Values.ToArray(), Is.EqualTo(new[] { "", "" }));
        }

        [Test]
        public void FixedEnumDictionaryTests() {
            var list = EnumDictionary<FixedState, string>.Create();
            Assert.That(list, Is.TypeOf<FixedEnumDictionary<FixedState, string>>());

            Assert.That(list.Count, Is.EqualTo(2));

            Assert.That(list[FixedState.Start], Is.Null);
            Assert.That(list[FixedState.End], Is.Null);
            Assert.That(list.Contains(new KeyValuePair<FixedState, string>(FixedState.Start, null)), Is.True);
            Assert.That(list.Contains(new KeyValuePair<FixedState, string>(FixedState.End, null)), Is.True);

            list[FixedState.Start] = "Z";
            Assert.That(list.ContainsKey(FixedState.Start), Is.True);
            Assert.That(list[FixedState.Start], Is.EqualTo("Z"));
            Assert.That(list.Contains(new KeyValuePair<FixedState, string>(FixedState.Start, "Z")), Is.True);
            Assert.That(list.Contains(new KeyValuePair<FixedState, string>(FixedState.Start, "x")), Is.False);

            list.Add(new KeyValuePair<FixedState, string>(FixedState.Start, "A"));
            Assert.That(list.ContainsKey(FixedState.Start), Is.True);
            Assert.That(list[FixedState.Start], Is.EqualTo("A"));
            Assert.That(list.Contains(new KeyValuePair<FixedState, string>(FixedState.Start, "A")), Is.True);
            Assert.That(list.Contains(new KeyValuePair<FixedState, string>(FixedState.Start, "x")), Is.False);

            list.Add(FixedState.End, "B");
            Assert.That(list.ContainsKey(FixedState.End), Is.True);
            Assert.That(list[FixedState.End], Is.EqualTo("B"));
            Assert.That(list.Contains(new KeyValuePair<FixedState, string>(FixedState.End, "B")), Is.True);
            Assert.That(list.Contains(new KeyValuePair<FixedState, string>(FixedState.End, "x")), Is.False);

            Assert.That(list.Keys.ToArray(), Is.EqualTo(new[] { FixedState.Start, FixedState.End }));
            Assert.That(list.Values.ToArray(), Is.EqualTo(new[] { "A", "B"}));
            
            var x = 0;
            foreach (var pair in list) {
                if (x == 0) {
                    Assert.That(pair.Key, Is.EqualTo(FixedState.Start));
                    Assert.That(pair.Value, Is.EqualTo("A"));
                } else if (x == 1) {
                    Assert.That(pair.Key, Is.EqualTo(FixedState.End));
                    Assert.That(pair.Value, Is.EqualTo("B"));
                }
                x++;
            }

            list.Remove(FixedState.Start);
            Assert.That(list.ContainsKey(FixedState.Start), Is.False);
            Assert.That(list[FixedState.Start], Is.EqualTo(null));

            list.Clear();
            Assert.That(list.ContainsKey(FixedState.End), Is.False);
            Assert.That(list[FixedState.End], Is.EqualTo(null));

            list.Fill(e => "");
            Assert.That(list.Values.ToArray(), Is.EqualTo(new[] { "", "" }));
        }
    }
}