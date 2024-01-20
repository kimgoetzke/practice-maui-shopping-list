namespace ShoppingList.Services;

public static class Logger
{
    public static void Log(string message)
    {
#if __ANDROID__
        Android.Util.Log.Info(Constants.LoggerTag, $"[XXX] {message}");
#endif
#if DEBUG
        Console.WriteLine($"[XXX] {message}");
#endif
    }
}