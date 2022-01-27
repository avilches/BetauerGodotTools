using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using Godot;
using Object = Godot.Object;

namespace Betauer {
    [AttributeUsage(AttributeTargets.Field)]
    public class InjectAttribute : Attribute {
    }

    [AttributeUsage(AttributeTargets.Field)]
    public class OnReadyAttribute : Attribute {
        public string Path;

        public OnReadyAttribute(string path) {
            Path = path;
        }
    }

    [AttributeUsage(AttributeTargets.Class)]
    public class SingletonAttribute : Attribute {
    }

    public class DiRepository {
        private readonly Dictionary<Type, object> _singletons = new Dictionary<Type, object>();
        private readonly Logger _logger = LoggerFactory.GetLogger(typeof(DiRepository));
        private readonly Node _bootstrap;

        public DiRepository(Node bootstrap) {
            _bootstrap = bootstrap;
        }

        public T AddSingleton<T>(T instance) {
            _singletons.Add(instance.GetType(), instance);
            if (instance is Node nodeInstance) {
                nodeInstance.Name = nodeInstance.GetType().Name;
                _logger.Info("Adding singleton node " + nodeInstance.GetType().Name + " as Bootstrap Node child");
                _bootstrap.AddChild(nodeInstance);
            }
            return instance;
        }

        public T GetSingleton<T>(Type type) {
            return (T)_singletons[type];
        }

        public void Scan() {
            var assemblies = new[] { _bootstrap.GetType().Assembly };
            Scan(assemblies);
        }

        public void Scan(Assembly[] assemblies) {
            Stopwatch stopwatch = Stopwatch.StartNew();
            int types = 0, added = 0;
            foreach (var assembly in assemblies) {
                foreach (Type type in assembly.GetTypes()) {
                    types++;
                    if (Attribute.GetCustomAttribute(type, typeof(SingletonAttribute),
                            false) is SingletonAttribute sa) {
                        var instance = CreateSingletonInstance(type);
                        AddSingleton(instance);
                        added++;
                        _logger.Info("Added Singleton " + type.Name + " (" + type.FullName + ", Assembly: " +
                                     assembly.FullName + ")");
                    }
                }
            }

            _logger.Info(
                $"Scanned {types} types. Singletons: {added}. Elapsed time: {stopwatch.ElapsedMilliseconds} ms");
            stopwatch.Stop();
        }

        private object CreateSingletonInstance(Type type) {
            try {
                var emptyConstructor = type.GetConstructors().Single(info => info.GetParameters().Length == 0);
                var instance = emptyConstructor.Invoke(null);
                return instance;
            } catch (Exception e) {
                Console.WriteLine(e);
                throw;
            }
        }

        public void AutoWire() {
            var error = false;
            foreach (var instance in _singletons.Values) {
                error |= InjectFields(instance);
            }
            if (error) {
                throw new Exception("AutoWire error: Check the console output");
            }
        }

        public void AutoWire(object instance) {
            var error = InjectFields(instance);
            if (error) {
                throw new Exception("AutoWire error in " + instance.GetType().FullName + ": Check the console output");
            }
        }

        private bool InjectFields(object target) {
            bool error = false;
            var fields = target.GetType().GetFields(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);
            foreach (var property in fields) {
                if (!(Attribute.GetCustomAttribute(property, typeof(InjectAttribute), false) is InjectAttribute inject))
                    continue;
                var found = _singletons.TryGetValue(property.FieldType, out var instance);
                if (!found) {
                    _logger.Error("Injectable property [" + property.FieldType.Name + " " + property.Name +
                                  "] not found while injecting fields in " + target.GetType().Name);
                    error = true;
                }
                property.SetValue(target, instance);
            }
            return error;
        }

        public void LoadOnReadyNodes(Node target) {
            var fields = target.GetType().GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);
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

        public void Dispose() {
            foreach (var instance in _singletons.Values) {
                if (instance is IDisposable obj) {
                    try {
                        _logger.Info("Disposing singleton: "+obj.GetType());
                        obj.Dispose();
                    } catch (Exception e) {
                        _logger.Error(e);
                    }
                }
            }
        }
    }

    public abstract class Di {
        protected Di() => DiBootstrap.DefaultRepository.AutoWire(this);
    }

    public abstract class DiNode : Node {
        protected DiNode() => DiBootstrap.DefaultRepository.AutoWire(this);

        public sealed override void _Ready() {
            DiBootstrap.DefaultRepository.LoadOnReadyNodes(this);
            Ready();
        }

        public virtual void Ready() {
        }
    }

    public abstract class DiControl : Control {
        protected DiControl() => DiBootstrap.DefaultRepository.AutoWire(this);

        public sealed override void _Ready() {
            DiBootstrap.DefaultRepository.LoadOnReadyNodes(this);
            Ready();
        }

