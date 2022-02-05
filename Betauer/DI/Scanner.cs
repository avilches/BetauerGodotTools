using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using Godot;

namespace Betauer.DI {
    [AttributeUsage(AttributeTargets.Class)]
    public class SingletonAttribute : Attribute {
        // public string Name { get; set; } // TODO: use Name so more than one factory can be used per name
        // public Type Type { get; set;  }
        // public Type[] Types { get; set;  }
    }

    [AttributeUsage(AttributeTargets.Class)]
    public class TransientAttribute : Attribute {
    }

    // TODO: add [Component] attribute: it means the class has inside services exposed as methods like:
    /*

    [Transient]
    public Node CreatePepeBean() {
        return new Node();
    }

    [Singleton]
    public SaveGameManager CreateSaveGameManager() {
        return new SaveGameManager();
    }

    // _container.Register<Node>(() => CreatePepeBean()).IsTransient()
    // _container.Register<SaveGameManager>(() => CreateSaveGameManager()).IsSingleton()

     */

    public class Scanner {
        private readonly Logger _logger = LoggerFactory.GetLogger(typeof(Scanner));
        private readonly Container _container;

        public Scanner(Container container) {
            _container = container;
        }

        public void Scan() {
            var assemblies = new[] { _container.Owner.GetType().Assembly };
            Scan(assemblies);
        }

        public void Scan(Assembly assembly) {
            Scan(new Assembly[] { assembly });
        }

        public void Scan(IEnumerable<Assembly> assemblies) {
            int types = 0, assembliesCount = 0;
            Stopwatch stopwatch = Stopwatch.StartNew();
            foreach (var assembly in assemblies) {
                assembliesCount++;
                foreach (Type type in assembly.GetTypes()) {
                    types++;
                    Scan(type);
                }
            }
            stopwatch.Stop();
            _logger.Info(
                $"Registered {types} types in {assembliesCount} assemblies. Elapsed time: {stopwatch.ElapsedMilliseconds} ms");
        }

        public void Scan(IEnumerable<Type> types) {
            foreach (Type type in types) {
                Scan(type);
            }
        }

        public void Scan<T>() {
            Scan(typeof(T));
        }

        public void Scan(Type type) {
            // TODO: include more types in the attribute
            // Attribute[] attrib = Attribute.GetCustomAttributes(type, false);
            // TransientAttribute tr = attrib.FirstOrDefault(attribute => attribute is TransientAttribute != null) as TransientAttribute;
            if (Attribute.GetCustomAttribute(type, typeof(SingletonAttribute), false) is
                SingletonAttribute singletonAttribute) {
                _container.Register(type, Lifetime.Singleton, new[] { type }).Build();
            } else if (Attribute.GetCustomAttribute(type, typeof(TransientAttribute), false) is
                       TransientAttribute transientAttribute) {
                _container.Register(type, Lifetime.Transient, new[] { type }).Build();
            }
        }
    }
}