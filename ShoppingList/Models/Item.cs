
using CommunityToolkit.Mvvm.ComponentModel;
using SQLite;
using SQLiteNetExtensions.Attributes;

namespace ShoppingList.Models;

public partial class Item : ObservableObject
{
    [PrimaryKey, AutoIncrement] public int Id { get; set; }

    [ObservableProperty] private string title = string.Empty;

    [ObservableProperty] private int quantity = 1;

    [ObservableProperty] private bool isImportant;

    [ObservableProperty] private Store from = Store.Anywhere;

    [ObservableProperty] private DateTime addedOn = DateTime.Now;
    
    // [ManyToOne("Id")] public ConfigurableStore ConfigurableStore { get; set; }
}