using ShoppingList.Services;
using ShoppingList.ViewModel;

namespace ShoppingList.Views;

public partial class MainPage
{
    private const uint AnimationDuration = 400u;
    private bool _isMenuOpen;
    private readonly MainViewModel _viewModel;

    public MainPage(MainViewModel viewModel)
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
    }

    private void OnEntryUnfocused(object sender, FocusEventArgs e)
    {
        AddButton.Focus();
    }

    private void CopyOnClicked(object? sender, EventArgs e)
    {
        ((MainViewModel)BindingContext).CopyToClipboard();
    }

    private async void OnTapSettings(object sender, EventArgs e)
    {
        if (!_isMenuOpen)
        {
            var resize = PageContentGrid.TranslateTo(-Width * 0.25, 0, AnimationDuration);
            var scaleDown = PageContentGrid.ScaleTo(0.75, AnimationDuration);
            var fadeOut = PageContentGrid.FadeTo(0.8, AnimationDuration);
            var rotate = PageContentGrid.RotateYTo(35, AnimationDuration, Easing.CubicIn);
            await Task.WhenAll(resize, scaleDown, fadeOut, rotate);
            _isMenuOpen = true;
            return;
        }

        await CloseMenu();
    }

    private async void OnTapGridArea(object sender, EventArgs e)
    {
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
        Logger.Log("OnInvokedSwipeItem");
    }
}