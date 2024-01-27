namespace ShoppingList.Utilities;

public static class Settings
{
    public static bool FirstRun
    {
        get => Preferences.Get(nameof(FirstRun), true);
        set => Preferences.Set(nameof(FirstRun), value);
    }

    public static Theme CurrentTheme
    {
        get => (Theme)Preferences.Get(nameof(CurrentTheme), (int)Theme.Light);
        set => Preferences.Set(nameof(CurrentTheme), (int)value);
    }

    public static void SetCurrentThemeFromSystem(AppTheme? systemTheme)
    {
        CurrentTheme = systemTheme switch
        {
            AppTheme.Light => CurrentTheme = Theme.Light,
            AppTheme.Dark => CurrentTheme = Theme.Dark,
            _ => CurrentTheme = Theme.Light
        };
    }

    public enum Theme
    {
        Light,
        Dark
    }
}
