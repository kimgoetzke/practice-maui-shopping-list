using System.Text.RegularExpressions;

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
        var match = MyRegex().Match(input);
        if (!match.Success) return (input, 1);
        var itemName = match.Groups[1].Value.Trim();
        return int.TryParse(match.Groups[2].Value, out var quantity) ? (itemName, quantity) : (itemName, 1);
    }

    [GeneratedRegex(@"(.*)\((\d+)\)")]
    private static partial Regex MyRegex();
}