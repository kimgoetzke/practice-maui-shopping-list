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
}