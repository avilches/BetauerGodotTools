using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Betauer.DI;
using Betauer.Reflection;
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
    public abstract class BaseResourceAttribute : Attribute {
        public readonly string Path;
        protected BaseResourceAttribute(string path) => Path = path;
    }

    public class SceneAttribute : BaseResourceAttribute {
        public SceneAttribute(string path) : base(path) {
        }
    }
    
    public class ResourceAttribute : BaseResourceAttribute {
        public ResourceAttribute(string path) : base(path) {
        }
    }

    public abstract class ResourceLoaderContainer : ResourceMetadataRegistry {
        public event Action<LoadingContext> OnProgress;
        private Func<Task>? _awaiter;
        private int _maxTime = 100;
        private readonly LinkedList<object> _targets = new LinkedList<object>();
        [Inject] public SceneTree SceneTree { get; set; }

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

        public ResourceLoaderContainer From(params object[] targets) {
            foreach (var target in targets)
                if (!_targets.Contains(target))
                    _targets.AddLast(target);
            return this;
        }

        public ResourceLoaderContainer Inject(params object[] targets) {
            InjectResources(_registry, targets);
            return this;
        }


        private const BindingFlags Flags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance;
        private const MemberTypes MemberFlags = MemberTypes.Field | MemberTypes.Property;

        public async Task Load(params object[] moreTargets) {
            Func<Task> awaiter = _awaiter ?? (async () => await SceneTree.AwaitIdleFrame());
            IEnumerable<object> targets = _targets.Concat(moreTargets);
            _registry = await Loader.Load(GetResourcesToLoad(targets), (context) => OnProgress?.Invoke(context), awaiter, _maxTime);
            InjectResources(_registry, targets);
        }
        

        public ResourceLoaderContainer Unload() {
            foreach (var key in _registry.Keys) {
                _registry[key].Resource.Dispose();
                _registry.Remove(key);
            }
            // Set to null all fields with attributes [Scene] and [Resource]
            foreach (var setter in GetType().GetSettersCached<BaseResourceAttribute>(MemberFlags, Flags))
                setter.SetValue(this, null);
            return this;
        }

        private static IEnumerable<string> GetResourcesToLoad(IEnumerable<object> targets) {
            var resources = new HashSet<string>();
            foreach (var target in targets)
                foreach (var setter in target.GetType().GetSettersCached<BaseResourceAttribute>(MemberFlags, Flags))
                    resources.Add(setter.Attribute.Path);
            return resources;
        }

        private static void InjectResources(IReadOnlyDictionary<string, ResourceMetadata> resources, IEnumerable<object> targets) {
            foreach (var target in targets) {
                foreach (var setter in target.GetType().GetSettersCached<BaseResourceAttribute>(MemberFlags, Flags))
                    if (setter.Attribute is SceneAttribute)
                        InjectScene(target, setter, resources[setter.Attribute.Path]);
                    else if (setter.Attribute is ResourceAttribute)
                        InjectResource(target, setter, resources[setter.Attribute.Path]);
            }
        }

        private static void InjectResource(object target, ISetter setter, ResourceMetadata resourceMetadata) {
            if (setter.Type == typeof(ResourceMetadata)) {
                // [Resource("")] public ResourceMetadata field
                setter.SetValue(target, resourceMetadata);
            } else if (setter.Type.IsGenericType &&
                       setter.Type.GetGenericTypeDefinition() == typeof(ResourceMetadata<>)) {
                var genericType = setter.Type.GetGenericArguments()[0];
                // [Resource("")] public ResourceMetadata<Resource or children> field
                if (genericType.IsInstanceOfType(resourceMetadata.Resource)) {
                    var resourceMetadataWithGeneric =
                        ResourceMetadata.CreateResourceMetadataWithGeneric(resourceMetadata, genericType);
                    setter.SetValue(target, resourceMetadataWithGeneric);
                } else {
                    // [Resource("")] public ResourceMetadata<non resource type> field
                    throw new ResourceLoaderException("Incompatible type ResourceMetadata<" + setter.Type + "> for " +
                                                      resourceMetadata.Resource.GetType() + ": " + resourceMetadata.Path);
                }
            } else if (setter.Type.IsInstanceOfType(resourceMetadata.Resource)) {
                // [Resource("")] public Resource field // matching the Resource type with the resource loaded
                setter.SetValue(target, resourceMetadata.Resource);
            } else {
                throw new ResourceLoaderException("Incompatible type " + setter.Type + " for " +
                                                  resourceMetadata.Resource.GetType() + ": " + resourceMetadata.Path);
            }
        }

        private static void InjectScene(object target, ISetter setter, ResourceMetadata resource) {
            if (!(resource.Resource is PackedScene packedScene)) {
                throw new ResourceLoaderException("Resource type: " + resource.Resource.GetType() +
                                                  " should be a PackedScene. " + resource.Path);
            }
            if (setter.Type.IsGenericType && setter.Type.GetGenericTypeDefinition() == typeof(Func<>)) {
                // [Scene("")] private Func<object> field
                var packedSceneToInstanceFunction = CreatePackedScene.CreateFunc(packedScene, setter.Type);
                setter.SetValue(target, packedSceneToInstanceFunction);
            } else if (typeof(Node).IsAssignableFrom(setter.Type)) {
                // [Scene("")] private Node(or child of) field
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
        
        private static class CreatePackedScene {
            private static Func<T> PackedSceneToInstance<T>(PackedScene value) where T : class =>
                () => value.Instance<T>();

            private static readonly MethodInfo ComposeMethod = typeof(CreatePackedScene)
                .GetMethod(nameof(PackedSceneToInstance), BindingFlags.Static | BindingFlags.NonPublic)!;

            internal static Delegate CreateFunc(PackedScene resource, Type returnType) {
                var closure = (Delegate)ComposeMethod.MakeGenericMethod(returnType.GetGenericArguments()[0])
                    .Invoke(null, new[] { resource });
                return closure;
            }
        }
    }
}