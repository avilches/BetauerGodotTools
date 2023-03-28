using System;
using System.Reflection;

namespace Betauer.DI.Attributes;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method | AttributeTargets.Property)]
public abstract class ServiceTemplateAttribute : Attribute {
    public abstract ProviderTemplate CreateProviderTemplate(MemberInfo memberInfo);
}

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method | AttributeTargets.Property)]
public abstract class FactoryTemplateAttribute : Attribute {
    public abstract FactoryTemplate CreateFactoryTemplate(MemberInfo memberInfo);
}