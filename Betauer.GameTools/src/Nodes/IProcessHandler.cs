namespace Betauer.Nodes;

public interface IProcessHandler : IEventHandler {
    public void Handle(double delta);
}