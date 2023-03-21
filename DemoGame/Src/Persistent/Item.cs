namespace Veronenger.Persistent;

public abstract class Item {
    public readonly int Id;
    public readonly string Name;
    public readonly string? Alias;

    protected Item(int id, string name, string alias) {
        Id = id;
        Name = name;
        Alias = alias;
    }
}