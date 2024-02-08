using ShoppingList.Models;
using ShoppingList.Services;
using ShoppingList.ViewModel;

namespace ShoppingList.Views;

public partial class DetailPage
{
    public DetailPage(Item item)
    {
        InitializeComponent();
        var storeService = IPlatformApplication.Current?.Services.GetService<IStoreService>();
        var itemService = IPlatformApplication.Current?.Services.GetService<IItemService>();
        if (itemService is null || storeService is null)
            throw new NullReferenceException("ItemService or StoreService is null");
        BindingContext = new DetailViewModel(item, storeService, itemService);
    }
}
