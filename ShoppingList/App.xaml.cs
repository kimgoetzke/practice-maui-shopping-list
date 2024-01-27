using ShoppingList.Utilities;

namespace ShoppingList;

public partial class App
{
    public App()
    {
        InitializeComponent();
        MainPage = new AppShell();

        var systemTheme = Application.Current?.RequestedTheme;
        Settings.SetCurrentThemeFromSystem(systemTheme);
        Logger.Log($"Current app theme is: {Settings.CurrentTheme}");
    }
}
