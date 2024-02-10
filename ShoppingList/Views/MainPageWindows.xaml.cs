using AsyncAwaitBestPractices;
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

    protected override void OnAppearing()
    {
        base.OnAppearing();
        DisplayPopUpOnFirstRun();
#if WINDOWS // To handle issue: https://github.com/dotnet/maui/issues/8573
        ThemeCollectionView.SelectedItem = _viewModel.CurrentTheme;
#endif
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
            var x = (Width - 250) / Width;
            var resize = PageContentGrid.ScaleTo(x, AnimationDuration);
            var move = PageContentGrid.TranslateTo(-Width * ((1 - x) / 2), 0, AnimationDuration);
            await Task.WhenAll(move, resize);

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
