using ShoppingList.Models;
using ShoppingList.ViewModel;

namespace ShoppingList;

public partial class MainPage : ContentPage
{
    public MainPage(MainViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
    
    protected override void OnAppearing()
    {
        base.OnAppearing();
        ((MainViewModel)BindingContext).NewItem = new Item { From = Store.Anywhere };
    }
    
    private void OnEntryUnfocused(object sender, FocusEventArgs e)
    {
        AddButton.Focus();
    }
}