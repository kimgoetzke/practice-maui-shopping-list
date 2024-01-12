using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ShoppingList.Models;

namespace ShoppingList.ViewModel;

[QueryProperty("Item", "Item")]
public partial class DetailViewModel : ObservableObject
{
    [ObservableProperty] Item item;
    
    public DetailViewModel(Item item)
    {
        Item = item;
    }

    [RelayCommand]
    async Task GoBack()
    {
        await Shell.Current.GoToAsync("..");
    }
}