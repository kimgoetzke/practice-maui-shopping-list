using ShoppingList.Data;
using ShoppingList.Models;
using ShoppingList.ViewModel;

namespace ShoppingList;

public partial class DetailPage
{
    public DetailPage(Item item, ItemDatabase database)
    {
        InitializeComponent();
        BindingContext = new DetailViewModel(item, database);
    }
    
    protected override void OnAppearing()
    {
        base.OnAppearing();
        ((DetailViewModel)BindingContext).Item.From = ((DetailViewModel)BindingContext).Item.From;
    }
}