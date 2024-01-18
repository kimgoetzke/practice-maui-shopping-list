using ShoppingList.Models;
using SQLite;

namespace ShoppingList.Data;

public class ItemDatabase
{
    private SQLiteAsyncConnection _database;

    private async Task Init()
    {
        // ReSharper disable once ConditionIsAlwaysTrueOrFalseAccordingToNullableAPIContract
        if (_database is not null)
            return;

        _database = new SQLiteAsyncConnection(Constants.DatabasePath, Constants.Flags);
        LogHandler.Log($"Database created at: {Constants.DatabasePath}");
        var itemTable = _database.CreateTableAsync<Item>();
        var storeTable = InitialiseConfigurableStores();
        await Task.WhenAll(itemTable, storeTable);
    }

    private async Task InitialiseConfigurableStores()
    {
        await _database.CreateTableAsync<ConfigurableStore>();
        await _database.InsertAsync(new ConfigurableStore { Name = "Anywhere" });
        var stores = await _database.Table<ConfigurableStore>().ToListAsync();
        LogHandler.Log($"Added default stores:");
        stores.ForEach(store => LogHandler.Log($" - {store.Name}"));
    }

    // Items
    public async Task<List<Item>> GetItemsAsync()
    {
        await Init();
        return await _database.Table<Item>().ToListAsync();
    }

    public async Task SaveItemAsync(Item item)
    {
        await Init();
        LogHandler.Log($"Adding or updating item #{item.Id}: {item.Title}");
        if (item.Id != 0)
        {
            await _database.UpdateAsync(item);
            return;
        }

        await _database.InsertAsync(item);
    }

    public async Task DeleteItemAsync(Item item)
    {
        await Init();
        LogHandler.Log($"Removing item #{item.Id}: {item.Title}");
        await _database.DeleteAsync(item);
    }

    public async Task DeleteAllItemsAsync()
    {
        await Init();
        LogHandler.Log($"Removing all item");
        var items = await _database.Table<Item>().ToListAsync();
        foreach (var item in items)
            await _database.DeleteAsync(item);
    }

    // Stores
    public async Task<List<ConfigurableStore>> GetStoresAsync()
    {
        await Init();
        return await _database.Table<ConfigurableStore>().ToListAsync();
    }

    public async Task SaveStoreAsync(ConfigurableStore store)
    {
        await Init();
        LogHandler.Log($"Adding or updating store #{store.Id}: {store.Name}");
        if (store.Id != 0)
        {
            await _database.UpdateAsync(store);
            return;
        }

        await _database.InsertAsync(store);
    }

    public async Task DeleteStoreAsync(ConfigurableStore store)
    {
        await Init();
        // var items = await _database.Table<Item>().ToListAsync();
        // var defaultStore = await _database.Table<ConfigurableStore>().FirstAsync(s => s.Name == "Anywhere");

        // foreach (var item in items.Where(item => item.ConfigurableStore.Id == store.Id))
        // {
        //     item.From = Store.Anywhere;
        //     item.ConfigurableStore = defaultStore;
        //     await _database.UpdateAsync(item);
        // }

        LogHandler.Log($"Removing item #{store.Id}: {store.Name}");
        await _database.DeleteAsync(store);
    }

    public async Task DeleteAllStoresAsync()
    {
        await Init();
        // var items = await _database.Table<Item>().ToListAsync();
        // var defaultStore = await _database.Table<ConfigurableStore>().FirstAsync(s => s.Name == "Anywhere");

        // foreach (var item in items.Where(item => item.ConfigurableStore.Id != 0))
        // {
        //     item.From = Store.Anywhere;
        //     item.ConfigurableStore = defaultStore;
        //     await _database.UpdateAsync(item);
        // }

        var stores = await _database.Table<Item>().ToListAsync();
        foreach (var store in stores.Where(store => store.Id != 0))
        {
            await _database.DeleteAsync(store);
        }

        LogHandler.Log($"Removing all stores");
    }
}