using System;
using Betauer.Tools.Reflection;

namespace Betauer.DI.Attributes;

[AttributeUsage(AttributeTargets.Class)]
public abstract class ServiceTemplateClassAttribute : Attribute {
    public abstract ServiceTemplate CreateServiceTemplate(object configuration);
}

[AttributeUsage(AttributeTargets.Class)]
public abstract class FactoryTemplateClassAttribute : Attribute {
    public abstract FactoryTemplate CreateFactoryTemplate(object configuration);
}

public abstract class MemberTemplateAttribute : Attribute {}

[AttributeUsage(AttributeTargets.Property | AttributeTargets.Method)]
public abstract class ServiceTemplateAttribute : MemberTemplateAttribute {
    public abstract ServiceTemplate CreateServiceTemplate(object configuration, IGetter getter);
}

[AttributeUsage(AttributeTargets.Property | AttributeTargets.Method)]
public abstract class FactoryTemplateAttribute : MemberTemplateAttribute {
    public abstract FactoryTemplate CreateFactoryTemplate(object configuration, IGetter getter);
}