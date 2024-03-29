﻿using CommunityToolkit.Maui;
using Microsoft.Extensions.Logging;
using ShoppingList.Services;
using ShoppingList.ViewModel;
using ShoppingList.Views;

namespace ShoppingList;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        return MauiApp
            .CreateBuilder()
            .UseMauiApp<App>()
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("Jost-Regular.ttf", "JostRegular");
                fonts.AddFont("Jost-SemiBold.ttf", "JostSemiBold");
            })
            .UseMauiCommunityToolkit()
            .RegisterServices()
            .RegisterViewModels()
            .RegisterViews()
            .Build();
    }

    private static MauiAppBuilder RegisterServices(this MauiAppBuilder builder)
    {
        builder.Services.AddSingleton<IDatabaseProvider, DatabaseProvider>();
        builder.Services.AddSingleton<IStoreService, StoreService>();
        builder.Services.AddSingleton<IItemService, ItemService>();
        builder.Services.AddSingleton<IClipboardService, ClipboardService>();
        builder.Services.AddLogging();

#if DEBUG
        builder.Logging.AddDebug();
#endif

        return builder;
    }

    private static MauiAppBuilder RegisterViewModels(this MauiAppBuilder builder)
    {
        builder.Services.AddSingleton<MainViewModel>();
        builder.Services.AddSingleton<StoresViewModel>();
        builder.Services.AddTransient<DetailViewModel>();
        return builder;
    }

    private static MauiAppBuilder RegisterViews(this MauiAppBuilder builder)
    {
        builder.Services.AddSingleton<MainPage>();
        builder.Services.AddSingleton<StoresPage>();
        builder.Services.AddTransient<DetailPage>();
        return builder;
    }
}
