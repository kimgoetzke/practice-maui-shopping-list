using CommunityToolkit.Mvvm.ComponentModel;
using SQLite;
using SQLiteNetExtensions.Attributes;

namespace ShoppingList.Models;

public partial class ConfigurableStore : ObservableObject
{
    [PrimaryKey, AutoIncrement] public int Id { get; set; }

    [ObservableProperty] private string _name = string.Empty;

    [OneToMany(CascadeOperations = CascadeOperation.All)]
    public List<Item> Items { get; set; } = [];
    
    public override string ToString()
    {
        // Replace with the string you want to display in the Picker
        return Name;
    }
}