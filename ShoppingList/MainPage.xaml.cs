using ShoppingList.Models;
using ShoppingList.ViewModel;

namespace ShoppingList;

public partial class MainPage
{
    private const uint AnimationDuration = 400u;
    private bool _isMenuOpen;

    public MainPage(MainViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }

    protected override async void OnNavigatedTo(NavigatedToEventArgs args)
    {
        base.OnNavigatedTo(args);
        var mvm = (MainViewModel)BindingContext;
        var loadItems = mvm.LoadItemsFromDatabase();
        var loadStores = mvm.LoadStoresFromDatabase();
        await Task.WhenAll(loadItems, loadStores);
        mvm.SortItems();
        
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
        ((MainViewModel)BindingContext).CopyToClipboard();
    }
    
    private void OnTapManageStores(object? sender, EventArgs e)
    {
        var db = ((MainViewModel)BindingContext).Database;
        // Navigation.PushModalAsync(typeof(StoresPage));
        Shell.Current.GoToAsync(nameof(StoresPage)); 
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
        LogHandler.Log("OnInvokedSwipeItem");
    }
}