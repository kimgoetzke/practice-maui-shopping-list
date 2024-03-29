using ShoppingList.Models;
using ShoppingList.Utilities;

namespace ShoppingList.Services;

public class ItemService(IDatabaseProvider db) : IItemService
{
    public async Task<List<Item>> GetAsync()
    {
        var connection = await db.GetConnection();
        return await connection.Table<Item>().ToListAsync();
    }

    public async Task CreateOrUpdateAsync(Item item)
    {
        var connection = await db.GetConnection();
        if (item.Id != 0)
        {
            await connection.UpdateAsync(item);
            return;
        }

        await connection.InsertAsync(item);
        Logger.Log($"Added or updated item: {item.ToLoggableString()}");
    }

    public async Task DeleteAsync(Item item)
    {
        Logger.Log($"Removing item: {item.Title} #{item.Id}");
        var connection = await db.GetConnection();
        await connection.DeleteAsync(item);
    }

    public async Task DeleteAllAsync()
    {
        var connection = await db.GetConnection();
        var items = await connection.Table<Item>().ToListAsync();
        foreach (var item in items)
            await connection.DeleteAsync(item);
        Logger.Log("Removed all items");
    }

    public async Task UpdateAllToDefaultStoreAsync()
    {
        var connection = await db.GetConnection();
        var items = await connection.Table<Item>().ToListAsync();
        foreach (var item in items.Where(item => item.StoreName != IStoreService.DefaultStoreName))
        {
            Logger.Log($"Updating item to use default store: {item.ToLoggableString()}");
            item.StoreName = IStoreService.DefaultStoreName;
            await connection.UpdateAsync(item);
        }

        Logger.Log($"Updated all items to use default store");
    }

    public async Task UpdateAllUsingStoreAsync(string storeName)
    {
        var connection = await db.GetConnection();
        var items = await connection.Table<Item>().ToListAsync();
        foreach (var item in items.Where(item => item.StoreName == storeName))
        {
            item.StoreName = IStoreService.DefaultStoreName;
            await connection.UpdateAsync(item);
        }
    }
}
