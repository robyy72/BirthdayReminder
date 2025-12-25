#region Usings
using Microsoft.Maui.Controls;
#endregion

namespace Mobile;

/// <summary>
/// Aim: Slide-In Flyout Menü mit Animation
/// </summary>
public partial class FlyoutMenu : ContentView
{
    #region Constants
    private const uint AnimationDuration = 250;
    #endregion

    #region Constructor
    public FlyoutMenu()
    {
        InitializeComponent();
        VersionLabel.Text = $"Version {AppInfo.Current.VersionString} ({AppInfo.Current.BuildString})";

        NavigationService.FlyoutOpenRequested += OnFlyoutOpenRequested;
        NavigationService.FlyoutCloseRequested += OnFlyoutCloseRequested;
    }
    #endregion

    #region Public Methods
    /// <summary>
    /// Aim: Öffnet das Flyout mit Animation
    /// </summary>
    public async Task Open()
    {
        IsVisible = true;
        await Task.WhenAll(
            FlyoutPanel.TranslateTo(0, 0, AnimationDuration, Easing.CubicOut),
            Overlay.FadeTo(1, AnimationDuration)
        );
    }

    /// <summary>
    /// Aim: Schließt das Flyout mit Animation
    /// </summary>
    public async Task Close()
    {
        await Task.WhenAll(
            FlyoutPanel.TranslateTo(-280, 0, AnimationDuration, Easing.CubicIn),
            Overlay.FadeTo(0, AnimationDuration)
        );
        IsVisible = false;
    }
    #endregion

    #region Event Handlers
    private async void OnFlyoutOpenRequested()
    {
        await Open();
    }

    private async void OnFlyoutCloseRequested()
    {
        await Close();
    }

    private async void OnOverlayTapped(object? sender, TappedEventArgs e)
    {
        NavigationService.CloseFlyout();
    }

    private async void OnOverviewTapped(object? sender, TappedEventArgs e)
    {
        NavigationService.CloseFlyout();
        await NavigationService.NavigateToRoot();
    }

    private async void OnSettingsTapped(object? sender, TappedEventArgs e)
    {
        NavigationService.CloseFlyout();
        await NavigationService.NavigateToRoot();
        await NavigationService.NavigateTo<AccountPage>();
    }

    private async void OnPrivacyTapped(object? sender, TappedEventArgs e)
    {
        NavigationService.CloseFlyout();
        await NavigationService.NavigateToRoot();
        await NavigationService.NavigateTo<PrivacyPage>();
    }
    #endregion
}
