#region Usings
using System.Collections.ObjectModel;
#endregion

namespace Mobile;

/// <summary>
/// Aim: Represents a context menu item with text and click action
/// </summary>
public class ContextMenuItem
{
    public string Text { get; set; } = string.Empty;
    public Action? ClickAction { get; set; }
}

/// <summary>
/// Aim: Wiederverwendbarer Header mit Hamburger-Menü, Back-Button und Context-Menü
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

    public static readonly BindableProperty ShowContextMenuProperty = BindableProperty.Create(
        nameof(ShowContextMenu),
        typeof(bool),
        typeof(CustomHeader),
        false,
        propertyChanged: OnContextMenuVisibilityChanged);
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

    public bool ShowContextMenu
    {
        get => (bool)GetValue(ShowContextMenuProperty);
        set => SetValue(ShowContextMenuProperty, value);
    }

    public ObservableCollection<ContextMenuItem> MenuItems { get; } = [];
    #endregion

    #region Constructor
    public CustomHeader()
    {
        InitializeComponent();
        UpdateButtonVisibility();

        App.ContextMenuOpenRequested += OnContextMenuOpenRequested;
        App.ContextMenuCloseRequested += OnContextMenuCloseRequested;
    }
    #endregion

    #region Public Methods
    /// <summary>
    /// Aim: Adds a menu item to the context menu
    /// Params: text - Display text, clickAction - Action to execute on click
    /// </summary>
    public void AddMenuItem(string text, Action clickAction)
    {
        var item = new ContextMenuItem { Text = text, ClickAction = clickAction };
        MenuItems.Add(item);
        BuildMenuItems();
    }

    /// <summary>
    /// Aim: Clears all menu items
    /// </summary>
    public void ClearMenuItems()
    {
        MenuItems.Clear();
        ContextMenuItems.Children.Clear();
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

    private static void OnContextMenuVisibilityChanged(BindableObject bindable, object oldValue, object newValue)
    {
        if (bindable is CustomHeader header)
        {
            header.ContextMenuButton.IsVisible = (bool)newValue;
        }
    }
    #endregion

    #region Private Methods
    private void UpdateButtonVisibility()
    {
        MenuButton.IsVisible = ShowMenuButton && !ShowBackButton;
        BackButton.IsVisible = ShowBackButton;
    }

    private void BuildMenuItems()
    {
        ContextMenuItems.Children.Clear();

        foreach (var item in MenuItems)
        {
            var label = new Label
            {
                Text = item.Text,
                Padding = new Thickness(15, 10),
                FontSize = 16,
                TextColor = Application.Current?.RequestedTheme == AppTheme.Dark
                    ? Colors.White
                    : Colors.Black
            };

            var tapGesture = new TapGestureRecognizer();
            tapGesture.Tapped += (s, e) =>
            {
                CloseContextMenu();
                item.ClickAction?.Invoke();
            };
            label.GestureRecognizers.Add(tapGesture);

            ContextMenuItems.Children.Add(label);
        }
    }

    private void OpenContextMenu()
    {
        if (MenuItems.Count == 0)
            return;

        ContextMenuDropdown.IsVisible = true;
    }

    private void CloseContextMenu()
    {
        ContextMenuDropdown.IsVisible = false;
    }

    private void ToggleContextMenu()
    {
        if (ContextMenuDropdown.IsVisible)
        {
            CloseContextMenu();
        }
        else
        {
            OpenContextMenu();
        }
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

    private void OnContextMenuButtonClicked(object? sender, EventArgs e)
    {
        ToggleContextMenu();
    }

    private void OnContextMenuOpenRequested()
    {
        OpenContextMenu();
    }

    private void OnContextMenuCloseRequested()
    {
        CloseContextMenu();
    }
    #endregion
}
