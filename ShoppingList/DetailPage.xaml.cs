using ShoppingList.Models;
using ShoppingList.ViewModel;

namespace ShoppingList;

public partial class DetailPage : ContentPage
{
    public DetailPage(Item item)
    {
        InitializeComponent();
        BindingContext = new DetailViewModel(item);
    }
    
    protected override void OnAppearing()
    {
        base.OnAppearing();
        ((DetailViewModel)BindingContext).Item.From = ((DetailViewModel)BindingContext).Item.From;
    }
}