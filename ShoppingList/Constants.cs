namespace ShoppingList;

public static class Constants
{
    public const string LoggerTag = "ShoppingList";

    private const string DatabaseFilename = "ShoppingList.db3";

    public const SQLite.SQLiteOpenFlags Flags =
        SQLite.SQLiteOpenFlags.ReadWrite |
        SQLite.SQLiteOpenFlags.Create |
        SQLite.SQLiteOpenFlags.SharedCache;

    public static string DatabasePath =>
        Path.Combine(FileSystem.AppDataDirectory, DatabaseFilename);
}