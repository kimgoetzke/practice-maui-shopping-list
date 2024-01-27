using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ShoppingList.Models;
using ShoppingList.Services;
using ShoppingList.Utilities;
using ShoppingList.Views;
using StringProcessor = ShoppingList.Utilities.StringProcessor;

namespace ShoppingList.ViewModel;

public partial class MainViewModel : ObservableObject
{
    [ObservableProperty]
    private ObservableCollection<Item> _items = [];

    [ObservableProperty]
    private ObservableCollection<ConfigurableStore> _stores = [];

    [ObservableProperty]
    private Item _newItem;

    [ObservableProperty]
    private ConfigurableStore? _currentStore;
    private readonly IStoreService _storeService;
    private readonly IItemService _itemService;
    private readonly IClipboardService _clipboardService;

    public MainViewModel(
        IStoreService storeService,
        IItemService itemService,
        IClipboardService clipboardService
    )
    {
        _storeService = storeService;
        _itemService = itemService;
        _clipboardService = clipboardService;
        NewItem = new Item();
    }

    [RelayCommand]
    private async Task AddItem()
    {
        // Don't add empty items
        if (string.IsNullOrWhiteSpace(NewItem.Title))
            return;

        // Capitalise first letter of each word
        NewItem.Title = StringProcessor.ProcessItemTitle(NewItem.Title);
        NewItem.StoreName = CurrentStore!.Name;

        // Add to list and database
        Items.Add(NewItem);
        await _itemService.CreateOrUpdateAsync(NewItem);
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
        await _itemService.DeleteAsync(i);
    }

    [RelayCommand]
    private async Task RemoveAllItems()
    {
        if (!await IsRequestConfirmedByUser())
            return;
        Items.Clear();
        await _itemService.DeleteAllAsync();
        Notifier.ShowToast("Removed all items from list");
    }

    private static async Task<bool> IsRequestConfirmedByUser()
    {
        return await Shell.Current.DisplayAlert(
            "Clear list",
            $"This will remove all items from your list. Are you sure you want to continue?",
            "Yes",
            "No"
        );
    }

    [RelayCommand]
    private async Task TogglePriority(Item i)
    {
        i.IsImportant = !i.IsImportant;
        await _itemService.CreateOrUpdateAsync(i);
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
    internal void InsertFromClipboard()
    {
        _clipboardService.InsertFromClipboardAsync(Stores, Items);
    }

    public void SortItems()
    {
        Items = new ObservableCollection<Item>(
            Items.OrderBy(i => i.StoreName).ThenByDescending(i => i.AddedOn)
        );
    }

    public async Task LoadItemsFromDatabase()
    {
        var loadedItems = await _itemService.GetAsync();
        Items.Clear();
        foreach (var i in loadedItems)
            Items.Add(i);
    }

    public async Task LoadStoresFromService()
    {
        var loadedStores = await _storeService.GetAllAsync();
        Stores.Clear();
        foreach (var s in loadedStores)
        {
            Stores.Add(s);
            if (s.Name == IStoreService.DefaultStoreName)
            {
                CurrentStore = s;
            }
        }
    }
}
