using ShoppingList.Utilities;

namespace ShoppingList;

public partial class App
{
    public App()
    {
        InitializeComponent();
        SetThemeToSystemThemeOnFirstRun();
        var currentTheme = Settings.CurrentTheme;
        Settings.LoadTheme(currentTheme);
        MainPage = new AppShell();
    }

    private static void SetThemeToSystemThemeOnFirstRun()
    {
        if (!Settings.FirstRun)
            return;
        Logger.Log("Setting current theme to system theme on first run");
        var systemTheme = Current?.RequestedTheme;
        Settings.SetCurrentThemeFromSystem(systemTheme);
    }
}
