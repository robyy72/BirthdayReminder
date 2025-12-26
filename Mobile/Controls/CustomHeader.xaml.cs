namespace Mobile;

/// <summary>
/// Aim: Wiederverwendbarer Header mit Hamburger-Menü oder Back-Button
/// </summary>
public partial class CustomHeader : ContentView
{
    #region Bindable Properties
    public static readonly BindableProperty TitleProperty = BindableProperty.Create(
        nameof(Title),
        typeof(string),
        typeof(CustomHeader),
        string.Empty,
        propertyChanged: OnTitleChanged);

    public static readonly BindableProperty ShowMenuButtonProperty = BindableProperty.Create(
        nameof(ShowMenuButton),
        typeof(bool),
        typeof(CustomHeader),
        true,
        propertyChanged: OnButtonVisibilityChanged);

    public static readonly BindableProperty ShowBackButtonProperty = BindableProperty.Create(
        nameof(ShowBackButton),
        typeof(bool),
        typeof(CustomHeader),
        false,
        propertyChanged: OnButtonVisibilityChanged);
    #endregion

    #region Properties
    public string Title
    {
        get => (string)GetValue(TitleProperty);
        set => SetValue(TitleProperty, value);
    }

    public bool ShowMenuButton
    {
        get => (bool)GetValue(ShowMenuButtonProperty);
        set => SetValue(ShowMenuButtonProperty, value);
    }

    public bool ShowBackButton
    {
        get => (bool)GetValue(ShowBackButtonProperty);
        set => SetValue(ShowBackButtonProperty, value);
    }
    #endregion

    #region Constructor
    public CustomHeader()
    {
        InitializeComponent();
        UpdateButtonVisibility();
    }
    #endregion

    #region Property Changed Handlers
    private static void OnTitleChanged(BindableObject bindable, object oldValue, object newValue)
    {
        if (bindable is CustomHeader header)
        {
            header.TitleLabel.Text = (string)newValue;
        }
    }

    private static void OnButtonVisibilityChanged(BindableObject bindable, object oldValue, object newValue)
    {
        if (bindable is CustomHeader header)
        {
            header.UpdateButtonVisibility();
        }
    }
    #endregion

    #region Private Methods
    private void UpdateButtonVisibility()
    {
        MenuButton.IsVisible = ShowMenuButton && !ShowBackButton;
        BackButton.IsVisible = ShowBackButton;
    }
    #endregion

    #region Event Handlers
    private void OnMenuClicked(object? sender, EventArgs e)
    {
        App.OpenFlyout();
    }

    private async void OnBackClicked(object? sender, EventArgs e)
    {
        await App.GoBackAsync();
    }
    #endregion
}
