using System;

namespace Betauer.DI.Attributes;

public interface IClassAttribute {
    public void Apply(Type type, Container.Builder builder);
}