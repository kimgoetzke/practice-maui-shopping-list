namespace ShoppingList;

public partial class AppShell : Shell
{
    public AppShell()
    {
        InitializeComponent();
        Routing.RegisterRoute(nameof(DetailPage), typeof(DetailPage));
    }
    
    // TODO: Beginner: Add copy to clipboard button.
    // TODO: Beginner: Add options menu button with sections for 1) stores, 2) dark mode, and 3) more...
    // TODO: Beginner: Make adding items form a collapsible menu.
    // TODO: Intermediate: Store notes permanently on device.
    // TODO: Intermediate: Make Store configurable (adding removing stores).
    // TODO: Advanced: Store Items in clouds and enable reusing between devices.
}