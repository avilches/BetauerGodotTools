using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Betauer.TestRunner;
using NUnit.Framework;

namespace Betauer.Tests {
    [TestFixture]
    public class TypeExtensionsTest {
        [Test]
        public void GetNameWithoutGenerics() {
            Assert.That(typeof(List<string>).GetNameWithoutGenerics(), Is.EqualTo("List"));
            Assert.That(typeof(Dictionary<string, List<string>>).GetNameWithoutGenerics(), Is.EqualTo("Dictionary"));
        }

        [Test]
        public void FindMethod() {
            var containsKey = typeof(Dictionary<string, List<string>>)
                .FindMethod("ContainsKey",
                    typeof(bool), BindingFlags.Public,
                    new[] { typeof(string) });

            Assert.That(containsKey.Name, Is.EqualTo("ContainsKey"));
            Assert.That(containsKey.ReturnType, Is.EqualTo(typeof(bool)));
            Assert.That(containsKey.GetParameters().ToList().Select(x => x.ParameterType).ToArray(),
                Is.EqualTo(new[] { typeof(string) }));

            var trimExcess = typeof(Dictionary<string, List<string>>)
                .FindMethod("TrimExcess", typeof(void));
            Assert.That(trimExcess.Name, Is.EqualTo("TrimExcess"));
            Assert.That(trimExcess.ReturnType, Is.EqualTo(typeof(void)));
            Assert.That(trimExcess.GetParameters().Length, Is.EqualTo(0));
        }
    }
}