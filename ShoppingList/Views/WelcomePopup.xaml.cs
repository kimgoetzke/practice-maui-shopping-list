namespace ShoppingList.Views;

public partial class WelcomePopup
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
