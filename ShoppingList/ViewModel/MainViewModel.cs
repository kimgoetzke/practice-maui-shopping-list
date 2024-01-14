using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ShoppingList.Models;

namespace ShoppingList.ViewModel;

public partial class MainViewModel : ObservableObject
{
    [ObservableProperty] ObservableCollection<Item> items;
    [ObservableProperty] Item newItem;
    IConnectivity connectivity;

    public List<Store> StoreOptions { get; } = Enum.GetValues(typeof(Store)).Cast<Store>().ToList();

    public MainViewModel(IConnectivity connectivity)
    {
        Items = [];
        NewItem = new Item { From = Store.Anywhere };
        this.connectivity = connectivity;
    }

    // Items
    [RelayCommand]
    private async Task AddItem()
    {
        if (string.IsNullOrWhiteSpace(NewItem.Title))
            return;

        if (connectivity.NetworkAccess != NetworkAccess.Internet)
        {
            await Shell.Current.DisplayAlert("Uh Oh!", "No Internet", "OK");
            return;
        }

        Items.Add(NewItem);
        NewItem = new Item();
        SortItems();
        OnPropertyChanged(nameof(NewItem));
    }

    [RelayCommand]
    private void RemoveItem(Item i)
    {
        Items.Remove(i);
    }

    [RelayCommand]
    private void TogglePriority(Item i)
    {
        i.IsImportant = !i.IsImportant;
        SortItems();
    }

    [RelayCommand]
    private static async Task TapItem(Item i)
    {
        await Shell.Current.Navigation.PushAsync(new DetailPage(i));
    }

    private void SortItems()
    {
        Items = new ObservableCollection<Item>(Items
            .OrderByDescending(i => i.From.ToString())
            .ThenByDescending(i => i.IsImportant)
            .ThenByDescending(i => i.Title));
    }
}