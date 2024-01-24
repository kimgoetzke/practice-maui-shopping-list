using ShoppingList.Models;

namespace ShoppingList.Services;

public class ItemService(IDatabaseProvider db)
{
    public async Task<List<Item>> GetItemsAsync()
    {
        var connection = await db.GetConnection();
        var items = await connection.Table<Item>().ToListAsync();
        return items;
    }

    public async Task SaveItemAsync(Item item)
    {
        var connection = await db.GetConnection();
        Logger.Log($"Adding or updating item: {item.ToLoggableString()}");
        if (item.Id != 0)
        {
            await connection.UpdateAsync(item);
            return;
        }

        await connection.InsertAsync(item);
    }

    public async Task DeleteItemAsync(Item item)
    {
        var connection = await db.GetConnection();
        Logger.Log($"Removing item: {item.Title} #{item.Id}");
        await connection.DeleteAsync(item);
    }

    public async Task DeleteAllItemsAsync()
    {
        Logger.Log("Removing all items");
        var connection = await db.GetConnection();
        var items = await connection.Table<Item>().ToListAsync();
        foreach (var item in items)
            await connection.DeleteAsync(item);
    }
}