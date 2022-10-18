using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Betauer.Reflection;
using Betauer.TestRunner;
using NUnit.Framework;

namespace Betauer.Tests {
    [TestFixture]
    [Only]
    public class TypeExtensionsTest {
        [Test]
        public void GetNameWithoutGenerics() {
            Assert.That(typeof(List<string>).GetNameWithoutGenerics(), Is.EqualTo("List"));
            Assert.That(typeof(Dictionary<string, List<string>>).GetNameWithoutGenerics(), Is.EqualTo("Dictionary"));
        }

        [Test]
        public void ImplementsInterfaceTests() {
            Assert.That(typeof(string).ImplementsInterface(typeof(IList<>)), Is.False);
            Assert.That(typeof(string).ImplementsInterface(typeof(IList<int>)), Is.False);
            
            Assert.That(typeof(List<>).ImplementsInterface(typeof(IList<>)), Is.True);

            Assert.That(typeof(List<>).ImplementsInterface(typeof(IList<int>)), Is.False);
            
            Assert.That(typeof(List<int>).ImplementsInterface(typeof(IList<>)), Is.True);
            Assert.That(typeof(List<int>).ImplementsInterface(typeof(IList<int>)), Is.True);
            Assert.That(typeof(List<int>).ImplementsInterface(typeof(IList<string>)), Is.False);            
        }
    }
}