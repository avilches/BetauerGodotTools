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

    public interface ISingleton2 : IInterface2_2 {
    }

    public interface INotScanned {
    }

    [Service]
    public class Singleton : ISingleton1, ISingleton2 {
    }

    [Service]
    public class MyService {
        [Inject] private INotScanned _singleton2_2;
        [Inject] private ISingleton2 _singleton2;
        [Inject] private ISingleton1 _singleton1;
        [Inject] private Singleton _singleton;
    }

    [Service(Lifestyle = Lifestyle.Transient)]
    public class TransientService {
        [Inject] private INotScanned _singleton2_2;
        [Inject] private ISingleton2 _singleton2;
        [Inject] private ISingleton1 _singleton1;
        [Inject] private Singleton _singleton;
    }

    public class NotScannedClass {
        [Inject] private INotScanned _singleton2_2;
        [Inject] private ISingleton2 _singleton2;
        [Inject] private ISingleton1 _singleton1;
        [Inject] private Singleton _singleton;
    }

    /*
     * Circular puro
     */

    [Service]
    public class Circular {
        [Inject] private Circular circular;
    }

    public interface ICircular {
    }

    /*
     * Circular con interface
     */

    [Service]
    public class CircularWithInterface : ICircular {
        [Inject] private ICircular circular;
    }


    /*
     * Circular transitiva
     */

    [Service]
    public class Circular1 {
        [Inject] private Circular2 _singleton2_2;
    }

    [Service]
    public class Circular2 {
        [Inject] private Circular1 _singleton2_2;
    }

    [TestFixture]
    public class DiRepositoryTests : Node {
        [Test(Description = "Types not found")]
        public void NotFound() {
            var di = new Container(this);
            di.Scanner.Scan(typeof(Singleton));
            di.Scanner.Scan(typeof(MyService));
            di.Scanner.Scan(typeof(TransientService));
            di.Scanner.Scan(typeof(NotScannedClass)); // Esto no debe hacer nada

            var s = di.Resolve<Singleton>();
            var s1 = di.Resolve<ISingleton1>();
            var s2 = di.Resolve<ISingleton2>();

            var my = di.Resolve<MyService>();
            // INotScanned is not scanned, so it doesn't know how to solve it

            var t1 = di.Resolve<TransientService>();
            var t2 = di.Resolve<TransientService>();

            var s22 = di.Resolve<INotScanned>();
            var a = di.Resolve<NotScannedClass>();

            /*
             * casos, que pasa cuando se inserta una instancia: se resuelven los inject pero ¿y si alguno todavia no esta?
             *
             * que pasa si se hace un Resolve(), se crea el singleton, y luego mas tarde se añade un servicio: el singleton no lo contendria
             *
             * tests: probar que scan de un tipo sin
             *
             *
             * es posible hacer referencias circulares
             */

            // di.Rewire(); // revisa que los singleton con instancias ya creadas contienen todos lo campos... bastaria con un hacer autowire a todo pero validando
        }
    }
}