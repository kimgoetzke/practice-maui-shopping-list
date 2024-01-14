using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ShoppingList.Models;

namespace ShoppingList.ViewModel;

[QueryProperty("Item", "Item")]
public partial class DetailViewModel : ObservableObject
{
    [ObservableProperty] Item item;
    
    public List<Store> StoreOptions { get; }  = Enum.GetValues(typeof(Store)).Cast<Store>().ToList();
    
    public DetailViewModel(Item item)
    {
        Item = item;
    }

    [RelayCommand]
    private static async Task GoBack()
    {
        await Shell.Current.GoToAsync("..");
    }
}