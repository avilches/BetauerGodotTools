using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Betauer.DI;
using Betauer.Signal;
using Godot;

namespace Betauer.Loader {
    public class ResourceLoaderException : Exception {
        public ResourceLoaderException(string message) : base(message) {
        }
    }

    public class BaseResourceMetadataRegistry {
        protected Dictionary<string, ResourceMetadata> _registry;

        public bool Contains(string res) => _registry.ContainsKey(res);
        public T? Resource<T>(string res) where T : class => _registry[res].Resource as T;
        public T? Scene<T>(string res) where T : class => Resource<PackedScene>(res)?.Instance<T>();
    }

    public class ResourceMetadataRegistry : BaseResourceMetadataRegistry {
        public async Task Load(IEnumerable<string> resourcesToLoad, Action<LoadingContext> progress,
            Func<Task> awaiter) {
            _registry = await Loader.Load(resourcesToLoad, progress, awaiter);
        }

        public void Unload() {
            foreach (var resourceMetadata in _registry.Values) resourceMetadata.Resource.Dispose();
            _registry.Clear();
        }
    }

    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public class ResourceAttribute : Attribute {
        public readonly string Path;

        public ResourceAttribute(string path) {
            Path = path;
        }
    }

    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public class SceneAttribute : Attribute {
        public readonly string Path;

        public SceneAttribute(string path) {
            Path = path;
        }
    }

    public abstract class ResourceLoaderContainer : ResourceMetadataRegistry {
        private LinkedList<Action<LoadingContext>>? _onProgressList;
        private Func<Task>? _awaiter;
        private int _maxTime = 100;
        private readonly LinkedList<object> _targets = new LinkedList<object>();
        [Inject] public SceneTree SceneTree;

        protected ResourceLoaderContainer() {
            _targets.AddLast(this);
        }

        public ResourceLoaderContainer SetMaxPollTime(int maxTime) {
            _maxTime = maxTime;
            return this;
        }

        public ResourceLoaderContainer SetAwaiter(Func<Task>? awaiter) {
            _awaiter = awaiter;
            return this;
        }

        public ResourceLoaderContainer OnProgress(Action<LoadingContext> progress) {
            _onProgressList ??= new LinkedList<Action<LoadingContext>>();
            _onProgressList.AddLast(progress);
            return this;
        }

        public ResourceLoaderContainer Bind(params object[] targets) {
            foreach (var target in targets)
                if (!_targets.Contains(target))
                    _targets.AddLast(target);
            return this;
        }

        public ResourceLoaderContainer Unbind(params object[] targets) {
            foreach (var target in targets) _targets.Remove(target);
            return this;
        }

        public async Task Load(params object[] moreTargets) {
            var progress = CombineOnProgress(_onProgressList);
            Func<Task> awaiter = _awaiter ?? (async () => await SceneTree.AwaitIdleFrame());
            IEnumerable<object> targets = _targets.Concat(moreTargets);
            _registry = await Loader.Load(GetResourcesToLoad(targets), progress, awaiter, _maxTime);
            InjectResources(_registry, targets);
        }

        public ResourceLoaderContainer Unload() {
            foreach (var key in _registry.Keys) {
                _registry[key].Resource.Dispose();
                _registry.Remove(key);
            }
            // Set to null all fields with attributes [Scene] and [Resource]
            foreach (var field in GetType().GetFields(Flags))
                if (Attribute.GetCustomAttribute(field, typeof(SceneAttribute), false) is SceneAttribute ||
                    Attribute.GetCustomAttribute(field, typeof(ResourceAttribute), false) is ResourceAttribute)
                    field.SetValue(this, null);

            foreach (var property in GetType().GetProperties(Flags))
                if (Attribute.GetCustomAttribute(property, typeof(SceneAttribute), false) is SceneAttribute ||
                    Attribute.GetCustomAttribute(property, typeof(ResourceAttribute), false) is ResourceAttribute)
                    property.SetValue(this, null);
            return this;
        }

        private static Action<LoadingContext>? CombineOnProgress(IEnumerable<Action<LoadingContext>>? progress) {
            if (progress == null) return null;
            void OnProgress(LoadingContext context) {
                foreach (var action in progress) action(context);
            }
            return OnProgress;
        }

        private const BindingFlags Flags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance;

