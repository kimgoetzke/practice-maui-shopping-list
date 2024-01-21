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
        if (!await IsRequestConfirmedByUser()) return;
        Items.Clear();
        await _itemService.DeleteAllItemsAsync();
        Notifier.ShowToast("Removed all items from list");
    }

    private static async Task<bool> IsRequestConfirmedByUser()
    {
        var isConfirmed =
            await Shell.Current.DisplayAlert("Remove all items from list", $"Are you sure you want to continue?", "Yes", "No");
        if (!isConfirmed) Notifier.ShowToast("Request cancelled");
        return isConfirmed;
    }

    [RelayCommand]
    private async Task TogglePriority(Item i)
    {
        i.IsImportant = !i.IsImportant;
        await _itemService.SaveItemAsync(i);
        SortItems();
    }

    [RelayCommand]
    private static async Task TapItem(Item i)
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
        if (IsClipboardEmpty(import)) return;
        if (!WasAbleToConvertToItemList(import!, out var addedItems, out var toImport)) return;
        if (!await IsImportConfirmedByUser(addedItems)) return;
        await ImportItemList(toImport, import!);
        Notifier.ShowToast($"Imported {addedItems} list from clipboard");
    }

    private static bool IsClipboardEmpty(string? import)
    {
        if (import != null) return false;
        Notifier.ShowToast("Nothing to import - your clipboard is empty");
        return true;
    }

    private static bool WasAbleToConvertToItemList(string import, out int addedItems, out List<Item> toImport)
    {
        Logger.Log("Extracted from clipboard: " + import.Replace(Environment.NewLine, ""));
        var text = import.Replace(Environment.NewLine, "").Split(",");
        addedItems = 0;
        toImport = [];
        foreach (var s in text)
        {
            if (string.IsNullOrWhiteSpace(s))
                continue;
            var (title, quantity) = ProcessStrings(s);
            var item = new Item
                { Title = title.Trim(), StoreName = StoreService.DefaultStoreName, Quantity = quantity };
            toImport.Add(item);
            addedItems++;
        }

        if (addedItems != 0) return true;
        Notifier.ShowToast("Nothing to import - your clipboard may contain invalid data");
        return false;
    }

    private static async Task<bool> IsImportConfirmedByUser(int addedItems)
    {
        var isConfirmed = await Shell.Current.DisplayAlert(
            "Import from clipboard",
            $"Extracted {addedItems} item(s) from your clipboard. Would you like to add the item(s) to your list?",
            "Yes",
            "No");

        if (!isConfirmed) Notifier.ShowToast("Import cancelled");
        return isConfirmed;
    }

    private async Task ImportItemList(List<Item> toImport, string import)
    {
        foreach (var item in toImport)
        {
            await _itemService.SaveItemAsync(item);
            Items.Add(item);
        }

        Logger.Log("Extracted from clipboard: " + import.Replace(Environment.NewLine, ""));
    }

    private static (string, int) ProcessStrings(string input)
    {
        var match = Regex.Match(input, @"(.*)\((\d+)\)");
        if (!match.Success) return (input, 1);
        var itemName = match.Groups[1].Value.Trim();
        return int.TryParse(match.Groups[2].Value, out var quantity) ? (itemName, quantity) : (itemName, 1);
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