using ShoppingList.Models;
using SQLite;

namespace ShoppingList.Data;

public class DbProvider
{
    public const string DefaultStoreName = "Anywhere";
    public SQLiteAsyncConnection Database { get; private set; }

    public async Task Init()
    {
        // ReSharper disable once ConditionIsAlwaysTrueOrFalseAccordingToNullableAPIContract
        if (Database is not null)
            return;

        Database = new SQLiteAsyncConnection(Constants.DatabasePath, Constants.Flags);
        LogHandler.Log($"Database created at: {Constants.DatabasePath}");
        var itemTable = Database.CreateTableAsync<Item>();
        var storeTable = InitialiseConfigurableStores();
        await Task.WhenAll(itemTable, storeTable);
    }

    private async Task InitialiseConfigurableStores()
    {
        await Database.CreateTableAsync<ConfigurableStore>();
        if (await Database.Table<ConfigurableStore>().CountAsync() != 0)
            return;
        await Database.InsertAsync(new ConfigurableStore { Name = DefaultStoreName });
        var stores = await Database.Table<ConfigurableStore>().ToListAsync();
        LogHandler.Log($"Added default stores:");
        stores.ForEach(store => LogHandler.Log($" - {store.Name}"));
    }
}