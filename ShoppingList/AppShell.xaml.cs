using ShoppingList.Views;

namespace ShoppingList;

public partial class AppShell : Shell
{
    public AppShell()
    {
        InitializeComponent();

        this.FindByName<ShellContent>("MainPageContent").ContentTemplate =
            DeviceInfo.Current.Platform == DevicePlatform.WinUI
                ? new DataTemplate(typeof(MainPageWindows))
                : new DataTemplate(typeof(MainPageAndroid));
        Routing.RegisterRoute(nameof(DetailPage), typeof(DetailPage));
        Routing.RegisterRoute(nameof(StoresPage), typeof(StoresPage));
    }
}
