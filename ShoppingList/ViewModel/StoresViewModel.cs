using System.Collections.ObjectModel;
using System.Globalization;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Maui.Core.Platform;
using ShoppingList.Models;
using ShoppingList.Services;

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
    private async Task AddStore(ITextInput view)
    {
        // Don't add empty items
        if (string.IsNullOrWhiteSpace(NewStore.Name))
            return;

        // Only allow unique store names
        if (Stores.Any(store => store.Name == NewStore.Name))
        {
            await Notifier.AwaitShowToast($"Cannot add '{NewStore.Name}' - it already exists");
            return;
        }

        // Capitalise first letter of each word
        var textInfo = new CultureInfo("en-US", false).TextInfo;
        NewStore.Name = textInfo.ToTitleCase(NewStore.Name.ToLower());

        // Add to list and database
        Stores.Add(NewStore);
        await _storeService.SaveStoreAsync(NewStore);

        // Make sure the UI is reset/updated
        var isKeyboardHidden = view.HideKeyboardAsync(CancellationToken.None);
        Logger.Log("Keyboard hidden: " + isKeyboardHidden);
        NewStore = new ConfigurableStore();
        OnPropertyChanged(nameof(NewStore));
        OnPropertyChanged(nameof(IsCollectionViewLargerThanThreshold));

        // Show toast on success
        await Notifier.AwaitShowToast($"Added: {NewStore.Name}");
    }

    [RelayCommand]
    private async Task RemoveStore(ConfigurableStore s)
    {
        if (s.Name == StoreService.DefaultStoreName)
        {
            Notifier.ShowToast("Cannot remove default store");
            return;
        }

        Stores.Remove(s);
        await _storeService.DeleteStoreAsync(s);
        OnPropertyChanged(nameof(IsCollectionViewLargerThanThreshold));
        Notifier.ShowToast($"Removed: {s.Name}");
    }

    [RelayCommand]
    private async Task ResetStores()
    {
        if (!await IsRequestConfirmedByUser()) return;
        await _storeService.ResetStoresAsync();
        await LoadStoresFromDatabase();
        OnPropertyChanged(nameof(IsCollectionViewLargerThanThreshold));
        Notifier.ShowToast("Reset stores");
    }

    private static async Task<bool> IsRequestConfirmedByUser()
    {
        var isConfirmed =
            await Shell.Current.DisplayAlert("Reset stores",
                $"This will remove all stores, except the 'Anywhere' store. Are you sure you want to continue?", 
                "Yes",
                "No");
        if (!isConfirmed) Notifier.ShowToast("Request cancelled");
        return isConfirmed;
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