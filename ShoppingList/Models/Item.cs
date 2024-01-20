using CommunityToolkit.Mvvm.ComponentModel;
using SQLite;

namespace ShoppingList.Models;

public partial class Item : ObservableObject
{
    [PrimaryKey, AutoIncrement] public int Id { get; set; }
    [ObservableProperty] private string _title = string.Empty;
    [ObservableProperty] private int _quantity = 1;
    [ObservableProperty] private bool _isImportant;
    [ObservableProperty] private DateTime _addedOn = DateTime.Now;
    [ObservableProperty] private string _storeName = "<Not set>";

    public override string ToString()
    {
        return Title;
    }

    public string ToLoggableString()
    {
        return
            $"Item.ToString() = {Title} #{Id} (store: {StoreName}, quantity: {Quantity}, important: {IsImportant})";
    }
}