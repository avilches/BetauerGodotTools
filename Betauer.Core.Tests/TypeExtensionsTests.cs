using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Betauer.Reflection;
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
    }
}