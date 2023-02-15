namespace Veronenger.Items;

public abstract class Item {
    public readonly int Id;
    public readonly string Name;
    public readonly string? Alias;

    public Item(int id, string name, string alias) {
        Id = id;
        Name = name;
        Alias = alias;
    }
}