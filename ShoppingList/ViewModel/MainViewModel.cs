using System.Collections.ObjectModel;
using System.Globalization;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ShoppingList.Models;
using ShoppingList.Services;
using ShoppingList.Views;

namespace ShoppingList.ViewModel;

public partial class MainViewModel : ObservableObject
{
    [ObservableProperty] private ObservableCollection<Item> _items;
    [ObservableProperty] private ObservableCollection<ConfigurableStore> _stores = [];
    [ObservableProperty] private Item _newItem;
    private readonly StoreService _storeService;
    private readonly ItemService _itemService;

    public MainViewModel(StoreService storeService, ItemService itemService)
    {
        Items = [];
        _storeService = storeService;
        _itemService = itemService;
        NewItem = new Item { From = Store.Anywhere };
        PopulateDefaultStore();
    }

    private async void PopulateDefaultStore()
    {
        var defaultStore = await _storeService.DefaultStore();
        NewItem.ConfigurableStore = defaultStore;
    }

    [RelayCommand]
    private async Task AddItem()
    {
        // Don't add empty items
        if (string.IsNullOrWhiteSpace(NewItem.Title))
            return;

        // Capitalise first letter of each word
        var textInfo = new CultureInfo("en-US", false).TextInfo;
        NewItem.Title = textInfo.ToTitleCase(NewItem.Title.ToLower());

        // Add to list and database
        Items.Add(NewItem);
        await _itemService.SaveItemAsync(NewItem);
        NotificationService.ShowToast($"Added: {NewItem.Title}");

        // Make sure the UI is reset/updated
        NewItem = new Item();
        SortItems();
        OnPropertyChanged(nameof(NewItem));
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
        await Shell.Current.Navigation.PushAsync(new DetailPage(i));
        // await Shell.Current.GoToAsync(nameof(DetailPage), true, new Dictionary<string, object>
        // {
        //     ["Item"] = i
        // });
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
        Logger.Log("Copied to clipboard: " + clipboardText.Replace(Environment.NewLine, ""));
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

    public async Task LoadStoresFromService()
    {
        var loadedStores = await _storeService.GetStoresAsync();
        Stores.Clear();
        foreach (var s in loadedStores)
            Stores.Add(s);
    }
}