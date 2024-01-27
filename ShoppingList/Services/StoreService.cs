using ShoppingList.Models;
using ShoppingList.Utilities;
using SQLite;

namespace ShoppingList.Services;

public class StoreService(IDatabaseProvider db) : IStoreService
{
    private ConfigurableStore? _defaultStore;

    public async Task<ConfigurableStore> GetDefaultStore()
    {
        if (_defaultStore == null)
        {
            var connection = await db.GetConnection();
            var loadedStore = await connection
                .Table<ConfigurableStore>()
                .FirstAsync(s => s.Name == IStoreService.DefaultStoreName);
            _defaultStore = loadedStore;
        }

        if (_defaultStore == null)
            throw new NullReferenceException("There is no default store in the database");

        return _defaultStore;
    }

    public async Task<List<ConfigurableStore>> GetAllAsync()
    {
        var connection = await db.GetConnection();
        return await connection.Table<ConfigurableStore>().ToListAsync();
    }

    public async Task CreateOrUpdateAsync(ConfigurableStore store)
    {
        var connection = await db.GetConnection();
        Logger.Log($"Adding or updating store: {store.ToLoggableString()}");
        if (store.Id != 0)
        {
            await connection.UpdateAsync(store);
            return;
        }

        await connection.InsertAsync(store);
    }

    public async Task CreateIfNotExistAsync(string name)
    {
        var connection = await db.GetConnection();
        Logger.Log($"Attempting to find store '{name}'");
        var item = await connection
            .Table<ConfigurableStore>()
            .Where(store => store.Name == name)
            .FirstOrDefaultAsync();
        if (item != null)
        {
            Logger.Log($"Found store {name} - no need to add it");
            return;
        }

        await connection.InsertAsync(new ConfigurableStore { Name = name });
        Logger.Log($"Added store {name}");
    }

    public async Task DeleteAsync(ConfigurableStore store)
    {
        var connection = await db.GetConnection();
        await SetStoresToDefaultOnMatchingItems(store.Name);
        await connection.DeleteAsync(store);
        Logger.Log($"Removing store: {store.ToLoggableString()}");
    }

    private async Task SetStoresToDefaultOnMatchingItems(string storeName)
    {
        var connection = await db.GetConnection();
        var items = await connection.Table<Item>().ToListAsync();
        foreach (var item in items.Where(item => item.StoreName == storeName))
        {
            item.StoreName = IStoreService.DefaultStoreName;
            await connection.UpdateAsync(item);
        }
    }

    public async Task DeleteAllAsync()
    {
        var connection = await db.GetConnection();
        await SetStoresToDefaultOnAllItems(connection);
        await RemoveAllExceptDefaultStore(connection);
        Logger.Log("Removing all stores");
    }

    private static async Task SetStoresToDefaultOnAllItems(SQLiteAsyncConnection connection)
    {
        var items = await connection.Table<Item>().ToListAsync();
        foreach (var item in items.Where(item => item.StoreName != IStoreService.DefaultStoreName))
        {
            item.StoreName = IStoreService.DefaultStoreName;
            await connection.UpdateAsync(item);
        }
    }

    private static async Task RemoveAllExceptDefaultStore(SQLiteAsyncConnection connection)
    {
        var stores = await connection.Table<ConfigurableStore>().ToListAsync();
        foreach (var store in stores.Where(store => store.Name != IStoreService.DefaultStoreName))
        {
            await connection.DeleteAsync(store);
        }
    }
}
