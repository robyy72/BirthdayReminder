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

        App.FlyoutOpenRequested += OnFlyoutOpenRequested;
        App.FlyoutCloseRequested += OnFlyoutCloseRequested;
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
            FlyoutPanel.TranslateTo(-250, 0, AnimationDuration, Easing.CubicIn),
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
        await Close();
    }

    private async void OnOverviewTapped(object? sender, TappedEventArgs e)
    {
        await Close();
        await App.NavigateToRootAsync();
    }

    private async void OnAllBirthdaysTapped(object? sender, TappedEventArgs e)
    {
        await Close();
        await App.NavigateToRootAsync();
        await App.NavigateToAsync<AllBirthdaysPage>();
    }

    private async void OnAccountTapped(object? sender, TappedEventArgs e)
    {
        await Close();
        await App.NavigateToRootAsync();
        await App.NavigateToAsync<AccountPage>();
    }

    private async void OnSettingsTapped(object? sender, TappedEventArgs e)
    {
        await Close();
        await App.NavigateToRootAsync();
        await App.NavigateToAsync<SettingsPage>();
    }

    private async void OnRemindersTapped(object? sender, TappedEventArgs e)
    {
        await Close();
        await App.NavigateToRootAsync();
        await App.NavigateToAsync<ReminderStandardSettings>();
    }

    private async void OnPrivacyTapped(object? sender, TappedEventArgs e)
    {
        await Close();
        await App.NavigateToRootAsync();
        await App.NavigateToAsync<PrivacyPage>();
    }

    private async void OnTicketsTapped(object? sender, TappedEventArgs e)
    {
        await Close();
        await App.NavigateToRootAsync();
        await App.NavigateToAsync<TicketListPage>(SupportType.Feedback);
    }
    #endregion
}
