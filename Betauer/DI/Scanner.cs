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
    public class ServiceAttribute : Attribute {
        public Lifestyle Lifestyle { get; set; } = Lifestyle.Singleton;
    }

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

        public void Scan(Type type) {
            if (Attribute.GetCustomAttribute(type, typeof(ServiceAttribute),
                    false) is ServiceAttribute sa) {
                // What if type is an interface
                _container.Register(type).Lifestyle(sa.Lifestyle).Build();
            }
        }

        public void AutoWire(object instance) {
            InjectFields(instance);
        }

        private void InjectFields(object target) {
            var fields = target.GetType()
                .GetFields(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);
            foreach (var property in fields) {
                if (!(Attribute.GetCustomAttribute(property, typeof(InjectAttribute), false) is InjectAttribute inject))
                    continue;
                try {
                    var found = _container.Resolve(property.FieldType);
                    property.SetValue(target, found);
                } catch (KeyNotFoundException) {
                    throw new Exception("Injectable property [" + property.FieldType.Name + " " + property.Name +
                                        "] not found while injecting fields in " + target.GetType().Name);
                }
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