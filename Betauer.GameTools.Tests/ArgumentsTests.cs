using System.Collections;
using System.Collections.Generic;
using Betauer.Application;
using Betauer.TestRunner;
using Godot;
using NUnit.Framework;

namespace Betauer.GameTools.Tests {
    [TestFixture]
    public class ArgumentsTest {
        [Test]
        public void BasicTest() {
            var a = Arguments.ParseArgs(
                new[] { "--s", "value-s", "-p", "-z", "value-z", "command1", "-x=value-x", "--yyy=value-yyy", "command2", "-", "empty" , "-", "empty" });

            Assert.That(a.Commands, Is.EqualTo(new List<string> {"command1", "command2" }));
            Assert.That(a.Options["s"], Is.EqualTo("value-s"));
            Assert.That(a.Options["p"], Is.EqualTo(""));
            Assert.That(a.Options["z"], Is.EqualTo("value-z"));
            Assert.That(a.Options["x"], Is.EqualTo("value-x"));
            Assert.That(a.Options["yyy"], Is.EqualTo("value-yyy"));
            Assert.That(a.Options[""], Is.EqualTo("empty"));
        }
        
        [Test]
        public void WeirdCasesTest() {
            var a = Arguments.ParseArgs(
                new[] { "a=a", "-s==", "--xx=", "-=aaa"});
            Assert.That(a.Commands, Is.EqualTo(new List<string> {"a=a"}));
            Assert.That(a.Options["s"], Is.EqualTo("="));
            Assert.That(a.Options["xx"], Is.EqualTo(""));
            Assert.That(a.Options[""], Is.EqualTo("aaa"));

        }
    }
}