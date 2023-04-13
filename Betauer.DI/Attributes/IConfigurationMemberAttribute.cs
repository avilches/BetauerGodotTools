using Betauer.Tools.FastReflection;

namespace Betauer.DI.Attributes;

public interface IConfigurationMemberAttribute {
    public void CreateProvider(object configuration, IGetter getter, Container.Builder builder);
}