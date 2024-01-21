using System.Collections.ObjectModel;
using System.Globalization;
using System.Text.RegularExpressions;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ShoppingList.Models;
using ShoppingList.Services;
using ShoppingList.Views;

namespace ShoppingList.ViewModel;

public partial class MainViewModel : ObservableObject
{
    [ObservableProperty] private ObservableCollection<Item> _items = [];
    [ObservableProperty] private ObservableCollection<ConfigurableStore> _stores = [];
    [ObservableProperty] private Item _newItem;
    [ObservableProperty] private ConfigurableStore? _currentStore;
    private readonly StoreService _storeService;
    private readonly ItemService _itemService;

    public MainViewModel(StoreService storeService, ItemService itemService)
    {
        _storeService = storeService;
        _itemService = itemService;
        NewItem = new Item();
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
        NewItem.StoreName = CurrentStore!.Name;

        // Add to list and database
        Items.Add(NewItem);
        await _itemService.SaveItemAsync(NewItem);
        Notifier.ShowToast($"Added: {NewItem.Title}");

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
        Notifier.ShowToast("Removed all items from list");
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
        Notifier.ShowToast("Copied list to clipboard");
    }

    [RelayCommand]
    internal async Task InsertFromClipboard()
    {
        var import = await Clipboard.GetTextAsync();
        if (import == null)
        {
            Notifier.ShowToast("Nothing to import - your clipboard is empty");
            return;
        }

        Logger.Log("Extracted from clipboard: " + import.Replace(Environment.NewLine, ""));
        var text = import.Replace(Environment.NewLine, "").Split(",");
        var addedItems = 0;
        foreach (var s in text)
        {
            if (string.IsNullOrWhiteSpace(s))
                continue;
            var (title, quantity) = ProcessItem(s);
            var item = new Item
                { Title = title.Trim(), StoreName = StoreService.DefaultStoreName, Quantity = quantity };
            await _itemService.SaveItemAsync(item);
            Items.Add(item);
            addedItems++;
        } 
        
        if (addedItems == 0)
        {
            Notifier.ShowToast("Nothing to import - your clipboard contains invalid data");
            return;
        }

        Logger.Log("Extracted from clipboard: " + import.Replace(Environment.NewLine, ""));
        Notifier.ShowToast($"Imported {addedItems} list from clipboard");
    }

    private static (string, int) ProcessItem(string input)
    {
        var match = Regex.Match(input, @"(.*)\((\d+)\)");
        if (!match.Success) return (input, 1);
        var itemName = match.Groups[1].Value.Trim();
        if (int.TryParse(match.Groups[2].Value, out var quantity))
        {
            return (itemName, quantity);
        }

        return (itemName, 1);
    }

    public void SortItems()
    {
        Items = new ObservableCollection<Item>(Items
            .OrderBy(i => i.StoreName)
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
        {
            Stores.Add(s);
            if (s.Name == StoreService.DefaultStoreName)
            {
                CurrentStore = s;
            }
        }
    }
}