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
/// Aim: Slide-In Context Menu von rechts (wie Flyout, aber schmaler und ohne Header)
/// </summary>
public partial class ContextMenu : ContentView
{
    #region Constants
    private const uint AnimationDuration = 200;
    #endregion

    #region Properties
    public ObservableCollection<ContextMenuItem> MenuItems { get; } = [];
    #endregion

    #region Constructor
    public ContextMenu()
    {
        InitializeComponent();

        App.ContextMenuOpenRequested += OnContextMenuOpenRequested;
        App.ContextMenuCloseRequested += OnContextMenuCloseRequested;
    }
    #endregion

    #region Public Methods
    /// <summary>
    /// Aim: Öffnet das Context Menu mit Unfold-Animation von rechts
    /// </summary>
    public async Task Open()
    {
        if (MenuItems.Count == 0)
            return;

        IsVisible = true;
        await Task.WhenAll(
            ContextMenuPanel.ScaleXTo(1, AnimationDuration, Easing.CubicOut),
            Overlay.FadeTo(1, AnimationDuration)
        );
    }

    /// <summary>
    /// Aim: Schließt das Context Menu mit Animation
    /// </summary>
    public async Task Close()
    {
        await Task.WhenAll(
            ContextMenuPanel.ScaleXTo(0, AnimationDuration, Easing.CubicIn),
            Overlay.FadeTo(0, AnimationDuration)
        );
        IsVisible = false;
    }

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
        MenuItemsContainer.Children.Clear();
    }
    #endregion

    #region Private Methods
    private void BuildMenuItems()
    {
        MenuItemsContainer.Children.Clear();

        foreach (var item in MenuItems)
        {
            var border = new Border
            {
                Padding = new Thickness(15, 12),
                BackgroundColor = Colors.Transparent,
                StrokeThickness = 0
            };

            var label = new Label
            {
                Text = item.Text,
                FontSize = 16,
                FontAttributes = FontAttributes.Bold,
                TextColor = Application.Current?.RequestedTheme == AppTheme.Dark
                    ? Colors.White
                    : Colors.Black,
                VerticalOptions = LayoutOptions.Center
            };

            border.Content = label;

            var tapGesture = new TapGestureRecognizer();
            tapGesture.Tapped += async (s, e) =>
            {
                await Close();
                item.ClickAction?.Invoke();
            };
            border.GestureRecognizers.Add(tapGesture);

            MenuItemsContainer.Children.Add(border);
        }
    }
    #endregion

    #region Event Handlers
    private async void OnOverlayTapped(object? sender, TappedEventArgs e)
    {
        await Close();
    }

    private async void OnContextMenuOpenRequested()
    {
        await Open();
    }

    private async void OnContextMenuCloseRequested()
    {
        await Close();
    }
    #endregion
}
