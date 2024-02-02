using CommunityToolkit.Mvvm.ComponentModel;
using ShoppingList.Utilities;

namespace ShoppingList.Models;

public partial class Theme : ObservableObject
{
    [ObservableProperty]
    private Settings.Theme _name;

    public override string ToString()
    {
        return Name.ToString();
    }
}
