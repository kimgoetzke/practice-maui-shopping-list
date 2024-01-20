using CommunityToolkit.Maui.Views;

namespace ShoppingList.Views;

public partial class SimplePopup : Popup
{
    public SimplePopup()
    {
        InitializeComponent();
    }
    
    private void OnButtonClicked(object sender, EventArgs e)
    {
        Close();
        Shell.Current.GoToAsync(nameof(StoresPage), true);
    }
}