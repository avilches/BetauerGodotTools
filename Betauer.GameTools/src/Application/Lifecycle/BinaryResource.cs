using Godot;

namespace Betauer.Application.Lifecycle;

public partial class BinaryResource(byte[] data) : Resource {
    public byte[] Data { get; } = data;
}