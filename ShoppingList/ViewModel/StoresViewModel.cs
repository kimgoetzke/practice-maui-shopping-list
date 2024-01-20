using System.Collections.ObjectModel;
using System.Globalization;
using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ShoppingList.Models;
using ShoppingList.Services;

#pragma warning disable CS4014

namespace ShoppingList.ViewModel;

public partial class StoresViewModel : ObservableObject
{
    [ObservableProperty] private ObservableCollection<ConfigurableStore> _stores;
    [ObservableProperty] private ConfigurableStore _newStore;
    private readonly StoreService _storeService;


    public StoresViewModel(StoreService storeService)
    {
        _newStore = new ConfigurableStore();
        Stores = [];
        _storeService = storeService;
    }

    [RelayCommand]
    private async Task AddStore()
    {
        // Don't add empty items
        if (string.IsNullOrWhiteSpace(NewStore.Name))
            return;

        // Only allow unique store names
        if (Stores.Any(store => store.Name == NewStore.Name))
        {
            await ShowToast("Cannot add: " + NewStore.Name + " - it already exists");
            return;
        }

        // Capitalise first letter of each word
        var textInfo = new CultureInfo("en-US", false).TextInfo;
        NewStore.Name = textInfo.ToTitleCase(NewStore.Name.ToLower());

        // Add to list and database
        Stores.Add(NewStore);
        await _storeService.SaveStoreAsync(NewStore);

        // Make sure the UI is reset/updated
        NewStore = new ConfigurableStore();
        OnPropertyChanged(nameof(NewStore));

        // Show toast on success
        await ShowToast("Added: " + NewStore.Name);
    }

    [RelayCommand]
    private async Task RemoveStore(ConfigurableStore s)
    {
        if (s.Name == StoreService.DefaultStoreName)
        {
            ShowToast("Cannot remove default store");
            return;
        }

        Stores.Remove(s);
        await _storeService.DeleteStoreAsync(s);
        ShowToast("Removed: " + s.Name);
    }

    [RelayCommand]
    private async Task ResetStores()
    {
        await _storeService.ResetStoresAsync();
        await LoadStoresFromDatabase();
        ShowToast("Reset stores");
    }

    [RelayCommand]
    private static async Task GoBack()
    {
        await Shell.Current.GoToAsync("..", true);
    }

    public async Task LoadStoresFromDatabase()
    {
        var loadedItems = await _storeService.GetStoresAsync();
        Stores.Clear();
        foreach (var i in loadedItems)
            Stores.Add(i);
    }

    private static async Task ShowToast(string message)
    {
        var cancellationTokenSource = new CancellationTokenSource();
        var toast = Toast.Make(message);
        await toast.Show(cancellationTokenSource.Token);
    }
}