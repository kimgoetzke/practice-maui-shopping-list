using SQLite;

namespace ShoppingList.Services;

public interface IDatabaseProvider
{
    Task<SQLiteAsyncConnection> GetConnection();
}