        public virtual void Ready() {
        }
    }

    public abstract class DiButton : Button {
        protected DiButton() => DiBootstrap.DefaultRepository.AutoWire(this);

        public sealed override void _Ready() {
            DiBootstrap.DefaultRepository.LoadOnReadyNodes(this);
            Ready();
        }

        public virtual void Ready() {
        }
    }

    public abstract class DiNode2D : Node2D {
        protected DiNode2D() => DiBootstrap.DefaultRepository.AutoWire(this);

        public sealed override void _Ready() {
            DiBootstrap.DefaultRepository.LoadOnReadyNodes(this);
            Ready();
        }

        public virtual void Ready() {
        }
    }

    public abstract class DiKinematicBody2D : KinematicBody2D {
        protected DiKinematicBody2D() => DiBootstrap.DefaultRepository.AutoWire(this);

        public sealed override void _Ready() {
            DiBootstrap.DefaultRepository.LoadOnReadyNodes(this);
            Ready();
        }

        public virtual void Ready() {
        }
    }

    public abstract class DiCamera2D : Camera2D {
        protected DiCamera2D() => DiBootstrap.DefaultRepository.AutoWire(this);

        public sealed override void _Ready() {
            DiBootstrap.DefaultRepository.LoadOnReadyNodes(this);
            Ready();
        }

        public virtual void Ready() {
        }
    }

    public abstract class DiArea2D : Area2D {
        protected DiArea2D() => DiBootstrap.DefaultRepository.AutoWire(this);

        public sealed override void _Ready() {
            DiBootstrap.DefaultRepository.LoadOnReadyNodes(this);
            Ready();
        }

        public virtual void Ready() {
        }
    }


    /**
     * DiBootstrap. Singleton + Node + Special Di (scan all + autowire ifself)
     */
    public abstract class
        DiBootstrap : Node /* needed because 1) it's an autoload 2) all Node singletons scanned will be added as child */ {
        public static DiRepository DefaultRepository;
        public static DiBootstrap Instance;
        public static readonly bool IsDevelopment = OS.HasFeature("editor");

        public DiBootstrap() {
            if (Instance != null) {
                throw new Exception("DiBootstrap can't be instantiated more than once: " + GetType().Name);
            }
            Instance = this;
            DefaultRepository = CreateDiRepository();
            DefaultRepository.Scan();
            DefaultRepository.AddSingleton<Func<SceneTree>>(GetTree);
            DefaultRepository.AutoWire();
            DefaultRepository.AutoWire(this);
        }

        public virtual DiRepository CreateDiRepository() {
            return new DiRepository(this);
        }

        public void CheckErrorPolicy(UnhandledExceptionPolicy unhandledExceptionPolicyConfig) {
            var unhandledExceptionPolicySetting = GetUnhandledExceptionPolicySetting();
            if (!IsDevelopment) {
                // This is a release
                if (unhandledExceptionPolicySetting == UnhandledExceptionPolicy.TerminateApplication) {
                    // If the app can crash with just an exception, then better to crash as soon as possible
                    throw new Exception(
                        "Please, don't use export a release with the mono/unhandled_exception_policy as Terminate Application");
                }
            }
            if (unhandledExceptionPolicySetting == UnhandledExceptionPolicy.LogError) {
                GD.Print("Current unhandled exception policy: Log Error");
                if (unhandledExceptionPolicyConfig == UnhandledExceptionPolicy.TerminateApplication) {
                    WriteUnhandledExceptionPolicy(UnhandledExceptionPolicy.TerminateApplication);
                }
            } else if (unhandledExceptionPolicySetting == UnhandledExceptionPolicy.TerminateApplication) {
                GD.Print("Current unhandled exception policy: Terminate Application");
                if (unhandledExceptionPolicyConfig == UnhandledExceptionPolicy.LogError) {
                    WriteUnhandledExceptionPolicy(UnhandledExceptionPolicy.LogError);
                }
            }
        }

        private void WriteUnhandledExceptionPolicy(UnhandledExceptionPolicy policy) {
            GD.PushWarning("Writing mono/unhandled_exception_policy=" + policy +
                           " in project.godot configuration. Restart to reload changes. Aborting...");
            ProjectSettings.SetSetting("mono/unhandled_exception_policy", (int)policy);
            ProjectSettings.Save();
            GetTree().Quit();
        }

        private static UnhandledExceptionPolicy GetUnhandledExceptionPolicySetting() {
            var x = ProjectSettings.GetSetting("mono/unhandled_exception_policy");
            return x is int policy && policy == (int)UnhandledExceptionPolicy.LogError
                ? UnhandledExceptionPolicy.LogError
                : UnhandledExceptionPolicy.TerminateApplication;
        }
    }

    public enum UnhandledExceptionPolicy {
        TerminateApplication = 0,
        LogError = 1
    }
}