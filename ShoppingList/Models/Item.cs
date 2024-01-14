using CommunityToolkit.Mvvm.ComponentModel;
using SQLite;

namespace ShoppingList.Models;

public partial class Item : ObservableObject
{
    [PrimaryKey, AutoIncrement] public int Id { get; set; }

    [ObservableProperty] string title = string.Empty;

    [ObservableProperty] int quantity = 1;

    [ObservableProperty] bool isImportant;

    [ObservableProperty] Store from = Store.Anywhere;

    [ObservableProperty] DateTime addedOn = DateTime.Now;
}