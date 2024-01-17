﻿using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ShoppingList.Data;
using ShoppingList.Models;

namespace ShoppingList.ViewModel;

public partial class MainViewModel : ObservableObject
{
    [ObservableProperty] ObservableCollection<Item> items;
    [ObservableProperty] Item newItem;
    private readonly IConnectivity _connectivity;
    private readonly ItemDatabase _database;

    public List<Store> StoreOptions { get; } = Enum.GetValues(typeof(Store)).Cast<Store>().ToList();

    public MainViewModel(IConnectivity connectivity, ItemDatabase database)
    {
        NewItem = new Item { From = Store.Anywhere };
        Items = [];
        _database = database;
        _connectivity = connectivity;
    }

    // Items
    [RelayCommand]
    private async Task AddItem()
    {
        if (string.IsNullOrWhiteSpace(NewItem.Title))
            return;

        if (_connectivity.NetworkAccess != NetworkAccess.Internet)
        {
            await Shell.Current.DisplayAlert("Uh Oh!", "No Internet", "OK");
            return;
        }

        Items.Add(NewItem);
        await _database.SaveItemAsync(NewItem);
        NewItem = new Item();
        SortItems();
        OnPropertyChanged(nameof(NewItem));
    }

    [RelayCommand]
    private async Task RemoveItem(Item i)
    {
        Items.Remove(i);
        await _database.DeleteItemAsync(i);
    }

    [RelayCommand]
    private async Task TogglePriority(Item i)
    {
        i.IsImportant = !i.IsImportant;
        await _database.SaveItemAsync(i);
        SortItems();
    }

    [RelayCommand]
    private async Task TapItem(Item i)
    {
        await Shell.Current.Navigation.PushAsync(new DetailPage(i, _database));
    }

    [RelayCommand]
    internal void CopyToClipboard()
    {
        var itemTitles = Items.Select(item => item.Title.Trim());
        var clipboardText = string.Join("," + Environment.NewLine, itemTitles);
        Clipboard.SetTextAsync(clipboardText);
        LogHandler.Log("Copied to clipboard: " + clipboardText.Replace(Environment.NewLine, ""));
    }

    public void SortItems()
    {
        Items = new ObservableCollection<Item>(Items
            .OrderByDescending(i => i.From.ToString())
            .ThenByDescending(i => i.AddedOn));
    }

    public async Task LoadItemsFromDatabase()
    {
        var loadedItems = await _database.GetItemsAsync();
        Items.Clear();
        foreach (var i in loadedItems)
            Items.Add(i);
    }
}