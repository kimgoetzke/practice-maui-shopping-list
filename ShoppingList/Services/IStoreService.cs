using ShoppingList.Models;

namespace ShoppingList.Services;

public interface IStoreService
{
    const string DefaultStoreName = "Anywhere";
    Task<ConfigurableStore> GetDefaultStore();
    Task<List<ConfigurableStore>> GetStoresAsync();
    Task SaveStoreAsync(ConfigurableStore store);
    Task DeleteStoreAsync(ConfigurableStore store);
    Task ResetStoresAsync();
}