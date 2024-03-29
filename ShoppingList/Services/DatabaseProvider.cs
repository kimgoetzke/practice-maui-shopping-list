using ShoppingList.Models;
using ShoppingList.Utilities;
using SQLite;

namespace ShoppingList.Services;

public class DatabaseProvider : IDatabaseProvider
{
    private const SQLiteOpenFlags Flags =
        SQLiteOpenFlags.ReadWrite | SQLiteOpenFlags.Create | SQLiteOpenFlags.SharedCache;

    private const string DatabaseFilename = "ShoppingList.db3";

    private static string DatabasePath =>
        Path.Combine(FileSystem.AppDataDirectory, DatabaseFilename);

    private SQLiteAsyncConnection? _connection;

    public async Task<SQLiteAsyncConnection> GetConnection()
    {
        await InitialiseDatabase();
        return _connection!;
    }

    private async ValueTask InitialiseDatabase()
    {
        if (_connection is not null)
            return;

        Logger.Log($"Initialising database");
        _connection = new SQLiteAsyncConnection(DatabasePath, Flags);
        Logger.Log($"Connected to: {DatabasePath}");
        var itemTable = InitialiseItemTable(_connection);
        var storeTable = InitialiseStoreTable(_connection);
        await Task.WhenAll(itemTable, storeTable).ConfigureAwait(false);
    }

    private static Task<CreateTableResult> InitialiseItemTable(SQLiteAsyncConnection connection)
    {
        return connection.CreateTableAsync<Item>();
    }

    private static async Task InitialiseStoreTable(SQLiteAsyncConnection connection)
    {
        await connection.CreateTableAsync<ConfigurableStore>();
        if (await connection.Table<ConfigurableStore>().CountAsync() != 0)
            return;

        await connection
            .InsertAsync(new ConfigurableStore { Name = IStoreService.DefaultStoreName })
            .ConfigureAwait(false);
        Logger.Log($"Added default store(s): {IStoreService.DefaultStoreName}");
    }
}
