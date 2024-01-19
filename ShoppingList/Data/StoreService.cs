using ShoppingList.Models;

namespace ShoppingList.Data;

public class StoreService(DbProvider dbProvider)
{
    public async Task<List<ConfigurableStore>> GetStoresAsync()
    {
        await dbProvider.Init();
        return await dbProvider.Database.Table<ConfigurableStore>().ToListAsync();
    }

    public async Task SaveStoreAsync(ConfigurableStore store)
    {
        await dbProvider.Init();
        LogHandler.Log($"Adding or updating store #{store.Id}: {store.Name}");
        if (store.Id != 0)
        {
            await dbProvider.Database.UpdateAsync(store);
            return;
        }

        await dbProvider.Database.InsertAsync(store);
    }

    public async Task DeleteStoreAsync(ConfigurableStore store)
    {
        await dbProvider.Init();
        await SetStoresToDefaultOnMatchingItems(store.Name);
        await dbProvider.Database.DeleteAsync(store);
        LogHandler.Log($"Removing item #{store.Id}: {store.Name}");
    }

    private async Task SetStoresToDefaultOnMatchingItems(string storeName)
    {
        var defaultStore = await dbProvider.Database.Table<ConfigurableStore>().FirstAsync(s => s.Name == DbProvider.DefaultStoreName);
        var items = await dbProvider.Database.Table<Item>().ToListAsync();
        foreach (var item in items.Where(item => item.ConfigurableStore?.Name == storeName))
        {
            item.From = Store.Anywhere;
            item.ConfigurableStore = defaultStore;
            await dbProvider.Database.UpdateAsync(item);
        }
    }

    public async Task ResetStoresAsync()
    {
        await dbProvider.Init();
        await SetStoresToDefaultOnAllItems();

        var stores = await dbProvider.Database.Table<ConfigurableStore>().ToListAsync();
        foreach (var store in stores.Where(store => store.Name != DbProvider.DefaultStoreName))
        {
            await dbProvider.Database.DeleteAsync(store);
        }

        LogHandler.Log($"Removing all stores");
        return;
    }

    private async Task SetStoresToDefaultOnAllItems()
    {
        var defaultStore = await dbProvider.Database.Table<ConfigurableStore>().FirstAsync(s => s.Name == DbProvider.DefaultStoreName);
        var items = await dbProvider.Database.Table<Item>().ToListAsync();
        foreach (var item in items.Where(item => item.ConfigurableStore?.Name != DbProvider.DefaultStoreName))
        {
            item.From = Store.Anywhere;
            item.ConfigurableStore = defaultStore;
            await dbProvider.Database.UpdateAsync(item);
        }
    }
}