using Android.App;
using Android.Content.PM;
using Android.OS;

namespace ShoppingList;

[Activity(
    Theme = "@style/ShoppingListAndroidSplashScreen",
    MainLauncher = true,
    ConfigurationChanges = ConfigChanges.ScreenSize
        | ConfigChanges.Orientation
        | ConfigChanges.UiMode
        | ConfigChanges.ScreenLayout
        | ConfigChanges.SmallestScreenSize
        | ConfigChanges.Density
)]
public class MainActivity : MauiAppCompatActivity
{
    protected override void OnCreate(Bundle? savedInstanceState)
    {
        base.OnCreate(savedInstanceState);
        RequestedOrientation = ScreenOrientation.Portrait;
    }
}
