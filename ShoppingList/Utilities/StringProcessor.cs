using System.Text.RegularExpressions;
using ShoppingList.Services;

namespace ShoppingList.Utilities;

public static partial class StringProcessor
{
    public static string ProcessItemTitle(string itemTitle)
    {
        var trimmed = itemTitle.Trim();
        return itemTitle.Length > 1 ? trimmed[..1].ToUpper() + trimmed[1..] : trimmed.ToUpper();
    }

    public static (string, int) ExtractItemTitleAndQuantity(string input)
    {
        var match = TitleAndQuantityRegex().Match(input);
        if (!match.Success)
            return (input, 1);
        var itemName = match.Groups[1].Value.Trim();
        return int.TryParse(match.Groups[2].Value, out var quantity)
            ? (itemName, quantity)
            : (itemName, 1);
    }

    public static bool IsStoreName(string input)
    {
        var match = StoreRegex().Match(input);
        return match.Success;
    }

    public static string ExtractStoreName(string input)
    {
        var match = StoreRegex().Match(input);
        return match.Success ? match.Groups[1].Value.Trim() : IStoreService.DefaultStoreName;
    }

    [GeneratedRegex(@"(.*)\((\d+)\)")]
    private static partial Regex TitleAndQuantityRegex();

    [GeneratedRegex(@"\[(.*)\]:")]
    private static partial Regex StoreRegex();
}
