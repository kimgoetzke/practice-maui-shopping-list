using ShoppingList.Models;
using SQLite;

namespace ShoppingList.Services;

public class DatabaseProvider : IDatabaseProvider
{
    private SQLiteAsyncConnection? _connection;

    public async Task<SQLiteAsyncConnection> GetConnection()
    {
        await InitialiseDatabase();
        return _connection!;
    }

    private async Task InitialiseDatabase()
    {
        if (_connection is not null)
            return;

        _connection = new SQLiteAsyncConnection(Constants.DatabasePath, Constants.Flags);
        Logger.Log($"Connected to: {Constants.DatabasePath}");
        var itemTable = InitialiseItemTable(_connection);
        var storeTable = InitialiseStoreTable(_connection);
        await Task.WhenAll(itemTable, storeTable);
    }

    private static async Task InitialiseItemTable(SQLiteAsyncConnection connection)
    {
        await connection.CreateTableAsync<Item>();
    }

    private static async Task InitialiseStoreTable(SQLiteAsyncConnection connection)
    {
        await connection.CreateTableAsync<ConfigurableStore>();
        if (await connection.Table<ConfigurableStore>().CountAsync() != 0)
            return;

        await connection.InsertAsync(new ConfigurableStore { Name = StoreService.DefaultStoreName });
        var stores = await connection.Table<ConfigurableStore>().ToListAsync();
        Logger.Log($"Added default stores:");
        stores.ForEach(store => Logger.Log($" - {store.Name}"));
    }
}