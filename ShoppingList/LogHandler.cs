namespace ShoppingList;

public static class LogHandler
{
    public static void Log(string message)
    {
#if __ANDROID__
        Android.Util.Log.Info(Constants.LoggerTag, $"xxx {message}");
#endif
#if DEBUG
        Console.WriteLine($"xxx {message}");
#endif
    }
}