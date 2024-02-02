using System.Collections.ObjectModel;
using ShoppingList.Resources.Styles;

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

    public static ObservableCollection<ShoppingList.Models.Theme> GetAllThemesAsCollection()
    {
        var themes = new ObservableCollection<ShoppingList.Models.Theme>
        {
            new() { Name = Theme.Light },
            new() { Name = Theme.Dark }
        };
        return themes;
    }

    public static void LoadTheme(Theme theme)
    {
        var mergedDictionaries = Application.Current?.Resources.MergedDictionaries;
        if (mergedDictionaries == null)
            return;
        mergedDictionaries.Clear();
        switch (theme)
        {
            case Theme.Dark:
                mergedDictionaries.Add(new DarkTheme());
                break;
            case Theme.Light:
            default:
                mergedDictionaries.Add(new LightTheme());
                break;
        }
        mergedDictionaries.Add(new Styles());

        CurrentTheme = theme;
        Logger.Log($"Current app theme is: {CurrentTheme}");
    }
}
