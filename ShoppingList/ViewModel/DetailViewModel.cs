using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ShoppingList.Data;
using ShoppingList.Models;

namespace ShoppingList.ViewModel;

[QueryProperty("Item", "Item")]
public partial class DetailViewModel : ObservableObject
{
    [ObservableProperty] Item item;
    private readonly ItemDatabase _database;

    public List<Store> StoreOptions { get; } = Enum.GetValues(typeof(Store)).Cast<Store>().ToList();

    public DetailViewModel(Item item, ItemDatabase database)
    {
        Item = item;
        _database = database;
    }

    [RelayCommand]
    private async Task GoBack()
    {
        await _database.SaveItemAsync(Item);
        var cancellationTokenSource = new CancellationTokenSource();
        var toast = Toast.Make("Updated: " + Item.Title);
#pragma warning disable CS4014
        toast.Show(cancellationTokenSource.Token);
#pragma warning restore CS4014
        await Shell.Current.GoToAsync("..", true);
    }
}