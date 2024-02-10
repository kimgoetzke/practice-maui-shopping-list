using AsyncAwaitBestPractices;
using CommunityToolkit.Maui.Views;
using ShoppingList.Utilities;
using ShoppingList.ViewModel;

namespace ShoppingList.Views;

public partial class MainPageAndroid
{
    private const uint AnimationDuration = 400u;
    private bool _isMenuOpen;
    private readonly MainViewModel _viewModel;

    public MainPageAndroid(MainViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
        _viewModel = viewModel;
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        _viewModel
            .LoadStoresFromDatabase()
            .SafeFireAndForget<Exception>(ex =>
            {
                Logger.Log($"Failed to load stores: {ex}");
                Notifier.ShowToast("Failed to load stores");
            });
        _viewModel
            .LoadItemsFromDatabase()
            .SafeFireAndForget<Exception>(ex =>
            {
                Logger.Log($"Failed to load items: {ex}");
                Notifier.ShowToast("Failed to load items");
            });
        DisplayPopUpOnFirstRun();
    }

    private void DisplayPopUpOnFirstRun()
    {
        if (!Settings.FirstRun)
            return;
        Logger.Log("First time running this application");
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
            if (Settings.CurrentTheme == Settings.Theme.Dark)
            {
                await DarkModeTransitionToSettings();
            }
            else
            {
                await LightModeTransitionToSettings();
            }

            _isMenuOpen = true;
            return;
        }

        await CloseMenu();
    }

    private async Task LightModeTransitionToSettings()
    {
        var resize = PageContentGrid.TranslateTo(-Width * 0.25, 0, AnimationDuration);
        var scaleDown = PageContentGrid.ScaleTo(0.75, AnimationDuration);
        var fadeOut = PageContentGrid.FadeTo(0.8, AnimationDuration);
        var rotate = PageContentGrid.RotateYTo(35, AnimationDuration, Easing.CubicIn);
        await Task.WhenAll(resize, scaleDown, fadeOut, rotate);
    }

    private async Task DarkModeTransitionToSettings()
    {
        var resize = PageContentGrid.TranslateTo(-Width * 0.25, 0, AnimationDuration);
        var scaleDown = PageContentGrid.ScaleTo(0.75, AnimationDuration);
        var rotate = PageContentGrid.RotateYTo(35, AnimationDuration, Easing.CubicIn);
        await Task.WhenAll(resize, scaleDown, rotate);
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
