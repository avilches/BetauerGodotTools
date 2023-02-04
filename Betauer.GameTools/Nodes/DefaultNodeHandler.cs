namespace Betauer.Nodes;

public class DefaultNodeHandler {
    public static readonly NodeHandler Instance = new() {
        Name = "NodeHandler",
        ZIndex = 1024
    };
}