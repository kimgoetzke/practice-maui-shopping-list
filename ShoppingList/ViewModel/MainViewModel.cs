using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace ShoppingList.ViewModel;

public partial class MainViewModel : ObservableObject
{
    [ObservableProperty] ObservableCollection<string> tasks;
    [ObservableProperty] string newTask;
    IConnectivity connectivity;

    public MainViewModel(IConnectivity connectivity)
    {
        Tasks = [];
        this.connectivity = connectivity;
    }


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