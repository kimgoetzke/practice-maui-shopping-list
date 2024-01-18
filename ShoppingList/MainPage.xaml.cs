using ShoppingList.Models;
using ShoppingList.ViewModel;
using CommunityToolkit.Maui.Alerts;

namespace ShoppingList;

public partial class MainPage : ContentPage
{
    public MainPage(MainViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }

    protected override async void OnNavigatedTo(NavigatedToEventArgs args)
    {
        base.OnNavigatedTo(args);
        var vm = (MainViewModel)BindingContext;
        await vm.LoadItemsFromDatabase();
        vm.SortItems();
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        ((MainViewModel)BindingContext).NewItem = new Item { From = Store.Anywhere };
    }

    private void OnEntryUnfocused(object sender, FocusEventArgs e)
    {
        AddButton.Focus();
    }

    private void CopyOnClicked(object? sender, EventArgs e)
    {
        var vm = (MainViewModel)BindingContext;
        vm.CopyToClipboard();

        var cancellationTokenSource = new CancellationTokenSource();
        var toast = Toast.Make("Copied list to clipboard");
        toast.Show(cancellationTokenSource.Token);
    }
}