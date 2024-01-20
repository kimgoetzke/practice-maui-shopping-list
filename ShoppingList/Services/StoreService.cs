using ShoppingList.Models;
using SQLite;

namespace ShoppingList.Services;

public class StoreService(DbProvider dbProvider)
{
    public const string DefaultStoreName = "Anywhere";
    private ConfigurableStore? _defaultStore;

    public async Task<ConfigurableStore> DefaultStore()
    {
        if (_defaultStore == null)
        {
            var connection = await dbProvider.GetConnection();
            var loadedStore = await connection.Table<ConfigurableStore>()
                .FirstAsync(s => s.Name == DefaultStoreName);
            _defaultStore = loadedStore;
        }

        if (_defaultStore == null)
            throw new NullReferenceException("There is no default store in the database");

        return _defaultStore;
    }

    public async Task<List<ConfigurableStore>> GetStoresAsync()
    {
        var connection = await dbProvider.GetConnection();
        return await connection.Table<ConfigurableStore>().ToListAsync();
    }

    public async Task SaveStoreAsync(ConfigurableStore store)
    {
        var connection = await dbProvider.GetConnection();
        Logger.Log($"Adding or updating store: {store.ToLoggableString()}");
        if (store.Id != 0)
        {
            await connection.UpdateAsync(store);
            return;
        }

        await connection.InsertAsync(store);
    }

    public async Task DeleteStoreAsync(ConfigurableStore store)
    {
        var connection = await dbProvider.GetConnection();
        await SetStoresToDefaultOnMatchingItems(store.Name);
        await connection.DeleteAsync(store);
        Logger.Log($"Removing store: {store.ToLoggableString()}");
    }

    private async Task SetStoresToDefaultOnMatchingItems(string storeName)
    {
        var defaultStore = await DefaultStore();
        var connection = await dbProvider.GetConnection();
        var items = await connection.Table<Item>().ToListAsync();
        foreach (var item in items.Where(item => item.ConfigurableStore?.Name == storeName))
        {
            item.From = Store.Anywhere;
            item.ConfigurableStore = defaultStore;
            await connection.UpdateAsync(item);
        }
    }

    public async Task ResetStoresAsync()
    {
        var connection = await dbProvider.GetConnection();
        await SetStoresToDefaultOnAllItems(connection);
        await RemoveAllExceptDefaultStore(connection);
        Logger.Log("Removing all stores");
    }

    private async Task SetStoresToDefaultOnAllItems(SQLiteAsyncConnection connection)
    {
        var defaultStore = await DefaultStore();
        var items = await connection.Table<Item>().ToListAsync();
        foreach (var item in items.Where(item => item.ConfigurableStore?.Name != DefaultStoreName))
        {
            item.From = Store.Anywhere;
            item.ConfigurableStore = defaultStore;
            await connection.UpdateAsync(item);
        }
    }

    private static async Task RemoveAllExceptDefaultStore(SQLiteAsyncConnection connection)
    {
        var stores = await connection.Table<ConfigurableStore>().ToListAsync();
        foreach (var store in stores.Where(store => store.Name != DefaultStoreName))
        {
            await connection.DeleteAsync(store);
        }
    }
}