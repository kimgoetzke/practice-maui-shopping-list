using CommunityToolkit.Mvvm.ComponentModel;

namespace ShoppingList.Models;

public partial class Item : ObservableObject
{
    [ObservableProperty] string title = string.Empty;

    [ObservableProperty] int quantity = 1;

    [ObservableProperty] bool isImportant;

    [ObservableProperty] Store from = Store.Anywhere;

    [ObservableProperty] DateTime addedOn = DateTime.Now;
}