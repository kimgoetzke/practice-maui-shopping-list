using ShoppingList.ViewModel;

namespace ShoppingList.Views;

public partial class StoresPage
{
    public StoresPage(StoresViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }

    private void OnEntryUnfocused(object sender, FocusEventArgs e)
    {
        AddStoreButton.Focus();
    }
}
