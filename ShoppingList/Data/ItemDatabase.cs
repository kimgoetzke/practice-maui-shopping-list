using ShoppingList.Models;
using SQLite;

namespace ShoppingList.Data;

public class ItemDatabase
{
    SQLiteAsyncConnection Database;

    private async Task Init()
    {
        // ReSharper disable once ConditionIsAlwaysTrueOrFalseAccordingToNullableAPIContract
        if (Database is not null)
            return;

        Database = new SQLiteAsyncConnection(Constants.DatabasePath, Constants.Flags);
        await Database.CreateTableAsync<Item>();
        LogHandler.Log($"Database created at: {Constants.DatabasePath}");
    }

    public async Task<List<Item>> GetItemsAsync()
    {
        await Init();
        return await Database.Table<Item>().ToListAsync();
    }

    public async Task SaveItemAsync(Item item)
    {
        await Init();
        LogHandler.Log($"Adding or updating item #{item.Id}: {item.Title}");
        if (item.Id != 0)
        {
            await Database.UpdateAsync(item);
            return;
        }

        await Database.InsertAsync(item);
    }

    public async Task DeleteItemAsync(Item item)
    {
        await Init();
        LogHandler.Log($"Removing item #{item.Id}: {item.Title}");
        await Database.DeleteAsync(item);
    }
}