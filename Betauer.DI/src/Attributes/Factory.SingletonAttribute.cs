using System;
using Betauer.Core;
using Betauer.DI.Factory;
using Betauer.DI.ServiceProvider;
using Betauer.Tools.FastReflection;

namespace Betauer.DI.Attributes;

public static partial class Factory {
    /// <summary>
    /// If Name is null, the singleton only can be resolved by type. This only possible when using with classes. Using [Factory.Singleton] in a property or method
    /// will take the name from the variable/method, so "service" will be the default name in this example: [Factory.Singleton] IService service => new Service()
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method | AttributeTargets.Property)]
    public class SingletonAttribute : Attribute, IClassAttribute, IConfigurationMemberAttribute {
        public string? Scope { get; init; }
        public string? Name { get; init; }
        public string? Flags { get; init; }
        public bool Lazy { get; init; } = false;

        // Example:
        // [Factory.Singleton<Node>]
        // class Element3Factory : IFactory<Node3D> {}
        public void Apply(Type type, Container.Builder builder) {                                    // type = typeof(Element3Factory)
            var factory = Activator.CreateInstance(type)!;                                     // new ElementFactory();
            var factoryType = type.FindGenericsFromInterfaceDefinition(typeof(IFactory<>))[0];       // Node3D from IFactory<Node3D> 
            var publicType = GetType().IsGenericType                                                  
                ? GetType().GetGenericArguments()[0]                                                 // Node because [Factory.Singleton<Node>]
                : factoryType;                                                                       // Node3D 
            var create = FactoryTools.From(factory);
            var provider = new SingletonProvider(publicType, factoryType, Scope, create, Name, Lazy, Provider.FlagsToMetadata(Flags));
            builder.RegisterFactory(factory);
            builder.Register(provider);
            if (Lazy) builder.Register(Provider.Lazy(provider)); // Non lazy singletons don't need the ILazy<T>
        }

        
        // Example:
        // [Factory.Singleton<Node>] IFactory<Node3D> Element3 => new ElementFactory() // class Element3Factory : IFactory<Node3D> {}
        public void Apply(object configuration, IGetter getter, Container.Builder builder) {
            var factory = getter.GetValue(configuration)!;                                                // new ElementFactory();
            var factoryType = factory.GetType().FindGenericsFromInterfaceDefinition(typeof(IFactory<>))[0];     // Node3D
            var publicType = GetType().IsGenericType                                                           
                ? GetType().GetGenericArguments()[0] // Trick to get the <T> from SingletonAttribute<T>         // Node because [Factory.Singleton<Node>]    
                : getter.MemberType.FindGenericsFromInterfaceDefinition(typeof(IFactory<>))[0];                       // Node3D because IFactory<Node3D> Element3                                              
            var create = FactoryTools.From(factory);
            var provider =  new SingletonProvider(publicType, factoryType, Scope, create, Name ?? getter.Name, Lazy, Provider.FlagsToMetadata(Flags));
            builder.RegisterFactory(factory);
            builder.Register(provider);
            if (Lazy) builder.Register(Provider.Lazy(provider)); // Non lazy singletons don't need the ILazy<T>
        }
    }

    public class SingletonAttribute<T> : SingletonAttribute {
    }

}