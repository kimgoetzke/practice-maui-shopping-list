using CommunityToolkit.Mvvm.ComponentModel;
using SQLite;

namespace ShoppingList.Models;

public partial class ConfigurableStore : ObservableObject
{
    [PrimaryKey, AutoIncrement] public int Id { get; set; }

    [ObservableProperty] private string _name = string.Empty;

    public override string ToString()
    {
        return Name;
    }

    public string ToLoggableString()
    {
        return $"{Name} #{Id}";
    }
}