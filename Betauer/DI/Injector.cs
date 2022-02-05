using System;
using System.Reflection;
using Godot;

namespace Betauer.DI {
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Method | AttributeTargets.Property)]
    public class InjectAttribute : Attribute {
        public bool Nullable { get; set; } = false;
    }

    [AttributeUsage(AttributeTargets.Field)]
    public class OnReadyAttribute : Attribute {
        public readonly string Path;

        public OnReadyAttribute(string path) {
            Path = path;
        }
    }

    public class InjectFieldException : Exception {
        public FieldInfo FieldInfo { get; }
        public object Instance { get; }

        public InjectFieldException(FieldInfo fieldInfo, object instance, string message) : base(message) {
            FieldInfo = fieldInfo;
            Instance = instance;
        }
    }

    public class Injector {
        private readonly Logger _logger = LoggerFactory.GetLogger(typeof(Injector));
        private readonly Container _container;

        public Injector(Container container) {
            _container = container;
        }

        public void InjectAllFields(object target, ResolveContext context) {
            if (target is Delegate) return;
            _logger.Debug("Injecting fields in " + target.GetType() + ": " + target.GetHashCode().ToString("X"));
            var fields = target.GetType()
                .GetFields(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);
            foreach (var field in fields) {
                if (!(Attribute.GetCustomAttribute(field, typeof(InjectAttribute), false) is InjectAttribute inject))
                    continue;
                InjectField(target, context, field, inject.Nullable);
            }
        }

        private void InjectField(object target, ResolveContext context, FieldInfo field, bool nullable) {
            if (field.GetValue(target) != null) {
                // Ignore the already defined values
                // TODO: test
                return;
            }
            if (!_container.Exist(field.FieldType)) {
                if (field.FieldType.IsGenericType &&
                    field.FieldType.GetGenericTypeDefinition() == typeof(Func<>) &&
                    field.FieldType.GetGenericArguments().Length == 1) {
                    var outType = field.FieldType.GetGenericArguments()[0];
                    // [Inject] private Func<TOut>
                    // Find a Func<TIn, TOut> where TIn is the current instance type or any of its base classes
                    var inputType = target.GetType();
                    object? service;
                    do {
                        var funcType = typeof(Func<,>).MakeGenericType(new[] { inputType, outType });
                        service = _container.Exist(funcType) ? _container.Resolve(funcType, context) : null;
                        if (service == null) {
                            inputType = inputType.BaseType;
                        }
                    } while (service == null && inputType != null);

                    MethodInfo inject = GetType().GetMethod("Inject", BindingFlags.Static | BindingFlags.Public)!;
                    if (service != null) {
                        // field.SetValue(target, service);
                        // Func<Node, RootSceneHolder> function = (Node node, RootSceneHolder scene) => { .... return sceneHolder; }

                        // Func<RootSceneHolder> service = () => function(node)

                        var closure = inject.MakeGenericMethod(new[] { inputType, outType })
                            .Invoke(this, new object[] { service, target });
                        field.SetValue(target, closure);
                        return;
                    }
                }

                if (!nullable) {
                    throw new InjectFieldException(field, target,
                        "Injectable property [" + field.FieldType.Name + " " + field.Name +
                        "] not found while injecting fields in " + target.GetType().Name);
                }
            } else {
                _logger.Debug("Injecting field " + target.GetType() + "(" + target + ")." + field.Name + " " +
                              field.FieldType.Name);
                var service = _container.Resolve(field.FieldType, context);
                field.SetValue(target, service);
            }
        }

        public static Func<T2> Inject<T1, T2>(Func<T1, T2> func, T1 inject) {
            return () => {
                GD.Print("Composing factory with " + inject);
                return func(inject);
            };
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