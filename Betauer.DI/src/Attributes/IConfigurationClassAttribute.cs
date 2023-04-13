namespace Betauer.DI.Attributes;

public interface IConfigurationClassAttribute {
    public void CreateProvider(object configuration, Container.Builder builder);
}