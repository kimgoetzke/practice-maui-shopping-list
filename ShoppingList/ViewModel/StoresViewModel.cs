using System.Collections.ObjectModel;
using System.Globalization;
using CommunityToolkit.Maui.Core.Platform;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ShoppingList.Models;
using ShoppingList.Services;
using ShoppingList.Utilities;

namespace ShoppingList.ViewModel;

public partial class StoresViewModel : ObservableObject
{
    [ObservableProperty]
    private ObservableCollection<ConfigurableStore> _stores;

    [ObservableProperty]
    private ConfigurableStore _newStore;
    private readonly IStoreService _storeService;

    public StoresViewModel(IStoreService storeService)
    {
        Stores = [];
        _newStore = new ConfigurableStore();
        _storeService = storeService;
    }

    [RelayCommand]
    private async Task AddStore(ITextInput view)
    {
        // Don't add empty items
        if (string.IsNullOrWhiteSpace(NewStore.Name))
            return;
        
        // Pre-process store
        NewStore.Name = StringProcessor.TrimAndCapitaliseFirstChar(NewStore.Name);
        
        // Only allow unique store names
        if (Stores.Any(store => store.Name == NewStore.Name))
        {
            await Notifier.AwaitShowToast($"Cannot add '{NewStore.Name}' - it already exists");
            return;
        }

        // Add to list and database
        Stores.Add(NewStore);
        await _storeService.CreateOrUpdateAsync(NewStore);

        // Show toast on success
        await Notifier.AwaitShowToast($"Added: {NewStore.Name}");

        // Make sure the UI is reset/updated
#pragma warning disable CA1416
        var isKeyboardHidden = view.HideKeyboardAsync(CancellationToken.None);
#pragma warning restore CA1416
        Logger.Log("Keyboard hidden: " + isKeyboardHidden);
        NewStore = new ConfigurableStore();
        OnPropertyChanged(nameof(NewStore));
        OnPropertyChanged(nameof(IsCollectionViewLargerThanThreshold));
    }

    [RelayCommand]
    private async Task RemoveStore(ConfigurableStore s)
    {
        if (s.Name == IStoreService.DefaultStoreName)
        {
            Notifier.ShowToast("Cannot remove default store");
            return;
        }

        Stores.Remove(s);
        await _storeService.DeleteAsync(s);
        OnPropertyChanged(nameof(IsCollectionViewLargerThanThreshold));
        Notifier.ShowToast($"Removed: {s.Name}");
    }

    [RelayCommand]
    private async Task ResetStores()
    {
        if (!await IsRequestConfirmedByUser())
            return;
        await _storeService.DeleteAllAsync();
        await LoadStoresFromDatabase();
        OnPropertyChanged(nameof(IsCollectionViewLargerThanThreshold));
        Notifier.ShowToast("Reset stores");
    }

    private static async Task<bool> IsRequestConfirmedByUser()
    {
        return await Shell.Current.DisplayAlert(
            "Reset stores",
            $"This will remove all stores, except the 'Anywhere' store. Are you sure you want to continue?",
            "Yes",
            "No"
        );
    }

    [RelayCommand]
    private static async Task GoBack()
    {
        await Shell.Current.GoToAsync("..", true);
    }

    public async Task LoadStoresFromDatabase()
    {
        var loadedItems = await _storeService.GetAllAsync();
        Stores.Clear();
        foreach (var i in loadedItems)
            Stores.Add(i);
    }

    // Used to toggle on/off the line separator between stores list and buttons
    public bool IsCollectionViewLargerThanThreshold
    {
        get
        {
            const int itemHeight = 45; // As defined in Styles.XAML
            var currentHeight = Stores.Count * itemHeight;
            return currentHeight >= 270;
        }
    }
}
