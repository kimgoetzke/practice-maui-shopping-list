namespace ShoppingList;

public partial class AppShell : Shell
{
    public AppShell()
    {
        InitializeComponent();
        Routing.RegisterRoute(nameof(DetailPage), typeof(DetailPage));
    }

    // TODO: Beginner: Make adding items form a collapsible menu.
    // TODO: Intermediate: Add options menu button with sections for 1) stores, 2) dark mode, and 3) more...
    // TODO: Intermediate: Make Store configurable (adding removing stores).
    // TODO: Advanced: Store Items in clouds and enable reusing between devices.
}