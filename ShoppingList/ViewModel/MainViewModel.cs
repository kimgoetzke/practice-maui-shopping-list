using System.Collections.ObjectModel;
using System.Globalization;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ShoppingList.Data;
using ShoppingList.Models;
using ShoppingList.Services;

namespace ShoppingList.ViewModel;

public partial class MainViewModel : ObservableObject
{
    public DbProvider Database { get; }
    [ObservableProperty] private ObservableCollection<Item> _items;
    [ObservableProperty] private ObservableCollection<ConfigurableStore> _stores;
    [ObservableProperty] private Item _newItem;
    private readonly IConnectivity _connectivity;
    private readonly StoresViewModel _storesViewModel;
    private readonly StoreService _storeService;
    private readonly ItemService _itemService;

    public ObservableCollection<ConfigurableStore> StoreOptions => _storesViewModel.Stores;

    public MainViewModel(IConnectivity connectivity, DbProvider database, StoreService storeService,
        ItemService itemService, StoresViewModel storesViewModel)
    {
        Items = [];
        Stores = [];
        Database = database;
        _connectivity = connectivity;
        _storeService = storeService;
        _itemService = itemService;
        _storesViewModel = storesViewModel;
        NewItem = new Item { From = Store.Anywhere };
    }

    // Items
    [RelayCommand]
    private async Task AddItem()
    {
        // Don't add empty items
        if (string.IsNullOrWhiteSpace(NewItem.Title))
            return;

        // Don't add items if there's no internet
        if (_connectivity.NetworkAccess != NetworkAccess.Internet)
        {
            await Shell.Current.DisplayAlert("Uh Oh!", "No Internet", "OK");
            return;
        }

        // Capitalise first letter of each word
        var textInfo = new CultureInfo("en-US", false).TextInfo;
        NewItem.Title = textInfo.ToTitleCase(NewItem.Title.ToLower());

        // Add to list and database
        Items.Add(NewItem);
        await _itemService.SaveItemAsync(NewItem);

        // Make sure the UI is reset/updated
        NewItem = new Item();
        SortItems();
        OnPropertyChanged(nameof(NewItem));

        // Show toast on success
        await NotificationService.ShowToast("Added: " + NewItem.Title);
    }

    [RelayCommand]
    private async Task RemoveItem(Item i)
    {
        Items.Remove(i);
        await _itemService.DeleteItemAsync(i);
    }

    [RelayCommand]
    private async Task RemoveAllItems()
    {
        Items.Clear();
        await _itemService.DeleteAllItemsAsync();
        NotificationService.ShowToast("Removed all items from list");
    }

    [RelayCommand]
    private async Task TogglePriority(Item i)
    {
        i.IsImportant = !i.IsImportant;
        await _itemService.SaveItemAsync(i);
        SortItems();
    }

    [RelayCommand]
    private async Task TapItem(Item i)
    {
        await Shell.Current.GoToAsync(nameof(DetailPage), true);
    }
    
    [RelayCommand]
    private async Task ManageStores()
    {
        await Shell.Current.GoToAsync(nameof(StoresPage), true);
    }

    [RelayCommand]
    internal void CopyToClipboard()
    {
        var itemTitles = Items.Select(item => item.Title.Trim());
        var clipboardText = string.Join("," + Environment.NewLine, itemTitles);
        Clipboard.SetTextAsync(clipboardText);
        LogHandler.Log("Copied to clipboard: " + clipboardText.Replace(Environment.NewLine, ""));
        NotificationService.ShowToast("Copied list to clipboard");
    }

    public void SortItems()
    {
        Items = new ObservableCollection<Item>(Items
            .OrderByDescending(i => i.From.ToString())
            .ThenByDescending(i => i.AddedOn));
    }

    public async Task LoadItemsFromDatabase()
    {
        var loadedItems = await _itemService.GetItemsAsync();
        Items.Clear();
        foreach (var i in loadedItems)
            Items.Add(i);
    }

    public async Task LoadStoresFromDatabase()
    {
        var loadedStores = await _storeService.GetStoresAsync();
        Stores.Clear();
        foreach (var s in loadedStores)
            Stores.Add(s);
    }
}