using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using Godot;

namespace Betauer.DI {
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Method | AttributeTargets.Property)]
    public class InjectAttribute : Attribute {
    }

    [AttributeUsage(AttributeTargets.Field)]
    public class OnReadyAttribute : Attribute {
        public readonly string Path;

        public OnReadyAttribute(string path) {
            Path = path;
        }
    }

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

    public class InjectFieldException : Exception {
        public FieldInfo FieldInfo { get; }
        public object Instance { get; }

        public InjectFieldException(FieldInfo fieldInfo, object instance, string message) : base(message) {
            FieldInfo = fieldInfo;
            Instance = instance;
        }
    }

    [Singleton]
    public class Scanner {
        private readonly Logger _logger = LoggerFactory.GetLogger(typeof(Scanner));

        private Container _container;

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
            if (Attribute.GetCustomAttribute(type, typeof(SingletonAttribute),
                           false) is SingletonAttribute singletonAttribute) {
                _container.Register(type, Lifetime.Singleton, new [] { type }).Build();
            } else if (Attribute.GetCustomAttribute(type, typeof(TransientAttribute),
                           false) is TransientAttribute transientAttribute) {
                _container.Register(type, Lifetime.Transient, new [] { type }).Build();
            }
        }

        internal void AutoWire(object instance, ResolveContext context) {
            InjectFields(instance, context);
        }

        private void InjectFields(object target, ResolveContext context) {
            _logger.Debug("Injecting fields in " + target.GetType() + ": " + target.GetHashCode().ToString("X"));
            var fields = target.GetType()
                .GetFields(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);
            foreach (var field in fields) {
                if (!(Attribute.GetCustomAttribute(field, typeof(InjectAttribute), false) is InjectAttribute inject))
                    continue;
                _logger.Debug("Injecting field " + target.GetType() + "(" + target + ")." + field.Name + " " +
                              field.FieldType.Name);
                if (!_container.Exist(field.FieldType)) {
                    throw new InjectFieldException(field, target,
                        "Injectable property [" + field.FieldType.Name + " " + field.Name +
                        "] not found while injecting fields in " + target.GetType().Name);
                }
                var service = _container.Resolve(field.FieldType, context);
                field.SetValue(target, service);
            }
        }

        public void LoadOnReadyNodes(Node target) {
            var fields = target.GetType().GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Public |
                                                    BindingFlags.Instance);
            foreach (var property in fields) {
                if (!(Attribute.GetCustomAttribute(property, typeof(OnReadyAttribute), false) is OnReadyAttribute
                        onReady))
                    continue;
                var instance = target.GetNode(onReady.Path);
                var fieldInfo = "[OnReady(\"" + onReady.Path + "\")] " + property.FieldType.Name + " " +
                                property.Name;
                if (instance == null) {
                    throw new Exception("OnReady path is null in field " + fieldInfo + ", class " +
                                        target.GetType().Name);
                } else if (instance.GetType() != property.FieldType) {
                    throw new Exception("OnReady path returned a wrong type (" + instance.GetType().Name +
                                        ") in field " + fieldInfo + ", class " +
                                        target.GetType().Name);
                }
                property.SetValue(target, instance);
            }
        }
    }
}