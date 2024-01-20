using CommunityToolkit.Mvvm.ComponentModel;
using SQLite;
using SQLiteNetExtensions.Attributes;

namespace ShoppingList.Models;

public partial class Item : ObservableObject
{
    [PrimaryKey, AutoIncrement] public int Id { get; set; }

    [ObservableProperty] private string title = string.Empty;

    [ObservableProperty] private int quantity = 1;

    [ObservableProperty] private bool isImportant;

    [ObservableProperty] private Store from = Store.Anywhere;

    [ObservableProperty] private DateTime addedOn = DateTime.Now;

    public Item()
    {
        // Figure out how to keep parameterless constructor for SQLite
        // while enforcing that a ConfigurableStore is always set
    }

    public Item(ConfigurableStore store)
    {
        ConfigurableStore = store;
    }

    [ForeignKey(typeof(ConfigurableStore))]
    [ManyToOne("Id")]
    public ConfigurableStore? ConfigurableStore { get; set; }

    public override string ToString()
    {
        return Title;
    }
    
    public string ToLoggableString()
    {
        return ConfigurableStore == null
            ? $"Item.ToString() = {Title} #{Id} (from: null, quantity: {Quantity}, important: {IsImportant})"
            : $"Item.ToString() = {Title} #{Id} (from {ConfigurableStore.Name} #{ConfigurableStore.Id}, quantity: {Quantity}, important: {IsImportant})";
    }
}