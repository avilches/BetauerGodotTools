using System;
using System.Collections.Generic;
using Betauer.DI;
using Betauer.TestRunner;
using Godot;
using NUnit.Framework;
using Container = Betauer.DI.Container;

namespace Betauer.Tests {

    public interface ISingleton1 {
    }

    public interface ISingleton2 {
    }

    [Service]
    public class Singleton : ISingleton1, ISingleton2{

    }


    [TestFixture]
    public class DiRepositoryTests : Node {
        [Test(Description = "Types not found")]
        public void NotFound() {
            var di = new Container(this);
            di.Scanner.Scan(typeof(Singleton));

        }
    }

}