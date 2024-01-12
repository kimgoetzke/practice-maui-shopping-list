using CommunityToolkit.Mvvm.ComponentModel;

namespace ShoppingList.Models;

[INotifyPropertyChanged]
public partial class Item
{
    [ObservableProperty] string title = string.Empty;

    [ObservableProperty] int quantity = 1;

    [ObservableProperty] bool isImportant;

    [ObservableProperty] Store where = Store.Lidl;

    [ObservableProperty] DateTime createdAt = DateTime.Now;
}