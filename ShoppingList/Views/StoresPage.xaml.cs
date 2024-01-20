using ShoppingList.ViewModel;

namespace ShoppingList.Views;

public partial class StoresPage
{
    public StoresPage(StoresViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }

    protected override async void OnNavigatedTo(NavigatedToEventArgs args)
    {
        base.OnNavigatedTo(args);
        await ((StoresViewModel)BindingContext).LoadStoresFromDatabase();
    }

    private void OnEntryUnfocused(object sender, FocusEventArgs e)
    {
        AddStoreButton.Focus();
    }
}