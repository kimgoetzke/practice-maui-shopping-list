namespace ShoppingList.Utilities;

public static class Logger
{
    private const string LoggerTag = "ShoppingList";

    public static void Log(string message)
    {
#if __ANDROID__
        Android.Util.Log.Info(LoggerTag, $"[XXX] {message}");
#elif DEBUG
        Console.WriteLine($"[XXX] {message}");
#endif
    }
}
