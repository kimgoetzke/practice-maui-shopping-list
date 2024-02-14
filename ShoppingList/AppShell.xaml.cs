using ShoppingList.Views;

namespace ShoppingList;

public partial class AppShell
{
    public AppShell()
    {
        InitializeComponent();
        Routing.RegisterRoute(nameof(DetailPage), typeof(DetailPage));
        Routing.RegisterRoute(nameof(StoresPage), typeof(StoresPage));
    }
}
