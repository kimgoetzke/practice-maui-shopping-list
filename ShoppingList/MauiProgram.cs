using Microsoft.Extensions.Logging;
using ShoppingList.Data;
using ShoppingList.ViewModel;
using CommunityToolkit.Maui;

namespace ShoppingList;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder.UseMauiApp<App>().ConfigureFonts(fonts =>
        {
            fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
            fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
        }).UseMauiCommunityToolkit();
        builder.Services.AddSingleton<IConnectivity>(Connectivity.Current);
        builder.Services.AddSingleton<MainPage>();
        builder.Services.AddSingleton<MainViewModel>();
        builder.Services.AddTransient<DetailPage>();
        builder.Services.AddTransient<DetailViewModel>();
        builder.Services.AddSingleton<ItemDatabase>();
        builder.Services.AddLogging();
#if DEBUG
        builder.Logging.AddDebug();
#endif
        return builder.Build();
    }
}