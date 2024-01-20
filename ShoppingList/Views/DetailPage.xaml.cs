using ShoppingList.Models;
using ShoppingList.Services;
using ShoppingList.ViewModel;
using ServiceProvider = ShoppingList.Services.ServiceProvider;

namespace ShoppingList.Views;

public partial class DetailPage
{
    public DetailPage(Item item)
    {
        InitializeComponent();
        var storeService = ServiceProvider.GetService<StoreService>();
        var itemService = ServiceProvider.GetService<ItemService>();
        BindingContext = new DetailViewModel(item, storeService, itemService);
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        ((DetailViewModel)BindingContext).Item.From = ((DetailViewModel)BindingContext).Item.From;
        Logger.Log($"{((DetailViewModel)BindingContext).Item.ToLoggableString()}");
    }
}