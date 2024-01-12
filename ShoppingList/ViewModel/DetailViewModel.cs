using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace ShoppingList.ViewModel;

[QueryProperty("Task", "Task")]
public partial class DetailViewModel : ObservableObject
{
    [ObservableProperty] string task;

    [RelayCommand]
    async Task GoBack()
    {
        await Shell.Current.GoToAsync("..");
    }
}