        private static IEnumerable<string> GetResourcesToLoad(IEnumerable<object> targets) {
            var resources = new HashSet<string>();
            foreach (var target in targets) {
                foreach (var field in target.GetType().GetFields(Flags))
                    if (Attribute.GetCustomAttribute(field, typeof(SceneAttribute), false) is SceneAttribute scene)
                        resources.Add(scene.Path);
                    else if (Attribute.GetCustomAttribute(field, typeof(ResourceAttribute), false) is ResourceAttribute
                             resource)
                        resources.Add(resource.Path);

                foreach (var property in target.GetType().GetProperties(Flags))
                    if (Attribute.GetCustomAttribute(property, typeof(SceneAttribute), false) is SceneAttribute scene)
                        resources.Add(scene.Path);
                    else if (Attribute.GetCustomAttribute(property, typeof(ResourceAttribute), false) is
                             ResourceAttribute resource)
                        resources.Add(resource.Path);
            }
            return resources;
        }

        private static void InjectResources(IReadOnlyDictionary<string, ResourceMetadata> resources,
            IEnumerable<object> targets) {
            foreach (var target in targets) {
                foreach (var field in target.GetType().GetFields(Flags)) {
                    if (Attribute.GetCustomAttribute(field, typeof(SceneAttribute), false) is SceneAttribute scene)
                        InjectScene(target, new Setter(field), resources[scene.Path]);
                    else if (Attribute.GetCustomAttribute(field, typeof(ResourceAttribute), false) is ResourceAttribute
                             resource)
                        InjectResource(target, new Setter(field), resources[resource.Path]);
                }

                foreach (var property in target.GetType().GetProperties(Flags)) {
                    if (Attribute.GetCustomAttribute(property, typeof(SceneAttribute), false) is SceneAttribute scene)
                        InjectScene(target, new Setter(property), resources[scene.Path]);
                    else if (Attribute.GetCustomAttribute(property, typeof(ResourceAttribute), false) is
                             ResourceAttribute resource)
                        InjectResource(target, new Setter(property), resources[resource.Path]);
                }
            }
        }

        private static Delegate CreatePackedSceneToInstanceFunction(PackedScene resource, Type returnType) {
            var closure = (Delegate)ComposeMethod.MakeGenericMethod(returnType.GetGenericArguments()[0])
                .Invoke(null, new[] { resource });
            return closure;
        }

        private static Func<T> PackedSceneToInstance<T>(PackedScene value) where T : class => () => value.Instance<T>();

        private static readonly MethodInfo ComposeMethod = typeof(ResourceLoaderContainer)
            .GetMethod(nameof(PackedSceneToInstance), BindingFlags.Static | BindingFlags.NonPublic)!;

        private static void InjectResource(object target, Setter setter, ResourceMetadata resource) {
            if (setter.Type == typeof(ResourceMetadata)) {
                setter.SetValue(target, resource);
            } else if (setter.Type.IsGenericType &&
                       setter.Type.GetGenericTypeDefinition() == typeof(ResourceMetadata<>)) {
                var genericType = setter.Type.GetGenericArguments()[0];
                if (genericType.IsInstanceOfType(resource.Resource)) {
                    setter.SetValue(target, ResourceMetadata.DynamicConstructor(resource, genericType));
                } else {
                    throw new ResourceLoaderException("Incompatible type ResourceMetadata<" + setter.Type + "> for " +
                                                      resource.Resource.GetType() + ": " + resource.Path);
                }
            } else if (setter.Type.IsInstanceOfType(resource.Resource)) {
                setter.SetValue(target, resource.Resource);
            } else {
                throw new ResourceLoaderException("Incompatible type " + setter.Type + " for " +
                                                  resource.Resource.GetType() + ": " + resource.Path);
            }
        }

        private static void InjectScene(object target, Setter setter, ResourceMetadata resource) {
            if (!(resource.Resource is PackedScene packedScene)) {
                throw new ResourceLoaderException("Resource type: " + resource.Resource.GetType() +
                                                  " should be a PackedScene. " + resource.Path);
            }
            if (setter.Type.IsGenericType && setter.Type.GetGenericTypeDefinition() == typeof(Func<>)) {
                var packedSceneToInstanceFunction = CreatePackedSceneToInstanceFunction(packedScene, setter.Type);
                setter.SetValue(target, packedSceneToInstanceFunction);
            } else if (typeof(Node).IsAssignableFrom(setter.Type)) {
                Node o = packedScene.Instance();
                if (setter.Type.IsInstanceOfType(o)) {
                    setter.SetValue(target, o);
                } else {
                    throw new ResourceLoaderException("Scene " + o.GetType() + " created should be " + setter.Type +
                                                      " for " + resource.Path);
                }
            } else {
                throw new ResourceLoaderException("Incompatible type " + setter.Type + " for " +
                                                  resource.Resource.GetType() + ": " + resource.Path);
            }
        }
    }
}