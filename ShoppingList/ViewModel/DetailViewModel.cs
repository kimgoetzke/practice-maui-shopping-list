using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ShoppingList.Data;
using ShoppingList.Models;
using ShoppingList.Services;

namespace ShoppingList.ViewModel;

[QueryProperty("Item", "Item")]
public partial class DetailViewModel : ObservableObject
{
    public ObservableCollection<ConfigurableStore> StoreOptions => _storesViewModel.Stores;
    [ObservableProperty] private Item _item;
    private readonly ItemService _itemService;
    private readonly StoresViewModel _storesViewModel;

    public DetailViewModel(Item item, StoresViewModel storesViewModel, ItemService itemService)
    {
        Item = item;
        _storesViewModel = storesViewModel;
        _itemService = itemService;
    }

    [RelayCommand]
    private async Task GoBack()
    {
        await _itemService.SaveItemAsync(Item);

#pragma warning disable CS4014
        NotificationService.ShowToast("Updated: " + Item.Title);
#pragma warning restore CS4014

        await Shell.Current.GoToAsync("..", true);
    }
}