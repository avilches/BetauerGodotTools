namespace Betauer.DI.Attributes;

public interface IConfigurationClassAttribute {
    public void Apply(object configuration, Container.Builder builder);
}