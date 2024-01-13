using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ShoppingList.Models;

namespace ShoppingList.ViewModel;

public partial class MainViewModel : ObservableObject
{
    [ObservableProperty] ObservableCollection<Item> items;
    [ObservableProperty] Item newItem;
    [ObservableProperty] ObservableCollection<string> tasks;
    [ObservableProperty] string newTask;
    IConnectivity connectivity;

    public List<Store> StoreOptions { get; } = Enum.GetValues(typeof(Store)).Cast<Store>().ToList();

    public MainViewModel(IConnectivity connectivity)
    {
        Tasks = [];
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
        OnPropertyChanged(nameof(NewItem));
    }

    [RelayCommand]
    private void RemoveItem(Item i)
    {
        Items.Remove(i);
    }

    [RelayCommand]
    private async Task TapItem(Item i)
    {
        await Shell.Current.Navigation.PushAsync(new DetailPage(i));
    }

    // Tasks
    [RelayCommand]
    private async Task Add()
    {
        if (string.IsNullOrWhiteSpace(NewTask))
            return;

        if (connectivity.NetworkAccess != NetworkAccess.Internet)
        {
            await Shell.Current.DisplayAlert("Uh Oh!", "No Internet", "OK");
            return;
        }

        Tasks.Add(NewTask);
        NewTask = string.Empty;
    }

    [RelayCommand]
    private void Remove(string s)
    {
        Tasks.Remove(s);
    }

    [RelayCommand]
    private async Task Tap(string s)
    {
        await Shell.Current.GoToAsync($"{nameof(DetailPage)}?Task={s}");
    }
}