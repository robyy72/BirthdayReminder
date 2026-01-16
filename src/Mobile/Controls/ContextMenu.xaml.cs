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
/// Aim: Slide-In Context Menu von rechts mit Header und TileFlyout-Style
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
            ContextMenuPanel.ScaleTo(1, AnimationDuration, Easing.CubicOut),
            Overlay.FadeTo(0.4, AnimationDuration) // Semi-transparent overlay
        );
    }

    /// <summary>
    /// Aim: Schließt das Context Menu mit Animation
    /// </summary>
    public async Task Close()
    {
        await Task.WhenAll(
            ContextMenuPanel.ScaleTo(0, AnimationDuration, Easing.CubicIn),
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
            // Using Frame instead of Border for Android compatibility
            var frame = new Frame
            {
                BackgroundColor = ResourceHelper.GetThemedColor("White", "Gray500"),
                BorderColor = Colors.Transparent,
                CornerRadius = 15,
                Padding = new Thickness(12, 10),
                Margin = new Thickness(5, 3),
                HasShadow = true
            };

            var label = new Label
            {
                Text = item.Text,
                TextColor = ResourceHelper.GetThemedColor("Black", "White"),
                FontSize = 16,
                FontAttributes = FontAttributes.Bold,
                VerticalOptions = LayoutOptions.Center
            };

            frame.Content = label;

            var tapGesture = new TapGestureRecognizer();
            tapGesture.Tapped += async (s, e) =>
            {
                await Close();
                item.ClickAction?.Invoke();
            };
            frame.GestureRecognizers.Add(tapGesture);

            MenuItemsContainer.Children.Add(frame);
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
