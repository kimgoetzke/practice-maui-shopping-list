using CommunityToolkit.Maui.Views;

namespace ShoppingList.Views;

public partial class WelcomePopup : Popup
{
    public WelcomePopup()
    {
        InitializeComponent();
    }

    private void OnButtonClicked(object sender, EventArgs e)
    {
        Close();
        Shell.Current.GoToAsync(nameof(StoresPage), true);
    }
}
