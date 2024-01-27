using System.Collections.ObjectModel;
using ShoppingList.Models;
using ShoppingList.Utilities;

namespace ShoppingList.Services;

public class ClipboardService(IStoreService storeService, IItemService itemService)
    : IClipboardService
{
    public async void InsertFromClipboardAsync(
        ObservableCollection<ConfigurableStore> observableStores,
        ObservableCollection<Item> observableItems
    )
    {
        var import = await Clipboard.GetTextAsync();
        if (IsClipboardEmpty(import))
            return;
        var stores = await storeService.GetAllAsync();
        if (
            !WasAbleToConvertToItemList(
                import!,
                stores,
                out var itemCount,
                out var storeCount,
                out var itemList,
                out var storeList
            )
        )
            return;
        if (!await IsImportConfirmedByUser(itemCount, storeCount))
            return;
        await CreateMissingStores(observableStores, storeList);
        await ImportItemList(observableItems, itemList);
        Notifier.ShowToast($"Imported {itemCount} items from clipboard");
    }

    private static bool IsClipboardEmpty(string? import)
    {
        if (import != null)
            return false;
        Notifier.ShowToast("Nothing to import - your clipboard is empty");
        return true;
    }

    private static bool WasAbleToConvertToItemList(
        string import,
        List<ConfigurableStore> stores,
        out int itemCount,
        out int storeCount,
        out List<Item> itemList,
        out List<ConfigurableStore> storeList
    )
    {
        Logger.Log("Extracted from clipboard: " + import.Replace(Environment.NewLine, ","));
        var storeName = IStoreService.DefaultStoreName;
        itemCount = 0;
        storeCount = 0;
        itemList = [];
        storeList = [];
        foreach (var substring in import.Replace(Environment.NewLine, ",").Split(","))
        {
            if (string.IsNullOrWhiteSpace(substring))
                continue;

            if (StringProcessor.IsStoreName(substring))
            {
                storeName = AddStore(stores, ref storeCount, storeList, substring);
                continue;
            }

            AddItem(ref itemCount, itemList, substring, storeName);
        }

        if (itemCount != 0)
            return true;
        Notifier.ShowToast("Nothing to import - your clipboard may contain invalid data");
        return false;
    }

    private static void AddItem(
        ref int itemCount,
        List<Item> itemList,
        string substring,
        string storeName
    )
    {
        var (title, quantity) = StringProcessor.ExtractItemTitleAndQuantity(substring);
        var processedTitle = StringProcessor.ProcessItemTitle(title);
        var item = new Item
        {
            Title = processedTitle,
            StoreName = storeName,
            Quantity = quantity
        };
        itemList.Add(item);
        itemCount++;
    }

    private static string AddStore(
        List<ConfigurableStore> stores,
        ref int storeCount,
        List<ConfigurableStore> storeList,
        string s
    )
    {
        var extractedName = StringProcessor.ExtractStoreName(s);
        var matchingStore = stores.Find(store => store.Name == extractedName);
        if (matchingStore != null)
            return extractedName;
        storeList.Add(new ConfigurableStore { Name = extractedName });
        storeCount++;
        return extractedName;
    }

    private static async Task<bool> IsImportConfirmedByUser(int itemCount, int storeCount)
    {
        var message = CreateToastMessage(itemCount, storeCount);
        var isConfirmed = await Shell.Current.DisplayAlert(
            "Import from clipboard",
            message,
            "Yes",
            "No"
        );

        if (!isConfirmed)
            Notifier.ShowToast("Import cancelled");
        return isConfirmed;
    }

    private static string CreateToastMessage(int itemCount, int storeCount)
    {
        return storeCount > 0
            ? $"Extracted {itemCount} item(s) and {storeCount} store(s) from your clipboard. Would you like to create the missing store(s) and add the item(s) to your list?"
            : $"Extracted {itemCount} item(s) from your clipboard. Would you like to add the item(s) to your list?";
    }

    private async Task CreateMissingStores(
        ObservableCollection<ConfigurableStore> stores,
        List<ConfigurableStore> toCreate
    )
    {
        foreach (var store in toCreate)
        {
            await storeService.CreateOrUpdateAsync(store);
            stores.Add(store);
        }
    }

    private async Task ImportItemList(ObservableCollection<Item> items, List<Item> toImport)
    {
        foreach (var item in toImport)
        {
            await itemService.CreateOrUpdateAsync(item);
            items.Add(item);
        }
    }
}
