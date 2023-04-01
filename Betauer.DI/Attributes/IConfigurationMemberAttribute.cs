using Betauer.Tools.Reflection;

namespace Betauer.DI.Attributes;

public interface IConfigurationMemberAttribute {
    public void CreateProvider(object configuration, IGetter getter, Container.Builder builder);
}