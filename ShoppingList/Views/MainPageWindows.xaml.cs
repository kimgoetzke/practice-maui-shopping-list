using CommunityToolkit.Maui.Views;
using ShoppingList.Utilities;
using ShoppingList.ViewModel;

namespace ShoppingList.Views;

public partial class MainPageWindows
{
    private const uint AnimationDuration = 400u;
    private bool _isMenuOpen;
    private readonly MainViewModel _viewModel;

    public MainPageWindows(MainViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
        _viewModel = viewModel;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        var loadItems = _viewModel.LoadItemsFromDatabase();
        var loadStores = _viewModel.LoadStoresFromService();
        await Task.WhenAll(loadItems, loadStores);
        _viewModel.SortItems();
        DisplayPopUpOnFirstRun();
    }

    private void DisplayPopUpOnFirstRun()
    {
        if (!Settings.FirstRun)
            return;
        Logger.Log("Showing welcome popup on first run");
        Settings.FirstRun = false;
        this.ShowPopup(new WelcomePopup());
    }

    private void OnEntryUnfocused(object sender, FocusEventArgs e) => AddButton.Focus();

    private void OnItemAdded(object? sender, EventArgs e) => EntryField.Focus();

    private async void OnTapGridArea(object sender, EventArgs e) => await CloseMenu();

    private void CopyOnClicked(object? sender, EventArgs e) => _viewModel.CopyToClipboard();

    private void ImportOnClicked(object? sender, EventArgs e) => _viewModel.InsertFromClipboard();

    private async void OnTapSettings(object sender, EventArgs e)
    {
        if (!_isMenuOpen)
        {
            var resize = PageContentGrid.TranslateTo(-Width, 0, AnimationDuration);
            await resize;

            _isMenuOpen = true;
            return;
        }

        await CloseMenu();
    }

    private async Task CloseMenu()
    {
        await PageContentGrid.RotateYTo(0, AnimationDuration / 2);
        var fadeIn = PageContentGrid.FadeTo(1, AnimationDuration / 2);
        var scaleBack = PageContentGrid.ScaleTo(1, AnimationDuration / 2);
        var resize = PageContentGrid.TranslateTo(0, 0, AnimationDuration / 2);
        await Task.WhenAll(fadeIn, scaleBack, resize);
        _isMenuOpen = false;
    }

    private void SwipeItemView_OnInvoked(object? sender, EventArgs e)
    {
        // TODO: Give user feedback through particles or animation
        Logger.Log("OnInvokedSwipeItem");
    }
}
