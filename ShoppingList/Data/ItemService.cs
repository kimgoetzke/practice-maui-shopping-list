using ShoppingList.Models;

namespace ShoppingList.Data;

public class ItemService(DbProvider dbProvider)
{
    public async Task<List<Item>> GetItemsAsync()
    {
        await dbProvider.Init();
        var items = await dbProvider.Database.Table<Item>().ToListAsync();
        return items;
    }

    public async Task SaveItemAsync(Item item)
    {
        await dbProvider.Init();
        LogHandler.Log($"Adding or updating item #{item.Id}: {item.Title}");
        if (item.Id != 0)
        {
            await dbProvider.Database.UpdateAsync(item);
            return;
        }

        await dbProvider.Database.InsertAsync(item);
    }

    public async Task DeleteItemAsync(Item item)
    {
        await dbProvider.Init();
        LogHandler.Log($"Removing item #{item.Id}: {item.Title}");
        await dbProvider.Database.DeleteAsync(item);
    }

    public async Task DeleteAllItemsAsync()
    {
        await dbProvider.Init();
        LogHandler.Log($"Removing all item");
        var items = await dbProvider.Database.Table<Item>().ToListAsync();
        foreach (var item in items)
            await dbProvider.Database.DeleteAsync(item);
    }
}