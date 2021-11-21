using System;
using System.Collections.Generic;
using System.Linq;
using Godot;

namespace Tools {
    public class InjectAttribute : Attribute {
    }

    public class SingletonAttribute : Attribute {
    }

    public class DiRepository {
        private readonly Dictionary<Type, object> _singletons = new Dictionary<Type, object>();
        private Logger _logger = LoggerFactory.GetLogger(typeof(DiRepository));

        public T AddSingleton<T>(T instance) {
            _singletons.Add(instance.GetType(), instance);
            return instance;
        }

        public T GetSingleton<T>(Type type) {
            return (T)_singletons[type];
        }

        public void Scan(Node node) {
            var assemblies = AppDomain.CurrentDomain.GetAssemblies();
            foreach (var assembly in assemblies) {
                foreach (Type type in assembly.GetTypes()) {
                    if (Attribute.GetCustomAttribute(type, typeof(SingletonAttribute),
                        false) is SingletonAttribute sa) {
                        var instance = CreateSingletonInstance(type);
                        AddSingleton(instance);
                        Console.WriteLine("Added Singleton "+type.Name+ " (" + type.FullName+ ", Assembly: "+assembly.FullName+")");
                    }
                }
            }

            bool error = false;
            foreach (var instance in _singletons.Values) {
                error |= InjectFields(instance);
                if (instance is Node nodeInstance) {
                    node.AddChild(nodeInstance);
                }
            }
            if (error) {
                throw new Exception("Scan error. Check the console output");
            }
        }

        private object CreateSingletonInstance(Type type) {
            var emptyConstructor = type.GetConstructors().Single(info => info.GetParameters().Length == 0);
            var instance = emptyConstructor.Invoke(null);
            return instance;
        }

        public void AutoWire(object instance) {
            var error = InjectFields(instance);
            if (error) {
                throw new Exception("AutoWire error in "+instance.GetType().FullName+": Check the console output");
            }
        }

        private bool InjectFields(object target) {
            bool error = false;
            foreach (var property in target.GetType().GetFields()) {
                if (!(Attribute.GetCustomAttribute(property, typeof(InjectAttribute), false) is InjectAttribute inject))
                    continue;
                var found = _singletons.TryGetValue(property.FieldType, out object instance);
                if (!found) {
                    Console.WriteLine("Injectable property ["+property.FieldType.Name+" "+property.Name+"] not found");
                    error = true;
                }
                property.SetValue(target, instance);
            }
            return error;
        }
    }

    public class GodotDiRepository : DiRepository {
        public static readonly GodotDiRepository Instance = new GodotDiRepository();

        private GodotDiRepository() {
        }
    }

    public class DiKinematicBody2D : KinematicBody2D {
        public DiKinematicBody2D() => GodotDiRepository.Instance.AutoWire(this);
    }

    public class Di {
        public Di() => GodotDiRepository.Instance.AutoWire(this);
    }

    public class DiNode : Node {
        public DiNode() => GodotDiRepository.Instance.AutoWire(this);
    }

    public class DiNode2D : Node2D {
        public DiNode2D() => GodotDiRepository.Instance.AutoWire(this);
    }

    public class DiCamera2D : Camera2D {
        public DiCamera2D() => GodotDiRepository.Instance.AutoWire(this);
    }

    public class DiArea2D : Area2D {
        public DiArea2D() => GodotDiRepository.Instance.AutoWire(this);
    }
}