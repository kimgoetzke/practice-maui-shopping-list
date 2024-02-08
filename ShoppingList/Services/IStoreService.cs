using ShoppingList.Models;

namespace ShoppingList.Services;

public interface IStoreService
{
    const string DefaultStoreName = "Anywhere";
    Task<ConfigurableStore> GetDefaultStore();
    Task<IEnumerable<ConfigurableStore>> GetAllAsync();
    Task CreateOrUpdateAsync(ConfigurableStore store);
    Task DeleteAsync(ConfigurableStore store);
    Task DeleteAllAsync();
}
