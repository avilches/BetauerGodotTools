using Betauer.DI.Attributes;

namespace Betauer.Application.Lifecycle.Attributes;

public class LoaderConfiguration : ConfigurationAttribute {
    public string Tag { get; set; }
    public string Name { get; set; }

    public LoaderConfiguration(string name) {
        Name = name;
    }

    public LoaderConfiguration(string name, string tag) {
        Tag = tag;
        Name = name;
    }
}