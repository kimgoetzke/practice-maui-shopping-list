using SQLite;

namespace ShoppingList.Models;

public class ConfigurableStore
{
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;

    public override string ToString()
    {
        return Name;
    }

    public string ToLoggableString()
    {
        return $"{Name} #{Id}";
    }
}
