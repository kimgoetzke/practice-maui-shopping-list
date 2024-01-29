﻿using CommunityToolkit.Maui.Views;
using CommunityToolkit.Mvvm.Messaging;
using CommunityToolkit.Mvvm.Messaging.Messages;
using ShoppingList.Resources.Styles;
using ShoppingList.Utilities;
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

    private void OnPickerSelectionChanged(object sender, EventArgs e)
    {
        var picker = sender as Picker;
        var themeString = picker!.SelectedItem!.ToString();
        var isParsed = Enum.TryParse(themeString, out Settings.Theme theme);
        if (!isParsed)
        {
            Logger.Log($"Failed to parse {themeString} to {nameof(Settings.Theme)}");
            return;
        }
        Settings.LoadTheme(theme);
    }
}
