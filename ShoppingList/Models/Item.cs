using SQLite;

namespace ShoppingList.Models;

public class Item
{
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public int Quantity { get; set; } = 1;
    public bool IsImportant { get; set; }
    public DateTime AddedOn { get; set; } = DateTime.Now;
    public string StoreName { get; set; } = "<Not set>";

    public override string ToString()
    {
        return Title;
    }

    public string ToLoggableString()
    {
        return $"{Title} #{Id} (store: {StoreName}, quantity: {Quantity}, important: {IsImportant})";
    }
}
