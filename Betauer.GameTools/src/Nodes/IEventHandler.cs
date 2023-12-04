namespace Betauer.Nodes;

public interface IEventHandler {
    string? Name { get; }
    public bool IsEnabled { get;  }
    public void Disable();
    public void Enable();

    public bool IsDestroyed { get; }
    public void Destroy();
}