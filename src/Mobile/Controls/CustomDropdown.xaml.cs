namespace Mobile;

/// <summary>
/// Aim: Custom dropdown control that shows options inline instead of a dialog.
/// </summary>
public partial class CustomDropdown : ContentView
{
    #region Bindable Properties
    public static readonly BindableProperty ItemsSourceProperty = BindableProperty.Create(
        nameof(ItemsSource),
        typeof(IList<string>),
        typeof(CustomDropdown),
        null,
        propertyChanged: OnItemsSourceChanged);

    public static readonly BindableProperty SelectedIndexProperty = BindableProperty.Create(
        nameof(SelectedIndex),
        typeof(int),
        typeof(CustomDropdown),
        -1,
        BindingMode.TwoWay,
        propertyChanged: OnSelectedIndexChanged);

    public static readonly BindableProperty PlaceholderProperty = BindableProperty.Create(
        nameof(Placeholder),
        typeof(string),
        typeof(CustomDropdown),
        "Select...");
    #endregion

    #region Properties
    public IList<string>? ItemsSource
    {
        get => (IList<string>?)GetValue(ItemsSourceProperty);
        set => SetValue(ItemsSourceProperty, value);
    }

    public int SelectedIndex
    {
        get => (int)GetValue(SelectedIndexProperty);
        set => SetValue(SelectedIndexProperty, value);
    }

    public string Placeholder
    {
        get => (string)GetValue(PlaceholderProperty);
        set => SetValue(PlaceholderProperty, value);
    }

    public string? SelectedItem => SelectedIndex >= 0 && ItemsSource != null && SelectedIndex < ItemsSource.Count
        ? ItemsSource[SelectedIndex]
        : null;
    #endregion

    #region Events
    public event EventHandler<EventArgs>? SelectedIndexChanged;
    #endregion

    #region Fields
    private bool _isOpen = false;
    #endregion

    #region Constructor
    public CustomDropdown()
    {
        InitializeComponent();
        UpdateSelectedText();
    }
    #endregion

    #region Property Changed Handlers
    private static void OnItemsSourceChanged(BindableObject bindable, object oldValue, object newValue)
    {
        if (bindable is CustomDropdown dropdown)
        {
            dropdown.BuildOptions();
            dropdown.UpdateSelectedText();
        }
    }

    private static void OnSelectedIndexChanged(BindableObject bindable, object oldValue, object newValue)
    {
        if (bindable is CustomDropdown dropdown)
        {
            dropdown.UpdateSelectedText();
            dropdown.SelectedIndexChanged?.Invoke(dropdown, EventArgs.Empty);
        }
    }
    #endregion

    #region Private Methods
    private void BuildOptions()
    {
        OptionsContainer.Children.Clear();

        if (ItemsSource == null)
            return;

        for (int i = 0; i < ItemsSource.Count; i++)
        {
            var index = i;
            var item = ItemsSource[i];

            var frame = new Frame
            {
                BackgroundColor = Colors.Transparent,
                BorderColor = Colors.Transparent,
                Padding = new Thickness(12, 10),
                HasShadow = false,
                CornerRadius = 0
            };

            var label = new Label
            {
                Text = item,
                FontSize = 16,
                TextColor = ResourceHelper.GetThemedColor("Black", "White"),
                VerticalOptions = LayoutOptions.Center
            };

            frame.Content = label;

            var tapGesture = new TapGestureRecognizer();
            tapGesture.Tapped += (s, e) => OnOptionSelected(index);
            frame.GestureRecognizers.Add(tapGesture);

            OptionsContainer.Children.Add(frame);

            // Add separator except for last item
            if (i < ItemsSource.Count - 1)
            {
                var separator = new BoxView
                {
                    HeightRequest = 1,
                    Color = ResourceHelper.GetThemedColor("Gray200", "Gray600"),
                    Margin = new Thickness(10, 0)
                };
                OptionsContainer.Children.Add(separator);
            }
        }
    }

    private void UpdateSelectedText()
    {
        if (SelectedIndex >= 0 && ItemsSource != null && SelectedIndex < ItemsSource.Count)
        {
            SelectedLabel.Text = ItemsSource[SelectedIndex];
        }
        else
        {
            SelectedLabel.Text = Placeholder;
        }
    }

    private void OnOptionSelected(int index)
    {
        SelectedIndex = index;
        CloseDropdown();
    }

    private void OpenDropdown()
    {
        _isOpen = true;
        DropdownFrame.IsVisible = true;
        ArrowLabel.Text = "▲";
    }

    private void CloseDropdown()
    {
        _isOpen = false;
        DropdownFrame.IsVisible = false;
        ArrowLabel.Text = "▼";
    }
    #endregion

    #region Event Handlers
    private void OnSelectionTapped(object? sender, TappedEventArgs e)
    {
        if (_isOpen)
        {
            CloseDropdown();
        }
        else
        {
            OpenDropdown();
        }
    }
    #endregion
}
