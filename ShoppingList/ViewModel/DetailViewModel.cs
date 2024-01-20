using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ShoppingList.Models;
using ShoppingList.Services;

namespace ShoppingList.ViewModel;

[QueryProperty("Item", "Item")]
public partial class DetailViewModel : ObservableObject
{
    [ObservableProperty] private ObservableCollection<ConfigurableStore> _stores = [];
    [ObservableProperty] private Item _item;
    [ObservableProperty] private ConfigurableStore _currentStore;

    private readonly ItemService _itemService;
    private readonly StoreService _storeService;

    public DetailViewModel(Item item, StoreService storeService, ItemService itemService)
    {
        Item = item;
        CurrentStore = new ConfigurableStore();
        _storeService = storeService;
        _itemService = itemService;
        SetStoreOptions();
    }

    [RelayCommand]
    private async Task GoBack()
    {
        Item.StoreName = CurrentStore.Name;
        await _itemService.SaveItemAsync(Item);

#pragma warning disable CS4014
        NotificationService.AwaitShowToast($"Updated: {Item.Title}");
#pragma warning restore CS4014
        
        await Shell.Current.GoToAsync("..", true);
    }

    private async void SetStoreOptions()
    {
        var loadedStores = await _storeService.GetStoresAsync();
        Stores.Clear();
        foreach (var s in loadedStores)
        {
            Stores.Add(s);
            if (s.Name == Item.StoreName)
            {
                CurrentStore = s;
            }
        }
    }
}