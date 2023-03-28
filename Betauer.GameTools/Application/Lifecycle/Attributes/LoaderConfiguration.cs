using Betauer.DI.Attributes;

namespace Betauer.Application.Lifecycle.Attributes;

public class LoaderConfiguration : ConfigurationAttribute {
    public string Tag { get; set; }

    public LoaderConfiguration() {
    }

    public LoaderConfiguration(string tag) {
        Tag = tag;
    }
}