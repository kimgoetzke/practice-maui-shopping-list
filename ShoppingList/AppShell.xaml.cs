namespace ShoppingList;

public partial class AppShell : Shell
{
    public AppShell()
    {
        InitializeComponent();
        Routing.RegisterRoute(nameof(DetailPage), typeof(DetailPage));
    }

    // TODO:
    //  - Convert to object with created date
    //  - Swipe left to highlight as important (bool, BG color change)
    //  - Swipe right to delete without prompt
}