using ShoppingList.Models;
using ShoppingList.ViewModel;

namespace ShoppingList;

public partial class DetailPage : ContentPage
{
    public DetailPage(Item item)
    {
        InitializeComponent();
        BindingContext = new DetailViewModel(item);;
    }
}