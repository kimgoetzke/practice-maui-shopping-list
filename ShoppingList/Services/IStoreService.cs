using ShoppingList.Models;

namespace ShoppingList.Services;

public interface IStoreService
{
    const string DefaultStoreName = "Anywhere";
    Task<ConfigurableStore> GetDefaultStore();
    Task<List<ConfigurableStore>> GetAllAsync();
    Task CreateOrUpdateAsync(ConfigurableStore store);
    Task CreateIfNotExistAsync(string name);
    Task DeleteAsync(ConfigurableStore store);
    Task DeleteAllAsync();
}
