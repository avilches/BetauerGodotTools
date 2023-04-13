using System;

namespace Betauer.DI.Attributes;

public interface IClassAttribute {
    public void CreateProvider(Type type, Container.Builder builder);
}