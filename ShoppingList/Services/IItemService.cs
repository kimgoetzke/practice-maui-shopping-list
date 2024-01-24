using ShoppingList.Models;

namespace ShoppingList.Services;

public interface IItemService
{
    Task<List<Item>> GetItemsAsync();
    Task SaveItemAsync(Item item);
    Task DeleteItemAsync(Item item);
    Task DeleteAllItemsAsync();
}